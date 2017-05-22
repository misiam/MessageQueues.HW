using System;
using NLog;

namespace HW.Logging
{
    public class Logger : ILogger
    {
        private static ILogger _currentLogger;
        private NLog.ILogger _actualLogger = LogManager.CreateNullLogger();


        public static ILogger Current
        {
            get { return _currentLogger ?? (_currentLogger = new Logger()); }
        }

        public virtual void LogInfo(string message, params object[] args)
        {
            _actualLogger.Trace(message, args);
        }

        public virtual void LogError(string message, params object[] args)
        {
            _actualLogger.Error(message, args);
        }

        public virtual void LogError(Exception exception)
        {
            _actualLogger.Error(exception);
        }

        public virtual void SetActualLogger(object logger)
        {
            _actualLogger = (NLog.ILogger)logger;
        }
    }
}
