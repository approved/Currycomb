using System;
using System.Threading.Tasks;
using Serilog;

namespace Currycomb.Gateway
{
    public class Program
    {
        private static readonly string LogFileName = $"logs/gateway/gateway_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{Environment.ProcessId}.txt";

        public static async Task Main()
        {
            Log.Logger = new LoggerConfiguration()
                   .MinimumLevel.Verbose()
                   .WriteTo.Async(x => x.Console())
                   .WriteTo.Async(x => x.File(LogFileName))
                   .CreateLogger();

            await new GatewayServer().Run();
        }
    }
}