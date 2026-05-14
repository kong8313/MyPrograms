using System;
using Confirmit.CATI.Telephony;
using DialerCommon;
using ConfirmitDialerInterface;
using System.Collections.Generic;
using Confirmit.CATI.Common.Logging;

namespace Confirmit.CATI.Telephony.Fakes
{
    public class StubIDialerAPI : IDialerAPI 
    {
        private IDialerAPI _inner;

        public StubIDialerAPI()
        {
            _inner = null;
        }

        public IDialerAPI Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate DialerInitializeResult InitializeInt32StringStringStringStringBooleanDelegate(int dialerId, string tenantId, string connectionParametersXml, string configurationParametersXml, string surveyDefaultParametersXml, bool sendInitializeToWebService);
        public InitializeInt32StringStringStringStringBooleanDelegate InitializeInt32StringStringStringStringBoolean;

        DialerInitializeResult IDialerAPI.Initialize(int dialerId, string tenantId, string connectionParametersXml, string configurationParametersXml, string surveyDefaultParametersXml, bool sendInitializeToWebService)
        {


            if (InitializeInt32StringStringStringStringBoolean != null)
            {
                return InitializeInt32StringStringStringStringBoolean(dialerId, tenantId, connectionParametersXml, configurationParametersXml, surveyDefaultParametersXml, sendInitializeToWebService);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).Initialize(dialerId, tenantId, connectionParametersXml, configurationParametersXml, surveyDefaultParametersXml, sendInitializeToWebService);
            }

            return default(DialerInitializeResult);
        }

        public delegate int ReleaseInt32Int32Delegate(int dialerId, int companyId);
        public ReleaseInt32Int32Delegate ReleaseInt32Int32;

        int IDialerAPI.Release(int dialerId, int companyId)
        {


            if (ReleaseInt32Int32 != null)
            {
                return ReleaseInt32Int32(dialerId, companyId);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).Release(dialerId, companyId);
            }

            return default(int);
        }

        public delegate DialerFeatures GetFeaturesStringDelegate(string tenantId);
        public GetFeaturesStringDelegate GetFeaturesString;

        DialerFeatures IDialerAPI.GetFeatures(string tenantId)
        {


            if (GetFeaturesString != null)
            {
                return GetFeaturesString(tenantId);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).GetFeatures(tenantId);
            }

            return default(DialerFeatures);
        }

        public delegate int StartCampaignStringArrayOfInt32Int64StringDialingModeStringBooleanStringDelegate(string tenantId, int[] dialerIds, long campaignId, string campaignName, DialingMode dialingMode, string campaignType, bool recordWholeInterview, string surveyParametersXml);
        public StartCampaignStringArrayOfInt32Int64StringDialingModeStringBooleanStringDelegate StartCampaignStringArrayOfInt32Int64StringDialingModeStringBooleanString;

        int IDialerAPI.StartCampaign(string tenantId, int[] dialerIds, long campaignId, string campaignName, DialingMode dialingMode, string campaignType, bool recordWholeInterview, string surveyParametersXml)
        {


            if (StartCampaignStringArrayOfInt32Int64StringDialingModeStringBooleanString != null)
            {
                return StartCampaignStringArrayOfInt32Int64StringDialingModeStringBooleanString(tenantId, dialerIds, campaignId, campaignName, dialingMode, campaignType, recordWholeInterview, surveyParametersXml);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).StartCampaign(tenantId, dialerIds, campaignId, campaignName, dialingMode, campaignType, recordWholeInterview, surveyParametersXml);
            }

            return default(int);
        }

        public delegate int StopCampaignStringArrayOfInt32Int64DialingModeDelegate(string tenantId, int[] dialerIds, long campaignId, DialingMode dialingMode);
        public StopCampaignStringArrayOfInt32Int64DialingModeDelegate StopCampaignStringArrayOfInt32Int64DialingMode;

        int IDialerAPI.StopCampaign(string tenantId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {


            if (StopCampaignStringArrayOfInt32Int64DialingMode != null)
            {
                return StopCampaignStringArrayOfInt32Int64DialingMode(tenantId, dialerIds, campaignId, dialingMode);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).StopCampaign(tenantId, dialerIds, campaignId, dialingMode);
            }

            return default(int);
        }

        public delegate int KillCampaignStringArrayOfInt32Int64DialingModeDelegate(string tenantId, int[] dialerIds, long campaignId, DialingMode dialingMode);
        public KillCampaignStringArrayOfInt32Int64DialingModeDelegate KillCampaignStringArrayOfInt32Int64DialingMode;

        int IDialerAPI.KillCampaign(string tenantId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {


            if (KillCampaignStringArrayOfInt32Int64DialingMode != null)
            {
                return KillCampaignStringArrayOfInt32Int64DialingMode(tenantId, dialerIds, campaignId, dialingMode);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).KillCampaign(tenantId, dialerIds, campaignId, dialingMode);
            }

            return default(int);
        }

        public delegate int LoginStringInt64StringStringAgentTypeStringStringBooleanBooleanIEnumerableOfKeyValuePairOfStringStringDelegate(string tenantId, long campaignId, string agentId, string agentName, AgentType agentType, string agentExtension, string userId, bool isPredictive, bool isLocal, IEnumerable<KeyValuePair<string, string>> agentAttributes);
        public LoginStringInt64StringStringAgentTypeStringStringBooleanBooleanIEnumerableOfKeyValuePairOfStringStringDelegate LoginStringInt64StringStringAgentTypeStringStringBooleanBooleanIEnumerableOfKeyValuePairOfStringString;

        int IDialerAPI.Login(string tenantId, long campaignId, string agentId, string agentName, AgentType agentType, string agentExtension, string userId, bool isPredictive, bool isLocal, IEnumerable<KeyValuePair<string, string>> agentAttributes)
        {


            if (LoginStringInt64StringStringAgentTypeStringStringBooleanBooleanIEnumerableOfKeyValuePairOfStringString != null)
            {
                return LoginStringInt64StringStringAgentTypeStringStringBooleanBooleanIEnumerableOfKeyValuePairOfStringString(tenantId, campaignId, agentId, agentName, agentType, agentExtension, userId, isPredictive, isLocal, agentAttributes);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).Login(tenantId, campaignId, agentId, agentName, agentType, agentExtension, userId, isPredictive, isLocal, agentAttributes);
            }

            return default(int);
        }

        public delegate int SetCampaignInt32Int64Int32Delegate(int companyId, long campaignId, int agentId);
        public SetCampaignInt32Int64Int32Delegate SetCampaignInt32Int64Int32;

        int IDialerAPI.SetCampaign(int companyId, long campaignId, int agentId)
        {


            if (SetCampaignInt32Int64Int32 != null)
            {
                return SetCampaignInt32Int64Int32(companyId, campaignId, agentId);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).SetCampaign(companyId, campaignId, agentId);
            }

            return default(int);
        }

        public delegate int LogoutStringInt64BooleanStringDelegate(string tenantId, long campaignId, bool isPredictive, string agentId);
        public LogoutStringInt64BooleanStringDelegate LogoutStringInt64BooleanString;

        int IDialerAPI.Logout(string tenantId, long campaignId, bool isPredictive, string agentId)
        {


            if (LogoutStringInt64BooleanString != null)
            {
                return LogoutStringInt64BooleanString(tenantId, campaignId, isPredictive, agentId);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).Logout(tenantId, campaignId, isPredictive, agentId);
            }

            return default(int);
        }

        public delegate int KillAgentStringInt64StringDelegate(string tenantId, long campaignId, string agentId);
        public KillAgentStringInt64StringDelegate KillAgentStringInt64String;

        int IDialerAPI.KillAgent(string tenantId, long campaignId, string agentId)
        {


            if (KillAgentStringInt64String != null)
            {
                return KillAgentStringInt64String(tenantId, campaignId, agentId);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).KillAgent(tenantId, campaignId, agentId);
            }

            return default(int);
        }

        public delegate int GoReadyStringInt64StringDelegate(string tenantId, long campaignId, string agentId);
        public GoReadyStringInt64StringDelegate GoReadyStringInt64String;

        int IDialerAPI.GoReady(string tenantId, long campaignId, string agentId)
        {


            if (GoReadyStringInt64String != null)
            {
                return GoReadyStringInt64String(tenantId, campaignId, agentId);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).GoReady(tenantId, campaignId, agentId);
            }

            return default(int);
        }

        public delegate int GoNotReadyStringInt64StringStringDelegate(string tenantId, long campaignId, string agentId, string breakName);
        public GoNotReadyStringInt64StringStringDelegate GoNotReadyStringInt64StringString;

        int IDialerAPI.GoNotReady(string tenantId, long campaignId, string agentId, string breakName)
        {


            if (GoNotReadyStringInt64StringString != null)
            {
                return GoNotReadyStringInt64StringString(tenantId, campaignId, agentId, breakName);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).GoNotReady(tenantId, campaignId, agentId, breakName);
            }

            return default(int);
        }

        public delegate int SendNumberStringInt64DialingModeInt32Int32Int32StringInt32BooleanDelegate(string tenantId, long campaignId, DialingMode diallingMode, int groupId, int contactId, int callId, string phoneNumber, int callAgingTimeout, bool isRecording);
        public SendNumberStringInt64DialingModeInt32Int32Int32StringInt32BooleanDelegate SendNumberStringInt64DialingModeInt32Int32Int32StringInt32Boolean;

        int IDialerAPI.SendNumber(string tenantId, long campaignId, DialingMode diallingMode, int groupId, int contactId, int callId, string phoneNumber, int callAgingTimeout, bool isRecording)
        {


            if (SendNumberStringInt64DialingModeInt32Int32Int32StringInt32Boolean != null)
            {
                return SendNumberStringInt64DialingModeInt32Int32Int32StringInt32Boolean(tenantId, campaignId, diallingMode, groupId, contactId, callId, phoneNumber, callAgingTimeout, isRecording);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).SendNumber(tenantId, campaignId, diallingMode, groupId, contactId, callId, phoneNumber, callAgingTimeout, isRecording);
            }

            return default(int);
        }

        public delegate int SendNumbersStringStringInt64DialingModeListOfCallInfoInt32BooleanDelegate(string requestId, string tenantId, long campaignId, DialingMode campaignDiallingMode, List<CallInfo> callList, int callAgingTimeout, bool isRecording);
        public SendNumbersStringStringInt64DialingModeListOfCallInfoInt32BooleanDelegate SendNumbersStringStringInt64DialingModeListOfCallInfoInt32Boolean;

        int IDialerAPI.SendNumbers(string requestId, string tenantId, long campaignId, DialingMode campaignDiallingMode, List<CallInfo> callList, int callAgingTimeout, bool isRecording)
        {


            if (SendNumbersStringStringInt64DialingModeListOfCallInfoInt32Boolean != null)
            {
                return SendNumbersStringStringInt64DialingModeListOfCallInfoInt32Boolean(requestId, tenantId, campaignId, campaignDiallingMode, callList, callAgingTimeout, isRecording);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).SendNumbers(requestId, tenantId, campaignId, campaignDiallingMode, callList, callAgingTimeout, isRecording);
            }

            return default(int);
        }

        public delegate int SendNumberToAgentStringInt64StringDialingModeInt32Int32StringBooleanStringDictionaryOfStringObjectDelegate(string tenantId, long campaignId, string agentId, DialingMode diallingMode, int contactId, int callId, string phoneNumber, bool isRecording, string callerId, Dictionary<string, Object> respondentVariables);
        public SendNumberToAgentStringInt64StringDialingModeInt32Int32StringBooleanStringDictionaryOfStringObjectDelegate SendNumberToAgentStringInt64StringDialingModeInt32Int32StringBooleanStringDictionaryOfStringObject;

        int IDialerAPI.SendNumberToAgent(string tenantId, long campaignId, string agentId, DialingMode diallingMode, int contactId, int callId, string phoneNumber, bool isRecording, string callerId, Dictionary<string, Object> respondentVariables)
        {


            if (SendNumberToAgentStringInt64StringDialingModeInt32Int32StringBooleanStringDictionaryOfStringObject != null)
            {
                return SendNumberToAgentStringInt64StringDialingModeInt32Int32StringBooleanStringDictionaryOfStringObject(tenantId, campaignId, agentId, diallingMode, contactId, callId, phoneNumber, isRecording, callerId, respondentVariables);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).SendNumberToAgent(tenantId, campaignId, agentId, diallingMode, contactId, callId, phoneNumber, isRecording, callerId, respondentVariables);
            }

            return default(int);
        }

        public delegate int SendNumberToAgentExStringInt64StringDialingModeInt32Int32StringInt32BooleanDelegate(string tenantId, long campaignId, string agentId, DialingMode dialingMode, int contactId, int callId, string phoneNumber, int callAgingTimeout, bool isRecording);
        public SendNumberToAgentExStringInt64StringDialingModeInt32Int32StringInt32BooleanDelegate SendNumberToAgentExStringInt64StringDialingModeInt32Int32StringInt32Boolean;

        int IDialerAPI.SendNumberToAgentEx(string tenantId, long campaignId, string agentId, DialingMode dialingMode, int contactId, int callId, string phoneNumber, int callAgingTimeout, bool isRecording)
        {


            if (SendNumberToAgentExStringInt64StringDialingModeInt32Int32StringInt32Boolean != null)
            {
                return SendNumberToAgentExStringInt64StringDialingModeInt32Int32StringInt32Boolean(tenantId, campaignId, agentId, dialingMode, contactId, callId, phoneNumber, callAgingTimeout, isRecording);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).SendNumberToAgentEx(tenantId, campaignId, agentId, dialingMode, contactId, callId, phoneNumber, callAgingTimeout, isRecording);
            }

            return default(int);
        }

        public delegate int RedialStringInt64StringInt32Int32StringBooleanStringDelegate(string tenantId, long campaignId, string agentId, int contactId, int callId, string phoneNumber, bool isRecording, string callerId);
        public RedialStringInt64StringInt32Int32StringBooleanStringDelegate RedialStringInt64StringInt32Int32StringBooleanString;

        int IDialerAPI.Redial(string tenantId, long campaignId, string agentId, int contactId, int callId, string phoneNumber, bool isRecording, string callerId)
        {


            if (RedialStringInt64StringInt32Int32StringBooleanString != null)
            {
                return RedialStringInt64StringInt32Int32StringBooleanString(tenantId, campaignId, agentId, contactId, callId, phoneNumber, isRecording, callerId);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).Redial(tenantId, campaignId, agentId, contactId, callId, phoneNumber, isRecording, callerId);
            }

            return default(int);
        }

        public delegate int HangupStringInt64StringInt32Int64Delegate(string tenantId, long campaignId, string agentId, int interviewId, long callId);
        public HangupStringInt64StringInt32Int64Delegate HangupStringInt64StringInt32Int64;

        int IDialerAPI.Hangup(string tenantId, long campaignId, string agentId, int interviewId, long callId)
        {


            if (HangupStringInt64StringInt32Int64 != null)
            {
                return HangupStringInt64StringInt32Int64(tenantId, campaignId, agentId, interviewId, callId);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).Hangup(tenantId, campaignId, agentId, interviewId, callId);
            }

            return default(int);
        }

        public delegate int CompleteCallStringInt64StringInterviewStatusBooleanStringInt32Int64Delegate(string tenantId, long campaignId, string agentId, InterviewStatus interviewStatus, bool makeAgentReady, string breakName, int interviewId, long callId);
        public CompleteCallStringInt64StringInterviewStatusBooleanStringInt32Int64Delegate CompleteCallStringInt64StringInterviewStatusBooleanStringInt32Int64;

        int IDialerAPI.CompleteCall(string tenantId, long campaignId, string agentId, InterviewStatus interviewStatus, bool makeAgentReady, string breakName, int interviewId, long callId)
        {


            if (CompleteCallStringInt64StringInterviewStatusBooleanStringInt32Int64 != null)
            {
                return CompleteCallStringInt64StringInterviewStatusBooleanStringInt32Int64(tenantId, campaignId, agentId, interviewStatus, makeAgentReady, breakName, interviewId, callId);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).CompleteCall(tenantId, campaignId, agentId, interviewStatus, makeAgentReady, breakName, interviewId, callId);
            }

            return default(int);
        }

        public delegate int SetNextInterviewStringInt64StringInterviewStatusInt64Int32Int64Delegate(string tenantId, long currentCampaignId, string agentId, InterviewStatus currentInterviewStatus, long nextCampaignId, int nextInterviewId, long nextCallId);
        public SetNextInterviewStringInt64StringInterviewStatusInt64Int32Int64Delegate SetNextInterviewStringInt64StringInterviewStatusInt64Int32Int64;

        int IDialerAPI.SetNextInterview(string tenantId, long currentCampaignId, string agentId, InterviewStatus currentInterviewStatus, long nextCampaignId, int nextInterviewId, long nextCallId)
        {


            if (SetNextInterviewStringInt64StringInterviewStatusInt64Int32Int64 != null)
            {
                return SetNextInterviewStringInt64StringInterviewStatusInt64Int32Int64(tenantId, currentCampaignId, agentId, currentInterviewStatus, nextCampaignId, nextInterviewId, nextCallId);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).SetNextInterview(tenantId, currentCampaignId, agentId, currentInterviewStatus, nextCampaignId, nextInterviewId, nextCallId);
            }

            return default(int);
        }

        public delegate int StartCustomIvrInterviewStringInt64StringInt32Int64StringDelegate(string tenantId, long campaignId, string agentId, int interviewId, long callId, string respondentSurveyLink);
        public StartCustomIvrInterviewStringInt64StringInt32Int64StringDelegate StartCustomIvrInterviewStringInt64StringInt32Int64String;

        int IDialerAPI.StartCustomIvrInterview(string tenantId, long campaignId, string agentId, int interviewId, long callId, string respondentSurveyLink)
        {


            if (StartCustomIvrInterviewStringInt64StringInt32Int64String != null)
            {
                return StartCustomIvrInterviewStringInt64StringInt32Int64String(tenantId, campaignId, agentId, interviewId, callId, respondentSurveyLink);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).StartCustomIvrInterview(tenantId, campaignId, agentId, interviewId, callId, respondentSurveyLink);
            }

            return default(int);
        }

        public delegate int UpdateInterviewStatusStringInt64StringInt32Int32InterviewStatusDelegate(string tenantId, long campaignId, string agentId, int interviewId, int callId, InterviewStatus interviewStatus);
        public UpdateInterviewStatusStringInt64StringInt32Int32InterviewStatusDelegate UpdateInterviewStatusStringInt64StringInt32Int32InterviewStatus;

        int IDialerAPI.UpdateInterviewStatus(string tenantId, long campaignId, string agentId, int interviewId, int callId, InterviewStatus interviewStatus)
        {


            if (UpdateInterviewStatusStringInt64StringInt32Int32InterviewStatus != null)
            {
                return UpdateInterviewStatusStringInt64StringInt32Int32InterviewStatus(tenantId, campaignId, agentId, interviewId, callId, interviewStatus);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).UpdateInterviewStatus(tenantId, campaignId, agentId, interviewId, callId, interviewStatus);
            }

            return default(int);
        }

        public delegate int SetTuningStringInt64StringStringStringStringStringStringDelegate(string tenantId, long campaignId, string abandonTarget, string abandonDelay, string estimatedTalkTime, string ringTimeoutOut, string previewTimeOut, string restrainedDialling);
        public SetTuningStringInt64StringStringStringStringStringStringDelegate SetTuningStringInt64StringStringStringStringStringString;

        int IDialerAPI.SetTuning(string tenantId, long campaignId, string abandonTarget, string abandonDelay, string estimatedTalkTime, string ringTimeoutOut, string previewTimeOut, string restrainedDialling)
        {


            if (SetTuningStringInt64StringStringStringStringStringString != null)
            {
                return SetTuningStringInt64StringStringStringStringStringString(tenantId, campaignId, abandonTarget, abandonDelay, estimatedTalkTime, ringTimeoutOut, previewTimeOut, restrainedDialling);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).SetTuning(tenantId, campaignId, abandonTarget, abandonDelay, estimatedTalkTime, ringTimeoutOut, previewTimeOut, restrainedDialling);
            }

            return default(int);
        }

        public delegate int SetGroupsStringInt64StringArrayOfInt32Delegate(string tenantId, long campaignId, string agentId, int[] agentGroups);
        public SetGroupsStringInt64StringArrayOfInt32Delegate SetGroupsStringInt64StringArrayOfInt32;

        int IDialerAPI.SetGroups(string tenantId, long campaignId, string agentId, int[] agentGroups)
        {


            if (SetGroupsStringInt64StringArrayOfInt32 != null)
            {
                return SetGroupsStringInt64StringArrayOfInt32(tenantId, campaignId, agentId, agentGroups);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).SetGroups(tenantId, campaignId, agentId, agentGroups);
            }

            return default(int);
        }

        public delegate int FlushNumbersStringArrayOfInt32Int64ListOfCallInfoDelegate(string tenantId, int[] dialerIds, long campaignId, List<CallInfo> callsList);
        public FlushNumbersStringArrayOfInt32Int64ListOfCallInfoDelegate FlushNumbersStringArrayOfInt32Int64ListOfCallInfo;

        int IDialerAPI.FlushNumbers(string tenantId, int[] dialerIds, long campaignId, List<CallInfo> callsList)
        {


            if (FlushNumbersStringArrayOfInt32Int64ListOfCallInfo != null)
            {
                return FlushNumbersStringArrayOfInt32Int64ListOfCallInfo(tenantId, dialerIds, campaignId, callsList);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).FlushNumbers(tenantId, dialerIds, campaignId, callsList);
            }

            return default(int);
        }

        public delegate int StartRecordingStringInt64StringInt32Int32StringDelegate(string tenantId, long campaignId, string agentId, int contactId, int callId, string label);
        public StartRecordingStringInt64StringInt32Int32StringDelegate StartRecordingStringInt64StringInt32Int32String;

        int IDialerAPI.StartRecording(string tenantId, long campaignId, string agentId, int contactId, int callId, string label)
        {


            if (StartRecordingStringInt64StringInt32Int32String != null)
            {
                return StartRecordingStringInt64StringInt32Int32String(tenantId, campaignId, agentId, contactId, callId, label);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).StartRecording(tenantId, campaignId, agentId, contactId, callId, label);
            }

            return default(int);
        }

        public delegate int StopRecordingStringInt64StringInt32Int32StopRecordingModeDelegate(string tenantId, long campaignId, string agentId, int contactId, int callId, StopRecordingMode stopRecordingMode);
        public StopRecordingStringInt64StringInt32Int32StopRecordingModeDelegate StopRecordingStringInt64StringInt32Int32StopRecordingMode;

        int IDialerAPI.StopRecording(string tenantId, long campaignId, string agentId, int contactId, int callId, StopRecordingMode stopRecordingMode)
        {


            if (StopRecordingStringInt64StringInt32Int32StopRecordingMode != null)
            {
                return StopRecordingStringInt64StringInt32Int32StopRecordingMode(tenantId, campaignId, agentId, contactId, callId, stopRecordingMode);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).StopRecording(tenantId, campaignId, agentId, contactId, callId, stopRecordingMode);
            }

            return default(int);
        }

        public delegate int StartPlaybackStringInt64StringInt32Int32StringInt32OutDelegate(string tenantId, long campaignId, string agentId, int interviewId, int callId, string fileName, out int timeOfPlayingInSeconds);
        public StartPlaybackStringInt64StringInt32Int32StringInt32OutDelegate StartPlaybackStringInt64StringInt32Int32StringInt32Out;

        int IDialerAPI.StartPlayback(string tenantId, long campaignId, string agentId, int interviewId, int callId, string fileName, out int timeOfPlayingInSeconds)
        {
            timeOfPlayingInSeconds = default(int);


            if (StartPlaybackStringInt64StringInt32Int32StringInt32Out != null)
            {
                return StartPlaybackStringInt64StringInt32Int32StringInt32Out(tenantId, campaignId, agentId, interviewId, callId, fileName, out timeOfPlayingInSeconds);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).StartPlayback(tenantId, campaignId, agentId, interviewId, callId, fileName, out timeOfPlayingInSeconds);
            }

            return default(int);
        }

        public delegate int StopPlaybackStringInt64StringInt32Delegate(string tenantId, long campaignId, string agentId, int callId);
        public StopPlaybackStringInt64StringInt32Delegate StopPlaybackStringInt64StringInt32;

        int IDialerAPI.StopPlayback(string tenantId, long campaignId, string agentId, int callId)
        {


            if (StopPlaybackStringInt64StringInt32 != null)
            {
                return StopPlaybackStringInt64StringInt32(tenantId, campaignId, agentId, callId);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).StopPlayback(tenantId, campaignId, agentId, callId);
            }

            return default(int);
        }

        public delegate int PauseOrResumePlaybackStringInt64StringInt32Delegate(string tenantId, long campaignId, string agentId, int callId);
        public PauseOrResumePlaybackStringInt64StringInt32Delegate PauseOrResumePlaybackStringInt64StringInt32;

        int IDialerAPI.PauseOrResumePlayback(string tenantId, long campaignId, string agentId, int callId)
        {


            if (PauseOrResumePlaybackStringInt64StringInt32 != null)
            {
                return PauseOrResumePlaybackStringInt64StringInt32(tenantId, campaignId, agentId, callId);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).PauseOrResumePlayback(tenantId, campaignId, agentId, callId);
            }

            return default(int);
        }

        public delegate int ToggleInterviewerListensToPlaybackOrRespondentStringInt64StringInt32Delegate(string tenantId, long campaignId, string agentId, int callId);
        public ToggleInterviewerListensToPlaybackOrRespondentStringInt64StringInt32Delegate ToggleInterviewerListensToPlaybackOrRespondentStringInt64StringInt32;

        int IDialerAPI.ToggleInterviewerListensToPlaybackOrRespondent(string tenantId, long campaignId, string agentId, int callId)
        {


            if (ToggleInterviewerListensToPlaybackOrRespondentStringInt64StringInt32 != null)
            {
                return ToggleInterviewerListensToPlaybackOrRespondentStringInt64StringInt32(tenantId, campaignId, agentId, callId);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).ToggleInterviewerListensToPlaybackOrRespondent(tenantId, campaignId, agentId, callId);
            }

            return default(int);
        }

        public delegate int StartMonitorStringStringStringStringRefDelegate(string tenantId, string agentId, string phoneNumber, ref string sessionId);
        public StartMonitorStringStringStringStringRefDelegate StartMonitorStringStringStringStringRef;

        int IDialerAPI.StartMonitor(string tenantId, string agentId, string phoneNumber, ref string sessionId)
        {


            if (StartMonitorStringStringStringStringRef != null)
            {
                return StartMonitorStringStringStringStringRef(tenantId, agentId, phoneNumber, ref sessionId);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).StartMonitor(tenantId, agentId, phoneNumber, ref sessionId);
            }

            return default(int);
        }

        public delegate int StopMonitorStringStringDelegate(string tenantId, string sessionId);
        public StopMonitorStringStringDelegate StopMonitorStringString;

        int IDialerAPI.StopMonitor(string tenantId, string sessionId)
        {


            if (StopMonitorStringString != null)
            {
                return StopMonitorStringString(tenantId, sessionId);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).StopMonitor(tenantId, sessionId);
            }

            return default(int);
        }

        public delegate int SetMonitorModeStringStringMonitorModeDelegate(string tenantId, string sessionId, MonitorMode monitorMode);
        public SetMonitorModeStringStringMonitorModeDelegate SetMonitorModeStringStringMonitorMode;

        int IDialerAPI.SetMonitorMode(string tenantId, string sessionId, MonitorMode monitorMode)
        {


            if (SetMonitorModeStringStringMonitorMode != null)
            {
                return SetMonitorModeStringStringMonitorMode(tenantId, sessionId, monitorMode);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).SetMonitorMode(tenantId, sessionId, monitorMode);
            }

            return default(int);
        }

        public delegate int CompletePreviewStringInt64StringInt32Int32StringBooleanDelegate(string tenantId, long campaignId, string agentId, int contactId, int callId, string phoneNumber, bool isRecording);
        public CompletePreviewStringInt64StringInt32Int32StringBooleanDelegate CompletePreviewStringInt64StringInt32Int32StringBoolean;

        int IDialerAPI.CompletePreview(string tenantId, long campaignId, string agentId, int contactId, int callId, string phoneNumber, bool isRecording)
        {


            if (CompletePreviewStringInt64StringInt32Int32StringBoolean != null)
            {
                return CompletePreviewStringInt64StringInt32Int32StringBoolean(tenantId, campaignId, agentId, contactId, callId, phoneNumber, isRecording);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).CompletePreview(tenantId, campaignId, agentId, contactId, callId, phoneNumber, isRecording);
            }

            return default(int);
        }

        public delegate bool IsPersonModeSupportedAgentTaskChoiceModeDelegate(AgentTaskChoiceMode mode);
        public IsPersonModeSupportedAgentTaskChoiceModeDelegate IsPersonModeSupportedAgentTaskChoiceMode;

        bool IDialerAPI.IsPersonModeSupported(AgentTaskChoiceMode mode)
        {


            if (IsPersonModeSupportedAgentTaskChoiceMode != null)
            {
                return IsPersonModeSupportedAgentTaskChoiceMode(mode);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).IsPersonModeSupported(mode);
            }

            return default(bool);
        }

        public delegate bool IsReloginNeededOnSurveyChangeDelegate();
        public IsReloginNeededOnSurveyChangeDelegate IsReloginNeededOnSurveyChange;

        bool IDialerAPI.IsReloginNeededOnSurveyChange()
        {


            if (IsReloginNeededOnSurveyChange != null)
            {
                return IsReloginNeededOnSurveyChange();
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).IsReloginNeededOnSurveyChange();
            }

            return default(bool);
        }

        public delegate bool HasInternalHealthControlDelegate();
        public HasInternalHealthControlDelegate HasInternalHealthControl;

        bool IDialerAPI.HasInternalHealthControl()
        {


            if (HasInternalHealthControl != null)
            {
                return HasInternalHealthControl();
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).HasInternalHealthControl();
            }

            return default(bool);
        }

        public delegate bool IsDynamicExtensionNumberAllowedBooleanDelegate(bool isAgentLocal);
        public IsDynamicExtensionNumberAllowedBooleanDelegate IsDynamicExtensionNumberAllowedBoolean;

        bool IDialerAPI.IsDynamicExtensionNumberAllowed(bool isAgentLocal)
        {


            if (IsDynamicExtensionNumberAllowedBoolean != null)
            {
                return IsDynamicExtensionNumberAllowedBoolean(isAgentLocal);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).IsDynamicExtensionNumberAllowed(isAgentLocal);
            }

            return default(bool);
        }

        public delegate DialerState GetStateInt32StringDelegate(int dialerId, string tenantId);
        public GetStateInt32StringDelegate GetStateInt32String;

        DialerState IDialerAPI.GetState(int dialerId, string tenantId)
        {


            if (GetStateInt32String != null)
            {
                return GetStateInt32String(dialerId, tenantId);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).GetState(dialerId, tenantId);
            }

            return default(DialerState);
        }

        public delegate CallOutcome TranslateOutcomeInt64Delegate(long outcome);
        public TranslateOutcomeInt64Delegate TranslateOutcomeInt64;

        CallOutcome IDialerAPI.TranslateOutcome(long outcome)
        {


            if (TranslateOutcomeInt64 != null)
            {
                return TranslateOutcomeInt64(outcome);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).TranslateOutcome(outcome);
            }

            return default(CallOutcome);
        }

        public delegate int SetConfigurationParametersStringStringDelegate(string tenantId, string configurationParametersXml);
        public SetConfigurationParametersStringStringDelegate SetConfigurationParametersStringString;

        int IDialerAPI.SetConfigurationParameters(string tenantId, string configurationParametersXml)
        {


            if (SetConfigurationParametersStringString != null)
            {
                return SetConfigurationParametersStringString(tenantId, configurationParametersXml);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).SetConfigurationParameters(tenantId, configurationParametersXml);
            }

            return default(int);
        }

        public delegate int ValidateCampaignParametersStringDelegate(string surveyParametersXml);
        public ValidateCampaignParametersStringDelegate ValidateCampaignParametersString;

        int IDialerAPI.ValidateCampaignParameters(string surveyParametersXml)
        {


            if (ValidateCampaignParametersString != null)
            {
                return ValidateCampaignParametersString(surveyParametersXml);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).ValidateCampaignParameters(surveyParametersXml);
            }

            return default(int);
        }

        public delegate int SetCampaignParametersStringArrayOfInt32Int64DialingModeBooleanStringDelegate(string tenantId, int[] dialerIds, long campaignId, DialingMode dialingMode, bool recordWholeInterview, string surveyParametersXml);
        public SetCampaignParametersStringArrayOfInt32Int64DialingModeBooleanStringDelegate SetCampaignParametersStringArrayOfInt32Int64DialingModeBooleanString;

        int IDialerAPI.SetCampaignParameters(string tenantId, int[] dialerIds, long campaignId, DialingMode dialingMode, bool recordWholeInterview, string surveyParametersXml)
        {


            if (SetCampaignParametersStringArrayOfInt32Int64DialingModeBooleanString != null)
            {
                return SetCampaignParametersStringArrayOfInt32Int64DialingModeBooleanString(tenantId, dialerIds, campaignId, dialingMode, recordWholeInterview, surveyParametersXml);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).SetCampaignParameters(tenantId, dialerIds, campaignId, dialingMode, recordWholeInterview, surveyParametersXml);
            }

            return default(int);
        }

        public delegate int GetTrunkLineStatesAndAlarmsStringInt32IEnumerableOfTrunkLineStateAndAlarmsOutDelegate(string tenantId, int dialerId, out IEnumerable<TrunkLineStateAndAlarms> trunkLineStatesAndAlarms);
        public GetTrunkLineStatesAndAlarmsStringInt32IEnumerableOfTrunkLineStateAndAlarmsOutDelegate GetTrunkLineStatesAndAlarmsStringInt32IEnumerableOfTrunkLineStateAndAlarmsOut;

        int IDialerAPI.GetTrunkLineStatesAndAlarms(string tenantId, int dialerId, out IEnumerable<TrunkLineStateAndAlarms> trunkLineStatesAndAlarms)
        {
            trunkLineStatesAndAlarms = default(IEnumerable<TrunkLineStateAndAlarms>);


            if (GetTrunkLineStatesAndAlarmsStringInt32IEnumerableOfTrunkLineStateAndAlarmsOut != null)
            {
                return GetTrunkLineStatesAndAlarmsStringInt32IEnumerableOfTrunkLineStateAndAlarmsOut(tenantId, dialerId, out trunkLineStatesAndAlarms);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).GetTrunkLineStatesAndAlarms(tenantId, dialerId, out trunkLineStatesAndAlarms);
            }

            return default(int);
        }

        public delegate int TransferToIvrStringInt64StringInt32Int32StringIEnumerableOfKeyValuePairOfStringStringDelegate(string tenantId, long campaignId, string agentId, int interviewId, int callId, string endpoint, IEnumerable<KeyValuePair<string, string>> attributes);
        public TransferToIvrStringInt64StringInt32Int32StringIEnumerableOfKeyValuePairOfStringStringDelegate TransferToIvrStringInt64StringInt32Int32StringIEnumerableOfKeyValuePairOfStringString;

        int IDialerAPI.TransferToIvr(string tenantId, long campaignId, string agentId, int interviewId, int callId, string endpoint, IEnumerable<KeyValuePair<string, string>> attributes)
        {


            if (TransferToIvrStringInt64StringInt32Int32StringIEnumerableOfKeyValuePairOfStringString != null)
            {
                return TransferToIvrStringInt64StringInt32Int32StringIEnumerableOfKeyValuePairOfStringString(tenantId, campaignId, agentId, interviewId, callId, endpoint, attributes);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).TransferToIvr(tenantId, campaignId, agentId, interviewId, callId, endpoint, attributes);
            }

            return default(int);
        }

        public delegate int IvrRenderVoiceXmlInt32Int64Int32StringDelegate(int companyId, long campaignId, int agentId, string voiceXml);
        public IvrRenderVoiceXmlInt32Int64Int32StringDelegate IvrRenderVoiceXmlInt32Int64Int32String;

        int IDialerAPI.IvrRenderVoiceXml(int companyId, long campaignId, int agentId, string voiceXml)
        {


            if (IvrRenderVoiceXmlInt32Int64Int32String != null)
            {
                return IvrRenderVoiceXmlInt32Int64Int32String(companyId, campaignId, agentId, voiceXml);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).IvrRenderVoiceXml(companyId, campaignId, agentId, voiceXml);
            }

            return default(int);
        }

        public delegate DialerErrorCode[] ConfigureInboundDdiNumbersInt32ArrayOfInboundDdiNumberDelegate(int companyId, InboundDdiNumber[] inboundDdiNumbers);
        public ConfigureInboundDdiNumbersInt32ArrayOfInboundDdiNumberDelegate ConfigureInboundDdiNumbersInt32ArrayOfInboundDdiNumber;

        DialerErrorCode[] IDialerAPI.ConfigureInboundDdiNumbers(int companyId, InboundDdiNumber[] inboundDdiNumbers)
        {


            if (ConfigureInboundDdiNumbersInt32ArrayOfInboundDdiNumber != null)
            {
                return ConfigureInboundDdiNumbersInt32ArrayOfInboundDdiNumber(companyId, inboundDdiNumbers);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).ConfigureInboundDdiNumbers(companyId, inboundDdiNumbers);
            }

            return default(DialerErrorCode[]);
        }

        public delegate int DropInboundCallInt32StringAudioMessageDescriptorDelegate(int companyId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor);
        public DropInboundCallInt32StringAudioMessageDescriptorDelegate DropInboundCallInt32StringAudioMessageDescriptor;

        int IDialerAPI.DropInboundCall(int companyId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor)
        {


            if (DropInboundCallInt32StringAudioMessageDescriptor != null)
            {
                return DropInboundCallInt32StringAudioMessageDescriptor(companyId, inboundCallId, audioMessageDescriptor);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).DropInboundCall(companyId, inboundCallId, audioMessageDescriptor);
            }

            return default(int);
        }

        public delegate int ConnectInboundCallInt32Int64StringCallInfoArrayOfInt64AudioMessageDescriptorDelegate(int companyId, long campaignId, string inboundCallId, CallInfo callInfo, long[] campaignIdsToBorrowAgentsFrom, AudioMessageDescriptor audioMessageDescriptor);
        public ConnectInboundCallInt32Int64StringCallInfoArrayOfInt64AudioMessageDescriptorDelegate ConnectInboundCallInt32Int64StringCallInfoArrayOfInt64AudioMessageDescriptor;

        int IDialerAPI.ConnectInboundCall(int companyId, long campaignId, string inboundCallId, CallInfo callInfo, long[] campaignIdsToBorrowAgentsFrom, AudioMessageDescriptor audioMessageDescriptor)
        {


            if (ConnectInboundCallInt32Int64StringCallInfoArrayOfInt64AudioMessageDescriptor != null)
            {
                return ConnectInboundCallInt32Int64StringCallInfoArrayOfInt64AudioMessageDescriptor(companyId, campaignId, inboundCallId, callInfo, campaignIdsToBorrowAgentsFrom, audioMessageDescriptor);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).ConnectInboundCall(companyId, campaignId, inboundCallId, callInfo, campaignIdsToBorrowAgentsFrom, audioMessageDescriptor);
            }

            return default(int);
        }

        public delegate int ConnectInboundCallToAgentInt32Int64StringCallInfoAudioMessageDescriptorDelegate(int companyId, long campaignId, string inboundCallId, CallInfo callInfo, AudioMessageDescriptor audioMessageDescriptor);
        public ConnectInboundCallToAgentInt32Int64StringCallInfoAudioMessageDescriptorDelegate ConnectInboundCallToAgentInt32Int64StringCallInfoAudioMessageDescriptor;

        int IDialerAPI.ConnectInboundCallToAgent(int companyId, long campaignId, string inboundCallId, CallInfo callInfo, AudioMessageDescriptor audioMessageDescriptor)
        {


            if (ConnectInboundCallToAgentInt32Int64StringCallInfoAudioMessageDescriptor != null)
            {
                return ConnectInboundCallToAgentInt32Int64StringCallInfoAudioMessageDescriptor(companyId, campaignId, inboundCallId, callInfo, audioMessageDescriptor);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).ConnectInboundCallToAgent(companyId, campaignId, inboundCallId, callInfo, audioMessageDescriptor);
            }

            return default(int);
        }

        public delegate int TransferStartInt32Int64StringInt32TransferTypeDelegate(int companyId, long campaignId, string transferId, int agentId, TransferType transferType);
        public TransferStartInt32Int64StringInt32TransferTypeDelegate TransferStartInt32Int64StringInt32TransferType;

        int IDialerAPI.TransferStart(int companyId, long campaignId, string transferId, int agentId, TransferType transferType)
        {


            if (TransferStartInt32Int64StringInt32TransferType != null)
            {
                return TransferStartInt32Int64StringInt32TransferType(companyId, campaignId, transferId, agentId, transferType);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).TransferStart(companyId, campaignId, transferId, agentId, transferType);
            }

            return default(int);
        }

        public delegate int TransferSetTargetInt32Int64StringTargetTypeStringBooleanDelegate(int companyId, long campaignId, string transferId, TargetType targetType, string targetResource, bool borrowAgentsFromAllCampaigns);
        public TransferSetTargetInt32Int64StringTargetTypeStringBooleanDelegate TransferSetTargetInt32Int64StringTargetTypeStringBoolean;

        int IDialerAPI.TransferSetTarget(int companyId, long campaignId, string transferId, TargetType targetType, string targetResource, bool borrowAgentsFromAllCampaigns)
        {


            if (TransferSetTargetInt32Int64StringTargetTypeStringBoolean != null)
            {
                return TransferSetTargetInt32Int64StringTargetTypeStringBoolean(companyId, campaignId, transferId, targetType, targetResource, borrowAgentsFromAllCampaigns);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).TransferSetTarget(companyId, campaignId, transferId, targetType, targetResource, borrowAgentsFromAllCampaigns);
            }

            return default(int);
        }

        public delegate int TransferSetConnectionStateInt32Int64StringConnectionStateDelegate(int companyId, long campaignId, string transferId, ConnectionState state);
        public TransferSetConnectionStateInt32Int64StringConnectionStateDelegate TransferSetConnectionStateInt32Int64StringConnectionState;

        int IDialerAPI.TransferSetConnectionState(int companyId, long campaignId, string transferId, ConnectionState state)
        {


            if (TransferSetConnectionStateInt32Int64StringConnectionState != null)
            {
                return TransferSetConnectionStateInt32Int64StringConnectionState(companyId, campaignId, transferId, state);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).TransferSetConnectionState(companyId, campaignId, transferId, state);
            }

            return default(int);
        }

        public delegate int TransferCompleteInt32Int64StringDelegate(int companyId, long campaignId, string transferId);
        public TransferCompleteInt32Int64StringDelegate TransferCompleteInt32Int64String;

        int IDialerAPI.TransferComplete(int companyId, long campaignId, string transferId)
        {


            if (TransferCompleteInt32Int64String != null)
            {
                return TransferCompleteInt32Int64String(companyId, campaignId, transferId);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).TransferComplete(companyId, campaignId, transferId);
            }

            return default(int);
        }

        public delegate int TransferCancelInt32Int64StringDelegate(int companyId, long campaignId, string transferId);
        public TransferCancelInt32Int64StringDelegate TransferCancelInt32Int64String;

        int IDialerAPI.TransferCancel(int companyId, long campaignId, string transferId)
        {


            if (TransferCancelInt32Int64String != null)
            {
                return TransferCancelInt32Int64String(companyId, campaignId, transferId);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).TransferCancel(companyId, campaignId, transferId);
            }

            return default(int);
        }

        public delegate int RegisterAgentSoftphoneInt32Int32Int32StringStringOutStringOutStringOutStringOutStringOutDelegate(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl);
        public RegisterAgentSoftphoneInt32Int32Int32StringStringOutStringOutStringOutStringOutStringOutDelegate RegisterAgentSoftphoneInt32Int32Int32StringStringOutStringOutStringOutStringOutStringOut;

        int IDialerAPI.RegisterAgentSoftphone(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl)
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
                return ((IDialerAPI)_inner).RegisterAgentSoftphone(companyId, dialerId, agentId, agentName, out login, out password, out host, out extension, out frontendUrl);
            }

            return default(int);
        }

        public delegate IEnumerable<LogFileInfo> GetLogFilesDelegate();
        public GetLogFilesDelegate GetLogFiles;

        IEnumerable<LogFileInfo> IDialerAPI.GetLogFiles()
        {


            if (GetLogFiles != null)
            {
                return GetLogFiles();
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).GetLogFiles();
            }

            return default(IEnumerable<LogFileInfo>);
        }

        public delegate byte[] GetLogFileBodyZippedStringDelegate(string fileName);
        public GetLogFileBodyZippedStringDelegate GetLogFileBodyZippedString;

        byte[] IDialerAPI.GetLogFileBodyZipped(string fileName)
        {


            if (GetLogFileBodyZippedString != null)
            {
                return GetLogFileBodyZippedString(fileName);
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).GetLogFileBodyZipped(fileName);
            }

            return default(byte[]);
        }

        public delegate string GetDialerVersionDelegate();
        public GetDialerVersionDelegate GetDialerVersion;

        string IDialerAPI.GetDialerVersion()
        {


            if (GetDialerVersion != null)
            {
                return GetDialerVersion();
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).GetDialerVersion();
            }

            return default(string);
        }

        public delegate CodiVersionInfoCommon GetCodiVersionInfoDelegate();
        public GetCodiVersionInfoDelegate GetCodiVersionInfo;

        CodiVersionInfoCommon IDialerAPI.GetCodiVersionInfo()
        {


            if (GetCodiVersionInfo != null)
            {
                return GetCodiVersionInfo();
            } else if (_inner != null)
            {
                return ((IDialerAPI)_inner).GetCodiVersionInfo();
            }

            return default(CodiVersionInfoCommon);
        }

        private bool _IsHangUpSupported;
        public Func<bool> IsHangUpSupportedGet;
        public Action<bool> IsHangUpSupportedSetBoolean;

        bool IDialerAPI.IsHangUpSupported
        {
            get
            {
                if (IsHangUpSupportedGet != null)
                {
                    return IsHangUpSupportedGet();
                } else if (_inner != null)
                {
                    return ((IDialerAPI)_inner).IsHangUpSupported;
                }

                if (IsHangUpSupportedSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsHangUpSupported;
                }

                return default(bool);
            }

        }

        private bool _IsPauseOrResumePlaybackSupported;
        public Func<bool> IsPauseOrResumePlaybackSupportedGet;
        public Action<bool> IsPauseOrResumePlaybackSupportedSetBoolean;

        bool IDialerAPI.IsPauseOrResumePlaybackSupported
        {
            get
            {
                if (IsPauseOrResumePlaybackSupportedGet != null)
                {
                    return IsPauseOrResumePlaybackSupportedGet();
                } else if (_inner != null)
                {
                    return ((IDialerAPI)_inner).IsPauseOrResumePlaybackSupported;
                }

                if (IsPauseOrResumePlaybackSupportedSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsPauseOrResumePlaybackSupported;
                }

                return default(bool);
            }

        }

        private bool _IsToggleInterviewerListensToPlaybackOrRespondentSupported;
        public Func<bool> IsToggleInterviewerListensToPlaybackOrRespondentSupportedGet;
        public Action<bool> IsToggleInterviewerListensToPlaybackOrRespondentSupportedSetBoolean;

        bool IDialerAPI.IsToggleInterviewerListensToPlaybackOrRespondentSupported
        {
            get
            {
                if (IsToggleInterviewerListensToPlaybackOrRespondentSupportedGet != null)
                {
                    return IsToggleInterviewerListensToPlaybackOrRespondentSupportedGet();
                } else if (_inner != null)
                {
                    return ((IDialerAPI)_inner).IsToggleInterviewerListensToPlaybackOrRespondentSupported;
                }

                if (IsToggleInterviewerListensToPlaybackOrRespondentSupportedSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsToggleInterviewerListensToPlaybackOrRespondentSupported;
                }

                return default(bool);
            }

        }

    }
}