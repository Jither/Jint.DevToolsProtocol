using System.Collections.Generic;
using System.Text;

namespace Jint.DevToolsProtocol.Logging
{
    public interface ILoggingProvider
    {
        void Log(LogEntryType logEntryType, string message);
    }
}
