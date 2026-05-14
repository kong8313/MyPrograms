using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.Supervisor.Core.Surveys;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.IntegrationTests.Tests.AsyncOperations
{
    [TestClass]
    public class EnableCallsAsyncOperationTest : BaseMockedIntegrationTest
    {
        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
        }
        
        [TestMethod]
        public void DisableCalls_CallAlreadyDisabled_CallHistoryRecordsAreNotLogged()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", DialMode = DialingMode.Predictive, IsOpen = true,
                    Interviews = new[] {
                            new InterviewData(){ Tag="S1.I1", Call = new CallData(){CallState = (int)CallState.LoadedToDialerPredictively}},
                            new InterviewData(){ Tag="S1.I2", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData(){ Tag="S1.I3", Call = new CallData(){CallState = (int)CallState.Scheduled}}
            }}}
            }.Create();

            var survey = context.GetSurvey("S1");

            var operation = CallManager.EnableCalls(survey.Id, false,
                new SelectedBatchParameters(context.GetInterviews("S1.I1", "S1.I2", "S1.I3").Select(x => x.Id)));

            // act
            ServiceLocator.Resolve<IAsyncOperationExecutor>().ExecuteOperationSync(operation);

            Assert.AreEqual(1, GetCallHistoryRecords(context.GetInterview("S1.I1")).Length);
            Assert.AreEqual(2, GetCallHistoryRecords(context.GetInterview("S1.I2")).Length);
            Assert.AreEqual((byte)OperationType.DisableCalls, GetCallHistoryRecords(context.GetInterview("S1.I2"))[0].OperationType);
            Assert.AreEqual((byte)OperationType.DisableCalls, GetCallHistoryRecords(context.GetInterview("S1.I2"))[1].OperationType);
            Assert.AreEqual(1, GetCallHistoryRecords(context.GetInterview("S1.I3")).Length);

            context.GetCalls("S1.I1", "S1.I2", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByUser);
        }

        [TestMethod]
        public void EnableCalls_CallAlreadyDisabled_CallHistoryRecordsAreNotLogged()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", DialMode = DialingMode.Predictive, IsOpen = true,
                    Interviews = new[] {
                            new InterviewData(){ Tag="S1.I1", Call = new CallData(){CallState = (int)CallState.LoadedToDialerPredictively}},
                            new InterviewData(){ Tag="S1.I2", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData(){ Tag="S1.I3", Call = new CallData(){CallState = (int)CallState.Scheduled}}
            }}}
            }.Create();

            var survey = context.GetSurvey("S1");

            var operation = CallManager.EnableCalls(survey.Id, true,
                new SelectedBatchParameters(context.GetInterviews("S1.I1", "S1.I2", "S1.I3").Select(x => x.Id)));

            // act
            ServiceLocator.Resolve<IAsyncOperationExecutor>().ExecuteOperationSync(operation);

            Assert.AreEqual(0, GetCallHistoryRecords(context.GetInterview("S1.I1")).Length);
            GetCallHistoryRecords(context.GetInterview("S1.I2")).Single(x => x.OperationType == (byte)OperationType.EnableCalls);
            Assert.AreEqual(0, GetCallHistoryRecords(context.GetInterview("S1.I3")).Length);

            context.GetCalls("S1.I1").Assert.IsTrue(x => x.CallState == (int)CallState.LoadedToDialerPredictively);
            context.GetCalls("S1.I2", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        private BvCallHistoryExEntity[] GetCallHistoryRecords(InterviewController interview)
        {
            return BvCallHistoryExAdapter.GetByCondition("SurveyId = @SurveyId AND InterviewId = @InterviewId", 
                new SqlParameter("@SurveyId", interview.Survey.Id),
                new SqlParameter("@InterviewId", interview.Id)).ToArray();
        }
    }
}
