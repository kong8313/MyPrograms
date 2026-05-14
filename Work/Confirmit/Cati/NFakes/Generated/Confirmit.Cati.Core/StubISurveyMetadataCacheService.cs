using System;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.Fakes
{
    public class StubISurveyMetadataCacheService : ISurveyMetadataCacheService 
    {
        private ISurveyMetadataCacheService _inner;

        public StubISurveyMetadataCacheService()
        {
            _inner = null;
        }

        public ISurveyMetadataCacheService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ResetSurveyCacheInt32Delegate(int surveyId);
        public ResetSurveyCacheInt32Delegate ResetSurveyCacheInt32;

        void ISurveyMetadataCacheService.ResetSurveyCache(int surveyId)
        {

            if (ResetSurveyCacheInt32 != null)
            {
                ResetSurveyCacheInt32(surveyId);
            } else if (_inner != null)
            {
                ((ISurveyMetadataCacheService)_inner).ResetSurveyCache(surveyId);
            }
        }

        public delegate ISurveyMetadataCache GetInt32Delegate(int surveyId);
        public GetInt32Delegate GetInt32;

        ISurveyMetadataCache ISurveyMetadataCacheService.Get(int surveyId)
        {


            if (GetInt32 != null)
            {
                return GetInt32(surveyId);
            } else if (_inner != null)
            {
                return ((ISurveyMetadataCacheService)_inner).Get(surveyId);
            }

            return default(ISurveyMetadataCache);
        }

    }
}