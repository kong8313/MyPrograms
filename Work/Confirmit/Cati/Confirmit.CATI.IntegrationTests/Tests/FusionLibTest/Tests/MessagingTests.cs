using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Supervisor.Core.Messaging;

namespace Confirmit.CATI.IntegrationTests.Tests.FusionLibTest.Tests
{
    /// <summary>
    /// Summary description for MessagingTests
    /// </summary>
    [TestClass]
    public class MessagingTests
    {
        const string UserName1 = "User1";
        const string UserName2 = "User2";
        const string Password = "password";

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private int _surveySID;

        private ISurveyStateService _surveyStateService;

        #region Initialize and Cleanup methods

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);

            _surveySID = SurveyRepository.Insert(new BvSurveyEntity
            {
                Name = BackendTools.GenerateSurveyName(),
                Description = "",
                DialMode = (byte)DialingMode.Manual,
            });
            _backendTools.LaunchAllHoursScript();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            _surveyStateService.Open(_surveySID);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        #endregion

        

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void SendOnlineMessage2OneOfTwoInterviewerGetMessageForEachInterviwer_InterviewersAreLogged_OneCorrectMessageIsReceived()
        {
            var interviewers = new List<int>();

            int user1ID = PersonTools.CreatePerson(UserName1, Password, AgentTaskChoiceMode.Manual);
            int user2ID = PersonTools.CreatePerson(UserName2, Password, AgentTaskChoiceMode.Manual);

            BackendTools.LoginPerson(user1ID, "");
            BackendTools.LoginPerson(user2ID, "");

            interviewers.Add(user1ID);

            var message = new Message
            {
                Body = "Message body",
                SupervisorName = "Supervisor name"
            };

            SendMessageManager.SendMessageToInterviewers(interviewers, true, message);

            var consoleHelper1 = new CatiWsHelper(UserName1, Password);
            Messages[] messageArray1 = consoleHelper1.ConsoleService.GetMessages();

            //When we call any WS method PersonId is stored in CallContext each time
            //When ConsoleServiceHelpers are created in one thread for several users CallContext will contain PersonId for last user.
            var consoleHelper2 = new CatiWsHelper(UserName2, Password);
            Messages[] messageArray2 = consoleHelper2.ConsoleService.GetMessages();

            Assert.AreEqual(1, messageArray1.Length);
            Assert.AreEqual(0, messageArray2.Length);

            Assert.AreEqual("Message body", messageArray1[0].Body);
            Assert.AreEqual("Supervisor name", messageArray1[0].SupervisorName);
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void SendOnlineMessage2TwoInterviewersGetMessageForEachInterviwer_InterviewersAreLogged_CorrectMessagesAreReceived()
        {
            var interviewers = new List<int>();

            int user1ID = PersonTools.CreatePerson(UserName1, Password, AgentTaskChoiceMode.Manual);
            int user2ID = PersonTools.CreatePerson(UserName2, Password, AgentTaskChoiceMode.Manual);

            BackendTools.LoginPerson(user1ID, "");
            BackendTools.LoginPerson(user2ID, "");

            interviewers.Add(user1ID); interviewers.Add(user2ID);

            var message = new Message
            {
                Body = "Message body",
                SupervisorName = "Supervisor name"
            };

            SendMessageManager.SendMessageToInterviewers(interviewers, true, message);

            var consoleHelper1 = new CatiWsHelper(UserName1, Password);
            Messages[] messageArray1 = consoleHelper1.ConsoleService.GetMessages();

            //When we call any WS method PersonId is stored in CallContext each time
            //When ConsoleServiceHelpers are created in one thread for several users CallContext will contain PersonId for last user.
            var consoleHelper2 = new CatiWsHelper(UserName2, Password);
            Messages[] messageArray2 = consoleHelper2.ConsoleService.GetMessages();

            Assert.AreEqual(1, messageArray1.Length);
            Assert.AreEqual(1, messageArray2.Length);

            Assert.AreEqual("Message body", messageArray1[0].Body);
            Assert.AreEqual("Message body", messageArray2[0].Body);

            Assert.AreEqual("Supervisor name", messageArray1[0].SupervisorName);
            Assert.AreEqual("Supervisor name", messageArray2[0].SupervisorName);
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void SendOfflineMessage2TwoInterviewersGetMessageForEachInterviwer_InterviewersAreNotLogged_CorrectMessagesAreReceived()
        {
            var interviewers = new List<int>();

            int user1ID = PersonTools.CreatePerson(UserName1, Password, AgentTaskChoiceMode.Manual);
            int user2ID = PersonTools.CreatePerson(UserName2, Password, AgentTaskChoiceMode.Manual);

            interviewers.Add(user1ID); interviewers.Add(user2ID);

            var message = new Message
            {
                Body = "Message body",
                SupervisorName = "Supervisor name"
            };

            SendMessageManager.SendMessageToInterviewers(interviewers, false, message);

            BackendTools.LoginPerson(user1ID, "");
            BackendTools.LoginPerson(user2ID, "");

            var consoleHelper1 = new CatiWsHelper(UserName1, Password);
            Messages[] messageArray1 = consoleHelper1.ConsoleService.GetMessages();

            //When we call any WS method PersonId is stored in CallContext each time
            //When ConsoleServiceHelpers are created in one thread for several users CallContext will contain PersonId for last user.
            var consoleHelper2 = new CatiWsHelper(UserName2, Password);
            Messages[] messageArray2 = consoleHelper2.ConsoleService.GetMessages();

            Assert.AreEqual(1, messageArray1.Length);
            Assert.AreEqual(1, messageArray2.Length);
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void SendTwoOnlineMessage2FirstInterviewerGetMessageForEachInterviwer_InterviewersAreLogged_TwoMessagesForFirstNoMessagesForSecondAreReceived()
        {
            var interviewers = new List<int>();

            int user1ID = PersonTools.CreatePerson(UserName1, Password, AgentTaskChoiceMode.Manual);
            int user2ID = PersonTools.CreatePerson(UserName2, Password, AgentTaskChoiceMode.Manual);

            BackendTools.LoginPerson(user1ID, "");
            BackendTools.LoginPerson(user2ID, "");

            interviewers.Add(user1ID);

            var message = new Message
            {
                Body = "body1",
                SupervisorName = "Supervisor name"
            };

            SendMessageManager.SendMessageToInterviewers(interviewers, true, message);

            message.Body = "body2";

            SendMessageManager.SendMessageToInterviewers(interviewers, true, message);

            var consoleHelper1 = new CatiWsHelper(UserName1, Password);
            Messages[] messageArray1 = consoleHelper1.ConsoleService.GetMessages();

            //When we call any WS method PersonId is stored in CallContext each time
            //When ConsoleServiceHelpers are created in one thread for several users CallContext will contain PersonId for last user.
            var consoleHelper2 = new CatiWsHelper(UserName2, Password);
            Messages[] messageArray2 = consoleHelper2.ConsoleService.GetMessages();

            Assert.AreEqual(2, messageArray1.Length);
            Assert.AreEqual(0, messageArray2.Length);

            Assert.AreEqual("body1", messageArray1[0].Body);
            Assert.AreEqual("body2", messageArray1[1].Body);

        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void SendTwoOnlineMessage2TwoInterviewerGetMessageForEachInterviwer_FirstInitiallyLoggedSecondLogggedAfterFirstMessage_TwoMessagesForFirstOneMessagesForSecondAreReceived()
        {
            var interviewers = new List<int>();

            int user1ID = PersonTools.CreatePerson(UserName1, Password, AgentTaskChoiceMode.Manual);
            int user2ID = PersonTools.CreatePerson(UserName2, Password, AgentTaskChoiceMode.Manual);

            BackendTools.LoginPerson(user1ID, "");

            interviewers.Add(user1ID); interviewers.Add(user2ID);

            var message = new Message
            {
                Body = "body1",
                SupervisorName = "Supervisor name"
            };

            SendMessageManager.SendMessageToInterviewers(interviewers, true, message);

            BackendTools.LoginPerson(user2ID, "");

            message.Body = "body2";

            SendMessageManager.SendMessageToInterviewers(interviewers, true, message);

            var consoleHelper1 = new CatiWsHelper(UserName1, Password);
            Messages[] messageArray1 = consoleHelper1.ConsoleService.GetMessages();

            //When we call any WS method PersonId is stored in CallContext each time
            //When ConsoleServiceHelpers are created in one thread for several users CallContext will contain PersonId for last user.
            var consoleHelper2 = new CatiWsHelper(UserName2, Password);
            Messages[] messageArray2 = consoleHelper2.ConsoleService.GetMessages();

            Assert.AreEqual(2, messageArray1.Length);
            Assert.AreEqual(1, messageArray2.Length);

            Assert.AreEqual("body1", messageArray1[0].Body);
            Assert.AreEqual("body2", messageArray1[1].Body);

            Assert.AreEqual("body2", messageArray2[0].Body);

        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void SendMessage2GroupWithTwoInterviewersGetMessageForEachInterviwer_InterviewersAreLogged_CorrectMessagesAreReceived()
        {
            var groups = new List<int>();

            int groupID = PersonTools.CreatePersonGroup("Test group");

            groups.Add(groupID);

            int user1ID = PersonTools.CreatePerson(UserName1, Password, AgentTaskChoiceMode.Manual, groups.ToArray());
            int user2ID = PersonTools.CreatePerson(UserName2, Password, AgentTaskChoiceMode.Manual, groups.ToArray());

            BackendTools.LoginPerson(user1ID, "");
            BackendTools.LoginPerson(user2ID, "");

            var message = new Message
            {
                Body = "Message body",
                SupervisorName = "Supervisor name"
            };

            SendMessageManager.SendMessageToGroups(groups.Select(x => x), true, message);

            var consoleHelper1 = new CatiWsHelper(UserName1, Password);
            Messages[] messageArray1 = consoleHelper1.ConsoleService.GetMessages();

            //When we call any WS method PersonId is stored in CallContext each time
            //When ConsoleServiceHelpers are created in one thread for several users CallContext will contain PersonId for last user.
            var consoleHelper2 = new CatiWsHelper(UserName2, Password);
            Messages[] messageArray2 = consoleHelper2.ConsoleService.GetMessages();

            Assert.AreEqual(1, messageArray1.Length);
            Assert.AreEqual(1, messageArray2.Length);

            Assert.AreEqual("Message body", messageArray1[0].Body);
            Assert.AreEqual("Message body", messageArray1[0].Body);

        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void SendMessage2GroupWithSubGroupGetMessageForEachInterviwer_InterviewersAreLogged_CorrectMessagesAreReceived()
        {
            var groups = new List<int>();

            int mainGroupID = PersonTools.CreatePersonGroup("MainGroup");

            int subGroupID = PersonTools.CreatePersonGroup("SubGroup", new[] { mainGroupID });

            groups.Add(mainGroupID);
            int user1ID = PersonTools.CreatePerson(UserName1, Password, AgentTaskChoiceMode.Manual, groups.ToArray());

            groups.Add(subGroupID);
            int user2ID = PersonTools.CreatePerson(UserName2, Password, AgentTaskChoiceMode.Manual, groups.ToArray());

            BackendTools.LoginPerson(user1ID, "");
            BackendTools.LoginPerson(user2ID, "");

            var message = new Message
            {
                Body = "Message body",
                SupervisorName = "Supervisor name"
            };

            groups.Clear();

            groups.Add(subGroupID);
            SendMessageManager.SendMessageToGroups(groups, true, message);

            var consoleHelper1 = new CatiWsHelper(UserName1, Password);
            Messages[] messageArray1 = consoleHelper1.ConsoleService.GetMessages();

            //When we call any WS method PersonId is stored in CallContext each time
            //When ConsoleServiceHelpers are created in one thread for several users CallContext will contain PersonId for last user.
            var consoleHelper2 = new CatiWsHelper(UserName2, Password);
            Messages[] messageArray2 = consoleHelper2.ConsoleService.GetMessages();

            Assert.AreEqual(0, messageArray1.Length);
            Assert.AreEqual(1, messageArray2.Length);

            groups.Add(mainGroupID);
            SendMessageManager.SendMessageToGroups(groups.Select(x => x), true, message);

            consoleHelper1 = new CatiWsHelper(UserName1, Password);
            messageArray1 = consoleHelper1.ConsoleService.GetMessages();

            consoleHelper2 = new CatiWsHelper(UserName2, Password);
            messageArray2 = consoleHelper2.ConsoleService.GetMessages();

            Assert.AreEqual(1, messageArray1.Length);
            Assert.AreEqual(1, messageArray2.Length);

        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void SendMessage2Survey_TwoInterviewersAreLoggedOnSurvey_CorrectMessagesAreReceived()
        {
            var surveys = new List<int>();

            int user1ID = PersonTools.CreatePerson(UserName1, Password, AgentTaskChoiceMode.CampaignAssignment);
            int user2ID = PersonTools.CreatePerson(UserName2, Password, AgentTaskChoiceMode.CampaignAssignment);

            BackendTools.LoginPerson(user1ID, "");
            PersonService.LoginPersonOnSurveyForSurveySelectionMode(user1ID, _surveySID);
            BackendTools.LoginPerson(user2ID, "");
            PersonService.LoginPersonOnSurveyForSurveySelectionMode(user2ID, _surveySID);

            surveys.Add(_surveySID);

            var message = new Message
            {
                Body = "Message body",
                SupervisorName = "Supervisor name"
            };

            SendMessageManager.SendMessageToSurveys(surveys.Select(x => x), message);

            var consoleHelper1 = new CatiWsHelper(UserName1, Password);
            Messages[] messageArray1 = consoleHelper1.ConsoleService.GetMessages();

            //When we call any WS method PersonId is stored in CallContext each time
            //When ConsoleServiceHelpers are created in one thread for several users CallContext will contain PersonId for last user.
            var consoleHelper2 = new CatiWsHelper(UserName2, Password);
            Messages[] messageArray2 = consoleHelper2.ConsoleService.GetMessages();

            Assert.AreEqual(1, messageArray1.Length);
            Assert.AreEqual(1, messageArray2.Length);

            Assert.AreEqual("Message body", messageArray1[0].Body);
            Assert.AreEqual("Message body", messageArray2[0].Body);

            Assert.AreEqual("Supervisor name", messageArray1[0].SupervisorName);
            Assert.AreEqual("Supervisor name", messageArray2[0].SupervisorName);

        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void SendMessage2Survey_OneInterviewerAreLoggedOnSurveySecondNotLogged_CorrectMessageForFirstNoMessageForSecondAreReceived()
        {
            var surveys = new List<int>();

            int user1ID = PersonTools.CreatePerson(UserName1, Password, AgentTaskChoiceMode.CampaignAssignment);
            int user2ID = PersonTools.CreatePerson(UserName2, Password, AgentTaskChoiceMode.CampaignAssignment);

            BackendTools.LoginPerson(user1ID, "");
            PersonService.LoginPersonOnSurveyForSurveySelectionMode(user1ID, _surveySID);
            BackendTools.LoginPerson(user2ID, "");
            PersonService.LoginPersonOnSurveyForSurveySelectionMode(user1ID, _surveySID);

            surveys.Add(_surveySID);

            var message = new Message
            {
                Body = "Message body",
                SupervisorName = "Supervisor name"
            };

            SendMessageManager.SendMessageToSurveys(surveys.Select(x => x), message);

            var consoleHelper1 = new CatiWsHelper(UserName1, Password);
            Messages[] messageArray1 = consoleHelper1.ConsoleService.GetMessages();

            //When we call any WS method PersonId is stored in CallContext each time
            //When ConsoleServiceHelpers are created in one thread for several users CallContext will contain PersonId for last user.
            var consoleHelper2 = new CatiWsHelper(UserName2, Password);
            Messages[] messageArray2 = consoleHelper2.ConsoleService.GetMessages();

            Assert.AreEqual(1, messageArray1.Length);
            Assert.AreEqual(0, messageArray2.Length);

            Assert.AreEqual("Message body", messageArray1[0].Body);
            Assert.AreEqual("Supervisor name", messageArray1[0].SupervisorName);

        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void SendTwoMessagesToInterviewer_UpdateCreationTimeForFirstStartCleanThread_OneMessageDeletedOneReceived()
        {
            var interviewers = new List<int>();

            int user1ID = PersonTools.CreatePerson(UserName1, Password, AgentTaskChoiceMode.Manual);

            BackendTools.LoginPerson(user1ID, "");

            interviewers.Add(user1ID);

            var message = new Message
            {
                Body = "Message body",
                SupervisorName = "Supervisor name"
            };

            SendMessageManager.SendMessageToInterviewers(interviewers, true, message);

            _framework.DbEngine.ExecuteNonQuery(
                "UPDATE bvMessages SET CreateTime = DateAdd(day, -7, GETUTCDATE())",
                CommandType.Text
                );

            ServiceLocator.Resolve<IPersonMessageService>().CleanMessages(TimeSpan.FromDays(7));

            var consoleHelper1 = new CatiWsHelper(UserName1, Password);
            Messages[] messageArray1 = consoleHelper1.ConsoleService.GetMessages();

            Assert.AreEqual(0, messageArray1.Length);
        }
    }
}
