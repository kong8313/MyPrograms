using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation
{
    public class SampleDataStorageFactory : ISampleDataStorageFactory
    {
        private readonly ISurveyConnectionStringProvider _surveyConnectionStringProvider;
        private readonly IRemoteDataCopier _remoteDataCopier;
        private readonly ISurveyDatabaseEngine _surveyDatabaseEngine;

        public SampleDataStorageFactory(
            ISurveyConnectionStringProvider surveyConnectionStringProvider, 
            IRemoteDataCopier remoteDataCopier,
            ISurveyDatabaseEngine surveyDatabaseEngine)
        {
            _surveyConnectionStringProvider = surveyConnectionStringProvider;
            _remoteDataCopier = remoteDataCopier;
            _surveyDatabaseEngine = surveyDatabaseEngine;
        }

        public ISampleDataStorage Create(int batchID, BvSurveyEntity survey, int startRangeOfInterviewId, bool isUpdateMode)
        {
            return new SampleDataStorage(_surveyConnectionStringProvider, _remoteDataCopier, _surveyDatabaseEngine, batchID, survey.SID, (SurveySchedulingMode)survey.SurveySchedulingMode, survey.IsRandomCallDeliveryEnabled, startRangeOfInterviewId, isUpdateMode);
        }
    }
}