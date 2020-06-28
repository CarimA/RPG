using System.Collections.Generic;

namespace PhotoVs.Utils.Logging
{
    public class Logger : List<ILogger>
    {
        private static Logger _instance;

        public static Logger Write =>
            _instance ??= new Logger
            {
                new ConsoleLogger(LogLevel.Trace),
                new TextLogger(LogLevel.Trace)
            };

        public void AddLogger(ILogger logger)
        {
            Write.Add(logger);
        }

        public void Trace(string message, params object[] args)
        {
            foreach (var logger in this)
                logger.LogTrace(message, args);
        }

        public void Info(string message, params object[] args)
        {
            foreach (var logger in this)
                logger.LogInfo(message, args);
        }

        public void Warn(string message, params object[] args)
        {
            foreach (var logger in this)
                logger.LogWarn(message, args);
        }

        public void Error(string message, params object[] args)
        {
            foreach (var logger in this)
                logger.LogError(message, args);
        }

        public void Fatal(string message, params object[] args)
        {
            foreach (var logger in this)
                logger.LogFatal(message, args);
        }
    }
}