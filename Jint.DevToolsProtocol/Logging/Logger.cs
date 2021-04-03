using System;
using System.Collections.Generic;
using System.Text;

namespace Jint.DevToolsProtocol.Logging
{
    public static class Logger
    {
        public static ILoggingProvider LoggingProvider { get; set; }

        public static void Info(string message)
        {
            LoggingProvider?.Log(LogEntryType.Info, message);
        }

        public static void Error(string message)
        {
            LoggingProvider?.Log(LogEntryType.Error, message);
        }

        public static void Warning(string message)
        {
            LoggingProvider?.Log(LogEntryType.Warning, message);
        }
    }
}
