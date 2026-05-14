using System;
using System.Diagnostics;
using System.IO;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace Confirmit.CATI.Installation.Common
{
    public class FileAndConsoleLogger : ILogger
    {
        private readonly string _logFilePath;
        private readonly object _lock;

        public FileAndConsoleLogger(string logFilePath)
        {
            _lock = new object();
            _logFilePath = logFilePath;
            CreateLogDirectoryAndRemoveOldLogFile();
        }

        private void CreateLogDirectoryAndRemoveOldLogFile()
        {
            string logDirectory = Path.GetDirectoryName(_logFilePath);
            if (logDirectory == null)
            {
                throw new Exception("Wrong log file path");
            }

            if (File.Exists(_logFilePath))
            {
                File.Delete(_logFilePath);
            }
            else if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
        }

        public void WriteLog(string message, params object[] parameters)
        {
            WriteLog(false, message, parameters);
        }

        public void WriteLog(bool isPrintOnConsole, string message, params object[] parameters)
        {
            WriteLog(isPrintOnConsole, TraceEventType.Information, message, parameters);
        }

        public void WriteLog(TraceEventType traceType, string message, params object[] parameters)
        {
            WriteLog(false, traceType, message, parameters);
        }

        public void WriteLog(bool isPrintOnConsole, TraceEventType traceType, string message, params object[] parameters)
        {
            lock (_lock)
            {
                if (parameters.Length > 0)
                {
                    message = string.Format(message, parameters);
                }

                using (var sw = new StreamWriter(_logFilePath, true))
                {
                    DateTime nowTime = DateTime.Now;
                    sw.WriteLine(nowTime.ToLongTimeString() + "." + nowTime.Millisecond + ": " + traceType + ": " + message);
                }

                if (isPrintOnConsole)
                {
                    Console.WriteLine(message);
                }
            }
        }
    }
}