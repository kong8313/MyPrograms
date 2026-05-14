using System;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation.Fakes
{
    public class StubISampleRecordStorage : ISampleRecordStorage 
    {
        private ISampleRecordStorage _inner;

        public StubISampleRecordStorage()
        {
            _inner = null;
        }

        public ISampleRecordStorage Inner
        {
            set {_inner = value;} get {return _inner;}
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

    }
}