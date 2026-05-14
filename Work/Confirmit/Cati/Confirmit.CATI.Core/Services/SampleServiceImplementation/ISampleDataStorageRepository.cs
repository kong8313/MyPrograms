using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation
{
    public interface ISampleDataStorageRepository 
    {
        ISampleDataStorage Create(int batchID, BvSurveyEntity survey, int startRangeOfInterviewId, bool isUpdateMode);
        ISampleDataStorage Create(BvSurveyEntity survey, int operationId);
        /// <summary>
        /// this method creates SampleDataStorage object for coresponding batch and survey
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        ISampleDataStorage Get(int batchId);

        void Delete(int batchId);
    }
}