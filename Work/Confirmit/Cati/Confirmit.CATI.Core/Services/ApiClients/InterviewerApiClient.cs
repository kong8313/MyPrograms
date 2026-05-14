using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.Configuration.Bootstrap;
using ConfirmitDialerInterface;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.Services.ApiClients
{
    public class InterviewerApiClient : ApiClientBase, IInterviewerApiClient
    {
        private const int MaxTryCnt = 3;

        private const string WriteScope = "catiinterviewer api.catiinterviewer.write";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICompanyInfo _companyInfo;
        private readonly IServiceDiscoveryClientProxy _serviceDiscoveryClientProxy;

        public InterviewerApiClient(
            IServiceDiscoveryClientProxy serviceDiscoveryClientProxy,
            IHttpClientFactory httpClientFactory,
            ICompanyInfo companyInfo,
            ITokenCacheService cacheService)
        {
            _serviceDiscoveryClientProxy = serviceDiscoveryClientProxy;
            _httpClientFactory = httpClientFactory;
            _companyInfo = companyInfo;
            _cacheService = cacheService;
        }

        public class NotifyParameters
        {
            public string Id { get; set; }
            public int CompanyId { get; set; }
        }

        public void NotifyScheduling(int companyId)
        {
            Notify("notifications/schedule", new NotifyParameters
            {
                CompanyId = companyId
            });
        }

        public class NotifyConsoleTerminatingParameters : NotifyParameters
        {
            public int PersonId { get; set; }
            public long? MonitoringSessionId { get; set; }
        }

        public void NotifyConsoleTerminating(int companyId, int personId, long? monitoringSessionId)
        {
            Notify("notifications/terminate", new NotifyConsoleTerminatingParameters
            {
                CompanyId = companyId,
                PersonId = personId,
                MonitoringSessionId = monitoringSessionId
            });
        }

        public class NotifyUpdatingLiveMonitoringStateParameters : NotifyParameters
        {
            public bool LiveMonitoringStarted { get; set; }
            public int PersonId { get; set; }
        }

        public void NotifyUpdatingLiveMonitoringState(bool liveMonitoringStarted, int companyId, int personId)
        {
            Notify("notifications/liveMonitoring", new NotifyUpdatingLiveMonitoringStateParameters
            {
                LiveMonitoringStarted = liveMonitoringStarted,
                CompanyId = companyId,
                PersonId = personId
            });
        }

        public class NotifyOutcomeParameters : NotifyParameters
        {
            public int DialerId { get; set; }
            public string TenantId { get; set; }
            public long CampaignId { get; set; }
            public int PersonId { get; set; }
            public string ContactId { get; set; }
            public long CallId { get; set; }
            public CallOutcome CallOutcome { get; set; }
            public string DialerCallerId { get; set; }
            public int RingTime { get; set; }
            public Dictionary<string, string> CallOutcomeMetadata { get; set; }
        }

        public void NotifyOutcome(int companyId, int dialerId, string tenantId, long campaignId, int personId,
            string contactId, long callId, CallOutcome callOutcome, string dialerCallerId,  int ringTime, Dictionary<string, string> callOutcomeMetadata)
        {
            Notify("notifications/dialerevents/calloutcome", new NotifyOutcomeParameters {
                CompanyId = companyId,
                DialerId = dialerId,
                TenantId = tenantId,
                CampaignId = campaignId,
                PersonId = personId,
                ContactId = contactId,
                CallId = callId,
                CallOutcome = callOutcome,
                DialerCallerId = dialerCallerId,
                RingTime = ringTime,
                CallOutcomeMetadata = callOutcomeMetadata
            });
        }

        public class NotifyUpdatingAgentStateParameters : NotifyParameters
        {
            public int DialerId { get; set; }
            public string TenantId { get; set; }
            public long CampaignId { get; set; }
            public int PersonId { get; set; }
            public AgentStateMsgs AgentState { get; set; }
        }

        public void NotifyUpdatingAgentState(int companyId, int dialerId, string tenantId, long campaignId, int personId,
            AgentStateMsgs agentState)
        {
            Notify("notifications/dialerevents/agentstate", new NotifyUpdatingAgentStateParameters
            {
                CompanyId = companyId,
                DialerId = dialerId,
                TenantId = tenantId,
                CampaignId = campaignId,
                PersonId = personId,
                AgentState = agentState
            });
        }

        public class NotifyScreenPopParameters : NotifyParameters
        {
            public int DialerId { get; set; }
            public string CustomerId { get; set; }
            public long CampaignId { get; set; }
            public int PersonId { get; set; }
            public string ContactId { get; set; }
            public long CallId { get; set; }
            public DialingMode CallDialingMode { get; set; }
        }

        public void NotifyScreenPop(int companyId, int dialerId, string customerId, long campaignId, int personId,
            string contactId, int callId, DialingMode callDialingMode)
        {
            Notify("notifications/dialerevents/screenpop", new NotifyScreenPopParameters
            {
                CompanyId = companyId,
                DialerId = dialerId,
                CustomerId = customerId,
                CampaignId = campaignId,
                PersonId = personId,
                ContactId = contactId,
                CallId = callId,
                CallDialingMode = callDialingMode
            });
        }

        public class NotifyCallDroppedByRespondentParameters : NotifyParameters
        {
            public int DialerId { get; set; }
            public long CampaignId { get; set; }
            public int PersonId { get; set; }
            public long CallId { get; set; }
        }

        public void NotifyCallDroppedByRespondent(int companyId, int dialerId, long campaignId, int personId, long callId)
        {
            Notify("notifications/dialerevents/calldroppedbyrespondent", new NotifyCallDroppedByRespondentParameters
            {
                CompanyId = companyId,
                DialerId = dialerId,
                CampaignId = campaignId,
                PersonId = personId,
                CallId = callId
            });
        }

        public class NotifyUpdatingTransferStateParameters : NotifyParameters
        {
            public int DialerId { get; set; }
            public string TransferId { get; set; }
            public ConsoleConnectionState ConnectionState { get; set; }
            public TransferParticipant Initiator { get; set; }
            public TransferParticipant Respondent { get; set; }
            public TransferParticipant Target { get; set; }
        }

        public void NotifyUpdatingTransferState(int companyId, int dialerId, string transferId, ConsoleTransferState consoleTransferState)
        {
            Notify("notifications/dialerevents/transferstate", new NotifyUpdatingTransferStateParameters
            {
                CompanyId = companyId,
                DialerId = dialerId,
                TransferId = transferId,
                ConnectionState = consoleTransferState.ConnectionState,
                Initiator = consoleTransferState.Initiator,
                Respondent = consoleTransferState.Respondent,
                Target = consoleTransferState.Target
            });
        }

        public class IvrSubmitNotificationData : NotifyParameters
        {
            public int DialerId { get; set; }
            public long CampaignId { get; set; }
            public int PersonId { get; set; }
            public KeyValuePair<string, string>[] Variables { get; set; }
        }

        public void NotifyIvrSubmit(int dialerId, string companyId, long campaignId, long agentId, KeyValuePair<string, string>[] variables)
        {
            _ = NotifyAsync("notifications/dialerevents/ivrsubmit", new IvrSubmitNotificationData
            {
                CompanyId = Int32.Parse(companyId),
                DialerId = dialerId,
                CampaignId = campaignId,
                PersonId = (int)agentId,
                Variables = variables
            });
        }

        public class TransferFinishNotificationData : NotifyParameters
        {
            public int SurveyId { get; set; }
            public int InterviewId { get; set; }
            public string TransferId { get; set; }
        }
        
        public void NotifyTransferFinished(int companyId, int surveyId, int interviewId, string transferId)
        {
            _ = NotifyAsync("notifications/transferFinish", new TransferFinishNotificationData
            {
                CompanyId = companyId,
                SurveyId = surveyId,
                InterviewId = interviewId,
                TransferId = transferId
            });
        }

        public class NotifyChangeAutomaticSurveyParameters : NotifyParameters
        {
            public int PersonId { get; set; }
            public int NewSurveyId { get; set; }
        }

        public void NotifyAutomaticSurveyChanged(int companyId, int personId, int newSurveyId)
        {
            Notify("notifications/automaticsurvey", new NotifyChangeAutomaticSurveyParameters
            {
                CompanyId = companyId,
                PersonId = personId,
                NewSurveyId = newSurveyId
            });
        }

        public class NotifyNewMessageParameters : NotifyParameters
        {
            public string Message { get; set; }
            public string SupervisorName { get; set; }
            public IEnumerable<int> Ids { get; set; }
        }

        public void NotifyNewMessage(int companyId, IEnumerable<int> personIds, string message, string supervisorName)
        {
            Notify("notifications/newmessage", new NotifyNewMessageParameters
            {
                CompanyId = companyId,
                Message = message,
                SupervisorName = supervisorName,
                Ids = personIds
            });
        }

        public class NotifyCustomIvrInterviewEndParameters : NotifyParameters
        {
            public int DialerId { get; set; }
            public long CampaignId { get; set; }
            public int PersonId { get; set; }
            public int InterviewId { get; set; }
            public long CallId { get; set; }
            public CallOutcome CallOutcome { get; set; }
        }
        
        public void NotifyCustomIvrInterviewEnd(int dialerId, int companyId, long campaignId, int agentId, int interviewId,
            long callId, CallOutcome callOutcome)
        {
            Notify("notifications/dialerevents/customivrinterviewend", new NotifyCustomIvrInterviewEndParameters
            {
                CompanyId = companyId,
                DialerId = dialerId,
                CampaignId = campaignId,
                PersonId = agentId,
                InterviewId = interviewId,
                CallId = callId,
                CallOutcome = callOutcome
            });
        }

        private void Notify<T>(string relativeUrl, T parameters) where T : NotifyParameters
        {
            var notifyCall = NotifyAsync(relativeUrl, parameters);
            if (HttpContext.Current != null)
            {
                notifyCall.Wait();
            }
        }

        private async Task NotifyAsync<T>(string relativeUrl, T parameters) where T : NotifyParameters
        {
            parameters.Id = Guid.NewGuid().ToString("N");
            HttpResponseMessage requestResult;
            int tryCnt = 0;

            do
            {
                if (tryCnt > 0)
                {
                    await Task.Delay(500 * tryCnt);
                }

                tryCnt++;

                try
                {
                    requestResult = await InvokeInternal(relativeUrl, parameters);

                    if (requestResult.StatusCode != HttpStatusCode.OK)
                    {
                        var message = $"{requestResult.RequestMessage.RequestUri} return unexpected {requestResult.StatusCode} code";
                        LogMessage(message, tryCnt);
                    }
                }
                catch (Exception ex)
                {
                    requestResult = null;
                    var message = $"Request to {relativeUrl} has failed with an error: {ex}";
                    LogMessage(message, tryCnt);
                }
            } while (requestResult?.StatusCode != HttpStatusCode.OK && tryCnt < MaxTryCnt);
        }

        private void LogMessage(string message, int tryCnt)
        {
            if (tryCnt < MaxTryCnt)
            {
                Trace.TraceWarning(message);
            }
            else
            {
                Trace.TraceError(message);
            }
        }

        private async Task<HttpResponseMessage> InvokeInternal<T>(string relativeUrl, T parameters) where T : NotifyParameters
        {
            return await MakeHttpRequestWithCachedToken(WriteScope, async (storedToken) => await MakePutRequest(relativeUrl, storedToken, parameters));
        }

        private async Task<HttpResponseMessage> MakePutRequest<T>(string relativeUrl, string accessToken, T parameters) where T : NotifyParameters
        {
            var baseAddress = GetBaseAddress();
            var requestUrl = CombineUrl(baseAddress, relativeUrl);

            var httpClient = _httpClientFactory.Get();

            HttpContent httpContent = new StringContent(
                JsonConvert.SerializeObject(parameters, new JsonSerializerSettings()),
                Encoding.UTF8);

            httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

            return await httpClient.PutAsync(requestUrl, accessToken, httpContent);
        }

        private Uri GetBaseAddress()
        {
            if (BootstrapConfig.IsContainerEnvironment && BackendInstance.IsInitialized)
            {
                // Special handling for tests running on k8s environment. Test companies include host name
                // of interviewer api being tested and we route requests to this api instead of release version  
                var match = Regex.Match(_companyInfo.CompanyName, @"TestCompany.*\[(?<host>r.*)]");

                if (match.Success)
                {
                    return new Uri($"http://{match.Groups["host"].Value}");
                }
            }

            return _serviceDiscoveryClientProxy.GetService(ConfirmitServiceNames.CatiInterviewerApiService);
        }
    }
}