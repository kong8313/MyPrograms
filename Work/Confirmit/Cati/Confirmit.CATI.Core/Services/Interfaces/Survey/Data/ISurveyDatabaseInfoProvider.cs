using System.Collections.Generic;
using BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data
{
    public interface ISurveyDatabaseInfoProvider
    {
        SurveyDatabaseFormInfo GetFormInfo(int surveyId, string name);
        IEnumerable<SurveyDatabaseFieldInfo> GetRespondentFieldsInfo(int surveyId);
    }
}