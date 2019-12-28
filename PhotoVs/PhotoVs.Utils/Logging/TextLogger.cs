using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoVs.Utils.Logging
{
    public class TextLogger : ILogger
    {
        private LogLevel _currentLevel;
        private string _fileName;

        // Directory.CreateDirectory(Path.Combine(myDocs, "PhotoVs/Logs"));

        public TextLogger(LogLevel level)
        {
            SetLogLevel(level);
            _fileName = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        }

        public void SetLogLevel(LogLevel level)
        {
            _currentLevel = level;
        }

        private void Write(string text, params object[] args)
        {
            File.AppendAllText(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    $"PhotoVs/Logs/{_fileName}.log"),
                string.Format(text, args));

        }

        public void Log(LogLevel level, string message, params object[] args)
        {
            if (level < _currentLevel)
                return;

            Write(
                $"[{DateTime.Now:hh:mm:ss}]\t{Enum.GetName(typeof(LogLevel), level).ToUpperInvariant()}\t\t{message}\n",
                args);
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
    }
}
