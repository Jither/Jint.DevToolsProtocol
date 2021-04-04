using Jint;
using Jint.DevToolsProtocol.Logging;
using Jint.DevToolsProtocol.Protocol;
using Jint.DevToolsProtocol.Server;
using Jint.Runtime.Debugger;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevToolsExample
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Logger.LoggingProvider = new ConsoleLoggingProvider();

            if (args.Length == 0)
            {
                Logger.Error("No script specified");
                return;
            }
            string scriptPath = args[0];

            int port = 9222;
            if (args.Length == 2)
            {
                if (!Int32.TryParse(args[1], out port))
                {
                    Logger.Error($"Invalid port number: {args[1]}");
                }
            }

            string script;
            try
            {
                script = File.ReadAllText(scriptPath, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed loading script '{scriptPath}': {ex.Message}");
                return;
            }

            var options = new ServerOptions().UsePort(port);

            var server = new MinimalServer(options);
            var engine = new Engine(options => options
                .DebugMode()
                .DebuggerStatementHandling(DebuggerStatementHandling.Script));

            var agent = new Agent(engine, server);

            agent.Ready += (sender, args) =>
            {
                // TODO: Brittle threading, just for testing
                Task.Run(() => engine.Execute(script, new Esprima.ParserOptions(scriptPath.Replace('\\', '/')) { AdaptRegexp = true, Tolerant = true }));
            };

            server.Start();

            Logger.Info("Press any key to exit...");
            Console.ReadKey();

            await server.StopAsync();
        }
    }
}
