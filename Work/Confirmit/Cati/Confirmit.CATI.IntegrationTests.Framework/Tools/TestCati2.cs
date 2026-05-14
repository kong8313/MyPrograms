using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Threading;
using System.Web;

using System.Linq;

using Confirmit.CATI.Common;
using BvCallHandlerLibrary.Tools;
using BvCallHandlerLibrary.Tools.Fakes;
using Confirmit.CATI.Common.ConsoleService;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data.Fakes;
using Confirmit.CATI.Core.Services.Survey.Data;
using Confirmit.CATI.Telephony;
using Confirmit.Security.Crypto.Web;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    // TODO: Now WS has ConsoleService type and it is hard not to change it to the IConsoleService as it uses CatiWsHelper.ConsoleService
    //       that is used in many tests. So actually now we could simply delete CatiWsHelper as all authorization now could be easy 
    //       overridden using container and stubs.
    public class TestCati2
    {
        public static class ITS
        {
            public static readonly int Init = 1;
            public static readonly int Busy = 2;
            public static readonly int NoReply = 3;
            public static readonly int Complete = 13;
            public static readonly int InterupBySystem = 26;
            public static readonly int TelephoneProblem = 29;
            public static readonly int Error = 30;
            public static readonly int FakeForComplete = 40;
            public static readonly int FakeForNoReply = 50;
            public static readonly int FakeForError = 60;
            public static readonly int FakeForTelephoneProblem = 70;
            public static readonly int FakeForInteruptBySystem = 80;
            public static readonly int FakeForBusy = 90;
        }

        private const string SuperName = "administrator";
        private CatiWsHelper _consoleServiceHelper;
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;

        public enum TerminateCalled
        {
            FromSupervisor,
            FromConsoleService
        }

        /// <summary>
        /// Gets/sets person mode of last Login method.
        /// </summary>
        public AgentTaskChoiceMode PersonMode
        {
            get;
            private set;
        }

        public int SurveySID
        {
            get;
            set;
        }

        public string StationId
        {
            get { return "stid1"; }
        }

        private string _surveyName = BackendTools.GenerateSurveyName();

        public string SurveyName
        {
            get { return _surveyName; }
        }

        public long CampaignId
        {
            get { return ProjectIdConverter.ProjectIdToCampaignId(SurveyName); }
        }

        private string CfSurveyDbName
        {
            get;
            set;
        }

        private string CfSurveyDbConnectionString { get; set; }

        public TestDialerHelper DialerHelper { get; set; }

        /// <summary>
        /// Interviewer identifier.
        /// </summary>
        public int PersonSID
        {
            get;
            protected set;
        }

        public bool IsLoginToDialer { get; protected set; }

        private LoginState LoginToDialerState
        {
            get { return IsLoginToDialer ? LoginState.LOGGED_IN : LoginState.NOT_LOGGED_IN; }
        }

        public bool AlreadyLoggedIn
        {
            get;
            protected set;
        }

        public LoginState CurrentLoggedInToDialerState
        {
            get;
            protected set;
        }

        public bool CurrentIsPredictive
        {
            get;
            protected set;
        }

        public IConsoleService WS
        {
            get { return _consoleServiceHelper.ConsoleService; }
        }

        public IConsoleStateService StateWS
        {
            get { return _consoleServiceHelper.ConsoleStateService; }
        }

        private readonly BackendTools _backendTools;

        public TestCati2(bool isUseSimulator, BackendTools backendTools, DialType dialType = DialType.Landline)
            : this(isUseSimulator, false, backendTools, dialType) { }

        public TestCati2(bool isUseSimulator, bool isUseConfirmitDb, BackendTools backendTools, DialType dialType = DialType.Landline) :
            this(isUseSimulator, isUseConfirmitDb, isUseSimulator, backendTools, dialType)
        { }

        public TestCati2(bool isUseSimulator, bool isUseConfirmitDb, bool isUseTelephony, BackendTools backendTools, DialType dialType = DialType.Landline)
            : this(isUseSimulator, isUseConfirmitDb, isUseTelephony, backendTools, new TestDialer(dialType), null)
        {
        }

        public TestCati2(bool isUseSimulator, bool isUseConfirmitDb, bool isUseTelephony, BackendTools backendTools, ITestDialer testDialer, string testSurveyName)
        {
            if (string.IsNullOrEmpty(testSurveyName))
            {
                _surveyName = _framework.TestSurveyName;
                CfSurveyDbName = _framework.TestSurveyDatabaseName;
            }
            else
            {
                _surveyName = testSurveyName;
                CfSurveyDbName = "testSurvey_" + testSurveyName;
            }

            IsLoginToDialer = false;

            // Setup stubs first before resolving any interfaces
            TestCatiInit(isUseSimulator, isUseConfirmitDb, isUseTelephony, testDialer);

            _backendTools = backendTools;
        }

        private void TestCatiInit(bool isUseSimulator, bool isUseConfirmitDb, bool isUseTelephony, ITestDialer testDialer)
        {
            if (isUseSimulator)
            {
                Stubs.SetNewIDialerApiStub((d) =>
                {
                    DialerHelper.SetBehaviorForGetDialerVersion(args => null);
                    DialerHelper.SetBehaviorForGetFeatures(args => null);
                    DialerHelper.SetBehaviorForSetCampaign(args => 0);
                    return DialerHelper.FakeDialer;
                });

                var stubIMnTciTools = new StubIMnTciTools
                {
                    DoesCompanyUseTelephony = () => isUseTelephony,
                    IsDialerConfigured = () => isUseTelephony,
                    CreateDialerRecordingInt32 = id => null
                };
                ServiceLocator.RegisterInstance<IMnTciTools>(stubIMnTciTools);
            }

            DialerHelper = new TestDialerHelper(testDialer);

            _framework.BackendInitialize(isUseTelephony, testDialer.DialType);

            if (isUseConfirmitDb)
            {
                CfSurveyDbConnectionString = IntegrationTestingFramework.Instance.GetConfirmitSqlServerConnectionString(CfSurveyDbName);
                if (CfSurveyDbName != _framework.TestSurveyDatabaseName)
                {
                    new SqlObjectCreator(_framework).CreateTestSurveyDatabase(CfSurveyDbName);
                }

                ServiceLocator.Register<IRespondentVariablesService, RespondentVariablesService>();
                ServiceLocator.Register<ISurveyDatabaseService, SurveyDatabaseService>();
                ServiceLocator.Register<IPersonSessionHistoryRepository, PersonSessionHistoryRepository>();
            }
        }

        public void CompareState(State actual, State expected)
        {
            Assert.AreEqual(expected.surveyId, actual.surveyId, "SurveyID");
            Assert.AreEqual(expected.interviewId, actual.interviewId, "InterviewID");
            Assert.IsTrue(
                InterviewUrlComparer.AreEqual(expected.interviewURL, actual.interviewURL),
                String.Format(
                    "InterviewURL. Expected value: {0} Actual value: {1}",
                    expected.interviewURL,
                    actual.interviewURL));
            Assert.AreEqual(expected.interviewState, actual.interviewState, "InterviewState");
            Assert.AreEqual(expected.callOutcome, actual.callOutcome, "CallOutcome");
            Assert.AreEqual((LoginState)expected.interviewerLoginState, (LoginState)actual.interviewerLoginState, "LoginState");
            Assert.AreEqual((LoginState)expected.interviewerLoginToDialerState, (LoginState)actual.interviewerLoginToDialerState, "LoginToDialerState");
            Assert.AreEqual(expected.problemState, actual.problemState, "ProblemState");
        }

        public void CheckState(State modal)
        {
            State current = StateWS.GetState();
            CompareState(current, modal);
        }

        public State WaitState(Func<State, bool> comparer)
        {
            return WaitState(_consoleServiceHelper, comparer);
        }

        public static State WaitState(CatiWsHelper ws, Func<State, bool> comparer)
        {
            DateTime deadLine = DateTime.Now + TimeSpan.FromMinutes(2);

            do
            {
                State state = ws.ConsoleStateService.GetState();

                if (comparer(state))
                {
                    return state;
                }

                Thread.Sleep(30);
            } while (deadLine > DateTime.Now);

            Assert.Fail("WaitInterviewState timeout expired");
            return null;
        }

        public State WaitInterviewState(InterviewState interviewState)
        {
            return WaitState(state => state.interviewState == (int)interviewState);
        }

        public static State WaitInterviewState(CatiWsHelper ws, InterviewState interviewState)
        {
            return WaitState(ws, state => state.interviewState == (int)interviewState);
        }

        public State WaitLoginToDialerState(LoginState loginToDialerState)
        {
            return WaitState(state => state.interviewerLoginToDialerState == (int)loginToDialerState);
        }

        /// <summary>
        /// Returns expected State which should occurs during login to dialer process (directly before and after).
        /// </summary>
        /// <param name="state">State of login process.</param>
        /// <returns>State expected to be returned from CatWS.GetState() method.</returns>
        public State GetExpectedStateLoginToDialer(LoginState state)
        {
            InterviewState interviewState =
                PersonMode == AgentTaskChoiceMode.Automatic
                    ? InterviewState.NO_CALLS
                    : (CurrentIsPredictive ? InterviewState.WAITING : InterviewState.SELECTING);

            return new State(
                (PersonMode == AgentTaskChoiceMode.CampaignAssignment && (state == LoginState.LOGGED_IN || state == LoginState.LOGGING_IN)) ? SurveyName : null,
                null,
                0,
                null,
                null,
                (int)interviewState,
                (int)CallOutcome.NotDefined,
                (int)LoginState.LOGGED_IN,
                (int)state,
                (int)DialerErrorCode.Success,
                0,
                false);
        }

        public int CreateSurveyWithPerson(DialingMode surveyMode, string user, string password, AgentTaskChoiceMode personMode, int? callCenterId = null, int? personId = null, SubRule subRule = null, int openEndReview = 0, DialType dialType = DialType.Landline)
        {
            var script = new TestScript(
                new[]{
                                 new SubRule( new Action(Action.Operation.SetNewITS, ITS.FakeForComplete.ToString(CultureInfo.InvariantCulture)),
                                              ITS.Complete, 0, 0, null, true ),
                                 new SubRule( new Action(Action.Operation.SetNewITS, ITS.FakeForNoReply.ToString(CultureInfo.InvariantCulture)),
                                              ITS.NoReply, 0, 0, null, true ),
                                 new SubRule( new Action(Action.Operation.SetNewITS, ITS.FakeForError.ToString(CultureInfo.InvariantCulture)),
                                              ITS.Error, 0, 0, null, true ),
                                 new SubRule( new Action(Action.Operation.SetNewITS, ITS.FakeForTelephoneProblem.ToString(CultureInfo.InvariantCulture)),
                                              ITS.TelephoneProblem, 0, 0, null, true ),
                                 new SubRule( new Action(Action.Operation.SetNewITS, ITS.FakeForInteruptBySystem.ToString(CultureInfo.InvariantCulture)),
                                              ITS.InterupBySystem, 0, 0, null, true ),
                                 new SubRule( new Action(Action.Operation.SetNewITS, ITS.FakeForBusy.ToString(CultureInfo.InvariantCulture)),
                                              ITS.Busy, 0, 0, null, true )
                             },
                             @"CATI\Schedule.xml");

            if (subRule != null)
            {
                script.Rules[0].SubRules.Add(subRule);
            }

            SurveySID = _backendTools.CreateSurvey(script, SurveyName, CfSurveyDbConnectionString, openEndReview:openEndReview);
            SetSurveyDialingMode(SurveySID, surveyMode);

            if (callCenterId.HasValue)
            {
                ServiceLocator.Resolve<ICallCenterService>().AssignSurvey(callCenterId.Value, SurveySID);
            }

            if (personId.HasValue)
                PersonSID = personId.Value;
            else
                CreatePerson(user, password, personMode, callCenterId, dialType);

            BackendTools.AssignCatiPersonToSurvey(SurveySID, PersonSID);

            if (surveyMode != DialingMode.Manual)
            {
                DialerHelper.AddRequestStartCampaign();
            }

            var surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            surveyStateService.Open(SurveySID);

            return SurveySID;
        }

        public void SendEventConnected(string callerId = null, int ringTimeSeconds = 0, Dictionary<string, string> callOutcomeMetadata = null)
        {
            var task = TaskRepository.GetByPerson(PersonSID);
            var interview = InterviewRepository.GetById(task.SurveySID, task.InterviewID);
            var call = CallQueueService.GetCallAndNoLock(SurveySID, interview.ID);
            DialerHelper.SendEventConnected(CampaignId, PersonSID, call.CallID, callerId, ringTimeSeconds, callOutcomeMetadata);
        }

        public void Login(string user, string password, AgentTaskChoiceMode personMode, bool connectToDialer, State expectedState, out PersonInfo personInfo)
        {
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer outProperties;

            _consoleServiceHelper = new CatiWsHelper(user, password);

            var consoleDescriptor = new ConsoleDescription();

            _consoleServiceHelper.ConsoleService.Login(StationId, consoleDescriptor, out personInfo, out diallerInfo, out outProperties);

            Assert.IsNotNull(outProperties);
            Assert.AreEqual(personMode, (AgentTaskChoiceMode)personInfo.PersonMode);
            Assert.AreEqual(connectToDialer, diallerInfo.ConnectedToDialer);

            AlreadyLoggedIn = personInfo.AlreadyLoggedIn;
            CurrentLoggedInToDialerState = (LoginState)diallerInfo.CurrentLoggedInToDialerState;
            CurrentIsPredictive = diallerInfo.CurrentIsPredictive;

            PersonMode = personMode;
            if (expectedState == null)
            {
                expectedState = GetExpectedStateLoginToDialer(CurrentLoggedInToDialerState);
            }

            BackendTools.RunSchedulingProcedure();

            CheckState(expectedState);
        }

        public void Login(string user, string password, AgentTaskChoiceMode personMode, bool connectToDialer)
        {
            PersonInfo personInfo;
            Login(user, password, personMode, connectToDialer, null, out personInfo);
        }

        public void Login(string user, string password, AgentTaskChoiceMode personMode, bool connectToDialer, out PersonInfo personInfo)
        {
            Login(user, password, personMode, connectToDialer, null, out personInfo);
        }

        public void Login(string user, string password, AgentTaskChoiceMode personMode, bool connectToDialer, State expectedState)
        {
            PersonInfo personInfo;
            Login(user, password, personMode, connectToDialer, expectedState, out personInfo);
        }

        public void Logout(bool useDialer)
        {
            if (useDialer)
            {
                var dialerEventsHandler = ServiceLocator.Resolve<IDialerEventsHandler>();
                DialerHelper.AddRequestLogout(() => dialerEventsHandler.OnDialerNotifyAgentState(
                0,
                "",
                0,
                PersonSID,
                ((int)AgentStateMsgs.LOGGEDOUT).ToString(CultureInfo.InvariantCulture)));
            }

            Logout();
        }

        public void Logout()
        {
            WS.SetPendingLogout(true);

            WaitState(state => state.interviewerLoginState == (int)LoginState.NOT_LOGGED_IN);

            WS.ConfirmLogout();
        }

        public BvInterviewEntity StartInterview_Progressive(string surveyId, int interviewId)
        {
            DialerHelper.AddRequestSendNumber();

            Assert.IsTrue(WS.StartInterview(surveyId, interviewId));

            var task = TaskRepository.GetByPerson(PersonSID);

            return InterviewRepository.GetById(task.SurveySID, task.InterviewID);
        }

        public BvInterviewEntity GetInterviewByID(int interviewId)
        {
            return Interviews.FirstOrDefault(interview => interview.ID == interviewId);
        }

        public void LoginToDialer(string extensionNumber)
        {
            DialerHelper.AddRequestLogin();
            {
                bool dummyIsPredictive;

                WS.LoginToDialer(
                    extensionNumber,
                    PersonMode == AgentTaskChoiceMode.CampaignAssignment ? SurveyName : null, out dummyIsPredictive);
            }

            CheckState(GetExpectedStateLoginToDialer(LoginState.LOGGING_IN));

            DialerHelper.AddRequestGoReady();
            {
                DialerHelper.SendEventNotifyAgentState(
                    CampaignId,
                    PersonSID,
                    "1");
            }

            CompareState(
                WaitLoginToDialerState(LoginState.LOGGED_IN),
                GetExpectedStateLoginToDialer(LoginState.LOGGED_IN));

            IsLoginToDialer = true;
        }

        public void LoginToDialer_Predictive(string extensionNumber, bool withInitEngine, string[] groups)
        {
            DialerHelper.AddRequestLogin();
            {
                bool isPredictive;

                WS.LoginToDialer(extensionNumber, SurveyName, out isPredictive);
                CurrentIsPredictive = isPredictive;
            }

            CheckState(GetExpectedStateLoginToDialer(LoginState.LOGGING_IN));

            DialerHelper.AddRequestGoReady();
            DialerHelper.AddRequestSetGroups();
            {
                DialerHelper.SendEventNotifyAgentState(
                    CampaignId,
                    PersonSID,
                    "1");
                {
                    CheckState(GetExpectedStateLoginToDialer(LoginState.LOGGED_IN));
                }
            }
            Trace.TraceInformation("{0}: Check", DateTime.Now.ToString(CultureInfo.InvariantCulture));

            CompareState(
                WaitLoginToDialerState(LoginState.LOGGED_IN),
                GetExpectedStateLoginToDialer(LoginState.LOGGED_IN));

            IsLoginToDialer = true;
        }

        BvInterviewEntity[] _interviews;
        public BvInterviewEntity[] Interviews
        {
            get
            {
                return _interviews;
            }
        }

        public BvInterviewEntity[] CreateInterviewsWithCalls(int count, bool useCfDb, DialType dialType = DialType.Landline)
        {
            var result = new List<BvInterviewEntity>();
            for (int i = 0; i < count; i++)
            {
                var interview = BackendTools.NewInterview(SurveySID, dialType);
                interview.ID = i + 1;
                interview.TelephoneNumber = "303030" + i.ToString(CultureInfo.InvariantCulture);
                interview.TransientState = ITS.Init;
                BackendTools.CreateInterview(interview);

                BvCallEntity call = BackendTools.NewCall(interview);
                BackendTools.CreateCall(call);

                result.Add(interview);

                if (useCfDb)
                {
                    using (var connection = new SqlConnection(_framework.GetConfirmitSqlServerConnectionString(CfSurveyDbName)))
                    {
                        connection.Open();

                        var command = new SqlCommand(
                            @"DELETE FROM respondent WHERE respid = @ID
                            SET IDENTITY_INSERT dbo.respondent ON
                            INSERT INTO respondent( respid ) VALUES( @ID )
                            SET IDENTITY_INSERT dbo.respondent OFF",
                            connection)
                        {
                            CommandType = CommandType.Text
                        };
                        command.Parameters.AddWithValue("@ID", interview.ID);

                        command.ExecuteNonQuery();
                    }
                }
            }

            _interviews = result.ToArray();

            return Interviews;
        }

        public BvInterviewEntity[] CreateInterviewsWithCalls(int count, DialType dialType = DialType.Landline)
        {
            return CreateInterviewsWithCalls(count, false, dialType);
        }

        public void CheckAllInterviews()
        {
            foreach (var interview in Interviews)
            {
                BackendTools.CheckInterview(interview);
            }
        }

        public BvInterviewEntity NoReplyWithSpecificOutcomeAndWaitNextInterview_Progressive(int interviewId1, CallOutcome outcome, int interviewId2)
        {
            DialerHelper.AddRequestCompleteCall(); //CompleteCall for the current interview
            DialerHelper.AddRequestSendNumber(); //SendNumber for the next interview

            var call = CallQueueService.GetCallAndNoLock(SurveySID, interviewId1);

            // первый кол не дозвонился
            DialerHelper.SendEventNotifyOutcome(CampaignId, PersonSID, call.CallID, outcome);

            CompareState(
                StateWS.GetState(),
                new State(SurveyName, null, interviewId2, null, null,
                           (int)InterviewState.DIALLING,
                           (int)outcome,
                           (int)LoginState.LOGGED_IN,
                           (int)LoginState.LOGGED_IN,
                           (int)DialerErrorCode.Success,
                           0,
                           false));

            return InterviewRepository.GetById(SurveySID, interviewId2);
        }

        public BvInterviewEntity NoReplyAndWaitNextInterview_Progressive(int interviewId1, int interviewId2)
        {
            DialerHelper.AddRequestCompleteCall(); //CompleteCall for the current interview
            DialerHelper.AddRequestSendNumber(); //SendNumber for the next interview

            var call = CallQueueService.GetCallAndNoLock(SurveySID, interviewId1);

            // первый кол не дозвонился
            DialerHelper.SendEventNotifyOutcome(CampaignId, PersonSID, call.CallID, CallOutcome.NoReply);

            CompareState(
                StateWS.GetState(),
                new State(SurveyName, null, interviewId2, null, null,
                           (int)InterviewState.DIALLING,
                           (int)CallOutcome.NoReply,
                           (int)LoginState.LOGGED_IN,
                           (int)LoginState.LOGGED_IN,
                           (int)DialerErrorCode.Success,
                           0,
                           false));

            return InterviewRepository.GetById(SurveySID, interviewId2);
        }

        public void SendEventNotifyOutcome(int interviewId, CallOutcome callOutcome)
        {
            var call = CallQueueService.GetCallAndNoLock(SurveySID, interviewId);

            DialerHelper.SendEventNotifyOutcome(CampaignId, PersonSID, call.CallID, callOutcome);
        }

        public string InterviewUrl(int interviewId)
        {
            return InterviewUrl(interviewId, String.Empty);
        }

        private string InterviewUrl(BvInterviewEntity interview)
        {
            return InterviewUrl(interview.ID, interview.ConfirmitSid);
        }

        private string InterviewUrl(int interviewId, string securityKey)
        {
            string sid = HttpUtility.UrlEncode(
                EncryptionUsingMachineKey.Encrypt(
                    DataProtection.All,
                    string.Format("r&{0}&s&{1}&__channel&cati", interviewId.ToString(CultureInfo.InvariantCulture), securityKey))
                );

            return String.Format(
                "http://localhost/wix/cati_{0}.aspx?__resume=1&__catiinterviewerid={1}&__sid__={2}",
                SurveyName,
                PersonSID,
                sid
                );
        }

        public void ReplyOnInterview_Progressive(BvInterviewEntity interview)
        {
            SendEventConnected();

            CompareState(
                WaitInterviewState(InterviewState.INTERVIEWING),
                new State(SurveyName, null, interview.ID, InterviewUrl(interview.ID), null,
                          (int)InterviewState.INTERVIEWING,
                          (int)CallOutcome.Connected,
                          (int)LoginState.LOGGED_IN,
                          (int)LoginState.LOGGED_IN,
                          (int)DialerErrorCode.Success,
                          0,
                          false));
        }

        public void CompleteInterview_Progressive(BvInterviewEntity interview)
        {
            CompleteInterview_Progressive(interview, true);
        }

        public void CompleteInterview_Progressive(BvInterviewEntity interview, bool compiteCall)
        {
            if (compiteCall)
            {
                DialerHelper.AddRequestCompleteCall();
            }

            CompleteInterview(interview);
        }

        public BvInterviewEntity StartInterview_ManualOrPreview(string surveyId, int interviewId)
        {
            Assert.IsTrue(WS.StartInterview(surveyId, interviewId));

            var state = WaitState(s =>
                s.interviewState == (int)InterviewState.INTERVIEWING ||
                s.interviewState == (int)InterviewState.NO_CALLS);

            if (state.interviewState == (int)InterviewState.NO_CALLS)
            {
                CheckState(new State(PersonMode == AgentTaskChoiceMode.CampaignAssignment ? SurveyName : null, null, 0, null, null,
                                 (int)InterviewState.NO_CALLS,
                                 (int)CallOutcome.NotDefined,
                                 (int)LoginState.LOGGED_IN,
                                 (int)LoginToDialerState,
                                 (int)DialerErrorCode.Success,
                                 0,
                                 false));
                return null;
            }

            int startInterviewId = WaitInterviewState(InterviewState.INTERVIEWING).interviewId;

            BvInterviewEntity result = GetInterviewByID(startInterviewId);

            Assert.IsNotNull(result, "interview is not starting");

            CheckState(new State(SurveyName, null, result.ID, InterviewUrl(result), null,
                                 (int)InterviewState.INTERVIEWING,
                                 (int)CallOutcome.NotDefined,
                                 (int)LoginState.LOGGED_IN,
                                 (int)LoginToDialerState,
                                 (int)DialerErrorCode.Success,
                                 0,
                                 false));

            return result;
        }

        public void Redial(BvInterviewEntity interview, CallOutcome outcome, string callerId = null, int ringTimeSeconds = 0, Dictionary<string, string> callOutcomeMetadata = null, string telephoneNumber = null)
        {
            DialerHelper.AddRequestRedial();
            WS.Dial(telephoneNumber ?? interview.TelephoneNumber, 1, 1);

            CheckState(new State(SurveyName, null, interview.ID, null, null,
                                 (int)InterviewState.REDIALLING,
                                 (int)CallOutcome.NotDefined,
                                 (int)LoginState.LOGGED_IN,
                                 (int)LoginToDialerState,
                                 (int)DialerErrorCode.Success,
                                 0,
                                 false));

            var call = CallQueueService.GetCallAndNoLock(SurveySID, interview.ID);

            DialerHelper.SendEventNotifyOutcome(CampaignId, PersonSID, call.CallID, outcome, callerId, ringTimeSeconds, callOutcomeMetadata);

            CheckState(
                new State(SurveyName, null, interview.ID, InterviewUrl(interview), null,
                                  (int)InterviewState.INTERVIEWING,
                                  (int)outcome,
                                  (int)LoginState.LOGGED_IN,
                                  (int)LoginToDialerState,
                                  (int)DialerErrorCode.Success,
                                  0,
                                  false));
        }

        public void Dial(BvInterviewEntity interview,
            int initiator,
            bool isOk,
            CallOutcome dialerExternalReturnOutcome, string callerId = null, int ringTimeSeconds = 0, Dictionary<string, string> callOutcomeMetadata = null)
        {
            DialerHelper.AddRequestSendNumber();

            WS.Dial(interview.TelephoneNumber, initiator, 1);
            CheckState(new State(SurveyName, null, interview.ID, null, null,
                                 (int)InterviewState.DIALLING,
                                 (int)CallOutcome.NotDefined,
                                 (int)LoginState.LOGGED_IN,
                                 (int)LoginState.LOGGED_IN,
                                 (int)DialerErrorCode.Success,
                                 0,
                                 false));

            if (isOk)
            {
                SendEventConnected(callerId, ringTimeSeconds, callOutcomeMetadata);

                CompareState(
                    WaitInterviewState(InterviewState.INTERVIEWING),
                    new State(SurveyName, null, interview.ID, InterviewUrl(interview.ID), null,
                              (int)InterviewState.INTERVIEWING,
                              (int)CallOutcome.Connected,
                              (int)LoginState.LOGGED_IN,
                              (int)LoginState.LOGGED_IN,
                              (int)DialerErrorCode.Success,
                              0,
                              false));
            }
            else
            {
                var call = CallQueueService.GetCallAndNoLock(SurveySID, interview.ID);

                DialerHelper.SendEventNotifyOutcome(CampaignId, PersonSID, call.CallID, CallOutcome.ReturnedNotDialled);

                CompareState(
                    WaitInterviewState(InterviewState.INTERVIEWING),
                    new State(SurveyName, null, interview.ID, InterviewUrl(interview.ID), null,
                              (int)InterviewState.INTERVIEWING,
                              (int)dialerExternalReturnOutcome,
                              (int)LoginState.LOGGED_IN,
                              (int)LoginState.LOGGED_IN,
                              (int)DialerErrorCode.Success,
                              0,
                              false));
            }
        }

        public BvInterviewEntity CompleteInterviewAndWaitNext_Preview(BvInterviewEntity interview)
        {
            return CompleteInterviewAndWaitNext(interview, true);
        }

        public BvInterviewEntity CompleteInterviewAndWaitNext_Manual(BvInterviewEntity interview)
        {
            return CompleteInterviewAndWaitNext(interview, false);
        }

        private BvInterviewEntity CompleteInterviewAndWaitNext(BvInterviewEntity interview, bool addRequestCompleteCall)
        {
            BvInterviewEntity result = null;

            if (addRequestCompleteCall)
            {
                DialerHelper.AddRequestCompleteCall();
            }

            CompleteInterview(interview);

            State state = WaitState(
                s => s.interviewState == (int)InterviewState.NO_CALLS ||
                     s.interviewState == (int)InterviewState.INTERVIEWING);

            if (state.interviewState == (int)InterviewState.NO_CALLS)
            {
                CompareState(state,
                              new State((PersonMode == AgentTaskChoiceMode.CampaignAssignment) ? SurveyName : null,
                                        null, 0, null, null,
                                        (int)InterviewState.NO_CALLS,
                                        (int)CallOutcome.NotDefined,
                                        (int)LoginState.LOGGED_IN,
                                        (int)LoginToDialerState,
                                        (int)DialerErrorCode.Success,
                                        0,
                                        false));
            }
            else
            {
                Assert.AreNotEqual(state.interviewId, interview.ID);

                result = GetInterviewByID(state.interviewId);

                Assert.IsNotNull(result);

                CompareState(state,
                             new State(SurveyName, null, result.ID, InterviewUrl(result), null,
                                       (int)InterviewState.INTERVIEWING,
                                       (int)CallOutcome.NotDefined,
                                       (int)LoginState.LOGGED_IN,
                                       (int)LoginToDialerState,
                                       (int)DialerErrorCode.Success,
                                       0,
                                       false));
            }

            return result;
        }

        private void CompleteInterview(BvInterviewEntity interview)
        {
            WS.WrapUp(interview.ID, true, 1, new CompletedInterviewDetails { InterviewDuration = 0, Its = "13", Status = "Complete" });
        }

        public void CompleteInterview_Manual(BvInterviewEntity interview)
        {
            CompleteInterview(interview);

            CompareState(
                WaitInterviewState(InterviewState.SELECTING),
                new State(null, null, 0, null, null,
                          (int)InterviewState.SELECTING,
                          (int)CallOutcome.NotDefined,
                          (int)LoginState.LOGGED_IN,
                          (int)LoginState.LOGGED_IN,
                          (int)DialerErrorCode.Success,
                          0,
                          false));
        }

        public void GetOpenedSurveys()
        {
            Survey[] surveys = WS.GetOpenedSurveys();

            Assert.AreEqual(surveys.Length, 1);
            Assert.AreEqual(surveys[0].id, SurveyName);
            Assert.AreEqual(surveys[0].name, ""/*TODO:BvFMWSHelper.SurveyName*/);
        }

        public void GetSurveyInterviews(int mustBeCount)
        {
            DataTable interviewsData = WS.GetSurveyInterviews(SurveyName, new SearchParameter[] { });

            Assert.AreEqual(interviewsData.Rows.Count, mustBeCount);

            foreach (DataRow row in interviewsData.Rows)
            {
                BvInterviewEntity interview = GetInterviewByID(int.Parse(row[0].ToString()));

                Assert.IsNotNull(interview);

                string respodentName = (row[1] != DBNull.Value) ? (string)row[1] : null;
                string telephoneNumber = (row[2] != DBNull.Value) ? (string)row[2] : null;

                Assert.AreEqual(respodentName, interview.RespondentName);
                Assert.AreEqual(telephoneNumber, interview.TelephoneNumber);
            }
        }

        public bool TerminateTask(TerminateCalled source, int personSid)
        {
            if (source == TerminateCalled.FromSupervisor)
            {
                DialerHelper.AddRequestCompleteCall();
                DialerHelper.AddRequestLogout();

                return TaskService.TerminateTask(
                        personSid,
                        new DatabaseTransactionOptions("TerminateTask", DeadlockPriority.Normal),
                        CallOutcome.InterruptedBySystem) != null;
            }
            // for called TerminateCalled.FromConsoleService
            WS.TerminateTask();
            return true;
        }

        public void CheckLogout()
        {
            Assert.IsNull(TaskRepository.GetByPerson(PersonSID));
        }

        /// <summary>
        /// Sets dialing mode for given survey.
        /// </summary>
        /// <param name="surveySid">Survey identifier.</param>
        /// <param name="mode">Dialing mode.</param>
        public void SetSurveyDialingMode(int surveySid, DialingMode mode)
        {
            SurveyService.SetDialingMode(surveySid, mode);
        }

        /// <summary>
        /// Creates person with given parameters and stores person sid in PersonSID property.
        /// </summary>
        /// <param name="user">User name.</param>
        /// <param name="password">User password.</param>
        /// <param name="personMode">Person mode.</param>
        /// <param name="callCenterId">call center id</param>
        public void CreatePerson(string user, string password, AgentTaskChoiceMode personMode, int? callCenterId = null, DialType dialType = DialType.Landline)
        {
            PersonSID = callCenterId.HasValue ?
                PersonTools.CreatePerson(user, password, personMode, callCenterId.Value, dialType) :
                PersonTools.CreatePerson(user, password, personMode, dialType);
        }

        public int CreateSurvey(TestScript scheduleXml)
        {
            return SurveySID = _backendTools.CreateSurvey(scheduleXml, SurveyName);
        }

        public void CheckCallAttemtCount(BvInterviewEntity interview, int callAttempt)
        {
            using (var connection = new SqlConnection(_framework.GetConfirmitSqlServerConnectionString(CfSurveyDbName)))
            {
                connection.Open();

                var command = new SqlCommand(
                    @"SELECT ISNULL( CallAttemptCount, 0 )
                    FROM dbo.respondent
                    WHERE respid = @ID",
                    connection)
                {
                    CommandType = CommandType.Text
                };
                command.Parameters.AddWithValue("@ID", interview.ID);

                var count = (int)command.ExecuteScalar();

                Assert.AreEqual(callAttempt, count);
            }

            if (callAttempt > 0)
            {
                var callAttemptsActual = _framework.DbEngine.ExecuteScalar<int>(
                    "select CallAttemptNumber from BvHistory where SurveyId = @SurveySID and InterviewId = @InterviewSID",
                    CommandType.Text,
                    new SqlParameter("@SurveySID", interview.SurveySID),
                    new SqlParameter("@InterviewSID", interview.ID));

                Assert.AreEqual(callAttempt, callAttemptsActual);
            }
        }
        

        public void CheckValueInBvTask(string field, object value)
        {
            using (var connection = new SqlConnection(IntegrationTestingFramework.Instance.DbEngine.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM BvTasks WHERE PersonSID = @PersonSID", connection))
                {
                    command.Parameters.AddWithValue("@PersonSID", PersonSID);

                    using (SqlDataReader sdr = command.ExecuteReader())
                    {
                        Assert.IsTrue(sdr.Read());
                        Assert.AreEqual(value, sdr[field]);
                    }
                }
            }
        }

        public bool IsThereARecordInBvTasksForThePerson()
        {
            return TaskRepository.GetByPerson(PersonSID) != null;
        }

        public BvTasksEntity GetBvTasksEntityForThePerson()
        {
            return TaskRepository.GetByPerson(PersonSID);
        }

        public void GetFroceOpenEndReview()
        {
            WS.GetForceOpenendReview(1);
        }

        public void CompleteInterviewWithLogout_Progressive(BvInterviewEntity interview)
        {
            WS.SetPendingLogout(true);

            DialerHelper.AddRequestCompleteCall();
            DialerHelper.AddRequestLogout();
            {
                WS.WrapUp(interview.ID, true, 1, new CompletedInterviewDetails { InterviewDuration = 0, Its = "13", Status = "Complete" });

                string surveyNameInContext = (PersonMode == AgentTaskChoiceMode.CampaignAssignment) ? SurveyName : null;

                CheckState(
                    new State(
                        surveyNameInContext,
                        null,
                        0,
                        null,
                        null,
                        (int)InterviewState.WAITING,
                        (int)CallOutcome.NotDefined,
                        (int)LoginState.LOGGING_OUT,
                        (int)LoginState.LOGGING_OUT,
                        (int)DialerErrorCode.Success,
                        0,
                        false));

                //Send NotifyAgentState
                DialerHelper.SendEventNotifyAgentState(
                    CampaignId,
                    PersonSID,
                    "4");

                CompareState(
                    WaitState(state => state.interviewerLoginToDialerState == (int)LoginState.NOT_LOGGED_IN),
                    new State(
                        surveyNameInContext,
                        null,
                        0,
                        null,
                        null,
                        (int)InterviewState.WAITING,
                        (int)CallOutcome.NotDefined,
                        (int)LoginState.NOT_LOGGED_IN,
                        (int)LoginState.NOT_LOGGED_IN,
                        (int)DialerErrorCode.Success,
                        0,
                        false));
            }

            WS.ConfirmLogout();
        }

        public void EmulateTelephonyErrorWhileLoginToDiallerNonRC()
        {
            var entity = BvTasksAdapter.GetByCondition(
                "[PersonSID] = @PersonSID",
                new SqlParameter("@PersonSID", PersonSID)).FirstOrDefault();

            if (entity != null)
            {
                entity.ProblemId = (int)DialerErrorCode.InvalidExtension; // error code doesn't matter. We just need some error
                entity.IsLoginRCToDialer = false;
                BvTasksAdapter.Update(entity);
            }
        }

        public void SetPersonLoginState(LoginState loginState)
        {
            var entity = BvTasksAdapter.GetByCondition(
                "[PersonSID] = @PersonSID",
                new SqlParameter("@PersonSID", PersonSID)).FirstOrDefault();

            if (entity != null)
            {
                entity.StatusLogout = (byte)loginState;
                BvTasksAdapter.Update(entity);
            }
        }

        public void SetPersonInterviewState(int interviewState, DialingMode dialingMode)
        {
            BvSpTasks_UpdateInterviewStateAdapter.ExecuteNonQuery(
                    PersonSID,
                    interviewState,
                    (byte)dialingMode);
        }

        public void SetPersonCallOutcome(int callOutcome)
        {
            var entity = BvTasksAdapter.GetByCondition(
                "[PersonSID] = @PersonSID",
                new SqlParameter("@PersonSID", PersonSID)).FirstOrDefault();

            Assert.IsNotNull(entity, "TestCati2.SetPersonCallOutcome: Record in task table is not found.");
            entity.CallOutcome = callOutcome;
            BvTasksAdapter.Update(entity);
        }

        public void StartInterview_Predictive(int callsCount)
        {
            WS.StartInterview(SurveyName, 0);

            var groups =
                new[]{ 1 /*dammy*/,
                PersonGroupRepository.GetByName("CATI Interviewers").SID,
                SurveySID};

            foreach (var groupId in groups)
            {
                //Test create call only for 
                if (groupId == SurveySID)
                    DialerHelper.AddRequestSendNumbers();

                DialerHelper.SendEventRequestCalls(CampaignId, groupId, callsCount);
            }
            /*Check call on phase = -2*/
        }

        public BvInterviewEntity ConnectToInterview_Predictive(BvInterviewEntity interview)
        {
            var call = CallQueueService.GetCallAndNoLock(SurveySID, interview.ID);
            DialerHelper.SendEventConnected(CampaignId, PersonSID, call.CallID);

            CheckState(new State(SurveyName, null, interview.ID, InterviewUrl(interview), null,
                     (int)InterviewState.INTERVIEWING,
                     (int)CallOutcome.Connected,
                     (int)LoginState.LOGGED_IN,
                     (int)LoginToDialerState,
                     (int)DialerErrorCode.Success,
                     0,
                     false));

            return interview;
        }

        public BvInterviewEntity NotConnectToInterview_Predictive(BvInterviewEntity interview, CallOutcome outcome)
        {
            var call = CallQueueService.GetCallAndNoLock(SurveySID, interview.ID);
            DialerHelper.SendEventNotifyOutcome(CampaignId, 0, call.CallID, outcome);

            CheckState(new State(SurveyName, null, 0, null, null,
                     (int)InterviewState.WAITING,
                     (int)CallOutcome.NotDefined,
                     (int)LoginState.LOGGED_IN,
                     (int)LoginToDialerState,
                     (int)DialerErrorCode.Success,
                     0,
                     false));

            return interview;
        }

        public BvInterviewEntity NotConnectToInterview_Predictive(int interviewerId, State expectedState, BvInterviewEntity interview, CallOutcome outcome)
        {
            var call = CallQueueService.GetCallAndNoLock(SurveySID, interview.ID);
            DialerHelper.SendEventNotifyOutcome(CampaignId, interviewerId, call.CallID, outcome);

            CheckState(expectedState);

            return interview;
        }

        public BvInterviewEntity PreviewScreenPopToInterview_Predictive(BvInterviewEntity interview)
        {
            var call = CallQueueService.GetCallAndNoLock(SurveySID, interview.ID);

            DialerHelper.SendEventScreenPop(CampaignId, PersonSID, interview.ID, call.CallID, DialingMode.Preview);

            CheckState(new State(SurveyName, null, interview.ID, InterviewUrl(interview), null,
                     (int)InterviewState.INTERVIEWING,
                     (int)CallOutcome.NotDefined,
                     (int)LoginState.LOGGED_IN,
                     (int)LoginToDialerState,
                     (int)DialerErrorCode.Success,
                     0,
                     false));

            return interview;
        }

        public void Dial_Predictive(BvInterviewEntity interview, DialingMode diallingMode, bool result)
        {
            if (diallingMode == DialingMode.Predictive)
            {
                WS.Dial(null, 0, 1);

                CheckState(new State(SurveyName, null, interview.ID, InterviewUrl(interview), null,
                     (int)InterviewState.INTERVIEWING,
                     (int)CallOutcome.NotDefined,
                     (int)LoginState.LOGGED_IN,
                     (int)LoginToDialerState,
                     (int)DialerErrorCode.Success,
                     0,
                     false));

                return;
            }

            if (diallingMode == DialingMode.Preview)
            {
                var call = CallQueueService.GetCallAndNoLock(SurveySID, interview.ID);

                DialerHelper.AddRequestCompletePreview();

                WS.Dial(null, 0, 1);

                CheckState(new State(SurveyName, null, interview.ID, null, null,
                     (int)InterviewState.DIALLING,
                     (int)CallOutcome.NotDefined,
                     (int)LoginState.LOGGED_IN,
                     (int)LoginToDialerState,
                     (int)DialerErrorCode.Success,
                     0,
                     false));

                if (result)
                {
                    DialerHelper.SendEventConnected(CampaignId, PersonSID, call.CallID);
                }
                else
                {
                    DialerHelper.SendEventNotifyOutcome(CampaignId, PersonSID, call.CallID, CallOutcome.NoReply);
                }

                CheckState(new State(SurveyName, null, interview.ID, InterviewUrl(interview), null,
                     (int)InterviewState.INTERVIEWING,
                     (int)(result ? CallOutcome.Connected : CallOutcome.NoReply),
                     (int)LoginState.LOGGED_IN,
                     (int)LoginToDialerState,
                     (int)DialerErrorCode.Success,
                     0,
                     false));
            }
        }

        public void Hangup(BvInterviewEntity interview, int initiator)
        {
            var task = TaskRepository.GetByPerson(PersonSID);
            var callOutCome = (CallOutcome)task.CallOutcome;

            if (callOutCome == CallOutcome.Connected)
            {
                DialerHelper.AddRequestHangup();
            }

            WS.Hangup(initiator);

            CheckState(new State(SurveyName, null, interview.ID, InterviewUrl(interview), null,
                 (int)InterviewState.INTERVIEWING,
                 (int)callOutCome,
                 (int)LoginState.LOGGED_IN,
                 (int)LoginToDialerState,
                 (int)DialerErrorCode.Success,
                 0,
                 false));
        }

        public void CompleteInterview_Predictive(BvInterviewEntity interview)
        {
            CompleteInterview_Predictive(interview, LoginState.LOGGED_IN);
        }

        public void CompleteInterview_Predictive(BvInterviewEntity interview, LoginState loginState)
        {
            DialerHelper.AddRequestCompleteCall();

            WS.WrapUp(interview.ID, true, 1, new CompletedInterviewDetails { InterviewDuration = 0, Its = "13", Status = "Complete" });

            CheckState(new State(SurveyName, null, 0, null, null,
                 (int)InterviewState.WAITING,
                 (int)CallOutcome.NotDefined,
                 (int)loginState,
                 (int)LoginToDialerState,
                 (int)DialerErrorCode.Success,
                 0,
                 false));
        }

        public BvInterviewEntity StartInterview_HybridProgressive(BvInterviewEntity interview)
        {
            string projectId = SurveyRepository.GetById(interview.SurveySID).Name;

            if ((DialingMode)interview.DialingMode == DialingMode.Preview)
            {
                return StartInterview_ManualOrPreview(projectId, interview.ID);
            }

            return StartInterview_Progressive(projectId, interview.ID);
        }

        public void Dial_HybridProgressive(BvInterviewEntity interview, bool isDialSucceed)
        {
            const int initiator = 0;

            if ((DialingMode)interview.DialingMode == DialingMode.Preview)
            {
                CallOutcome callOutcome = isDialSucceed ? CallOutcome.Completed : CallOutcome.ReturnedNotDialled;
                Dial(interview, initiator, isDialSucceed, callOutcome);
            }
        }

        public void CheckActivityView(Action<BvSpGetListSurveyTasksEntity> checkFunction)
        {
            using (var surveysBatch = TransferBatch.Create())
            {
                surveysBatch.Insert(new[] { SurveySID });

                var permissionRepository = ServiceLocator.Resolve<IUserSurveyPermissionRepository>();
                permissionRepository.Insert(SuperName, SurveyName);

                using (var interviewersBatch = TransferBatch.Create())
                {
                    interviewersBatch.Insert(new[] { PersonSID });

                    var entity =
                        BvSpGetListSurveyTasksAdapter.ExecuteEntityList(surveysBatch.Value,
                                                                        interviewersBatch.Value,
                                                                        ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId(),
                                                                        CallCenterTools.DefaultId, SuperName).FirstOrDefault();
                    Assert.IsNotNull(entity);

                    checkFunction(entity);
                }
            }
        }

        public void SetPendingBreakStatus_Predictive_SimultaneouslyDeliverCall(int interviewId, int callId)
        {
            var survey = SurveyRepository.GetById(SurveySID);

            var dialerEventsHandler = ServiceLocator.Resolve<IDialerEventsHandler>();
            DialerHelper.AddRequestGoNotReady(() => dialerEventsHandler.OnDialerNotifyOutcome(
                0,
                "",
                survey.CampaignId,
                PersonSID,
                interviewId.ToString(CultureInfo.InvariantCulture),
                callId,
                (int)CallOutcome.Connected,
                null,
                TimeSpan.FromSeconds(0),
                null));

            WS.SetPendingBreakStatus(PendingBreakStatus.Break, 1);
        }

        public void InitializeWithExistsSurveyAndPerson(BvSurveyEntity survey, int personId, IEnumerable<BvInterviewEntity> interviews)
        {
            _surveyName = survey.Name;
            SurveySID = survey.SID;
            PersonSID = personId;
            _interviews = interviews.ToArray();


        }

        public void CheckValueInBvInterview(int interviewId, string field, object value)
        {
            using (var connection = new SqlConnection(IntegrationTestingFramework.Instance.DbEngine.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM BvInterview WHERE ID = @InterviewID", connection))
                {
                    command.Parameters.AddWithValue("@InterviewID", interviewId);

                    using (SqlDataReader sdr = command.ExecuteReader())
                    {
                        Assert.IsTrue(sdr.Read());
                        Assert.AreEqual(value, sdr[field]);
                    }
                }
            }
        }
    }
}
