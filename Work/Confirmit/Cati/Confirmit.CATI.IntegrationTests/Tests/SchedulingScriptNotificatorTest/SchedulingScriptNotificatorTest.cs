using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.IntegrationTests.Framework;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SqlServer.Management.Smo;

using Confirmit.CATI.Common;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Mail;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Mail.Fakes;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Backend.Threads;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.IntegrationTests.Tests.SchedulingScriptNotificatorTest
{
    /// <summary>
    /// Summary description for SchedulingScriptNotificationServiceIntegrationTest
    /// </summary>
    [TestClass]
    public class SchedulingScriptNotificatorTest : BaseMockedIntegrationTest
    {
        public TestContext TestContext { get; set; }
        
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private const int BatchId = 2;
        private const int RecordsCount = 4;

        private string _projectId;
        private DatabaseEngine _confirmitDb;
        private const string ToEmail = "test@firmsw.no";
        private readonly IEnumerable<int> _timeZones = Enumerable.Range(1, RecordsCount);
        private const int StartRespId = 1;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();

            _confirmitDb = ConfirmitTools.GetConfirmitSurveyDbOnTest(out _projectId);
            new BackendTools(_framework);
        }


        private TableInfo[] GetTestDataWithQ1Q2()
        {
            var c1 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 3, Name = "q1", QuotaIds = null };
            var c2 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 4, Name = "q2", QuotaIds = null };
            var c3 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 32, Name = "CallAttemptCount", QuotaIds = null };
            var p1 = new ColumnInfo { DataType = SqlDataType.Int, Name = "responseid" };
            var p2 = new ColumnInfo { DataType = SqlDataType.Int, Name = "respid" };

            var t1 = new TableInfo { Name = "response1", ReplicationColumns = new[] { c1, c2 }, PrimaryKeyColumns = new[] { p1 } };
            var t2 = new TableInfo { Name = "respondent", ReplicationColumns = new[] { c3 }, PrimaryKeyColumns = new[] { p2 } };

            return new[] { t1, t2 };
        }

        private void AddSample_TestWithScript(TestScript script, IEnumerable<string> expectedMessages)
        {
            List<MailMessage> messages = new List<MailMessage>();
            var stubIMailSender = new StubIMailSender
            {
                SendMailMailMessage = mailMessage => { messages.Add(mailMessage); }
            };
            ServiceLocator.RegisterInstance<IMailSender>(stubIMailSender);

            BackendToolsObject.CreateSurvey(script, _projectId, _confirmitDb.ConnectionString);
            BvSurveyEntity survey = SurveyRepository.GetByName(_projectId);
            survey.NotificationEmail = ToEmail;
            SurveyRepository.Update(survey);

            var testData = GetTestDataWithQ1Q2();
            BackendTools.EnableChangeTracking(_confirmitDb, testData);
            new ManagementService().UpdateSurveyReplicationScheme(_projectId, testData);

            BackendToolsObject.AddSample(
                    _projectId,
                    BatchId,
                    (int)SchedulingMode.Full, StartRespId, RecordsCount, _timeZones);

            Assert.AreEqual(1, messages.Count, "Messages count is wrong.");

            Assert.IsTrue(messages[0].To.ToString().Contains(ToEmail), "The Email To address is not corrrectly added.");

            Assert.IsNotNull(messages[0].Subject, "The message subject is not defined.");
            TestContext.WriteLine("The message subject:");
            TestContext.WriteLine(messages[0].Subject);

            Assert.IsTrue(messages[0].Subject.Contains(_projectId) && messages[0].Subject.Contains(_projectId));

            Assert.IsNotNull(messages[0].Body, "The message body is not defined");
            TestContext.WriteLine("The message body:");
            TestContext.WriteLine(messages[0].Body);

            foreach (string ts in expectedMessages)
            {
                StringAssert.Contains(messages[0].Body, ts,
                    String.Format("The error message '{0}' is missed in the message body.", ts));
            }
        }


        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void AddSample_ErrorInFullScheduling_NotificationEmailIsSent()
        {
            const string testExceptionMessage = "SetNewITS My test error";
            var script = new TestScript(
                    new Action(
                        Action.Operation.SetNewITS,
                        "15",
                        String.Format("throw new UserMessageException(\"{0}\");return false;", testExceptionMessage)),
                    @"Scheduling2007\Schedule.xml");

            AddSample_TestWithScript(script, new[] { testExceptionMessage });

        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void AddSample_SetErrorInFullScheduling_NotificationEmailIsSent()
        {
            const string testExceptionMessage = "q1";
            var script = new TestScript(
                    new Action(
                        Action.Operation.SetNewITS,
                        "15",
                        String.Format("fr('{0}').setValue('2') == 2", testExceptionMessage)),
                    @"Scheduling2007\Schedule.xml");

            AddSample_TestWithScript(script, new[] { testExceptionMessage, "update" });
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void AddSample_ReplicatedVariableGetErrorInFullScheduling_NotificationEmailIsSent()
        {
            const string testExceptionMessage = "Not Existing Variable";
            var script = new TestScript(
                    new Action(
                        Action.Operation.SetNewITS,
                        "15",
                        String.Format("fr('{0}').get() == 1", testExceptionMessage)),
                    @"Scheduling2007\Schedule.xml");

            AddSample_TestWithScript(script, new[] { testExceptionMessage, "found" });
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void AddSample_InterviewVariableGetErrorInFullScheduling_NotificationEmailIsSent()
        {
            const string testExceptionMessage = "Not Existing Variable";
            var script = new TestScript(
                    new Action(
                        Action.Operation.RecallAfterNumberOfShiftsSpecifiedByVariable,
                        testExceptionMessage),
                    @"Scheduling2007\Schedule.xml");

            AddSample_TestWithScript(script, new[] { testExceptionMessage, "found" });
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void ManagementServiceTest_SaveInterviewHistoryAndControlData_ErrorInSchedulingScript_NotificationEmailIsSent()
        {
            string userName = Guid.NewGuid().ToString();
            string password = userName;

            MailMessage message = null;
            var stubIMailSender = new StubIMailSender
            {
                SendMailMailMessage = mailMessage => { message = mailMessage; }
            };
            ServiceLocator.RegisterInstance<IMailSender>(stubIMailSender);

            const string testExceptionMessage = "SetNewITS My test error";
            var script = new TestScript(
                    new Action(
                        Action.Operation.SetNewITS,
                        "15",
                        String.Format("throw new UserMessageException(\"{0}\");return false;", testExceptionMessage)),
                    @"Scheduling2007\Schedule.xml");



            var test = new TestCati2(true, false, BackendToolsObject);
            test.CreateSurveyWithPerson(DialingMode.Preview, userName, password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            new DatabaseEngine().ExecuteNonQuery("UPDATE BvInterview SET BatchId = 1");

            var survey = SurveyRepository.GetById(test.SurveySID);
            survey.NotificationEmail = ToEmail;
            SurveyRepository.Update(survey);
            BackendToolsObject.LaunchScript(test.SurveySID, script);

            test.Login(userName, password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer("1234");

            //start interview
            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);

            test.ReplyOnInterview_Progressive(interview);

            test.WS.WrapUp(interview.ID, false, 1, new CompletedInterviewDetails());

            Assert.AreEqual(1, GetNotSentErrorsCount());

            ServiceLocator.Resolve<ScheduleErrorsNotificationThread>().DoWork();

            Assert.AreEqual(0, GetNotSentErrorsCount());

            Assert.IsNotNull(message, "Message was not created.");

            Assert.IsTrue(message.To.ToString().Contains(ToEmail), "The Email To address is not corrrectly added.");

            Assert.IsNotNull(message.Subject, "The message subject is not defined.");
            TestContext.WriteLine("The message subject:");
            TestContext.WriteLine(message.Subject);

            Assert.IsTrue(message.Subject.Contains(test.SurveyName) && message.Subject.Contains(test.SurveyName));

            Assert.IsNotNull(message.Body, "The message body is not defined");
            TestContext.WriteLine("The message body:");
            TestContext.WriteLine(message.Body);

            StringAssert.Contains(message.Body, testExceptionMessage,
                    String.Format("The error message '{0}' is missed in the message body.", testExceptionMessage));

        }

        [TestMethod, Owner(@"FIRM\DmitryS")]
        public void TwoFulfillAppointmentActionsInARow_ErrorSent()
        {
            var testExceptionMessage = "Action \"Fulfill specified appointment\": No appointment is set. The appointment created in a previous call attempt has already been fulfilled";

            DateTime appointermtTime = DateTime.Parse("2010-01-25T10:00:00");

            string userName = Guid.NewGuid().ToString();

            MailMessage message = null;
            var stubIMailSender = new StubIMailSender
            {
                SendMailMailMessage = mailMessage => { message = mailMessage; }
            };
            ServiceLocator.RegisterInstance<IMailSender>(stubIMailSender);

            var script = new TestScript(
                    new[]
                    {
                        new SubRule(
                            new[]
                            {
                                new Action(Action.Operation.FulfillTheSpecifiedAppointment, "0" )
                            })
                    },
                    new Shift(1, 1, "1.00:00:00", "0.00:00:00"));

            var test = new TestCati2(true, false, BackendToolsObject);
            var SID = test.CreateSurvey(script);
            var interviews = test.CreateInterviewsWithCalls(1);

            var survey = SurveyRepository.GetById(SID);
            survey.NotificationEmail = ToEmail;

            BackendTools.AddAppointment(interviews[0].ID, SID, appointermtTime);
            // Fulfills specified appointment
            BackendTools.FireEvent(new BvInterviewWithOriginEntity(interviews[0]));
            // Error appointment already fulfilled
            BackendTools.FireEvent(new BvInterviewWithOriginEntity(interviews[0]));

            Assert.AreEqual(2, GetNotSentErrorsCount());

            ServiceLocator.Resolve<ScheduleErrorsNotificationThread>().DoWork();

            Assert.AreEqual(0, GetNotSentErrorsCount());

            Assert.IsNotNull(message);
            StringAssert.Contains(message.Body, testExceptionMessage);
        }

        private int GetNotSentErrorsCount()
        {
            return ServiceLocator.Resolve<IScheduleErrorRepository>().GetNotSentErrors().Count();
        }
    }
}
