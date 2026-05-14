using System.Data;

using Confirmit.CATI.Backend.Threads;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.BulkCopy
{
    [TestClass]
    public class BulkCopyTests
    {
        IntegrationTestingFramework framework = IntegrationTestingFramework.Instance;

        [TestInitialize]
        public void TestInitialize()
        {
            framework.TestInitialize();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void BulkCopyThread_BulkCopyInterviewerActivityEvents_Ok()
        {
            var databaseEngine = new DatabaseEngine(ServiceLocator.Resolve<IConnectionStrings>().ConfirmlogConnectionString);
            var count0 = databaseEngine.ExecuteScalar<int>("SELECT COUNT(*) FROM CatiInterviewerActivity", CommandType.Text);

            var confirmLogoutEvent = new ConfirmLogoutEvent();
            confirmLogoutEvent.Save(1);

            var forcedLogoutEvent = new ForcedLogoutEvent();
            forcedLogoutEvent.Save(1);

            var setPendingBreakEvent = new SetPendingBreakStatusEvent();
            setPendingBreakEvent.Save(
                1,
                2,
                "Foo Survey",
                PendingBreakStatus.Break,
                3,
                LoginState.LOGGING_IN,
                1);

            ServiceLocator.Resolve<BulkCopyThread>().BulkCopyInterviewerActivityEvents();

            var count1 = databaseEngine.ExecuteScalar<int>("SELECT COUNT(*) FROM CatiInterviewerActivity", CommandType.Text);
            Assert.AreEqual(count0, count1-3);
        }
    }
}
