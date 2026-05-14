using System.Threading.Tasks;
using Confirmit.CATI.Core.Reports.CustomInterviewerProductivityReport;

namespace Confirmit.CATI.Core.Services
{
    public interface ISupervisorApiClient
    {
        Task<InterviewerProductivityReportTemplate> GetSystemTemplate();
        Task<InterviewerProductivityReportTemplate> GetTemplate(int templateId);
    }
}