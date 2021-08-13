using System.Threading.Tasks;
using Serilog;
using Currycomb.Common.Network;
using System.Threading.Channels;
using System.Threading;
using System;

namespace Currycomb.Gateway
{
    public class ServiceManager<TService> : IService
        where TService : IService
    {
        record ActiveService(Guid Id, TService Service, TaskCompletionSource Invalidated, CancellationTokenSource Cts, Task RunTask);

        private static readonly ILogger _log = Log
            .ForContext<ServiceManager<TService>>()
            .ForContext("service", typeof(TService).Name);

        readonly object _activeServiceSwapLock = new();
        readonly Channel<TService> _serviceQueue = Channel.CreateUnbounded<TService>();

        ActiveService? _activeService;
        Task<TService>? _activeServiceQueued;

        public ValueTask AddServiceInstance(TService service)
            => _serviceQueue.Writer.WriteAsync(service);

        public ValueTask HandleAsync(bool requireDelivery, bool isMeta, WrappedPacket packet)
            => WithService(requireDelivery, x => x.Service.HandleAsync(isMeta, packet));

        private async ValueTask WithService(bool requireDelivery, Func<ActiveService, ValueTask> func, CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                // If we're not required to send it out and we don't have an instane of the service on hand, don't bother.
                if (!requireDelivery && _activeService == null && _serviceQueue.Reader.Count == 0)
                {
                    Log.Information("Dropping packet as no {@type} is connected and 'requireDelivery' is {requireDelivery}.", typeof(TService), requireDelivery);
                    return;
                }

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
                    _log.Error(e, "Failed to handle packet, yeeting {@type} instance [{@id}] and retrying.", typeof(TService), act.Id);
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
                    _log.ForContext("status", _activeService.RunTask.Status)
                        .Information("{@service} {@status}");

                    return _activeService;
                }

                _activeServiceQueued ??= _serviceQueue.Reader.ReadAsync(ct).AsTask();
            }

            _log.Warning("No active {@service} found, awaiting a new instance.");
            TService service = await _activeServiceQueued;

            lock (_activeServiceSwapLock)
            {
                if (_activeService == null)
                {
                    _log.Information("New active {@service} found and swapped in.");

                    CancellationTokenSource cts = new();
                    return _activeService = new(Guid.NewGuid(), service, new(), cts, service.RunAsync(cts.Token));
                }
            }

            _log.Information("{@service} was already swapped in.");

            await _serviceQueue.Writer.WriteAsync(service, ct);
            return await GetActiveService(ct);
        }

        private void InvalidateActiveService()
        {
            lock (_activeServiceSwapLock)
            {
                if (_activeService == null)
                    return;

                _log.Warning("Invalidating {@service}", _activeService.Service);
                _activeServiceQueued = null;

                _activeService.Invalidated.SetResult();
                _activeService.Cts.Cancel();

                _activeService = null;
            }
        }

        public async Task ReadPacketsToChannelAsync(ChannelWriter<WrappedPacketContainer> channel, CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                _log.Information("Started Reading Packets from {service}");
                ActiveService service = await GetActiveService(ct);

                // Read all incoming packets, if any exist
                var mixedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, service.Cts.Token);
                try
                {
                    _log.Debug("ServiceManager | Reading from service {@service} to channel");
                    await service.Service.ReadPacketsToChannelAsync(channel, mixedCts.Token);
                }
                catch (OperationCanceledException) { /* this just means we're done */ }

                _log.Debug("ServiceManager | Finished reading from service {@service}");

                if (ct.IsCancellationRequested)
                    return;

                _log.Debug("ServiceManager | Waiting for {@service} to be invalidated");
                // If we run out of packets we don't want to retry until the service is invalidated
                await service.Invalidated.Task;

                _log.Debug("ServiceManager | {@service} was invalidated");
            }
        }
    }
}
