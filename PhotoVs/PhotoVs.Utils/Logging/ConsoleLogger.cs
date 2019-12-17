using System;

namespace PhotoVs.Utils.Logging
{
    public class ConsoleLogger : ILogger
    {
        private LogLevel _currentLevel;

        public ConsoleLogger(LogLevel level)
        {
            SetLogLevel(level);
        }

        public void SetLogLevel(LogLevel level)
        {
            _currentLevel = level;
        }

        public void Log(LogLevel level, string message, params object[] args)
        {
            if (level < _currentLevel)
                return;

            ResetFormatting();
            SetFormatting(level);
            Console.WriteLine(
                $"[{DateTime.Now:hh:mm:ss}]\t{Enum.GetName(typeof(LogLevel), level).ToUpperInvariant()}\t\t{message}",
                args);
            ResetFormatting();
        }

        public void LogTrace(string message, params object[] args)
        {
            Log(LogLevel.Trace, message, args);
        }

        public void LogInfo(string message, params object[] args)
        {
            Log(LogLevel.Info, message, args);
        }

        public void LogWarn(string message, params object[] args)
        {
            Log(LogLevel.Warn, message, args);
        }

        public void LogError(string message, params object[] args)
        {
            Log(LogLevel.Error, message, args);
        }

        public void LogFatal(string message, params object[] args)
        {
            Log(LogLevel.Fatal, message, args);
        }

        private void ResetFormatting()
        {
            SetFormatting(LogLevel.Info);
        }

        private void SetFormatting(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    SetConsoleColor(ConsoleColor.Black, ConsoleColor.Gray);
                    break;

                case LogLevel.Info:
                    SetConsoleColor();
                    break;

                case LogLevel.Warn:
                    SetConsoleColor(ConsoleColor.Black, ConsoleColor.Yellow);
                    break;

                case LogLevel.Error:
                    SetConsoleColor(ConsoleColor.Black, ConsoleColor.Red);
                    break;

                case LogLevel.Fatal:
                    SetConsoleColor(ConsoleColor.Red);
                    break;

                default:
                    LogError("Level ", level.ToString(), " not found as formatter.");
                    break;
            }
        }

        private static void SetConsoleColor(ConsoleColor backgroundColor = ConsoleColor.Black,
            ConsoleColor foregroundColor = ConsoleColor.White)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
        }
    }
}