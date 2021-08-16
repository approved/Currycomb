using System;
using Serilog;
using System.Collections.Concurrent;
using Currycomb.Common.Network.Game;
using System.Threading.Channels;
using Currycomb.Common.Network;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Currycomb.Common;
using System.Linq;

namespace Currycomb.Gateway.Clients
{
    public class ClientCollection
    {
        private static readonly ILogger log = Log.ForContext<ClientCollection>();

        ConcurrentDictionary<Guid, ClientConnection> _clientDict = new();
        ConcurrentDictionary<Guid, Task> _taskDict = new();

        public ClientConnection? GetClientById(Guid id) => _clientDict.TryGetValue(id, out var client) ? client : null;
        public State? GetClientState(Guid id) => GetClientById(id)?.State;

        public void SetClientState(Guid id, State state)
        {
            log.Information("Setting state for {@id} to {@state}", id, state);

            if (_clientDict.TryGetValue(id, out var client))
                client.SetState(state);

            // If the client doesn't exist, ignore the request
        }

        public void AddClient(ClientConnection client)
        {
            log.Information("Adding client {@id}", client.Id);

            if (!_clientDict.TryAdd(client.Id, client))
            {
                // This should never happen, exception being if we generate the same GUID twice, which again should never happen.
                log.Error("Could not add client connection to dictionary, client already exists.");
                client.Dispose();
            }
            else
            {
                _clientDict[client.Id] = client;
                _newClient.Writer.TryWrite(client);
            }
        }

        private void RemoveClient(Guid id)
        {
            log.Information("Removing client {@id}", id);

            if (!_clientDict.TryRemove(id, out var client))
                log.Error("Could not remove client connection from dictionary, client does not exist.");
        }

        private Channel<ClientConnection> _newClient = Channel.CreateUnbounded<ClientConnection>();

        public async Task SendPacketAsync(Guid client, WrappedPacket packet)
        {
            if (client == Constants.ClientId.BroadcastGuid)
                await Task.WhenAll(_clientDict.Values.Select(x => x.SendPacketAsync(packet.Data)));
            else if (_clientDict.TryGetValue(client, out var conn))
                await conn.SendPacketAsync(packet.Data);

            Log.Information("Attempted to send packet to unknown client: {@client}", client);
        }

        public async Task ReadPacketsToChannel(ChannelWriter<(State, WrappedPacket)> writer, CancellationToken ct = default)
        {
            List<Task<Guid>> tasks = new() { new TaskCompletionSource<Guid>().Task };
            Task<Task<Guid>>? handleOldClients;

            Task? handleNewClients = Task.Run(async () =>
            {
                await foreach (var client in _newClient.Reader.ReadAllAsync(ct))
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        await client.ReadPacketsToChannelAsync(writer, ct);
                        return client.Id;
                    }));
                }
            }, ct);

            while (!ct.IsCancellationRequested)
            {
                // This does not wait for all clients' tasks, just ones that existed the last time a client left.
                // TODO: Consider making this wait for all clients' tasks.
                handleOldClients = Task.WhenAny(tasks);

                if (await Task.WhenAny(handleNewClients, handleOldClients) == handleNewClients)
                    break;

                Task<Guid> clientTask = await handleOldClients;
                Guid clientId = await clientTask;

                log.Information("Client disconnected, removing task.");
                tasks.Remove(clientTask);
                RemoveClient(clientId);
            }

            await Task.WhenAll(tasks);
        }
    }
}
