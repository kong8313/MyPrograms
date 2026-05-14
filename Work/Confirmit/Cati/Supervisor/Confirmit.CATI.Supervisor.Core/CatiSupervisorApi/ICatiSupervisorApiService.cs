using Confirmit.CATI.Core.Reports.CustomInterviewerProductivityReport;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.CatiSupervisorApi
{
    public interface ICatiSupervisorApiService
    {
        List<InterviewerProductivityReportTemplate> GetAllTemplates();

        InterviewerProductivityReportTemplate GetByTemplateId(int id);
    }
}