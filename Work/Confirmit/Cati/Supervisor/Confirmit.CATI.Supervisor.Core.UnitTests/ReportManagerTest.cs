using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.Reports;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.Core.UnitTests
{
    [TestClass]
    public class ReportManagerTest
    {
        private List<BvSpAlertsHistoryAggregatedReportEntity> _alertsHistoryAggregatedReportdata;

        [TestInitialize]
        public void TestInitialize()
        {
            _alertsHistoryAggregatedReportdata = new List<BvSpAlertsHistoryAggregatedReportEntity>
            {
                new BvSpAlertsHistoryAggregatedReportEntity
                    {
                        PersonId = 1000,
                        PersonName = "p1",
                        AnswerSubmissionAmberCounts = 10,
                        AnswerSubmissionRedCounts = 3,
                        QuickAnswerSubmissionAmberCounts = 47,
                        QuickAnswerSubmissionRedCounts = 9,
                    }
            };
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void ConvertArrayToStringParameter_NullArray_ReturnsNull()
        {
            var result = ReportManager.ConvertArrayToStringParameter<int>(null);
            Assert.IsNull(result);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void ConvertArrayToStringParameter_EmptyArray_ReturnsNull()
        {
            var result = ReportManager.ConvertArrayToStringParameter<int>(new int[] {});
            Assert.IsNull(result);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void ConvertArrayToStringParameter_SingleElement_ReturnsElementAsString()
        {
            var result = ReportManager.ConvertArrayToStringParameter<int>(new []{25});
            Assert.AreEqual("25", result);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void ConvertArrayToStringParameter_3Element_ReturnsElementsSeparatedByComa()
        {
            var result = ReportManager.ConvertArrayToStringParameter<int>(new[] { 25, 30, 3 });
            Assert.AreEqual("25,30,3", result);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void ProcessAggregatedAlertsHistoryData_LastSubmissionAlert_ReturnsDataForLastSubmission()
        {
            var result = ReportManager.ProcessAggregatedAlertsHistoryData(
                _alertsHistoryAggregatedReportdata, InterviewerSubmissionAlert.LastSubmission);

            Assert.AreEqual(1, result.Count);
            var item = result.Single();

            Assert.AreEqual(1000, item.InterviewerId);
            Assert.AreEqual("p1", item.InterviewerName);
            Assert.AreEqual(10, item.AmberCount);
            Assert.AreEqual(3, item.RedCount);
            Assert.AreEqual(13, item.TotalCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void ProcessAggregatedAlertsHistoryData_QuickAnswerAlert_ReturnsDataForQuickAnswer()
        {
            var result = ReportManager.ProcessAggregatedAlertsHistoryData(
                _alertsHistoryAggregatedReportdata, InterviewerSubmissionAlert.QuickAnswer);

            Assert.AreEqual(1, result.Count);
            var item = result.Single();

            Assert.AreEqual(1000, item.InterviewerId);
            Assert.AreEqual("p1", item.InterviewerName);
            Assert.AreEqual(47, item.AmberCount);
            Assert.AreEqual(9, item.RedCount);
            Assert.AreEqual(56, item.TotalCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void ProcessAggregatedAlertsHistoryData_AllAlert_ReturnsSumForLastSubmissionAndQuickAnswer()
        {
            var result = ReportManager.ProcessAggregatedAlertsHistoryData(
                _alertsHistoryAggregatedReportdata, InterviewerSubmissionAlert.All);

            Assert.AreEqual(1, result.Count);
            var item = result.Single();

            Assert.AreEqual(1000, item.InterviewerId);
            Assert.AreEqual("p1", item.InterviewerName);
            Assert.AreEqual(10 + 47, item.AmberCount);
            Assert.AreEqual(3 + 9, item.RedCount);
            Assert.AreEqual(10 + 47 + 3 + 9, item.TotalCount);
        }
    }
}
