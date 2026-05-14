using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.Core.Reports.InterviewerProductivityCustomReportEntity;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIInterviewerProductivityReportTemplateRepository : IInterviewerProductivityReportTemplateRepository 
    {
        private IInterviewerProductivityReportTemplateRepository _inner;

        public StubIInterviewerProductivityReportTemplateRepository()
        {
            _inner = null;
        }

        public IInterviewerProductivityReportTemplateRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<InterviewerProductivityReportTemplate> GetAllDelegate();
        public GetAllDelegate GetAll;

        List<InterviewerProductivityReportTemplate> IInterviewerProductivityReportTemplateRepository.GetAll()
        {


            if (GetAll != null)
            {
                return GetAll();
            } else if (_inner != null)
            {
                return ((IInterviewerProductivityReportTemplateRepository)_inner).GetAll();
            }

            return default(List<InterviewerProductivityReportTemplate>);
        }

        public delegate InterviewerProductivityReportTemplate GetByIdInt32Delegate(int id);
        public GetByIdInt32Delegate GetByIdInt32;

        InterviewerProductivityReportTemplate IInterviewerProductivityReportTemplateRepository.GetById(int id)
        {


            if (GetByIdInt32 != null)
            {
                return GetByIdInt32(id);
            } else if (_inner != null)
            {
                return ((IInterviewerProductivityReportTemplateRepository)_inner).GetById(id);
            }

            return default(InterviewerProductivityReportTemplate);
        }

    }
}