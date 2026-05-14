using System.IO;

namespace TeamCityBuildEngine
{
    public class LogPathPresenter
    {
        private readonly string _solutionRoot;

        public LogPathPresenter(string solutionRoot)
        {
            _solutionRoot = solutionRoot;
        }

        public string GetLogPath(string logFileName)
        {
            string logFolderPath = Path.Combine(_solutionRoot, @"assemblies\Logs");
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }

            if (!string.IsNullOrEmpty(logFileName))
            {
                return Path.Combine(logFolderPath, logFileName);
            }

            return logFolderPath;
        }
    }
}