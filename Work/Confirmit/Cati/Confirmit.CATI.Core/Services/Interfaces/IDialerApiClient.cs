using System.Collections.Generic;
using Confirmit.CATI.Core.Services.ApiClients.Models;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface IDialerApiClient
    {
        DialerErrorCode StartCampaign(int[] dialerIds, string surveyId, string surveyName, string surveyParametersXml, DialingMode dialingMode, bool recordWholeInterview);
        DialerErrorCode StopCampaign(string surveyId, int[] dialerIds, DialingMode dialingMode);
        DialerErrorCode KillCampaign(string surveyId, int[] dialerIds, DialingMode dialingMode);
        DialerErrorCode SetCampaignParameters(string surveyId, int[] dialerIds, string surveyParametersXml, DialingMode dialingMode, bool recordWholeInterview);
        DialerErrorCode Logout(int dialerId, string surveyId, int agentId, bool isPredictive);
        DialerErrorCode KillAgent(int dialerId, string surveyId, int agentId);
        DialerErrorCode SetGroups(int dialerId, string surveyId, int agentId, int[] agentGroups);
        DialerErrorCode CompleteCall(int dialerId, string surveyId, int agentId, int interviewId, long callId, bool makeAgentReady, string breakName, InterviewStatus interviewStatus);
        DialerErrorCode SendNumbers(string requestId, int dialerId, string surveyId, DialingMode campaignDialingMode, int callAgingTimeout, List<CallInfo> callList);
        DialerErrorCode FlushNumbers(int[] dialerIds, string surveyId, List<CallInfo> callList);
        DialerErrorCode StartRecording(int dialerId, string surveyId, int agentId, int interviewId, long callId, string label);
        DialerErrorCode StopRecording(int dialerId, string surveyId, int agentId, int interviewId, long callId, StopRecordingMode stopRecordingMode);
        DialerErrorCode CompletePreview(int dialerId, string surveyId, int agentId, int interviewId, long callId, string phoneNumber, bool isRecording);
        AudioRecordInfo[] GetAudioRecords(string surveyId, int interviewId, int dialerId);
        bool[] AreRecordsExists(string surveyId, int[] interviewIds, int dialerId);
        AudioFile GetAudioFile(int dialerId, string audioUrl);
        DialerErrorCode ConnectInboundCall(int dialerId, string surveyId, string inboundCallId, CallInfo callInfo, string[] surveyIdsToBorrowAgentsFrom, AudioMessageDescriptor audioMessageDescriptor);
        DialerErrorCode DropInboundCall(int dialerId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor);
        StartMonitorResponse StartMonitor(int dialerId, int agentId, string phoneNumber, string sessionId);
        DialerErrorCode StopMonitor(int dialerId, string sessionId);
        DialerErrorCode SetMonitorMode(int dialerId, string sessionId, MonitorMode monitorMode);
        DialerResponse Initialize(int dialerId);
        DialerErrorCode InitializeRecording(int dialerId);
        DialerErrorCode Release(int dialerId);
        GetFeaturesResponse GetFeatures(int dialerId);
        GetStateResponse GetState(int dialerId);
        GetLogFilesResponse GetLogFiles(int dialerId);
        GetLogFileBodyZippedResponse GetLogFileBodyZipped(int dialerId, string fileName);
        ConfigureInboundDdiNumbersResponse ConfigureInboundDdiNumbers(int dialerId, InboundDdiNumber[] inboundDdiNumbers);
        GetDialerInfoResponse GetDialerInfo(int dialerId);
        DialerErrorCode TransferCancel(int dialerId, string surveyId, string transferId);
    }
}