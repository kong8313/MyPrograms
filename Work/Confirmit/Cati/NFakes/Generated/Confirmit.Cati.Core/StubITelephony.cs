using System;
using Confirmit.CATI.Core.Telephony;
using ConfirmitDialerInterface;
using System.Collections.Generic;
using Confirmit.CATI.Common.Logging;

namespace Confirmit.CATI.Core.Telephony.Fakes
{
    public class StubITelephony : ITelephony 
    {
        private ITelephony _inner;

        public StubITelephony()
        {
            _inner = null;
        }

        public ITelephony Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void InitializeDialersDelegate();
        public InitializeDialersDelegate InitializeDialers;

        void ITelephonyCore.InitializeDialers()
        {

            if (InitializeDialers != null)
            {
                InitializeDialers();
            } else if (_inner != null)
            {
                ((ITelephonyCore)_inner).InitializeDialers();
            }
        }

        public delegate void UninitializeDialersBooleanDelegate(bool releaseDialerWs);
        public UninitializeDialersBooleanDelegate UninitializeDialersBoolean;

        void ITelephonyCore.UninitializeDialers(bool releaseDialerWs)
        {

            if (UninitializeDialersBoolean != null)
            {
                UninitializeDialersBoolean(releaseDialerWs);
            } else if (_inner != null)
            {
                ((ITelephonyCore)_inner).UninitializeDialers(releaseDialerWs);
            }
        }

        public delegate ICollection<DialerStartCampaignResult> StartCampaignInt64StringDialingModeStringStringDelegate(long campaignId, string campaignName, DialingMode dialingMode, string campaignType, string surveyParametersXml);
        public StartCampaignInt64StringDialingModeStringStringDelegate StartCampaignInt64StringDialingModeStringString;

        ICollection<DialerStartCampaignResult> ITelephonyCore.StartCampaign(long campaignId, string campaignName, DialingMode dialingMode, string campaignType, string surveyParametersXml)
        {


            if (StartCampaignInt64StringDialingModeStringString != null)
            {
                return StartCampaignInt64StringDialingModeStringString(campaignId, campaignName, dialingMode, campaignType, surveyParametersXml);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).StartCampaign(campaignId, campaignName, dialingMode, campaignType, surveyParametersXml);
            }

            return default(ICollection<DialerStartCampaignResult>);
        }

        public delegate void StopCampaignInt64DialingModeDelegate(long campaignId, DialingMode dialingMode);
        public StopCampaignInt64DialingModeDelegate StopCampaignInt64DialingMode;

        void ITelephonyCore.StopCampaign(long campaignId, DialingMode dialingMode)
        {

            if (StopCampaignInt64DialingMode != null)
            {
                StopCampaignInt64DialingMode(campaignId, dialingMode);
            } else if (_inner != null)
            {
                ((ITelephonyCore)_inner).StopCampaign(campaignId, dialingMode);
            }
        }

        public delegate void KillCampaignInt64DialingModeDelegate(long campaignId, DialingMode dialingMode);
        public KillCampaignInt64DialingModeDelegate KillCampaignInt64DialingMode;

        void ITelephonyCore.KillCampaign(long campaignId, DialingMode dialingMode)
        {

            if (KillCampaignInt64DialingMode != null)
            {
                KillCampaignInt64DialingMode(campaignId, dialingMode);
            } else if (_inner != null)
            {
                ((ITelephonyCore)_inner).KillCampaign(campaignId, dialingMode);
            }
        }

        public delegate DialerErrorCode LoginInt32Int64StringStringAgentTypeStringStringBooleanBooleanIEnumerableOfKeyValuePairOfStringStringDelegate(int dialerId, long campaignId, string agentId, string agentName, AgentType agentType, string agentExtension, string userId, bool isPredictive, bool isLocal, IEnumerable<KeyValuePair<string, string>> agentAttributes);
        public LoginInt32Int64StringStringAgentTypeStringStringBooleanBooleanIEnumerableOfKeyValuePairOfStringStringDelegate LoginInt32Int64StringStringAgentTypeStringStringBooleanBooleanIEnumerableOfKeyValuePairOfStringString;

        DialerErrorCode ITelephonyCore.Login(int dialerId, long campaignId, string agentId, string agentName, AgentType agentType, string agentExtension, string userId, bool isPredictive, bool isLocal, IEnumerable<KeyValuePair<string, string>> agentAttributes)
        {


            if (LoginInt32Int64StringStringAgentTypeStringStringBooleanBooleanIEnumerableOfKeyValuePairOfStringString != null)
            {
                return LoginInt32Int64StringStringAgentTypeStringStringBooleanBooleanIEnumerableOfKeyValuePairOfStringString(dialerId, campaignId, agentId, agentName, agentType, agentExtension, userId, isPredictive, isLocal, agentAttributes);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).Login(dialerId, campaignId, agentId, agentName, agentType, agentExtension, userId, isPredictive, isLocal, agentAttributes);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SetCampaignInt32Int64Int32Delegate(int dialerId, long campaignId, int agentId);
        public SetCampaignInt32Int64Int32Delegate SetCampaignInt32Int64Int32;

        DialerErrorCode ITelephonyCore.SetCampaign(int dialerId, long campaignId, int agentId)
        {


            if (SetCampaignInt32Int64Int32 != null)
            {
                return SetCampaignInt32Int64Int32(dialerId, campaignId, agentId);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).SetCampaign(dialerId, campaignId, agentId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode LogoutInt32Int64BooleanStringDelegate(int dialerId, long campaignId, bool isPredictive, string agentId);
        public LogoutInt32Int64BooleanStringDelegate LogoutInt32Int64BooleanString;

        DialerErrorCode ITelephonyCore.Logout(int dialerId, long campaignId, bool isPredictive, string agentId)
        {


            if (LogoutInt32Int64BooleanString != null)
            {
                return LogoutInt32Int64BooleanString(dialerId, campaignId, isPredictive, agentId);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).Logout(dialerId, campaignId, isPredictive, agentId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode KillAgentInt32Int64StringDelegate(int dialerId, long campaignId, string agentId);
        public KillAgentInt32Int64StringDelegate KillAgentInt32Int64String;

        DialerErrorCode ITelephonyCore.KillAgent(int dialerId, long campaignId, string agentId)
        {


            if (KillAgentInt32Int64String != null)
            {
                return KillAgentInt32Int64String(dialerId, campaignId, agentId);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).KillAgent(dialerId, campaignId, agentId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode GoReadyInt32Int64StringDelegate(int dialerId, long campaignId, string agentId);
        public GoReadyInt32Int64StringDelegate GoReadyInt32Int64String;

        DialerErrorCode ITelephonyCore.GoReady(int dialerId, long campaignId, string agentId)
        {


            if (GoReadyInt32Int64String != null)
            {
                return GoReadyInt32Int64String(dialerId, campaignId, agentId);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).GoReady(dialerId, campaignId, agentId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode GoNotReadyInt32Int64StringStringDelegate(int dialerId, long campaignId, string agentId, string breakName);
        public GoNotReadyInt32Int64StringStringDelegate GoNotReadyInt32Int64StringString;

        DialerErrorCode ITelephonyCore.GoNotReady(int dialerId, long campaignId, string agentId, string breakName)
        {


            if (GoNotReadyInt32Int64StringString != null)
            {
                return GoNotReadyInt32Int64StringString(dialerId, campaignId, agentId, breakName);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).GoNotReady(dialerId, campaignId, agentId, breakName);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SendNumberInt32Int64StringDialingModeInt32Int32Int32StringInt32BooleanDelegate(int dialerId, long campaignId, string agentId, DialingMode diallingMode, int groupId, int contactId, int callId, string phoneNumber, int callAgingTimeout, bool isRecording);
        public SendNumberInt32Int64StringDialingModeInt32Int32Int32StringInt32BooleanDelegate SendNumberInt32Int64StringDialingModeInt32Int32Int32StringInt32Boolean;

        DialerErrorCode ITelephonyCore.SendNumber(int dialerId, long campaignId, string agentId, DialingMode diallingMode, int groupId, int contactId, int callId, string phoneNumber, int callAgingTimeout, bool isRecording)
        {


            if (SendNumberInt32Int64StringDialingModeInt32Int32Int32StringInt32Boolean != null)
            {
                return SendNumberInt32Int64StringDialingModeInt32Int32Int32StringInt32Boolean(dialerId, campaignId, agentId, diallingMode, groupId, contactId, callId, phoneNumber, callAgingTimeout, isRecording);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).SendNumber(dialerId, campaignId, agentId, diallingMode, groupId, contactId, callId, phoneNumber, callAgingTimeout, isRecording);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SendNumbersInt32StringInt64DialingModeListOfCallInfoInt32BooleanDelegate(int dialerId, string requestId, long campaignId, DialingMode campaignDiallingMode, List<CallInfo> callList, int callAgingTimeout, bool isRecording);
        public SendNumbersInt32StringInt64DialingModeListOfCallInfoInt32BooleanDelegate SendNumbersInt32StringInt64DialingModeListOfCallInfoInt32Boolean;

        DialerErrorCode ITelephonyCore.SendNumbers(int dialerId, string requestId, long campaignId, DialingMode campaignDiallingMode, List<CallInfo> callList, int callAgingTimeout, bool isRecording)
        {


            if (SendNumbersInt32StringInt64DialingModeListOfCallInfoInt32Boolean != null)
            {
                return SendNumbersInt32StringInt64DialingModeListOfCallInfoInt32Boolean(dialerId, requestId, campaignId, campaignDiallingMode, callList, callAgingTimeout, isRecording);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).SendNumbers(dialerId, requestId, campaignId, campaignDiallingMode, callList, callAgingTimeout, isRecording);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SendNumberToAgentInt32Int64StringDialingModeInt32Int32StringBooleanStringDictionaryOfStringObjectDelegate(int dialerId, long campaignId, string agentId, DialingMode diallingMode, int contactId, int callId, string phoneNumber, bool isRecording, string callerId, Dictionary<string, Object> respondentVariables);
        public SendNumberToAgentInt32Int64StringDialingModeInt32Int32StringBooleanStringDictionaryOfStringObjectDelegate SendNumberToAgentInt32Int64StringDialingModeInt32Int32StringBooleanStringDictionaryOfStringObject;

        DialerErrorCode ITelephonyCore.SendNumberToAgent(int dialerId, long campaignId, string agentId, DialingMode diallingMode, int contactId, int callId, string phoneNumber, bool isRecording, string callerId, Dictionary<string, Object> respondentVariables)
        {


            if (SendNumberToAgentInt32Int64StringDialingModeInt32Int32StringBooleanStringDictionaryOfStringObject != null)
            {
                return SendNumberToAgentInt32Int64StringDialingModeInt32Int32StringBooleanStringDictionaryOfStringObject(dialerId, campaignId, agentId, diallingMode, contactId, callId, phoneNumber, isRecording, callerId, respondentVariables);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).SendNumberToAgent(dialerId, campaignId, agentId, diallingMode, contactId, callId, phoneNumber, isRecording, callerId, respondentVariables);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SendNumberToAgentExInt32Int64StringDialingModeInt32Int32StringInt32BooleanDelegate(int dialerId, long campaignId, string agentId, DialingMode dialingMode, int contactId, int callId, string phoneNumber, int callAgingTimeout, bool isRecording);
        public SendNumberToAgentExInt32Int64StringDialingModeInt32Int32StringInt32BooleanDelegate SendNumberToAgentExInt32Int64StringDialingModeInt32Int32StringInt32Boolean;

        DialerErrorCode ITelephonyCore.SendNumberToAgentEx(int dialerId, long campaignId, string agentId, DialingMode dialingMode, int contactId, int callId, string phoneNumber, int callAgingTimeout, bool isRecording)
        {


            if (SendNumberToAgentExInt32Int64StringDialingModeInt32Int32StringInt32Boolean != null)
            {
                return SendNumberToAgentExInt32Int64StringDialingModeInt32Int32StringInt32Boolean(dialerId, campaignId, agentId, dialingMode, contactId, callId, phoneNumber, callAgingTimeout, isRecording);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).SendNumberToAgentEx(dialerId, campaignId, agentId, dialingMode, contactId, callId, phoneNumber, callAgingTimeout, isRecording);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode RedialInt32Int64StringInt32Int32StringBooleanStringDelegate(int dialerId, long campaignId, string agentId, int contactId, int callId, string phoneNumber, bool isRecording, string callerid);
        public RedialInt32Int64StringInt32Int32StringBooleanStringDelegate RedialInt32Int64StringInt32Int32StringBooleanString;

        DialerErrorCode ITelephonyCore.Redial(int dialerId, long campaignId, string agentId, int contactId, int callId, string phoneNumber, bool isRecording, string callerid)
        {


            if (RedialInt32Int64StringInt32Int32StringBooleanString != null)
            {
                return RedialInt32Int64StringInt32Int32StringBooleanString(dialerId, campaignId, agentId, contactId, callId, phoneNumber, isRecording, callerid);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).Redial(dialerId, campaignId, agentId, contactId, callId, phoneNumber, isRecording, callerid);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode HangupInt32Int64StringInt32Int64Delegate(int dialerId, long campaignId, string agentId, int contactId, long callId);
        public HangupInt32Int64StringInt32Int64Delegate HangupInt32Int64StringInt32Int64;

        DialerErrorCode ITelephonyCore.Hangup(int dialerId, long campaignId, string agentId, int contactId, long callId)
        {


            if (HangupInt32Int64StringInt32Int64 != null)
            {
                return HangupInt32Int64StringInt32Int64(dialerId, campaignId, agentId, contactId, callId);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).Hangup(dialerId, campaignId, agentId, contactId, callId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode CompleteCallInt32Int64StringInt32BooleanStringInterviewStatusInt64Delegate(int dialerId, long campaignId, string agentId, int contactId, bool makeAgentReady, string breakName, InterviewStatus status, long callId);
        public CompleteCallInt32Int64StringInt32BooleanStringInterviewStatusInt64Delegate CompleteCallInt32Int64StringInt32BooleanStringInterviewStatusInt64;

        DialerErrorCode ITelephonyCore.CompleteCall(int dialerId, long campaignId, string agentId, int contactId, bool makeAgentReady, string breakName, InterviewStatus status, long callId)
        {


            if (CompleteCallInt32Int64StringInt32BooleanStringInterviewStatusInt64 != null)
            {
                return CompleteCallInt32Int64StringInt32BooleanStringInterviewStatusInt64(dialerId, campaignId, agentId, contactId, makeAgentReady, breakName, status, callId);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).CompleteCall(dialerId, campaignId, agentId, contactId, makeAgentReady, breakName, status, callId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SetNextInterviewInt32Int64StringInterviewStatusInt64Int32Int64Delegate(int dialerId, long currentCampaignId, string agentId, InterviewStatus currentInterviewStatus, long nextCampaignId, int nextInterviewId, long nextCallId);
        public SetNextInterviewInt32Int64StringInterviewStatusInt64Int32Int64Delegate SetNextInterviewInt32Int64StringInterviewStatusInt64Int32Int64;

        DialerErrorCode ITelephonyCore.SetNextInterview(int dialerId, long currentCampaignId, string agentId, InterviewStatus currentInterviewStatus, long nextCampaignId, int nextInterviewId, long nextCallId)
        {


            if (SetNextInterviewInt32Int64StringInterviewStatusInt64Int32Int64 != null)
            {
                return SetNextInterviewInt32Int64StringInterviewStatusInt64Int32Int64(dialerId, currentCampaignId, agentId, currentInterviewStatus, nextCampaignId, nextInterviewId, nextCallId);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).SetNextInterview(dialerId, currentCampaignId, agentId, currentInterviewStatus, nextCampaignId, nextInterviewId, nextCallId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode UpdateInterviewStatusInt32Int64StringInt32Int32InterviewStatusDelegate(int dialerId, long campaignId, string agentId, int interviewId, int callId, InterviewStatus interviewStatus);
        public UpdateInterviewStatusInt32Int64StringInt32Int32InterviewStatusDelegate UpdateInterviewStatusInt32Int64StringInt32Int32InterviewStatus;

        DialerErrorCode ITelephonyCore.UpdateInterviewStatus(int dialerId, long campaignId, string agentId, int interviewId, int callId, InterviewStatus interviewStatus)
        {


            if (UpdateInterviewStatusInt32Int64StringInt32Int32InterviewStatus != null)
            {
                return UpdateInterviewStatusInt32Int64StringInt32Int32InterviewStatus(dialerId, campaignId, agentId, interviewId, callId, interviewStatus);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).UpdateInterviewStatus(dialerId, campaignId, agentId, interviewId, callId, interviewStatus);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SetGroupsInt32Int64StringArrayOfInt32Delegate(int dialerId, long campaignId, string agentId, int[] agentGroups);
        public SetGroupsInt32Int64StringArrayOfInt32Delegate SetGroupsInt32Int64StringArrayOfInt32;

        DialerErrorCode ITelephonyCore.SetGroups(int dialerId, long campaignId, string agentId, int[] agentGroups)
        {


            if (SetGroupsInt32Int64StringArrayOfInt32 != null)
            {
                return SetGroupsInt32Int64StringArrayOfInt32(dialerId, campaignId, agentId, agentGroups);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).SetGroups(dialerId, campaignId, agentId, agentGroups);
            }

            return default(DialerErrorCode);
        }

        public delegate void FlushNumbersInt64ListOfCallInfoDelegate(long campaignId, List<CallInfo> callsList);
        public FlushNumbersInt64ListOfCallInfoDelegate FlushNumbersInt64ListOfCallInfo;

        void ITelephonyCore.FlushNumbers(long campaignId, List<CallInfo> callsList)
        {

            if (FlushNumbersInt64ListOfCallInfo != null)
            {
                FlushNumbersInt64ListOfCallInfo(campaignId, callsList);
            } else if (_inner != null)
            {
                ((ITelephonyCore)_inner).FlushNumbers(campaignId, callsList);
            }
        }

        public delegate DialerErrorCode StartRecordingInt32Int64StringInt32Int32StringDelegate(int dialerId, long campaignId, string agentId, int contactId, int callId, string label);
        public StartRecordingInt32Int64StringInt32Int32StringDelegate StartRecordingInt32Int64StringInt32Int32String;

        DialerErrorCode ITelephonyCore.StartRecording(int dialerId, long campaignId, string agentId, int contactId, int callId, string label)
        {


            if (StartRecordingInt32Int64StringInt32Int32String != null)
            {
                return StartRecordingInt32Int64StringInt32Int32String(dialerId, campaignId, agentId, contactId, callId, label);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).StartRecording(dialerId, campaignId, agentId, contactId, callId, label);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode StopRecordingInt32Int64StringInt32Int32StopRecordingModeDelegate(int dialerId, long campaignId, string agentId, int contactId, int callId, StopRecordingMode stopRecordingMode);
        public StopRecordingInt32Int64StringInt32Int32StopRecordingModeDelegate StopRecordingInt32Int64StringInt32Int32StopRecordingMode;

        DialerErrorCode ITelephonyCore.StopRecording(int dialerId, long campaignId, string agentId, int contactId, int callId, StopRecordingMode stopRecordingMode)
        {


            if (StopRecordingInt32Int64StringInt32Int32StopRecordingMode != null)
            {
                return StopRecordingInt32Int64StringInt32Int32StopRecordingMode(dialerId, campaignId, agentId, contactId, callId, stopRecordingMode);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).StopRecording(dialerId, campaignId, agentId, contactId, callId, stopRecordingMode);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode StartMonitorInt32StringStringStringRefDelegate(int dialerId, string agentId, string phoneNumber, ref string sessionId);
        public StartMonitorInt32StringStringStringRefDelegate StartMonitorInt32StringStringStringRef;

        DialerErrorCode ITelephonyCore.StartMonitor(int dialerId, string agentId, string phoneNumber, ref string sessionId)
        {


            if (StartMonitorInt32StringStringStringRef != null)
            {
                return StartMonitorInt32StringStringStringRef(dialerId, agentId, phoneNumber, ref sessionId);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).StartMonitor(dialerId, agentId, phoneNumber, ref sessionId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode StopMonitorInt32StringInt32StringDelegate(int dialerId, string agentId, int contactId, string sessionId);
        public StopMonitorInt32StringInt32StringDelegate StopMonitorInt32StringInt32String;

        DialerErrorCode ITelephonyCore.StopMonitor(int dialerId, string agentId, int contactId, string sessionId)
        {


            if (StopMonitorInt32StringInt32String != null)
            {
                return StopMonitorInt32StringInt32String(dialerId, agentId, contactId, sessionId);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).StopMonitor(dialerId, agentId, contactId, sessionId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SetMonitorModeInt32StringStringMonitorModeDelegate(int dialerId, string agentId, string sessionId, MonitorMode monitorMode);
        public SetMonitorModeInt32StringStringMonitorModeDelegate SetMonitorModeInt32StringStringMonitorMode;

        DialerErrorCode ITelephonyCore.SetMonitorMode(int dialerId, string agentId, string sessionId, MonitorMode monitorMode)
        {


            if (SetMonitorModeInt32StringStringMonitorMode != null)
            {
                return SetMonitorModeInt32StringStringMonitorMode(dialerId, agentId, sessionId, monitorMode);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).SetMonitorMode(dialerId, agentId, sessionId, monitorMode);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode CompletePreviewInt32Int64StringInt32Int32StringBooleanDelegate(int dialerId, long campaignId, string agentId, int contactId, int callId, string phoneNumber, bool isRecording);
        public CompletePreviewInt32Int64StringInt32Int32StringBooleanDelegate CompletePreviewInt32Int64StringInt32Int32StringBoolean;

        DialerErrorCode ITelephonyCore.CompletePreview(int dialerId, long campaignId, string agentId, int contactId, int callId, string phoneNumber, bool isRecording)
        {


            if (CompletePreviewInt32Int64StringInt32Int32StringBoolean != null)
            {
                return CompletePreviewInt32Int64StringInt32Int32StringBoolean(dialerId, campaignId, agentId, contactId, callId, phoneNumber, isRecording);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).CompletePreview(dialerId, campaignId, agentId, contactId, callId, phoneNumber, isRecording);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode TransferToIvrInt32Int64StringInt32Int32StringIEnumerableOfKeyValuePairOfStringStringDelegate(int dialerId, long campaignId, string agentId, int interviewId, int callId, string endpoint, IEnumerable<KeyValuePair<string, string>> attributes);
        public TransferToIvrInt32Int64StringInt32Int32StringIEnumerableOfKeyValuePairOfStringStringDelegate TransferToIvrInt32Int64StringInt32Int32StringIEnumerableOfKeyValuePairOfStringString;

        DialerErrorCode ITelephonyCore.TransferToIvr(int dialerId, long campaignId, string agentId, int interviewId, int callId, string endpoint, IEnumerable<KeyValuePair<string, string>> attributes)
        {


            if (TransferToIvrInt32Int64StringInt32Int32StringIEnumerableOfKeyValuePairOfStringString != null)
            {
                return TransferToIvrInt32Int64StringInt32Int32StringIEnumerableOfKeyValuePairOfStringString(dialerId, campaignId, agentId, interviewId, callId, endpoint, attributes);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).TransferToIvr(dialerId, campaignId, agentId, interviewId, callId, endpoint, attributes);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode IvrRenderVoiceXmlInt32Int32Int64Int32Int32StringDelegate(int dialerId, int companyId, long campaignId, int agentId, int contactId, string voiceXml);
        public IvrRenderVoiceXmlInt32Int32Int64Int32Int32StringDelegate IvrRenderVoiceXmlInt32Int32Int64Int32Int32String;

        DialerErrorCode ITelephonyCore.IvrRenderVoiceXml(int dialerId, int companyId, long campaignId, int agentId, int contactId, string voiceXml)
        {


            if (IvrRenderVoiceXmlInt32Int32Int64Int32Int32String != null)
            {
                return IvrRenderVoiceXmlInt32Int32Int64Int32Int32String(dialerId, companyId, campaignId, agentId, contactId, voiceXml);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).IvrRenderVoiceXml(dialerId, companyId, campaignId, agentId, contactId, voiceXml);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode[] ConfigureInboundDdiNumbersInt32ArrayOfInboundDdiNumberDelegate(int dialerId, InboundDdiNumber[] inboundDdiNumbers);
        public ConfigureInboundDdiNumbersInt32ArrayOfInboundDdiNumberDelegate ConfigureInboundDdiNumbersInt32ArrayOfInboundDdiNumber;

        DialerErrorCode[] ITelephonyCore.ConfigureInboundDdiNumbers(int dialerId, InboundDdiNumber[] inboundDdiNumbers)
        {


            if (ConfigureInboundDdiNumbersInt32ArrayOfInboundDdiNumber != null)
            {
                return ConfigureInboundDdiNumbersInt32ArrayOfInboundDdiNumber(dialerId, inboundDdiNumbers);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).ConfigureInboundDdiNumbers(dialerId, inboundDdiNumbers);
            }

            return default(DialerErrorCode[]);
        }

        public delegate DialerErrorCode DropInboundCallInt32StringAudioMessageDescriptorDelegate(int dialerId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor);
        public DropInboundCallInt32StringAudioMessageDescriptorDelegate DropInboundCallInt32StringAudioMessageDescriptor;

        DialerErrorCode ITelephonyCore.DropInboundCall(int dialerId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor)
        {


            if (DropInboundCallInt32StringAudioMessageDescriptor != null)
            {
                return DropInboundCallInt32StringAudioMessageDescriptor(dialerId, inboundCallId, audioMessageDescriptor);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).DropInboundCall(dialerId, inboundCallId, audioMessageDescriptor);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode ConnectInboundCallInt32Int64Int32Int32StringCallInfoArrayOfInt64AudioMessageDescriptorDelegate(int dialerId, long campaignId, int agentId, int contactId, string inboundCallId, CallInfo callInfo, long[] campaignIdsToBorrowAgentsFrom, AudioMessageDescriptor audioMessageDescriptor);
        public ConnectInboundCallInt32Int64Int32Int32StringCallInfoArrayOfInt64AudioMessageDescriptorDelegate ConnectInboundCallInt32Int64Int32Int32StringCallInfoArrayOfInt64AudioMessageDescriptor;

        DialerErrorCode ITelephonyCore.ConnectInboundCall(int dialerId, long campaignId, int agentId, int contactId, string inboundCallId, CallInfo callInfo, long[] campaignIdsToBorrowAgentsFrom, AudioMessageDescriptor audioMessageDescriptor)
        {


            if (ConnectInboundCallInt32Int64Int32Int32StringCallInfoArrayOfInt64AudioMessageDescriptor != null)
            {
                return ConnectInboundCallInt32Int64Int32Int32StringCallInfoArrayOfInt64AudioMessageDescriptor(dialerId, campaignId, agentId, contactId, inboundCallId, callInfo, campaignIdsToBorrowAgentsFrom, audioMessageDescriptor);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).ConnectInboundCall(dialerId, campaignId, agentId, contactId, inboundCallId, callInfo, campaignIdsToBorrowAgentsFrom, audioMessageDescriptor);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode ConnectInboundCallToAgentInt32Int64Int32Int32StringCallInfoAudioMessageDescriptorDelegate(int dialerId, long campaignId, int agentId, int contactId, string inboundCallId, CallInfo callInfo, AudioMessageDescriptor audioMessageDescriptor);
        public ConnectInboundCallToAgentInt32Int64Int32Int32StringCallInfoAudioMessageDescriptorDelegate ConnectInboundCallToAgentInt32Int64Int32Int32StringCallInfoAudioMessageDescriptor;

        DialerErrorCode ITelephonyCore.ConnectInboundCallToAgent(int dialerId, long campaignId, int agentId, int contactId, string inboundCallId, CallInfo callInfo, AudioMessageDescriptor audioMessageDescriptor)
        {


            if (ConnectInboundCallToAgentInt32Int64Int32Int32StringCallInfoAudioMessageDescriptor != null)
            {
                return ConnectInboundCallToAgentInt32Int64Int32Int32StringCallInfoAudioMessageDescriptor(dialerId, campaignId, agentId, contactId, inboundCallId, callInfo, audioMessageDescriptor);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).ConnectInboundCallToAgent(dialerId, campaignId, agentId, contactId, inboundCallId, callInfo, audioMessageDescriptor);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode TransferStartInt32Int64StringInt32Int32TransferTypeDelegate(int dialerId, long campaignId, string transferId, int agentId, int contactId, TransferType transferType);
        public TransferStartInt32Int64StringInt32Int32TransferTypeDelegate TransferStartInt32Int64StringInt32Int32TransferType;

        DialerErrorCode ITelephonyCore.TransferStart(int dialerId, long campaignId, string transferId, int agentId, int contactId, TransferType transferType)
        {


            if (TransferStartInt32Int64StringInt32Int32TransferType != null)
            {
                return TransferStartInt32Int64StringInt32Int32TransferType(dialerId, campaignId, transferId, agentId, contactId, transferType);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).TransferStart(dialerId, campaignId, transferId, agentId, contactId, transferType);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode TransferSetTargetInt32Int64StringInt32Int32TargetTypeStringBooleanDelegate(int dialerId, long campaignId, string transferId, int agentId, int contactId, TargetType targetType, string targetResource, bool borrowAgentsFromAllCampaigns);
        public TransferSetTargetInt32Int64StringInt32Int32TargetTypeStringBooleanDelegate TransferSetTargetInt32Int64StringInt32Int32TargetTypeStringBoolean;

        DialerErrorCode ITelephonyCore.TransferSetTarget(int dialerId, long campaignId, string transferId, int agentId, int contactId, TargetType targetType, string targetResource, bool borrowAgentsFromAllCampaigns)
        {


            if (TransferSetTargetInt32Int64StringInt32Int32TargetTypeStringBoolean != null)
            {
                return TransferSetTargetInt32Int64StringInt32Int32TargetTypeStringBoolean(dialerId, campaignId, transferId, agentId, contactId, targetType, targetResource, borrowAgentsFromAllCampaigns);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).TransferSetTarget(dialerId, campaignId, transferId, agentId, contactId, targetType, targetResource, borrowAgentsFromAllCampaigns);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode TransferSetConnectionStateInt32Int64StringInt32Int32ConnectionStateDelegate(int dialerId, long campaignId, string transferId, int agentId, int contactId, ConnectionState state);
        public TransferSetConnectionStateInt32Int64StringInt32Int32ConnectionStateDelegate TransferSetConnectionStateInt32Int64StringInt32Int32ConnectionState;

        DialerErrorCode ITelephonyCore.TransferSetConnectionState(int dialerId, long campaignId, string transferId, int agentId, int contactId, ConnectionState state)
        {


            if (TransferSetConnectionStateInt32Int64StringInt32Int32ConnectionState != null)
            {
                return TransferSetConnectionStateInt32Int64StringInt32Int32ConnectionState(dialerId, campaignId, transferId, agentId, contactId, state);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).TransferSetConnectionState(dialerId, campaignId, transferId, agentId, contactId, state);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode TransferCompleteInt32Int64StringInt32Int32Delegate(int dialerId, long campaignId, string transferId, int agentId, int contactId);
        public TransferCompleteInt32Int64StringInt32Int32Delegate TransferCompleteInt32Int64StringInt32Int32;

        DialerErrorCode ITelephonyCore.TransferComplete(int dialerId, long campaignId, string transferId, int agentId, int contactId)
        {


            if (TransferCompleteInt32Int64StringInt32Int32 != null)
            {
                return TransferCompleteInt32Int64StringInt32Int32(dialerId, campaignId, transferId, agentId, contactId);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).TransferComplete(dialerId, campaignId, transferId, agentId, contactId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode TransferCancelInt32Int64StringInt32Int32Delegate(int dialerId, long campaignId, string transferId, int agentId, int contactId);
        public TransferCancelInt32Int64StringInt32Int32Delegate TransferCancelInt32Int64StringInt32Int32;

        DialerErrorCode ITelephonyCore.TransferCancel(int dialerId, long campaignId, string transferId, int agentId, int contactId)
        {


            if (TransferCancelInt32Int64StringInt32Int32 != null)
            {
                return TransferCancelInt32Int64StringInt32Int32(dialerId, campaignId, transferId, agentId, contactId);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).TransferCancel(dialerId, campaignId, transferId, agentId, contactId);
            }

            return default(DialerErrorCode);
        }

        public delegate bool IsPersonModeSupportedAgentTaskChoiceModeNullableOfInt32Delegate(AgentTaskChoiceMode mode, int? dialerId);
        public IsPersonModeSupportedAgentTaskChoiceModeNullableOfInt32Delegate IsPersonModeSupportedAgentTaskChoiceModeNullableOfInt32;

        bool ITelephonyCore.IsPersonModeSupported(AgentTaskChoiceMode mode, int? dialerId)
        {


            if (IsPersonModeSupportedAgentTaskChoiceModeNullableOfInt32 != null)
            {
                return IsPersonModeSupportedAgentTaskChoiceModeNullableOfInt32(mode, dialerId);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).IsPersonModeSupported(mode, dialerId);
            }

            return default(bool);
        }

        public delegate bool IsReloginNeededOnSurveyChangeNullableOfInt32Delegate(int? dialerId);
        public IsReloginNeededOnSurveyChangeNullableOfInt32Delegate IsReloginNeededOnSurveyChangeNullableOfInt32;

        bool ITelephonyCore.IsReloginNeededOnSurveyChange(int? dialerId)
        {


            if (IsReloginNeededOnSurveyChangeNullableOfInt32 != null)
            {
                return IsReloginNeededOnSurveyChangeNullableOfInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).IsReloginNeededOnSurveyChange(dialerId);
            }

            return default(bool);
        }

        public delegate CallOutcome TranslateOutcomeInt64Delegate(long outcome);
        public TranslateOutcomeInt64Delegate TranslateOutcomeInt64;

        CallOutcome ITelephonyCore.TranslateOutcome(long outcome)
        {


            if (TranslateOutcomeInt64 != null)
            {
                return TranslateOutcomeInt64(outcome);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).TranslateOutcome(outcome);
            }

            return default(CallOutcome);
        }

        public delegate bool IsHangUpSupportedDelegate();
        public IsHangUpSupportedDelegate IsHangUpSupported;

        bool ITelephonyCore.IsHangUpSupported()
        {


            if (IsHangUpSupported != null)
            {
                return IsHangUpSupported();
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).IsHangUpSupported();
            }

            return default(bool);
        }

        public delegate bool IsDynamicExtensionNumberAllowedBooleanNullableOfInt32Delegate(bool isAgentLocal, int? dialerId);
        public IsDynamicExtensionNumberAllowedBooleanNullableOfInt32Delegate IsDynamicExtensionNumberAllowedBooleanNullableOfInt32;

        bool ITelephonyCore.IsDynamicExtensionNumberAllowed(bool isAgentLocal, int? dialerId)
        {


            if (IsDynamicExtensionNumberAllowedBooleanNullableOfInt32 != null)
            {
                return IsDynamicExtensionNumberAllowedBooleanNullableOfInt32(isAgentLocal, dialerId);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).IsDynamicExtensionNumberAllowed(isAgentLocal, dialerId);
            }

            return default(bool);
        }

        public delegate DialerErrorCode SetConfigurationParametersInt32StringDelegate(int dialerId, string configurationParametersXml);
        public SetConfigurationParametersInt32StringDelegate SetConfigurationParametersInt32String;

        DialerErrorCode ITelephonyCore.SetConfigurationParameters(int dialerId, string configurationParametersXml)
        {


            if (SetConfigurationParametersInt32String != null)
            {
                return SetConfigurationParametersInt32String(dialerId, configurationParametersXml);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).SetConfigurationParameters(dialerId, configurationParametersXml);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode ValidateCampaignParametersStringDelegate(string surveyParametersXml);
        public ValidateCampaignParametersStringDelegate ValidateCampaignParametersString;

        DialerErrorCode ITelephonyCore.ValidateCampaignParameters(string surveyParametersXml)
        {


            if (ValidateCampaignParametersString != null)
            {
                return ValidateCampaignParametersString(surveyParametersXml);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).ValidateCampaignParameters(surveyParametersXml);
            }

            return default(DialerErrorCode);
        }

        public delegate void SetCampaignParametersInt64DialingModeStringDelegate(long campaignId, DialingMode dialingMode, string surveyParametersXml);
        public SetCampaignParametersInt64DialingModeStringDelegate SetCampaignParametersInt64DialingModeString;

        void ITelephonyCore.SetCampaignParameters(long campaignId, DialingMode dialingMode, string surveyParametersXml)
        {

            if (SetCampaignParametersInt64DialingModeString != null)
            {
                SetCampaignParametersInt64DialingModeString(campaignId, dialingMode, surveyParametersXml);
            } else if (_inner != null)
            {
                ((ITelephonyCore)_inner).SetCampaignParameters(campaignId, dialingMode, surveyParametersXml);
            }
        }

        public delegate DialerErrorCode RegisterAgentSoftphoneInt32Int32Int32StringStringOutStringOutStringOutStringOutStringOutDelegate(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl);
        public RegisterAgentSoftphoneInt32Int32Int32StringStringOutStringOutStringOutStringOutStringOutDelegate RegisterAgentSoftphoneInt32Int32Int32StringStringOutStringOutStringOutStringOutStringOut;

        DialerErrorCode ITelephonyCore.RegisterAgentSoftphone(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl)
        {
            login = default(string);
            password = default(string);
            host = default(string);
            extension = default(string);
            frontendUrl = default(string);


            if (RegisterAgentSoftphoneInt32Int32Int32StringStringOutStringOutStringOutStringOutStringOut != null)
            {
                return RegisterAgentSoftphoneInt32Int32Int32StringStringOutStringOutStringOutStringOutStringOut(companyId, dialerId, agentId, agentName, out login, out password, out host, out extension, out frontendUrl);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).RegisterAgentSoftphone(companyId, dialerId, agentId, agentName, out login, out password, out host, out extension, out frontendUrl);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode StartCustomIvrInterviewInt32Int64StringInt32Int64StringDelegate(int dialerId, long campaignId, string agentId, int interviewId, long callId, string respondentSurveyLink);
        public StartCustomIvrInterviewInt32Int64StringInt32Int64StringDelegate StartCustomIvrInterviewInt32Int64StringInt32Int64String;

        DialerErrorCode ITelephonyCore.StartCustomIvrInterview(int dialerId, long campaignId, string agentId, int interviewId, long callId, string respondentSurveyLink)
        {


            if (StartCustomIvrInterviewInt32Int64StringInt32Int64String != null)
            {
                return StartCustomIvrInterviewInt32Int64StringInt32Int64String(dialerId, campaignId, agentId, interviewId, callId, respondentSurveyLink);
            } else if (_inner != null)
            {
                return ((ITelephonyCore)_inner).StartCustomIvrInterview(dialerId, campaignId, agentId, interviewId, callId, respondentSurveyLink);
            }

            return default(DialerErrorCode);
        }

        public delegate void InitializeRecordingDelegate();
        public InitializeRecordingDelegate InitializeRecording;

        void ITelephonyRecording.InitializeRecording()
        {

            if (InitializeRecording != null)
            {
                InitializeRecording();
            } else if (_inner != null)
            {
                ((ITelephonyRecording)_inner).InitializeRecording();
            }
        }

        public delegate IEnumerable<AudioRecordInfo> GetAudioRecordsInt32Int32Delegate(int surveyId, int interviewId);
        public GetAudioRecordsInt32Int32Delegate GetAudioRecordsInt32Int32;

        IEnumerable<AudioRecordInfo> ITelephonyRecording.GetAudioRecords(int surveyId, int interviewId)
        {


            if (GetAudioRecordsInt32Int32 != null)
            {
                return GetAudioRecordsInt32Int32(surveyId, interviewId);
            } else if (_inner != null)
            {
                return ((ITelephonyRecording)_inner).GetAudioRecords(surveyId, interviewId);
            }

            return default(IEnumerable<AudioRecordInfo>);
        }

        public delegate bool[] AreRecordsExistsInt32ArrayOfInt32Delegate(int surveyId, int[] interviewIds);
        public AreRecordsExistsInt32ArrayOfInt32Delegate AreRecordsExistsInt32ArrayOfInt32;

        bool[] ITelephonyRecording.AreRecordsExists(int surveyId, int[] interviewIds)
        {


            if (AreRecordsExistsInt32ArrayOfInt32 != null)
            {
                return AreRecordsExistsInt32ArrayOfInt32(surveyId, interviewIds);
            } else if (_inner != null)
            {
                return ((ITelephonyRecording)_inner).AreRecordsExists(surveyId, interviewIds);
            }

            return default(bool[]);
        }

        public delegate AudioFile GetAudioFileInt32StringDelegate(int dialerId, string audioUrl);
        public GetAudioFileInt32StringDelegate GetAudioFileInt32String;

        AudioFile ITelephonyRecording.GetAudioFile(int dialerId, string audioUrl)
        {


            if (GetAudioFileInt32String != null)
            {
                return GetAudioFileInt32String(dialerId, audioUrl);
            } else if (_inner != null)
            {
                return ((ITelephonyRecording)_inner).GetAudioFile(dialerId, audioUrl);
            }

            return default(AudioFile);
        }

        public delegate DialerErrorCode StartPlaybackInt32Int64StringInt32Int32StringInt32OutDelegate(int dialerId, long campaignId, string agentId, int interviewId, int callId, string fileName, out int timeOfPlayingInSeconds);
        public StartPlaybackInt32Int64StringInt32Int32StringInt32OutDelegate StartPlaybackInt32Int64StringInt32Int32StringInt32Out;

        DialerErrorCode ITelephonyPlayback.StartPlayback(int dialerId, long campaignId, string agentId, int interviewId, int callId, string fileName, out int timeOfPlayingInSeconds)
        {
            timeOfPlayingInSeconds = default(int);


            if (StartPlaybackInt32Int64StringInt32Int32StringInt32Out != null)
            {
                return StartPlaybackInt32Int64StringInt32Int32StringInt32Out(dialerId, campaignId, agentId, interviewId, callId, fileName, out timeOfPlayingInSeconds);
            } else if (_inner != null)
            {
                return ((ITelephonyPlayback)_inner).StartPlayback(dialerId, campaignId, agentId, interviewId, callId, fileName, out timeOfPlayingInSeconds);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode StopPlaybackInt32Int64StringInt32Int32Delegate(int dialerId, long campaignId, string agentId, int interviewId, int callId);
        public StopPlaybackInt32Int64StringInt32Int32Delegate StopPlaybackInt32Int64StringInt32Int32;

        DialerErrorCode ITelephonyPlayback.StopPlayback(int dialerId, long campaignId, string agentId, int interviewId, int callId)
        {


            if (StopPlaybackInt32Int64StringInt32Int32 != null)
            {
                return StopPlaybackInt32Int64StringInt32Int32(dialerId, campaignId, agentId, interviewId, callId);
            } else if (_inner != null)
            {
                return ((ITelephonyPlayback)_inner).StopPlayback(dialerId, campaignId, agentId, interviewId, callId);
            }

            return default(DialerErrorCode);
        }

        public delegate bool IsPauseOrResumePlaybackSupportedNullableOfInt32Delegate(int? dialerId);
        public IsPauseOrResumePlaybackSupportedNullableOfInt32Delegate IsPauseOrResumePlaybackSupportedNullableOfInt32;

        bool ITelephonyPlayback.IsPauseOrResumePlaybackSupported(int? dialerId)
        {


            if (IsPauseOrResumePlaybackSupportedNullableOfInt32 != null)
            {
                return IsPauseOrResumePlaybackSupportedNullableOfInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ITelephonyPlayback)_inner).IsPauseOrResumePlaybackSupported(dialerId);
            }

            return default(bool);
        }

        public delegate bool IsToggleInterviewerListensToPlaybackOrRespondentSupportedNullableOfInt32Delegate(int? dialerId);
        public IsToggleInterviewerListensToPlaybackOrRespondentSupportedNullableOfInt32Delegate IsToggleInterviewerListensToPlaybackOrRespondentSupportedNullableOfInt32;

        bool ITelephonyPlayback.IsToggleInterviewerListensToPlaybackOrRespondentSupported(int? dialerId)
        {


            if (IsToggleInterviewerListensToPlaybackOrRespondentSupportedNullableOfInt32 != null)
            {
                return IsToggleInterviewerListensToPlaybackOrRespondentSupportedNullableOfInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ITelephonyPlayback)_inner).IsToggleInterviewerListensToPlaybackOrRespondentSupported(dialerId);
            }

            return default(bool);
        }

        public delegate DialerErrorCode PauseOrResumePlaybackInt32Int64StringInt32Int32Delegate(int dialerId, long campaignId, string agentId, int interviewId, int callId);
        public PauseOrResumePlaybackInt32Int64StringInt32Int32Delegate PauseOrResumePlaybackInt32Int64StringInt32Int32;

        DialerErrorCode ITelephonyPlayback.PauseOrResumePlayback(int dialerId, long campaignId, string agentId, int interviewId, int callId)
        {


            if (PauseOrResumePlaybackInt32Int64StringInt32Int32 != null)
            {
                return PauseOrResumePlaybackInt32Int64StringInt32Int32(dialerId, campaignId, agentId, interviewId, callId);
            } else if (_inner != null)
            {
                return ((ITelephonyPlayback)_inner).PauseOrResumePlayback(dialerId, campaignId, agentId, interviewId, callId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode ToggleInterviewerListensToPlaybackOrRespondentInt32Int64StringInt32Int32Delegate(int dialerId, long campaignId, string agentId, int interviewId, int callId);
        public ToggleInterviewerListensToPlaybackOrRespondentInt32Int64StringInt32Int32Delegate ToggleInterviewerListensToPlaybackOrRespondentInt32Int64StringInt32Int32;

        DialerErrorCode ITelephonyPlayback.ToggleInterviewerListensToPlaybackOrRespondent(int dialerId, long campaignId, string agentId, int interviewId, int callId)
        {


            if (ToggleInterviewerListensToPlaybackOrRespondentInt32Int64StringInt32Int32 != null)
            {
                return ToggleInterviewerListensToPlaybackOrRespondentInt32Int64StringInt32Int32(dialerId, campaignId, agentId, interviewId, callId);
            } else if (_inner != null)
            {
                return ((ITelephonyPlayback)_inner).ToggleInterviewerListensToPlaybackOrRespondent(dialerId, campaignId, agentId, interviewId, callId);
            }

            return default(DialerErrorCode);
        }

        public delegate IEnumerable<LogFileInfo> GetLogFilesInt32Delegate(int dialerId);
        public GetLogFilesInt32Delegate GetLogFilesInt32;

        IEnumerable<LogFileInfo> ITelephonyFacilities.GetLogFiles(int dialerId)
        {


            if (GetLogFilesInt32 != null)
            {
                return GetLogFilesInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ITelephonyFacilities)_inner).GetLogFiles(dialerId);
            }

            return default(IEnumerable<LogFileInfo>);
        }

        public delegate byte[] GetLogFileBodyZippedInt32StringDelegate(int dialerId, string fileName);
        public GetLogFileBodyZippedInt32StringDelegate GetLogFileBodyZippedInt32String;

        byte[] ITelephonyFacilities.GetLogFileBodyZipped(int dialerId, string fileName)
        {


            if (GetLogFileBodyZippedInt32String != null)
            {
                return GetLogFileBodyZippedInt32String(dialerId, fileName);
            } else if (_inner != null)
            {
                return ((ITelephonyFacilities)_inner).GetLogFileBodyZipped(dialerId, fileName);
            }

            return default(byte[]);
        }

        public delegate string GetDialerVersionInt32Delegate(int dialerId);
        public GetDialerVersionInt32Delegate GetDialerVersionInt32;

        string ITelephonyFacilities.GetDialerVersion(int dialerId)
        {


            if (GetDialerVersionInt32 != null)
            {
                return GetDialerVersionInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ITelephonyFacilities)_inner).GetDialerVersion(dialerId);
            }

            return default(string);
        }

        public delegate void SendGoReadyInt32Int64Int64FuncOfStringDelegate(int dialerId, long campaignId, long agentId, Func<string> logInfoFunc);
        public SendGoReadyInt32Int64Int64FuncOfStringDelegate SendGoReadyInt32Int64Int64FuncOfString;

        void ITelephony.SendGoReady(int dialerId, long campaignId, long agentId, Func<string> logInfoFunc)
        {

            if (SendGoReadyInt32Int64Int64FuncOfString != null)
            {
                SendGoReadyInt32Int64Int64FuncOfString(dialerId, campaignId, agentId, logInfoFunc);
            } else if (_inner != null)
            {
                ((ITelephony)_inner).SendGoReady(dialerId, campaignId, agentId, logInfoFunc);
            }
        }

        public delegate void SendGoNotReadyInt32Int64StringStringFuncOfStringDelegate(int dialerId, long campaignId, string agentId, string breakName, Func<string> logInfoFunc);
        public SendGoNotReadyInt32Int64StringStringFuncOfStringDelegate SendGoNotReadyInt32Int64StringStringFuncOfString;

        void ITelephony.SendGoNotReady(int dialerId, long campaignId, string agentId, string breakName, Func<string> logInfoFunc)
        {

            if (SendGoNotReadyInt32Int64StringStringFuncOfString != null)
            {
                SendGoNotReadyInt32Int64StringStringFuncOfString(dialerId, campaignId, agentId, breakName, logInfoFunc);
            } else if (_inner != null)
            {
                ((ITelephony)_inner).SendGoNotReady(dialerId, campaignId, agentId, breakName, logInfoFunc);
            }
        }

        public delegate void SendSetGroupsInt32Int64Int64ArrayOfInt32Delegate(int dialerId, long campaignId, long agentId, int[] userGroups);
        public SendSetGroupsInt32Int64Int64ArrayOfInt32Delegate SendSetGroupsInt32Int64Int64ArrayOfInt32;

        void ITelephony.SendSetGroups(int dialerId, long campaignId, long agentId, int[] userGroups)
        {

            if (SendSetGroupsInt32Int64Int64ArrayOfInt32 != null)
            {
                SendSetGroupsInt32Int64Int64ArrayOfInt32(dialerId, campaignId, agentId, userGroups);
            } else if (_inner != null)
            {
                ((ITelephony)_inner).SendSetGroups(dialerId, campaignId, agentId, userGroups);
            }
        }

        public delegate void UpdateDialersCollectionDelegate();
        public UpdateDialersCollectionDelegate UpdateDialersCollection;

        void ITelephony.UpdateDialersCollection()
        {

            if (UpdateDialersCollection != null)
            {
                UpdateDialersCollection();
            } else if (_inner != null)
            {
                ((ITelephony)_inner).UpdateDialersCollection();
            }
        }

    }
}