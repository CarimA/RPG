using System.Collections.Generic;

namespace PhotoVs.Utils.Logging
{
    public class Logger : List<ILogger>
    {
        private static Logger _instance;

        public static Logger Write =>
            _instance ??= new Logger
            {
                new ConsoleLogger(LogLevel.Trace)
            };

        public void Trace(string message, params object[] args)
        {
            ForEach(logger => logger.LogTrace(message, args));
        }

        public void Info(string message, params object[] args)
        {
            ForEach(logger => logger.LogInfo(message, args));
        }

        public void Warn(string message, params object[] args)
        {
            ForEach(logger => logger.LogWarn(message, args));
        }

        public void Error(string message, params object[] args)
        {
            ForEach(logger => logger.LogError(message, args));
        }

        public void Fatal(string message, params object[] args)
        {
            ForEach(logger => logger.LogFatal(message, args));
        }
    }
}