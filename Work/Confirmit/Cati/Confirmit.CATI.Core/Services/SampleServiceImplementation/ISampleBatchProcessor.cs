using Confirmit.CATI.Core.Services.InterviewServiceImplementation;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation
{
    public interface ISampleBatchProcessor
    {
        void Process(SampleContext context, int startRangeOfInterviewId);
        RespondentRecord[] Records { get; }
    }
}