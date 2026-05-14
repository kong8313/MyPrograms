using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.TelephonyProblemStates.ProblemState;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Supervisor.Core.Activity;
using Confirmit.CATI.Common;
using Confirmit.CATI.Supervisor.Core.Export;
using Confirmit.CATI.Supervisor.Core.Export.CollectionProvider.SpecificProvider;
using Confirmit.CATI.Supervisor.Core.Common;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Supervisor.Core.UnitTests.Export
{
    [TestClass]
    public class CollectionExportProviderTest
    {
        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void InterviewerActivityExportProvider_TwoTaskActivityRow_TwoExportRecord()
        {            
            List<TaskActivityInfo> list = new List<TaskActivityInfo>();
            list.Add(GetTaskActivityInfo("p1"));
            list.Add(GetTaskActivityInfo("p2"));

            InterviewerActivityExportProvider exportProvider = new InterviewerActivityExportProvider(list);

            Assert.AreEqual(2, exportProvider.Count());                  
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void InterviewerActivityExportProvider_OneTaskActivityRow_ValidExportContext()
        {
            List<TaskActivityInfo> list = new List<TaskActivityInfo>();

            list.Add(GetTaskActivityInfo("p1234567", 
                                         "SurveyName", 
                                         InterviewState.INTERVIEWING, 
                                         0, 
                                         DialingMode.Manual));            

            InterviewerActivityExportProvider exportProvider = new InterviewerActivityExportProvider(list);            

            IExportRecordProvider recordProvider = exportProvider.First();

            Assert.AreEqual("p1234567", recordProvider["ProjectId"]);
            Assert.AreEqual("SurveyName", recordProvider["ProjectName"]);
            Assert.AreEqual(StringHelper.GetStringFromEnum(InterviewState.INTERVIEWING), recordProvider["InterviewState"]);
            Assert.AreEqual(new CatiProblemStateFactory(new CatiProblemStateInfo(string.Empty)).GetState(0).Message, recordProvider["ProblemState"]);
            Assert.AreEqual(StringHelper.GetStringFromEnum(DialingMode.Manual), recordProvider["DiallingMode"]);
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void AppointmentActivityExportProvider_TwoAppointmentActivityRow_TwoExportRecord()
        {
            List<AppointmentActivityInfo> list = new List<AppointmentActivityInfo>();
            list.Add(GetAppointmentActivityInfo("p1"));
            list.Add(GetAppointmentActivityInfo("p2"));


            AppointmentActivityExportProvider exportProvider = new AppointmentActivityExportProvider(list, false, 1);

            Assert.AreEqual(2, exportProvider.Count());
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void AppointmentActivityExportProvider_OneAppointmentActivityRow_ValidExportContext()
        {            
            List<AppointmentActivityInfo> list = new List<AppointmentActivityInfo>();
            list.Add(GetAppointmentActivityInfo("p1234567", "SurveyName", 1, 2, 3));

            AppointmentActivityExportProvider exportProvider = new AppointmentActivityExportProvider(list, true, 1);            

            IExportRecordProvider recordProvider = exportProvider.First();

            Assert.AreEqual("p1234567", recordProvider["ProjectID"]);
            Assert.AreEqual("SurveyName", recordProvider["ProjectName"]);
            Assert.AreEqual(1, recordProvider["SurveySID"]);
            Assert.AreEqual(2, recordProvider["InterviewID"]);
            Assert.AreEqual(3, recordProvider["CallID"]);                        
        }

        #region Private methods

        private TaskActivityInfo GetTaskActivityInfo(string projectId)
        {
            TaskActivityInfo info = new TaskActivityInfo()
            {
                ProjectId = projectId
            };

            return info;
        }

        private TaskActivityInfo GetTaskActivityInfo(string projectId,
                                                     string projectName,
                                                     InterviewState interviewState,
                                                     int problemState,
                                                     DialingMode diallingMode)
        {
            TaskActivityInfo info = new TaskActivityInfo()
            {
                ProjectId = projectId,
                ProjectName = projectName,
                InterviewState = interviewState,
                ProblemState = problemState,
                DiallingMode = diallingMode,
                TimeCallDelivered = null,
                LastKeepAliveTime = null
            };

            return info;
        }

        private AppointmentActivityInfo GetAppointmentActivityInfo(string projectId)
        {
            AppointmentActivityInfo info = new AppointmentActivityInfo()
            {
                ProjectID = projectId
            };

            return info;
        }

        private AppointmentActivityInfo GetAppointmentActivityInfo(string projectId,
                                                                   string projectName,
                                                                   int surveyId,
                                                                   int interviewId,
                                                                   int callId)
        {
            AppointmentActivityInfo info = new AppointmentActivityInfo()
            {
                ProjectID = projectId,
                ProjectName = projectName,
                SurveySID = surveyId,
                InterviewID = interviewId,
                CallID = callId
            };

            return info;
        }
        #endregion

    }
}
