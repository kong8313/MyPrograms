using System;
using System.Diagnostics;
using System.Linq;
using BvCallHandlerLibrary;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Adapter.TableType;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony.DialingWorkflow;
using ConfirmitDialerInterface;
using DialingMode = ConfirmitDialerInterface.DialingMode;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public class ConsoleLoginToDialerProcessor : IConsoleLoginToDialerProcessor
    {
        private readonly IDialerCollection _dialerCollection;
        private readonly ITelephony _telephony;
        private readonly IDialersRepository _dialersRepository;
        private readonly IDialerSettings _dialerSettings;
        private readonly ISurveyRepository _surveyRepository;

        public ConsoleLoginToDialerProcessor(
            IDialerCollection dialerCollection,
            ITelephony telephony,
            IDialersRepository dialersRepository,
            IDialerSettings dialerSettings,
            ISurveyRepository surveyRepository)
        {
            _dialerCollection = dialerCollection;
            _telephony = telephony;
            _dialersRepository = dialersRepository;
            _dialerSettings = dialerSettings;
            _surveyRepository = surveyRepository;
        }

        public BvSurveyEntity LoginToDialer(
            BvPersonEntity person,
            BvTasksEntity task,
            string extensionNumber,
            BvSurveyEntity survey,
            out bool isPredictive)
        {
            isPredictive = false;

            var personMode = (AgentTaskChoiceMode)person.ManualSelection;

            if (string.IsNullOrEmpty(extensionNumber))
            {
                if (string.IsNullOrEmpty(task.StationExtensionNumber) == false)
                {
                    extensionNumber = task.StationExtensionNumber;
                }
                else
                {
                    throw new InternalErrorException(String.Format(
                        "ConsoleService.LoginToDialer: Person tries to login to dialer but interviewer extension number is empty. " +
                        "/// personId={0}, dialerId={1}",
                        task.PersonSID,
                        task.DialerId));
                }
            }

            survey = SetInitialSurveyForAutomaticMode(survey, person);

            var dialingModeType = DialingMode.Preview; // If no concrete survey is pointed then use preview login logic

            if (survey != null)
            {
                dialingModeType = BvCallHandlerRoot.GetDialingMode(task, survey, null);
            }

            //Here we have just known which survey was selected in survey assignment mode
            //so we should rebuild calls cahce for this person
            if (personMode == AgentTaskChoiceMode.CampaignAssignment &&
                (task == null || task.SurveySID == 0) && survey != null)
            {
                PersonService.LoginPersonOnSurveyForSurveySelectionMode(task.PersonSID, survey.SID);
            }

            var surveySid = (survey != null) ? survey.SID : 0;

            var dialingMode = DialingModeFactory.CreateDialingMode(dialingModeType);

            dialingMode.CheckPersonCanLoginToDialer(person);

            AssignActiveDialerIfNeeded(task, surveySid);

            isPredictive = dialingMode.GetType() == typeof(PredictiveDialingMode);

            var personDialerAttributes = PersonService.GetPersonDialerAttributes(person);

            var beginLoginResult = dialingMode.Login(
                person,
                task,
                survey,
                extensionNumber,
                personDialerAttributes);

            switch (beginLoginResult)
            {
                case DialerErrorCode.Success:
                    break; //Success

                case DialerErrorCode.AgentAlreadyLoggedIn:
                    Trace.TraceWarning(
                        "ConsoleService:LoginToDialer: Person is already logged in to dialer. " +
                        "/// personId={0}, dialerId={1}",
                        task.PersonSID,
                        task.DialerId);

                    // Try GoReady, and continue to work with the agent
                    // In fact the following code is the same as in 
                    // \\Projects\Units\bv7\Multimode_V14\BvCallHandlerLibrary\MNTCILibraryEvents.cs
                    // (//EndLogin event). Probably the merge of these to methods will be made 
                    // along with dialer code refactoring. Not now.
                    // Note, campaignId can be "" at this point. It means that 
                    // user was logged in via RCLogin, so neither survey nor interview were defined.
                    var result = _telephony.GoReady(
                        task.DialerId,
                        survey != null ? survey.CampaignId : 0,
                        task.PersonSID.ToString());

                    if (result != DialerErrorCode.Success &&
                        result != DialerErrorCode.AgentAlreadyLoggedIn)
                    {
                        Trace.TraceWarning(
                            "ConsoleService:LoginToDialer: Person login to dialer failed with GoReady error. " +
                            "/// personId={0}, dialerId={1}, GoReady error code={2}",
                            task.PersonSID,
                            task.DialerId,
                            result);

                        BvSpTasks_UpdateProblemStateAdapter.ExecuteNonQuery(
                            task.PersonSID,
                            (int)result);
                        break;
                    }

                    //Reflect logged in to dialer state in BvTasks table
                    BvSpTasks_UpdateLoggedInToDialerStateAdapter.ExecuteNonQuery(
                        task.PersonSID,
                        (byte)LoginState.LOGGED_IN);

                    //Predictive support:
                    //May be we should call SetGroups here as well. But may be not.

                    break;

                default:

                    Trace.TraceWarning("ConsoleService:LoginToDialer: Person login to dialer failed. personId={0}, dialerId={1}, error code={2}",
                                      task.PersonSID, task.DialerId, beginLoginResult);

                    BvSpTasks_UpdateLoggedInToDialerStateAdapter.ExecuteNonQuery(
                        task.PersonSID,
                        (byte)LoginState.NOT_LOGGED_IN);

                    break;
            }

            return survey;
        }

        private BvSurveyEntity SetInitialSurveyForAutomaticMode(BvSurveyEntity survey, BvPersonEntity person)
        {
            //Currently for CODI dialers "Automatic" mode is only supported by Sytel and it is required that we login person to a any survey before we send fist SendNumberToAgent
            //All subsequent SendNumberToAgent could be sent for any survey. So we take first assigned survey to login to dialler.
            //TODO: Later on we should probably need to remove SetCampaign for non-predictive surveys

            if (_dialerSettings.Dialer == DiallerType.Generic && person.ManualSelection == (int) AgentTaskChoiceMode.Automatic)
            {
                survey = survey ?? PersonService.GetOpenedSurveysForInterviewer(person.SID )
                                    .FirstOrDefault(s => s.DialingMode == DialingMode.Preview || s.DialingMode == DialingMode.Automatic);
                if (survey == null)
                {
                    throw new InternalErrorException(
                        $"ConsoleService.LoginToDialer: There are no assigned surveys for this person - personId={person.SID}");
                }
            }

            return survey;
        }

        private void AssignActiveDialerIfNeeded(BvTasksEntity task, int surveySid)
        {
            if (task.DialerId != 0)
            {
                if (!_dialerCollection.IsDialerInitialized(task.DialerId))
                {
                    throw new InternalErrorException(
                        "ConsoleLoginToDialerProcessor: Person tries to login to dialer but the dialer is not initialized. " +
                        $"/// personId={task.PersonSID}, dialerId={task.DialerId}");
                }

                if (!_dialersRepository.GetById(task.DialerId).IsActive)
                {
                    throw new LoginToInactiveDialerException(
                        "ConsoleLoginToDialerProcessor: Person tries to login to dialer but the dialer is not activated. " +
                        $"/// personId={task.PersonSID}, dialerId={task.DialerId}", task.DialerId);
                }

                return;
            }

            var dialerId = _dialersRepository.GetNextAvailableDialer(surveySid, (DialType)task.DialTypeId);

            if (!dialerId.HasValue)
            {
                throw new InternalErrorException(
                    "ConsoleLoginToDialerProcessor: Person tries to login to dialer but there is no active dialer available. " +
                    $"/// personId={task.PersonSID}");
            }

            task.DialerId = dialerId.Value;

            var taskRepository = ServiceLocator.Resolve<ITaskRepository>();
            taskRepository.Update(task);
        }
    }
}
