using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.TimeService;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.DialingWorkflow
{
    public class DialingMode : IDialingMode
    {
        protected ConfirmitDialerInterface.DialingMode Mode { get; private set; }

        public DialingMode(ConfirmitDialerInterface.DialingMode mode)
        {
            Mode = mode;
        }

        public virtual void CheckPersonCanLoginToDialer(BvPersonEntity person)
        {

        }

        public virtual DialerErrorCode Login(
            BvPersonEntity person, 
            BvTasksEntity task,
            BvSurveyEntity survey,
            string extensionNumber,
            IEnumerable<KeyValuePair<string, string>> personDialerAttributes)
        {
            var campaignId = (survey != null) ? survey.CampaignId : 0;

            // Reflect user logging in to dialer state in BvTasks table, obtain dialer userId.

            //TODO: the stored procedure must not insert, only update, check and rename BvSpTasks_InsertUpdate_2
            //Special handling for Automatic mode - do not store survey id in bvtasks.
            BvSpTasks_InsertUpdate_2Adapter.ExecuteNonQuery(
                task.PersonSID,
                //MaximL:May be we should use condition like: person.ManualSelection == (int) AgentTaskChoiceMode.SurveyAssignment
                (survey != null && person.ManualSelection != (int) AgentTaskChoiceMode.Automatic) ? survey.SID : 0,
                extensionNumber,
                (byte)LoginState.LOGGING_IN,
                true,
                (byte)Mode);

            var telephony = ServiceLocator.Resolve<ITelephony>();

            return telephony.Login(
                task.DialerId,
                campaignId,
                person.SID.ToString(CultureInfo.InvariantCulture),
                person.Name,
                (AgentType)person.Type,
                extensionNumber,
                string.Empty,
                false,
                task.IsDialerAgentLocal,
                personDialerAttributes);
        }

        public virtual void BeforeStartInterview(BvTasksEntity task, BvPersonEntity person)
        {
            TaskService.MoveTaskToState(task, InterviewState.WAITING, Mode);
        }

        public virtual void StartInterview(
            int personId,
            int dialerId,
            BvSurveyEntity survey,
            BvInterviewEntity interview,
            int timezoneId)
        {
            var rowsAffected = UpdateBvTasks(
                personId,
                survey.SID,
                interview.ID,
                InterviewState.INTERVIEWING,
                ServiceLocator.Resolve<ITimeService>().GetUtcNow(),
                CallOutcome.NotDefined,
                timezoneId);

            if (rowsAffected == 0)
            {
                // Seems the person is not still logged in
                Trace.TraceError(
                    "DialingMode.StartInterview: The person is not logged in." +
                    "interviewId = {0}, personId = {1}, dialerId = {2}, " +
                    "surveyId = {3}, surveyName = {4}, respondentTelephoneNumber = {5}, " +
                    "timezoneId = {6}, callerId = {7}",
                    interview.ID,
                    personId,
                    dialerId,
                    survey.SID,
                    survey.Name,
                    interview.TelephoneNumber,
                    timezoneId,
                    interview.ExtensionNumber);
            }
        }

        // Auxilary functions
        // TODO: refactor the next two functions: either move to another class or delete them at all
        // (in the latter case refactoring of BvSpTasks_Update_2 stored procedure is required)
        protected int UpdateBvTasks(
            int personSid,
            int surveySid,
            int interviewId,
            InterviewState interviewState,
            DateTime? timeCallDelivered,
            CallOutcome callOutcome,
            int tzId)
        {
            int callId; // fake, when we do not need to return this value

            return UpdateBvTasks(
                Mode,
                personSid,
                surveySid,
                interviewId,
                interviewState,
                timeCallDelivered,
                callOutcome,
                tzId,
                out callId);
        }

        public static int UpdateBvTasks(
            ConfirmitDialerInterface.DialingMode mode,
            int personSid,
            int surveySid,
            int interviewId,
            InterviewState interviewState,
            DateTime? timeCallDelivered,
            CallOutcome callOutcome,
            int tzId,
            out int callId)
        {
            var taskUpdateEntity = BvSpTasks_Update_2Adapter.ExecuteEntity(
                personSid,
                surveySid,
                interviewId,
                (byte)interviewState,
                timeCallDelivered,
                (int)callOutcome,
                tzId,
                (byte)mode);

            int rowsAffected = 0;
            callId = 0;
            if (taskUpdateEntity != null)
            {
                rowsAffected = taskUpdateEntity.RowCount.GetValueOrDefault();
                callId = taskUpdateEntity.CallId.GetValueOrDefault();
            }

            return rowsAffected;
        }
    }
}
