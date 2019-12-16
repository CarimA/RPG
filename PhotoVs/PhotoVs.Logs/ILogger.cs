namespace PhotoVs.Logs
{
    public interface ILogger
    {
        void SetLogLevel(LogLevel level);

        void Log(LogLevel level, string message, params object[] args);

        void LogTrace(string message, params object[] args);
        void LogDebug(string message, params object[] args);
        void LogInfo(string message, params object[] args);
        void LogWarn(string message, params object[] args);
        void LogError(string message, params object[] args);
        void LogFatal(string message, params object[] args);
    }
}