using System;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache;
using BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.Fakes
{
    public class StubISurveyMetadataCache : ISurveyMetadataCache 
    {
        private ISurveyMetadataCache _inner;

        public StubISurveyMetadataCache()
        {
            _inner = null;
        }

        public ISurveyMetadataCache Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate FormDescBase GetFormDescStringDelegate(string name);
        public GetFormDescStringDelegate GetFormDescString;

        FormDescBase ISurveyMetadataCache.GetFormDesc(string name)
        {


            if (GetFormDescString != null)
            {
                return GetFormDescString(name);
            } else if (_inner != null)
            {
                return ((ISurveyMetadataCache)_inner).GetFormDesc(name);
            }

            return default(FormDescBase);
        }

        public delegate FormDescBase GetReplFormDescStringDelegate(string name);
        public GetReplFormDescStringDelegate GetReplFormDescString;

        FormDescBase ISurveyMetadataCache.GetReplFormDesc(string name)
        {


            if (GetReplFormDescString != null)
            {
                return GetReplFormDescString(name);
            } else if (_inner != null)
            {
                return ((ISurveyMetadataCache)_inner).GetReplFormDesc(name);
            }

            return default(FormDescBase);
        }

        public delegate SurveyDatabaseFieldInfo GetRespondentFieldDescStringDelegate(string fieldName);
        public GetRespondentFieldDescStringDelegate GetRespondentFieldDescString;

        SurveyDatabaseFieldInfo ISurveyMetadataCache.GetRespondentFieldDesc(string fieldName)
        {


            if (GetRespondentFieldDescString != null)
            {
                return GetRespondentFieldDescString(fieldName);
            } else if (_inner != null)
            {
                return ((ISurveyMetadataCache)_inner).GetRespondentFieldDesc(fieldName);
            }

            return default(SurveyDatabaseFieldInfo);
        }

    }
}