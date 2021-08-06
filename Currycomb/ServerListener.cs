using Currycomb.Packets;
using Serilog;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Currycomb
{
    public class ServerListener
    {
        // TODO: Move to configuration file
        // Open a listener on port 25565 (default MC port)
        public static TcpListener Listener = new(IPAddress.Any, 25565);

        [STAThread]
        internal static void StartListener()
        {
            Listener.Start();
            try
            {
                while (true)
                {
                    // Accept the Client
                    TcpClient? client = Listener.AcceptTcpClient();
                    if (client is not null && client.Connected)
                    {
                        Log.Information($"{client.Client.RemoteEndPoint} is attempting to connect...");

                        // Send the client stream to be processed
                        try
                        {
                            PacketHandler.ProcessRemoteData(client);
                        }
                        catch (Exception ex)
                        {
                            client.Close();
                            throw;
                        }
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
                Listener.Stop();
            }
        }
    }
}
