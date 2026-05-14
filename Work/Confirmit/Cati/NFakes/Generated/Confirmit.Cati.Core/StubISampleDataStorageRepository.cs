using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation.Fakes
{
    public class StubISampleDataStorageRepository : ISampleDataStorageRepository 
    {
        private ISampleDataStorageRepository _inner;

        public StubISampleDataStorageRepository()
        {
            _inner = null;
        }

        public ISampleDataStorageRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate ISampleDataStorage CreateInt32BvSurveyEntityInt32BooleanDelegate(int batchID, BvSurveyEntity survey, int startRangeOfInterviewId, bool isUpdateMode);
        public CreateInt32BvSurveyEntityInt32BooleanDelegate CreateInt32BvSurveyEntityInt32Boolean;

        ISampleDataStorage ISampleDataStorageRepository.Create(int batchID, BvSurveyEntity survey, int startRangeOfInterviewId, bool isUpdateMode)
        {


            if (CreateInt32BvSurveyEntityInt32Boolean != null)
            {
                return CreateInt32BvSurveyEntityInt32Boolean(batchID, survey, startRangeOfInterviewId, isUpdateMode);
            } else if (_inner != null)
            {
                return ((ISampleDataStorageRepository)_inner).Create(batchID, survey, startRangeOfInterviewId, isUpdateMode);
            }

            return default(ISampleDataStorage);
        }

        public delegate ISampleDataStorage CreateBvSurveyEntityInt32Delegate(BvSurveyEntity survey, int operationId);
        public CreateBvSurveyEntityInt32Delegate CreateBvSurveyEntityInt32;

        ISampleDataStorage ISampleDataStorageRepository.Create(BvSurveyEntity survey, int operationId)
        {


            if (CreateBvSurveyEntityInt32 != null)
            {
                return CreateBvSurveyEntityInt32(survey, operationId);
            } else if (_inner != null)
            {
                return ((ISampleDataStorageRepository)_inner).Create(survey, operationId);
            }

            return default(ISampleDataStorage);
        }

        public delegate ISampleDataStorage GetInt32Delegate(int batchId);
        public GetInt32Delegate GetInt32;

        ISampleDataStorage ISampleDataStorageRepository.Get(int batchId)
        {


            if (GetInt32 != null)
            {
                return GetInt32(batchId);
            } else if (_inner != null)
            {
                return ((ISampleDataStorageRepository)_inner).Get(batchId);
            }

            return default(ISampleDataStorage);
        }

        public delegate void DeleteInt32Delegate(int batchId);
        public DeleteInt32Delegate DeleteInt32;

        void ISampleDataStorageRepository.Delete(int batchId)
        {

            if (DeleteInt32 != null)
            {
                DeleteInt32(batchId);
            } else if (_inner != null)
            {
                ((ISampleDataStorageRepository)_inner).Delete(batchId);
            }
        }

    }
}