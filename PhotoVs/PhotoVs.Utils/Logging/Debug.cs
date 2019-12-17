using System;
using System.Collections.Generic;

namespace PhotoVs.Utils.Logging
{
    public class Debug : List<ILogger>
    {
        private static Debug _instance;

        public static Debug Log =>
            _instance ??= new Debug
            {
                new ConsoleLogger(LogLevel.Trace)
            };

        public ILogger this[Type type] => Find(logger => logger.GetType() == type);

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