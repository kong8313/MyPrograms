using System;
using System.Diagnostics;
using System.IO;
using TeamCityBuildEngine.Interfaces;

namespace TeamCityBuildEngine.CommonEngines
{
    public class FileLogger : ILogger
    {
        private readonly string _logFilePath;

        public FileLogger(string logFilePath)
        {
            _logFilePath = logFilePath;
            CreateLogDirectory();
        }

        private void CreateLogDirectory()
        { 
            string logDirectory = Path.GetDirectoryName(_logFilePath);
            if (logDirectory == null)
            {
                throw new Exception("Wrong log file path");
            }

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
        }

        public void WriteLog(string message, params object[] parameters)
        {
            WriteLog(TraceEventType.Information, message, parameters);
        }

        public void WriteLog(TraceEventType traceType, string message, params object[] parameters)
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
        }
    }
}
