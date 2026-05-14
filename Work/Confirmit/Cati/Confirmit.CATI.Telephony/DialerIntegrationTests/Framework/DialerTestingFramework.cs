using System;
using System.Diagnostics;
using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Common.WcfTools;
using Confirmit.CATI.Common.WcfTools.ConsoleMessageHeader;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Fakes;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.Core.SystemSettings;
using ConfirmitDialerInterface;

namespace DialerIntegrationTests.Framework
{
    public class DialerTestingFramework
    {
        public DialerTestingFramework()
        {
            var serviceLocator = new ServiceLocator();

            serviceLocator.Cleanup();
            serviceLocator.Initialize();

            new SystemSettingUnitTestRegistrator().RegisterTypes(serviceLocator);

            ServiceLocator.Register<IDialerApiClient, StubIDialerApiClient>();
            ServiceLocator.Register<ISideBySideManager, SideBySideManager>();
            ServiceLocator.Register<IMessageHeaderAccessor, MessageHeaderAccessor>();
            ServiceLocator.Register<IAuthorizationMessageHeaderReader, AuthorizationMessageHeaderReader>();
            new SystemSettingUnitTestRegistrator().RegisterTypes(serviceLocator);
            ServiceLocator.Resolve<ISideBySideManager>().SideBySideName = "Test";
        }

        public void ExecuteTest(Action<ITestDialer> test)
        {
            ExecuteTest(test, true);
        }
        
        public void ExecuteTest(Action<ITestDialer> test, bool deleteStateFile)
        {
            var testName = GetTestName();

            using (var dialer = new TestCodiSimulatorDialer())
            {
                Trace.TraceInformation(""); // Just an empty line in the log for better reading
                Trace.TraceInformation($"{DateTimeAsString()}\t>>>>>>>>>>>>>>>>>>>>> DIALER TEST START. Dialer Type: [{dialer}], Test: [{testName}]");

                var timer = Stopwatch.StartNew();

                if (deleteStateFile)
                {
                    dialer.DeleteStateFile();
                }
                else
                {
                    dialer.LogStateFileExists();
                }

                dialer.ExpectDialerState(DialerState.Available);
                dialer.Init();
                dialer.WaitDialerStateNoticiation();

                test(dialer);
                dialer.Clear();

                timer.Stop();

                Trace.TraceInformation(
                    $" {DateTimeAsString()}\t<<<<<<<<<<<<<<<<<<<<< DIALER TEST FINISED. Dialer Type: [{dialer}], Test: [{testName}], Duration: [{TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds)}]\r\n\r\n");
            }
        }

        private static string GetTestName()
        {
            var stackTrace = new StackTrace(2, true);

            for (var i=0; i<3; i++)
            {
                var stackFrame = stackTrace.GetFrame(i);
                var methodName = stackFrame.GetMethod().Name;

                if (!methodName.StartsWith("ExecuteTest"))
                {
                    return stackFrame.ToString();
                }
            }

            return stackTrace.ToString();
        }

        private string DateTimeAsString()
        {
            // Currently we use following format: YYYY-MM-DD hh:mm:ss.mmm
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

    }
}