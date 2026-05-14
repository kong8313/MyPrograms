using System;
using ConfirmitDialerInterface;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Common;
using System.Collections.Concurrent;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIDialerSettings : IDialerSettings 
    {
        private IDialerSettings _inner;

        public StubIDialerSettings()
        {
            _inner = null;
        }

        public IDialerSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate AudioMessageDescriptor GetInboundAudioMessageAudioMessageTypeDelegate(AudioMessageType audioMessageType);
        public GetInboundAudioMessageAudioMessageTypeDelegate GetInboundAudioMessageAudioMessageType;

        AudioMessageDescriptor IDialerSettings.GetInboundAudioMessage(AudioMessageType audioMessageType)
        {


            if (GetInboundAudioMessageAudioMessageType != null)
            {
                return GetInboundAudioMessageAudioMessageType(audioMessageType);
            } else if (_inner != null)
            {
                return ((IDialerSettings)_inner).GetInboundAudioMessage(audioMessageType);
            }

            return default(AudioMessageDescriptor);
        }

        public delegate string GetInboundAudioMessageSourceAudioMessageTypeDelegate(AudioMessageType audioMessageType);
        public GetInboundAudioMessageSourceAudioMessageTypeDelegate GetInboundAudioMessageSourceAudioMessageType;

        string IDialerSettings.GetInboundAudioMessageSource(AudioMessageType audioMessageType)
        {


            if (GetInboundAudioMessageSourceAudioMessageType != null)
            {
                return GetInboundAudioMessageSourceAudioMessageType(audioMessageType);
            } else if (_inner != null)
            {
                return ((IDialerSettings)_inner).GetInboundAudioMessageSource(audioMessageType);
            }

            return default(string);
        }

        public delegate int GetInboundAudioMessageRepeatCountAudioMessageTypeDelegate(AudioMessageType audioMessageType);
        public GetInboundAudioMessageRepeatCountAudioMessageTypeDelegate GetInboundAudioMessageRepeatCountAudioMessageType;

        int IDialerSettings.GetInboundAudioMessageRepeatCount(AudioMessageType audioMessageType)
        {


            if (GetInboundAudioMessageRepeatCountAudioMessageType != null)
            {
                return GetInboundAudioMessageRepeatCountAudioMessageType(audioMessageType);
            } else if (_inner != null)
            {
                return ((IDialerSettings)_inner).GetInboundAudioMessageRepeatCount(audioMessageType);
            }

            return default(int);
        }

        private int _AllCatiServicesAreStartedEstimatedTime;
        public Func<int> AllCatiServicesAreStartedEstimatedTimeGet;
        public Action<int> AllCatiServicesAreStartedEstimatedTimeSetInt32;

        int IDialerSettings.AllCatiServicesAreStartedEstimatedTime
        {
            get
            {
                if (AllCatiServicesAreStartedEstimatedTimeGet != null)
                {
                    return AllCatiServicesAreStartedEstimatedTimeGet();
                } else if (_inner != null)
                {
                    return ((IDialerSettings)_inner).AllCatiServicesAreStartedEstimatedTime;
                }

                if (AllCatiServicesAreStartedEstimatedTimeSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AllCatiServicesAreStartedEstimatedTime;
                }

                return default(int);
            }

            set
            {
                if (AllCatiServicesAreStartedEstimatedTimeSetInt32 != null)
                {
                    AllCatiServicesAreStartedEstimatedTimeSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerSettings)_inner).AllCatiServicesAreStartedEstimatedTime = value;
                    return;
                }

                if (AllCatiServicesAreStartedEstimatedTimeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AllCatiServicesAreStartedEstimatedTime = value;
                }

            }
        }

        private int _AudioRecordingsPageSize;
        public Func<int> AudioRecordingsPageSizeGet;
        public Action<int> AudioRecordingsPageSizeSetInt32;

        int IDialerSettings.AudioRecordingsPageSize
        {
            get
            {
                if (AudioRecordingsPageSizeGet != null)
                {
                    return AudioRecordingsPageSizeGet();
                } else if (_inner != null)
                {
                    return ((IDialerSettings)_inner).AudioRecordingsPageSize;
                }

                if (AudioRecordingsPageSizeSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AudioRecordingsPageSize;
                }

                return default(int);
            }

            set
            {
                if (AudioRecordingsPageSizeSetInt32 != null)
                {
                    AudioRecordingsPageSizeSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerSettings)_inner).AudioRecordingsPageSize = value;
                    return;
                }

                if (AudioRecordingsPageSizeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AudioRecordingsPageSize = value;
                }

            }
        }

        private string _DefaultSurveyParameters;
        public Func<string> DefaultSurveyParametersGet;
        public Action<string> DefaultSurveyParametersSetString;

        string IDialerSettings.DefaultSurveyParameters
        {
            get
            {
                if (DefaultSurveyParametersGet != null)
                {
                    return DefaultSurveyParametersGet();
                } else if (_inner != null)
                {
                    return ((IDialerSettings)_inner).DefaultSurveyParameters;
                }

                if (DefaultSurveyParametersSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DefaultSurveyParameters;
                }

                return default(string);
            }

            set
            {
                if (DefaultSurveyParametersSetString != null)
                {
                    DefaultSurveyParametersSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerSettings)_inner).DefaultSurveyParameters = value;
                    return;
                }

                if (DefaultSurveyParametersGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _DefaultSurveyParameters = value;
                }

            }
        }

        private int _DelayForGetAudioRecordsMs;
        public Func<int> DelayForGetAudioRecordsMsGet;
        public Action<int> DelayForGetAudioRecordsMsSetInt32;

        int IDialerSettings.DelayForGetAudioRecordsMs
        {
            get
            {
                if (DelayForGetAudioRecordsMsGet != null)
                {
                    return DelayForGetAudioRecordsMsGet();
                } else if (_inner != null)
                {
                    return ((IDialerSettings)_inner).DelayForGetAudioRecordsMs;
                }

                if (DelayForGetAudioRecordsMsSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DelayForGetAudioRecordsMs;
                }

                return default(int);
            }

            set
            {
                if (DelayForGetAudioRecordsMsSetInt32 != null)
                {
                    DelayForGetAudioRecordsMsSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerSettings)_inner).DelayForGetAudioRecordsMs = value;
                    return;
                }

                if (DelayForGetAudioRecordsMsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _DelayForGetAudioRecordsMs = value;
                }

            }
        }

        private string _DialerType;
        public Func<string> DialerTypeGet;
        public Action<string> DialerTypeSetString;

        string IDialerSettings.DialerType
        {
            get
            {
                if (DialerTypeGet != null)
                {
                    return DialerTypeGet();
                } else if (_inner != null)
                {
                    return ((IDialerSettings)_inner).DialerType;
                }

                if (DialerTypeSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DialerType;
                }

                return default(string);
            }

            set
            {
                if (DialerTypeSetString != null)
                {
                    DialerTypeSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerSettings)_inner).DialerType = value;
                    return;
                }

                if (DialerTypeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _DialerType = value;
                }

            }
        }

        private int _HealthControlCheckPeriod;
        public Func<int> HealthControlCheckPeriodGet;
        public Action<int> HealthControlCheckPeriodSetInt32;

        int IDialerSettings.HealthControlCheckPeriod
        {
            get
            {
                if (HealthControlCheckPeriodGet != null)
                {
                    return HealthControlCheckPeriodGet();
                } else if (_inner != null)
                {
                    return ((IDialerSettings)_inner).HealthControlCheckPeriod;
                }

                if (HealthControlCheckPeriodSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _HealthControlCheckPeriod;
                }

                return default(int);
            }

            set
            {
                if (HealthControlCheckPeriodSetInt32 != null)
                {
                    HealthControlCheckPeriodSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerSettings)_inner).HealthControlCheckPeriod = value;
                    return;
                }

                if (HealthControlCheckPeriodGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _HealthControlCheckPeriod = value;
                }

            }
        }

        private int _HealthControlStopWaitTime;
        public Func<int> HealthControlStopWaitTimeGet;
        public Action<int> HealthControlStopWaitTimeSetInt32;

        int IDialerSettings.HealthControlStopWaitTime
        {
            get
            {
                if (HealthControlStopWaitTimeGet != null)
                {
                    return HealthControlStopWaitTimeGet();
                } else if (_inner != null)
                {
                    return ((IDialerSettings)_inner).HealthControlStopWaitTime;
                }

                if (HealthControlStopWaitTimeSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _HealthControlStopWaitTime;
                }

                return default(int);
            }

            set
            {
                if (HealthControlStopWaitTimeSetInt32 != null)
                {
                    HealthControlStopWaitTimeSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerSettings)_inner).HealthControlStopWaitTime = value;
                    return;
                }

                if (HealthControlStopWaitTimeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _HealthControlStopWaitTime = value;
                }

            }
        }

        private int _HealthControlUnavailableTimeoutInMs;
        public Func<int> HealthControlUnavailableTimeoutInMsGet;
        public Action<int> HealthControlUnavailableTimeoutInMsSetInt32;

        int IDialerSettings.HealthControlUnavailableTimeoutInMs
        {
            get
            {
                if (HealthControlUnavailableTimeoutInMsGet != null)
                {
                    return HealthControlUnavailableTimeoutInMsGet();
                } else if (_inner != null)
                {
                    return ((IDialerSettings)_inner).HealthControlUnavailableTimeoutInMs;
                }

                if (HealthControlUnavailableTimeoutInMsSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _HealthControlUnavailableTimeoutInMs;
                }

                return default(int);
            }

            set
            {
                if (HealthControlUnavailableTimeoutInMsSetInt32 != null)
                {
                    HealthControlUnavailableTimeoutInMsSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerSettings)_inner).HealthControlUnavailableTimeoutInMs = value;
                    return;
                }

                if (HealthControlUnavailableTimeoutInMsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _HealthControlUnavailableTimeoutInMs = value;
                }

            }
        }

        private bool _IgnoreDialerIdFromStationId;
        public Func<bool> IgnoreDialerIdFromStationIdGet;
        public Action<bool> IgnoreDialerIdFromStationIdSetBoolean;

        bool IDialerSettings.IgnoreDialerIdFromStationId
        {
            get
            {
                if (IgnoreDialerIdFromStationIdGet != null)
                {
                    return IgnoreDialerIdFromStationIdGet();
                } else if (_inner != null)
                {
                    return ((IDialerSettings)_inner).IgnoreDialerIdFromStationId;
                }

                if (IgnoreDialerIdFromStationIdSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IgnoreDialerIdFromStationId;
                }

                return default(bool);
            }

            set
            {
                if (IgnoreDialerIdFromStationIdSetBoolean != null)
                {
                    IgnoreDialerIdFromStationIdSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerSettings)_inner).IgnoreDialerIdFromStationId = value;
                    return;
                }

                if (IgnoreDialerIdFromStationIdGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _IgnoreDialerIdFromStationId = value;
                }

            }
        }

        private string _InboundAudioMessagesJson;
        public Func<string> InboundAudioMessagesJsonGet;
        public Action<string> InboundAudioMessagesJsonSetString;

        string IDialerSettings.InboundAudioMessagesJson
        {
            get
            {
                if (InboundAudioMessagesJsonGet != null)
                {
                    return InboundAudioMessagesJsonGet();
                } else if (_inner != null)
                {
                    return ((IDialerSettings)_inner).InboundAudioMessagesJson;
                }

                if (InboundAudioMessagesJsonSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InboundAudioMessagesJson;
                }

                return default(string);
            }

            set
            {
                if (InboundAudioMessagesJsonSetString != null)
                {
                    InboundAudioMessagesJsonSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerSettings)_inner).InboundAudioMessagesJson = value;
                    return;
                }

                if (InboundAudioMessagesJsonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _InboundAudioMessagesJson = value;
                }

            }
        }

        private int _InterviewerPredictiveSafeBreakWaitTimeout;
        public Func<int> InterviewerPredictiveSafeBreakWaitTimeoutGet;
        public Action<int> InterviewerPredictiveSafeBreakWaitTimeoutSetInt32;

        int IDialerSettings.InterviewerPredictiveSafeBreakWaitTimeout
        {
            get
            {
                if (InterviewerPredictiveSafeBreakWaitTimeoutGet != null)
                {
                    return InterviewerPredictiveSafeBreakWaitTimeoutGet();
                } else if (_inner != null)
                {
                    return ((IDialerSettings)_inner).InterviewerPredictiveSafeBreakWaitTimeout;
                }

                if (InterviewerPredictiveSafeBreakWaitTimeoutSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InterviewerPredictiveSafeBreakWaitTimeout;
                }

                return default(int);
            }

            set
            {
                if (InterviewerPredictiveSafeBreakWaitTimeoutSetInt32 != null)
                {
                    InterviewerPredictiveSafeBreakWaitTimeoutSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerSettings)_inner).InterviewerPredictiveSafeBreakWaitTimeout = value;
                    return;
                }

                if (InterviewerPredictiveSafeBreakWaitTimeoutGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _InterviewerPredictiveSafeBreakWaitTimeout = value;
                }

            }
        }

        private bool _OpenSurveysOnDialersIndividually;
        public Func<bool> OpenSurveysOnDialersIndividuallyGet;
        public Action<bool> OpenSurveysOnDialersIndividuallySetBoolean;

        bool IDialerSettings.OpenSurveysOnDialersIndividually
        {
            get
            {
                if (OpenSurveysOnDialersIndividuallyGet != null)
                {
                    return OpenSurveysOnDialersIndividuallyGet();
                } else if (_inner != null)
                {
                    return ((IDialerSettings)_inner).OpenSurveysOnDialersIndividually;
                }

                if (OpenSurveysOnDialersIndividuallySetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _OpenSurveysOnDialersIndividually;
                }

                return default(bool);
            }

            set
            {
                if (OpenSurveysOnDialersIndividuallySetBoolean != null)
                {
                    OpenSurveysOnDialersIndividuallySetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerSettings)_inner).OpenSurveysOnDialersIndividually = value;
                    return;
                }

                if (OpenSurveysOnDialersIndividuallyGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _OpenSurveysOnDialersIndividually = value;
                }

            }
        }

        private string _RespondentVariablesToSend;
        public Func<string> RespondentVariablesToSendGet;
        public Action<string> RespondentVariablesToSendSetString;

        string IDialerSettings.RespondentVariablesToSend
        {
            get
            {
                if (RespondentVariablesToSendGet != null)
                {
                    return RespondentVariablesToSendGet();
                } else if (_inner != null)
                {
                    return ((IDialerSettings)_inner).RespondentVariablesToSend;
                }

                if (RespondentVariablesToSendSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _RespondentVariablesToSend;
                }

                return default(string);
            }

            set
            {
                if (RespondentVariablesToSendSetString != null)
                {
                    RespondentVariablesToSendSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerSettings)_inner).RespondentVariablesToSend = value;
                    return;
                }

                if (RespondentVariablesToSendGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _RespondentVariablesToSend = value;
                }

            }
        }

        private int _ServiceCallsRetryLimit;
        public Func<int> ServiceCallsRetryLimitGet;
        public Action<int> ServiceCallsRetryLimitSetInt32;

        int IDialerSettings.ServiceCallsRetryLimit
        {
            get
            {
                if (ServiceCallsRetryLimitGet != null)
                {
                    return ServiceCallsRetryLimitGet();
                } else if (_inner != null)
                {
                    return ((IDialerSettings)_inner).ServiceCallsRetryLimit;
                }

                if (ServiceCallsRetryLimitSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ServiceCallsRetryLimit;
                }

                return default(int);
            }

            set
            {
                if (ServiceCallsRetryLimitSetInt32 != null)
                {
                    ServiceCallsRetryLimitSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerSettings)_inner).ServiceCallsRetryLimit = value;
                    return;
                }

                if (ServiceCallsRetryLimitGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ServiceCallsRetryLimit = value;
                }

            }
        }

        private string _SettingsTemplatesJson;
        public Func<string> SettingsTemplatesJsonGet;
        public Action<string> SettingsTemplatesJsonSetString;

        string IDialerSettings.SettingsTemplatesJson
        {
            get
            {
                if (SettingsTemplatesJsonGet != null)
                {
                    return SettingsTemplatesJsonGet();
                } else if (_inner != null)
                {
                    return ((IDialerSettings)_inner).SettingsTemplatesJson;
                }

                if (SettingsTemplatesJsonSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SettingsTemplatesJson;
                }

                return default(string);
            }

            set
            {
                if (SettingsTemplatesJsonSetString != null)
                {
                    SettingsTemplatesJsonSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerSettings)_inner).SettingsTemplatesJson = value;
                    return;
                }

                if (SettingsTemplatesJsonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SettingsTemplatesJson = value;
                }

            }
        }

        private int _WaitDialerNotificationAtEnableDialerCommandTimeoutInMs;
        public Func<int> WaitDialerNotificationAtEnableDialerCommandTimeoutInMsGet;
        public Action<int> WaitDialerNotificationAtEnableDialerCommandTimeoutInMsSetInt32;

        int IDialerSettings.WaitDialerNotificationAtEnableDialerCommandTimeoutInMs
        {
            get
            {
                if (WaitDialerNotificationAtEnableDialerCommandTimeoutInMsGet != null)
                {
                    return WaitDialerNotificationAtEnableDialerCommandTimeoutInMsGet();
                } else if (_inner != null)
                {
                    return ((IDialerSettings)_inner).WaitDialerNotificationAtEnableDialerCommandTimeoutInMs;
                }

                if (WaitDialerNotificationAtEnableDialerCommandTimeoutInMsSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _WaitDialerNotificationAtEnableDialerCommandTimeoutInMs;
                }

                return default(int);
            }

            set
            {
                if (WaitDialerNotificationAtEnableDialerCommandTimeoutInMsSetInt32 != null)
                {
                    WaitDialerNotificationAtEnableDialerCommandTimeoutInMsSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerSettings)_inner).WaitDialerNotificationAtEnableDialerCommandTimeoutInMs = value;
                    return;
                }

                if (WaitDialerNotificationAtEnableDialerCommandTimeoutInMsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _WaitDialerNotificationAtEnableDialerCommandTimeoutInMs = value;
                }

            }
        }

        private DiallerType _Dialer;
        public Func<DiallerType> DialerGet;
        public Action<DiallerType> DialerSetDiallerType;

        DiallerType IDialerSettings.Dialer
        {
            get
            {
                if (DialerGet != null)
                {
                    return DialerGet();
                } else if (_inner != null)
                {
                    return ((IDialerSettings)_inner).Dialer;
                }

                if (DialerSetDiallerType == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Dialer;
                }

                return default(DiallerType);
            }

        }

        private ConcurrentDictionary<AudioMessageType, AudioMessageDescriptor> _InboundAudioMessagesDictionary;
        public Func<ConcurrentDictionary<AudioMessageType, AudioMessageDescriptor>> InboundAudioMessagesDictionaryGet;
        public Action<ConcurrentDictionary<AudioMessageType, AudioMessageDescriptor>> InboundAudioMessagesDictionarySetConcurrentDictionaryOfAudioMessageTypeAudioMessageDescriptor;

        ConcurrentDictionary<AudioMessageType, AudioMessageDescriptor> IDialerSettings.InboundAudioMessagesDictionary
        {
            get
            {
                if (InboundAudioMessagesDictionaryGet != null)
                {
                    return InboundAudioMessagesDictionaryGet();
                } else if (_inner != null)
                {
                    return ((IDialerSettings)_inner).InboundAudioMessagesDictionary;
                }

                if (InboundAudioMessagesDictionarySetConcurrentDictionaryOfAudioMessageTypeAudioMessageDescriptor == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InboundAudioMessagesDictionary;
                }

                return default(ConcurrentDictionary<AudioMessageType, AudioMessageDescriptor>);
            }

            set
            {
                if (InboundAudioMessagesDictionarySetConcurrentDictionaryOfAudioMessageTypeAudioMessageDescriptor != null)
                {
                    InboundAudioMessagesDictionarySetConcurrentDictionaryOfAudioMessageTypeAudioMessageDescriptor(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDialerSettings)_inner).InboundAudioMessagesDictionary = value;
                    return;
                }

                if (InboundAudioMessagesDictionaryGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _InboundAudioMessagesDictionary = value;
                }

            }
        }

    }
}