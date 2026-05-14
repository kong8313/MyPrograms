using System.Collections.Generic;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.SystemTestFramework;
using Confirmit.SystemTestFramework.Controllers;
using Confirmit.SystemTestFramework.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.SystemTests
{
    [TestClass]
    public class DialerNotifyInboundCallHandlerSystemTest : BaseSystemTests
    {
        [TestInitialize]
        public void Initialize()
        {
            TestsGroupName = "Inbound";

            TestInitialize();
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void Handler_new_nonpredictive_calls_should_make_new_respondent()
        {
            var inboundCallNumber = "+790222168861";
            var callerNumber = "+790222168862";
            var dialerId = 1;

            var interviewsCount = 2;
            var file = SampleGenerator.Generate(interviewsCount, ColumnType.TelephoneNumber);

            ProjectId = Confirmit.Surveys.ImportFromFile(PathToSurvey);
            Confirmit.Surveys[ProjectId].Launch();
            Confirmit.Surveys[ProjectId].AddRespondents(file);
            Confirmit.Cati.Surveys[ProjectId].Open();
            Thread.Sleep(2000);

            Confirmit.Cati.Surveys[ProjectId].SetInboundBehavior(InboundSurveyBehavior.MatchAndCreate);

            var surveyId = Confirmit.Cati.Surveys[ProjectId].Sid;
            Environment.Dialers[dialerId].SimulateInboundCall(surveyId, inboundCallNumber, callerNumber);

            List<Respondent> respondents;
            List<BvInterviewEntity> interviews;
            BvSvyScheduleEntity call = null;
            int attempt = 0;
            do
            {
                Thread.Sleep(1000);

                respondents = Confirmit.Surveys[ProjectId].RespondentsData.Get();
                interviews = Confirmit.Cati.Surveys[ProjectId].CallManagement.GetInterviews();
                if (interviews.Count > 2)
                {
                    call = Confirmit.Cati.Surveys[ProjectId].CallManagement.GetCall(interviews[2].ID);
                }

                attempt++;
            }
            while (attempt < 60 && (respondents.Count < 3 || interviews.Count < 3 || call != null));
            
            Assert.IsTrue(respondents.Count == 3 && interviews.Count == 3, string.Format("Looks like new respondent wasn't added. Count of respondents from confirmit database={0}, Count of interviews from our database={1}", respondents.Count, interviews.Count));

            Assert.AreEqual("1", respondents[0].Values["TelephoneNumber"].ToString());
            Assert.AreEqual("2", respondents[1].Values["TelephoneNumber"].ToString());
            Assert.AreEqual("+790222168862", respondents[2].Values["TelephoneNumber"].ToString());

            Assert.AreEqual("1", interviews[0].TelephoneNumber);
            Assert.AreEqual("2", interviews[1].TelephoneNumber);
            Assert.AreEqual("+790222168862", interviews[2].TelephoneNumber);

            Assert.IsNull(call, "Call was not removed");

            Cleanup();
        }
    }
}