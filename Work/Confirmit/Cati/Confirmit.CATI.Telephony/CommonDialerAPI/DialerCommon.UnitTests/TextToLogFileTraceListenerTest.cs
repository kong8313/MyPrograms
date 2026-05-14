using System;
using System.Diagnostics;
using System.IO;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.Telephony;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ILoggerCodi = ConfirmitDialerInterface.ILogger;

namespace DialerCommon.UnitTests
{
    /// <summary>
    /// Summary description for LoggerTest
    /// </summary>
    [TestClass]
    public class TextToLogFileTraceListenerTest
    {
        private TextToLogFileTraceListener _currentListener;
        private TraceSource _traceSource;

        [TestInitialize]
        public void TestInitialize()
        {
            _traceSource = new TraceSource("TextToLogFileTraceListenerTestTraceSource")
                {
                    Switch = { Level = SourceLevels.All }
                };
        }

        [TestMethod, Owner(@"FIRM\alm"), Bug(79129)]
        public void ListenerAutoinitializedProperlyNoExceptionIsTrown()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            var actionsToTest = new Action[]
                {
                    () => Trace.TraceInformation(testName + " test: Trace.TraceInformation with no arguments."),
                    () => Trace.TraceWarning(testName + " test: Trace.TraceWarning with no arguments."),
                    () => Trace.TraceError(testName + " test: Trace.TraceError with no arguments."),
                    () => Trace.TraceInformation(testName + " test: Trace.TraceInformation {0}.", "with argument"),
                    () => Trace.TraceWarning(testName + " test: Trace.TraceWarning {0}.", "with argument"),
                    () => Trace.TraceError(testName + " test: Trace.TraceError {0}.", "with argument"),

                    // via TraceSource
                    () => _traceSource.TraceEvent(TraceEventType.Information, 0,
                        testName + " test: TraceSource.TraceEvent [TraceEventType.Information]."),
                    () => _traceSource.TraceEvent(TraceEventType.Warning, 0,
                        testName + " test: TraceSource.TraceEvent [TraceEventType.Warning]."),
                    () => _traceSource.TraceEvent(TraceEventType.Error, 0,
                        testName + " test: TraceSource.TraceEvent [TraceEventType.Error]."),
                        
                    // Direct listener methods call
                    () => _currentListener.Write(testName + " test: TextToLogFileTraceListener.Write"),
                    () => _currentListener.WriteLine(testName + " test: TextToLogFileTraceListener.WriteLine")
                };

            try
            {
                for (var i = 0; i < actionsToTest.Length; i++)
                {
                    CreateAndAttachNewListener(testName + i.ToString("00") + "_%datetime%.log");
                    actionsToTest[i]();
                    _currentListener.Close();
                }
            }
            finally
            {
                Trace.Listeners.Remove(_currentListener);
            }
        }

        private void CreateAndAttachNewListener(string fileName)
        {
            Trace.Listeners.Remove(_currentListener);

            CreateListenerAndAttachToSource(fileName, 10);

            Trace.Listeners.Add(_currentListener);
        }

        private void CreateListenerAndAttachToSource(string fileName, int fileSizeLimit)
        {
            _currentListener = new TextToLogFileTraceListener(@".\Logs", fileName, fileSizeLimit);
            _traceSource.Listeners.Clear();
            _traceSource.Listeners.Add(_currentListener);
        }

        [TestMethod, Owner(@"FIRM\alm")]
        [Ignore] // It's a manual test
        public void AutomaticLogFileSplittingWorksProperly()
        {
            var testName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            CreateListenerAndAttachToSource("LogfileSplittingTest.log", 1); // 1 megabyte file size limit

            ILoggerCodi logger = new Logger(_traceSource);

            logger.Info(testName, "A new first level log file is created");

            // Recreate listener with the same name
            _currentListener.Close();
            CreateListenerAndAttachToSource("LogfileSplittingTest.log", 1);

            logger.Info(testName, "Listener is recreated. Log file remains the same.");

            FloodLogFile(logger, @".\Logs\LogfileSplittingTest.log");

            logger.Info(testName, "This message should appear in newly created second level log file");

            // Recreate listener with the same name
            _currentListener.Close();
            CreateListenerAndAttachToSource("LogfileSplittingTest.log", 1);

            logger.Info(testName, "Listener is recreated. Log file remains the same.");

            // And once again
            FloodLogFile(logger, @".\Logs\LogfileSplittingTest.1.log");

            logger.Info(testName, "This message should appear in newly created third level log file");

            // Recreate listener with the same name
            _currentListener.Close();
            CreateListenerAndAttachToSource("LogfileSplittingTest.log", 1);

            logger.Info(testName, "Listener is recreated. Log file remains the same.");

            Assert.Fail(); // In order to prevent test results deletion by VS 2012.
        }

        private void FloodLogFile(ILoggerCodi logger, string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            const int fileSizeLimit = TextToLogFileTraceListener.Megabyte;

            while (fileInfo.Length < fileSizeLimit)
            {
                logger.Info("AutomaticLogfileSplittingWorksProperly", "Filling the log ...");
                fileInfo.Refresh();
            }

            // And once again in order to switch to the next log file
            logger.Info("AutomaticLogfileSplittingWorksProperly", "Filling the log ...");
        }
    }
}
