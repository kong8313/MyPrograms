using System;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.SystemSettings.Console;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIConsoleSettingsGroup : IConsoleSettingsGroup 
    {
        private IConsoleSettingsGroup _inner;

        public StubIConsoleSettingsGroup()
        {
            _inner = null;
        }

        public IConsoleSettingsGroup Inner
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

        private IBBCCSettings _BBCC;
        public Func<IBBCCSettings> BBCCGet;
        public Action<IBBCCSettings> BBCCSetIBBCCSettings;

        IBBCCSettings IConsoleSettings.BBCC
        {
            get
            {
                if (BBCCGet != null)
                {
                    return BBCCGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).BBCC;
                }

                if (BBCCSetIBBCCSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _BBCC;
                }

                return default(IBBCCSettings);
            }

        }

        private IMetricsSettings _Metrics;
        public Func<IMetricsSettings> MetricsGet;
        public Action<IMetricsSettings> MetricsSetIMetricsSettings;

        IMetricsSettings IConsoleSettings.Metrics
        {
            get
            {
                if (MetricsGet != null)
                {
                    return MetricsGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).Metrics;
                }

                if (MetricsSetIMetricsSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Metrics;
                }

                return default(IMetricsSettings);
            }

        }

        private bool _AllowTransferToAssignedSurveysOnly;
        public Func<bool> AllowTransferToAssignedSurveysOnlyGet;
        public Action<bool> AllowTransferToAssignedSurveysOnlySetBoolean;

        bool IConsoleSettings.AllowTransferToAssignedSurveysOnly
        {
            get
            {
                if (AllowTransferToAssignedSurveysOnlyGet != null)
                {
                    return AllowTransferToAssignedSurveysOnlyGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).AllowTransferToAssignedSurveysOnly;
                }

                if (AllowTransferToAssignedSurveysOnlySetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AllowTransferToAssignedSurveysOnly;
                }

                return default(bool);
            }

            set
            {
                if (AllowTransferToAssignedSurveysOnlySetBoolean != null)
                {
                    AllowTransferToAssignedSurveysOnlySetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).AllowTransferToAssignedSurveysOnly = value;
                    return;
                }

                if (AllowTransferToAssignedSurveysOnlyGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AllowTransferToAssignedSurveysOnly = value;
                }

            }
        }

        private string _CompanyLogoUrl;
        public Func<string> CompanyLogoUrlGet;
        public Action<string> CompanyLogoUrlSetString;

        string IConsoleSettings.CompanyLogoUrl
        {
            get
            {
                if (CompanyLogoUrlGet != null)
                {
                    return CompanyLogoUrlGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).CompanyLogoUrl;
                }

                if (CompanyLogoUrlSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CompanyLogoUrl;
                }

                return default(string);
            }

            set
            {
                if (CompanyLogoUrlSetString != null)
                {
                    CompanyLogoUrlSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).CompanyLogoUrl = value;
                    return;
                }

                if (CompanyLogoUrlGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _CompanyLogoUrl = value;
                }

            }
        }

        private bool _EnableAbilityToCancelDial;
        public Func<bool> EnableAbilityToCancelDialGet;
        public Action<bool> EnableAbilityToCancelDialSetBoolean;

        bool IConsoleSettings.EnableAbilityToCancelDial
        {
            get
            {
                if (EnableAbilityToCancelDialGet != null)
                {
                    return EnableAbilityToCancelDialGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableAbilityToCancelDial;
                }

                if (EnableAbilityToCancelDialSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableAbilityToCancelDial;
                }

                return default(bool);
            }

            set
            {
                if (EnableAbilityToCancelDialSetBoolean != null)
                {
                    EnableAbilityToCancelDialSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableAbilityToCancelDial = value;
                    return;
                }

                if (EnableAbilityToCancelDialGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableAbilityToCancelDial = value;
                }

            }
        }

        private bool _EnableAbilityToCreateAppointmensOutsideOfThePermittedShiftTimes;
        public Func<bool> EnableAbilityToCreateAppointmensOutsideOfThePermittedShiftTimesGet;
        public Action<bool> EnableAbilityToCreateAppointmensOutsideOfThePermittedShiftTimesSetBoolean;

        bool IConsoleSettings.EnableAbilityToCreateAppointmensOutsideOfThePermittedShiftTimes
        {
            get
            {
                if (EnableAbilityToCreateAppointmensOutsideOfThePermittedShiftTimesGet != null)
                {
                    return EnableAbilityToCreateAppointmensOutsideOfThePermittedShiftTimesGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableAbilityToCreateAppointmensOutsideOfThePermittedShiftTimes;
                }

                if (EnableAbilityToCreateAppointmensOutsideOfThePermittedShiftTimesSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableAbilityToCreateAppointmensOutsideOfThePermittedShiftTimes;
                }

                return default(bool);
            }

            set
            {
                if (EnableAbilityToCreateAppointmensOutsideOfThePermittedShiftTimesSetBoolean != null)
                {
                    EnableAbilityToCreateAppointmensOutsideOfThePermittedShiftTimesSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableAbilityToCreateAppointmensOutsideOfThePermittedShiftTimes = value;
                    return;
                }

                if (EnableAbilityToCreateAppointmensOutsideOfThePermittedShiftTimesGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableAbilityToCreateAppointmensOutsideOfThePermittedShiftTimes = value;
                }

            }
        }

        private bool _EnableAppointmensListToolbarButton;
        public Func<bool> EnableAppointmensListToolbarButtonGet;
        public Action<bool> EnableAppointmensListToolbarButtonSetBoolean;

        bool IConsoleSettings.EnableAppointmensListToolbarButton
        {
            get
            {
                if (EnableAppointmensListToolbarButtonGet != null)
                {
                    return EnableAppointmensListToolbarButtonGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableAppointmensListToolbarButton;
                }

                if (EnableAppointmensListToolbarButtonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableAppointmensListToolbarButton;
                }

                return default(bool);
            }

            set
            {
                if (EnableAppointmensListToolbarButtonSetBoolean != null)
                {
                    EnableAppointmensListToolbarButtonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableAppointmensListToolbarButton = value;
                    return;
                }

                if (EnableAppointmensListToolbarButtonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableAppointmensListToolbarButton = value;
                }

            }
        }

        private bool _EnableAppointmentTimeZoneAdjustment;
        public Func<bool> EnableAppointmentTimeZoneAdjustmentGet;
        public Action<bool> EnableAppointmentTimeZoneAdjustmentSetBoolean;

        bool IConsoleSettings.EnableAppointmentTimeZoneAdjustment
        {
            get
            {
                if (EnableAppointmentTimeZoneAdjustmentGet != null)
                {
                    return EnableAppointmentTimeZoneAdjustmentGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableAppointmentTimeZoneAdjustment;
                }

                if (EnableAppointmentTimeZoneAdjustmentSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableAppointmentTimeZoneAdjustment;
                }

                return default(bool);
            }

            set
            {
                if (EnableAppointmentTimeZoneAdjustmentSetBoolean != null)
                {
                    EnableAppointmentTimeZoneAdjustmentSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableAppointmentTimeZoneAdjustment = value;
                    return;
                }

                if (EnableAppointmentTimeZoneAdjustmentGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableAppointmentTimeZoneAdjustment = value;
                }

            }
        }

        private bool _EnableAppointmentToolbarButton;
        public Func<bool> EnableAppointmentToolbarButtonGet;
        public Action<bool> EnableAppointmentToolbarButtonSetBoolean;

        bool IConsoleSettings.EnableAppointmentToolbarButton
        {
            get
            {
                if (EnableAppointmentToolbarButtonGet != null)
                {
                    return EnableAppointmentToolbarButtonGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableAppointmentToolbarButton;
                }

                if (EnableAppointmentToolbarButtonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableAppointmentToolbarButton;
                }

                return default(bool);
            }

            set
            {
                if (EnableAppointmentToolbarButtonSetBoolean != null)
                {
                    EnableAppointmentToolbarButtonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableAppointmentToolbarButton = value;
                    return;
                }

                if (EnableAppointmentToolbarButtonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableAppointmentToolbarButton = value;
                }

            }
        }

        private bool _EnableAutomaticScrolling;
        public Func<bool> EnableAutomaticScrollingGet;
        public Action<bool> EnableAutomaticScrollingSetBoolean;

        bool IConsoleSettings.EnableAutomaticScrolling
        {
            get
            {
                if (EnableAutomaticScrollingGet != null)
                {
                    return EnableAutomaticScrollingGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableAutomaticScrolling;
                }

                if (EnableAutomaticScrollingSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableAutomaticScrolling;
                }

                return default(bool);
            }

            set
            {
                if (EnableAutomaticScrollingSetBoolean != null)
                {
                    EnableAutomaticScrollingSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableAutomaticScrolling = value;
                    return;
                }

                if (EnableAutomaticScrollingGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableAutomaticScrolling = value;
                }

            }
        }

        private bool _EnableChangeTaskChoiceToolbarButton;
        public Func<bool> EnableChangeTaskChoiceToolbarButtonGet;
        public Action<bool> EnableChangeTaskChoiceToolbarButtonSetBoolean;

        bool IConsoleSettings.EnableChangeTaskChoiceToolbarButton
        {
            get
            {
                if (EnableChangeTaskChoiceToolbarButtonGet != null)
                {
                    return EnableChangeTaskChoiceToolbarButtonGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableChangeTaskChoiceToolbarButton;
                }

                if (EnableChangeTaskChoiceToolbarButtonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableChangeTaskChoiceToolbarButton;
                }

                return default(bool);
            }

            set
            {
                if (EnableChangeTaskChoiceToolbarButtonSetBoolean != null)
                {
                    EnableChangeTaskChoiceToolbarButtonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableChangeTaskChoiceToolbarButton = value;
                    return;
                }

                if (EnableChangeTaskChoiceToolbarButtonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableChangeTaskChoiceToolbarButton = value;
                }

            }
        }

        private bool _EnableCheckSpellingToolbarButton;
        public Func<bool> EnableCheckSpellingToolbarButtonGet;
        public Action<bool> EnableCheckSpellingToolbarButtonSetBoolean;

        bool IConsoleSettings.EnableCheckSpellingToolbarButton
        {
            get
            {
                if (EnableCheckSpellingToolbarButtonGet != null)
                {
                    return EnableCheckSpellingToolbarButtonGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableCheckSpellingToolbarButton;
                }

                if (EnableCheckSpellingToolbarButtonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableCheckSpellingToolbarButton;
                }

                return default(bool);
            }

            set
            {
                if (EnableCheckSpellingToolbarButtonSetBoolean != null)
                {
                    EnableCheckSpellingToolbarButtonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableCheckSpellingToolbarButton = value;
                    return;
                }

                if (EnableCheckSpellingToolbarButtonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableCheckSpellingToolbarButton = value;
                }

            }
        }

        private bool _EnableExternalCallTransferToolbarButton;
        public Func<bool> EnableExternalCallTransferToolbarButtonGet;
        public Action<bool> EnableExternalCallTransferToolbarButtonSetBoolean;

        bool IConsoleSettings.EnableExternalCallTransferToolbarButton
        {
            get
            {
                if (EnableExternalCallTransferToolbarButtonGet != null)
                {
                    return EnableExternalCallTransferToolbarButtonGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableExternalCallTransferToolbarButton;
                }

                if (EnableExternalCallTransferToolbarButtonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableExternalCallTransferToolbarButton;
                }

                return default(bool);
            }

            set
            {
                if (EnableExternalCallTransferToolbarButtonSetBoolean != null)
                {
                    EnableExternalCallTransferToolbarButtonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableExternalCallTransferToolbarButton = value;
                    return;
                }

                if (EnableExternalCallTransferToolbarButtonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableExternalCallTransferToolbarButton = value;
                }

            }
        }

        private bool _EnableFastForwardToolbarButton;
        public Func<bool> EnableFastForwardToolbarButtonGet;
        public Action<bool> EnableFastForwardToolbarButtonSetBoolean;

        bool IConsoleSettings.EnableFastForwardToolbarButton
        {
            get
            {
                if (EnableFastForwardToolbarButtonGet != null)
                {
                    return EnableFastForwardToolbarButtonGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableFastForwardToolbarButton;
                }

                if (EnableFastForwardToolbarButtonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableFastForwardToolbarButton;
                }

                return default(bool);
            }

            set
            {
                if (EnableFastForwardToolbarButtonSetBoolean != null)
                {
                    EnableFastForwardToolbarButtonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableFastForwardToolbarButton = value;
                    return;
                }

                if (EnableFastForwardToolbarButtonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableFastForwardToolbarButton = value;
                }

            }
        }

        private bool _EnableHangUpToolbarButton;
        public Func<bool> EnableHangUpToolbarButtonGet;
        public Action<bool> EnableHangUpToolbarButtonSetBoolean;

        bool IConsoleSettings.EnableHangUpToolbarButton
        {
            get
            {
                if (EnableHangUpToolbarButtonGet != null)
                {
                    return EnableHangUpToolbarButtonGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableHangUpToolbarButton;
                }

                if (EnableHangUpToolbarButtonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableHangUpToolbarButton;
                }

                return default(bool);
            }

            set
            {
                if (EnableHangUpToolbarButtonSetBoolean != null)
                {
                    EnableHangUpToolbarButtonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableHangUpToolbarButton = value;
                    return;
                }

                if (EnableHangUpToolbarButtonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableHangUpToolbarButton = value;
                }

            }
        }

        private bool _EnableInternalCallTransferToolbarButton;
        public Func<bool> EnableInternalCallTransferToolbarButtonGet;
        public Action<bool> EnableInternalCallTransferToolbarButtonSetBoolean;

        bool IConsoleSettings.EnableInternalCallTransferToolbarButton
        {
            get
            {
                if (EnableInternalCallTransferToolbarButtonGet != null)
                {
                    return EnableInternalCallTransferToolbarButtonGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableInternalCallTransferToolbarButton;
                }

                if (EnableInternalCallTransferToolbarButtonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableInternalCallTransferToolbarButton;
                }

                return default(bool);
            }

            set
            {
                if (EnableInternalCallTransferToolbarButtonSetBoolean != null)
                {
                    EnableInternalCallTransferToolbarButtonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableInternalCallTransferToolbarButton = value;
                    return;
                }

                if (EnableInternalCallTransferToolbarButtonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableInternalCallTransferToolbarButton = value;
                }

            }
        }

        private bool _EnableInterviewsRandomization;
        public Func<bool> EnableInterviewsRandomizationGet;
        public Action<bool> EnableInterviewsRandomizationSetBoolean;

        bool IConsoleSettings.EnableInterviewsRandomization
        {
            get
            {
                if (EnableInterviewsRandomizationGet != null)
                {
                    return EnableInterviewsRandomizationGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableInterviewsRandomization;
                }

                if (EnableInterviewsRandomizationSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableInterviewsRandomization;
                }

                return default(bool);
            }

            set
            {
                if (EnableInterviewsRandomizationSetBoolean != null)
                {
                    EnableInterviewsRandomizationSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableInterviewsRandomization = value;
                    return;
                }

                if (EnableInterviewsRandomizationGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableInterviewsRandomization = value;
                }

            }
        }

        private bool _EnableLogoutAfterFinishToolbarButton;
        public Func<bool> EnableLogoutAfterFinishToolbarButtonGet;
        public Action<bool> EnableLogoutAfterFinishToolbarButtonSetBoolean;

        bool IConsoleSettings.EnableLogoutAfterFinishToolbarButton
        {
            get
            {
                if (EnableLogoutAfterFinishToolbarButtonGet != null)
                {
                    return EnableLogoutAfterFinishToolbarButtonGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableLogoutAfterFinishToolbarButton;
                }

                if (EnableLogoutAfterFinishToolbarButtonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableLogoutAfterFinishToolbarButton;
                }

                return default(bool);
            }

            set
            {
                if (EnableLogoutAfterFinishToolbarButtonSetBoolean != null)
                {
                    EnableLogoutAfterFinishToolbarButtonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableLogoutAfterFinishToolbarButton = value;
                    return;
                }

                if (EnableLogoutAfterFinishToolbarButtonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableLogoutAfterFinishToolbarButton = value;
                }

            }
        }

        private bool _EnableLogoutFromErrorAndWaitingScreen;
        public Func<bool> EnableLogoutFromErrorAndWaitingScreenGet;
        public Action<bool> EnableLogoutFromErrorAndWaitingScreenSetBoolean;

        bool IConsoleSettings.EnableLogoutFromErrorAndWaitingScreen
        {
            get
            {
                if (EnableLogoutFromErrorAndWaitingScreenGet != null)
                {
                    return EnableLogoutFromErrorAndWaitingScreenGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableLogoutFromErrorAndWaitingScreen;
                }

                if (EnableLogoutFromErrorAndWaitingScreenSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableLogoutFromErrorAndWaitingScreen;
                }

                return default(bool);
            }

            set
            {
                if (EnableLogoutFromErrorAndWaitingScreenSetBoolean != null)
                {
                    EnableLogoutFromErrorAndWaitingScreenSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableLogoutFromErrorAndWaitingScreen = value;
                    return;
                }

                if (EnableLogoutFromErrorAndWaitingScreenGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableLogoutFromErrorAndWaitingScreen = value;
                }

            }
        }

        private bool _EnableLogoutToolbarButton;
        public Func<bool> EnableLogoutToolbarButtonGet;
        public Action<bool> EnableLogoutToolbarButtonSetBoolean;

        bool IConsoleSettings.EnableLogoutToolbarButton
        {
            get
            {
                if (EnableLogoutToolbarButtonGet != null)
                {
                    return EnableLogoutToolbarButtonGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableLogoutToolbarButton;
                }

                if (EnableLogoutToolbarButtonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableLogoutToolbarButton;
                }

                return default(bool);
            }

            set
            {
                if (EnableLogoutToolbarButtonSetBoolean != null)
                {
                    EnableLogoutToolbarButtonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableLogoutToolbarButton = value;
                    return;
                }

                if (EnableLogoutToolbarButtonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableLogoutToolbarButton = value;
                }

            }
        }

        private bool _EnableMessageFormToolbarButton;
        public Func<bool> EnableMessageFormToolbarButtonGet;
        public Action<bool> EnableMessageFormToolbarButtonSetBoolean;

        bool IConsoleSettings.EnableMessageFormToolbarButton
        {
            get
            {
                if (EnableMessageFormToolbarButtonGet != null)
                {
                    return EnableMessageFormToolbarButtonGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableMessageFormToolbarButton;
                }

                if (EnableMessageFormToolbarButtonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableMessageFormToolbarButton;
                }

                return default(bool);
            }

            set
            {
                if (EnableMessageFormToolbarButtonSetBoolean != null)
                {
                    EnableMessageFormToolbarButtonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableMessageFormToolbarButton = value;
                    return;
                }

                if (EnableMessageFormToolbarButtonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableMessageFormToolbarButton = value;
                }

            }
        }

        private bool _EnableNextPageToolbarButton;
        public Func<bool> EnableNextPageToolbarButtonGet;
        public Action<bool> EnableNextPageToolbarButtonSetBoolean;

        bool IConsoleSettings.EnableNextPageToolbarButton
        {
            get
            {
                if (EnableNextPageToolbarButtonGet != null)
                {
                    return EnableNextPageToolbarButtonGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableNextPageToolbarButton;
                }

                if (EnableNextPageToolbarButtonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableNextPageToolbarButton;
                }

                return default(bool);
            }

            set
            {
                if (EnableNextPageToolbarButtonSetBoolean != null)
                {
                    EnableNextPageToolbarButtonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableNextPageToolbarButton = value;
                    return;
                }

                if (EnableNextPageToolbarButtonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableNextPageToolbarButton = value;
                }

            }
        }

        private bool _EnablePersistentConnectionClosing;
        public Func<bool> EnablePersistentConnectionClosingGet;
        public Action<bool> EnablePersistentConnectionClosingSetBoolean;

        bool IConsoleSettings.EnablePersistentConnectionClosing
        {
            get
            {
                if (EnablePersistentConnectionClosingGet != null)
                {
                    return EnablePersistentConnectionClosingGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnablePersistentConnectionClosing;
                }

                if (EnablePersistentConnectionClosingSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnablePersistentConnectionClosing;
                }

                return default(bool);
            }

            set
            {
                if (EnablePersistentConnectionClosingSetBoolean != null)
                {
                    EnablePersistentConnectionClosingSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnablePersistentConnectionClosing = value;
                    return;
                }

                if (EnablePersistentConnectionClosingGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnablePersistentConnectionClosing = value;
                }

            }
        }

        private bool _EnablePreviousPageToolbarButton;
        public Func<bool> EnablePreviousPageToolbarButtonGet;
        public Action<bool> EnablePreviousPageToolbarButtonSetBoolean;

        bool IConsoleSettings.EnablePreviousPageToolbarButton
        {
            get
            {
                if (EnablePreviousPageToolbarButtonGet != null)
                {
                    return EnablePreviousPageToolbarButtonGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnablePreviousPageToolbarButton;
                }

                if (EnablePreviousPageToolbarButtonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnablePreviousPageToolbarButton;
                }

                return default(bool);
            }

            set
            {
                if (EnablePreviousPageToolbarButtonSetBoolean != null)
                {
                    EnablePreviousPageToolbarButtonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnablePreviousPageToolbarButton = value;
                    return;
                }

                if (EnablePreviousPageToolbarButtonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnablePreviousPageToolbarButton = value;
                }

            }
        }

        private bool _EnableRedialNewNumberRedialDialogAbility;
        public Func<bool> EnableRedialNewNumberRedialDialogAbilityGet;
        public Action<bool> EnableRedialNewNumberRedialDialogAbilitySetBoolean;

        bool IConsoleSettings.EnableRedialNewNumberRedialDialogAbility
        {
            get
            {
                if (EnableRedialNewNumberRedialDialogAbilityGet != null)
                {
                    return EnableRedialNewNumberRedialDialogAbilityGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableRedialNewNumberRedialDialogAbility;
                }

                if (EnableRedialNewNumberRedialDialogAbilitySetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableRedialNewNumberRedialDialogAbility;
                }

                return default(bool);
            }

            set
            {
                if (EnableRedialNewNumberRedialDialogAbilitySetBoolean != null)
                {
                    EnableRedialNewNumberRedialDialogAbilitySetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableRedialNewNumberRedialDialogAbility = value;
                    return;
                }

                if (EnableRedialNewNumberRedialDialogAbilityGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableRedialNewNumberRedialDialogAbility = value;
                }

            }
        }

        private bool _EnableRedialToolbarButton;
        public Func<bool> EnableRedialToolbarButtonGet;
        public Action<bool> EnableRedialToolbarButtonSetBoolean;

        bool IConsoleSettings.EnableRedialToolbarButton
        {
            get
            {
                if (EnableRedialToolbarButtonGet != null)
                {
                    return EnableRedialToolbarButtonGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableRedialToolbarButton;
                }

                if (EnableRedialToolbarButtonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableRedialToolbarButton;
                }

                return default(bool);
            }

            set
            {
                if (EnableRedialToolbarButtonSetBoolean != null)
                {
                    EnableRedialToolbarButtonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableRedialToolbarButton = value;
                    return;
                }

                if (EnableRedialToolbarButtonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableRedialToolbarButton = value;
                }

            }
        }

        private bool _EnableRedoToolbarButton;
        public Func<bool> EnableRedoToolbarButtonGet;
        public Action<bool> EnableRedoToolbarButtonSetBoolean;

        bool IConsoleSettings.EnableRedoToolbarButton
        {
            get
            {
                if (EnableRedoToolbarButtonGet != null)
                {
                    return EnableRedoToolbarButtonGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableRedoToolbarButton;
                }

                if (EnableRedoToolbarButtonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableRedoToolbarButton;
                }

                return default(bool);
            }

            set
            {
                if (EnableRedoToolbarButtonSetBoolean != null)
                {
                    EnableRedoToolbarButtonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableRedoToolbarButton = value;
                    return;
                }

                if (EnableRedoToolbarButtonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableRedoToolbarButton = value;
                }

            }
        }

        private bool _EnableRefreshToolbarButton;
        public Func<bool> EnableRefreshToolbarButtonGet;
        public Action<bool> EnableRefreshToolbarButtonSetBoolean;

        bool IConsoleSettings.EnableRefreshToolbarButton
        {
            get
            {
                if (EnableRefreshToolbarButtonGet != null)
                {
                    return EnableRefreshToolbarButtonGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableRefreshToolbarButton;
                }

                if (EnableRefreshToolbarButtonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableRefreshToolbarButton;
                }

                return default(bool);
            }

            set
            {
                if (EnableRefreshToolbarButtonSetBoolean != null)
                {
                    EnableRefreshToolbarButtonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableRefreshToolbarButton = value;
                    return;
                }

                if (EnableRefreshToolbarButtonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableRefreshToolbarButton = value;
                }

            }
        }

        private bool _EnableSoftphoneIntegration;
        public Func<bool> EnableSoftphoneIntegrationGet;
        public Action<bool> EnableSoftphoneIntegrationSetBoolean;

        bool IConsoleSettings.EnableSoftphoneIntegration
        {
            get
            {
                if (EnableSoftphoneIntegrationGet != null)
                {
                    return EnableSoftphoneIntegrationGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableSoftphoneIntegration;
                }

                if (EnableSoftphoneIntegrationSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableSoftphoneIntegration;
                }

                return default(bool);
            }

            set
            {
                if (EnableSoftphoneIntegrationSetBoolean != null)
                {
                    EnableSoftphoneIntegrationSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableSoftphoneIntegration = value;
                    return;
                }

                if (EnableSoftphoneIntegrationGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableSoftphoneIntegration = value;
                }

            }
        }

        private bool _EnableTakeBreakToolbarButton;
        public Func<bool> EnableTakeBreakToolbarButtonGet;
        public Action<bool> EnableTakeBreakToolbarButtonSetBoolean;

        bool IConsoleSettings.EnableTakeBreakToolbarButton
        {
            get
            {
                if (EnableTakeBreakToolbarButtonGet != null)
                {
                    return EnableTakeBreakToolbarButtonGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableTakeBreakToolbarButton;
                }

                if (EnableTakeBreakToolbarButtonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableTakeBreakToolbarButton;
                }

                return default(bool);
            }

            set
            {
                if (EnableTakeBreakToolbarButtonSetBoolean != null)
                {
                    EnableTakeBreakToolbarButtonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableTakeBreakToolbarButton = value;
                    return;
                }

                if (EnableTakeBreakToolbarButtonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableTakeBreakToolbarButton = value;
                }

            }
        }

        private bool _EnableTerminateToolbarButton;
        public Func<bool> EnableTerminateToolbarButtonGet;
        public Action<bool> EnableTerminateToolbarButtonSetBoolean;

        bool IConsoleSettings.EnableTerminateToolbarButton
        {
            get
            {
                if (EnableTerminateToolbarButtonGet != null)
                {
                    return EnableTerminateToolbarButtonGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableTerminateToolbarButton;
                }

                if (EnableTerminateToolbarButtonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableTerminateToolbarButton;
                }

                return default(bool);
            }

            set
            {
                if (EnableTerminateToolbarButtonSetBoolean != null)
                {
                    EnableTerminateToolbarButtonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableTerminateToolbarButton = value;
                    return;
                }

                if (EnableTerminateToolbarButtonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableTerminateToolbarButton = value;
                }

            }
        }

        private bool _EnableTwoWayMessaging;
        public Func<bool> EnableTwoWayMessagingGet;
        public Action<bool> EnableTwoWayMessagingSetBoolean;

        bool IConsoleSettings.EnableTwoWayMessaging
        {
            get
            {
                if (EnableTwoWayMessagingGet != null)
                {
                    return EnableTwoWayMessagingGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnableTwoWayMessaging;
                }

                if (EnableTwoWayMessagingSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableTwoWayMessaging;
                }

                return default(bool);
            }

            set
            {
                if (EnableTwoWayMessagingSetBoolean != null)
                {
                    EnableTwoWayMessagingSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnableTwoWayMessaging = value;
                    return;
                }

                if (EnableTwoWayMessagingGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableTwoWayMessaging = value;
                }

            }
        }

        private bool _EnforceManualSelectionForCellPhonePerson;
        public Func<bool> EnforceManualSelectionForCellPhonePersonGet;
        public Action<bool> EnforceManualSelectionForCellPhonePersonSetBoolean;

        bool IConsoleSettings.EnforceManualSelectionForCellPhonePerson
        {
            get
            {
                if (EnforceManualSelectionForCellPhonePersonGet != null)
                {
                    return EnforceManualSelectionForCellPhonePersonGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).EnforceManualSelectionForCellPhonePerson;
                }

                if (EnforceManualSelectionForCellPhonePersonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnforceManualSelectionForCellPhonePerson;
                }

                return default(bool);
            }

            set
            {
                if (EnforceManualSelectionForCellPhonePersonSetBoolean != null)
                {
                    EnforceManualSelectionForCellPhonePersonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).EnforceManualSelectionForCellPhonePerson = value;
                    return;
                }

                if (EnforceManualSelectionForCellPhonePersonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnforceManualSelectionForCellPhonePerson = value;
                }

            }
        }

        private bool _ForceUpdateToNewVersion;
        public Func<bool> ForceUpdateToNewVersionGet;
        public Action<bool> ForceUpdateToNewVersionSetBoolean;

        bool IConsoleSettings.ForceUpdateToNewVersion
        {
            get
            {
                if (ForceUpdateToNewVersionGet != null)
                {
                    return ForceUpdateToNewVersionGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).ForceUpdateToNewVersion;
                }

                if (ForceUpdateToNewVersionSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ForceUpdateToNewVersion;
                }

                return default(bool);
            }

            set
            {
                if (ForceUpdateToNewVersionSetBoolean != null)
                {
                    ForceUpdateToNewVersionSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).ForceUpdateToNewVersion = value;
                    return;
                }

                if (ForceUpdateToNewVersionGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ForceUpdateToNewVersion = value;
                }

            }
        }

        private int _GoodConnectionThresholdMs;
        public Func<int> GoodConnectionThresholdMsGet;
        public Action<int> GoodConnectionThresholdMsSetInt32;

        int IConsoleSettings.GoodConnectionThresholdMs
        {
            get
            {
                if (GoodConnectionThresholdMsGet != null)
                {
                    return GoodConnectionThresholdMsGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).GoodConnectionThresholdMs;
                }

                if (GoodConnectionThresholdMsSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _GoodConnectionThresholdMs;
                }

                return default(int);
            }

            set
            {
                if (GoodConnectionThresholdMsSetInt32 != null)
                {
                    GoodConnectionThresholdMsSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).GoodConnectionThresholdMs = value;
                    return;
                }

                if (GoodConnectionThresholdMsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _GoodConnectionThresholdMs = value;
                }

            }
        }

        private bool _IncludeOpenEndReviewTimeInInterviewDuration;
        public Func<bool> IncludeOpenEndReviewTimeInInterviewDurationGet;
        public Action<bool> IncludeOpenEndReviewTimeInInterviewDurationSetBoolean;

        bool IConsoleSettings.IncludeOpenEndReviewTimeInInterviewDuration
        {
            get
            {
                if (IncludeOpenEndReviewTimeInInterviewDurationGet != null)
                {
                    return IncludeOpenEndReviewTimeInInterviewDurationGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).IncludeOpenEndReviewTimeInInterviewDuration;
                }

                if (IncludeOpenEndReviewTimeInInterviewDurationSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IncludeOpenEndReviewTimeInInterviewDuration;
                }

                return default(bool);
            }

            set
            {
                if (IncludeOpenEndReviewTimeInInterviewDurationSetBoolean != null)
                {
                    IncludeOpenEndReviewTimeInInterviewDurationSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).IncludeOpenEndReviewTimeInInterviewDuration = value;
                    return;
                }

                if (IncludeOpenEndReviewTimeInInterviewDurationGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _IncludeOpenEndReviewTimeInInterviewDuration = value;
                }

            }
        }

        private int _InterviewsCountShownInManualMode;
        public Func<int> InterviewsCountShownInManualModeGet;
        public Action<int> InterviewsCountShownInManualModeSetInt32;

        int IConsoleSettings.InterviewsCountShownInManualMode
        {
            get
            {
                if (InterviewsCountShownInManualModeGet != null)
                {
                    return InterviewsCountShownInManualModeGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).InterviewsCountShownInManualMode;
                }

                if (InterviewsCountShownInManualModeSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InterviewsCountShownInManualMode;
                }

                return default(int);
            }

            set
            {
                if (InterviewsCountShownInManualModeSetInt32 != null)
                {
                    InterviewsCountShownInManualModeSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).InterviewsCountShownInManualMode = value;
                    return;
                }

                if (InterviewsCountShownInManualModeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _InterviewsCountShownInManualMode = value;
                }

            }
        }

        private int _KeepAliveCallsToSave;
        public Func<int> KeepAliveCallsToSaveGet;
        public Action<int> KeepAliveCallsToSaveSetInt32;

        int IConsoleSettings.KeepAliveCallsToSave
        {
            get
            {
                if (KeepAliveCallsToSaveGet != null)
                {
                    return KeepAliveCallsToSaveGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).KeepAliveCallsToSave;
                }

                if (KeepAliveCallsToSaveSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _KeepAliveCallsToSave;
                }

                return default(int);
            }

            set
            {
                if (KeepAliveCallsToSaveSetInt32 != null)
                {
                    KeepAliveCallsToSaveSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).KeepAliveCallsToSave = value;
                    return;
                }

                if (KeepAliveCallsToSaveGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _KeepAliveCallsToSave = value;
                }

            }
        }

        private int _KeepAliveInterval;
        public Func<int> KeepAliveIntervalGet;
        public Action<int> KeepAliveIntervalSetInt32;

        int IConsoleSettings.KeepAliveInterval
        {
            get
            {
                if (KeepAliveIntervalGet != null)
                {
                    return KeepAliveIntervalGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).KeepAliveInterval;
                }

                if (KeepAliveIntervalSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _KeepAliveInterval;
                }

                return default(int);
            }

            set
            {
                if (KeepAliveIntervalSetInt32 != null)
                {
                    KeepAliveIntervalSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).KeepAliveInterval = value;
                    return;
                }

                if (KeepAliveIntervalGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _KeepAliveInterval = value;
                }

            }
        }

        private int _LinkedInterviewsLimit;
        public Func<int> LinkedInterviewsLimitGet;
        public Action<int> LinkedInterviewsLimitSetInt32;

        int IConsoleSettings.LinkedInterviewsLimit
        {
            get
            {
                if (LinkedInterviewsLimitGet != null)
                {
                    return LinkedInterviewsLimitGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).LinkedInterviewsLimit;
                }

                if (LinkedInterviewsLimitSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _LinkedInterviewsLimit;
                }

                return default(int);
            }

            set
            {
                if (LinkedInterviewsLimitSetInt32 != null)
                {
                    LinkedInterviewsLimitSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).LinkedInterviewsLimit = value;
                    return;
                }

                if (LinkedInterviewsLimitGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _LinkedInterviewsLimit = value;
                }

            }
        }

        private bool _ManualCallsInsideShiftOnly;
        public Func<bool> ManualCallsInsideShiftOnlyGet;
        public Action<bool> ManualCallsInsideShiftOnlySetBoolean;

        bool IConsoleSettings.ManualCallsInsideShiftOnly
        {
            get
            {
                if (ManualCallsInsideShiftOnlyGet != null)
                {
                    return ManualCallsInsideShiftOnlyGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).ManualCallsInsideShiftOnly;
                }

                if (ManualCallsInsideShiftOnlySetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ManualCallsInsideShiftOnly;
                }

                return default(bool);
            }

            set
            {
                if (ManualCallsInsideShiftOnlySetBoolean != null)
                {
                    ManualCallsInsideShiftOnlySetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).ManualCallsInsideShiftOnly = value;
                    return;
                }

                if (ManualCallsInsideShiftOnlyGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ManualCallsInsideShiftOnly = value;
                }

            }
        }

        private bool _ManualDialTypeSelection;
        public Func<bool> ManualDialTypeSelectionGet;
        public Action<bool> ManualDialTypeSelectionSetBoolean;

        bool IConsoleSettings.ManualDialTypeSelection
        {
            get
            {
                if (ManualDialTypeSelectionGet != null)
                {
                    return ManualDialTypeSelectionGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).ManualDialTypeSelection;
                }

                if (ManualDialTypeSelectionSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ManualDialTypeSelection;
                }

                return default(bool);
            }

            set
            {
                if (ManualDialTypeSelectionSetBoolean != null)
                {
                    ManualDialTypeSelectionSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).ManualDialTypeSelection = value;
                    return;
                }

                if (ManualDialTypeSelectionGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ManualDialTypeSelection = value;
                }

            }
        }

        private int _NoCallsTimeout;
        public Func<int> NoCallsTimeoutGet;
        public Action<int> NoCallsTimeoutSetInt32;

        int IConsoleSettings.NoCallsTimeout
        {
            get
            {
                if (NoCallsTimeoutGet != null)
                {
                    return NoCallsTimeoutGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).NoCallsTimeout;
                }

                if (NoCallsTimeoutSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _NoCallsTimeout;
                }

                return default(int);
            }

            set
            {
                if (NoCallsTimeoutSetInt32 != null)
                {
                    NoCallsTimeoutSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).NoCallsTimeout = value;
                    return;
                }

                if (NoCallsTimeoutGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _NoCallsTimeout = value;
                }

            }
        }

        private int _NormalConnectionThresholdMs;
        public Func<int> NormalConnectionThresholdMsGet;
        public Action<int> NormalConnectionThresholdMsSetInt32;

        int IConsoleSettings.NormalConnectionThresholdMs
        {
            get
            {
                if (NormalConnectionThresholdMsGet != null)
                {
                    return NormalConnectionThresholdMsGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).NormalConnectionThresholdMs;
                }

                if (NormalConnectionThresholdMsSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _NormalConnectionThresholdMs;
                }

                return default(int);
            }

            set
            {
                if (NormalConnectionThresholdMsSetInt32 != null)
                {
                    NormalConnectionThresholdMsSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).NormalConnectionThresholdMs = value;
                    return;
                }

                if (NormalConnectionThresholdMsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _NormalConnectionThresholdMs = value;
                }

            }
        }

        private bool _OrderInterviewsByPriority;
        public Func<bool> OrderInterviewsByPriorityGet;
        public Action<bool> OrderInterviewsByPrioritySetBoolean;

        bool IConsoleSettings.OrderInterviewsByPriority
        {
            get
            {
                if (OrderInterviewsByPriorityGet != null)
                {
                    return OrderInterviewsByPriorityGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).OrderInterviewsByPriority;
                }

                if (OrderInterviewsByPrioritySetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _OrderInterviewsByPriority;
                }

                return default(bool);
            }

            set
            {
                if (OrderInterviewsByPrioritySetBoolean != null)
                {
                    OrderInterviewsByPrioritySetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).OrderInterviewsByPriority = value;
                    return;
                }

                if (OrderInterviewsByPriorityGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _OrderInterviewsByPriority = value;
                }

            }
        }

        private int _RandomizationInterviewCount;
        public Func<int> RandomizationInterviewCountGet;
        public Action<int> RandomizationInterviewCountSetInt32;

        int IConsoleSettings.RandomizationInterviewCount
        {
            get
            {
                if (RandomizationInterviewCountGet != null)
                {
                    return RandomizationInterviewCountGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).RandomizationInterviewCount;
                }

                if (RandomizationInterviewCountSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _RandomizationInterviewCount;
                }

                return default(int);
            }

            set
            {
                if (RandomizationInterviewCountSetInt32 != null)
                {
                    RandomizationInterviewCountSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).RandomizationInterviewCount = value;
                    return;
                }

                if (RandomizationInterviewCountGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _RandomizationInterviewCount = value;
                }

            }
        }

        private bool _ShowRedialButtonSetting;
        public Func<bool> ShowRedialButtonSettingGet;
        public Action<bool> ShowRedialButtonSettingSetBoolean;

        bool IConsoleSettings.ShowRedialButtonSetting
        {
            get
            {
                if (ShowRedialButtonSettingGet != null)
                {
                    return ShowRedialButtonSettingGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).ShowRedialButtonSetting;
                }

                if (ShowRedialButtonSettingSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ShowRedialButtonSetting;
                }

                return default(bool);
            }

            set
            {
                if (ShowRedialButtonSettingSetBoolean != null)
                {
                    ShowRedialButtonSettingSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).ShowRedialButtonSetting = value;
                    return;
                }

                if (ShowRedialButtonSettingGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ShowRedialButtonSetting = value;
                }

            }
        }

        private int _StateServiceSessionTimeoutInMinutes;
        public Func<int> StateServiceSessionTimeoutInMinutesGet;
        public Action<int> StateServiceSessionTimeoutInMinutesSetInt32;

        int IConsoleSettings.StateServiceSessionTimeoutInMinutes
        {
            get
            {
                if (StateServiceSessionTimeoutInMinutesGet != null)
                {
                    return StateServiceSessionTimeoutInMinutesGet();
                } else if (_inner != null)
                {
                    return ((IConsoleSettings)_inner).StateServiceSessionTimeoutInMinutes;
                }

                if (StateServiceSessionTimeoutInMinutesSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _StateServiceSessionTimeoutInMinutes;
                }

                return default(int);
            }

            set
            {
                if (StateServiceSessionTimeoutInMinutesSetInt32 != null)
                {
                    StateServiceSessionTimeoutInMinutesSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IConsoleSettings)_inner).StateServiceSessionTimeoutInMinutes = value;
                    return;
                }

                if (StateServiceSessionTimeoutInMinutesGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _StateServiceSessionTimeoutInMinutes = value;
                }

            }
        }

    }
}