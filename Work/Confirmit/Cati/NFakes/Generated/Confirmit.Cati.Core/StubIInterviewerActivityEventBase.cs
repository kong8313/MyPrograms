using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;

namespace Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging.Fakes
{
    public class StubIInterviewerActivityEventBase : IInterviewerActivityEventBase 
    {
        private IInterviewerActivityEventBase _inner;

        public StubIInterviewerActivityEventBase()
        {
            _inner = null;
        }

        public IInterviewerActivityEventBase Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void UpdateEventPropertiesFromTaskBvTasksEntityDelegate(BvTasksEntity task);
        public UpdateEventPropertiesFromTaskBvTasksEntityDelegate UpdateEventPropertiesFromTaskBvTasksEntity;

        void IInterviewerActivityEventBase.UpdateEventPropertiesFromTask(BvTasksEntity task)
        {

            if (UpdateEventPropertiesFromTaskBvTasksEntity != null)
            {
                UpdateEventPropertiesFromTaskBvTasksEntity(task);
            } else if (_inner != null)
            {
                ((IInterviewerActivityEventBase)_inner).UpdateEventPropertiesFromTask(task);
            }
        }

        public delegate string DetailsToXmlDelegate();
        public DetailsToXmlDelegate DetailsToXml;

        string IInterviewerActivityEventBase.DetailsToXml()
        {


            if (DetailsToXml != null)
            {
                return DetailsToXml();
            } else if (_inner != null)
            {
                return ((IInterviewerActivityEventBase)_inner).DetailsToXml();
            }

            return default(string);
        }

        public delegate void AddTimingStringDelegate(string timingName);
        public AddTimingStringDelegate AddTimingString;

        void IInterviewerActivityEventBase.AddTiming(string timingName)
        {

            if (AddTimingString != null)
            {
                AddTimingString(timingName);
            } else if (_inner != null)
            {
                ((IInterviewerActivityEventBase)_inner).AddTiming(timingName);
            }
        }

        private InterviewerActivityEventType _EventTypeId;
        public Func<InterviewerActivityEventType> EventTypeIdGet;
        public Action<InterviewerActivityEventType> EventTypeIdSetInterviewerActivityEventType;

        InterviewerActivityEventType IInterviewerActivityEventBase.EventTypeId
        {
            get
            {
                if (EventTypeIdGet != null)
                {
                    return EventTypeIdGet();
                } else if (_inner != null)
                {
                    return ((IInterviewerActivityEventBase)_inner).EventTypeId;
                }

                if (EventTypeIdSetInterviewerActivityEventType == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EventTypeId;
                }

                return default(InterviewerActivityEventType);
            }

        }

        private string _EventTypeName;
        public Func<string> EventTypeNameGet;
        public Action<string> EventTypeNameSetString;

        string IInterviewerActivityEventBase.EventTypeName
        {
            get
            {
                if (EventTypeNameGet != null)
                {
                    return EventTypeNameGet();
                } else if (_inner != null)
                {
                    return ((IInterviewerActivityEventBase)_inner).EventTypeName;
                }

                if (EventTypeNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EventTypeName;
                }

                return default(string);
            }

        }

        private string _ServerName;
        public Func<string> ServerNameGet;
        public Action<string> ServerNameSetString;

        string IInterviewerActivityEventBase.ServerName
        {
            get
            {
                if (ServerNameGet != null)
                {
                    return ServerNameGet();
                } else if (_inner != null)
                {
                    return ((IInterviewerActivityEventBase)_inner).ServerName;
                }

                if (ServerNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ServerName;
                }

                return default(string);
            }

        }

        private int _CompanyId;
        public Func<int> CompanyIdGet;
        public Action<int> CompanyIdSetInt32;

        int IInterviewerActivityEventBase.CompanyId
        {
            get
            {
                if (CompanyIdGet != null)
                {
                    return CompanyIdGet();
                } else if (_inner != null)
                {
                    return ((IInterviewerActivityEventBase)_inner).CompanyId;
                }

                if (CompanyIdSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CompanyId;
                }

                return default(int);
            }

        }

        private int? _SurveySid;
        public Func<int?> SurveySidGet;
        public Action<int?> SurveySidSetNullableOfInt32;

        int? IInterviewerActivityEventBase.SurveySid
        {
            get
            {
                if (SurveySidGet != null)
                {
                    return SurveySidGet();
                } else if (_inner != null)
                {
                    return ((IInterviewerActivityEventBase)_inner).SurveySid;
                }

                if (SurveySidSetNullableOfInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SurveySid;
                }

                return default(int?);
            }

            set
            {
                if (SurveySidSetNullableOfInt32 != null)
                {
                    SurveySidSetNullableOfInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IInterviewerActivityEventBase)_inner).SurveySid = value;
                    return;
                }

                if (SurveySidGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SurveySid = value;
                }

            }
        }

        private string _SurveyName;
        public Func<string> SurveyNameGet;
        public Action<string> SurveyNameSetString;

        string IInterviewerActivityEventBase.SurveyName
        {
            get
            {
                if (SurveyNameGet != null)
                {
                    return SurveyNameGet();
                } else if (_inner != null)
                {
                    return ((IInterviewerActivityEventBase)_inner).SurveyName;
                }

                if (SurveyNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SurveyName;
                }

                return default(string);
            }

            set
            {
                if (SurveyNameSetString != null)
                {
                    SurveyNameSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IInterviewerActivityEventBase)_inner).SurveyName = value;
                    return;
                }

                if (SurveyNameGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SurveyName = value;
                }

            }
        }

        private int _InterviewerSid;
        public Func<int> InterviewerSidGet;
        public Action<int> InterviewerSidSetInt32;

        int IInterviewerActivityEventBase.InterviewerSid
        {
            get
            {
                if (InterviewerSidGet != null)
                {
                    return InterviewerSidGet();
                } else if (_inner != null)
                {
                    return ((IInterviewerActivityEventBase)_inner).InterviewerSid;
                }

                if (InterviewerSidSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InterviewerSid;
                }

                return default(int);
            }

            set
            {
                if (InterviewerSidSetInt32 != null)
                {
                    InterviewerSidSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IInterviewerActivityEventBase)_inner).InterviewerSid = value;
                    return;
                }

                if (InterviewerSidGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _InterviewerSid = value;
                }

            }
        }

        private DateTime _StartTime;
        public Func<DateTime> StartTimeGet;
        public Action<DateTime> StartTimeSetDateTime;

        DateTime IInterviewerActivityEventBase.StartTime
        {
            get
            {
                if (StartTimeGet != null)
                {
                    return StartTimeGet();
                } else if (_inner != null)
                {
                    return ((IInterviewerActivityEventBase)_inner).StartTime;
                }

                if (StartTimeSetDateTime == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _StartTime;
                }

                return default(DateTime);
            }

        }

        private DateTime _FinishTime;
        public Func<DateTime> FinishTimeGet;
        public Action<DateTime> FinishTimeSetDateTime;

        DateTime IInterviewerActivityEventBase.FinishTime
        {
            get
            {
                if (FinishTimeGet != null)
                {
                    return FinishTimeGet();
                } else if (_inner != null)
                {
                    return ((IInterviewerActivityEventBase)_inner).FinishTime;
                }

                if (FinishTimeSetDateTime == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _FinishTime;
                }

                return default(DateTime);
            }

        }

        private TimeSpan _Duration;
        public Func<TimeSpan> DurationGet;
        public Action<TimeSpan> DurationSetTimeSpan;

        TimeSpan IInterviewerActivityEventBase.Duration
        {
            get
            {
                if (DurationGet != null)
                {
                    return DurationGet();
                } else if (_inner != null)
                {
                    return ((IInterviewerActivityEventBase)_inner).Duration;
                }

                if (DurationSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Duration;
                }

                return default(TimeSpan);
            }

        }

        private string _PhoneNumber;
        public Func<string> PhoneNumberGet;
        public Action<string> PhoneNumberSetString;

        string IInterviewerActivityEventBase.PhoneNumber
        {
            get
            {
                if (PhoneNumberGet != null)
                {
                    return PhoneNumberGet();
                } else if (_inner != null)
                {
                    return ((IInterviewerActivityEventBase)_inner).PhoneNumber;
                }

                if (PhoneNumberSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _PhoneNumber;
                }

                return default(string);
            }

            set
            {
                if (PhoneNumberSetString != null)
                {
                    PhoneNumberSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IInterviewerActivityEventBase)_inner).PhoneNumber = value;
                    return;
                }

                if (PhoneNumberGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _PhoneNumber = value;
                }

            }
        }

        private int? _InterviewId;
        public Func<int?> InterviewIdGet;
        public Action<int?> InterviewIdSetNullableOfInt32;

        int? IInterviewerActivityEventBase.InterviewId
        {
            get
            {
                if (InterviewIdGet != null)
                {
                    return InterviewIdGet();
                } else if (_inner != null)
                {
                    return ((IInterviewerActivityEventBase)_inner).InterviewId;
                }

                if (InterviewIdSetNullableOfInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InterviewId;
                }

                return default(int?);
            }

            set
            {
                if (InterviewIdSetNullableOfInt32 != null)
                {
                    InterviewIdSetNullableOfInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IInterviewerActivityEventBase)_inner).InterviewId = value;
                    return;
                }

                if (InterviewIdGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _InterviewId = value;
                }

            }
        }

    }
}