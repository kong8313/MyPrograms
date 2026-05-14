using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation
{
    public interface ISampleDataStorageFactory
    {
        ISampleDataStorage Create(int batchID, BvSurveyEntity survey, int startRangeOfInterviewId, bool isUpdateMode);
    }
}