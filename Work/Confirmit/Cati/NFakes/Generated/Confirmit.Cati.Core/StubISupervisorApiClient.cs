using System;
using Confirmit.CATI.Core.Services;
using System.Threading.Tasks;
using Confirmit.CATI.Core.Reports.CustomInterviewerProductivityReport;

namespace Confirmit.CATI.Core.Services.Fakes
{
    public class StubISupervisorApiClient : ISupervisorApiClient 
    {
        private ISupervisorApiClient _inner;

        public StubISupervisorApiClient()
        {
            _inner = null;
        }

        public ISupervisorApiClient Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate Task<InterviewerProductivityReportTemplate> GetSystemTemplateDelegate();
        public GetSystemTemplateDelegate GetSystemTemplate;

        Task<InterviewerProductivityReportTemplate> ISupervisorApiClient.GetSystemTemplate()
        {


            if (GetSystemTemplate != null)
            {
                return GetSystemTemplate();
            } else if (_inner != null)
            {
                return ((ISupervisorApiClient)_inner).GetSystemTemplate();
            }

            return default(Task<InterviewerProductivityReportTemplate>);
        }

        public delegate Task<InterviewerProductivityReportTemplate> GetTemplateInt32Delegate(int templateId);
        public GetTemplateInt32Delegate GetTemplateInt32;

        Task<InterviewerProductivityReportTemplate> ISupervisorApiClient.GetTemplate(int templateId)
        {


            if (GetTemplateInt32 != null)
            {
                return GetTemplateInt32(templateId);
            } else if (_inner != null)
            {
                return ((ISupervisorApiClient)_inner).GetTemplate(templateId);
            }

            return default(Task<InterviewerProductivityReportTemplate>);
        }

    }
}