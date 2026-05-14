using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.Interfaces;
using System.Threading;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubISurveyArchiveService : ISurveyArchiveService 
    {
        private ISurveyArchiveService _inner;

        public StubISurveyArchiveService()
        {
            _inner = null;
        }

        public ISurveyArchiveService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string ArchiveBvSurveyEntityDelegate(BvSurveyEntity survey);
        public ArchiveBvSurveyEntityDelegate ArchiveBvSurveyEntity;

        string ISurveyArchiveService.Archive(BvSurveyEntity survey)
        {


            if (ArchiveBvSurveyEntity != null)
            {
                return ArchiveBvSurveyEntity(survey);
            } else if (_inner != null)
            {
                return ((ISurveyArchiveService)_inner).Archive(survey);
            }

            return default(string);
        }

        public delegate string RestoreInt32StringCancellationTokenDelegate(int surveyId, string data, CancellationToken cancellationToken);
        public RestoreInt32StringCancellationTokenDelegate RestoreInt32StringCancellationToken;

        string ISurveyArchiveService.Restore(int surveyId, string data, CancellationToken cancellationToken)
        {


            if (RestoreInt32StringCancellationToken != null)
            {
                return RestoreInt32StringCancellationToken(surveyId, data, cancellationToken);
            } else if (_inner != null)
            {
                return ((ISurveyArchiveService)_inner).Restore(surveyId, data, cancellationToken);
            }

            return default(string);
        }

    }
}