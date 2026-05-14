using System;
using ConfirmitDialerInterface;
using Confirmit.CATI.Core.Services.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.Core.Services.ApiClients.Models;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIDialerApiClient : IDialerApiClient 
    {
        private IDialerApiClient _inner;

        public StubIDialerApiClient()
        {
            _inner = null;
        }

        public IDialerApiClient Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate DialerErrorCode StartCampaignArrayOfInt32StringStringStringDialingModeBooleanDelegate(int[] dialerIds, string surveyId, string surveyName, string surveyParametersXml, DialingMode dialingMode, bool recordWholeInterview);
        public StartCampaignArrayOfInt32StringStringStringDialingModeBooleanDelegate StartCampaignArrayOfInt32StringStringStringDialingModeBoolean;

        DialerErrorCode IDialerApiClient.StartCampaign(int[] dialerIds, string surveyId, string surveyName, string surveyParametersXml, DialingMode dialingMode, bool recordWholeInterview)
        {


            if (StartCampaignArrayOfInt32StringStringStringDialingModeBoolean != null)
            {
                return StartCampaignArrayOfInt32StringStringStringDialingModeBoolean(dialerIds, surveyId, surveyName, surveyParametersXml, dialingMode, recordWholeInterview);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).StartCampaign(dialerIds, surveyId, surveyName, surveyParametersXml, dialingMode, recordWholeInterview);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode StopCampaignStringArrayOfInt32DialingModeDelegate(string surveyId, int[] dialerIds, DialingMode dialingMode);
        public StopCampaignStringArrayOfInt32DialingModeDelegate StopCampaignStringArrayOfInt32DialingMode;

        DialerErrorCode IDialerApiClient.StopCampaign(string surveyId, int[] dialerIds, DialingMode dialingMode)
        {


            if (StopCampaignStringArrayOfInt32DialingMode != null)
            {
                return StopCampaignStringArrayOfInt32DialingMode(surveyId, dialerIds, dialingMode);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).StopCampaign(surveyId, dialerIds, dialingMode);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode KillCampaignStringArrayOfInt32DialingModeDelegate(string surveyId, int[] dialerIds, DialingMode dialingMode);
        public KillCampaignStringArrayOfInt32DialingModeDelegate KillCampaignStringArrayOfInt32DialingMode;

        DialerErrorCode IDialerApiClient.KillCampaign(string surveyId, int[] dialerIds, DialingMode dialingMode)
        {


            if (KillCampaignStringArrayOfInt32DialingMode != null)
            {
                return KillCampaignStringArrayOfInt32DialingMode(surveyId, dialerIds, dialingMode);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).KillCampaign(surveyId, dialerIds, dialingMode);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SetCampaignParametersStringArrayOfInt32StringDialingModeBooleanDelegate(string surveyId, int[] dialerIds, string surveyParametersXml, DialingMode dialingMode, bool recordWholeInterview);
        public SetCampaignParametersStringArrayOfInt32StringDialingModeBooleanDelegate SetCampaignParametersStringArrayOfInt32StringDialingModeBoolean;

        DialerErrorCode IDialerApiClient.SetCampaignParameters(string surveyId, int[] dialerIds, string surveyParametersXml, DialingMode dialingMode, bool recordWholeInterview)
        {


            if (SetCampaignParametersStringArrayOfInt32StringDialingModeBoolean != null)
            {
                return SetCampaignParametersStringArrayOfInt32StringDialingModeBoolean(surveyId, dialerIds, surveyParametersXml, dialingMode, recordWholeInterview);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).SetCampaignParameters(surveyId, dialerIds, surveyParametersXml, dialingMode, recordWholeInterview);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode LogoutInt32StringInt32BooleanDelegate(int dialerId, string surveyId, int agentId, bool isPredictive);
        public LogoutInt32StringInt32BooleanDelegate LogoutInt32StringInt32Boolean;

        DialerErrorCode IDialerApiClient.Logout(int dialerId, string surveyId, int agentId, bool isPredictive)
        {


            if (LogoutInt32StringInt32Boolean != null)
            {
                return LogoutInt32StringInt32Boolean(dialerId, surveyId, agentId, isPredictive);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).Logout(dialerId, surveyId, agentId, isPredictive);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode KillAgentInt32StringInt32Delegate(int dialerId, string surveyId, int agentId);
        public KillAgentInt32StringInt32Delegate KillAgentInt32StringInt32;

        DialerErrorCode IDialerApiClient.KillAgent(int dialerId, string surveyId, int agentId)
        {


            if (KillAgentInt32StringInt32 != null)
            {
                return KillAgentInt32StringInt32(dialerId, surveyId, agentId);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).KillAgent(dialerId, surveyId, agentId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SetGroupsInt32StringInt32ArrayOfInt32Delegate(int dialerId, string surveyId, int agentId, int[] agentGroups);
        public SetGroupsInt32StringInt32ArrayOfInt32Delegate SetGroupsInt32StringInt32ArrayOfInt32;

        DialerErrorCode IDialerApiClient.SetGroups(int dialerId, string surveyId, int agentId, int[] agentGroups)
        {


            if (SetGroupsInt32StringInt32ArrayOfInt32 != null)
            {
                return SetGroupsInt32StringInt32ArrayOfInt32(dialerId, surveyId, agentId, agentGroups);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).SetGroups(dialerId, surveyId, agentId, agentGroups);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode CompleteCallInt32StringInt32Int32Int64BooleanStringInterviewStatusDelegate(int dialerId, string surveyId, int agentId, int interviewId, long callId, bool makeAgentReady, string breakName, InterviewStatus interviewStatus);
        public CompleteCallInt32StringInt32Int32Int64BooleanStringInterviewStatusDelegate CompleteCallInt32StringInt32Int32Int64BooleanStringInterviewStatus;

        DialerErrorCode IDialerApiClient.CompleteCall(int dialerId, string surveyId, int agentId, int interviewId, long callId, bool makeAgentReady, string breakName, InterviewStatus interviewStatus)
        {


            if (CompleteCallInt32StringInt32Int32Int64BooleanStringInterviewStatus != null)
            {
                return CompleteCallInt32StringInt32Int32Int64BooleanStringInterviewStatus(dialerId, surveyId, agentId, interviewId, callId, makeAgentReady, breakName, interviewStatus);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).CompleteCall(dialerId, surveyId, agentId, interviewId, callId, makeAgentReady, breakName, interviewStatus);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SendNumbersStringInt32StringDialingModeInt32ListOfCallInfoDelegate(string requestId, int dialerId, string surveyId, DialingMode campaignDialingMode, int callAgingTimeout, List<CallInfo> callList);
        public SendNumbersStringInt32StringDialingModeInt32ListOfCallInfoDelegate SendNumbersStringInt32StringDialingModeInt32ListOfCallInfo;

        DialerErrorCode IDialerApiClient.SendNumbers(string requestId, int dialerId, string surveyId, DialingMode campaignDialingMode, int callAgingTimeout, List<CallInfo> callList)
        {


            if (SendNumbersStringInt32StringDialingModeInt32ListOfCallInfo != null)
            {
                return SendNumbersStringInt32StringDialingModeInt32ListOfCallInfo(requestId, dialerId, surveyId, campaignDialingMode, callAgingTimeout, callList);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).SendNumbers(requestId, dialerId, surveyId, campaignDialingMode, callAgingTimeout, callList);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode FlushNumbersArrayOfInt32StringListOfCallInfoDelegate(int[] dialerIds, string surveyId, List<CallInfo> callList);
        public FlushNumbersArrayOfInt32StringListOfCallInfoDelegate FlushNumbersArrayOfInt32StringListOfCallInfo;

        DialerErrorCode IDialerApiClient.FlushNumbers(int[] dialerIds, string surveyId, List<CallInfo> callList)
        {


            if (FlushNumbersArrayOfInt32StringListOfCallInfo != null)
            {
                return FlushNumbersArrayOfInt32StringListOfCallInfo(dialerIds, surveyId, callList);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).FlushNumbers(dialerIds, surveyId, callList);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode StartRecordingInt32StringInt32Int32Int64StringDelegate(int dialerId, string surveyId, int agentId, int interviewId, long callId, string label);
        public StartRecordingInt32StringInt32Int32Int64StringDelegate StartRecordingInt32StringInt32Int32Int64String;

        DialerErrorCode IDialerApiClient.StartRecording(int dialerId, string surveyId, int agentId, int interviewId, long callId, string label)
        {


            if (StartRecordingInt32StringInt32Int32Int64String != null)
            {
                return StartRecordingInt32StringInt32Int32Int64String(dialerId, surveyId, agentId, interviewId, callId, label);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).StartRecording(dialerId, surveyId, agentId, interviewId, callId, label);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode StopRecordingInt32StringInt32Int32Int64StopRecordingModeDelegate(int dialerId, string surveyId, int agentId, int interviewId, long callId, StopRecordingMode stopRecordingMode);
        public StopRecordingInt32StringInt32Int32Int64StopRecordingModeDelegate StopRecordingInt32StringInt32Int32Int64StopRecordingMode;

        DialerErrorCode IDialerApiClient.StopRecording(int dialerId, string surveyId, int agentId, int interviewId, long callId, StopRecordingMode stopRecordingMode)
        {


            if (StopRecordingInt32StringInt32Int32Int64StopRecordingMode != null)
            {
                return StopRecordingInt32StringInt32Int32Int64StopRecordingMode(dialerId, surveyId, agentId, interviewId, callId, stopRecordingMode);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).StopRecording(dialerId, surveyId, agentId, interviewId, callId, stopRecordingMode);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode CompletePreviewInt32StringInt32Int32Int64StringBooleanDelegate(int dialerId, string surveyId, int agentId, int interviewId, long callId, string phoneNumber, bool isRecording);
        public CompletePreviewInt32StringInt32Int32Int64StringBooleanDelegate CompletePreviewInt32StringInt32Int32Int64StringBoolean;

        DialerErrorCode IDialerApiClient.CompletePreview(int dialerId, string surveyId, int agentId, int interviewId, long callId, string phoneNumber, bool isRecording)
        {


            if (CompletePreviewInt32StringInt32Int32Int64StringBoolean != null)
            {
                return CompletePreviewInt32StringInt32Int32Int64StringBoolean(dialerId, surveyId, agentId, interviewId, callId, phoneNumber, isRecording);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).CompletePreview(dialerId, surveyId, agentId, interviewId, callId, phoneNumber, isRecording);
            }

            return default(DialerErrorCode);
        }

        public delegate AudioRecordInfo[] GetAudioRecordsStringInt32Int32Delegate(string surveyId, int interviewId, int dialerId);
        public GetAudioRecordsStringInt32Int32Delegate GetAudioRecordsStringInt32Int32;

        AudioRecordInfo[] IDialerApiClient.GetAudioRecords(string surveyId, int interviewId, int dialerId)
        {


            if (GetAudioRecordsStringInt32Int32 != null)
            {
                return GetAudioRecordsStringInt32Int32(surveyId, interviewId, dialerId);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).GetAudioRecords(surveyId, interviewId, dialerId);
            }

            return default(AudioRecordInfo[]);
        }

        public delegate bool[] AreRecordsExistsStringArrayOfInt32Int32Delegate(string surveyId, int[] interviewIds, int dialerId);
        public AreRecordsExistsStringArrayOfInt32Int32Delegate AreRecordsExistsStringArrayOfInt32Int32;

        bool[] IDialerApiClient.AreRecordsExists(string surveyId, int[] interviewIds, int dialerId)
        {


            if (AreRecordsExistsStringArrayOfInt32Int32 != null)
            {
                return AreRecordsExistsStringArrayOfInt32Int32(surveyId, interviewIds, dialerId);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).AreRecordsExists(surveyId, interviewIds, dialerId);
            }

            return default(bool[]);
        }

        public delegate AudioFile GetAudioFileInt32StringDelegate(int dialerId, string audioUrl);
        public GetAudioFileInt32StringDelegate GetAudioFileInt32String;

        AudioFile IDialerApiClient.GetAudioFile(int dialerId, string audioUrl)
        {


            if (GetAudioFileInt32String != null)
            {
                return GetAudioFileInt32String(dialerId, audioUrl);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).GetAudioFile(dialerId, audioUrl);
            }

            return default(AudioFile);
        }

        public delegate DialerErrorCode ConnectInboundCallInt32StringStringCallInfoArrayOfStringAudioMessageDescriptorDelegate(int dialerId, string surveyId, string inboundCallId, CallInfo callInfo, string[] surveyIdsToBorrowAgentsFrom, AudioMessageDescriptor audioMessageDescriptor);
        public ConnectInboundCallInt32StringStringCallInfoArrayOfStringAudioMessageDescriptorDelegate ConnectInboundCallInt32StringStringCallInfoArrayOfStringAudioMessageDescriptor;

        DialerErrorCode IDialerApiClient.ConnectInboundCall(int dialerId, string surveyId, string inboundCallId, CallInfo callInfo, string[] surveyIdsToBorrowAgentsFrom, AudioMessageDescriptor audioMessageDescriptor)
        {


            if (ConnectInboundCallInt32StringStringCallInfoArrayOfStringAudioMessageDescriptor != null)
            {
                return ConnectInboundCallInt32StringStringCallInfoArrayOfStringAudioMessageDescriptor(dialerId, surveyId, inboundCallId, callInfo, surveyIdsToBorrowAgentsFrom, audioMessageDescriptor);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).ConnectInboundCall(dialerId, surveyId, inboundCallId, callInfo, surveyIdsToBorrowAgentsFrom, audioMessageDescriptor);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode DropInboundCallInt32StringAudioMessageDescriptorDelegate(int dialerId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor);
        public DropInboundCallInt32StringAudioMessageDescriptorDelegate DropInboundCallInt32StringAudioMessageDescriptor;

        DialerErrorCode IDialerApiClient.DropInboundCall(int dialerId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor)
        {


            if (DropInboundCallInt32StringAudioMessageDescriptor != null)
            {
                return DropInboundCallInt32StringAudioMessageDescriptor(dialerId, inboundCallId, audioMessageDescriptor);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).DropInboundCall(dialerId, inboundCallId, audioMessageDescriptor);
            }

            return default(DialerErrorCode);
        }

        public delegate StartMonitorResponse StartMonitorInt32Int32StringStringDelegate(int dialerId, int agentId, string phoneNumber, string sessionId);
        public StartMonitorInt32Int32StringStringDelegate StartMonitorInt32Int32StringString;

        StartMonitorResponse IDialerApiClient.StartMonitor(int dialerId, int agentId, string phoneNumber, string sessionId)
        {


            if (StartMonitorInt32Int32StringString != null)
            {
                return StartMonitorInt32Int32StringString(dialerId, agentId, phoneNumber, sessionId);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).StartMonitor(dialerId, agentId, phoneNumber, sessionId);
            }

            return default(StartMonitorResponse);
        }

        public delegate DialerErrorCode StopMonitorInt32StringDelegate(int dialerId, string sessionId);
        public StopMonitorInt32StringDelegate StopMonitorInt32String;

        DialerErrorCode IDialerApiClient.StopMonitor(int dialerId, string sessionId)
        {


            if (StopMonitorInt32String != null)
            {
                return StopMonitorInt32String(dialerId, sessionId);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).StopMonitor(dialerId, sessionId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SetMonitorModeInt32StringMonitorModeDelegate(int dialerId, string sessionId, MonitorMode monitorMode);
        public SetMonitorModeInt32StringMonitorModeDelegate SetMonitorModeInt32StringMonitorMode;

        DialerErrorCode IDialerApiClient.SetMonitorMode(int dialerId, string sessionId, MonitorMode monitorMode)
        {


            if (SetMonitorModeInt32StringMonitorMode != null)
            {
                return SetMonitorModeInt32StringMonitorMode(dialerId, sessionId, monitorMode);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).SetMonitorMode(dialerId, sessionId, monitorMode);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerResponse InitializeInt32Delegate(int dialerId);
        public InitializeInt32Delegate InitializeInt32;

        DialerResponse IDialerApiClient.Initialize(int dialerId)
        {


            if (InitializeInt32 != null)
            {
                return InitializeInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).Initialize(dialerId);
            }

            return default(DialerResponse);
        }

        public delegate DialerErrorCode InitializeRecordingInt32Delegate(int dialerId);
        public InitializeRecordingInt32Delegate InitializeRecordingInt32;

        DialerErrorCode IDialerApiClient.InitializeRecording(int dialerId)
        {


            if (InitializeRecordingInt32 != null)
            {
                return InitializeRecordingInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).InitializeRecording(dialerId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode ReleaseInt32Delegate(int dialerId);
        public ReleaseInt32Delegate ReleaseInt32;

        DialerErrorCode IDialerApiClient.Release(int dialerId)
        {


            if (ReleaseInt32 != null)
            {
                return ReleaseInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).Release(dialerId);
            }

            return default(DialerErrorCode);
        }

        public delegate GetFeaturesResponse GetFeaturesInt32Delegate(int dialerId);
        public GetFeaturesInt32Delegate GetFeaturesInt32;

        GetFeaturesResponse IDialerApiClient.GetFeatures(int dialerId)
        {


            if (GetFeaturesInt32 != null)
            {
                return GetFeaturesInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).GetFeatures(dialerId);
            }

            return default(GetFeaturesResponse);
        }

        public delegate GetStateResponse GetStateInt32Delegate(int dialerId);
        public GetStateInt32Delegate GetStateInt32;

        GetStateResponse IDialerApiClient.GetState(int dialerId)
        {


            if (GetStateInt32 != null)
            {
                return GetStateInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).GetState(dialerId);
            }

            return default(GetStateResponse);
        }

        public delegate GetLogFilesResponse GetLogFilesInt32Delegate(int dialerId);
        public GetLogFilesInt32Delegate GetLogFilesInt32;

        GetLogFilesResponse IDialerApiClient.GetLogFiles(int dialerId)
        {


            if (GetLogFilesInt32 != null)
            {
                return GetLogFilesInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).GetLogFiles(dialerId);
            }

            return default(GetLogFilesResponse);
        }

        public delegate GetLogFileBodyZippedResponse GetLogFileBodyZippedInt32StringDelegate(int dialerId, string fileName);
        public GetLogFileBodyZippedInt32StringDelegate GetLogFileBodyZippedInt32String;

        GetLogFileBodyZippedResponse IDialerApiClient.GetLogFileBodyZipped(int dialerId, string fileName)
        {


            if (GetLogFileBodyZippedInt32String != null)
            {
                return GetLogFileBodyZippedInt32String(dialerId, fileName);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).GetLogFileBodyZipped(dialerId, fileName);
            }

            return default(GetLogFileBodyZippedResponse);
        }

        public delegate ConfigureInboundDdiNumbersResponse ConfigureInboundDdiNumbersInt32ArrayOfInboundDdiNumberDelegate(int dialerId, InboundDdiNumber[] inboundDdiNumbers);
        public ConfigureInboundDdiNumbersInt32ArrayOfInboundDdiNumberDelegate ConfigureInboundDdiNumbersInt32ArrayOfInboundDdiNumber;

        ConfigureInboundDdiNumbersResponse IDialerApiClient.ConfigureInboundDdiNumbers(int dialerId, InboundDdiNumber[] inboundDdiNumbers)
        {


            if (ConfigureInboundDdiNumbersInt32ArrayOfInboundDdiNumber != null)
            {
                return ConfigureInboundDdiNumbersInt32ArrayOfInboundDdiNumber(dialerId, inboundDdiNumbers);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).ConfigureInboundDdiNumbers(dialerId, inboundDdiNumbers);
            }

            return default(ConfigureInboundDdiNumbersResponse);
        }

        public delegate GetDialerInfoResponse GetDialerInfoInt32Delegate(int dialerId);
        public GetDialerInfoInt32Delegate GetDialerInfoInt32;

        GetDialerInfoResponse IDialerApiClient.GetDialerInfo(int dialerId)
        {


            if (GetDialerInfoInt32 != null)
            {
                return GetDialerInfoInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).GetDialerInfo(dialerId);
            }

            return default(GetDialerInfoResponse);
        }

        public delegate DialerErrorCode TransferCancelInt32StringStringDelegate(int dialerId, string surveyId, string transferId);
        public TransferCancelInt32StringStringDelegate TransferCancelInt32StringString;

        DialerErrorCode IDialerApiClient.TransferCancel(int dialerId, string surveyId, string transferId)
        {


            if (TransferCancelInt32StringString != null)
            {
                return TransferCancelInt32StringString(dialerId, surveyId, transferId);
            } else if (_inner != null)
            {
                return ((IDialerApiClient)_inner).TransferCancel(dialerId, surveyId, transferId);
            }

            return default(DialerErrorCode);
        }

    }
}