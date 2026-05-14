using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.ServiceModel;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Random;
using Confirmit.CATI.Telephony;
using Confirmit.CATI.Telephony.DialerService;
using ConfirmitDialerInterface;
using DialerCommon.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialerIntegrationTests.Framework
{
    public abstract class TestDialer : IDisposable
    {
        private bool _disposed = false;  

        protected readonly ICommonLogger Log;

        protected string CompanyId = (Randomizer.Next(1, 1000)).ToString(CultureInfo.InvariantCulture);
        private string _dialerName;

        protected Uri TestDialerServiceUri;
        protected Uri TestDialerServiceUriWithSideBySide;
        protected Uri TestEventsHandlerServiceUri;

        protected ServiceHost TestDialerService;
        protected ServiceHost TestEventsHandlerService;

        public string ConnectionParametersXml { get; protected set; }

        protected string ExpectedNotificationMethod;
        protected AgentStateMsgs ExpectedUserState;
        protected CallOutcome ExpectedOutcome;

        protected readonly long CampaignId = Int32.MaxValue + (long)Randomizer.Next(10000000);

        protected const string ExtensionNumber = "12345";
        protected const int NotificationTimeout = 10000; // 10 seconds

        public abstract void StopSimulator();
        protected abstract void Release();

        protected TestDialer(string dialerName)
        {
            _dialerName = dialerName;

            Log = new Logger("IntTest" + _dialerName);

            TestDialerServiceUri =
                new Uri("http://localhost:80/Temporary_Listen_Addresses/Test" + _dialerName + "DialerService");

            TestDialerServiceUriWithSideBySide =
                new Uri("http://localhost:80/Temporary_Listen_Addresses.Test/Test" + _dialerName + "DialerService");

            TestEventsHandlerServiceUri =
                new Uri("http://localhost:80/Test/Temporary_Listen_Addresses/TestEventsHandlerService" + CompanyId);
        }

        protected void StartServices(Type type)
        {
            StartServices(new ServiceHost(type, TestDialerServiceUriWithSideBySide));
        }


        protected void StartServices(DialerService testDialerService)
        {
            StartServices(new ServiceHost(testDialerService, TestDialerServiceUriWithSideBySide));
        }

        private void StartServices(ServiceHost testDialerService)
        {
            TestDialerService = testDialerService;
            TestEventsHandlerService = new ServiceHost(typeof(TestDialerEventsHandlerService), TestEventsHandlerServiceUri);

            // Start both services simultaniously to speed up tests execution.
            Log.Info("TestDialer.StartServices", "Starting sevices: " +
                "[" + TestDialerServiceUriWithSideBySide.AbsoluteUri + "], " +
                "[" + TestEventsHandlerServiceUri.AbsoluteUri + "] ...");

            var waitHandles = new[]
            {
                TestDialerService.BeginOpen(null, null).AsyncWaitHandle,
                TestEventsHandlerService.BeginOpen(null, null).AsyncWaitHandle
            };

            //if (!WaitHandle.WaitAll(waitHandles, TimeSpan.FromSeconds(10)))
            // Note, WaitHandle.WaitAll is not supported here - System.NotSupportedException: WaitAll for multiple handles on a STA thread is not supported.
            // So we wait for each handle separately

            Assert.IsTrue(waitHandles[0].WaitOne(TimeSpan.FromSeconds(10)), "Service is not started: " + TestDialerServiceUriWithSideBySide.AbsoluteUri);
            Log.Info("TestDialer.StartServices", "Service is started: [{0}] ...", TestDialerServiceUriWithSideBySide.AbsoluteUri);

            Assert.IsTrue(waitHandles[1].WaitOne(TimeSpan.FromSeconds(10)), "Service is not started: " + TestEventsHandlerServiceUri.AbsoluteUri);
            Log.Info("TestDialer.StartServices", "Service is started: [{0}] ...", TestEventsHandlerServiceUri.AbsoluteUri);
        }

        public void LogStateFileExists()
        {
            LogStateFileExists(DialerServiceState.GetServiceStateFileFullPath());
        }

        private void LogStateFileExists(string stateFileFullPath)
        {
            Trace.TraceInformation("State file [{0}] exists: {1}", stateFileFullPath, File.Exists(stateFileFullPath));
        }

        public void DeleteStateFile()
        {
            var stateFileFullPath = DialerServiceState.GetServiceStateFileFullPath();

            LogStateFileExists(stateFileFullPath);

            try
            {
                File.Delete(stateFileFullPath);
                Trace.TraceInformation("State file [{0}] is deleted. Exists: {1}", stateFileFullPath,
                    File.Exists(stateFileFullPath));
            }
            catch (Exception ex)
            {
                Trace.TraceInformation(ex.ToString());
            }
        }

        private static string TimeStamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        protected static void TraceInformation(string source, string message)
        {
            Trace.TraceInformation("{0}, {1}: {2}", TimeStamp(), source, message);
        }

        private string ClassName()
        {
            return GetType().Name + "{child of TestDialer}";
        }

        public void Clear()
        {
            Log.Info(ClassName() + ".Clear", "");

            StopSimulator();
            Release();
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            var methodName = ClassName() + ".Dispose";

            Log.Info(methodName,
                "TestEventsHandlerService is [{0}], TestDialerService is [{1}] /// _disposed={2}, disposing={3}",
                (TestEventsHandlerService == null) ? "null" : "not null",
                (TestDialerService == null) ? "null" : "not null",
                _disposed, disposing);

            if (_disposed)
            {
                return;
            }

            if (!disposing)
            {
                return;
            }

            try
            {
                if (TestEventsHandlerService != null)
                {
                    Log.Info(methodName, "TestEventsHandlerService.Abort");

                    TestEventsHandlerService.Abort();
                }
            }
            catch (Exception ex)
            {
                Log.Error(methodName, ex.ToString());
            }

            try
            {
                if (TestDialerService != null)
                {
                    Log.Info(methodName, "TestDialerService.Abort");

                    TestDialerService.Abort();
                }
            }
            catch (Exception ex)
            {
                Log.Error(methodName, ex.ToString());
            }

            _disposed = true;
        }
    }
}
