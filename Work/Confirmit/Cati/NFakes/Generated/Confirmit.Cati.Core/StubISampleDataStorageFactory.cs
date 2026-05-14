using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation.Fakes
{
    public class StubISampleDataStorageFactory : ISampleDataStorageFactory 
    {
        private ISampleDataStorageFactory _inner;

        public StubISampleDataStorageFactory()
        {
            _inner = null;
        }

        public ISampleDataStorageFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate ISampleDataStorage CreateInt32BvSurveyEntityInt32BooleanDelegate(int batchID, BvSurveyEntity survey, int startRangeOfInterviewId, bool isUpdateMode);
        public CreateInt32BvSurveyEntityInt32BooleanDelegate CreateInt32BvSurveyEntityInt32Boolean;

        ISampleDataStorage ISampleDataStorageFactory.Create(int batchID, BvSurveyEntity survey, int startRangeOfInterviewId, bool isUpdateMode)
        {


            if (CreateInt32BvSurveyEntityInt32Boolean != null)
            {
                return CreateInt32BvSurveyEntityInt32Boolean(batchID, survey, startRangeOfInterviewId, isUpdateMode);
            } else if (_inner != null)
            {
                return ((ISampleDataStorageFactory)_inner).Create(batchID, survey, startRangeOfInterviewId, isUpdateMode);
            }

            return default(ISampleDataStorage);
        }

    }
}