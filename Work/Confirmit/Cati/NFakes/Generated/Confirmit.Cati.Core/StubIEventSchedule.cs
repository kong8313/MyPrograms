using System;
using BvDotNetScript.Interfaces;
using BvDotNetScript.ScriptObjects;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services;

namespace BvDotNetScript.Interfaces.Fakes
{
    public class StubIEventSchedule : IEventSchedule 
    {
        private IEventSchedule _inner;

        public StubIEventSchedule()
        {
            _inner = null;
        }

        public IEventSchedule Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate CallAttempt[] GetCallHistoryDelegate();
        public GetCallHistoryDelegate GetCallHistory;

        CallAttempt[] IEventSchedule.GetCallHistory()
        {


            if (GetCallHistory != null)
            {
                return GetCallHistory();
            } else if (_inner != null)
            {
                return ((IEventSchedule)_inner).GetCallHistory();
            }

            return default(CallAttempt[]);
        }

        public delegate CallAttempt[] GetCallHistoryExtendedStatusDelegate(ExtendedStatus extendedStatus);
        public GetCallHistoryExtendedStatusDelegate GetCallHistoryExtendedStatus;

        CallAttempt[] IEventSchedule.GetCallHistory(ExtendedStatus extendedStatus)
        {


            if (GetCallHistoryExtendedStatus != null)
            {
                return GetCallHistoryExtendedStatus(extendedStatus);
            } else if (_inner != null)
            {
                return ((IEventSchedule)_inner).GetCallHistory(extendedStatus);
            }

            return default(CallAttempt[]);
        }

        public delegate CallAttempt[] GetCallHistoryStringDelegate(string telephoneNumber);
        public GetCallHistoryStringDelegate GetCallHistoryString;

        CallAttempt[] IEventSchedule.GetCallHistory(string telephoneNumber)
        {


            if (GetCallHistoryString != null)
            {
                return GetCallHistoryString(telephoneNumber);
            } else if (_inner != null)
            {
                return ((IEventSchedule)_inner).GetCallHistory(telephoneNumber);
            }

            return default(CallAttempt[]);
        }

        public delegate CallAttempt[] GetCallHistoryExtendedStatusInt32Delegate(ExtendedStatus extendedStatus, int withinFirstN);
        public GetCallHistoryExtendedStatusInt32Delegate GetCallHistoryExtendedStatusInt32;

        CallAttempt[] IEventSchedule.GetCallHistory(ExtendedStatus extendedStatus, int withinFirstN)
        {


            if (GetCallHistoryExtendedStatusInt32 != null)
            {
                return GetCallHistoryExtendedStatusInt32(extendedStatus, withinFirstN);
            } else if (_inner != null)
            {
                return ((IEventSchedule)_inner).GetCallHistory(extendedStatus, withinFirstN);
            }

            return default(CallAttempt[]);
        }

        public delegate void AddCallBvCallEntityDelegate(BvCallEntity call);
        public AddCallBvCallEntityDelegate AddCallBvCallEntity;

        void IEventSchedule.AddCall(BvCallEntity call)
        {

            if (AddCallBvCallEntity != null)
            {
                AddCallBvCallEntity(call);
            } else if (_inner != null)
            {
                ((IEventSchedule)_inner).AddCall(call);
            }
        }

        private BvSurveyEntity _Survey;
        public Func<BvSurveyEntity> SurveyGet;
        public Action<BvSurveyEntity> SurveySetBvSurveyEntity;

        BvSurveyEntity IEventSchedule.Survey
        {
            get
            {
                if (SurveyGet != null)
                {
                    return SurveyGet();
                } else if (_inner != null)
                {
                    return ((IEventSchedule)_inner).Survey;
                }

                if (SurveySetBvSurveyEntity == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Survey;
                }

                return default(BvSurveyEntity);
            }

        }

        private BvInterviewWithOriginEntity _Interview;
        public Func<BvInterviewWithOriginEntity> InterviewGet;
        public Action<BvInterviewWithOriginEntity> InterviewSetBvInterviewWithOriginEntity;

        BvInterviewWithOriginEntity IEventSchedule.Interview
        {
            get
            {
                if (InterviewGet != null)
                {
                    return InterviewGet();
                } else if (_inner != null)
                {
                    return ((IEventSchedule)_inner).Interview;
                }

                if (InterviewSetBvInterviewWithOriginEntity == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Interview;
                }

                return default(BvInterviewWithOriginEntity);
            }

        }

        private BvCallEntity _LastCall;
        public Func<BvCallEntity> LastCallGet;
        public Action<BvCallEntity> LastCallSetBvCallEntity;

        BvCallEntity IEventSchedule.LastCall
        {
            get
            {
                if (LastCallGet != null)
                {
                    return LastCallGet();
                } else if (_inner != null)
                {
                    return ((IEventSchedule)_inner).LastCall;
                }

                if (LastCallSetBvCallEntity == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _LastCall;
                }

                return default(BvCallEntity);
            }

        }

        private BvCallEntity _NewCall;
        public Func<BvCallEntity> NewCallGet;
        public Action<BvCallEntity> NewCallSetBvCallEntity;

        BvCallEntity IEventSchedule.NewCall
        {
            get
            {
                if (NewCallGet != null)
                {
                    return NewCallGet();
                } else if (_inner != null)
                {
                    return ((IEventSchedule)_inner).NewCall;
                }

                if (NewCallSetBvCallEntity == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _NewCall;
                }

                return default(BvCallEntity);
            }

            set
            {
                if (NewCallSetBvCallEntity != null)
                {
                    NewCallSetBvCallEntity(value);
                    return;
                } else if (_inner != null)
                {
                    ((IEventSchedule)_inner).NewCall = value;
                    return;
                }

                if (NewCallGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _NewCall = value;
                }

            }
        }

        private DateTime _Time;
        public Func<DateTime> TimeGet;
        public Action<DateTime> TimeSetDateTime;

        DateTime IEventSchedule.Time
        {
            get
            {
                if (TimeGet != null)
                {
                    return TimeGet();
                } else if (_inner != null)
                {
                    return ((IEventSchedule)_inner).Time;
                }

                if (TimeSetDateTime == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Time;
                }

                return default(DateTime);
            }

        }

        private SchedulingScriptExecutionReason _ExecutionReason;
        public Func<SchedulingScriptExecutionReason> ExecutionReasonGet;
        public Action<SchedulingScriptExecutionReason> ExecutionReasonSetSchedulingScriptExecutionReason;

        SchedulingScriptExecutionReason IEventSchedule.ExecutionReason
        {
            get
            {
                if (ExecutionReasonGet != null)
                {
                    return ExecutionReasonGet();
                } else if (_inner != null)
                {
                    return ((IEventSchedule)_inner).ExecutionReason;
                }

                if (ExecutionReasonSetSchedulingScriptExecutionReason == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ExecutionReason;
                }

                return default(SchedulingScriptExecutionReason);
            }

        }

        private long _BatchID;
        public Func<long> BatchIDGet;
        public Action<long> BatchIDSetInt64;

        long IEventSchedule.BatchID
        {
            get
            {
                if (BatchIDGet != null)
                {
                    return BatchIDGet();
                } else if (_inner != null)
                {
                    return ((IEventSchedule)_inner).BatchID;
                }

                if (BatchIDSetInt64 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _BatchID;
                }

                return default(long);
            }

        }

        private ShiftService _Shifts;
        public Func<ShiftService> ShiftsGet;
        public Action<ShiftService> ShiftsSetShiftService;

        ShiftService IEventSchedule.Shifts
        {
            get
            {
                if (ShiftsGet != null)
                {
                    return ShiftsGet();
                } else if (_inner != null)
                {
                    return ((IEventSchedule)_inner).Shifts;
                }

                if (ShiftsSetShiftService == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Shifts;
                }

                return default(ShiftService);
            }

        }

        private int _CallCenterID;
        public Func<int> CallCenterIDGet;
        public Action<int> CallCenterIDSetInt32;

        int IEventSchedule.CallCenterID
        {
            get
            {
                if (CallCenterIDGet != null)
                {
                    return CallCenterIDGet();
                } else if (_inner != null)
                {
                    return ((IEventSchedule)_inner).CallCenterID;
                }

                if (CallCenterIDSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CallCenterID;
                }

                return default(int);
            }

        }

        private ProcessSampleMode _ProcessSampleMode;
        public Func<ProcessSampleMode> ProcessSampleModeGet;
        public Action<ProcessSampleMode> ProcessSampleModeSetProcessSampleMode;

        ProcessSampleMode IEventSchedule.ProcessSampleMode
        {
            get
            {
                if (ProcessSampleModeGet != null)
                {
                    return ProcessSampleModeGet();
                } else if (_inner != null)
                {
                    return ((IEventSchedule)_inner).ProcessSampleMode;
                }

                if (ProcessSampleModeSetProcessSampleMode == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ProcessSampleMode;
                }

                return default(ProcessSampleMode);
            }

        }

        private string _CliNumber;
        public Func<string> CliNumberGet;
        public Action<string> CliNumberSetString;

        string IEventSchedule.CliNumber
        {
            get
            {
                if (CliNumberGet != null)
                {
                    return CliNumberGet();
                } else if (_inner != null)
                {
                    return ((IEventSchedule)_inner).CliNumber;
                }

                if (CliNumberSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CliNumber;
                }

                return default(string);
            }

        }

        private string _DdiNumber;
        public Func<string> DdiNumberGet;
        public Action<string> DdiNumberSetString;

        string IEventSchedule.DdiNumber
        {
            get
            {
                if (DdiNumberGet != null)
                {
                    return DdiNumberGet();
                } else if (_inner != null)
                {
                    return ((IEventSchedule)_inner).DdiNumber;
                }

                if (DdiNumberSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DdiNumber;
                }

                return default(string);
            }

        }

        private string _ExtendedStatus;
        public Func<string> ExtendedStatusGet;
        public Action<string> ExtendedStatusSetString;

        string IEventSchedule.ExtendedStatus
        {
            get
            {
                if (ExtendedStatusGet != null)
                {
                    return ExtendedStatusGet();
                } else if (_inner != null)
                {
                    return ((IEventSchedule)_inner).ExtendedStatus;
                }

                if (ExtendedStatusSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ExtendedStatus;
                }

                return default(string);
            }

        }

        private DialingAttempt _LastDialingAttempt;
        public Func<DialingAttempt> LastDialingAttemptGet;
        public Action<DialingAttempt> LastDialingAttemptSetDialingAttempt;

        DialingAttempt IEventSchedule.LastDialingAttempt
        {
            get
            {
                if (LastDialingAttemptGet != null)
                {
                    return LastDialingAttemptGet();
                } else if (_inner != null)
                {
                    return ((IEventSchedule)_inner).LastDialingAttempt;
                }

                if (LastDialingAttemptSetDialingAttempt == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _LastDialingAttempt;
                }

                return default(DialingAttempt);
            }

        }

        private DialingAttempt[] _LastCallDialingAttempts;
        public Func<DialingAttempt[]> LastCallDialingAttemptsGet;
        public Action<DialingAttempt[]> LastCallDialingAttemptsSetArrayOfDialingAttempt;

        DialingAttempt[] IEventSchedule.LastCallDialingAttempts
        {
            get
            {
                if (LastCallDialingAttemptsGet != null)
                {
                    return LastCallDialingAttemptsGet();
                } else if (_inner != null)
                {
                    return ((IEventSchedule)_inner).LastCallDialingAttempts;
                }

                if (LastCallDialingAttemptsSetArrayOfDialingAttempt == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _LastCallDialingAttempts;
                }

                return default(DialingAttempt[]);
            }

        }

    }
}