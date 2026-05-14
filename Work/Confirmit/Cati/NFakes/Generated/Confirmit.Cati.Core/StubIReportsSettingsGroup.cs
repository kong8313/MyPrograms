using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIReportsSettingsGroup : IReportsSettingsGroup 
    {
        private IReportsSettingsGroup _inner;

        public StubIReportsSettingsGroup()
        {
            _inner = null;
        }

        public IReportsSettingsGroup Inner
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

        private int _CallHistoryReportCallHistoryRowsLimit;
        public Func<int> CallHistoryReportCallHistoryRowsLimitGet;
        public Action<int> CallHistoryReportCallHistoryRowsLimitSetInt32;

        int IReportsSettings.CallHistoryReportCallHistoryRowsLimit
        {
            get
            {
                if (CallHistoryReportCallHistoryRowsLimitGet != null)
                {
                    return CallHistoryReportCallHistoryRowsLimitGet();
                } else if (_inner != null)
                {
                    return ((IReportsSettings)_inner).CallHistoryReportCallHistoryRowsLimit;
                }

                if (CallHistoryReportCallHistoryRowsLimitSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CallHistoryReportCallHistoryRowsLimit;
                }

                return default(int);
            }

            set
            {
                if (CallHistoryReportCallHistoryRowsLimitSetInt32 != null)
                {
                    CallHistoryReportCallHistoryRowsLimitSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReportsSettings)_inner).CallHistoryReportCallHistoryRowsLimit = value;
                    return;
                }

                if (CallHistoryReportCallHistoryRowsLimitGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _CallHistoryReportCallHistoryRowsLimit = value;
                }

            }
        }

        private bool _CallHistoryReportEnabled;
        public Func<bool> CallHistoryReportEnabledGet;
        public Action<bool> CallHistoryReportEnabledSetBoolean;

        bool IReportsSettings.CallHistoryReportEnabled
        {
            get
            {
                if (CallHistoryReportEnabledGet != null)
                {
                    return CallHistoryReportEnabledGet();
                } else if (_inner != null)
                {
                    return ((IReportsSettings)_inner).CallHistoryReportEnabled;
                }

                if (CallHistoryReportEnabledSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CallHistoryReportEnabled;
                }

                return default(bool);
            }

            set
            {
                if (CallHistoryReportEnabledSetBoolean != null)
                {
                    CallHistoryReportEnabledSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReportsSettings)_inner).CallHistoryReportEnabled = value;
                    return;
                }

                if (CallHistoryReportEnabledGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _CallHistoryReportEnabled = value;
                }

            }
        }

        private int _CallHistoryReportHour;
        public Func<int> CallHistoryReportHourGet;
        public Action<int> CallHistoryReportHourSetInt32;

        int IReportsSettings.CallHistoryReportHour
        {
            get
            {
                if (CallHistoryReportHourGet != null)
                {
                    return CallHistoryReportHourGet();
                } else if (_inner != null)
                {
                    return ((IReportsSettings)_inner).CallHistoryReportHour;
                }

                if (CallHistoryReportHourSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CallHistoryReportHour;
                }

                return default(int);
            }

            set
            {
                if (CallHistoryReportHourSetInt32 != null)
                {
                    CallHistoryReportHourSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReportsSettings)_inner).CallHistoryReportHour = value;
                    return;
                }

                if (CallHistoryReportHourGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _CallHistoryReportHour = value;
                }

            }
        }

        private int _CallHistoryReportInterviewerBreaksRowsLimit;
        public Func<int> CallHistoryReportInterviewerBreaksRowsLimitGet;
        public Action<int> CallHistoryReportInterviewerBreaksRowsLimitSetInt32;

        int IReportsSettings.CallHistoryReportInterviewerBreaksRowsLimit
        {
            get
            {
                if (CallHistoryReportInterviewerBreaksRowsLimitGet != null)
                {
                    return CallHistoryReportInterviewerBreaksRowsLimitGet();
                } else if (_inner != null)
                {
                    return ((IReportsSettings)_inner).CallHistoryReportInterviewerBreaksRowsLimit;
                }

                if (CallHistoryReportInterviewerBreaksRowsLimitSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CallHistoryReportInterviewerBreaksRowsLimit;
                }

                return default(int);
            }

            set
            {
                if (CallHistoryReportInterviewerBreaksRowsLimitSetInt32 != null)
                {
                    CallHistoryReportInterviewerBreaksRowsLimitSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReportsSettings)_inner).CallHistoryReportInterviewerBreaksRowsLimit = value;
                    return;
                }

                if (CallHistoryReportInterviewerBreaksRowsLimitGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _CallHistoryReportInterviewerBreaksRowsLimit = value;
                }

            }
        }

        private int _CallHistoryReportLoginLogoutEventsRowsLimit;
        public Func<int> CallHistoryReportLoginLogoutEventsRowsLimitGet;
        public Action<int> CallHistoryReportLoginLogoutEventsRowsLimitSetInt32;

        int IReportsSettings.CallHistoryReportLoginLogoutEventsRowsLimit
        {
            get
            {
                if (CallHistoryReportLoginLogoutEventsRowsLimitGet != null)
                {
                    return CallHistoryReportLoginLogoutEventsRowsLimitGet();
                } else if (_inner != null)
                {
                    return ((IReportsSettings)_inner).CallHistoryReportLoginLogoutEventsRowsLimit;
                }

                if (CallHistoryReportLoginLogoutEventsRowsLimitSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CallHistoryReportLoginLogoutEventsRowsLimit;
                }

                return default(int);
            }

            set
            {
                if (CallHistoryReportLoginLogoutEventsRowsLimitSetInt32 != null)
                {
                    CallHistoryReportLoginLogoutEventsRowsLimitSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReportsSettings)_inner).CallHistoryReportLoginLogoutEventsRowsLimit = value;
                    return;
                }

                if (CallHistoryReportLoginLogoutEventsRowsLimitGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _CallHistoryReportLoginLogoutEventsRowsLimit = value;
                }

            }
        }

        private string _CallHistoryReportRecepients;
        public Func<string> CallHistoryReportRecepientsGet;
        public Action<string> CallHistoryReportRecepientsSetString;

        string IReportsSettings.CallHistoryReportRecepients
        {
            get
            {
                if (CallHistoryReportRecepientsGet != null)
                {
                    return CallHistoryReportRecepientsGet();
                } else if (_inner != null)
                {
                    return ((IReportsSettings)_inner).CallHistoryReportRecepients;
                }

                if (CallHistoryReportRecepientsSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CallHistoryReportRecepients;
                }

                return default(string);
            }

            set
            {
                if (CallHistoryReportRecepientsSetString != null)
                {
                    CallHistoryReportRecepientsSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReportsSettings)_inner).CallHistoryReportRecepients = value;
                    return;
                }

                if (CallHistoryReportRecepientsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _CallHistoryReportRecepients = value;
                }

            }
        }

        private string _CallHistoryReportReplicatedVariables;
        public Func<string> CallHistoryReportReplicatedVariablesGet;
        public Action<string> CallHistoryReportReplicatedVariablesSetString;

        string IReportsSettings.CallHistoryReportReplicatedVariables
        {
            get
            {
                if (CallHistoryReportReplicatedVariablesGet != null)
                {
                    return CallHistoryReportReplicatedVariablesGet();
                } else if (_inner != null)
                {
                    return ((IReportsSettings)_inner).CallHistoryReportReplicatedVariables;
                }

                if (CallHistoryReportReplicatedVariablesSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CallHistoryReportReplicatedVariables;
                }

                return default(string);
            }

            set
            {
                if (CallHistoryReportReplicatedVariablesSetString != null)
                {
                    CallHistoryReportReplicatedVariablesSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReportsSettings)_inner).CallHistoryReportReplicatedVariables = value;
                    return;
                }

                if (CallHistoryReportReplicatedVariablesGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _CallHistoryReportReplicatedVariables = value;
                }

            }
        }

        private bool _CallHistoryReportReplicatedVariablesEnabled;
        public Func<bool> CallHistoryReportReplicatedVariablesEnabledGet;
        public Action<bool> CallHistoryReportReplicatedVariablesEnabledSetBoolean;

        bool IReportsSettings.CallHistoryReportReplicatedVariablesEnabled
        {
            get
            {
                if (CallHistoryReportReplicatedVariablesEnabledGet != null)
                {
                    return CallHistoryReportReplicatedVariablesEnabledGet();
                } else if (_inner != null)
                {
                    return ((IReportsSettings)_inner).CallHistoryReportReplicatedVariablesEnabled;
                }

                if (CallHistoryReportReplicatedVariablesEnabledSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CallHistoryReportReplicatedVariablesEnabled;
                }

                return default(bool);
            }

            set
            {
                if (CallHistoryReportReplicatedVariablesEnabledSetBoolean != null)
                {
                    CallHistoryReportReplicatedVariablesEnabledSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReportsSettings)_inner).CallHistoryReportReplicatedVariablesEnabled = value;
                    return;
                }

                if (CallHistoryReportReplicatedVariablesEnabledGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _CallHistoryReportReplicatedVariablesEnabled = value;
                }

            }
        }

        private bool _InterviewerProductivityReportEnabled;
        public Func<bool> InterviewerProductivityReportEnabledGet;
        public Action<bool> InterviewerProductivityReportEnabledSetBoolean;

        bool IReportsSettings.InterviewerProductivityReportEnabled
        {
            get
            {
                if (InterviewerProductivityReportEnabledGet != null)
                {
                    return InterviewerProductivityReportEnabledGet();
                } else if (_inner != null)
                {
                    return ((IReportsSettings)_inner).InterviewerProductivityReportEnabled;
                }

                if (InterviewerProductivityReportEnabledSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InterviewerProductivityReportEnabled;
                }

                return default(bool);
            }

            set
            {
                if (InterviewerProductivityReportEnabledSetBoolean != null)
                {
                    InterviewerProductivityReportEnabledSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReportsSettings)_inner).InterviewerProductivityReportEnabled = value;
                    return;
                }

                if (InterviewerProductivityReportEnabledGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _InterviewerProductivityReportEnabled = value;
                }

            }
        }

        private int _InterviewerProductivityReportHour;
        public Func<int> InterviewerProductivityReportHourGet;
        public Action<int> InterviewerProductivityReportHourSetInt32;

        int IReportsSettings.InterviewerProductivityReportHour
        {
            get
            {
                if (InterviewerProductivityReportHourGet != null)
                {
                    return InterviewerProductivityReportHourGet();
                } else if (_inner != null)
                {
                    return ((IReportsSettings)_inner).InterviewerProductivityReportHour;
                }

                if (InterviewerProductivityReportHourSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InterviewerProductivityReportHour;
                }

                return default(int);
            }

            set
            {
                if (InterviewerProductivityReportHourSetInt32 != null)
                {
                    InterviewerProductivityReportHourSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReportsSettings)_inner).InterviewerProductivityReportHour = value;
                    return;
                }

                if (InterviewerProductivityReportHourGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _InterviewerProductivityReportHour = value;
                }

            }
        }

        private string _InterviewerProductivityReportRecepients;
        public Func<string> InterviewerProductivityReportRecepientsGet;
        public Action<string> InterviewerProductivityReportRecepientsSetString;

        string IReportsSettings.InterviewerProductivityReportRecepients
        {
            get
            {
                if (InterviewerProductivityReportRecepientsGet != null)
                {
                    return InterviewerProductivityReportRecepientsGet();
                } else if (_inner != null)
                {
                    return ((IReportsSettings)_inner).InterviewerProductivityReportRecepients;
                }

                if (InterviewerProductivityReportRecepientsSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InterviewerProductivityReportRecepients;
                }

                return default(string);
            }

            set
            {
                if (InterviewerProductivityReportRecepientsSetString != null)
                {
                    InterviewerProductivityReportRecepientsSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReportsSettings)_inner).InterviewerProductivityReportRecepients = value;
                    return;
                }

                if (InterviewerProductivityReportRecepientsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _InterviewerProductivityReportRecepients = value;
                }

            }
        }

        private int _ReportGenerationTimeout;
        public Func<int> ReportGenerationTimeoutGet;
        public Action<int> ReportGenerationTimeoutSetInt32;

        int IReportsSettings.ReportGenerationTimeout
        {
            get
            {
                if (ReportGenerationTimeoutGet != null)
                {
                    return ReportGenerationTimeoutGet();
                } else if (_inner != null)
                {
                    return ((IReportsSettings)_inner).ReportGenerationTimeout;
                }

                if (ReportGenerationTimeoutSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ReportGenerationTimeout;
                }

                return default(int);
            }

            set
            {
                if (ReportGenerationTimeoutSetInt32 != null)
                {
                    ReportGenerationTimeoutSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReportsSettings)_inner).ReportGenerationTimeout = value;
                    return;
                }

                if (ReportGenerationTimeoutGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ReportGenerationTimeout = value;
                }

            }
        }

        private int _ScheduledInterviewerProductivityReportTemplateId;
        public Func<int> ScheduledInterviewerProductivityReportTemplateIdGet;
        public Action<int> ScheduledInterviewerProductivityReportTemplateIdSetInt32;

        int IReportsSettings.ScheduledInterviewerProductivityReportTemplateId
        {
            get
            {
                if (ScheduledInterviewerProductivityReportTemplateIdGet != null)
                {
                    return ScheduledInterviewerProductivityReportTemplateIdGet();
                } else if (_inner != null)
                {
                    return ((IReportsSettings)_inner).ScheduledInterviewerProductivityReportTemplateId;
                }

                if (ScheduledInterviewerProductivityReportTemplateIdSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ScheduledInterviewerProductivityReportTemplateId;
                }

                return default(int);
            }

            set
            {
                if (ScheduledInterviewerProductivityReportTemplateIdSetInt32 != null)
                {
                    ScheduledInterviewerProductivityReportTemplateIdSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReportsSettings)_inner).ScheduledInterviewerProductivityReportTemplateId = value;
                    return;
                }

                if (ScheduledInterviewerProductivityReportTemplateIdGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ScheduledInterviewerProductivityReportTemplateId = value;
                }

            }
        }

        private bool _SurveyOverviewReportEnabled;
        public Func<bool> SurveyOverviewReportEnabledGet;
        public Action<bool> SurveyOverviewReportEnabledSetBoolean;

        bool IReportsSettings.SurveyOverviewReportEnabled
        {
            get
            {
                if (SurveyOverviewReportEnabledGet != null)
                {
                    return SurveyOverviewReportEnabledGet();
                } else if (_inner != null)
                {
                    return ((IReportsSettings)_inner).SurveyOverviewReportEnabled;
                }

                if (SurveyOverviewReportEnabledSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SurveyOverviewReportEnabled;
                }

                return default(bool);
            }

            set
            {
                if (SurveyOverviewReportEnabledSetBoolean != null)
                {
                    SurveyOverviewReportEnabledSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReportsSettings)_inner).SurveyOverviewReportEnabled = value;
                    return;
                }

                if (SurveyOverviewReportEnabledGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SurveyOverviewReportEnabled = value;
                }

            }
        }

        private int _SurveyOverviewReportHour;
        public Func<int> SurveyOverviewReportHourGet;
        public Action<int> SurveyOverviewReportHourSetInt32;

        int IReportsSettings.SurveyOverviewReportHour
        {
            get
            {
                if (SurveyOverviewReportHourGet != null)
                {
                    return SurveyOverviewReportHourGet();
                } else if (_inner != null)
                {
                    return ((IReportsSettings)_inner).SurveyOverviewReportHour;
                }

                if (SurveyOverviewReportHourSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SurveyOverviewReportHour;
                }

                return default(int);
            }

            set
            {
                if (SurveyOverviewReportHourSetInt32 != null)
                {
                    SurveyOverviewReportHourSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReportsSettings)_inner).SurveyOverviewReportHour = value;
                    return;
                }

                if (SurveyOverviewReportHourGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SurveyOverviewReportHour = value;
                }

            }
        }

        private string _SurveyOverviewReportRecepients;
        public Func<string> SurveyOverviewReportRecepientsGet;
        public Action<string> SurveyOverviewReportRecepientsSetString;

        string IReportsSettings.SurveyOverviewReportRecepients
        {
            get
            {
                if (SurveyOverviewReportRecepientsGet != null)
                {
                    return SurveyOverviewReportRecepientsGet();
                } else if (_inner != null)
                {
                    return ((IReportsSettings)_inner).SurveyOverviewReportRecepients;
                }

                if (SurveyOverviewReportRecepientsSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SurveyOverviewReportRecepients;
                }

                return default(string);
            }

            set
            {
                if (SurveyOverviewReportRecepientsSetString != null)
                {
                    SurveyOverviewReportRecepientsSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReportsSettings)_inner).SurveyOverviewReportRecepients = value;
                    return;
                }

                if (SurveyOverviewReportRecepientsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SurveyOverviewReportRecepients = value;
                }

            }
        }

        private bool _SurveyProductivityReportEnabled;
        public Func<bool> SurveyProductivityReportEnabledGet;
        public Action<bool> SurveyProductivityReportEnabledSetBoolean;

        bool IReportsSettings.SurveyProductivityReportEnabled
        {
            get
            {
                if (SurveyProductivityReportEnabledGet != null)
                {
                    return SurveyProductivityReportEnabledGet();
                } else if (_inner != null)
                {
                    return ((IReportsSettings)_inner).SurveyProductivityReportEnabled;
                }

                if (SurveyProductivityReportEnabledSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SurveyProductivityReportEnabled;
                }

                return default(bool);
            }

            set
            {
                if (SurveyProductivityReportEnabledSetBoolean != null)
                {
                    SurveyProductivityReportEnabledSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReportsSettings)_inner).SurveyProductivityReportEnabled = value;
                    return;
                }

                if (SurveyProductivityReportEnabledGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SurveyProductivityReportEnabled = value;
                }

            }
        }

        private int _SurveyProductivityReportHour;
        public Func<int> SurveyProductivityReportHourGet;
        public Action<int> SurveyProductivityReportHourSetInt32;

        int IReportsSettings.SurveyProductivityReportHour
        {
            get
            {
                if (SurveyProductivityReportHourGet != null)
                {
                    return SurveyProductivityReportHourGet();
                } else if (_inner != null)
                {
                    return ((IReportsSettings)_inner).SurveyProductivityReportHour;
                }

                if (SurveyProductivityReportHourSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SurveyProductivityReportHour;
                }

                return default(int);
            }

            set
            {
                if (SurveyProductivityReportHourSetInt32 != null)
                {
                    SurveyProductivityReportHourSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReportsSettings)_inner).SurveyProductivityReportHour = value;
                    return;
                }

                if (SurveyProductivityReportHourGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SurveyProductivityReportHour = value;
                }

            }
        }

        private string _SurveyProductivityReportRecepients;
        public Func<string> SurveyProductivityReportRecepientsGet;
        public Action<string> SurveyProductivityReportRecepientsSetString;

        string IReportsSettings.SurveyProductivityReportRecepients
        {
            get
            {
                if (SurveyProductivityReportRecepientsGet != null)
                {
                    return SurveyProductivityReportRecepientsGet();
                } else if (_inner != null)
                {
                    return ((IReportsSettings)_inner).SurveyProductivityReportRecepients;
                }

                if (SurveyProductivityReportRecepientsSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SurveyProductivityReportRecepients;
                }

                return default(string);
            }

            set
            {
                if (SurveyProductivityReportRecepientsSetString != null)
                {
                    SurveyProductivityReportRecepientsSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReportsSettings)_inner).SurveyProductivityReportRecepients = value;
                    return;
                }

                if (SurveyProductivityReportRecepientsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SurveyProductivityReportRecepients = value;
                }

            }
        }

    }
}