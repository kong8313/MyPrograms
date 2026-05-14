using System.Globalization;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.ReportsTests
{
    [TestClass]
    public class AlertsHistoryReportTest : BaseMockedIntegrationTest
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();

            _personId = PersonTools.CreatePerson(PersonName, AgentTaskChoiceMode.Automatic);
        }

        private const string PersonName = "u1";
        private int _personId;

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void AlertsHistoryReport_PersonWithoutSurveys_ZeroTotalCount()
        {
            var count = 0;

            BvSpAlertsHistoryReportAdapter.ExecuteEntityList(
                _personId.ToString(CultureInfo.InvariantCulture), 
                null,
                "",
                1,
                50,
                "PersonId",
                false,
                out count);

            Assert.AreEqual(0, count);
        }
    }
}
