using System.Data;

namespace Confirmit.CATI.Supervisor.Core.Surveys
{
    public interface ISurveyQuotasExportInfoProvider
    {
        int SurveyId { get; }
        string[] GetQuotaNames();
        DataTable GetQuotaInfo(string quotaName);
    }
}