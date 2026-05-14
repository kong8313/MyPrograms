using System;
using Confirmit.CATI.Core.Services.Survey;
using System.Threading;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;

namespace Confirmit.CATI.Core.Services.Survey.Fakes
{
    public class StubIRespondentsSynchronizationProcessor : IRespondentsSynchronizationProcessor 
    {
        private IRespondentsSynchronizationProcessor _inner;

        public StubIRespondentsSynchronizationProcessor()
        {
            _inner = null;
        }

        public IRespondentsSynchronizationProcessor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SynchronizeRespondentsRespondentsSynchronizationContextCancellationTokenDelegate(RespondentsSynchronizationContext context, CancellationToken cancellationToken);
        public SynchronizeRespondentsRespondentsSynchronizationContextCancellationTokenDelegate SynchronizeRespondentsRespondentsSynchronizationContextCancellationToken;

        void IRespondentsSynchronizationProcessor.SynchronizeRespondents(RespondentsSynchronizationContext context, CancellationToken cancellationToken)
        {

            if (SynchronizeRespondentsRespondentsSynchronizationContextCancellationToken != null)
            {
                SynchronizeRespondentsRespondentsSynchronizationContextCancellationToken(context, cancellationToken);
            } else if (_inner != null)
            {
                ((IRespondentsSynchronizationProcessor)_inner).SynchronizeRespondents(context, cancellationToken);
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