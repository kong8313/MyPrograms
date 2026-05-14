using System;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Common.Logging;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation.Fakes
{
    public class StubISampleDataStorage : ISampleDataStorage 
    {
        private ISampleDataStorage _inner;

        public StubISampleDataStorage()
        {
            _inner = null;
        }

        public ISampleDataStorage Inner
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

        public delegate void InsertInterviewBvInterviewEntityDelegate(BvInterviewEntity interview);
        public InsertInterviewBvInterviewEntityDelegate InsertInterviewBvInterviewEntity;

        void ISampleDataStorage.InsertInterview(BvInterviewEntity interview)
        {

            if (InsertInterviewBvInterviewEntity != null)
            {
                InsertInterviewBvInterviewEntity(interview);
            } else if (_inner != null)
            {
                ((ISampleDataStorage)_inner).InsertInterview(interview);
            }
        }

        public delegate void UpdateInterviewBvInterviewEntityDelegate(BvInterviewEntity interview);
        public UpdateInterviewBvInterviewEntityDelegate UpdateInterviewBvInterviewEntity;

        void ISampleDataStorage.UpdateInterview(BvInterviewEntity interview)
        {

            if (UpdateInterviewBvInterviewEntity != null)
            {
                UpdateInterviewBvInterviewEntity(interview);
            } else if (_inner != null)
            {
                ((ISampleDataStorage)_inner).UpdateInterview(interview);
            }
        }

        public delegate void DeleteInterviewInt32Int32Delegate(int surveySID, int interviewID);
        public DeleteInterviewInt32Int32Delegate DeleteInterviewInt32Int32;

        void ISampleDataStorage.DeleteInterview(int surveySID, int interviewID)
        {

            if (DeleteInterviewInt32Int32 != null)
            {
                DeleteInterviewInt32Int32(surveySID, interviewID);
            } else if (_inner != null)
            {
                ((ISampleDataStorage)_inner).DeleteInterview(surveySID, interviewID);
            }
        }

        public delegate void InsertCallBvCallEntityDelegate(BvCallEntity call);
        public InsertCallBvCallEntityDelegate InsertCallBvCallEntity;

        void ISampleDataStorage.InsertCall(BvCallEntity call)
        {

            if (InsertCallBvCallEntity != null)
            {
                InsertCallBvCallEntity(call);
            } else if (_inner != null)
            {
                ((ISampleDataStorage)_inner).InsertCall(call);
            }
        }

        public delegate void UpdateCallBvCallEntityDelegate(BvCallEntity call);
        public UpdateCallBvCallEntityDelegate UpdateCallBvCallEntity;

        void ISampleDataStorage.UpdateCall(BvCallEntity call)
        {

            if (UpdateCallBvCallEntity != null)
            {
                UpdateCallBvCallEntity(call);
            } else if (_inner != null)
            {
                ((ISampleDataStorage)_inner).UpdateCall(call);
            }
        }

        public delegate void DeleteCallInt32Int32Delegate(int surveySID, int interviewID);
        public DeleteCallInt32Int32Delegate DeleteCallInt32Int32;

        void ISampleDataStorage.DeleteCall(int surveySID, int interviewID)
        {

            if (DeleteCallInt32Int32 != null)
            {
                DeleteCallInt32Int32(surveySID, interviewID);
            } else if (_inner != null)
            {
                ((ISampleDataStorage)_inner).DeleteCall(surveySID, interviewID);
            }
        }

        public delegate void SaveCurrentRecordDelegate();
        public SaveCurrentRecordDelegate SaveCurrentRecord;

        void ISampleDataStorage.SaveCurrentRecord()
        {

            if (SaveCurrentRecord != null)
            {
                SaveCurrentRecord();
            } else if (_inner != null)
            {
                ((ISampleDataStorage)_inner).SaveCurrentRecord();
            }
        }

        public delegate void CommitIEventDetailsDelegate(IEventDetails eventDetails);
        public CommitIEventDetailsDelegate CommitIEventDetails;

        void ISampleDataStorage.Commit(IEventDetails eventDetails)
        {

            if (CommitIEventDetails != null)
            {
                CommitIEventDetails(eventDetails);
            } else if (_inner != null)
            {
                ((ISampleDataStorage)_inner).Commit(eventDetails);
            }
        }

        private BvInterviewEntity _Interview;
        public Func<BvInterviewEntity> InterviewGet;
        public Action<BvInterviewEntity> InterviewSetBvInterviewEntity;

        BvInterviewEntity ISampleRecordStorage.Interview
        {
            get
            {
                if (InterviewGet != null)
                {
                    return InterviewGet();
                } else if (_inner != null)
                {
                    return ((ISampleRecordStorage)_inner).Interview;
                }

                if (InterviewSetBvInterviewEntity == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Interview;
                }

                return default(BvInterviewEntity);
            }

            set
            {
                if (InterviewSetBvInterviewEntity != null)
                {
                    InterviewSetBvInterviewEntity(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISampleRecordStorage)_inner).Interview = value;
                    return;
                }

                if (InterviewGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _Interview = value;
                }

            }
        }

        private BvCallEntity _Call;
        public Func<BvCallEntity> CallGet;
        public Action<BvCallEntity> CallSetBvCallEntity;

        BvCallEntity ISampleRecordStorage.Call
        {
            get
            {
                if (CallGet != null)
                {
                    return CallGet();
                } else if (_inner != null)
                {
                    return ((ISampleRecordStorage)_inner).Call;
                }

                if (CallSetBvCallEntity == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Call;
                }

                return default(BvCallEntity);
            }

            set
            {
                if (CallSetBvCallEntity != null)
                {
                    CallSetBvCallEntity(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISampleRecordStorage)_inner).Call = value;
                    return;
                }

                if (CallGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _Call = value;
                }

            }
        }

        private int _BatchID;
        public Func<int> BatchIDGet;
        public Action<int> BatchIDSetInt32;

        int ISampleDataStorage.BatchID
        {
            get
            {
                if (BatchIDGet != null)
                {
                    return BatchIDGet();
                } else if (_inner != null)
                {
                    return ((ISampleDataStorage)_inner).BatchID;
                }

                if (BatchIDSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _BatchID;
                }

                return default(int);
            }

            set
            {
                if (BatchIDSetInt32 != null)
                {
                    BatchIDSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISampleDataStorage)_inner).BatchID = value;
                    return;
                }

                if (BatchIDGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _BatchID = value;
                }

            }
        }

        private int _SurveySID;
        public Func<int> SurveySIDGet;
        public Action<int> SurveySIDSetInt32;

        int ISampleDataStorage.SurveySID
        {
            get
            {
                if (SurveySIDGet != null)
                {
                    return SurveySIDGet();
                } else if (_inner != null)
                {
                    return ((ISampleDataStorage)_inner).SurveySID;
                }

                if (SurveySIDSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SurveySID;
                }

                return default(int);
            }

            set
            {
                if (SurveySIDSetInt32 != null)
                {
                    SurveySIDSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISampleDataStorage)_inner).SurveySID = value;
                    return;
                }

                if (SurveySIDGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SurveySID = value;
                }

            }
        }

        private int _OperationId;
        public Func<int> OperationIdGet;
        public Action<int> OperationIdSetInt32;

        int ISampleDataStorage.OperationId
        {
            get
            {
                if (OperationIdGet != null)
                {
                    return OperationIdGet();
                } else if (_inner != null)
                {
                    return ((ISampleDataStorage)_inner).OperationId;
                }

                if (OperationIdSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _OperationId;
                }

                return default(int);
            }

            set
            {
                if (OperationIdSetInt32 != null)
                {
                    OperationIdSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISampleDataStorage)_inner).OperationId = value;
                    return;
                }

                if (OperationIdGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _OperationId = value;
                }

            }
        }

        private bool _IsCallDisabledByFCD;
        public Func<bool> IsCallDisabledByFCDGet;
        public Action<bool> IsCallDisabledByFCDSetBoolean;

        bool ISampleDataStorage.IsCallDisabledByFCD
        {
            get
            {
                if (IsCallDisabledByFCDGet != null)
                {
                    return IsCallDisabledByFCDGet();
                } else if (_inner != null)
                {
                    return ((ISampleDataStorage)_inner).IsCallDisabledByFCD;
                }

                if (IsCallDisabledByFCDSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsCallDisabledByFCD;
                }

                return default(bool);
            }

            set
            {
                if (IsCallDisabledByFCDSetBoolean != null)
                {
                    IsCallDisabledByFCDSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISampleDataStorage)_inner).IsCallDisabledByFCD = value;
                    return;
                }

                if (IsCallDisabledByFCDGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _IsCallDisabledByFCD = value;
                }

            }
        }

    }
}