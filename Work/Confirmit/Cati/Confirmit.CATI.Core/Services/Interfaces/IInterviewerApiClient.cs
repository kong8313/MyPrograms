using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using ConfirmitDialerInterface;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface IInterviewerApiClient
    {
        void NotifyScheduling(int companyId);

        void NotifyConsoleTerminating(int companyId, int personId, long? monitoringSessionId);

        void NotifyUpdatingLiveMonitoringState(bool liveMonitoringStarted, int companyId, int personId);

        void NotifyOutcome(int companyId, int dialerId, string tenantId, long campaignId, int personId,
            string contactId, long callId, CallOutcome callOutcome, string dialerCallerId,  int ringTime, Dictionary<string, string> callOutcomeMetadata);

        void NotifyUpdatingAgentState(int companyId, int dialerId, string tenantId, long campaignId, int personId,
            AgentStateMsgs agentState);

        void NotifyScreenPop(int companyId, int dialerId, string customerId, long campaignId, int personId,
            string contactId, int callId, DialingMode callDialingMode);

        void NotifyCallDroppedByRespondent(int companyId, int dialerId, long campaignId, int personId, long callId);

        void NotifyUpdatingTransferState(int companyId, int dialerId, string transferId, ConsoleTransferState consoleTransferState);
        void NotifyTransferFinished(int companyId, int surveyId, int interviewId, string transferId);
        void NotifyAutomaticSurveyChanged(int companyId, int personId, int surveyId);
        void NotifyNewMessage(int companyId, IEnumerable<int> personIds, string message, string supervisorName);
        void NotifyIvrSubmit(int dialerId, string companyId, long campaignId, long agentId, KeyValuePair<string, string>[] variables);

        void NotifyCustomIvrInterviewEnd(int dialerId, int companyId, long campaignId, int agentId, int interviewId, long callId, CallOutcome callOutcome);
    }
}