using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Runtime.Caching;
using System;
using System.Threading;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache
{
    public class SurveyMetadataCacheService : ISurveyMetadataCacheService
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly MemoryCache _cache = new MemoryCache("SurveyMetadataCache");
        private readonly CacheItemPolicy _cachePolicy = new CacheItemPolicy
        {
            SlidingExpiration = TimeSpan.FromHours(3)
        };

        public SurveyMetadataCacheService(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        public void ResetSurveyCache(int surveyId)
        {
            var survey = _surveyRepository.GetById(surveyId);

            var evt = new ResetSurveyMetadataCacheEvent(surveyId, survey.Name);

            _cache.Remove(surveyId.ToString());

            evt.Finish();
        }

        public ISurveyMetadataCache Get(int surveyId)
        {
            var newSurveyCacheEntry = new Lazy<SurveyMetadataCache>(() => new SurveyMetadataCache(surveyId), LazyThreadSafetyMode.PublicationOnly);

            var result = (Lazy<SurveyMetadataCache>)_cache.AddOrGetExisting(surveyId.ToString(), newSurveyCacheEntry, _cachePolicy);

            return (result ?? newSurveyCacheEntry).Value;
        }
    }

}
