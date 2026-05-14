using System;
using Confirmit.CATI.Core.SystemSettings.Alerting;

namespace Confirmit.CATI.Core.SystemSettings.Alerting.Fakes
{
    public class StubIWaitingStateSettings : IWaitingStateSettings 
    {
        private IWaitingStateSettings _inner;

        public StubIWaitingStateSettings()
        {
            _inner = null;
        }

        public IWaitingStateSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private string _ExcludedInterviewerGroups;
        public Func<string> ExcludedInterviewerGroupsGet;
        public Action<string> ExcludedInterviewerGroupsSetString;

        string IWaitingStateSettings.ExcludedInterviewerGroups
        {
            get
            {
                if (ExcludedInterviewerGroupsGet != null)
                {
                    return ExcludedInterviewerGroupsGet();
                } else if (_inner != null)
                {
                    return ((IWaitingStateSettings)_inner).ExcludedInterviewerGroups;
                }

                if (ExcludedInterviewerGroupsSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ExcludedInterviewerGroups;
                }

                return default(string);
            }

            set
            {
                if (ExcludedInterviewerGroupsSetString != null)
                {
                    ExcludedInterviewerGroupsSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IWaitingStateSettings)_inner).ExcludedInterviewerGroups = value;
                    return;
                }

                if (ExcludedInterviewerGroupsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ExcludedInterviewerGroups = value;
                }

            }
        }

        private bool _IsAlertEnabled;
        public Func<bool> IsAlertEnabledGet;
        public Action<bool> IsAlertEnabledSetBoolean;

        bool IWaitingStateSettings.IsAlertEnabled
        {
            get
            {
                if (IsAlertEnabledGet != null)
                {
                    return IsAlertEnabledGet();
                } else if (_inner != null)
                {
                    return ((IWaitingStateSettings)_inner).IsAlertEnabled;
                }

                if (IsAlertEnabledSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsAlertEnabled;
                }

                return default(bool);
            }

            set
            {
                if (IsAlertEnabledSetBoolean != null)
                {
                    IsAlertEnabledSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IWaitingStateSettings)_inner).IsAlertEnabled = value;
                    return;
                }

                if (IsAlertEnabledGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _IsAlertEnabled = value;
                }

            }
        }

        private TimeSpan _NotificationFrequency;
        public Func<TimeSpan> NotificationFrequencyGet;
        public Action<TimeSpan> NotificationFrequencySetTimeSpan;

        TimeSpan IWaitingStateSettings.NotificationFrequency
        {
            get
            {
                if (NotificationFrequencyGet != null)
                {
                    return NotificationFrequencyGet();
                } else if (_inner != null)
                {
                    return ((IWaitingStateSettings)_inner).NotificationFrequency;
                }

                if (NotificationFrequencySetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _NotificationFrequency;
                }

                return default(TimeSpan);
            }

            set
            {
                if (NotificationFrequencySetTimeSpan != null)
                {
                    NotificationFrequencySetTimeSpan(value);
                    return;
                } else if (_inner != null)
                {
                    ((IWaitingStateSettings)_inner).NotificationFrequency = value;
                    return;
                }

                if (NotificationFrequencyGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _NotificationFrequency = value;
                }

            }
        }

        private int _NumberOfInterviewers;
        public Func<int> NumberOfInterviewersGet;
        public Action<int> NumberOfInterviewersSetInt32;

        int IWaitingStateSettings.NumberOfInterviewers
        {
            get
            {
                if (NumberOfInterviewersGet != null)
                {
                    return NumberOfInterviewersGet();
                } else if (_inner != null)
                {
                    return ((IWaitingStateSettings)_inner).NumberOfInterviewers;
                }

                if (NumberOfInterviewersSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _NumberOfInterviewers;
                }

                return default(int);
            }

            set
            {
                if (NumberOfInterviewersSetInt32 != null)
                {
                    NumberOfInterviewersSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IWaitingStateSettings)_inner).NumberOfInterviewers = value;
                    return;
                }

                if (NumberOfInterviewersGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _NumberOfInterviewers = value;
                }

            }
        }

        private int _NumberOfMinutes;
        public Func<int> NumberOfMinutesGet;
        public Action<int> NumberOfMinutesSetInt32;

        int IWaitingStateSettings.NumberOfMinutes
        {
            get
            {
                if (NumberOfMinutesGet != null)
                {
                    return NumberOfMinutesGet();
                } else if (_inner != null)
                {
                    return ((IWaitingStateSettings)_inner).NumberOfMinutes;
                }

                if (NumberOfMinutesSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _NumberOfMinutes;
                }

                return default(int);
            }

            set
            {
                if (NumberOfMinutesSetInt32 != null)
                {
                    NumberOfMinutesSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IWaitingStateSettings)_inner).NumberOfMinutes = value;
                    return;
                }

                if (NumberOfMinutesGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _NumberOfMinutes = value;
                }

            }
        }

    }
}