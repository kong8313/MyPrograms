using System;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation.Fakes
{
    public class StubISampleBatchProcessor : ISampleBatchProcessor 
    {
        private ISampleBatchProcessor _inner;

        public StubISampleBatchProcessor()
        {
            _inner = null;
        }

        public ISampleBatchProcessor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ProcessSampleContextInt32Delegate(SampleContext context, int startRangeOfInterviewId);
        public ProcessSampleContextInt32Delegate ProcessSampleContextInt32;

        void ISampleBatchProcessor.Process(SampleContext context, int startRangeOfInterviewId)
        {

            if (ProcessSampleContextInt32 != null)
            {
                ProcessSampleContextInt32(context, startRangeOfInterviewId);
            } else if (_inner != null)
            {
                ((ISampleBatchProcessor)_inner).Process(context, startRangeOfInterviewId);
            }
        }

        private RespondentRecord[] _Records;
        public Func<RespondentRecord[]> RecordsGet;
        public Action<RespondentRecord[]> RecordsSetArrayOfRespondentRecord;

        RespondentRecord[] ISampleBatchProcessor.Records
        {
            get
            {
                if (RecordsGet != null)
                {
                    return RecordsGet();
                } else if (_inner != null)
                {
                    return ((ISampleBatchProcessor)_inner).Records;
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