using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using Serilog;

namespace Currycomb.BroadcastService
{
    public class Program
    {
        abstract record Event;

        record ConnectEvent(WebSocket WebSocket) : Event;
        record DisconnectEvent(WebSocket WebSocket) : Event;
        record BroadcastEvent(WebSocketMessageType Type, byte[] EventData) : Event;

        private static readonly string LogFileName = $"logs/broadcast_service/broadcast_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{Environment.ProcessId}.txt";

        public static async Task Main()
        {
            Log.Logger = new LoggerConfiguration()
                   .MinimumLevel.Verbose()
                   .WriteTo.Async(x => x.Console())
                   .WriteTo.Async(x => x.File(LogFileName))
                   .CreateLogger();

            List<Task> clientTaskList = new List<Task>
            {
                // Task.WhenAny throws if it is given an empty list before .NET 6.0
                new TaskCompletionSource().Task
            };

            HttpListener listener = new();
            listener.Prefixes.Add("http://127.0.0.1:10002/");
            listener.Start();

            Log.Information("Started listener");

            CancellationTokenSource cts = new();

            CancellationToken ct = cts.Token;

            Channel<Event> eventChannel = Channel.CreateUnbounded<Event>();

            Task eventHandler = Task.Run(async () =>
            {
                List<WebSocket> connected = new();

                while (!ct.IsCancellationRequested)
                {
                    Event ev = await eventChannel.Reader.ReadAsync(ct);
                    Log.Information($"Handling event: {ev}");

                    switch (ev)
                    {
                        case ConnectEvent e:
                            connected.Add(e.WebSocket);
                            break;
                        case DisconnectEvent e:
                            connected.Remove(e.WebSocket);
                            break;
                        case BroadcastEvent e:
                            Log.Information("Broadcasting: " + Encoding.UTF8.GetString(e.EventData));
                            byte[] bytes = e.EventData;
                            foreach (WebSocket ws in connected)
                            {
                                await ws.SendAsync(bytes, e.Type, true, ct);
                            }
                            break;
                    }
                }
            });

            Task incomingHandler = Task.Run(async () =>
            {
                Log.Information("Starting incoming handler?");

                Task<HttpListenerWebSocketContext>? webSocketTask = null;
                Task<Task>? dropClientTask = null;

                // TODO: Use the cancellation token properly by cancelling if we're waiting for something
                while (!ct.IsCancellationRequested)
                {
                    webSocketTask ??= AcceptWebSocketAsync(listener);
                    dropClientTask ??= Task.WhenAny(clientTaskList);

                    Task completed = await Task.WhenAny(dropClientTask, webSocketTask);

                    if (completed == webSocketTask)
                    {
                        Log.Information("Client connected.");

                        WebSocket ws = (await webSocketTask).WebSocket;
                        await eventChannel.Writer.WriteAsync(new ConnectEvent(ws));

                        clientTaskList.Add(Task.Run(async () =>
                        {
                            byte[] buffer = new byte[1024];

                            // TODO: Stitch packets
                            while (!ct.IsCancellationRequested)
                            {
                                WebSocketReceiveResult data = await ws.ReceiveAsync(buffer, ct);

                                await eventChannel.Writer.WriteAsync(data.MessageType switch
                                {
                                    WebSocketMessageType.Close => new DisconnectEvent(ws),
                                    WebSocketMessageType type => new BroadcastEvent(type, buffer[..data.Count]),
                                }, ct);
                            }
                        }));

                        webSocketTask = null;
                    }
                    else if (completed == dropClientTask)
                    {
                        Log.Information("Client dropped.");
                        clientTaskList.Remove(await dropClientTask);
                        dropClientTask = null;
                    }
                }
            });

            Log.Information("Initialized.");
            await Task.WhenAll(incomingHandler, eventHandler);
        }

        static async Task<HttpListenerWebSocketContext> AcceptWebSocketAsync(HttpListener listener, string? subProtocol = null)
            => await (await listener.GetContextAsync()).AcceptWebSocketAsync(subProtocol);
    }
}
