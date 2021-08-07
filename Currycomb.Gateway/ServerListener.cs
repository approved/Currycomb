using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Currycomb.Gateway.ClientData;
using Serilog;
using Currycomb.Gateway.Network;

namespace Currycomb.Gateway
{
    public class ServerListener
    {
        internal static async Task StartListener(TcpListener listener, IncomingPacketDispatcher incomingPacketDispatcher)
        {
            List<Task> clientTaskList = new();
            listener.Start();
            try
            {
                Task<TcpClient>? newClientTask = null;
                Task<Task>? dropClientTask = null;
                while (true)
                {
                    // Accept the Client
                    newClientTask ??= listener.AcceptTcpClientAsync();
                    dropClientTask ??= Task.WhenAny(clientTaskList);

                    Log.Information("Awating client connection");
                    Task completed = await Task.WhenAny(dropClientTask, newClientTask);
                    if (completed == newClientTask)
                    {
                        /*
                         * TODO: Comment
                         */
                        Log.Information("Handlding incoming client");
                        TcpClient client = await newClientTask;
                        ClientConnection clientCon = new(client.GetStream());
                        clientTaskList.Add(clientCon.RunAsync(incomingPacketDispatcher));

                        newClientTask = null;
                    }
                    else if (completed == dropClientTask)
                    {
                        /*
                         * TODO: Comment
                         */
                        clientTaskList.Remove(await dropClientTask);
                        dropClientTask = null;
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
