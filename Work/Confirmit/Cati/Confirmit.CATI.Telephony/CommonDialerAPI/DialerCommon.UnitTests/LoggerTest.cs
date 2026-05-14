using System;
using System.Diagnostics;
using System.Collections.Generic;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.Telephony;
using DialerCommon.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ILoggerCodi = ConfirmitDialerInterface.ILogger;

namespace DialerCommon.UnitTests
{
    /// <summary>
    /// Summary description for LoggerTest
    /// </summary>
    [TestClass]
    public class LoggerTest
    {
        private readonly ILoggerCodi _logger = new Logger("DialerCommonTest");

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void TestPossibleCallsToLogger()
        {
            //Neither correct nor incorrect use cases must not throw any exceptions. move this comment higher

            //Correct use cases

#pragma warning disable 612,618
            _logger.WriteLine(TraceEventType.Information, "\n\nCORRECT USE CASES", "\n");
#pragma warning restore 612,618

            TestCaseWriteLine("Correct call. Test case 1.", "message: Correct call. Test case 1.");
            TestCaseWriteLine("Correct call. Test case 2.", "message: Correct call. Test case 2 [{0}, {1}, {2}].");

            _logger.Info("", ""); // Just an empty line separator
            TestCase("Correct call. Test case 3.", "message: Correct call. Test case 3 [{0}].", "param 0");
            TestCase("Correct call. Test case 4.", "message: Correct call. Test case 4 [{0}, {1}].", "param 0", "param 1");
            TestCase("Correct call. Test case 5.", "message: Correct call. Test case 5 [{0}].", "param 0", "param 1");
            TestCase("Correct call. Test case 6.", "message: Correct call. Test case 6.", new object[] { });
            TestCase("Correct call. Test case 7.", () => string.Format("message: Correct call. Test case 7 [{0}, {1}].", "param 0 ", "param 1"));
            TestCase("Correct call. Test case 8.", () => string.Format("message: Correct call. Test case 8.", new object[] { }));

            // Incorrect use cases

            _logger.Info("\n\nINCORRECT USE CASES", "\n");

            TestCase("Incorrect call. Test case 1.", "message: Incorrect call. Test case 1.", null);
            TestCase("Incorrect call. Test case 2.", "message: Incorrect call. Test case 2 [{0}].", null);
            TestCase("Incorrect call. Test case 3.", "message: Incorrect call. Test case 3 [{0}].", new object[] { });
            TestCase("Incorrect call. Test case 4.", "message: Incorrect call. Test case 4 [{0}].");
            TestCase("Incorrect call. Test case 5.", "message: Incorrect call. Test case 5 [{0}, {1}].", "param 0");
            TestCase("Incorrect call. Test case 6.", null, "param 0", "param 1");
            TestCase(null, "message: Incorrect call. Test case 7 [{0}, {1}].", "param 0", "param 1");

            TestCase("Incorrect call. Test case 8.", () => string.Format("message: Incorrect call. Test case 8.", null));
            TestCase("Incorrect call. Test case 9.", () => string.Format("message: Incorrect call. Test case 9 [{0}].", null));
            TestCase("Incorrect call. Test case 10.", () => string.Format("message: Incorrect call. Test case 10 [{0}].", new object[] { }));
            TestCase("Incorrect call. Test case 11.", () => string.Format("message: Incorrect call. Test case 11 [{0}]."));
            TestCase("Incorrect call. Test case 12.", () => string.Format("message: Incorrect call. Test case 12 [{0}, {1}].", "param 0"));
            TestCase("Incorrect call. Test case 13.", () => { throw new Exception("Test Exception (LoggerTest.TestPossibleCallsToLogger) "); });
            TestCase("Incorrect call. Test case 14.", () => null);
            TestCase(null, () => string.Format("message: Incorrect call. Test case 15 [{0}, {1}].", "param 0 ", "param 1"));
        }

        /// <summary>
        /// Tests WriteLine cases.
        /// The function is not just an overload but suffixed with 'WriteLine' in order 
        /// to avoid wrong overload call in some test cases
        /// </summary>
        /// <param name="sourceCodeLocation"></param>
        /// <param name="message"></param>
        private void TestCaseWriteLine(string sourceCodeLocation, string message)
        {
            foreach (TraceEventType eventType in Enum.GetValues(typeof(TraceEventType)))
            {
#pragma warning disable 612,618
                _logger.WriteLine(eventType, sourceCodeLocation, message);
#pragma warning restore 612,618        
            }

            _logger.Info("---------------------", ""); // Just a separator
        }

        private void TestCase(string sourceCodeLocation, string message, params object[] args)
        {
            _logger.Error(sourceCodeLocation, message, args);
            _logger.Warning(sourceCodeLocation, message, args);
            _logger.Info(sourceCodeLocation, message, args);
            _logger.Verbose(sourceCodeLocation, message, args);
            _logger.Info("---------------------", ""); // Just a separator
        }

        private void TestCase(string sourceCodeLocation, Func<string> messageFunc)
        {
            _logger.Error(sourceCodeLocation, messageFunc);
            _logger.Warning(sourceCodeLocation, messageFunc);
            _logger.Info(sourceCodeLocation, messageFunc);
            _logger.Verbose(sourceCodeLocation, messageFunc);
            _logger.Info("---------------------", ""); // Just a separator
        }

        [TestMethod, Owner(@"FIRM\alm"), Cr(74869)]
        public void UtcOffsetIsFormattedProperly()
        {
            var testCases = new Dictionary<TimeSpan, string>
                {
                    // Positive values
                    { new TimeSpan(3, 0, 0), "+3" },
                    { new TimeSpan(11, 0, 0), "+11" },
                    { new TimeSpan(5, 30, 0), "+530" },
                    { new TimeSpan(5, 45, 0), "+545" },
                    { new TimeSpan(10, 30, 0), "+1030" },

                    // Negative values
                    { new TimeSpan(-3, 0, 0), "-3" },
                    { new TimeSpan(-11, 0, 0), "-11" },
                    { new TimeSpan(-5, -30, 0), "-530" },
                    { new TimeSpan(-5, -45, 0), "-545" },
                    { new TimeSpan(-10, -30, 0), "-1030" }
                };

            var testCaseKey = default(TimeSpan);

            IUtcOffsetSource utcOffsetSource = new Logging.Fakes.StubIUtcOffsetSource
                {
// ReSharper disable AccessToModifiedClosure
                    Get = () => testCaseKey
// ReSharper restore AccessToModifiedClosure
                };

            var target = new UtcOffsetString(utcOffsetSource);

            foreach (var testCase in testCases)
            {
                testCaseKey = testCase.Key;
                Assert.AreEqual(testCase.Value, target.ToString(),
                    string.Format("Wrong conversion for [{0}]", testCaseKey));
            }
        }
    }
}
