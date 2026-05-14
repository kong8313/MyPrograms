using System;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;

namespace Confirmit.CATI.Core.Services.Survey.Fakes
{
    public class StubISynchronizationProcessor : IRespondentsSynchronizationProcessor 
    {
        private IRespondentsSynchronizationProcessor _inner;

        public StubISynchronizationProcessor()
        {
            _inner = null;
        }

        public IRespondentsSynchronizationProcessor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ProcessSynchronizationContextInt32Delegate(RespondentsSynchronizationContext context);
        public ProcessSynchronizationContextInt32Delegate ProcessSynchronizationContextInt32;

        void IRespondentsSynchronizationProcessor.SynchronizeRespondents(RespondentsSynchronizationContext context)
        {

            if (ProcessSynchronizationContextInt32 != null)
            {
                ProcessSynchronizationContextInt32(context);
            } else if (_inner != null)
            {
                ((IRespondentsSynchronizationProcessor)_inner).SynchronizeRespondents(context);
            }
        }

        private RespondentRecord[] _Records;
        public Func<RespondentRecord[]> RecordsGet;
        public Action<RespondentRecord[]> RecordsSetArrayOfRespondentRecord;

        RespondentRecord[] IRespondentsSynchronizationProcessor.Records
        {
            get
            {
                if (RecordsGet != null)
                {
                    return RecordsGet();
                } else if (_inner != null)
                {
                    return ((IRespondentsSynchronizationProcessor)_inner).Records;
                }

                if (RecordsSetArrayOfRespondentRecord == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Records;
                }

                return default(RespondentRecord[]);
            }

        }

    }
}