using System;

namespace Jint.DevToolsProtocol.Logging
{
    public class ConsoleLoggingProvider : ILoggingProvider
    {
        private ConsoleColor _defaultColor;

        public ConsoleLoggingProvider()
        {
            _defaultColor = Console.ForegroundColor;
        }

        public void Log(LogEntryType logEntryType, string message)
        {
            switch (logEntryType)
            {
                case LogEntryType.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogEntryType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogEntryType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
            
            Console.WriteLine(message);

            Console.ForegroundColor = _defaultColor;
        }
    }
}
