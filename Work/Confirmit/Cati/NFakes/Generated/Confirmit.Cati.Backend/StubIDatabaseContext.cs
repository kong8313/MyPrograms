using System;
using Confirmit.CATI.Backend.WebApiServices;
using System.Data.Entity;
using Confirmit.CATI.Backend.WebApiServices.Models;

namespace Confirmit.CATI.Backend.WebApiServices.Fakes
{
    public class StubIDatabaseContext : IDatabaseContext 
    {
        private IDatabaseContext _inner;

        public StubIDatabaseContext()
        {
            _inner = null;
        }

        public IDatabaseContext Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void DisposeDelegate();
        public DisposeDelegate Dispose;

        void IDisposable.Dispose()
        {

            if (Dispose != null)
            {
                Dispose();
            } else if (_inner != null)
            {
                ((IDisposable)_inner).Dispose();
            }
        }

        private string _ExecutionLog;
        public Func<string> ExecutionLogGet;
        public Action<string> ExecutionLogSetString;

        string IDatabaseContext.ExecutionLog
        {
            get
            {
                if (ExecutionLogGet != null)
                {
                    return ExecutionLogGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseContext)_inner).ExecutionLog;
                }

                if (ExecutionLogSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ExecutionLog;
                }

                return default(string);
            }

        }

        private DbSet<BreakHistory> _BreakHistory;
        public Func<DbSet<BreakHistory>> BreakHistoryGet;
        public Action<DbSet<BreakHistory>> BreakHistorySetDbSetOfBreakHistory;

        DbSet<BreakHistory> IDatabaseContext.BreakHistory
        {
            get
            {
                if (BreakHistoryGet != null)
                {
                    return BreakHistoryGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseContext)_inner).BreakHistory;
                }

                if (BreakHistorySetDbSetOfBreakHistory == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _BreakHistory;
                }

                return default(DbSet<BreakHistory>);
            }

            set
            {
                if (BreakHistorySetDbSetOfBreakHistory != null)
                {
                    BreakHistorySetDbSetOfBreakHistory(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDatabaseContext)_inner).BreakHistory = value;
                    return;
                }

                if (BreakHistoryGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _BreakHistory = value;
                }

            }
        }

        private DbSet<CallHistory> _CallHistory;
        public Func<DbSet<CallHistory>> CallHistoryGet;
        public Action<DbSet<CallHistory>> CallHistorySetDbSetOfCallHistory;

        DbSet<CallHistory> IDatabaseContext.CallHistory
        {
            get
            {
                if (CallHistoryGet != null)
                {
                    return CallHistoryGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseContext)_inner).CallHistory;
                }

                if (CallHistorySetDbSetOfCallHistory == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CallHistory;
                }

                return default(DbSet<CallHistory>);
            }

            set
            {
                if (CallHistorySetDbSetOfCallHistory != null)
                {
                    CallHistorySetDbSetOfCallHistory(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDatabaseContext)_inner).CallHistory = value;
                    return;
                }

                if (CallHistoryGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _CallHistory = value;
                }

            }
        }

        private DbSet<InterviewerSessionHistory> _InterviewerSessionHistory;
        public Func<DbSet<InterviewerSessionHistory>> InterviewerSessionHistoryGet;
        public Action<DbSet<InterviewerSessionHistory>> InterviewerSessionHistorySetDbSetOfInterviewerSessionHistory;

        DbSet<InterviewerSessionHistory> IDatabaseContext.InterviewerSessionHistory
        {
            get
            {
                if (InterviewerSessionHistoryGet != null)
                {
                    return InterviewerSessionHistoryGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseContext)_inner).InterviewerSessionHistory;
                }

                if (InterviewerSessionHistorySetDbSetOfInterviewerSessionHistory == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InterviewerSessionHistory;
                }

                return default(DbSet<InterviewerSessionHistory>);
            }

            set
            {
                if (InterviewerSessionHistorySetDbSetOfInterviewerSessionHistory != null)
                {
                    InterviewerSessionHistorySetDbSetOfInterviewerSessionHistory(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDatabaseContext)_inner).InterviewerSessionHistory = value;
                    return;
                }

                if (InterviewerSessionHistoryGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _InterviewerSessionHistory = value;
                }

            }
        }

        private DbSet<Interviewer> _Interviewer;
        public Func<DbSet<Interviewer>> InterviewerGet;
        public Action<DbSet<Interviewer>> InterviewerSetDbSetOfInterviewer;

        DbSet<Interviewer> IDatabaseContext.Interviewer
        {
            get
            {
                if (InterviewerGet != null)
                {
                    return InterviewerGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseContext)_inner).Interviewer;
                }

                if (InterviewerSetDbSetOfInterviewer == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Interviewer;
                }

                return default(DbSet<Interviewer>);
            }

            set
            {
                if (InterviewerSetDbSetOfInterviewer != null)
                {
                    InterviewerSetDbSetOfInterviewer(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDatabaseContext)_inner).Interviewer = value;
                    return;
                }

                if (InterviewerGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _Interviewer = value;
                }

            }
        }

        private DbSet<Group> _InterviewerGroup;
        public Func<DbSet<Group>> InterviewerGroupGet;
        public Action<DbSet<Group>> InterviewerGroupSetDbSetOfGroup;

        DbSet<Group> IDatabaseContext.InterviewerGroup
        {
            get
            {
                if (InterviewerGroupGet != null)
                {
                    return InterviewerGroupGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseContext)_inner).InterviewerGroup;
                }

                if (InterviewerGroupSetDbSetOfGroup == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InterviewerGroup;
                }

                return default(DbSet<Group>);
            }

            set
            {
                if (InterviewerGroupSetDbSetOfGroup != null)
                {
                    InterviewerGroupSetDbSetOfGroup(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDatabaseContext)_inner).InterviewerGroup = value;
                    return;
                }

                if (InterviewerGroupGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _InterviewerGroup = value;
                }

            }
        }

        private DbSet<Membership> _Membership;
        public Func<DbSet<Membership>> MembershipGet;
        public Action<DbSet<Membership>> MembershipSetDbSetOfMembership;

        DbSet<Membership> IDatabaseContext.Membership
        {
            get
            {
                if (MembershipGet != null)
                {
                    return MembershipGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseContext)_inner).Membership;
                }

                if (MembershipSetDbSetOfMembership == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Membership;
                }

                return default(DbSet<Membership>);
            }

            set
            {
                if (MembershipSetDbSetOfMembership != null)
                {
                    MembershipSetDbSetOfMembership(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDatabaseContext)_inner).Membership = value;
                    return;
                }

                if (MembershipGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _Membership = value;
                }

            }
        }

        private DbSet<Survey> _Survey;
        public Func<DbSet<Survey>> SurveyGet;
        public Action<DbSet<Survey>> SurveySetDbSetOfSurvey;

        DbSet<Survey> IDatabaseContext.Survey
        {
            get
            {
                if (SurveyGet != null)
                {
                    return SurveyGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseContext)_inner).Survey;
                }

                if (SurveySetDbSetOfSurvey == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Survey;
                }

                return default(DbSet<Survey>);
            }

            set
            {
                if (SurveySetDbSetOfSurvey != null)
                {
                    SurveySetDbSetOfSurvey(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDatabaseContext)_inner).Survey = value;
                    return;
                }

                if (SurveyGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _Survey = value;
                }

            }
        }

        private DbSet<TelephoneBlacklistItem> _TelephoneBlacklist;
        public Func<DbSet<TelephoneBlacklistItem>> TelephoneBlacklistGet;
        public Action<DbSet<TelephoneBlacklistItem>> TelephoneBlacklistSetDbSetOfTelephoneBlacklistItem;

        DbSet<TelephoneBlacklistItem> IDatabaseContext.TelephoneBlacklist
        {
            get
            {
                if (TelephoneBlacklistGet != null)
                {
                    return TelephoneBlacklistGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseContext)_inner).TelephoneBlacklist;
                }

                if (TelephoneBlacklistSetDbSetOfTelephoneBlacklistItem == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _TelephoneBlacklist;
                }

                return default(DbSet<TelephoneBlacklistItem>);
            }

            set
            {
                if (TelephoneBlacklistSetDbSetOfTelephoneBlacklistItem != null)
                {
                    TelephoneBlacklistSetDbSetOfTelephoneBlacklistItem(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDatabaseContext)_inner).TelephoneBlacklist = value;
                    return;
                }

                if (TelephoneBlacklistGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _TelephoneBlacklist = value;
                }

            }
        }

    }
}