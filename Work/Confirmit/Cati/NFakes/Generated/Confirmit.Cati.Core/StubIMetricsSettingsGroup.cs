using System;
using Confirmit.CATI.Core.SystemSettings.Console;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Console.Fakes
{
    public class StubIMetricsSettingsGroup : IMetricsSettingsGroup 
    {
        private IMetricsSettingsGroup _inner;

        public StubIMetricsSettingsGroup()
        {
            _inner = null;
        }

        public IMetricsSettingsGroup Inner
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

        private bool _EnableAppointmentsMade;
        public Func<bool> EnableAppointmentsMadeGet;
        public Action<bool> EnableAppointmentsMadeSetBoolean;

        bool IMetricsSettings.EnableAppointmentsMade
        {
            get
            {
                if (EnableAppointmentsMadeGet != null)
                {
                    return EnableAppointmentsMadeGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableAppointmentsMade;
                }

                if (EnableAppointmentsMadeSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableAppointmentsMade;
                }

                return default(bool);
            }

            set
            {
                if (EnableAppointmentsMadeSetBoolean != null)
                {
                    EnableAppointmentsMadeSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableAppointmentsMade = value;
                    return;
                }

                if (EnableAppointmentsMadeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableAppointmentsMade = value;
                }

            }
        }

        private bool _EnableAverageCallAttemptsPerHour;
        public Func<bool> EnableAverageCallAttemptsPerHourGet;
        public Action<bool> EnableAverageCallAttemptsPerHourSetBoolean;

        bool IMetricsSettings.EnableAverageCallAttemptsPerHour
        {
            get
            {
                if (EnableAverageCallAttemptsPerHourGet != null)
                {
                    return EnableAverageCallAttemptsPerHourGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableAverageCallAttemptsPerHour;
                }

                if (EnableAverageCallAttemptsPerHourSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableAverageCallAttemptsPerHour;
                }

                return default(bool);
            }

            set
            {
                if (EnableAverageCallAttemptsPerHourSetBoolean != null)
                {
                    EnableAverageCallAttemptsPerHourSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableAverageCallAttemptsPerHour = value;
                    return;
                }

                if (EnableAverageCallAttemptsPerHourGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableAverageCallAttemptsPerHour = value;
                }

            }
        }

        private bool _EnableAverageCompletedInterviewsPerHour;
        public Func<bool> EnableAverageCompletedInterviewsPerHourGet;
        public Action<bool> EnableAverageCompletedInterviewsPerHourSetBoolean;

        bool IMetricsSettings.EnableAverageCompletedInterviewsPerHour
        {
            get
            {
                if (EnableAverageCompletedInterviewsPerHourGet != null)
                {
                    return EnableAverageCompletedInterviewsPerHourGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableAverageCompletedInterviewsPerHour;
                }

                if (EnableAverageCompletedInterviewsPerHourSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableAverageCompletedInterviewsPerHour;
                }

                return default(bool);
            }

            set
            {
                if (EnableAverageCompletedInterviewsPerHourSetBoolean != null)
                {
                    EnableAverageCompletedInterviewsPerHourSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableAverageCompletedInterviewsPerHour = value;
                    return;
                }

                if (EnableAverageCompletedInterviewsPerHourGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableAverageCompletedInterviewsPerHour = value;
                }

            }
        }

        private bool _EnableAverageConnectedCallTime;
        public Func<bool> EnableAverageConnectedCallTimeGet;
        public Action<bool> EnableAverageConnectedCallTimeSetBoolean;

        bool IMetricsSettings.EnableAverageConnectedCallTime
        {
            get
            {
                if (EnableAverageConnectedCallTimeGet != null)
                {
                    return EnableAverageConnectedCallTimeGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableAverageConnectedCallTime;
                }

                if (EnableAverageConnectedCallTimeSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableAverageConnectedCallTime;
                }

                return default(bool);
            }

            set
            {
                if (EnableAverageConnectedCallTimeSetBoolean != null)
                {
                    EnableAverageConnectedCallTimeSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableAverageConnectedCallTime = value;
                    return;
                }

                if (EnableAverageConnectedCallTimeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableAverageConnectedCallTime = value;
                }

            }
        }

        private bool _EnableAverageWrapTime;
        public Func<bool> EnableAverageWrapTimeGet;
        public Action<bool> EnableAverageWrapTimeSetBoolean;

        bool IMetricsSettings.EnableAverageWrapTime
        {
            get
            {
                if (EnableAverageWrapTimeGet != null)
                {
                    return EnableAverageWrapTimeGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableAverageWrapTime;
                }

                if (EnableAverageWrapTimeSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableAverageWrapTime;
                }

                return default(bool);
            }

            set
            {
                if (EnableAverageWrapTimeSetBoolean != null)
                {
                    EnableAverageWrapTimeSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableAverageWrapTime = value;
                    return;
                }

                if (EnableAverageWrapTimeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableAverageWrapTime = value;
                }

            }
        }

        private bool _EnableBreakDuration;
        public Func<bool> EnableBreakDurationGet;
        public Action<bool> EnableBreakDurationSetBoolean;

        bool IMetricsSettings.EnableBreakDuration
        {
            get
            {
                if (EnableBreakDurationGet != null)
                {
                    return EnableBreakDurationGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableBreakDuration;
                }

                if (EnableBreakDurationSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableBreakDuration;
                }

                return default(bool);
            }

            set
            {
                if (EnableBreakDurationSetBoolean != null)
                {
                    EnableBreakDurationSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableBreakDuration = value;
                    return;
                }

                if (EnableBreakDurationGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableBreakDuration = value;
                }

            }
        }

        private bool _EnableCallAttempts;
        public Func<bool> EnableCallAttemptsGet;
        public Action<bool> EnableCallAttemptsSetBoolean;

        bool IMetricsSettings.EnableCallAttempts
        {
            get
            {
                if (EnableCallAttemptsGet != null)
                {
                    return EnableCallAttemptsGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableCallAttempts;
                }

                if (EnableCallAttemptsSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableCallAttempts;
                }

                return default(bool);
            }

            set
            {
                if (EnableCallAttemptsSetBoolean != null)
                {
                    EnableCallAttemptsSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableCallAttempts = value;
                    return;
                }

                if (EnableCallAttemptsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableCallAttempts = value;
                }

            }
        }

        private bool _EnableCallAttemptsPerComplete;
        public Func<bool> EnableCallAttemptsPerCompleteGet;
        public Action<bool> EnableCallAttemptsPerCompleteSetBoolean;

        bool IMetricsSettings.EnableCallAttemptsPerComplete
        {
            get
            {
                if (EnableCallAttemptsPerCompleteGet != null)
                {
                    return EnableCallAttemptsPerCompleteGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableCallAttemptsPerComplete;
                }

                if (EnableCallAttemptsPerCompleteSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableCallAttemptsPerComplete;
                }

                return default(bool);
            }

            set
            {
                if (EnableCallAttemptsPerCompleteSetBoolean != null)
                {
                    EnableCallAttemptsPerCompleteSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableCallAttemptsPerComplete = value;
                    return;
                }

                if (EnableCallAttemptsPerCompleteGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableCallAttemptsPerComplete = value;
                }

            }
        }

        private bool _EnableCallAttemptsPerCompleteAboveAverageComparison;
        public Func<bool> EnableCallAttemptsPerCompleteAboveAverageComparisonGet;
        public Action<bool> EnableCallAttemptsPerCompleteAboveAverageComparisonSetBoolean;

        bool IMetricsSettings.EnableCallAttemptsPerCompleteAboveAverageComparison
        {
            get
            {
                if (EnableCallAttemptsPerCompleteAboveAverageComparisonGet != null)
                {
                    return EnableCallAttemptsPerCompleteAboveAverageComparisonGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableCallAttemptsPerCompleteAboveAverageComparison;
                }

                if (EnableCallAttemptsPerCompleteAboveAverageComparisonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableCallAttemptsPerCompleteAboveAverageComparison;
                }

                return default(bool);
            }

            set
            {
                if (EnableCallAttemptsPerCompleteAboveAverageComparisonSetBoolean != null)
                {
                    EnableCallAttemptsPerCompleteAboveAverageComparisonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableCallAttemptsPerCompleteAboveAverageComparison = value;
                    return;
                }

                if (EnableCallAttemptsPerCompleteAboveAverageComparisonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableCallAttemptsPerCompleteAboveAverageComparison = value;
                }

            }
        }

        private bool _EnableCallAttemptsPerCompleteBelowAverageComparison;
        public Func<bool> EnableCallAttemptsPerCompleteBelowAverageComparisonGet;
        public Action<bool> EnableCallAttemptsPerCompleteBelowAverageComparisonSetBoolean;

        bool IMetricsSettings.EnableCallAttemptsPerCompleteBelowAverageComparison
        {
            get
            {
                if (EnableCallAttemptsPerCompleteBelowAverageComparisonGet != null)
                {
                    return EnableCallAttemptsPerCompleteBelowAverageComparisonGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableCallAttemptsPerCompleteBelowAverageComparison;
                }

                if (EnableCallAttemptsPerCompleteBelowAverageComparisonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableCallAttemptsPerCompleteBelowAverageComparison;
                }

                return default(bool);
            }

            set
            {
                if (EnableCallAttemptsPerCompleteBelowAverageComparisonSetBoolean != null)
                {
                    EnableCallAttemptsPerCompleteBelowAverageComparisonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableCallAttemptsPerCompleteBelowAverageComparison = value;
                    return;
                }

                if (EnableCallAttemptsPerCompleteBelowAverageComparisonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableCallAttemptsPerCompleteBelowAverageComparison = value;
                }

            }
        }

        private bool _EnableCallAttemptsPerHour;
        public Func<bool> EnableCallAttemptsPerHourGet;
        public Action<bool> EnableCallAttemptsPerHourSetBoolean;

        bool IMetricsSettings.EnableCallAttemptsPerHour
        {
            get
            {
                if (EnableCallAttemptsPerHourGet != null)
                {
                    return EnableCallAttemptsPerHourGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableCallAttemptsPerHour;
                }

                if (EnableCallAttemptsPerHourSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableCallAttemptsPerHour;
                }

                return default(bool);
            }

            set
            {
                if (EnableCallAttemptsPerHourSetBoolean != null)
                {
                    EnableCallAttemptsPerHourSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableCallAttemptsPerHour = value;
                    return;
                }

                if (EnableCallAttemptsPerHourGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableCallAttemptsPerHour = value;
                }

            }
        }

        private bool _EnableCallAttemptsPerHourAboveAverageComparison;
        public Func<bool> EnableCallAttemptsPerHourAboveAverageComparisonGet;
        public Action<bool> EnableCallAttemptsPerHourAboveAverageComparisonSetBoolean;

        bool IMetricsSettings.EnableCallAttemptsPerHourAboveAverageComparison
        {
            get
            {
                if (EnableCallAttemptsPerHourAboveAverageComparisonGet != null)
                {
                    return EnableCallAttemptsPerHourAboveAverageComparisonGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableCallAttemptsPerHourAboveAverageComparison;
                }

                if (EnableCallAttemptsPerHourAboveAverageComparisonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableCallAttemptsPerHourAboveAverageComparison;
                }

                return default(bool);
            }

            set
            {
                if (EnableCallAttemptsPerHourAboveAverageComparisonSetBoolean != null)
                {
                    EnableCallAttemptsPerHourAboveAverageComparisonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableCallAttemptsPerHourAboveAverageComparison = value;
                    return;
                }

                if (EnableCallAttemptsPerHourAboveAverageComparisonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableCallAttemptsPerHourAboveAverageComparison = value;
                }

            }
        }

        private bool _EnableCallAttemptsPerHourBelowAverageComparison;
        public Func<bool> EnableCallAttemptsPerHourBelowAverageComparisonGet;
        public Action<bool> EnableCallAttemptsPerHourBelowAverageComparisonSetBoolean;

        bool IMetricsSettings.EnableCallAttemptsPerHourBelowAverageComparison
        {
            get
            {
                if (EnableCallAttemptsPerHourBelowAverageComparisonGet != null)
                {
                    return EnableCallAttemptsPerHourBelowAverageComparisonGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableCallAttemptsPerHourBelowAverageComparison;
                }

                if (EnableCallAttemptsPerHourBelowAverageComparisonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableCallAttemptsPerHourBelowAverageComparison;
                }

                return default(bool);
            }

            set
            {
                if (EnableCallAttemptsPerHourBelowAverageComparisonSetBoolean != null)
                {
                    EnableCallAttemptsPerHourBelowAverageComparisonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableCallAttemptsPerHourBelowAverageComparison = value;
                    return;
                }

                if (EnableCallAttemptsPerHourBelowAverageComparisonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableCallAttemptsPerHourBelowAverageComparison = value;
                }

            }
        }

        private bool _EnableCallConnectedTime;
        public Func<bool> EnableCallConnectedTimeGet;
        public Action<bool> EnableCallConnectedTimeSetBoolean;

        bool IMetricsSettings.EnableCallConnectedTime
        {
            get
            {
                if (EnableCallConnectedTimeGet != null)
                {
                    return EnableCallConnectedTimeGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableCallConnectedTime;
                }

                if (EnableCallConnectedTimeSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableCallConnectedTime;
                }

                return default(bool);
            }

            set
            {
                if (EnableCallConnectedTimeSetBoolean != null)
                {
                    EnableCallConnectedTimeSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableCallConnectedTime = value;
                    return;
                }

                if (EnableCallConnectedTimeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableCallConnectedTime = value;
                }

            }
        }

        private bool _EnableCallsConnected;
        public Func<bool> EnableCallsConnectedGet;
        public Action<bool> EnableCallsConnectedSetBoolean;

        bool IMetricsSettings.EnableCallsConnected
        {
            get
            {
                if (EnableCallsConnectedGet != null)
                {
                    return EnableCallsConnectedGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableCallsConnected;
                }

                if (EnableCallsConnectedSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableCallsConnected;
                }

                return default(bool);
            }

            set
            {
                if (EnableCallsConnectedSetBoolean != null)
                {
                    EnableCallsConnectedSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableCallsConnected = value;
                    return;
                }

                if (EnableCallsConnectedGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableCallsConnected = value;
                }

            }
        }

        private bool _EnableInterviewerMetrics;
        public Func<bool> EnableInterviewerMetricsGet;
        public Action<bool> EnableInterviewerMetricsSetBoolean;

        bool IMetricsSettings.EnableInterviewerMetrics
        {
            get
            {
                if (EnableInterviewerMetricsGet != null)
                {
                    return EnableInterviewerMetricsGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableInterviewerMetrics;
                }

                if (EnableInterviewerMetricsSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableInterviewerMetrics;
                }

                return default(bool);
            }

            set
            {
                if (EnableInterviewerMetricsSetBoolean != null)
                {
                    EnableInterviewerMetricsSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableInterviewerMetrics = value;
                    return;
                }

                if (EnableInterviewerMetricsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableInterviewerMetrics = value;
                }

            }
        }

        private bool _EnableInterviewsCompleted;
        public Func<bool> EnableInterviewsCompletedGet;
        public Action<bool> EnableInterviewsCompletedSetBoolean;

        bool IMetricsSettings.EnableInterviewsCompleted
        {
            get
            {
                if (EnableInterviewsCompletedGet != null)
                {
                    return EnableInterviewsCompletedGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableInterviewsCompleted;
                }

                if (EnableInterviewsCompletedSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableInterviewsCompleted;
                }

                return default(bool);
            }

            set
            {
                if (EnableInterviewsCompletedSetBoolean != null)
                {
                    EnableInterviewsCompletedSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableInterviewsCompleted = value;
                    return;
                }

                if (EnableInterviewsCompletedGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableInterviewsCompleted = value;
                }

            }
        }

        private bool _EnableInterviewsCompletedPerHour;
        public Func<bool> EnableInterviewsCompletedPerHourGet;
        public Action<bool> EnableInterviewsCompletedPerHourSetBoolean;

        bool IMetricsSettings.EnableInterviewsCompletedPerHour
        {
            get
            {
                if (EnableInterviewsCompletedPerHourGet != null)
                {
                    return EnableInterviewsCompletedPerHourGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableInterviewsCompletedPerHour;
                }

                if (EnableInterviewsCompletedPerHourSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableInterviewsCompletedPerHour;
                }

                return default(bool);
            }

            set
            {
                if (EnableInterviewsCompletedPerHourSetBoolean != null)
                {
                    EnableInterviewsCompletedPerHourSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableInterviewsCompletedPerHour = value;
                    return;
                }

                if (EnableInterviewsCompletedPerHourGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableInterviewsCompletedPerHour = value;
                }

            }
        }

        private bool _EnableInterviewsCompletedPerHourAboveAverageComparison;
        public Func<bool> EnableInterviewsCompletedPerHourAboveAverageComparisonGet;
        public Action<bool> EnableInterviewsCompletedPerHourAboveAverageComparisonSetBoolean;

        bool IMetricsSettings.EnableInterviewsCompletedPerHourAboveAverageComparison
        {
            get
            {
                if (EnableInterviewsCompletedPerHourAboveAverageComparisonGet != null)
                {
                    return EnableInterviewsCompletedPerHourAboveAverageComparisonGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableInterviewsCompletedPerHourAboveAverageComparison;
                }

                if (EnableInterviewsCompletedPerHourAboveAverageComparisonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableInterviewsCompletedPerHourAboveAverageComparison;
                }

                return default(bool);
            }

            set
            {
                if (EnableInterviewsCompletedPerHourAboveAverageComparisonSetBoolean != null)
                {
                    EnableInterviewsCompletedPerHourAboveAverageComparisonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableInterviewsCompletedPerHourAboveAverageComparison = value;
                    return;
                }

                if (EnableInterviewsCompletedPerHourAboveAverageComparisonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableInterviewsCompletedPerHourAboveAverageComparison = value;
                }

            }
        }

        private bool _EnableInterviewsCompletedPerHourBelowAverageComparison;
        public Func<bool> EnableInterviewsCompletedPerHourBelowAverageComparisonGet;
        public Action<bool> EnableInterviewsCompletedPerHourBelowAverageComparisonSetBoolean;

        bool IMetricsSettings.EnableInterviewsCompletedPerHourBelowAverageComparison
        {
            get
            {
                if (EnableInterviewsCompletedPerHourBelowAverageComparisonGet != null)
                {
                    return EnableInterviewsCompletedPerHourBelowAverageComparisonGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableInterviewsCompletedPerHourBelowAverageComparison;
                }

                if (EnableInterviewsCompletedPerHourBelowAverageComparisonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableInterviewsCompletedPerHourBelowAverageComparison;
                }

                return default(bool);
            }

            set
            {
                if (EnableInterviewsCompletedPerHourBelowAverageComparisonSetBoolean != null)
                {
                    EnableInterviewsCompletedPerHourBelowAverageComparisonSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableInterviewsCompletedPerHourBelowAverageComparison = value;
                    return;
                }

                if (EnableInterviewsCompletedPerHourBelowAverageComparisonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableInterviewsCompletedPerHourBelowAverageComparison = value;
                }

            }
        }

        private bool _EnableLoginSessionDuration;
        public Func<bool> EnableLoginSessionDurationGet;
        public Action<bool> EnableLoginSessionDurationSetBoolean;

        bool IMetricsSettings.EnableLoginSessionDuration
        {
            get
            {
                if (EnableLoginSessionDurationGet != null)
                {
                    return EnableLoginSessionDurationGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableLoginSessionDuration;
                }

                if (EnableLoginSessionDurationSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableLoginSessionDuration;
                }

                return default(bool);
            }

            set
            {
                if (EnableLoginSessionDurationSetBoolean != null)
                {
                    EnableLoginSessionDurationSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableLoginSessionDuration = value;
                    return;
                }

                if (EnableLoginSessionDurationGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableLoginSessionDuration = value;
                }

            }
        }

        private bool _EnableRefusals;
        public Func<bool> EnableRefusalsGet;
        public Action<bool> EnableRefusalsSetBoolean;

        bool IMetricsSettings.EnableRefusals
        {
            get
            {
                if (EnableRefusalsGet != null)
                {
                    return EnableRefusalsGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableRefusals;
                }

                if (EnableRefusalsSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableRefusals;
                }

                return default(bool);
            }

            set
            {
                if (EnableRefusalsSetBoolean != null)
                {
                    EnableRefusalsSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableRefusals = value;
                    return;
                }

                if (EnableRefusalsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableRefusals = value;
                }

            }
        }

        private bool _EnableTotalCompletedInterviews;
        public Func<bool> EnableTotalCompletedInterviewsGet;
        public Action<bool> EnableTotalCompletedInterviewsSetBoolean;

        bool IMetricsSettings.EnableTotalCompletedInterviews
        {
            get
            {
                if (EnableTotalCompletedInterviewsGet != null)
                {
                    return EnableTotalCompletedInterviewsGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableTotalCompletedInterviews;
                }

                if (EnableTotalCompletedInterviewsSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableTotalCompletedInterviews;
                }

                return default(bool);
            }

            set
            {
                if (EnableTotalCompletedInterviewsSetBoolean != null)
                {
                    EnableTotalCompletedInterviewsSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableTotalCompletedInterviews = value;
                    return;
                }

                if (EnableTotalCompletedInterviewsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableTotalCompletedInterviews = value;
                }

            }
        }

        private bool _EnableTotalInterviewingTime;
        public Func<bool> EnableTotalInterviewingTimeGet;
        public Action<bool> EnableTotalInterviewingTimeSetBoolean;

        bool IMetricsSettings.EnableTotalInterviewingTime
        {
            get
            {
                if (EnableTotalInterviewingTimeGet != null)
                {
                    return EnableTotalInterviewingTimeGet();
                } else if (_inner != null)
                {
                    return ((IMetricsSettings)_inner).EnableTotalInterviewingTime;
                }

                if (EnableTotalInterviewingTimeSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableTotalInterviewingTime;
                }

                return default(bool);
            }

            set
            {
                if (EnableTotalInterviewingTimeSetBoolean != null)
                {
                    EnableTotalInterviewingTimeSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMetricsSettings)_inner).EnableTotalInterviewingTime = value;
                    return;
                }

                if (EnableTotalInterviewingTimeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableTotalInterviewingTime = value;
                }

            }
        }

    }
}