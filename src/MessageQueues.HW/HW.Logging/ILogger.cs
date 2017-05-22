using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW.Logging
{
    public interface ILogger
    {
        void LogInfo(string message, params object[] args);
        void LogError(string message, params object[] args);
        void LogError(Exception exception);
        void SetActualLogger(object logger);
    }
}
