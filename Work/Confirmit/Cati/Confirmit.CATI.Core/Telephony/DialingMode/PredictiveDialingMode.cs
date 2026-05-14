using System.Collections.Generic;
using System.Globalization;
using BvCallHandlerLibrary;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.DialingWorkflow
{
    public class PredictiveDialingMode : DialingMode
    {
        public PredictiveDialingMode() 
            : base(ConfirmitDialerInterface.DialingMode.Predictive)
        {
            
        }
        public override void CheckPersonCanLoginToDialer(BvPersonEntity person)
        {
            if ((AgentTaskChoiceMode) person.ManualSelection == AgentTaskChoiceMode.Manual)
                throw new ManualUserInPredictiveModeException(
                    "ConsoleService:LoginToDialer. Predictive dialing surveys are not available in manual selection mode.");
        }

        public override DialerErrorCode Login(BvPersonEntity person, BvTasksEntity task, BvSurveyEntity survey, string extensionNumber, IEnumerable<KeyValuePair<string, string>> personDialerAttributes)
        {
            // TODO survey != null : survey must not be null because it is predictive, may be we need to throw if survey == null
            var campaignId = survey != null ? survey.CampaignId : 0;

            CheckPersonCanLoginToDialer(person);

            //Reflect user logging in to dialer state in BvTasks table, obtain dialer userId.
            BvSpTasks_InsertUpdate_2Adapter.ExecuteNonQuery(
                task.PersonSID,
                (survey != null) ? survey.SID : 0,
                extensionNumber,
                (byte)LoginState.LOGGING_IN,
                false,
                (byte)Mode);

            TaskService.MoveTaskToState(task, InterviewState.WAITING, Mode);

            var telephony = ServiceLocator.Resolve<ITelephony>();

            return telephony.Login(
                task.DialerId,
                campaignId,
                person.SID.ToString(CultureInfo.InvariantCulture),
                person.Name,
                (AgentType)person.Type,
                extensionNumber,
                string.Empty,
                true,
                task.IsDialerAgentLocal,
                personDialerAttributes);
        }

        public override void BeforeStartInterview(BvTasksEntity task, BvPersonEntity person)
        {
            var taskDialType = (DialType)task.DialTypeId;

            //Check some possible interviewer mode inconsistencies for predictive surveys 
            if (BvCallHandlerRoot.IsLoggedInToDialer(task))
            {
                //It means logged in to dialer
                var dielerDialType = (DialType)ServiceLocator.Resolve<IDialersRepository>().GetById(task.DialerId).DialTypeId;
                
                if (taskDialType != dielerDialType)
                {
                    throw new ManualUserInPredictiveModeException(
                        string.Format("Person with dialType={0} should not be logged in to dialer with dialtype={1}.", 
                        taskDialType, dielerDialType));
                }

                if (person.ManualSelection == (int)AgentTaskChoiceMode.Manual)
                {
                    throw new ManualUserInPredictiveModeException(
                        "Predictive dialing surveys are not available in manual selection mode.");
                }
            }
            else
            {
                if (taskDialType == DialType.Cellphone)
                {
                    return;
                }

                //Not logged in to dialer
                if (person.ManualSelection != (int)AgentTaskChoiceMode.Manual)
                {
                    throw new PredictiveSurveyWithoutDialerException(
                        "Predictive dialing surveys without a dialer are only available in manual selection mode.");
                }
            }
        }

        public override void StartInterview(
            int personId,
            int dialerId,
            BvSurveyEntity survey,
            BvInterviewEntity interview,
            int timezoneId)
        {
            // In case of predictive the person record in BvTasks must already be updated
            // (from OnConnected handler)
            // So do nothing.
        }
    }
}
