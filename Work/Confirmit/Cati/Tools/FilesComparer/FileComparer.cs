using System;
using System.IO;
using System.Text.RegularExpressions;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace FilesComparer
{
    public class FileComparer
    {
        private readonly ILogger _logger;
        private readonly bool _logWrongComparedFiles;
        private readonly string[] _ignoreFileMasks;

        private readonly AssemblyComparer _assemblyComparer;
        private readonly ExternalInvoker _externalInvoker;

        public FileComparer(ILogger logger, AssemblyComparer assemblyComparer, ExternalInvoker externalInvoker, string ignoreFileMask, bool logWrongComparedFiles)
        {
            _logger = logger;
            _ignoreFileMasks = ignoreFileMask.Split(new [] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            _logWrongComparedFiles = logWrongComparedFiles;
            _assemblyComparer = assemblyComparer;
            _externalInvoker = externalInvoker ;
        }

        public CompareState AreTwoFilesEqual(string filePath1, string filePath2)
        {
            string fileName = Path.GetFileName(filePath1);
            if (fileName == null || !IsNeedToCompare(Path.GetFileName(fileName)))
            { 
                return CompareState.Skipped;
            }

            if (fileName.EndsWith(".dll") || fileName.EndsWith(".exe"))
            {
                return CompareBinaryFiles(filePath1, filePath2);
            }

            return CompareTextFiles(filePath1, filePath2);
        }

        public void CompareTwoFilesInAraxis(string filePath1, string filePath2, string tempComparingPath)
        {
            string fileName = Path.GetFileName(filePath1);

            if (fileName!= null && (fileName.EndsWith(".dll") || fileName.EndsWith(".exe")))
            {
                CompareBinaryFiles(filePath1, filePath2);

                string ilFilePath1 = Path.Combine(tempComparingPath, "1", RemoveExtention(fileName) + ".il");
                string ilFilePath2 = Path.Combine(tempComparingPath, "2", RemoveExtention(fileName) + ".il");
                
                RunAraxis(ilFilePath1, ilFilePath2);
            }
            else
            {
                RunAraxis(filePath1, filePath2);
            }
        }

        private void RunAraxis(string filePath1, string filePath2)
        {
            _externalInvoker.Invoke(@"c:\Program Files (x86)\Araxis\Araxis Merge\Compare.exe", string.Format("/wait /2 \"{0}\" \"{1}\"", filePath1, filePath2), false);
            
        }

        private CompareState CompareTextFiles(string filePath1, string filePath2)
        {
            string contentF1 = File.ReadAllText(filePath1);
            string contentF2 = File.ReadAllText(filePath2);

            if (contentF1 == contentF2)
            { 
                return CompareState.Equal;
            }

            return CompareState.Different;
        }

        private CompareState CompareBinaryFiles(string filePath1, string filePath2)
        {
            try
            {
                string filePathWithoutExtention1 = RemoveExtention(filePath1);
                string filePathWithoutExtention2 = RemoveExtention(filePath2);
                if (_assemblyComparer.Compare(filePathWithoutExtention1, filePathWithoutExtention2) == 0) 
                {
                    return CompareState.Equal;
                }

                return CompareState.Different;
            }
            catch (Exception ex)
            {
                if (_logWrongComparedFiles)
                {
                    _logger.WriteLog("Cannot compare files:\r\n\t{0}\r\n\t{1}\r\nbecause\r\n{2}", filePath1, filePath2, ex.ToString());
                }

                return CompareState.NotCompared; 
            }
        }

        private string RemoveExtention(string filePath)
        {
            string extention = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(extention))
            {
                throw new Exception("Wrong file path\r\n\t" + filePath);
            }

            int n = filePath.IndexOf(extention, StringComparison.Ordinal);
            return filePath.Substring(0, n);
        }

        private bool IsNeedToCompare(string fileName)
        {
            foreach (var ignoreFileMask in _ignoreFileMasks)
            {
                var mask = new Regex(ignoreFileMask.Replace(".", "[.]").Replace("*", ".*").Replace("?", "."), RegexOptions.IgnoreCase);
                if (mask.IsMatch(fileName))
                {
                    return false;
                }
            }

            return true;
        }
    }
}