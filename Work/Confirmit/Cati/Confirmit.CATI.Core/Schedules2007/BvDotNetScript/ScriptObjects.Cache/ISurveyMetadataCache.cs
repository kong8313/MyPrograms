using BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache
{
    public interface ISurveyMetadataCache
    {
        FormDescBase GetFormDesc(string name);
        FormDescBase GetReplFormDesc(string name);
        SurveyDatabaseFieldInfo GetRespondentFieldDesc(string fieldName);
    }
}