using Confirmit.CATI.Core.Batch;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface IReviewerService
    {
        string CreateSessionForReview(string sessionName, int surveyId, string userName, BatchParameters batchParameters);

        string GetReviewerUrlTemplate();
    }
}
