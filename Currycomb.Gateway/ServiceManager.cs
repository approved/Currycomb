using System.Threading.Tasks;
using Serilog;
using Currycomb.Common.Network;
using System.Collections.Generic;
using System.Threading.Channels;
using Currycomb.Common.Network.Broadcast;
using System.Threading;
using System.Runtime.CompilerServices;
using System;

namespace Currycomb.Gateway
{
    public class ServiceManager<TService> : IService
        where TService : IService
    {
        record ActiveService(TService Service, TaskCompletionSource Invalidated, CancellationTokenSource Cts, Task RunTask);

        private static ILogger log = Log.ForContext<ServiceManager<TService>>()
                                        .ForContext("service", typeof(TService).Name);

        object _activeServiceSwapLock = new();
        Channel<TService> _serviceQueue = Channel.CreateUnbounded<TService>();
        ActiveService? _activeService;

        public async ValueTask AddServiceInstance(TService service)
            => await _serviceQueue.Writer.WriteAsync(service);

        public async ValueTask HandleAsync(WrappedPacket packet)
        {
            // TODO: Handle failure during routing.
            ActiveService act = await GetActiveService();

            try
            {
                await act.Service.HandleAsync(packet);
            }
            catch (Exception e)
            {
                log.Error(e, "Failed to handle packet, yeeting {@type} service and retrying.", typeof(TService));

                InvalidateActiveService();
                await HandleAsync(packet);
            }
        }

        public async ValueTask HandleAsync(ComEvent evt)
        {
            // TODO: Handle failure during routing.
            ActiveService act = await GetActiveService();

            try
            {
                await act.Service.HandleAsync(evt);
            }
            catch (Exception e)
            {
                log.Error(e, "Failed to handle packet, yeeting {@type} service and retrying.", typeof(TService));

                InvalidateActiveService();
                await HandleAsync(evt);
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
            }

            log.Warning("No active {@service} found, awaiting a new instance.");
            TService service = await _serviceQueue.Reader.ReadAsync(ct);

            lock (_activeServiceSwapLock)
            {
                if (_activeService == null)
                {
                    log.Information("New active {@service} found and swapped in.");

                    CancellationTokenSource cts = new();
                    return _activeService = new(service, new(), cts, service.RunAsync(cts.Token));
                }
            }

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
                _activeService.Invalidated.SetResult();
                _activeService.Cts.Cancel();

                _activeService = null;
            }
        }

        public async IAsyncEnumerable<WrappedPacketContainer> ReadPacketsAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            log.Information("Started Reading Packets from {service}");
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    ActiveService service = await GetActiveService(ct);

                    // Read all incoming packets, if any exist
                    await foreach (WrappedPacketContainer wpc in service.Service.ReadPacketsAsync(ct))
                    {
                        log.Information("Received packet from {@service}", service.Service.GetType());
                        yield return wpc;
                    }

                    log.Information("No more packets in queue from {@service}", service.Service.GetType());

                    if (ct.IsCancellationRequested)
                        break;

                    // If we run out of packets we don't want to retry until the service is invalidated
                    await service.Invalidated.Task;
                }
            }
            finally
            {
                log.Error("ReadPacketsAsync Ended");
            }
        }
    }
}
