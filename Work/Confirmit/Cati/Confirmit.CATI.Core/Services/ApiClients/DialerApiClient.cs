using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services.ApiClients.Models;
using Confirmit.CATI.Core.Services.Interfaces;
using ConfirmitDialerInterface;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.Services.ApiClients
{
    public class DialerApiClient : ApiClientBase, IDialerApiClient
    {
        private const string WriteScope = "catidialer api.catidialer";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceDiscoveryClientProxy _serviceDiscoveryClientProxy;
        private readonly ICompanyInfo _companyInfo;

        public DialerApiClient(
            IServiceDiscoveryClientProxy serviceDiscoveryClientProxy,
            IHttpClientFactory httpClientFactory,
            ITokenCacheService cacheService, ICompanyInfo companyInfo)
        {
            _serviceDiscoveryClientProxy = serviceDiscoveryClientProxy;
            _httpClientFactory = httpClientFactory;
            _companyInfo = companyInfo;
            _cacheService = cacheService;
        }

        public DialerErrorCode StartCampaign(int[] dialerIds, string surveyId, string surveyName, string surveyParametersXml, DialingMode dialingMode, bool recordWholeInterview)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&surveyId={surveyId}&surveyName={HttpUtility.UrlEncode(surveyName)}&dialingMode={dialingMode}&recordWholeInterview={recordWholeInterview}";
            return PostAndGetDialerErrorCode($"survey/startcampaign?{queryParams}", new {
                dialerIds,
                surveyParametersXml
            });
        }

        public DialerErrorCode StopCampaign(string surveyId, int[] dialerIds, DialingMode dialingMode)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&surveyId={surveyId}&dialingMode={dialingMode}";
            return PostAndGetDialerErrorCode($"survey/stopcampaign?{queryParams}", dialerIds);
        }

        public DialerErrorCode KillCampaign(string surveyId, int[] dialerIds, DialingMode dialingMode)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&surveyId={surveyId}&dialingMode={dialingMode}";
            return PostAndGetDialerErrorCode($"survey/killcampaign?{queryParams}", dialerIds);
        }

        public DialerErrorCode SetCampaignParameters(string surveyId, int[] dialerIds, string surveyParametersXml, DialingMode dialingMode, bool recordWholeInterview)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&surveyId={surveyId}&dialingMode={dialingMode}&recordWholeInterview={recordWholeInterview}";
            return PostAndGetDialerErrorCode($"survey/setcampaignparameters?{queryParams}", new {
                dialerIds,
                surveyParametersXml
            });
        }

        public DialerErrorCode Logout(int dialerId, string surveyId, int agentId, bool isPredictive)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&dialerId={dialerId}&surveyId={surveyId}&agentId={agentId}&isPredictive={isPredictive}";
            return PostAndGetDialerErrorCode($"agent/logout?{queryParams}");
        }

        public DialerErrorCode KillAgent(int dialerId, string surveyId, int agentId)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&dialerId={dialerId}&agentId={agentId}&surveyId={surveyId}";
            return PostAndGetDialerErrorCode($"agent/killagent?{queryParams}");
        }

        public DialerErrorCode SetGroups(int dialerId, string surveyId, int agentId, int[] agentGroups)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&dialerId={dialerId}&agentId={agentId}&surveyId={surveyId}";
            return PostAndGetDialerErrorCode($"agent/setgroups?{queryParams}", agentGroups);
        }

        public DialerErrorCode CompleteCall(int dialerId, string surveyId, int agentId, int interviewId, long callId, bool makeAgentReady, string breakName, InterviewStatus interviewStatus)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&dialerId={dialerId}&agentId={agentId}&surveyId={surveyId}&interviewId={interviewId}&callId={callId}&makeAgentReady={makeAgentReady}&breakName={HttpUtility.UrlEncode(breakName)}";
            return PostAndGetDialerErrorCode($"call/completecall?{queryParams}", interviewStatus);
        }

        public DialerErrorCode SendNumbers(string requestId, int dialerId, string surveyId, DialingMode campaignDialingMode, int callAgingTimeout, List<CallInfo> callList)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&surveyId={surveyId}&dialerId={dialerId}&requestId={HttpUtility.UrlEncode(requestId)}&campaignDialingMode={campaignDialingMode}&callAgingTimeout={callAgingTimeout}";
            return PostAndGetDialerErrorCode($"callqueue/sendnumbers?{queryParams}", callList);
        }

        public DialerErrorCode FlushNumbers(int[] dialerIds, string surveyId, List<CallInfo> callList)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&surveyId={surveyId}";
            return PostAndGetDialerErrorCode($"callqueue/flushnumbers?{queryParams}", new {
                dialerIds,
                callList
            });
        }

        public DialerErrorCode StartRecording(int dialerId, string surveyId, int agentId, int interviewId, long callId, string label)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&surveyId={surveyId}&dialerId={dialerId}&agentId={agentId}&interviewId={interviewId}&callId={callId}&label={HttpUtility.UrlEncode(label)}";
            return PostAndGetDialerErrorCode($"call/startrecording?{queryParams}");
        }

        public DialerErrorCode StopRecording(int dialerId, string surveyId, int agentId, int interviewId, long callId, StopRecordingMode stopRecordingMode)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&surveyId={surveyId}&dialerId={dialerId}&agentId={agentId}&interviewId={interviewId}&callId={callId}&stopRecordingMode={stopRecordingMode}";
            return PostAndGetDialerErrorCode($"call/stoprecording?{queryParams}");
        }

        public DialerErrorCode CompletePreview(int dialerId, string surveyId, int agentId, int interviewId, long callId, string phoneNumber, bool isRecording)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&surveyId={surveyId}&dialerId={dialerId}&agentId={agentId}&interviewId={interviewId}&callId={callId}&phoneNumber={HttpUtility.UrlEncode(phoneNumber)}&isRecording={isRecording}";
            return PostAndGetDialerErrorCode($"call/completepreview?{queryParams}");
        }

        public AudioRecordInfo[] GetAudioRecords(string surveyId, int interviewId, int dialerId)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&surveyId={surveyId}&dialerId={dialerId}&interviewId={interviewId}";
            var dialerResponse = Get<GetAudioRecordsResponse>($"audiorecords/getaudiorecords?{queryParams}");

            if (dialerResponse.DialerErrorCode == DialerErrorCode.Success)
                return dialerResponse.AudioRecords;

            return Array.Empty<AudioRecordInfo>();
        }

        public bool[] AreRecordsExists(string surveyId, int[] interviewIds, int dialerId)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&surveyId={surveyId}&dialerId={dialerId}";
            var dialerResponse = Post<AreAudioRecordsExistsResponse>($"audiorecords/arerecordsexists?{queryParams}", interviewIds);

            if (dialerResponse.DialerErrorCode == DialerErrorCode.Success)
                return dialerResponse.AudioRecordExistenceFlags;

            return Enumerable.Repeat(false, interviewIds.Count()).ToArray();
        }

        public AudioFile GetAudioFile(int dialerId, string audioUrl)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&dialerId={dialerId}&audioUrl={HttpUtility.UrlEncode(audioUrl)}";
            var dialerResponse = Get<GetAudioFileResponse>($"audiorecords/getaudiofile?{queryParams}");

            if (dialerResponse.DialerErrorCode == DialerErrorCode.Success)
                return dialerResponse.AudioFile;

            return new AudioFile();
        }

        public DialerErrorCode ConnectInboundCall(int dialerId, string surveyId, string inboundCallId, CallInfo callInfo, string[] surveyIdsToBorrowAgentsFrom, AudioMessageDescriptor audioMessageDescriptor)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&surveyId={surveyId}&dialerId={dialerId}&inboundCallId={HttpUtility.UrlEncode(inboundCallId)}";
            return PostAndGetDialerErrorCode($"inboundcall/connectinboundcall?{queryParams}", new {
                CallInfo = callInfo,
                SurveyIdsToBorrowAgentsFrom = surveyIdsToBorrowAgentsFrom,
                AudioMessageDescriptor = audioMessageDescriptor
            });
        }

        public DialerErrorCode DropInboundCall(int dialerId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&dialerId={dialerId}&inboundCallId={HttpUtility.UrlEncode(inboundCallId)}";
            return PostAndGetDialerErrorCode($"inboundcall/dropinboundcall?{queryParams}", new {
                AudioMessageDescriptor = audioMessageDescriptor
            });
        }

        public StartMonitorResponse StartMonitor(int dialerId, int agentId, string phoneNumber, string sessionId)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&dialerId={dialerId}&agentId={agentId}&phoneNumber={HttpUtility.UrlEncode(phoneNumber)}&sessionId={HttpUtility.UrlEncode(sessionId)}";
            return Post<StartMonitorResponse>($"monitoring/startmonitor?{queryParams}");
        }

        public DialerErrorCode StopMonitor(int dialerId, string sessionId)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&dialerId={dialerId}&sessionId={HttpUtility.UrlEncode(sessionId)}";
            return PostAndGetDialerErrorCode($"monitoring/stopmonitor?{queryParams}");
        }

        public DialerErrorCode SetMonitorMode(int dialerId, string sessionId, MonitorMode monitorMode)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&dialerId={dialerId}&sessionId={HttpUtility.UrlEncode(sessionId)}&monitorMode={monitorMode}";
            return PostAndGetDialerErrorCode($"monitoring/setmonitormode?{queryParams}");
        }

        public DialerResponse Initialize(int dialerId)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&dialerId={dialerId}";
            return Post<DialerResponse>($"initialize?{queryParams}");
        }

        public DialerErrorCode InitializeRecording(int dialerId)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&dialerId={dialerId}";
            return PostAndGetDialerErrorCode($"initializerecording?{queryParams}");
        }

        public DialerErrorCode Release(int dialerId)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&dialerId={dialerId}";
            return PostAndGetDialerErrorCode($"release?{queryParams}");
        }

        public GetFeaturesResponse GetFeatures(int dialerId)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&dialerId={dialerId}";
            return Get<GetFeaturesResponse>($"getfeatures?{queryParams}");
        }

        public GetStateResponse GetState(int dialerId)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&dialerId={dialerId}";
            return Get<GetStateResponse>($"getstate?{queryParams}");
        }

        public GetLogFilesResponse GetLogFiles(int dialerId)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&dialerId={dialerId}";
            return Get<GetLogFilesResponse>($"getlogfiles?{queryParams}");
        }

        public GetLogFileBodyZippedResponse GetLogFileBodyZipped(int dialerId, string fileName)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&dialerId={dialerId}&fileName={HttpUtility.UrlEncode(fileName)}";
            return Get<GetLogFileBodyZippedResponse>($"getlogfilebodyzipped?{queryParams}");
        }

        public ConfigureInboundDdiNumbersResponse ConfigureInboundDdiNumbers(int dialerId, InboundDdiNumber[] inboundDdiNumbers)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&dialerId={dialerId}";
            return Post<ConfigureInboundDdiNumbersResponse>($"configureinboundddinumbers?{queryParams}", inboundDdiNumbers);
        }

        public GetDialerInfoResponse GetDialerInfo(int dialerId)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&dialerId={dialerId}";
            return Get<GetDialerInfoResponse>($"getdialerinfo?{queryParams}");
        }

        public DialerErrorCode TransferCancel(int dialerId, string surveyId, string transferId)
        {
            var queryParams = $@"companyId={_companyInfo.CompanyId}&dialerId={dialerId}&surveyId={surveyId}&transferId={HttpUtility.UrlEncode(transferId)}";
            return PostAndGetDialerErrorCode($"transfer/transfercancel?{queryParams}");
        }

        private DialerErrorCode PostAndGetDialerErrorCode(string url)
        {
            return Post<DialerResponse>(url).DialerErrorCode;
        }

        private DialerErrorCode PostAndGetDialerErrorCode(string url, object body)
        {
            return Post<DialerResponse>(url, body).DialerErrorCode;
        }

        private T Post<T>(string url) where T : DialerResponse, new()
        {
            return AsyncTaskRunner.RunSync(() => PostAsync<T>(url, new { }));
        }

        private T Post<T>(string url, object body) where T : DialerResponse, new()
        {
            return AsyncTaskRunner.RunSync(() => PostAsync<T>(url, body));
        }

        private async Task<T> PostAsync<T>(string url, object body) where T : DialerResponse, new()
        {
            var response = await MakeHttpRequestWithCachedToken(WriteScope,
                async storedToken => await PostAsync(url, body, storedToken));

            return await HandleDialerResponse<T>(response);
        }

        private async Task<HttpResponseMessage> PostAsync(string url, object body, string accessToken)
        {
            var baseAddress = _serviceDiscoveryClientProxy.GetService(ConfirmitServiceNames.DialerApiService);
            var route = CombineUrl(baseAddress, url);

            var json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpClient = _httpClientFactory.Get();
            return await httpClient.PostAsync(route, accessToken, content);
        }

        private T Get<T>(string url) where T : DialerResponse, new()
        {
            return AsyncTaskRunner.RunSync(() => GetAsync<T>(url));
        }

        private async Task<T> GetAsync<T>(string url) where T : DialerResponse, new()
        {
            var response = await MakeHttpRequestWithCachedToken(WriteScope,
                async storedToken => await GetAsync(url, storedToken));

            return await HandleDialerResponse<T>(response);
        }

        private static async Task<T> HandleDialerResponse<T>(HttpResponseMessage response) where T : DialerResponse, new()
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                if (response.StatusCode == HttpStatusCode.InternalServerError && response.Content != null)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(errorBody))
                    {
                        var error = JsonConvert.DeserializeObject<DialerApiErrorResponse>(errorBody);
                        return new T { DialerErrorCode = DialerErrorCode.UnknownError, ErrorMessage = error.ErrorMessage };
                    }
                }

                return new T { DialerErrorCode = DialerErrorCode.UnknownError };
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        private async Task<HttpResponseMessage> GetAsync(string url, string accessToken)
        {
            var baseAddress = _serviceDiscoveryClientProxy.GetService(ConfirmitServiceNames.DialerApiService);
            var route = CombineUrl(baseAddress, url);

            var httpClient = _httpClientFactory.Get();
            return await httpClient.GetResponseAsync(route, accessToken);
        }
    }
}