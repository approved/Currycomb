using System.Threading.Tasks;
using Currycomb.Common.Network;
using System.Threading.Channels;
using System.Threading;
using Serilog;
using System;
using System.Collections.Concurrent;
using Currycomb.Gateway.Network.Services;
using System.Collections.Generic;

namespace Currycomb.Gateway
{
    public class ServiceCollection
    {
        private static readonly ILogger log = Log.ForContext<ServiceCollection>();

        object _servicesLock = new();
        ConcurrentDictionary<Guid, ServiceManager> _services = new();
        Channel<ServiceInstance> _newService = Channel.CreateUnbounded<ServiceInstance>();

        public ICollection<ServiceManager> Services => _services.Values;

        public async ValueTask Add(ServiceInstance service)
        {
            bool existed = false;
            ServiceManager manager = _services.AddOrUpdate(
                    service.ServiceId,
                    s => new ServiceManager(s),
                    (_, m) => { existed = true; return m; });

            await manager.AddServiceInstance(service);

            // Only "announce" a service if it's a new service.
            if (!existed)
                await _newService.Writer.WriteAsync(service);
        }

        public async Task ReadPacketsToChannel(ChannelWriter<WrappedPacketContainer> writer, CancellationToken ct = default)
        {
            List<Task<Guid>> tasks = new() { new TaskCompletionSource<Guid>().Task };
            Channel<Task> newTasks = Channel.CreateUnbounded<Task>(new()
            {
                SingleReader = true,
                SingleWriter = true,
            });

            Task? handleAddingItems = Task.Run(async () =>
            {
                await foreach (var svc in _newService.Reader.ReadAllAsync(ct))
                {
                    Task<Guid> task = Task.Run(async () =>
                    {
                        await svc.ReadPacketsToChannelAsync(writer, ct);
                        return svc.ServiceId;
                    });

                    tasks.Add(task);
                    await newTasks.Writer.WriteAsync(task);
                }
            }, ct);

            Task<Task>? handleNewTask = null;
            while (!ct.IsCancellationRequested)
            {
                handleNewTask ??= newTasks.Reader.ReadAsync(ct).AsTask();

                Task<Task<Guid>> handleRemItem = Task.WhenAny(tasks);
                Task finished = await Task.WhenAny(handleNewTask, handleAddingItems, handleRemItem);

                // Await it for good measure so that we raise any exceptions.
                await finished;

                // If the task for listening for new services exits, we're done.
                if (finished == handleAddingItems)
                    break;

                // Reset the task and let it recreate the other one.
                if (finished == handleNewTask)
                {
                    handleNewTask = null;
                    continue;
                }

                // If the task for removing items finishes we'll want to clean up the finished item by ID.
                if (finished == handleRemItem)
                {
                    Task<Guid> task = await handleRemItem;
                    Guid id = await task;

                    log.Information("Service disconnected, removing task.");
                    tasks.Remove(task);
                    _services.Remove(id, out _);
                }
            }

            await Task.WhenAll(tasks);
        }
    }
}
