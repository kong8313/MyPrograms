using System;
using System.Globalization;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections;

namespace DialerCommon
{
    /// <summary>
    /// Inherited IsThreadSafe property is 'false'
    /// It means that .Net will use external 'global lock' to provide thread safety
    /// http://msdn.microsoft.com/en-us/library/system.diagnostics.trace.usegloballock(v=vs.110).aspx
    /// </summary>
    class TextToLogFileTraceListener : TraceListener
    {
        public const int Megabyte = 1048576;
        private const int DefaultFileSizeLimit = 100 * Megabyte; // 100 megabytes

        private string _fullFileName;
        private string _fullFileNameWithoutExtension;
        private string _extension;
        private int _suffixCounter;
        private int _fileSizeLimit;

        private TextWriterTraceListener _workingListener;

        /// <summary>
        /// The empty constructor is needed because of we have test constructor with parameters below.
        /// Note: We can't initialize in the constructor because of Attributes collection is empty here for some reason.
        /// </summary>
        public TextToLogFileTraceListener()
        {
        }

        /// <summary>
        /// The constructor is used in tests only.
        /// </summary>
        public TextToLogFileTraceListener(string loggingPath, string loggingFileName, int fileSizeLimit)
        {
            Attributes.Clear(); // Just in case

            Attributes.Add("LoggingPath", loggingPath);
            Attributes.Add("LoggingFileName", loggingFileName);
            Attributes.Add("FileSizeLimit", fileSizeLimit.ToString(CultureInfo.InvariantCulture));
        }

        private void Initialize()
        {
            _fileSizeLimit = FileSizeLimit;
            _suffixCounter = 0;

            _fullFileName = CompileFileName(LoggingPath + LoggingFileName);

            CreateDirectory(_fullFileName);

            _fullFileNameWithoutExtension = Path.Combine(
                Path.GetDirectoryName(_fullFileName), Path.GetFileNameWithoutExtension(_fullFileName));
            _extension = Path.GetExtension(_fullFileName);

            _workingListener = new TextWriterTraceListener(_fullFileName);

            if (File.Exists(_fullFileName))
            {
                CheckSizeAndCreateNextFileIfNeeded();
            }
        }

        public override void Write(string s)
        {
            if (_workingListener == null)
            {
                // It's an unexpected case that means the method is called directly, not via TraceEvent.
                // Anyway let's try to initialize and work further.
                Initialize();
            }

            _workingListener.Write(s);
        }

        public override void WriteLine(string s)
        {
            if (_workingListener == null)
            {
                // It's an unexpected case that means the method is called directly, not via TraceEvent.
                // Anyway let's try to initialize and work further.
                Initialize();
            }

            _workingListener.WriteLine(s);
        }

        // Write and WriteLine are called from the 'base.TraceEvent' method.
        public override void TraceEvent(
            TraceEventCache eventCache,
            String source,
            TraceEventType eventType,
            int id,
            string message)
        {
            if (_workingListener == null)
            {
                // It means the very first request. Initialize everything...
                Initialize();
            }

            base.TraceEvent(eventCache, source, eventType, id, message);

            CheckSizeAndCreateNextFileIfNeeded();
        }

        // Write and WriteLine are called from the 'base.TraceEvent' method.
        public override void TraceEvent(
            TraceEventCache eventCache,
            String source,
            TraceEventType eventType,
            int id,
            string format,
            params object[] args)
        {
            if (_workingListener == null)
            {
                // It means the very first request. Initialize everything...
                Initialize();
            }

            base.TraceEvent(eventCache, source, eventType, id, format, args);

            CheckSizeAndCreateNextFileIfNeeded();
        }

        private void CheckSizeAndCreateNextFileIfNeeded()
        {
            var fileInfo = new FileInfo(_fullFileName);

            if (fileInfo.Length < _fileSizeLimit)
            {
                return;
            }

            while (true)
            {
                _suffixCounter++;
                _fullFileName = _fullFileNameWithoutExtension + "." + _suffixCounter + _extension;

                if (!File.Exists(_fullFileName))
                {
                    // A new file
                    break;
                }

                // File already exists. Check its size.
                fileInfo = new FileInfo(_fullFileName);

                if (fileInfo.Length < _fileSizeLimit)
                {
                    // Size limit is not yet reached
                    break;
                }
            }

            if (_workingListener != null)
            {
                _workingListener.Flush();
                _workingListener.Close();
            }

            _workingListener = new TextWriterTraceListener(_fullFileName);
        }

        protected override string[] GetSupportedAttributes()
        {
            return new[] { "LoggingPath", "LoggingFileName", "FileSizeLimit" };
        }

        public override void Close()
        {
            _workingListener.Close();
        }

        public override void Flush()
        {
            _workingListener.Flush();
        }

        private string LoggingPath
        {
            get
            {
                // 'null' means the path is not configured. Use the default path in this case.
                var loggingPath = GetAttribute("loggingpath") ?? DefaultLoggingPath;

                if (loggingPath.Length > 0)
                {
                    if (loggingPath[loggingPath.Length - 1] != '\\')
                    {
                        loggingPath += '\\';
                    }
                }

                return loggingPath;
            }
        }

        public string LoggingFileName
        {
            get
            {
                var loggingFileName = GetAttribute("loggingfilename");

                if (string.IsNullOrWhiteSpace(loggingFileName))
                {
                    // 'null' means the path is not configured. Empty string is not a file name. Use the default file name.
                    return DefaultLoggingFileName;
                }

                return loggingFileName;
            }
        }

        private int FileSizeLimit
        {
            get
            {
                var fileSizeLimitString = GetAttribute("filesizelimit");

                int fileSizeLimit;
                int.TryParse(fileSizeLimitString, out fileSizeLimit);

                return (fileSizeLimit > 0) ? (fileSizeLimit * Megabyte) : DefaultFileSizeLimit;
            }
        }

        private string GetAttribute(string attributeName)
        {
            return (string)Attributes.Cast<DictionaryEntry>()
                .FirstOrDefault(attr => ((string)attr.Key).ToLower() == attributeName.ToLower()).Value;
        }
        
        private string DefaultLoggingPath
        {
            get
            {
                return Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + @"\Log";
            }
        }

        private string DefaultLoggingFileName
        {
            get
            {
                return "%datetime%.log";
            }
        }

        /// <summary>
        /// Check if the path exists and try to create if it does not
        /// </summary>
        /// <param name="fullFileName"></param>
        private void CreateDirectory(string fullFileName)
        {
            var directoryName = Path.GetDirectoryName(fullFileName);

            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
        }

        /// <summary>
        /// Replaces template variables %datetime% and %instance% with values
        /// </summary>
        /// <param name="templateFileName">Template file name that may include 
        /// %datetime% and %instance% variables
        /// </param>
        /// <returns>File name string generated based on the template</returns>
        private string CompileFileName(string templateFileName)
        {
            string fileName = templateFileName.Replace("%datetime%", GetDateTimeFormattedForFileName());
            fileName = fileName.Replace("%date%", DateTime.Now.ToString("yyyyMMdd"));

            string instanceName = string.Empty;
            string[] commandLineArgs = Environment.GetCommandLineArgs();

            // Try to find instance name as value of "-Instance" or "/Instance" switch in the command line
            for (int i = 1; i < commandLineArgs.Length; i++)
            {
                if ((string.Compare(commandLineArgs[i], "-Instance", true) == 0) ||
                     (string.Compare(commandLineArgs[i], "/Instance", true) == 0))
                {
                    ++i;
                    if (i < commandLineArgs.Length)
                    {
                        instanceName = commandLineArgs[i];
                    }
                }
            }

            if (string.IsNullOrEmpty(instanceName))
            {
                // We've found no instance name in the command line, so assume that it's the default instance
                instanceName = "Default";
            }

            fileName = fileName.Replace("%instance%", instanceName);

            return fileName;
        }

        private string GetDateTimeFormattedForFileName()
        {
            return string.Format("{0:yyyyMMdd}T{0:HHmmss.fffzzz}", DateTime.Now).Replace(":", string.Empty);
        }
    }
}
