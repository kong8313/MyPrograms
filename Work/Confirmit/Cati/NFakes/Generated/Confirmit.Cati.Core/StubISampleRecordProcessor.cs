using System;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.ManagementService;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation.Fakes
{
    public class StubISampleRecordProcessor : ISampleRecordProcessor 
    {
        private ISampleRecordProcessor _inner;

        public StubISampleRecordProcessor()
        {
            _inner = null;
        }

        public ISampleRecordProcessor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ProcessISampleRecordStorageSampleProcessingStateContainerRespondentRecordBvInterviewWithOriginEntityProcessSampleModeDelegate(ISampleRecordStorage storage, SampleProcessingStateContainer stateContainer, RespondentRecord record, BvInterviewWithOriginEntity interview, ProcessSampleMode processSampleMode);
        public ProcessISampleRecordStorageSampleProcessingStateContainerRespondentRecordBvInterviewWithOriginEntityProcessSampleModeDelegate ProcessISampleRecordStorageSampleProcessingStateContainerRespondentRecordBvInterviewWithOriginEntityProcessSampleMode;

        void ISampleRecordProcessor.Process(ISampleRecordStorage storage, SampleProcessingStateContainer stateContainer, RespondentRecord record, BvInterviewWithOriginEntity interview, ProcessSampleMode processSampleMode)
        {

            if (ProcessISampleRecordStorageSampleProcessingStateContainerRespondentRecordBvInterviewWithOriginEntityProcessSampleMode != null)
            {
                ProcessISampleRecordStorageSampleProcessingStateContainerRespondentRecordBvInterviewWithOriginEntityProcessSampleMode(storage, stateContainer, record, interview, processSampleMode);
            } else if (_inner != null)
            {
                ((ISampleRecordProcessor)_inner).Process(storage, stateContainer, record, interview, processSampleMode);
            }
        }

        public delegate void OnCompletedDelegate();
        public OnCompletedDelegate OnCompleted;

        void ISampleRecordProcessor.OnCompleted()
        {

            if (OnCompleted != null)
            {
                OnCompleted();
            } else if (_inner != null)
            {
                ((ISampleRecordProcessor)_inner).OnCompleted();
            }
        }

    }
}