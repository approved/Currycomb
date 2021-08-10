using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Currycomb.Gateway.ClientData;
using Serilog;
using Currycomb.Gateway.Network;
using Currycomb.Common.Network;
using Currycomb.Common.Network.Meta;
using System.IO;

namespace Currycomb.Gateway
{
    public class ServerListener
    {
        internal static async Task StartListener(TcpListener listener, PacketServiceRouter c2sPacketRouter, WrappedPacketStream authStream)
        {
            listener.Start();

            List<Task> clientTaskList = new()
            {
                // Task.WhenAny throws if it is given an empty list before .NET 6.0
                new TaskCompletionSource().Task
            };

            Dictionary<Guid, ClientConnection> clientDict = new();

            try
            {
                Task<TcpClient>? newClientTask = null;
                Task<Task>? dropClientTask = null;

                Task<WrappedPacketContainer>? authPacketTask = null;

                MetaPacketHandler mph = new();
                MetaPacketRouter<MetaContext> router = mph.Router;


                // TODO: Split connection management and packet management into two separate threads
                while (true)
                {
                    // Accept the Client
                    newClientTask ??= listener.AcceptTcpClientAsync();
                    authPacketTask ??= authStream.ReadAsync(true);

                    dropClientTask ??= Task.WhenAny(clientTaskList);


                    Log.Information("Awaiting a task.");
                    Task completed = await Task.WhenAny(
                        // Handle disconnecting clients
                        dropClientTask,
                        // Handle connecting clients
                        newClientTask,

                        // Handle auth-related packets (AuthService -> Client)
                        authPacketTask
                    );

                    if (completed == newClientTask)
                    {
                        Log.Information("Handling incoming client");
                        TcpClient client = await newClientTask;
                        ClientConnection clientCon = new(client.GetStream());

                        clientDict.Add(clientCon.Id, clientCon);
                        clientTaskList.Add(clientCon.RunAsync(c2sPacketRouter));

                        newClientTask = null;
                    }
                    else if (completed == dropClientTask)
                    {
                        Log.Information("Handling dropped client");
                        clientTaskList.Remove(await dropClientTask);

                        // TODO: Remove the client from the client dict
                        // clientDict.Remove(???.Id);

                        dropClientTask = null;
                    }
                    else if (completed == authPacketTask)
                    {
                        Log.Information("Received packet");
                        WrappedPacketContainer pktc = await authPacketTask;
                        WrappedPacket packet = pktc.Packet;

                        Log.Information("Received packet for " + packet.ClientId);

                        if (!clientDict.TryGetValue(packet.ClientId, out var client))
                        {
                            throw new InvalidOperationException($"Received packet with unknown ClientId from AuthService: {packet.ClientId}");
                        }

                        if (pktc.IsMetaPacket)
                        {
                            Log.Information("Handling meta packet related to {@clientId}.", packet.ClientId);
                            MetaContext context = new(client);

                            using MemoryStream ms = new MemoryStream(packet.GetOrCreatePacketByteArray(), false);
                            using BinaryReader br = new(ms);

                            await router.HandlePacketAsync(context, br);
                        }
                        else
                        {
                            Log.Information("Forwarding packet to {@clientId}.", packet.ClientId);
                            await client.SendPacketAsync(packet.Data);
                        }

                        authPacketTask = null;
                    }
                    else
                    {
                        throw new InvalidOperationException("Unknown task completed?!");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Fatal error occured.");
                throw;
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
