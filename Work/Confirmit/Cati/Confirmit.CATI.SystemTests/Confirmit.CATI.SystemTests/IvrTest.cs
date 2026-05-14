using System;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.SystemTestFramework;
using Confirmit.SystemTestFramework.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.SystemTests
{
    [TestClass]
    public class IvrTest : BaseSystemTests
    {
        [TestInitialize]
        public void Initialize()
        {
            TestsGroupName = "Ivr";

            TestInitialize();
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void VoiceXmlService_CorrectProcessingOfTwoInterviews()
        {
            var interviewsCount = 2;
            var file = SampleGenerator.Generate(interviewsCount, ColumnType.TelephoneNumber);

            ProjectId = Confirmit.Surveys.ImportFromFile(PathToSurvey);
            Confirmit.Surveys[ProjectId].Launch();
            Confirmit.Surveys[ProjectId].AddRespondents(file);
            Confirmit.Cati.Surveys[ProjectId].Open();
            var personSid = Confirmit.Cati.Interviewers.AddIvrAgent();
            Confirmit.Cati.Interviewers[personSid].AssignToSurvey(ProjectId);

            var pause = TimeSpan.FromSeconds(1);
            var startTime = DateTime.Now;
            var deadTime = startTime.AddMinutes(10);
            var interviewId = 1;
            var expectedStatus = 13;

            while (DateTime.Now < deadTime)
            {
                var status = Confirmit.Cati.Surveys[ProjectId].CallManagement.GetInterview(interviewId).TransientState;

                if (status == expectedStatus)
                {
                    if (interviewId == interviewsCount)
                    {
                        break;
                    }

                    interviewId++;
                }

                Thread.Sleep(pause);
            }

            if (interviewId != interviewsCount)
            {
                throw new TimeoutException("Calls were not processed by IVR agent in required time");
            }

            var task = Confirmit.Cati.ActivityViews.InterviewersList.GetTask(personSid);
            var agentStateIsNoCalls = task.InterviewState == (byte)InterviewState.NO_CALLS;
            Assert.IsTrue(agentStateIsNoCalls);

            CheckForEquality(ProjectId,
    "SELECT respid FROM respondent", @"
respid 
1      
2      ");
            CheckForEquality(ProjectId,
                "SELECT responseid,respid FROM response_control", @"
responseid respid 
1          1      
2          2      ");
            CheckForContent(ProjectId, "SELECT * FROM response0", "NULL", false);

            Confirmit.Cati.Interviewers[personSid].LockIvrAgent();
            Confirmit.Cati.Interviews.TerminateTaskByPerson(personSid);
            Confirmit.Cati.Interviewers[personSid].DeletePerson();

            Cleanup();
        }
    }
}