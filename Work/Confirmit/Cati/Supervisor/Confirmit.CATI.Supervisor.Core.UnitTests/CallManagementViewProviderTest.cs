using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.CallManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.Core.UnitTests
{
    [TestClass]
    public class CallManagementViewProviderTest
    {
        private CallManagementViewsProvider _callManagementViewProvider;        

        [TestInitialize]
        public void TestInitialize()
        {
            _callManagementViewProvider = new CallManagementViewsProvider();
            
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void GetDefaultViews_EmptyObject_DefaultCustomViewsInformationIsReturned()
        {
            var defaultViews = _callManagementViewProvider.GetDefaultViews();
            
            Assert.AreEqual(5, defaultViews.Views.Count);
            Assert.IsTrue(defaultViews.Views.All(x => x.Columns.All(y => y.IsVisible)));

            VerifyDefaultCustomViews(defaultViews, false);
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void MergeViews_DefaultViewsAndCustomViewsAreMergedCorrectly()
        {
            var defaultViews = _callManagementViewProvider.GetDefaultViews();

            var customViews = new CallManagementViews { Views = new List<CallManagementView>() };
            customViews.Views.Add(new CallManagementView
            {
                Name = "Test",
                Columns = new List<CallManagementColumn>
                {
                    new CallManagementColumn { ColumnKey = CallManagementColumnKey.ApptTimeText, IsVisible = true},
                    new CallManagementColumn { ColumnKey = CallManagementColumnKey.QuestionColumnsPosition, IsVisible = false}
                },
                IsDefault = false
            });

            var callManagementViews = _callManagementViewProvider.MergeViews(defaultViews, customViews);

            Assert.AreEqual(6, callManagementViews.Views.Count);

            VerifyDefaultCustomViews(callManagementViews);

            Assert.AreEqual("Test", callManagementViews.Views[5].Name);
            Assert.AreEqual(false, callManagementViews.Views[5].IsDefault);
            Assert.AreEqual(18, callManagementViews.Views[5].Columns.Count);
            Assert.AreEqual(true, callManagementViews.Views[5].Columns[0].IsVisible);
            Assert.AreEqual(CallManagementColumnKey.ApptTimeText, callManagementViews.Views[5].Columns[0].ColumnKey);
            Assert.AreEqual(false, callManagementViews.Views[5].Columns[1].IsVisible);
            Assert.AreEqual(CallManagementColumnKey.QuestionColumnsPosition, callManagementViews.Views[5].Columns[1].ColumnKey);
            Assert.AreEqual(false, callManagementViews.Views[5].Columns[17].IsVisible);
            Assert.AreEqual(CallManagementColumnKey.CallState, callManagementViews.Views[5].Columns[17].ColumnKey);
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void MergeViews_CustomViewContainsImpossibleColumnsKey_WrongColumnIsRemovedFromMergedView()
        {
            var defaultViews = _callManagementViewProvider.GetDefaultViews();

            var customViews = new CallManagementViews { Views = new List<CallManagementView>() };
            customViews.Views.Add(new CallManagementView
            {
                Name = "Test",
                Columns = new List<CallManagementColumn>
                {
                    new CallManagementColumn { ColumnKey = CallManagementColumnKey.ApptTimeText, IsVisible = true},
                    new CallManagementColumn { ColumnKey = (CallManagementColumnKey)100, IsVisible = true},
                    new CallManagementColumn { ColumnKey = CallManagementColumnKey.QuestionColumnsPosition, IsVisible = false},
                },
                IsDefault = false
            });

            var callManagementViews = _callManagementViewProvider.MergeViews(defaultViews, customViews);

            Assert.AreEqual(6, callManagementViews.Views.Count);

            VerifyDefaultCustomViews(callManagementViews);

            Assert.AreEqual("Test", callManagementViews.Views[5].Name);
            Assert.AreEqual(false, callManagementViews.Views[5].IsDefault);
            Assert.AreEqual(18, callManagementViews.Views[5].Columns.Count);
            Assert.AreEqual(true, callManagementViews.Views[5].Columns[0].IsVisible);
            Assert.AreEqual(CallManagementColumnKey.ApptTimeText, callManagementViews.Views[5].Columns[0].ColumnKey);
            Assert.AreEqual(false, callManagementViews.Views[5].Columns[1].IsVisible);
            Assert.AreEqual(CallManagementColumnKey.QuestionColumnsPosition, callManagementViews.Views[5].Columns[1].ColumnKey);
            Assert.AreEqual(false, callManagementViews.Views[5].Columns[17].IsVisible);
            Assert.AreEqual(CallManagementColumnKey.CallState, callManagementViews.Views[5].Columns[17].ColumnKey);
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void MergeViews_CustomViewIsDefaultView_ScheduledViewIsNotDefault()
        {
            var defaultViews = _callManagementViewProvider.GetDefaultViews();

            var customViews = new CallManagementViews { Views = new List<CallManagementView>() };
            customViews.Views.Add(new CallManagementView
            {
                Name = "Test",
                Columns = new List<CallManagementColumn>
                {
                    new CallManagementColumn { ColumnKey = CallManagementColumnKey.ApptTimeText, IsVisible = true},
                    new CallManagementColumn { ColumnKey = CallManagementColumnKey.QuestionColumnsPosition, IsVisible = false}
                },
                IsDefault = true
            });

            var callManagementViews = _callManagementViewProvider.MergeViews(defaultViews, customViews);

            Assert.AreEqual(6, callManagementViews.Views.Count);

            VerifyDefaultCustomViews(callManagementViews, false);

            Assert.AreEqual("Test", callManagementViews.Views[5].Name);
            Assert.AreEqual(true, callManagementViews.Views[5].IsDefault);
            Assert.AreEqual(18, callManagementViews.Views[5].Columns.Count);
            Assert.AreEqual(true, callManagementViews.Views[5].Columns[0].IsVisible);
            Assert.AreEqual(CallManagementColumnKey.ApptTimeText, callManagementViews.Views[5].Columns[0].ColumnKey);
            Assert.AreEqual(false, callManagementViews.Views[5].Columns[1].IsVisible);
            Assert.AreEqual(CallManagementColumnKey.QuestionColumnsPosition, callManagementViews.Views[5].Columns[1].ColumnKey);
            Assert.AreEqual(false, callManagementViews.Views[5].Columns[17].IsVisible);
            Assert.AreEqual(CallManagementColumnKey.CallState, callManagementViews.Views[5].Columns[17].ColumnKey);
        }

        private void VerifyDefaultCustomViews(CallManagementViews defaultViews, bool isScheduledDefault = true)
        {
            Assert.AreEqual("Scheduled", defaultViews.Views[0].Name);
            Assert.AreEqual("High priority", defaultViews.Views[1].Name);
            Assert.AreEqual("Not Scheduled", defaultViews.Views[2].Name);
            Assert.AreEqual("All", defaultViews.Views[3].Name);
            Assert.AreEqual("Sent to dialer", defaultViews.Views[4].Name);

            Assert.AreEqual(isScheduledDefault, defaultViews.Views[0].IsDefault);
            Assert.AreEqual(false, defaultViews.Views[1].IsDefault);
            Assert.AreEqual(false, defaultViews.Views[2].IsDefault);
            Assert.AreEqual(false, defaultViews.Views[3].IsDefault);
            Assert.AreEqual(false, defaultViews.Views[4].IsDefault);

            Assert.AreEqual(18, defaultViews.Views[0].Columns.Count);
            Assert.AreEqual(17, defaultViews.Views[1].Columns.Count);
            Assert.AreEqual(10, defaultViews.Views[2].Columns.Count);
            Assert.AreEqual(15, defaultViews.Views[3].Columns.Count);
            Assert.AreEqual(17, defaultViews.Views[4].Columns.Count);

            Assert.AreEqual(CallManagementColumnKey.InterviewID, defaultViews.Views[0].Columns[0].ColumnKey);
            Assert.AreEqual(CallManagementColumnKey.CallState, defaultViews.Views[0].Columns[17].ColumnKey);

            Assert.AreEqual(CallManagementColumnKey.InterviewID, defaultViews.Views[1].Columns[0].ColumnKey);
            Assert.AreEqual(CallManagementColumnKey.ExpTimeText, defaultViews.Views[1].Columns[16].ColumnKey);

            Assert.AreEqual(CallManagementColumnKey.InterviewID, defaultViews.Views[2].Columns[0].ColumnKey);
            Assert.AreEqual(CallManagementColumnKey.LastCallTimeText, defaultViews.Views[2].Columns[9].ColumnKey);

            Assert.AreEqual(CallManagementColumnKey.InterviewID, defaultViews.Views[3].Columns[0].ColumnKey);
            Assert.AreEqual(CallManagementColumnKey.ReviewStatus, defaultViews.Views[3].Columns[14].ColumnKey);

            Assert.AreEqual(CallManagementColumnKey.InterviewID, defaultViews.Views[4].Columns[0].ColumnKey);
            Assert.AreEqual(CallManagementColumnKey.ExpTimeText, defaultViews.Views[4].Columns[16].ColumnKey);
        }
    }
}