using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Quota.Data;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface IQuotaInfoService
    {
        bool HasQuotas(int surveyId);
        string[] GetQuotaFields(int surveyId, int quotaId);
        QuotaInfo[] GetQuotaInfos(int surveyId);
        string[] GetQuotaFields(int surveyId, string quotaName);
        string GetQuotaName(int surveyId, int quotaId);
        string GetQuotaTable(BvSurveyEntity survey, int quotaId);
        string GetQuotaTable(BvSurveyEntity survey, string name);
        string[] GetCellValues(int surveyId, int quotaId, int cellId, string[] fields);
        bool IsExists(BvSurveyEntity survey, string quotaName);
        Dictionary<string, string> GellQuotaCellValuesMap(string projectId, string quotaName);
    }
}
