using Confirmit.CATI.Core.Batch;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public interface IAsyncOperationParameters
    {
        int SurveyId { get; }
    }

    public interface IAsyncBatchedOperationParameters : IAsyncOperationParameters
    {
        BatchParameters BatchParameters { get; }
    }
}