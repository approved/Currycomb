using Currycomb.Attributes;
using Currycomb.Packets;
using Serilog;
using Serilog.Core;
using System;
using System.Reflection;
using System.Threading;

namespace Currycomb
{
    public class Server
    {
        private static string LogFileName = $"logs/manager_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{Environment.ProcessId}.txt";
        public static Thread ListenerThread;

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                   .MinimumLevel.Verbose()
                   .WriteTo.Console()
                   .WriteTo.File(LogFileName)
                   .CreateLogger();

            InitPacketDictionary();

            ListenerThread = new Thread(ServerListener.StartListener)
            {
                IsBackground = true,
            };
            ListenerThread.Start();

            string input = string.Empty;
            do
            {
                input = Console.ReadLine() ?? string.Empty;
            }
            while (!input.ToLower().Trim().Equals("quit"));

            //Attempt to interrupt the listener thread
            try
            {
                ListenerThread.Interrupt();
            }
            finally
            {
                Log.Information("Closing Server...");
            }
        }

        private static void InitPacketDictionary()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(ClientPacketAttribute), false).Length > 0)
                {
                    ClientPacketAttribute? attrib = Attribute.GetCustomAttribute(type, typeof(ClientPacketAttribute)) as ClientPacketAttribute;
                    if (attrib is not null)
                    {
                        foreach (int id in attrib.PacketIDs)
                        {
                            if (!PacketHandler.ClientPackets.ContainsKey(id))
                            {
                                PacketHandler.ClientPackets.Add(id, type);
                            }
                        }
                    }
                }
            }
        }
    }
}
