using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Currycomb.Common.Network;
using Currycomb.Common.Network.Game;
using Currycomb.Common.Network.Meta;
using Currycomb.Gateway.Network.Services;
using Serilog;

namespace Currycomb.Gateway
{
    // Responsible for managing one service (service ID, not service instance)
    public class ServiceManager : IService
    {
        public readonly Guid ServiceId;

        private static readonly ILogger _log = Log
            .ForContext<ServiceManager>()
            .ForContext("service", typeof(ServiceInstance).Name);

        readonly object _activeLock = new();
        readonly Channel<ServiceInstance> _pending = Channel.CreateUnbounded<ServiceInstance>();

        Active? _active;
        Task<ServiceInstance>? _activeQueued;

        public ServiceManager(Guid serviceId)
            => ServiceId = serviceId;

        public ServiceManager(Guid serviceId, ServiceInstance service) : this(serviceId)
            => AddServiceInstance(service);

        public string Name => GetActive()?.Service.Name ?? "Unknown.";

        public ValueTask AddServiceInstance(ServiceInstance service)
            => _pending.Writer.WriteAsync(service);

        public async ValueTask SendAsync(bool isMeta, WrappedPacket packet)
        {
            while (GetActive() is Active active)
            {
                try
                {
                    await active.Service.SendAsync(isMeta, packet);
                    return;
                }
                catch (Exception ex)
                {
                    _log.Error(ex, "Failed to send packet to service {ServiceId}.", ServiceId);
                    InvalidateActiveService(active.Id);
                }
            }
        }

        public async Task ReadPacketsToChannelAsync(ChannelWriter<WrappedPacketContainer> channel, CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                _log.Information("Started Reading Packets from {service}");
                Active service = await GetActiveAsync(ct);

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

        public bool Supports(MetaPacketId packetId) => GetActive()?.Service.Supports(packetId) ?? false;
        public bool Supports(GamePacketId packetId) => GetActive()?.Service.Supports(packetId) ?? false;

        private Active? GetActive()
        {
            lock (_activeLock)
                if (_active != null)
                    return _active;

            if (_pending.Reader.TryRead(out ServiceInstance? newService))
                return ReplaceActive(newService);

            return null;
        }

        private async Task<Active> GetActiveAsync(CancellationToken ct = default)
        {
            lock (_activeLock)
            {
                if (_active != null)
                    return _active;

                _activeQueued ??= _pending.Reader.ReadAsync(ct).AsTask();
            }

            _log.Warning("No active {@service} found, awaiting a new instance.");
            ServiceInstance service = await _activeQueued;

            if (ReplaceActive(service) is Active act)
                return act;

            _log.Information("{@service} was already swapped in.");
            await _pending.Writer.WriteAsync(service, ct);
            return await GetActiveAsync(ct);
        }

        private void InvalidateActiveService(Guid id)
        {
            lock (_activeLock)
            {
                if (_active == null || _active.Id != id)
                    return;

                _log.Warning("Invalidating {@service}", _active.Service);
                _activeQueued = null;

                _active.Invalidated.SetResult();
                _active.Cts.Cancel();

                _active = null;
            }
        }

        private Active? ReplaceActive(ServiceInstance service)
        {
            lock (_activeLock)
            {
                if (_active == null)
                {
                    CancellationTokenSource cts = new();
                    _active = new(Guid.NewGuid(), service, new(), cts, service.RunAsync(cts.Token));

                    return _active;
                }
            }

            return null;
        }
        record Active(Guid Id, ServiceInstance Service, TaskCompletionSource Invalidated, CancellationTokenSource Cts, Task RunTask);
    }
}
