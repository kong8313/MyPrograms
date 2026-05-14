using System;
using Confirmit.CATI.Core.Reports.InterviewerProductivityCustomReportEntity;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Reports.InterviewerProductivityCustomReportEntity.Fakes
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

        public delegate List<InterviewerProductivityReportTemplate> GetAllDelegate();
        public GetAllDelegate GetAll;

        List<InterviewerProductivityReportTemplate> ICatiSupervisorApiService.GetAll()
        {


            if (GetAll != null)
            {
                return GetAll();
            } else if (_inner != null)
            {
                return ((ICatiSupervisorApiService)_inner).GetAll();
            }

            return default(List<InterviewerProductivityReportTemplate>);
        }

        public delegate InterviewerProductivityReportTemplate GetByIdInt32Delegate(int id);
        public GetByIdInt32Delegate GetByIdInt32;

        InterviewerProductivityReportTemplate ICatiSupervisorApiService.GetById(int id)
        {


            if (GetByIdInt32 != null)
            {
                return GetByIdInt32(id);
            } else if (_inner != null)
            {
                return ((ICatiSupervisorApiService)_inner).GetById(id);
            }

            return default(InterviewerProductivityReportTemplate);
        }

    }
}