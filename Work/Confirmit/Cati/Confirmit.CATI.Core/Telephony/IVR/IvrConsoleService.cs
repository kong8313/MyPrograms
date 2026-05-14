using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.PersonLogin;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony.Console;
using Confirmit.CATI.Core.Telephony.IVR.Interfaces;
using Confirmit.SurveyVoiceXml.Service.Client;
using Confirmit.SurveyVoiceXml.Service.Client.Models;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony
{
    public class IvrConsoleService : IIvrConsoleService
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly IPersonRepository _personRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IInternalVoiceXmlApiFactory _internalVoiceXmlApiFactory;
        private readonly IInterviewService _interviewService;
        private readonly IInterviewRepository _interviewRepository;
        private readonly IDialersRepository _dialersRepository;
        private readonly IDialerAvailabilityManager _dialerAvailabilityManager;
        private readonly ITelephony _telephony;
        private readonly IConsoleWrapUpProcessor _consoleWrapUpProcessor;
        private readonly IConsoleStartInterviewProcessor _consoleStartInterviewProcessor;
        private readonly IConsoleLoginProcessor _consoleLoginProcessor;
        private readonly IConsoleLoginToDialerProcessor _consoleLoginToDialerProcessor;
        private readonly IStationInfoFactory _stationInfoFactory;
        private readonly IToggleSettings _toggleSettings;
        private readonly IIvrSettings _ivrSettings;
        private readonly IIvrSettingsRepository _ivrSettingsRepository;
        private readonly IConsoleTransferStartProcessor _consoleTransferStartProcessor;
        private readonly IConsoleTransferCompleteProcessor _consoleTransferCompleteProcessor;
        private readonly IConsoleTransferCancelProcessor _consoleTransferCancelProcessor;
        private readonly ICompanyInfo _companyInfo;
        private readonly IIvrVariablesProvider _ivrVariablesProvider;
        private readonly IConsoleTransferProcessProcessor _consoleTransferProcessProcessor;
        private readonly IAsyncManager _asyncManager;

        public IvrConsoleService(
            ISurveyRepository surveyRepository,
            IPersonRepository personRepository,
            ITaskRepository taskRepository,
            IInterviewRepository interviewRepository,
            IDialersRepository dialersRepository,
            IInterviewService interviewService,
            IInternalVoiceXmlApiFactory internalVoiceXmlApiFactory,
            IDialerAvailabilityManager dialerAvailabilityManager,
            ITelephony telephony,
            IConsoleWrapUpProcessor consoleWrapUpProcessor,
            IConsoleStartInterviewProcessor consoleStartInterviewProcessor,
            IConsoleLoginProcessor consoleLoginProcessor,
            IConsoleLoginToDialerProcessor consoleLoginToDialerProcessor,
            IConsoleTransferStartProcessor consoleTransferStartProcessor,
            IConsoleTransferCompleteProcessor consoleTransferCompleteProcessor,
            IConsoleTransferCancelProcessor consoleTransferCancelProcessor,
            IStationInfoFactory stationInfoFactory,
            IToggleSettings toggleSettings,
            IIvrSettings ivrSettings,
            IIvrSettingsRepository ivrSettingsRepository,
            ICompanyInfo companyInfo,
            IIvrVariablesProvider ivrVariablesProvider,
            IConsoleTransferProcessProcessor consoleTransferProcessProcessor, 
            IAsyncManager asyncManager)
        {
            _surveyRepository = surveyRepository;
            _personRepository = personRepository;
            _taskRepository = taskRepository;
            _internalVoiceXmlApiFactory = internalVoiceXmlApiFactory;
            _interviewService = interviewService;
            _interviewRepository = interviewRepository;
            _dialersRepository = dialersRepository;
            _dialerAvailabilityManager = dialerAvailabilityManager;
            _telephony = telephony;
            _consoleWrapUpProcessor = consoleWrapUpProcessor;
            _consoleStartInterviewProcessor = consoleStartInterviewProcessor;
            _consoleLoginProcessor = consoleLoginProcessor;
            _consoleLoginToDialerProcessor = consoleLoginToDialerProcessor;
            _stationInfoFactory = stationInfoFactory;
            _toggleSettings = toggleSettings;
            _ivrSettings = ivrSettings;
            _ivrSettingsRepository = ivrSettingsRepository;
            _consoleTransferStartProcessor = consoleTransferStartProcessor;
            _consoleTransferCompleteProcessor = consoleTransferCompleteProcessor;
            _consoleTransferCancelProcessor = consoleTransferCancelProcessor;
            _companyInfo = companyInfo;
            _ivrVariablesProvider = ivrVariablesProvider;
            _consoleTransferProcessProcessor = consoleTransferProcessProcessor;
            _asyncManager = asyncManager;
        }

        public void ExecutePeriodicalWork(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!_toggleSettings.EnableIVR || _toggleSettings.CatiAgent.IvrThread)
            {
                return;
            }

            foreach( var ivrConsole in GetIvrConsoles().ToArray())
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                
                try
                {
                    ManageIvrAgentLifecycle(ivrConsole.Person, ivrConsole.Task);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(
                        "Following error occured during processing of IVR agent '{0}' with name '{1}': Exception:{2}",
                        ivrConsole.Person.SID, ivrConsole.Person.Name, ex);
                }
            }
        }

        private class IvrConsole
        {
            public BvTasksEntity Task;
            public BvPersonEntity Person;
        }

        private IEnumerable<IvrConsole> GetIvrConsoles()
        {
            using (var reader = BvSpPerson_ListWithTasksByTypeAdapter.ExecuteReader((int) AgentType.IvrAgent))
            {
                while (reader.Read())
                {
                    yield return new IvrConsole()
                    {
                        Person = BvPersonAdapter.ReadEntity(reader),
                        Task = reader["PersonSID"] is DBNull ? null : BvTasksAdapter.ReadEntity(reader)
                    };
                }
            }
        }

        private void ManageIvrAgentLifecycle(BvPersonEntity person, BvTasksEntity task)
        {
            if (person.IsLocked)
            {
                return;
            }
            
            if (IsPersonLoggedInToDialer(task))
            {
                if (!_dialerAvailabilityManager.IsDialerInitializedAndAvaialble(task.DialerId))
                {
                    TaskService.TerminateTask(
                        task.PersonSID,
                        new DatabaseTransactionOptions("IvrConsoleService.TerminateTask", DeadlockPriority.Supervisor));

                    return;
                }
            }

            if (!IsPersonLoggedIn(task))
            {
                task = Login(person, task);
            }

            if (!IsPersonLoggedInToDialer(task))
            {
                var initialSurvey = PersonService.GetOpenedSurveysForInterviewer(person.SID)
                    .FirstOrDefault(survey => survey.DialingMode == DialingMode.Automatic || 
                                              survey.DialingMode == DialingMode.Preview);

                if (initialSurvey == null)
                {
                    return; 
                }

                var dialerId = _dialersRepository.GetNextAvailableDialer(task.SurveySID, (DialType)Enum.Parse(typeof(DialType), task.DialTypeId.ToString()), task.CallCenterID);

                if (dialerId == null)
                {
                    return;
                }


                task = LoginToDialer(person, task, initialSurvey);
            }

            if (IsPersonReadyToStartInterview(task))
            {
                StartInterview(person, task);
            }

            if(_consoleTransferProcessProcessor.ShouldProcessTransfer(task))
                _consoleTransferProcessProcessor.ProcessTransfer(person);
        }

        private static bool IsPersonReadyToStartInterview(BvTasksEntity task)
        {
            return IsPersonLoggedIn(task) &&
                   task.InterviewState == (int)InterviewState.NO_CALLS &&
                   task.LoggedInToDialerState == (int)LoginState.LOGGED_IN &&
                   task.ProblemId == 0;
        }

        private static bool IsPersonLoggedIn(BvTasksEntity task)
        {
            return task != null && task.StatusLogout != (int)LoginState.NOT_LOGGED_IN;
        }

        private static bool IsPersonLoggedInToDialer(BvTasksEntity task)
        {
            return IsPersonLoggedIn(task) && task.LoggedInToDialerState != (int)(LoginState.NOT_LOGGED_IN);
        }

        private List<LanguageSpecificSettingsModel> GetLanguageSpecificSettingsModel()
        {
            var ivrLanguageSpecificSettings = _ivrSettingsRepository.GetAll();

            var result = new List<LanguageSpecificSettingsModel>();

            foreach (var ivrLanguageSpecificSetting in ivrLanguageSpecificSettings)
            {
                result.Add(new LanguageSpecificSettingsModel()
                {
                    WrongInputAudioUrl = ivrLanguageSpecificSetting.WrongInputAudioUrl,
                    WrongInputExitAudioUrl = ivrLanguageSpecificSetting.WrongInputExitAudioUrl,
                    WrongInputExitText = ivrLanguageSpecificSetting.WrongInputExitText,
                    WrongInputText = ivrLanguageSpecificSetting.WrongInputText,
                    LanguageId = ivrLanguageSpecificSetting.LanguageId
                });
            }

            return result;
        }

        private SettingsModel GetSettingsModel()
        {
            return new SettingsModel
            {
                Beep = _ivrSettings.Beep,
                DtmfTerm = _ivrSettings.DtmfTerm,
                FinalSilence = _ivrSettings.FinalSilence,
                MaxTime = _ivrSettings.MaxTime,
                TermChar = _ivrSettings.TermChar,
                RecordType = _ivrSettings.RecordType,
                LanguageSpecificSettings = GetLanguageSpecificSettingsModel()
            };
        }

        private VoiceXmlPagePostModel GetVoceXmlPagePostModel(BvTasksEntity task, bool setSid, KeyValuePair<string, string>[] variables)
        {
            var survey = _surveyRepository.GetById(task.SurveySID);
            var interview = _interviewRepository.GetByIdWithCheck(task.SurveySID, task.InterviewID);
            var callId = task.CallID.GetValueOrDefault();

            var voiceXmlPagePostModel = new VoiceXmlPagePostModel()
            {
                CompanyId = _companyInfo.CompanyId,
                ProjectId = survey.ProjectId,
                InterviewId = interview.ID,
                CallId = callId,
                InterviewerId = task.PersonSID,
                Sid = setSid ? _interviewService.GenereteSecurityKey(interview) : null,
                Variables = _ivrVariablesProvider.ConvertToIvrVariables(variables),
                SettingsModel = GetSettingsModel(),
            };

            return voiceXmlPagePostModel;
        }

        public void ProcessAgentState(BvTasksEntity task)
        {
            var person = _personRepository.GetById(task.PersonSID);

            if (person.Type != (byte)AgentType.IvrAgent || _toggleSettings.CatiAgent.IvrThread)
            {
                return;
            }
            
            ManageIvrAgentLifecycle(person, task);
        }

        public void ProcessTransferState(BvActiveDialEntity dial, string transferId, TransferState transferState)
        {
            if (dial.MainPersonId == 0)
                return;

            if (transferState.TargetState != TargetState.Connected)
                return;

            var person = _personRepository.GetById(dial.MainPersonId);

            if (person?.Type != (byte)AgentType.IvrAgent)
                return;

            using (TaskLocker.TryLock(dial.MainPersonId, out var task))
            {
                if (task == null || task.Context.TransferId != transferId)
                    return;

                var tcEvt = new TransferCompleteEvent();

                _consoleTransferCompleteProcessor.TransferComplete(task, person, tcEvt);

                _taskRepository.Update(task);

                tcEvt.Save();
            }
        }

        public void ProcessCallOnConnect(BvTasksEntity task)
        {
            var person = _personRepository.GetById(task.PersonSID);

            if (person.Type != (byte)AgentType.IvrAgent)
            {
                return;
            }

            if (task.InterviewState == (int)InterviewState.INCOMING_TRANSFER &&
                !_toggleSettings.CatiAgent.IvrThread)
            {
                ManageIvrAgentLifecycle(person, task);
                return;
            }

            if (task.InterviewState == (int)InterviewState.INTERVIEWING)
            {
                var service = _internalVoiceXmlApiFactory.CreateApiClient();

                var page = service.Main.InitialPage(GetVoceXmlPagePostModel(task, true,
                    new KeyValuePair<string, string>[0]));

                ProcessVoiceXmlPage(person, task, page);
            }
        }

        public void ProcessIvrSubmit(BvTasksEntity task, long campaignId, KeyValuePair<string, string>[] variables)
        {
            // Verification that current task and data from voice xml have the same survey id and interviewer id
            var survey = _surveyRepository.GetByCampaignId(campaignId);
            int? interviewId = _ivrVariablesProvider.GetInterviewId(variables);
            if (survey.SID != task.SurveySID || interviewId != task.InterviewID)
            {
                Trace.TraceWarning(
                    "Got voice xml page from different interview. Current survey sid={0}, current interview id={1}, survey sid from voice xml={2}, interview id from voice xml={3}.",
                    task.SurveySID, task.InterviewID, survey.SID, interviewId.HasValue ? interviewId.ToString() : "[no value]. See previous warning for additional information ");
                return;
            }

            var person = _personRepository.GetById(task.PersonSID);

            var service = _internalVoiceXmlApiFactory.CreateApiClient();

            var page = service.Main.NextPage(GetVoceXmlPagePostModel(task, false, variables));

            ProcessVoiceXmlPage(person, task, page);
        }

        private void ProcessVoiceXmlPage(BvPersonEntity person, BvTasksEntity task, VoiceXmlPageModel page)
        {
            var survey = _surveyRepository.GetById(task.SurveySID);

            if(page.TransferConfiguration != null)
            {
                var options = new TransferOptions()
                {
                    Type = (ConsoleTransferType) Enum.Parse(typeof(ConsoleTransferType), page.TransferConfiguration.Type),
                    Resource = page.TransferConfiguration.TransferTarget
                };

                var tsEvt = new TransferStartEvent(options);

                _consoleTransferStartProcessor.TransferStart(task, person, options, tsEvt);

                _taskRepository.Update(task);

                tsEvt.Save();

                if (options.Type == ConsoleTransferType.InternalCold ||
                    options.Type == ConsoleTransferType.ExternalCold)
                {
                    var tcEvt = new TransferCompleteEvent();

                    _consoleTransferCompleteProcessor.TransferComplete(task, person, tcEvt);

                    _taskRepository.Update(task);

                    tcEvt.Save();
                }
                else
                {
                    var asyncTask = _asyncManager.CreateTask(() => TransferCancelHandler(task.PersonSID, task.Context.TransferId, page));

                    _asyncManager.ScheduleTask(_ivrSettings.TransferTimeout, asyncTask);
                }
            }
            else if (!(bool)page.IsLastPage)
            {
                _telephony.IvrRenderVoiceXml(
                    task.DialerId,
                    _companyInfo.CompanyId,
                    survey.CampaignId,
                    task.PersonSID, 
                    task.InterviewID,
                    page.VoiceXml);
            }
            else
            {
                var evt = new WrapUpEvent();
                var details = new CompletedInterviewDetails
                {
                    InterviewDuration = page.Duration.GetValueOrDefault(),
                    Its = page.Its,
                    Status = page.Status
                };

                evt.Details.InterviewDetails = details;

                _consoleWrapUpProcessor.WrapUp(person, task, task.InterviewID, true, 1, details, WrapUpReason.CompeteInterview, evt);

                evt.Save();
            }
        }

        private void TransferCancelHandler(int personId, string transferId, VoiceXmlPageModel page)
        {
            using (TaskLocker.TryLock(personId, out var task))
            {
                if (task == null)
                    return;

                if (task.InterviewState != (byte) InterviewState.OUTGOING_TRANSFER ||
                    task.Context.TransferId != transferId)
                    return;

                var tcEvt = new TransferCancelEvent();

                var person = _personRepository.GetById(personId);

                _consoleTransferCancelProcessor.TransferCancel(task, person, tcEvt);

                _taskRepository.Update(task);

                tcEvt.Save();

                var survey = _surveyRepository.GetById(task.SurveySID);

                var variables = GetVariablesFromVoiceXml(page.VoiceXml);

                ProcessIvrSubmit(task, survey.CampaignId, variables);
            }
        }

        public static KeyValuePair<string, string>[] GetVariablesFromVoiceXml(string voiceXml)
        {
            var doc = XDocument.Parse(voiceXml);

            RemoveNamespaces(doc);

            var formTag = doc.XPathSelectElement("vxml/form");

            if (formTag == null)
            {
                throw new Exception("No valid voice xml found.");
            }

            var variables = formTag.Descendants("var").Select(
                item => new KeyValuePair<string, string>(
                    item.Attribute("name").Value,
                    item.Attribute("expr").Value)).ToArray();
            return variables;
        }

        private static void RemoveNamespaces(XDocument doc)
        {
            foreach (var e in doc.Root.DescendantsAndSelf())
            {
                if (e.Name.Namespace != XNamespace.None)
                {
                    e.Name = XNamespace.None.GetName(e.Name.LocalName);
                }

                e.ReplaceAttributes(
                    e.Attributes()
                        .Select(
                            a =>
                                a.IsNamespaceDeclaration
                                    ? null
                                    : a.Name.Namespace != XNamespace.None
                                        ? new XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value)
                                        : a));
            }
        }


        private ConfirmitDialerInterface.TransferType ConvertType(string type)
        {
            switch(type)
            {
                case "InternalCold":
                    return ConfirmitDialerInterface.TransferType.InternalCold;
                default:
                    throw new NotImplementedException($"Not suported type '{type}'. Supported types are 'IvrToAgent' and 'AgentToIvr'");
            }
        }

        private void StartInterview(BvPersonEntity person, BvTasksEntity task)
        {
            var activityEvent = new StartInterviewEvent();

            var survey = _consoleStartInterviewProcessor.Startinterview(person, task, null, 0, activityEvent);

            activityEvent.Save(
                task.PersonSID,
                0,
                (survey != null) ? (int?)survey.SID : null,
                (survey != null) ? survey.Name : null);
        }

        private BvTasksEntity Login(BvPersonEntity person, BvTasksEntity task)
        {
            var activityEvent = new LoginEvent();

            using (new EventDetailsScope(activityEvent.Details))
            {

                var stationInfo = _stationInfoFactory.Create("ivr01", person);

                bool isAlreadyLoggedIn;
                task = _consoleLoginProcessor.Login(person, task, stationInfo, out isAlreadyLoggedIn);

                activityEvent.Details.DialerInfo = _consoleLoginProcessor.GetDialerInfo(task, stationInfo, isAlreadyLoggedIn);
                activityEvent.Details.PersonInfo = _consoleLoginProcessor.GetPersonInfo(person, task, isAlreadyLoggedIn);

            }

            activityEvent.Save(person.SID);

            return UpdateInterviewState(task.PersonSID, InterviewState.SELECTING);
        }

        private BvTasksEntity UpdateInterviewState(int personId, InterviewState state)
        {
            BvSpTasks_UpdateInterviewStateAdapter.ExecuteNonQuery(personId, (int)state,
                (byte) DialingMode.Automatic);

            return _taskRepository.GetByPerson(personId);
        }

        private BvTasksEntity LoginToDialer(BvPersonEntity person, BvTasksEntity task, BvSurveyEntity survey)
        {
            var activityEvent = new LoginToDialerEvent();

            bool isPredictive;

            _consoleLoginToDialerProcessor.LoginToDialer(person, task, null, survey, out isPredictive);

            activityEvent.Save(
                task.PersonSID,
                survey.SID,
                survey.Name,
                task.DialerId,
                null,
                isPredictive);

            return UpdateInterviewState(task.PersonSID, InterviewState.NO_CALLS);
        }
    }
}