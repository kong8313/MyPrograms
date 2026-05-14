using System;
using Confirmit.CATI.Supervisor.Core.CatiSupervisorApi;
using System.Collections.Generic;
using Confirmit.CATI.Core.Reports.CustomInterviewerProductivityReport;

namespace Confirmit.CATI.Supervisor.Core.CatiSupervisorApi.Fakes
{
    public class StubICatiSupervisorApiService : ICatiSupervisorApiService 
    {
        private ICatiSupervisorApiService _inner;

        public StubICatiSupervisorApiService()
        {
            _inner = null;
        }

        public ICatiSupervisorApiService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<InterviewerProductivityReportTemplate> GetAllTemplatesDelegate();
        public GetAllTemplatesDelegate GetAllTemplates;

        List<InterviewerProductivityReportTemplate> ICatiSupervisorApiService.GetAllTemplates()
        {


            if (GetAllTemplates != null)
            {
                return GetAllTemplates();
            } else if (_inner != null)
            {
                return ((ICatiSupervisorApiService)_inner).GetAllTemplates();
            }

            return default(List<InterviewerProductivityReportTemplate>);
        }

        public delegate InterviewerProductivityReportTemplate GetByTemplateIdInt32Delegate(int id);
        public GetByTemplateIdInt32Delegate GetByTemplateIdInt32;

        InterviewerProductivityReportTemplate ICatiSupervisorApiService.GetByTemplateId(int id)
        {


            if (GetByTemplateIdInt32 != null)
            {
                return GetByTemplateIdInt32(id);
            } else if (_inner != null)
            {
                return ((ICatiSupervisorApiService)_inner).GetByTemplateId(id);
            }

            return default(InterviewerProductivityReportTemplate);
        }

    }
}