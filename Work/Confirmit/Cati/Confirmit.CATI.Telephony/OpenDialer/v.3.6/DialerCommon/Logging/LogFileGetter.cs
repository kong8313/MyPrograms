using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Confirmit.CATI.Common.Logging;
using DialerCommon;
using System.IO.Compression;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace DialerCommon.Logging
{
    public class LogFileGetter
    {
        private readonly TextToLogFileTraceListener _listener;

        private readonly LogFileNameGenerator _logFileNameGenerator;

        public LogFileGetter(TraceSource traceSource)
        {
            _listener = traceSource.Listeners.OfType<TextToLogFileTraceListener>().FirstOrDefault();
            _logFileNameGenerator = new LogFileNameGenerator(_listener?.LoggingFileName);
        }

        public IEnumerable<LogFileInfo> GetLogFiles()
        {
            if (_listener == null) return new List<LogFileInfo>();
            var dir = new DirectoryInfo(_listener.LoggingPath);
            return dir.GetFiles()
                .Where(f => _logFileNameGenerator.CheckFileName(f.Name))
                .Select(e => new LogFileInfo(e.Name, e.Length, e.CreationTimeUtc, e.LastWriteTimeUtc)).OrderBy(i => i.LastWriteTimeUtc);
        }

        protected byte[] GetLogFileBody(string fileName)
        {
            CheckFileName(fileName);
            if (_listener == null) return null;
            var filePath = Path.Combine(_listener.LoggingPath, fileName);
            if (!File.Exists(filePath)) return null;
            //use additional parameters for the ability to read a file at competitive access  
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        public byte[] GetLogFileBodyZipped(string fileName)
        {
            CheckFileName(fileName);
            var body = GetLogFileBody(fileName);
            if (body == null) return null;

            using (var zipStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                {
                    var entry = zipArchive.CreateEntry(fileName);
                    using (var entryStream = entry.Open())
                    {
                        entryStream.Write(body, 0, body.Length);
                    }
                }
                zipStream.Seek(0, SeekOrigin.Begin);
                var buffer = new byte[zipStream.Length];
                zipStream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        private void CheckFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new System.ArgumentException($"Argument {nameof(fileName)} (value = '{fileName}') is empty", nameof(fileName));
            if (!IsValidFilename(fileName))
                throw new System.ArgumentException($"Argument {nameof(fileName)} (value = '{fileName}') contains invalid char.", nameof(fileName));
            if (!_logFileNameGenerator.CheckFileName(fileName))
                throw new System.ArgumentException("Invalid log file name.", nameof(fileName));
        }

        private bool IsValidFilename(string fileName)
        {
            var containsABadCharacter = new Regex("[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]");
            return !containsABadCharacter.IsMatch(fileName);
        }
    }
}