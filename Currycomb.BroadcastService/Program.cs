using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Currycomb.BroadcastService
{
    public class Program
    {
        abstract record Event;

        record ConnectEvent(WebSocket WebSocket) : Event;
        record DisconnectEvent(WebSocket WebSocket) : Event;
        record BroadcastEvent(WebSocketMessageType Type, byte[] EventData) : Event;

        public static async Task Main()
        {
            List<Task> clientTaskList = new();
            HttpListener listener = new();
            listener.Prefixes.Add("http://127.0.0.1:10002/");
            listener.Start();

            Console.WriteLine("Started listener");

            CancellationTokenSource cts = new();

            CancellationToken ct = cts.Token;

            Channel<Event> eventChannel = Channel.CreateUnbounded<Event>();

            Task eventHandler = Task.Run(async () =>
            {
                List<WebSocket> connected = new();

                while (!ct.IsCancellationRequested)
                {
                    Event ev = await eventChannel.Reader.ReadAsync(ct);
                    Console.WriteLine($"Handling event: {ev}");

                    switch (ev)
                    {
                        case ConnectEvent e:
                            connected.Add(e.WebSocket);
                            break;
                        case DisconnectEvent e:
                            connected.Remove(e.WebSocket);
                            break;
                        case BroadcastEvent e:
                            Console.WriteLine("Broadcasting: " + Encoding.UTF8.GetString(e.EventData));
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
                        clientTaskList.Remove(await dropClientTask);
                        dropClientTask = null;
                    }
                }
            });

            Console.WriteLine("Initialized.");
            await Task.WhenAll(incomingHandler, eventHandler);
        }

        static async Task<HttpListenerWebSocketContext> AcceptWebSocketAsync(HttpListener listener, string? subProtocol = null)
            => await (await listener.GetContextAsync()).AcceptWebSocketAsync(subProtocol);
    }
}
