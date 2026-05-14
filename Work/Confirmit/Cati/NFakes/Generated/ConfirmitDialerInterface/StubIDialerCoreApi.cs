using System;
using ConfirmitDialerInterface;
using System.Collections.Generic;

namespace ConfirmitDialerInterface.Fakes
{
    public class StubIDialerCoreApi : IDialerCoreApi 
    {
        private IDialerCoreApi _inner;

        public StubIDialerCoreApi()
        {
            _inner = null;
        }

        public IDialerCoreApi Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetNameDelegate();
        public GetNameDelegate GetName;

        string IDialerCoreApi.GetName()
        {


            if (GetName != null)
            {
                return GetName();
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).GetName();
            }

            return default(string);
        }

        public delegate string GetVersionDelegate();
        public GetVersionDelegate GetVersion;

        string IDialerCoreApi.GetVersion()
        {


            if (GetVersion != null)
            {
                return GetVersion();
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).GetVersion();
            }

            return default(string);
        }

        public delegate DialerErrorCode InitializeInt32Int32StringDelegate(int companyId, int dialerId, string configurationParametersXml);
        public InitializeInt32Int32StringDelegate InitializeInt32Int32String;

        DialerErrorCode IDialerCoreApi.Initialize(int companyId, int dialerId, string configurationParametersXml)
        {


            if (InitializeInt32Int32String != null)
            {
                return InitializeInt32Int32String(companyId, dialerId, configurationParametersXml);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).Initialize(companyId, dialerId, configurationParametersXml);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode ReleaseInt32Int32Delegate(int dialerId, int companyId);
        public ReleaseInt32Int32Delegate ReleaseInt32Int32;

        DialerErrorCode IDialerCoreApi.Release(int dialerId, int companyId)
        {


            if (ReleaseInt32Int32 != null)
            {
                return ReleaseInt32Int32(dialerId, companyId);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).Release(dialerId, companyId);
            }

            return default(DialerErrorCode);
        }

        public delegate IDialerFeatures GetFeaturesInt32Int32Delegate(int companyId, int dialerId);
        public GetFeaturesInt32Int32Delegate GetFeaturesInt32Int32;

        IDialerFeatures IDialerCoreApi.GetFeatures(int companyId, int dialerId)
        {


            if (GetFeaturesInt32Int32 != null)
            {
                return GetFeaturesInt32Int32(companyId, dialerId);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).GetFeatures(companyId, dialerId);
            }

            return default(IDialerFeatures);
        }

        public delegate DialerErrorCode RestoreDialerDriverStateInt32StringDelegate(int companyId, string filename);
        public RestoreDialerDriverStateInt32StringDelegate RestoreDialerDriverStateInt32String;

        DialerErrorCode IDialerCoreApi.RestoreDialerDriverState(int companyId, string filename)
        {


            if (RestoreDialerDriverStateInt32String != null)
            {
                return RestoreDialerDriverStateInt32String(companyId, filename);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).RestoreDialerDriverState(companyId, filename);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SaveDialerDriverStateStringDelegate(string filename);
        public SaveDialerDriverStateStringDelegate SaveDialerDriverStateString;

        DialerErrorCode IDialerCoreApi.SaveDialerDriverState(string filename)
        {


            if (SaveDialerDriverStateString != null)
            {
                return SaveDialerDriverStateString(filename);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).SaveDialerDriverState(filename);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SetConfigurationParametersInt32StringDelegate(int companyId, string configurationParametersXml);
        public SetConfigurationParametersInt32StringDelegate SetConfigurationParametersInt32String;

        DialerErrorCode IDialerCoreApi.SetConfigurationParameters(int companyId, string configurationParametersXml)
        {


            if (SetConfigurationParametersInt32String != null)
            {
                return SetConfigurationParametersInt32String(companyId, configurationParametersXml);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).SetConfigurationParameters(companyId, configurationParametersXml);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerState GetStateInt32Int32Delegate(int companyId, int dialerId);
        public GetStateInt32Int32Delegate GetStateInt32Int32;

        DialerState IDialerCoreApi.GetState(int companyId, int dialerId)
        {


            if (GetStateInt32Int32 != null)
            {
                return GetStateInt32Int32(companyId, dialerId);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).GetState(companyId, dialerId);
            }

            return default(DialerState);
        }

        public delegate DialerErrorCode StartCampaignInt32ArrayOfInt32Int64StringDialingModeBooleanStringDelegate(int companyId, int[] dialerIds, long campaignId, string campaignName, DialingMode dialingMode, bool recordWholeInterview, string campaignParametersXml);
        public StartCampaignInt32ArrayOfInt32Int64StringDialingModeBooleanStringDelegate StartCampaignInt32ArrayOfInt32Int64StringDialingModeBooleanString;

        DialerErrorCode IDialerCoreApi.StartCampaign(int companyId, int[] dialerIds, long campaignId, string campaignName, DialingMode dialingMode, bool recordWholeInterview, string campaignParametersXml)
        {


            if (StartCampaignInt32ArrayOfInt32Int64StringDialingModeBooleanString != null)
            {
                return StartCampaignInt32ArrayOfInt32Int64StringDialingModeBooleanString(companyId, dialerIds, campaignId, campaignName, dialingMode, recordWholeInterview, campaignParametersXml);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).StartCampaign(companyId, dialerIds, campaignId, campaignName, dialingMode, recordWholeInterview, campaignParametersXml);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode StopCampaignInt32ArrayOfInt32Int64DialingModeDelegate(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode);
        public StopCampaignInt32ArrayOfInt32Int64DialingModeDelegate StopCampaignInt32ArrayOfInt32Int64DialingMode;

        DialerErrorCode IDialerCoreApi.StopCampaign(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {


            if (StopCampaignInt32ArrayOfInt32Int64DialingMode != null)
            {
                return StopCampaignInt32ArrayOfInt32Int64DialingMode(companyId, dialerIds, campaignId, dialingMode);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).StopCampaign(companyId, dialerIds, campaignId, dialingMode);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode KillCampaignInt32ArrayOfInt32Int64DialingModeDelegate(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode);
        public KillCampaignInt32ArrayOfInt32Int64DialingModeDelegate KillCampaignInt32ArrayOfInt32Int64DialingMode;

        DialerErrorCode IDialerCoreApi.KillCampaign(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {


            if (KillCampaignInt32ArrayOfInt32Int64DialingMode != null)
            {
                return KillCampaignInt32ArrayOfInt32Int64DialingMode(companyId, dialerIds, campaignId, dialingMode);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).KillCampaign(companyId, dialerIds, campaignId, dialingMode);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SetCampaignParametersInt32ArrayOfInt32Int64DialingModeBooleanStringDelegate(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode, bool recordWholeInterview, string campaignParametersXml);
        public SetCampaignParametersInt32ArrayOfInt32Int64DialingModeBooleanStringDelegate SetCampaignParametersInt32ArrayOfInt32Int64DialingModeBooleanString;

        DialerErrorCode IDialerCoreApi.SetCampaignParameters(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode, bool recordWholeInterview, string campaignParametersXml)
        {


            if (SetCampaignParametersInt32ArrayOfInt32Int64DialingModeBooleanString != null)
            {
                return SetCampaignParametersInt32ArrayOfInt32Int64DialingModeBooleanString(companyId, dialerIds, campaignId, dialingMode, recordWholeInterview, campaignParametersXml);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).SetCampaignParameters(companyId, dialerIds, campaignId, dialingMode, recordWholeInterview, campaignParametersXml);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode LoginInt32Int32Int64Int32StringAgentTypeStringResourceBindingTypeBooleanIEnumerableOfKeyValuePairOfStringStringDelegate(int companyId, int dialerId, long campaignId, int agentId, string agentName, AgentType agentType, string agentConnectionString, ResourceBindingType resourceBindingType, bool isPredictive, IEnumerable<KeyValuePair<string, string>> agentAttributes);
        public LoginInt32Int32Int64Int32StringAgentTypeStringResourceBindingTypeBooleanIEnumerableOfKeyValuePairOfStringStringDelegate LoginInt32Int32Int64Int32StringAgentTypeStringResourceBindingTypeBooleanIEnumerableOfKeyValuePairOfStringString;

        DialerErrorCode IDialerCoreApi.Login(int companyId, int dialerId, long campaignId, int agentId, string agentName, AgentType agentType, string agentConnectionString, ResourceBindingType resourceBindingType, bool isPredictive, IEnumerable<KeyValuePair<string, string>> agentAttributes)
        {


            if (LoginInt32Int32Int64Int32StringAgentTypeStringResourceBindingTypeBooleanIEnumerableOfKeyValuePairOfStringString != null)
            {
                return LoginInt32Int32Int64Int32StringAgentTypeStringResourceBindingTypeBooleanIEnumerableOfKeyValuePairOfStringString(companyId, dialerId, campaignId, agentId, agentName, agentType, agentConnectionString, resourceBindingType, isPredictive, agentAttributes);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).Login(companyId, dialerId, campaignId, agentId, agentName, agentType, agentConnectionString, resourceBindingType, isPredictive, agentAttributes);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SetCampaignInt32Int32Int64Int32Delegate(int companyId, int dialerId, long campaignId, int agentId);
        public SetCampaignInt32Int32Int64Int32Delegate SetCampaignInt32Int32Int64Int32;

        DialerErrorCode IDialerCoreApi.SetCampaign(int companyId, int dialerId, long campaignId, int agentId)
        {


            if (SetCampaignInt32Int32Int64Int32 != null)
            {
                return SetCampaignInt32Int32Int64Int32(companyId, dialerId, campaignId, agentId);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).SetCampaign(companyId, dialerId, campaignId, agentId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode LogoutInt32Int32Int64Int32BooleanDelegate(int companyId, int dialerId, long campaignId, int agentId, bool isPredictive);
        public LogoutInt32Int32Int64Int32BooleanDelegate LogoutInt32Int32Int64Int32Boolean;

        DialerErrorCode IDialerCoreApi.Logout(int companyId, int dialerId, long campaignId, int agentId, bool isPredictive)
        {


            if (LogoutInt32Int32Int64Int32Boolean != null)
            {
                return LogoutInt32Int32Int64Int32Boolean(companyId, dialerId, campaignId, agentId, isPredictive);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).Logout(companyId, dialerId, campaignId, agentId, isPredictive);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode KillAgentInt32Int32Int64Int32Delegate(int companyId, int dialerId, long campaignId, int agentId);
        public KillAgentInt32Int32Int64Int32Delegate KillAgentInt32Int32Int64Int32;

        DialerErrorCode IDialerCoreApi.KillAgent(int companyId, int dialerId, long campaignId, int agentId)
        {


            if (KillAgentInt32Int32Int64Int32 != null)
            {
                return KillAgentInt32Int32Int64Int32(companyId, dialerId, campaignId, agentId);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).KillAgent(companyId, dialerId, campaignId, agentId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode GoReadyInt32Int32Int64Int32Delegate(int companyId, int dialerId, long campaignId, int agentId);
        public GoReadyInt32Int32Int64Int32Delegate GoReadyInt32Int32Int64Int32;

        DialerErrorCode IDialerCoreApi.GoReady(int companyId, int dialerId, long campaignId, int agentId)
        {


            if (GoReadyInt32Int32Int64Int32 != null)
            {
                return GoReadyInt32Int32Int64Int32(companyId, dialerId, campaignId, agentId);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).GoReady(companyId, dialerId, campaignId, agentId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode GoNotReadyInt32Int32Int64Int32StringDelegate(int companyId, int dialerId, long campaignId, int agentId, string breakName);
        public GoNotReadyInt32Int32Int64Int32StringDelegate GoNotReadyInt32Int32Int64Int32String;

        DialerErrorCode IDialerCoreApi.GoNotReady(int companyId, int dialerId, long campaignId, int agentId, string breakName)
        {


            if (GoNotReadyInt32Int32Int64Int32String != null)
            {
                return GoNotReadyInt32Int32Int64Int32String(companyId, dialerId, campaignId, agentId, breakName);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).GoNotReady(companyId, dialerId, campaignId, agentId, breakName);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SetGroupsInt32Int32Int64Int32ArrayOfInt32Delegate(int companyId, int dialerId, long campaignId, int agentId, int[] agentGroups);
        public SetGroupsInt32Int32Int64Int32ArrayOfInt32Delegate SetGroupsInt32Int32Int64Int32ArrayOfInt32;

        DialerErrorCode IDialerCoreApi.SetGroups(int companyId, int dialerId, long campaignId, int agentId, int[] agentGroups)
        {


            if (SetGroupsInt32Int32Int64Int32ArrayOfInt32 != null)
            {
                return SetGroupsInt32Int32Int64Int32ArrayOfInt32(companyId, dialerId, campaignId, agentId, agentGroups);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).SetGroups(companyId, dialerId, campaignId, agentId, agentGroups);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SendNumberToAgentInt32Int32Int64Int32DialingModeInt32Int64StringBooleanStringDictionaryOfStringObjectDelegate(int companyId, int dialerId, long campaignId, int agentId, DialingMode diallingMode, int interviewId, long callId, string phoneNumber, bool isRecording, string callerId, Dictionary<string, Object> respondentVariables);
        public SendNumberToAgentInt32Int32Int64Int32DialingModeInt32Int64StringBooleanStringDictionaryOfStringObjectDelegate SendNumberToAgentInt32Int32Int64Int32DialingModeInt32Int64StringBooleanStringDictionaryOfStringObject;

        DialerErrorCode IDialerCoreApi.SendNumberToAgent(int companyId, int dialerId, long campaignId, int agentId, DialingMode diallingMode, int interviewId, long callId, string phoneNumber, bool isRecording, string callerId, Dictionary<string, Object> respondentVariables)
        {


            if (SendNumberToAgentInt32Int32Int64Int32DialingModeInt32Int64StringBooleanStringDictionaryOfStringObject != null)
            {
                return SendNumberToAgentInt32Int32Int64Int32DialingModeInt32Int64StringBooleanStringDictionaryOfStringObject(companyId, dialerId, campaignId, agentId, diallingMode, interviewId, callId, phoneNumber, isRecording, callerId, respondentVariables);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).SendNumberToAgent(companyId, dialerId, campaignId, agentId, diallingMode, interviewId, callId, phoneNumber, isRecording, callerId, respondentVariables);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode RedialInt32Int32Int64Int32Int32Int64StringBooleanStringDelegate(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, string phoneNumber, bool isRecording, string callerId);
        public RedialInt32Int32Int64Int32Int32Int64StringBooleanStringDelegate RedialInt32Int32Int64Int32Int32Int64StringBooleanString;

        DialerErrorCode IDialerCoreApi.Redial(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, string phoneNumber, bool isRecording, string callerId)
        {


            if (RedialInt32Int32Int64Int32Int32Int64StringBooleanString != null)
            {
                return RedialInt32Int32Int64Int32Int32Int64StringBooleanString(companyId, dialerId, campaignId, agentId, interviewId, callId, phoneNumber, isRecording, callerId);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).Redial(companyId, dialerId, campaignId, agentId, interviewId, callId, phoneNumber, isRecording, callerId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SendNumbersStringInt32Int32Int64DialingModeListOfCallInfoInt32Delegate(string requestId, int companyId, int dialerId, long campaignId, DialingMode campaignDialingMode, List<CallInfo> callList, int callAgingTimeout);
        public SendNumbersStringInt32Int32Int64DialingModeListOfCallInfoInt32Delegate SendNumbersStringInt32Int32Int64DialingModeListOfCallInfoInt32;

        DialerErrorCode IDialerCoreApi.SendNumbers(string requestId, int companyId, int dialerId, long campaignId, DialingMode campaignDialingMode, List<CallInfo> callList, int callAgingTimeout)
        {


            if (SendNumbersStringInt32Int32Int64DialingModeListOfCallInfoInt32 != null)
            {
                return SendNumbersStringInt32Int32Int64DialingModeListOfCallInfoInt32(requestId, companyId, dialerId, campaignId, campaignDialingMode, callList, callAgingTimeout);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).SendNumbers(requestId, companyId, dialerId, campaignId, campaignDialingMode, callList, callAgingTimeout);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode HangupInt32Int32Int64Int32Int32Int64Delegate(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId);
        public HangupInt32Int32Int64Int32Int32Int64Delegate HangupInt32Int32Int64Int32Int32Int64;

        DialerErrorCode IDialerCoreApi.Hangup(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId)
        {


            if (HangupInt32Int32Int64Int32Int32Int64 != null)
            {
                return HangupInt32Int32Int64Int32Int32Int64(companyId, dialerId, campaignId, agentId, interviewId, callId);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).Hangup(companyId, dialerId, campaignId, agentId, interviewId, callId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode CompleteCallInt32Int32Int64Int32InterviewStatusBooleanStringInt32Int64Delegate(int companyId, int dialerId, long campaignId, int agentId, InterviewStatus interviewStatus, bool makeAgentReady, string breakName, int interviewId, long callId);
        public CompleteCallInt32Int32Int64Int32InterviewStatusBooleanStringInt32Int64Delegate CompleteCallInt32Int32Int64Int32InterviewStatusBooleanStringInt32Int64;

        DialerErrorCode IDialerCoreApi.CompleteCall(int companyId, int dialerId, long campaignId, int agentId, InterviewStatus interviewStatus, bool makeAgentReady, string breakName, int interviewId, long callId)
        {


            if (CompleteCallInt32Int32Int64Int32InterviewStatusBooleanStringInt32Int64 != null)
            {
                return CompleteCallInt32Int32Int64Int32InterviewStatusBooleanStringInt32Int64(companyId, dialerId, campaignId, agentId, interviewStatus, makeAgentReady, breakName, interviewId, callId);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).CompleteCall(companyId, dialerId, campaignId, agentId, interviewStatus, makeAgentReady, breakName, interviewId, callId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SetNextInterviewInt32Int32Int64Int32InterviewStatusInt64Int32Int64Delegate(int companyId, int dialerId, long currentCampaignId, int agentId, InterviewStatus currentInterviewStatus, long nextCampaignId, int nextInterviewId, long nextCallId);
        public SetNextInterviewInt32Int32Int64Int32InterviewStatusInt64Int32Int64Delegate SetNextInterviewInt32Int32Int64Int32InterviewStatusInt64Int32Int64;

        DialerErrorCode IDialerCoreApi.SetNextInterview(int companyId, int dialerId, long currentCampaignId, int agentId, InterviewStatus currentInterviewStatus, long nextCampaignId, int nextInterviewId, long nextCallId)
        {


            if (SetNextInterviewInt32Int32Int64Int32InterviewStatusInt64Int32Int64 != null)
            {
                return SetNextInterviewInt32Int32Int64Int32InterviewStatusInt64Int32Int64(companyId, dialerId, currentCampaignId, agentId, currentInterviewStatus, nextCampaignId, nextInterviewId, nextCallId);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).SetNextInterview(companyId, dialerId, currentCampaignId, agentId, currentInterviewStatus, nextCampaignId, nextInterviewId, nextCallId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode StartCustomIvrInterviewInt32Int32Int64Int32Int32Int64StringDelegate(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, string respondentSurveyLink);
        public StartCustomIvrInterviewInt32Int32Int64Int32Int32Int64StringDelegate StartCustomIvrInterviewInt32Int32Int64Int32Int32Int64String;

        DialerErrorCode IDialerCoreApi.StartCustomIvrInterview(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, string respondentSurveyLink)
        {


            if (StartCustomIvrInterviewInt32Int32Int64Int32Int32Int64String != null)
            {
                return StartCustomIvrInterviewInt32Int32Int64Int32Int32Int64String(companyId, dialerId, campaignId, agentId, interviewId, callId, respondentSurveyLink);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).StartCustomIvrInterview(companyId, dialerId, campaignId, agentId, interviewId, callId, respondentSurveyLink);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode CompletePreviewInt32Int32Int64Int32Int32Int64StringBooleanDelegate(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, string phoneNumber, bool isRecording);
        public CompletePreviewInt32Int32Int64Int32Int32Int64StringBooleanDelegate CompletePreviewInt32Int32Int64Int32Int32Int64StringBoolean;

        DialerErrorCode IDialerCoreApi.CompletePreview(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, string phoneNumber, bool isRecording)
        {


            if (CompletePreviewInt32Int32Int64Int32Int32Int64StringBoolean != null)
            {
                return CompletePreviewInt32Int32Int64Int32Int32Int64StringBoolean(companyId, dialerId, campaignId, agentId, interviewId, callId, phoneNumber, isRecording);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).CompletePreview(companyId, dialerId, campaignId, agentId, interviewId, callId, phoneNumber, isRecording);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode FlushNumbersInt32ArrayOfInt32Int64ListOfCallInfoDelegate(int companyId, int[] dialerIds, long campaignId, List<CallInfo> callList);
        public FlushNumbersInt32ArrayOfInt32Int64ListOfCallInfoDelegate FlushNumbersInt32ArrayOfInt32Int64ListOfCallInfo;

        DialerErrorCode IDialerCoreApi.FlushNumbers(int companyId, int[] dialerIds, long campaignId, List<CallInfo> callList)
        {


            if (FlushNumbersInt32ArrayOfInt32Int64ListOfCallInfo != null)
            {
                return FlushNumbersInt32ArrayOfInt32Int64ListOfCallInfo(companyId, dialerIds, campaignId, callList);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).FlushNumbers(companyId, dialerIds, campaignId, callList);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode StartRecordingInt32Int32Int64Int32Int32Int64StringDelegate(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, string label);
        public StartRecordingInt32Int32Int64Int32Int32Int64StringDelegate StartRecordingInt32Int32Int64Int32Int32Int64String;

        DialerErrorCode IDialerCoreApi.StartRecording(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, string label)
        {


            if (StartRecordingInt32Int32Int64Int32Int32Int64String != null)
            {
                return StartRecordingInt32Int32Int64Int32Int32Int64String(companyId, dialerId, campaignId, agentId, interviewId, callId, label);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).StartRecording(companyId, dialerId, campaignId, agentId, interviewId, callId, label);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode StopRecordingInt32Int32Int64Int32Int32Int64StopRecordingModeDelegate(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, StopRecordingMode stopRecordingMode);
        public StopRecordingInt32Int32Int64Int32Int32Int64StopRecordingModeDelegate StopRecordingInt32Int32Int64Int32Int32Int64StopRecordingMode;

        DialerErrorCode IDialerCoreApi.StopRecording(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, StopRecordingMode stopRecordingMode)
        {


            if (StopRecordingInt32Int32Int64Int32Int32Int64StopRecordingMode != null)
            {
                return StopRecordingInt32Int32Int64Int32Int32Int64StopRecordingMode(companyId, dialerId, campaignId, agentId, interviewId, callId, stopRecordingMode);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).StopRecording(companyId, dialerId, campaignId, agentId, interviewId, callId, stopRecordingMode);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode StartPlaybackInt32Int32Int64Int32Int32Int64StringInt32OutDelegate(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, string fileName, out int timeOfPlayingInSeconds);
        public StartPlaybackInt32Int32Int64Int32Int32Int64StringInt32OutDelegate StartPlaybackInt32Int32Int64Int32Int32Int64StringInt32Out;

        DialerErrorCode IDialerCoreApi.StartPlayback(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, string fileName, out int timeOfPlayingInSeconds)
        {
            timeOfPlayingInSeconds = default(int);


            if (StartPlaybackInt32Int32Int64Int32Int32Int64StringInt32Out != null)
            {
                return StartPlaybackInt32Int32Int64Int32Int32Int64StringInt32Out(companyId, dialerId, campaignId, agentId, interviewId, callId, fileName, out timeOfPlayingInSeconds);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).StartPlayback(companyId, dialerId, campaignId, agentId, interviewId, callId, fileName, out timeOfPlayingInSeconds);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode StopPlaybackInt32Int32Int64Int32Int64Delegate(int companyId, int dialerId, long campaignId, int agentId, long callId);
        public StopPlaybackInt32Int32Int64Int32Int64Delegate StopPlaybackInt32Int32Int64Int32Int64;

        DialerErrorCode IDialerCoreApi.StopPlayback(int companyId, int dialerId, long campaignId, int agentId, long callId)
        {


            if (StopPlaybackInt32Int32Int64Int32Int64 != null)
            {
                return StopPlaybackInt32Int32Int64Int32Int64(companyId, dialerId, campaignId, agentId, callId);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).StopPlayback(companyId, dialerId, campaignId, agentId, callId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode PauseOrResumePlaybackInt32Int32Int64Int32Int64Delegate(int companyId, int dialerId, long campaignId, int agentId, long callId);
        public PauseOrResumePlaybackInt32Int32Int64Int32Int64Delegate PauseOrResumePlaybackInt32Int32Int64Int32Int64;

        DialerErrorCode IDialerCoreApi.PauseOrResumePlayback(int companyId, int dialerId, long campaignId, int agentId, long callId)
        {


            if (PauseOrResumePlaybackInt32Int32Int64Int32Int64 != null)
            {
                return PauseOrResumePlaybackInt32Int32Int64Int32Int64(companyId, dialerId, campaignId, agentId, callId);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).PauseOrResumePlayback(companyId, dialerId, campaignId, agentId, callId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode ToggleInterviewerListensToPlaybackOrRespondentInt32Int32Int64Int32Int64Delegate(int companyId, int dialerId, long campaignId, int agentId, long callId);
        public ToggleInterviewerListensToPlaybackOrRespondentInt32Int32Int64Int32Int64Delegate ToggleInterviewerListensToPlaybackOrRespondentInt32Int32Int64Int32Int64;

        DialerErrorCode IDialerCoreApi.ToggleInterviewerListensToPlaybackOrRespondent(int companyId, int dialerId, long campaignId, int agentId, long callId)
        {


            if (ToggleInterviewerListensToPlaybackOrRespondentInt32Int32Int64Int32Int64 != null)
            {
                return ToggleInterviewerListensToPlaybackOrRespondentInt32Int32Int64Int32Int64(companyId, dialerId, campaignId, agentId, callId);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).ToggleInterviewerListensToPlaybackOrRespondent(companyId, dialerId, campaignId, agentId, callId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode StartMonitorInt32Int32Int32StringStringResourceBindingTypeStringRefDelegate(int companyId, int dialerId, int agentId, string supervisorName, string supervisorConnectionString, ResourceBindingType resourceBindingType, ref string sessionId);
        public StartMonitorInt32Int32Int32StringStringResourceBindingTypeStringRefDelegate StartMonitorInt32Int32Int32StringStringResourceBindingTypeStringRef;

        DialerErrorCode IDialerCoreApi.StartMonitor(int companyId, int dialerId, int agentId, string supervisorName, string supervisorConnectionString, ResourceBindingType resourceBindingType, ref string sessionId)
        {


            if (StartMonitorInt32Int32Int32StringStringResourceBindingTypeStringRef != null)
            {
                return StartMonitorInt32Int32Int32StringStringResourceBindingTypeStringRef(companyId, dialerId, agentId, supervisorName, supervisorConnectionString, resourceBindingType, ref sessionId);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).StartMonitor(companyId, dialerId, agentId, supervisorName, supervisorConnectionString, resourceBindingType, ref sessionId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode StopMonitorInt32Int32StringDelegate(int companyId, int dialerId, string sessionId);
        public StopMonitorInt32Int32StringDelegate StopMonitorInt32Int32String;

        DialerErrorCode IDialerCoreApi.StopMonitor(int companyId, int dialerId, string sessionId)
        {


            if (StopMonitorInt32Int32String != null)
            {
                return StopMonitorInt32Int32String(companyId, dialerId, sessionId);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).StopMonitor(companyId, dialerId, sessionId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SetMonitorModeInt32Int32StringMonitorModeDelegate(int companyId, int dialerId, string sessionId, MonitorMode monitorMode);
        public SetMonitorModeInt32Int32StringMonitorModeDelegate SetMonitorModeInt32Int32StringMonitorMode;

        DialerErrorCode IDialerCoreApi.SetMonitorMode(int companyId, int dialerId, string sessionId, MonitorMode monitorMode)
        {


            if (SetMonitorModeInt32Int32StringMonitorMode != null)
            {
                return SetMonitorModeInt32Int32StringMonitorMode(companyId, dialerId, sessionId, monitorMode);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).SetMonitorMode(companyId, dialerId, sessionId, monitorMode);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode GetTrunkLineStatesAndAlarmsInt32Int32IEnumerableOfTrunkLineStateAndAlarmsOutDelegate(int companyId, int dialerId, out IEnumerable<TrunkLineStateAndAlarms> trunkLineStatesAndAlarms);
        public GetTrunkLineStatesAndAlarmsInt32Int32IEnumerableOfTrunkLineStateAndAlarmsOutDelegate GetTrunkLineStatesAndAlarmsInt32Int32IEnumerableOfTrunkLineStateAndAlarmsOut;

        DialerErrorCode IDialerCoreApi.GetTrunkLineStatesAndAlarms(int companyId, int dialerId, out IEnumerable<TrunkLineStateAndAlarms> trunkLineStatesAndAlarms)
        {
            trunkLineStatesAndAlarms = default(IEnumerable<TrunkLineStateAndAlarms>);


            if (GetTrunkLineStatesAndAlarmsInt32Int32IEnumerableOfTrunkLineStateAndAlarmsOut != null)
            {
                return GetTrunkLineStatesAndAlarmsInt32Int32IEnumerableOfTrunkLineStateAndAlarmsOut(companyId, dialerId, out trunkLineStatesAndAlarms);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).GetTrunkLineStatesAndAlarms(companyId, dialerId, out trunkLineStatesAndAlarms);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode TransferToIvrInt32Int32Int64Int32Int32Int64StringIEnumerableOfKeyValuePairOfStringStringDelegate(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, string endpoint, IEnumerable<KeyValuePair<string, string>> attributes);
        public TransferToIvrInt32Int32Int64Int32Int32Int64StringIEnumerableOfKeyValuePairOfStringStringDelegate TransferToIvrInt32Int32Int64Int32Int32Int64StringIEnumerableOfKeyValuePairOfStringString;

        DialerErrorCode IDialerCoreApi.TransferToIvr(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, string endpoint, IEnumerable<KeyValuePair<string, string>> attributes)
        {


            if (TransferToIvrInt32Int32Int64Int32Int32Int64StringIEnumerableOfKeyValuePairOfStringString != null)
            {
                return TransferToIvrInt32Int32Int64Int32Int32Int64StringIEnumerableOfKeyValuePairOfStringString(companyId, dialerId, campaignId, agentId, interviewId, callId, endpoint, attributes);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).TransferToIvr(companyId, dialerId, campaignId, agentId, interviewId, callId, endpoint, attributes);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode IvrRenderVoiceXmlInt32Int32Int64Int32StringDelegate(int companyId, int dialerId, long campaignId, int agentId, string voiceXml);
        public IvrRenderVoiceXmlInt32Int32Int64Int32StringDelegate IvrRenderVoiceXmlInt32Int32Int64Int32String;

        DialerErrorCode IDialerCoreApi.IvrRenderVoiceXml(int companyId, int dialerId, long campaignId, int agentId, string voiceXml)
        {


            if (IvrRenderVoiceXmlInt32Int32Int64Int32String != null)
            {
                return IvrRenderVoiceXmlInt32Int32Int64Int32String(companyId, dialerId, campaignId, agentId, voiceXml);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).IvrRenderVoiceXml(companyId, dialerId, campaignId, agentId, voiceXml);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode[] ConfigureInboundDdiNumbersInt32Int32ArrayOfInboundDdiNumberDelegate(int companyId, int dialerId, InboundDdiNumber[] inboundDdiNumbers);
        public ConfigureInboundDdiNumbersInt32Int32ArrayOfInboundDdiNumberDelegate ConfigureInboundDdiNumbersInt32Int32ArrayOfInboundDdiNumber;

        DialerErrorCode[] IDialerCoreApi.ConfigureInboundDdiNumbers(int companyId, int dialerId, InboundDdiNumber[] inboundDdiNumbers)
        {


            if (ConfigureInboundDdiNumbersInt32Int32ArrayOfInboundDdiNumber != null)
            {
                return ConfigureInboundDdiNumbersInt32Int32ArrayOfInboundDdiNumber(companyId, dialerId, inboundDdiNumbers);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).ConfigureInboundDdiNumbers(companyId, dialerId, inboundDdiNumbers);
            }

            return default(DialerErrorCode[]);
        }

        public delegate DialerErrorCode DropInboundCallInt32Int32StringAudioMessageDescriptorDelegate(int companyId, int dialerId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor);
        public DropInboundCallInt32Int32StringAudioMessageDescriptorDelegate DropInboundCallInt32Int32StringAudioMessageDescriptor;

        DialerErrorCode IDialerCoreApi.DropInboundCall(int companyId, int dialerId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor)
        {


            if (DropInboundCallInt32Int32StringAudioMessageDescriptor != null)
            {
                return DropInboundCallInt32Int32StringAudioMessageDescriptor(companyId, dialerId, inboundCallId, audioMessageDescriptor);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).DropInboundCall(companyId, dialerId, inboundCallId, audioMessageDescriptor);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode ConnectInboundCallInt32Int32Int64StringCallInfoArrayOfInt64AudioMessageDescriptorDelegate(int companyId, int dialerId, long campaignId, string inboundCallId, CallInfo callInfo, long[] campaignIdsToBorrowAgentsFrom, AudioMessageDescriptor audioMessageDescriptor);
        public ConnectInboundCallInt32Int32Int64StringCallInfoArrayOfInt64AudioMessageDescriptorDelegate ConnectInboundCallInt32Int32Int64StringCallInfoArrayOfInt64AudioMessageDescriptor;

        DialerErrorCode IDialerCoreApi.ConnectInboundCall(int companyId, int dialerId, long campaignId, string inboundCallId, CallInfo callInfo, long[] campaignIdsToBorrowAgentsFrom, AudioMessageDescriptor audioMessageDescriptor)
        {


            if (ConnectInboundCallInt32Int32Int64StringCallInfoArrayOfInt64AudioMessageDescriptor != null)
            {
                return ConnectInboundCallInt32Int32Int64StringCallInfoArrayOfInt64AudioMessageDescriptor(companyId, dialerId, campaignId, inboundCallId, callInfo, campaignIdsToBorrowAgentsFrom, audioMessageDescriptor);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).ConnectInboundCall(companyId, dialerId, campaignId, inboundCallId, callInfo, campaignIdsToBorrowAgentsFrom, audioMessageDescriptor);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode ConnectInboundCallToAgentInt32Int32Int64StringCallInfoAudioMessageDescriptorDelegate(int companyId, int dialerId, long campaignId, string inboundCallId, CallInfo callInfo, AudioMessageDescriptor audioMessageDescriptor);
        public ConnectInboundCallToAgentInt32Int32Int64StringCallInfoAudioMessageDescriptorDelegate ConnectInboundCallToAgentInt32Int32Int64StringCallInfoAudioMessageDescriptor;

        DialerErrorCode IDialerCoreApi.ConnectInboundCallToAgent(int companyId, int dialerId, long campaignId, string inboundCallId, CallInfo callInfo, AudioMessageDescriptor audioMessageDescriptor)
        {


            if (ConnectInboundCallToAgentInt32Int32Int64StringCallInfoAudioMessageDescriptor != null)
            {
                return ConnectInboundCallToAgentInt32Int32Int64StringCallInfoAudioMessageDescriptor(companyId, dialerId, campaignId, inboundCallId, callInfo, audioMessageDescriptor);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).ConnectInboundCallToAgent(companyId, dialerId, campaignId, inboundCallId, callInfo, audioMessageDescriptor);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode TransferStartInt32Int32Int64StringInt32TransferTypeDelegate(int companyId, int dialerId, long campaignId, string transferId, int agentId, TransferType transferType);
        public TransferStartInt32Int32Int64StringInt32TransferTypeDelegate TransferStartInt32Int32Int64StringInt32TransferType;

        DialerErrorCode IDialerCoreApi.TransferStart(int companyId, int dialerId, long campaignId, string transferId, int agentId, TransferType transferType)
        {


            if (TransferStartInt32Int32Int64StringInt32TransferType != null)
            {
                return TransferStartInt32Int32Int64StringInt32TransferType(companyId, dialerId, campaignId, transferId, agentId, transferType);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).TransferStart(companyId, dialerId, campaignId, transferId, agentId, transferType);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode TransferSetTargetInt32Int32Int64StringTargetTypeStringBooleanDelegate(int companyId, int dialerId, long campaignId, string transferId, TargetType targetType, string targetResource, bool borrowAgentsFromAllCampaigns);
        public TransferSetTargetInt32Int32Int64StringTargetTypeStringBooleanDelegate TransferSetTargetInt32Int32Int64StringTargetTypeStringBoolean;

        DialerErrorCode IDialerCoreApi.TransferSetTarget(int companyId, int dialerId, long campaignId, string transferId, TargetType targetType, string targetResource, bool borrowAgentsFromAllCampaigns)
        {


            if (TransferSetTargetInt32Int32Int64StringTargetTypeStringBoolean != null)
            {
                return TransferSetTargetInt32Int32Int64StringTargetTypeStringBoolean(companyId, dialerId, campaignId, transferId, targetType, targetResource, borrowAgentsFromAllCampaigns);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).TransferSetTarget(companyId, dialerId, campaignId, transferId, targetType, targetResource, borrowAgentsFromAllCampaigns);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode TransferSetConnectionStateInt32Int32Int64StringConnectionStateDelegate(int companyId, int dialerId, long campaignId, string transferId, ConnectionState state);
        public TransferSetConnectionStateInt32Int32Int64StringConnectionStateDelegate TransferSetConnectionStateInt32Int32Int64StringConnectionState;

        DialerErrorCode IDialerCoreApi.TransferSetConnectionState(int companyId, int dialerId, long campaignId, string transferId, ConnectionState state)
        {


            if (TransferSetConnectionStateInt32Int32Int64StringConnectionState != null)
            {
                return TransferSetConnectionStateInt32Int32Int64StringConnectionState(companyId, dialerId, campaignId, transferId, state);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).TransferSetConnectionState(companyId, dialerId, campaignId, transferId, state);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode TransferCompleteInt32Int32Int64StringDelegate(int companyId, int dialerId, long campaignId, string transferId);
        public TransferCompleteInt32Int32Int64StringDelegate TransferCompleteInt32Int32Int64String;

        DialerErrorCode IDialerCoreApi.TransferComplete(int companyId, int dialerId, long campaignId, string transferId)
        {


            if (TransferCompleteInt32Int32Int64String != null)
            {
                return TransferCompleteInt32Int32Int64String(companyId, dialerId, campaignId, transferId);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).TransferComplete(companyId, dialerId, campaignId, transferId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode TransferCancelInt32Int32Int64StringDelegate(int companyId, int dialerId, long campaignId, string transferId);
        public TransferCancelInt32Int32Int64StringDelegate TransferCancelInt32Int32Int64String;

        DialerErrorCode IDialerCoreApi.TransferCancel(int companyId, int dialerId, long campaignId, string transferId)
        {


            if (TransferCancelInt32Int32Int64String != null)
            {
                return TransferCancelInt32Int32Int64String(companyId, dialerId, campaignId, transferId);
            } else if (_inner != null)
            {
                return ((IDialerCoreApi)_inner).TransferCancel(companyId, dialerId, campaignId, transferId);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode RegisterAgentSoftphoneInt32Int32Int32StringStringOutStringOutStringOutStringOutStringOutDelegate(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl);
        public RegisterAgentSoftphoneInt32Int32Int32StringStringOutStringOutStringOutStringOutStringOutDelegate RegisterAgentSoftphoneInt32Int32Int32StringStringOutStringOutStringOutStringOutStringOut;

        DialerErrorCode IDialerCoreApi.RegisterAgentSoftphone(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl)
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
                return ((IDialerCoreApi)_inner).RegisterAgentSoftphone(companyId, dialerId, agentId, agentName, out login, out password, out host, out extension, out frontendUrl);
            }

            return default(DialerErrorCode);
        }

    }
}