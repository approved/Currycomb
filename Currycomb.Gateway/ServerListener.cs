using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Currycomb.Gateway.ClientData;
using Serilog;
using Currycomb.Gateway.Network;
using Currycomb.Common.Network;

namespace Currycomb.Gateway
{
    public class ServerListener
    {
        internal static async Task StartListener(TcpListener listener, IncomingPacketRouter c2sPacketRouter, WrappedPacketStream authStream)
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

                Task<WrappedPacket>? authPacketTask = null;

                // TODO: Split connection management and packet management into two separate threads
                while (true)
                {
                    // Accept the Client
                    newClientTask ??= listener.AcceptTcpClientAsync();
                    authPacketTask ??= authStream.ReadAutoAckAsync();

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
                        WrappedPacket packet = await authPacketTask;
                        Log.Information("Received packet for " + packet.ClientId);

                        if (!clientDict.TryGetValue(packet.ClientId, out var client))
                        {
                            throw new InvalidOperationException($"Received packet with unknown ClientId from AuthService: {packet.ClientId}");
                        }

                        Log.Information($"Forwarding packet to {packet.ClientId}.");
                        await client.SendPacketAsync(packet.Packet);

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
                Log.Error($"Fatal error occured: {Environment.NewLine}{ex.StackTrace}");
                throw;
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
