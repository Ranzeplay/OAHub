using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace OAHub.Base.Interfaces
{
    public interface IOALogger
    {
        void LogData(string content, LogLevel logLevel);

        KeyValuePair<LogLevel, string> GetLog(long id);
    }
}
