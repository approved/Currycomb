using System.Threading.Tasks;
using Serilog;
using Currycomb.Common.Network;
using System.Threading.Channels;
using Currycomb.Common.Network.Broadcast;
using System.Threading;
using System;

namespace Currycomb.Gateway
{
    public class ServiceManager<TService> : IService
        where TService : IService
    {
        record ActiveService(Guid Id, TService Service, TaskCompletionSource Invalidated, CancellationTokenSource Cts, Task RunTask);

        private static readonly ILogger log = Log
            .ForContext<ServiceManager<TService>>()
            .ForContext("service", typeof(TService).Name);

        readonly object _activeServiceSwapLock = new();
        readonly Channel<TService> _serviceQueue = Channel.CreateUnbounded<TService>();

        ActiveService? _activeService;
        Task<TService>? _activeServiceQueued;

        public ValueTask AddServiceInstance(TService service)
            => _serviceQueue.Writer.WriteAsync(service);

        public ValueTask HandleAsync(WrappedPacket packet)
            => WithService(x => x.Service.HandleAsync(packet));

        public ValueTask HandleAsync(ComEvent evt)
            => WithService(x => x.Service.HandleAsync(evt));

        private async ValueTask WithService(Func<ActiveService, ValueTask> func, CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                ActiveService act = await GetActiveService(ct);

                if (ct.IsCancellationRequested)
                    return;

                try
                {
                    await func(act);
                    return;
                }
                catch (Exception e)
                {
                    log.Error(e, "Failed to handle packet, yeeting {@type} instance [{@id}] and retrying.", typeof(TService), act.Id);
                    InvalidateActiveService();
                }
            }
        }

        private async Task<ActiveService> GetActiveService(CancellationToken ct = default)
        {
            lock (_activeServiceSwapLock)
            {
                if (_activeService != null)
                {
                    log.Information("{@service} {0}", _activeService.RunTask.Status);
                    return _activeService;
                }

                _activeServiceQueued ??= _serviceQueue.Reader.ReadAsync(ct).AsTask();
            }

            log.Warning("No active {@service} found, awaiting a new instance.");
            TService service = await _activeServiceQueued;

            lock (_activeServiceSwapLock)
            {
                if (_activeService == null)
                {
                    log.Information("New active {@service} found and swapped in.");

                    CancellationTokenSource cts = new();
                    return _activeService = new(Guid.NewGuid(), service, new(), cts, service.RunAsync(cts.Token));
                }
            }

            log.Information("{@service} was already swapped in.");

            await _serviceQueue.Writer.WriteAsync(service, ct);
            return await GetActiveService(ct);
        }

        private void InvalidateActiveService()
        {
            lock (_activeServiceSwapLock)
            {
                if (_activeService == null)
                    return;

                log.Warning("Invalidating {@service}", _activeService.Service);
                _activeServiceQueued = null;

                _activeService.Invalidated.SetResult();
                _activeService.Cts.Cancel();

                _activeService = null;
            }
        }

        public async Task ReadPacketsToChannelAsync(ChannelWriter<WrappedPacketContainer> channel, CancellationToken ct = default)
        {
            log.Information("Started Reading Packets from {service}");
            ActiveService service = await GetActiveService(ct);

            // Read all incoming packets, if any exist
            var mixedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, service.Cts.Token);
            try
            {
                log.Debug("Reading from service {@service} to channel");
                await service.Service.ReadPacketsToChannelAsync(channel, mixedCts.Token);
            }
            catch (OperationCanceledException) { /* this just means we're done */ }

            log.Debug("Finished reading from service {@service}");

            if (ct.IsCancellationRequested)
                return;

            log.Debug("Waiting for {@service} to be invalidated");
            // If we run out of packets we don't want to retry until the service is invalidated
            await service.Invalidated.Task;

            log.Debug("{@service} was invalidated");
        }
    }
}
