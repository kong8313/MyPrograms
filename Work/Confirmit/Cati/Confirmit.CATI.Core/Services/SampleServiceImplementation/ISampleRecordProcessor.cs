using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation
{
    public interface ISampleRecordProcessor
    {
        void Process(ISampleRecordStorage storage, SampleProcessingStateContainer stateContainer, RespondentRecord record, BvInterviewWithOriginEntity interview, ProcessSampleMode processSampleMode);
        void OnCompleted();
    }
}