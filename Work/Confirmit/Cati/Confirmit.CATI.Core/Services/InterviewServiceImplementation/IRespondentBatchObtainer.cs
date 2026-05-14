using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.InterviewServiceImplementation
{
    public interface IRespondentBatchObtainer
    {
        RespondentRecord[] GetRespondentBatchPartition(BvSurveyEntity survey, int batchId, int startRangeOfInterviewId, int partitionSize, bool isSampleUpdate);
        RespondentRecord[] GetRespondentsForSynchronization(BvSurveyEntity survey, int partitionSize);
    }
}