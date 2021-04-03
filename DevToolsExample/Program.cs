using Jint.DevToolsProtocol.Logging;
using Jint.DevToolsProtocol.Protocol;
using Jint.DevToolsProtocol.Server;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevToolsExample
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Logger.LoggingProvider = new ConsoleLoggingProvider();

            var server = new MinimalServer();
            var agent = new Agent(server);
            server.Start();

            Logger.Info("Press any key to exit...");
            Console.ReadKey();

            await server.StopAsync();
        }
    }
}
