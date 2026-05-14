using System;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.SystemSettings.Toggle;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIToggleSettingsGroup : IToggleSettingsGroup 
    {
        private IToggleSettingsGroup _inner;

        public StubIToggleSettingsGroup()
        {
            _inner = null;
        }

        public IToggleSettingsGroup Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void OnChangedDelegate();
        public OnChangedDelegate OnChanged;

        void ISystemSettingsNotifyChanged.OnChanged()
        {

            if (OnChanged != null)
            {
                OnChanged();
            } else if (_inner != null)
            {
                ((ISystemSettingsNotifyChanged)_inner).OnChanged();
            }
        }

        private bool _ShowDialType;
        public Func<bool> ShowDialTypeGet;
        public Action<bool> ShowDialTypeSetBoolean;

        bool IToggleSettings.ShowDialType
        {
            get
            {
                if (ShowDialTypeGet != null)
                {
                    return ShowDialTypeGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).ShowDialType;
                }

                if (ShowDialTypeSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ShowDialType;
                }

                return default(bool);
            }

        }

        private IBBCCSettings _BBCC;
        public Func<IBBCCSettings> BBCCGet;
        public Action<IBBCCSettings> BBCCSetIBBCCSettings;

        IBBCCSettings IToggleSettings.BBCC
        {
            get
            {
                if (BBCCGet != null)
                {
                    return BBCCGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).BBCC;
                }

                if (BBCCSetIBBCCSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _BBCC;
                }

                return default(IBBCCSettings);
            }

        }

        private ICatiAgentSettings _CatiAgent;
        public Func<ICatiAgentSettings> CatiAgentGet;
        public Action<ICatiAgentSettings> CatiAgentSetICatiAgentSettings;

        ICatiAgentSettings IToggleSettings.CatiAgent
        {
            get
            {
                if (CatiAgentGet != null)
                {
                    return CatiAgentGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).CatiAgent;
                }

                if (CatiAgentSetICatiAgentSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CatiAgent;
                }

                return default(ICatiAgentSettings);
            }

        }

        private ISupervisorSettings _Supervisor;
        public Func<ISupervisorSettings> SupervisorGet;
        public Action<ISupervisorSettings> SupervisorSetISupervisorSettings;

        ISupervisorSettings IToggleSettings.Supervisor
        {
            get
            {
                if (SupervisorGet != null)
                {
                    return SupervisorGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).Supervisor;
                }

                if (SupervisorSetISupervisorSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Supervisor;
                }

                return default(ISupervisorSettings);
            }

        }

        private bool _BvSvyScheduleDeadlockReduction;
        public Func<bool> BvSvyScheduleDeadlockReductionGet;
        public Action<bool> BvSvyScheduleDeadlockReductionSetBoolean;

        bool IToggleSettings.BvSvyScheduleDeadlockReduction
        {
            get
            {
                if (BvSvyScheduleDeadlockReductionGet != null)
                {
                    return BvSvyScheduleDeadlockReductionGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).BvSvyScheduleDeadlockReduction;
                }

                if (BvSvyScheduleDeadlockReductionSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _BvSvyScheduleDeadlockReduction;
                }

                return default(bool);
            }

            set
            {
                if (BvSvyScheduleDeadlockReductionSetBoolean != null)
                {
                    BvSvyScheduleDeadlockReductionSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).BvSvyScheduleDeadlockReduction = value;
                    return;
                }

                if (BvSvyScheduleDeadlockReductionGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _BvSvyScheduleDeadlockReduction = value;
                }

            }
        }

        private bool _DirectlyInsertResponses;
        public Func<bool> DirectlyInsertResponsesGet;
        public Action<bool> DirectlyInsertResponsesSetBoolean;

        bool IToggleSettings.DirectlyInsertResponses
        {
            get
            {
                if (DirectlyInsertResponsesGet != null)
                {
                    return DirectlyInsertResponsesGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).DirectlyInsertResponses;
                }

                if (DirectlyInsertResponsesSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DirectlyInsertResponses;
                }

                return default(bool);
            }

            set
            {
                if (DirectlyInsertResponsesSetBoolean != null)
                {
                    DirectlyInsertResponsesSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).DirectlyInsertResponses = value;
                    return;
                }

                if (DirectlyInsertResponsesGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _DirectlyInsertResponses = value;
                }

            }
        }

        private bool _EnableAgentAssistedDialling;
        public Func<bool> EnableAgentAssistedDiallingGet;
        public Action<bool> EnableAgentAssistedDiallingSetBoolean;

        bool IToggleSettings.EnableAgentAssistedDialling
        {
            get
            {
                if (EnableAgentAssistedDiallingGet != null)
                {
                    return EnableAgentAssistedDiallingGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).EnableAgentAssistedDialling;
                }

                if (EnableAgentAssistedDiallingSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableAgentAssistedDialling;
                }

                return default(bool);
            }

            set
            {
                if (EnableAgentAssistedDiallingSetBoolean != null)
                {
                    EnableAgentAssistedDiallingSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).EnableAgentAssistedDialling = value;
                    return;
                }

                if (EnableAgentAssistedDiallingGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableAgentAssistedDialling = value;
                }

            }
        }

        private bool _EnableAlertsConfiguration;
        public Func<bool> EnableAlertsConfigurationGet;
        public Action<bool> EnableAlertsConfigurationSetBoolean;

        bool IToggleSettings.EnableAlertsConfiguration
        {
            get
            {
                if (EnableAlertsConfigurationGet != null)
                {
                    return EnableAlertsConfigurationGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).EnableAlertsConfiguration;
                }

                if (EnableAlertsConfigurationSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableAlertsConfiguration;
                }

                return default(bool);
            }

            set
            {
                if (EnableAlertsConfigurationSetBoolean != null)
                {
                    EnableAlertsConfigurationSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).EnableAlertsConfiguration = value;
                    return;
                }

                if (EnableAlertsConfigurationGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableAlertsConfiguration = value;
                }

            }
        }

        private bool _EnableCustomCallAttemptFields;
        public Func<bool> EnableCustomCallAttemptFieldsGet;
        public Action<bool> EnableCustomCallAttemptFieldsSetBoolean;

        bool IToggleSettings.EnableCustomCallAttemptFields
        {
            get
            {
                if (EnableCustomCallAttemptFieldsGet != null)
                {
                    return EnableCustomCallAttemptFieldsGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).EnableCustomCallAttemptFields;
                }

                if (EnableCustomCallAttemptFieldsSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableCustomCallAttemptFields;
                }

                return default(bool);
            }

            set
            {
                if (EnableCustomCallAttemptFieldsSetBoolean != null)
                {
                    EnableCustomCallAttemptFieldsSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).EnableCustomCallAttemptFields = value;
                    return;
                }

                if (EnableCustomCallAttemptFieldsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableCustomCallAttemptFields = value;
                }

            }
        }

        private bool _EnableDesktopConsoleLogin;
        public Func<bool> EnableDesktopConsoleLoginGet;
        public Action<bool> EnableDesktopConsoleLoginSetBoolean;

        bool IToggleSettings.EnableDesktopConsoleLogin
        {
            get
            {
                if (EnableDesktopConsoleLoginGet != null)
                {
                    return EnableDesktopConsoleLoginGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).EnableDesktopConsoleLogin;
                }

                if (EnableDesktopConsoleLoginSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableDesktopConsoleLogin;
                }

                return default(bool);
            }

            set
            {
                if (EnableDesktopConsoleLoginSetBoolean != null)
                {
                    EnableDesktopConsoleLoginSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).EnableDesktopConsoleLogin = value;
                    return;
                }

                if (EnableDesktopConsoleLoginGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableDesktopConsoleLogin = value;
                }

            }
        }

        private bool _EnableDesktopLiveMonitoring;
        public Func<bool> EnableDesktopLiveMonitoringGet;
        public Action<bool> EnableDesktopLiveMonitoringSetBoolean;

        bool IToggleSettings.EnableDesktopLiveMonitoring
        {
            get
            {
                if (EnableDesktopLiveMonitoringGet != null)
                {
                    return EnableDesktopLiveMonitoringGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).EnableDesktopLiveMonitoring;
                }

                if (EnableDesktopLiveMonitoringSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableDesktopLiveMonitoring;
                }

                return default(bool);
            }

            set
            {
                if (EnableDesktopLiveMonitoringSetBoolean != null)
                {
                    EnableDesktopLiveMonitoringSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).EnableDesktopLiveMonitoring = value;
                    return;
                }

                if (EnableDesktopLiveMonitoringGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableDesktopLiveMonitoring = value;
                }

            }
        }

        private bool _EnableDesktopMonitoringConsole;
        public Func<bool> EnableDesktopMonitoringConsoleGet;
        public Action<bool> EnableDesktopMonitoringConsoleSetBoolean;

        bool IToggleSettings.EnableDesktopMonitoringConsole
        {
            get
            {
                if (EnableDesktopMonitoringConsoleGet != null)
                {
                    return EnableDesktopMonitoringConsoleGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).EnableDesktopMonitoringConsole;
                }

                if (EnableDesktopMonitoringConsoleSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableDesktopMonitoringConsole;
                }

                return default(bool);
            }

            set
            {
                if (EnableDesktopMonitoringConsoleSetBoolean != null)
                {
                    EnableDesktopMonitoringConsoleSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).EnableDesktopMonitoringConsole = value;
                    return;
                }

                if (EnableDesktopMonitoringConsoleGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableDesktopMonitoringConsole = value;
                }

            }
        }

        private bool _EnableExternalTransfer;
        public Func<bool> EnableExternalTransferGet;
        public Action<bool> EnableExternalTransferSetBoolean;

        bool IToggleSettings.EnableExternalTransfer
        {
            get
            {
                if (EnableExternalTransferGet != null)
                {
                    return EnableExternalTransferGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).EnableExternalTransfer;
                }

                if (EnableExternalTransferSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableExternalTransfer;
                }

                return default(bool);
            }

            set
            {
                if (EnableExternalTransferSetBoolean != null)
                {
                    EnableExternalTransferSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).EnableExternalTransfer = value;
                    return;
                }

                if (EnableExternalTransferGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableExternalTransfer = value;
                }

            }
        }

        private bool _EnableHttpKeepAliveForDialer;
        public Func<bool> EnableHttpKeepAliveForDialerGet;
        public Action<bool> EnableHttpKeepAliveForDialerSetBoolean;

        bool IToggleSettings.EnableHttpKeepAliveForDialer
        {
            get
            {
                if (EnableHttpKeepAliveForDialerGet != null)
                {
                    return EnableHttpKeepAliveForDialerGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).EnableHttpKeepAliveForDialer;
                }

                if (EnableHttpKeepAliveForDialerSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableHttpKeepAliveForDialer;
                }

                return default(bool);
            }

            set
            {
                if (EnableHttpKeepAliveForDialerSetBoolean != null)
                {
                    EnableHttpKeepAliveForDialerSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).EnableHttpKeepAliveForDialer = value;
                    return;
                }

                if (EnableHttpKeepAliveForDialerGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableHttpKeepAliveForDialer = value;
                }

            }
        }

        private bool _EnableHubIntegration;
        public Func<bool> EnableHubIntegrationGet;
        public Action<bool> EnableHubIntegrationSetBoolean;

        bool IToggleSettings.EnableHubIntegration
        {
            get
            {
                if (EnableHubIntegrationGet != null)
                {
                    return EnableHubIntegrationGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).EnableHubIntegration;
                }

                if (EnableHubIntegrationSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableHubIntegration;
                }

                return default(bool);
            }

            set
            {
                if (EnableHubIntegrationSetBoolean != null)
                {
                    EnableHubIntegrationSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).EnableHubIntegration = value;
                    return;
                }

                if (EnableHubIntegrationGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableHubIntegration = value;
                }

            }
        }

        private bool _EnableInbound;
        public Func<bool> EnableInboundGet;
        public Action<bool> EnableInboundSetBoolean;

        bool IToggleSettings.EnableInbound
        {
            get
            {
                if (EnableInboundGet != null)
                {
                    return EnableInboundGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).EnableInbound;
                }

                if (EnableInboundSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableInbound;
                }

                return default(bool);
            }

            set
            {
                if (EnableInboundSetBoolean != null)
                {
                    EnableInboundSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).EnableInbound = value;
                    return;
                }

                if (EnableInboundGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableInbound = value;
                }

            }
        }

        private bool _EnableInboundForPreviewInPredictiveMode;
        public Func<bool> EnableInboundForPreviewInPredictiveModeGet;
        public Action<bool> EnableInboundForPreviewInPredictiveModeSetBoolean;

        bool IToggleSettings.EnableInboundForPreviewInPredictiveMode
        {
            get
            {
                if (EnableInboundForPreviewInPredictiveModeGet != null)
                {
                    return EnableInboundForPreviewInPredictiveModeGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).EnableInboundForPreviewInPredictiveMode;
                }

                if (EnableInboundForPreviewInPredictiveModeSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableInboundForPreviewInPredictiveMode;
                }

                return default(bool);
            }

            set
            {
                if (EnableInboundForPreviewInPredictiveModeSetBoolean != null)
                {
                    EnableInboundForPreviewInPredictiveModeSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).EnableInboundForPreviewInPredictiveMode = value;
                    return;
                }

                if (EnableInboundForPreviewInPredictiveModeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableInboundForPreviewInPredictiveMode = value;
                }

            }
        }

        private bool _EnableInternalTransfer;
        public Func<bool> EnableInternalTransferGet;
        public Action<bool> EnableInternalTransferSetBoolean;

        bool IToggleSettings.EnableInternalTransfer
        {
            get
            {
                if (EnableInternalTransferGet != null)
                {
                    return EnableInternalTransferGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).EnableInternalTransfer;
                }

                if (EnableInternalTransferSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableInternalTransfer;
                }

                return default(bool);
            }

            set
            {
                if (EnableInternalTransferSetBoolean != null)
                {
                    EnableInternalTransferSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).EnableInternalTransfer = value;
                    return;
                }

                if (EnableInternalTransferGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableInternalTransfer = value;
                }

            }
        }

        private bool _EnableInterviewerMetricsConfiguration;
        public Func<bool> EnableInterviewerMetricsConfigurationGet;
        public Action<bool> EnableInterviewerMetricsConfigurationSetBoolean;

        bool IToggleSettings.EnableInterviewerMetricsConfiguration
        {
            get
            {
                if (EnableInterviewerMetricsConfigurationGet != null)
                {
                    return EnableInterviewerMetricsConfigurationGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).EnableInterviewerMetricsConfiguration;
                }

                if (EnableInterviewerMetricsConfigurationSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableInterviewerMetricsConfiguration;
                }

                return default(bool);
            }

            set
            {
                if (EnableInterviewerMetricsConfigurationSetBoolean != null)
                {
                    EnableInterviewerMetricsConfigurationSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).EnableInterviewerMetricsConfiguration = value;
                    return;
                }

                if (EnableInterviewerMetricsConfigurationGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableInterviewerMetricsConfiguration = value;
                }

            }
        }

        private bool _EnableIVR;
        public Func<bool> EnableIVRGet;
        public Action<bool> EnableIVRSetBoolean;

        bool IToggleSettings.EnableIVR
        {
            get
            {
                if (EnableIVRGet != null)
                {
                    return EnableIVRGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).EnableIVR;
                }

                if (EnableIVRSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableIVR;
                }

                return default(bool);
            }

            set
            {
                if (EnableIVRSetBoolean != null)
                {
                    EnableIVRSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).EnableIVR = value;
                    return;
                }

                if (EnableIVRGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableIVR = value;
                }

            }
        }

        private bool _EnableMonitoringBargingMode;
        public Func<bool> EnableMonitoringBargingModeGet;
        public Action<bool> EnableMonitoringBargingModeSetBoolean;

        bool IToggleSettings.EnableMonitoringBargingMode
        {
            get
            {
                if (EnableMonitoringBargingModeGet != null)
                {
                    return EnableMonitoringBargingModeGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).EnableMonitoringBargingMode;
                }

                if (EnableMonitoringBargingModeSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableMonitoringBargingMode;
                }

                return default(bool);
            }

            set
            {
                if (EnableMonitoringBargingModeSetBoolean != null)
                {
                    EnableMonitoringBargingModeSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).EnableMonitoringBargingMode = value;
                    return;
                }

                if (EnableMonitoringBargingModeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableMonitoringBargingMode = value;
                }

            }
        }

        private bool _EnableMonitoringCoachingMode;
        public Func<bool> EnableMonitoringCoachingModeGet;
        public Action<bool> EnableMonitoringCoachingModeSetBoolean;

        bool IToggleSettings.EnableMonitoringCoachingMode
        {
            get
            {
                if (EnableMonitoringCoachingModeGet != null)
                {
                    return EnableMonitoringCoachingModeGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).EnableMonitoringCoachingMode;
                }

                if (EnableMonitoringCoachingModeSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableMonitoringCoachingMode;
                }

                return default(bool);
            }

            set
            {
                if (EnableMonitoringCoachingModeSetBoolean != null)
                {
                    EnableMonitoringCoachingModeSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).EnableMonitoringCoachingMode = value;
                    return;
                }

                if (EnableMonitoringCoachingModeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableMonitoringCoachingMode = value;
                }

            }
        }

        private bool _EnableSeamlessSurveySwitching;
        public Func<bool> EnableSeamlessSurveySwitchingGet;
        public Action<bool> EnableSeamlessSurveySwitchingSetBoolean;

        bool IToggleSettings.EnableSeamlessSurveySwitching
        {
            get
            {
                if (EnableSeamlessSurveySwitchingGet != null)
                {
                    return EnableSeamlessSurveySwitchingGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).EnableSeamlessSurveySwitching;
                }

                if (EnableSeamlessSurveySwitchingSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableSeamlessSurveySwitching;
                }

                return default(bool);
            }

            set
            {
                if (EnableSeamlessSurveySwitchingSetBoolean != null)
                {
                    EnableSeamlessSurveySwitchingSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).EnableSeamlessSurveySwitching = value;
                    return;
                }

                if (EnableSeamlessSurveySwitchingGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableSeamlessSurveySwitching = value;
                }

            }
        }

        private bool _EnableTCPA;
        public Func<bool> EnableTCPAGet;
        public Action<bool> EnableTCPASetBoolean;

        bool IToggleSettings.EnableTCPA
        {
            get
            {
                if (EnableTCPAGet != null)
                {
                    return EnableTCPAGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).EnableTCPA;
                }

                if (EnableTCPASetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableTCPA;
                }

                return default(bool);
            }

            set
            {
                if (EnableTCPASetBoolean != null)
                {
                    EnableTCPASetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).EnableTCPA = value;
                    return;
                }

                if (EnableTCPAGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableTCPA = value;
                }

            }
        }

        private bool _EnforceCatiHostNameForSurveys;
        public Func<bool> EnforceCatiHostNameForSurveysGet;
        public Action<bool> EnforceCatiHostNameForSurveysSetBoolean;

        bool IToggleSettings.EnforceCatiHostNameForSurveys
        {
            get
            {
                if (EnforceCatiHostNameForSurveysGet != null)
                {
                    return EnforceCatiHostNameForSurveysGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).EnforceCatiHostNameForSurveys;
                }

                if (EnforceCatiHostNameForSurveysSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnforceCatiHostNameForSurveys;
                }

                return default(bool);
            }

            set
            {
                if (EnforceCatiHostNameForSurveysSetBoolean != null)
                {
                    EnforceCatiHostNameForSurveysSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).EnforceCatiHostNameForSurveys = value;
                    return;
                }

                if (EnforceCatiHostNameForSurveysGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnforceCatiHostNameForSurveys = value;
                }

            }
        }

        private bool _NewCallManagement;
        public Func<bool> NewCallManagementGet;
        public Action<bool> NewCallManagementSetBoolean;

        bool IToggleSettings.NewCallManagement
        {
            get
            {
                if (NewCallManagementGet != null)
                {
                    return NewCallManagementGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).NewCallManagement;
                }

                if (NewCallManagementSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _NewCallManagement;
                }

                return default(bool);
            }

            set
            {
                if (NewCallManagementSetBoolean != null)
                {
                    NewCallManagementSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).NewCallManagement = value;
                    return;
                }

                if (NewCallManagementGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _NewCallManagement = value;
                }

            }
        }

        private bool _RabbitMqCacheInvalidation;
        public Func<bool> RabbitMqCacheInvalidationGet;
        public Action<bool> RabbitMqCacheInvalidationSetBoolean;

        bool IToggleSettings.RabbitMqCacheInvalidation
        {
            get
            {
                if (RabbitMqCacheInvalidationGet != null)
                {
                    return RabbitMqCacheInvalidationGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).RabbitMqCacheInvalidation;
                }

                if (RabbitMqCacheInvalidationSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _RabbitMqCacheInvalidation;
                }

                return default(bool);
            }

            set
            {
                if (RabbitMqCacheInvalidationSetBoolean != null)
                {
                    RabbitMqCacheInvalidationSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).RabbitMqCacheInvalidation = value;
                    return;
                }

                if (RabbitMqCacheInvalidationGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _RabbitMqCacheInvalidation = value;
                }

            }
        }

        private bool _ReadInterviewLanguageDirectly;
        public Func<bool> ReadInterviewLanguageDirectlyGet;
        public Action<bool> ReadInterviewLanguageDirectlySetBoolean;

        bool IToggleSettings.ReadInterviewLanguageDirectly
        {
            get
            {
                if (ReadInterviewLanguageDirectlyGet != null)
                {
                    return ReadInterviewLanguageDirectlyGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).ReadInterviewLanguageDirectly;
                }

                if (ReadInterviewLanguageDirectlySetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ReadInterviewLanguageDirectly;
                }

                return default(bool);
            }

            set
            {
                if (ReadInterviewLanguageDirectlySetBoolean != null)
                {
                    ReadInterviewLanguageDirectlySetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).ReadInterviewLanguageDirectly = value;
                    return;
                }

                if (ReadInterviewLanguageDirectlyGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ReadInterviewLanguageDirectly = value;
                }

            }
        }

        private bool _SendConsoleStateInRmqPayload;
        public Func<bool> SendConsoleStateInRmqPayloadGet;
        public Action<bool> SendConsoleStateInRmqPayloadSetBoolean;

        bool IToggleSettings.SendConsoleStateInRmqPayload
        {
            get
            {
                if (SendConsoleStateInRmqPayloadGet != null)
                {
                    return SendConsoleStateInRmqPayloadGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).SendConsoleStateInRmqPayload;
                }

                if (SendConsoleStateInRmqPayloadSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SendConsoleStateInRmqPayload;
                }

                return default(bool);
            }

            set
            {
                if (SendConsoleStateInRmqPayloadSetBoolean != null)
                {
                    SendConsoleStateInRmqPayloadSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).SendConsoleStateInRmqPayload = value;
                    return;
                }

                if (SendConsoleStateInRmqPayloadGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SendConsoleStateInRmqPayload = value;
                }

            }
        }

        private bool _SendGoNotReadyImmediately;
        public Func<bool> SendGoNotReadyImmediatelyGet;
        public Action<bool> SendGoNotReadyImmediatelySetBoolean;

        bool IToggleSettings.SendGoNotReadyImmediately
        {
            get
            {
                if (SendGoNotReadyImmediatelyGet != null)
                {
                    return SendGoNotReadyImmediatelyGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).SendGoNotReadyImmediately;
                }

                if (SendGoNotReadyImmediatelySetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SendGoNotReadyImmediately;
                }

                return default(bool);
            }

            set
            {
                if (SendGoNotReadyImmediatelySetBoolean != null)
                {
                    SendGoNotReadyImmediatelySetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).SendGoNotReadyImmediately = value;
                    return;
                }

                if (SendGoNotReadyImmediatelyGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SendGoNotReadyImmediately = value;
                }

            }
        }

        private bool _UseNewDialerApi;
        public Func<bool> UseNewDialerApiGet;
        public Action<bool> UseNewDialerApiSetBoolean;

        bool IToggleSettings.UseNewDialerApi
        {
            get
            {
                if (UseNewDialerApiGet != null)
                {
                    return UseNewDialerApiGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).UseNewDialerApi;
                }

                if (UseNewDialerApiSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _UseNewDialerApi;
                }

                return default(bool);
            }

            set
            {
                if (UseNewDialerApiSetBoolean != null)
                {
                    UseNewDialerApiSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).UseNewDialerApi = value;
                    return;
                }

                if (UseNewDialerApiGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _UseNewDialerApi = value;
                }

            }
        }

        private bool _UseReactSurveyList;
        public Func<bool> UseReactSurveyListGet;
        public Action<bool> UseReactSurveyListSetBoolean;

        bool IToggleSettings.UseReactSurveyList
        {
            get
            {
                if (UseReactSurveyListGet != null)
                {
                    return UseReactSurveyListGet();
                } else if (_inner != null)
                {
                    return ((IToggleSettings)_inner).UseReactSurveyList;
                }

                if (UseReactSurveyListSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _UseReactSurveyList;
                }

                return default(bool);
            }

            set
            {
                if (UseReactSurveyListSetBoolean != null)
                {
                    UseReactSurveyListSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IToggleSettings)_inner).UseReactSurveyList = value;
                    return;
                }

                if (UseReactSurveyListGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _UseReactSurveyList = value;
                }

            }
        }

    }
}