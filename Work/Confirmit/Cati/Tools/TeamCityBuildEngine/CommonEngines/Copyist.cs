using System.Collections.Generic;
using System.IO;
using TeamCityBuildEngine.Interfaces;

namespace TeamCityBuildEngine.CommonEngines
{
    public class Copyist : ICopyist
    {
        private readonly ILogger _logger;
        private List<string> _exceptDirectoryPaths;

        public Copyist(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Copy the file and write information in log
        /// </summary>        
        /// <param name="sourceFileName">Sourse file path</param>
        /// <param name="destFileName">Destination file path</param>
        public void CopyFile(string sourceFileName, string destFileName)
        {
            if (_logger != null)
            {
                _logger.WriteLog("Copy from\r\n{0}\r\nto\r\n{1}", sourceFileName, destFileName);
            }

            string destDirectory = Path.GetDirectoryName(destFileName) ?? string.Empty;
            if (!Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }

            if (File.Exists(destFileName))
            {
                File.SetAttributes(destFileName, FileAttributes.Normal);
            }

            File.Copy(sourceFileName, destFileName, true);
        }

        /// <summary>
        /// Copy the file and write information in log
        /// </summary>        
        /// <param name="sourceDirectoryName">Sourse directory path</param>
        /// <param name="destDirectoryName">Destination directory path</param>
        public void CopyDirectory(string sourceDirectoryName, string destDirectoryName)
        {
            CopyDirectory(sourceDirectoryName, destDirectoryName, new List<string>());
        }

        /// <summary>
        /// Copy the file and write information in log
        /// </summary>        
        /// <param name="sourceDirectoryName">Sourse directory path</param>
        /// <param name="destDirectoryName">Destination directory path</param>
        /// <param name="exceptDirectoryPaths">Except directory path list. These paths won't be copied</param>
        public void CopyDirectory(string sourceDirectoryName, string destDirectoryName, List<string> exceptDirectoryPaths)
        {
            if (Directory.Exists(destDirectoryName))
            {
                RemoveDirectory(destDirectoryName);
            }

            _exceptDirectoryPaths = exceptDirectoryPaths;
            CopyDir(sourceDirectoryName, destDirectoryName);
        }

        /// <summary>
        /// Recursived removing of directory with read-only files
        /// </summary>
        /// <param name="destDirectoryName">Path to directory</param>
        public void RemoveDirectory(string destDirectoryName)
        {
            if (!Directory.Exists(destDirectoryName))
            {
                return;
            }

            foreach (string fileSource in Directory.GetFiles(destDirectoryName))
            {
                File.SetAttributes(fileSource, FileAttributes.Normal);
            }

            foreach (string dirPath in Directory.GetDirectories(destDirectoryName))
            {
                RemoveDirectory(dirPath);
            }

            _logger.WriteLog("Remove directory\r\n{0}", destDirectoryName);
            Directory.Delete(destDirectoryName, true);
        }

        /// <summary>
        /// Recursive function to copy directory
        /// </summary>
        /// <param name="fromDir">Sourse directory path</param>
        /// <param name="toDir">Destination directory path</param>
        private void CopyDir(string fromDir, string toDir)
        {
            Directory.CreateDirectory(toDir);
            if (_logger != null)
            {
                _logger.WriteLog("Copy from\r\n{0}\r\nto\r\n{1}", fromDir, toDir);
            }

            foreach (string fileSource in Directory.GetFiles(fromDir))
            {
                File.Copy(fileSource, Path.Combine(toDir, Path.GetFileName(fileSource) ?? string.Empty), true);
            }

            foreach (string dirSource in Directory.GetDirectories(fromDir))
            {
                if (!_exceptDirectoryPaths.Contains(dirSource))
                {
                    CopyDir(dirSource, Path.Combine(toDir, Path.GetFileName(dirSource) ?? string.Empty));
                }
            }
        }
    }
}
