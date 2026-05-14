using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Services.Survey.Data
{
    public class InterviewFormDataServiceFactory : IInterviewDataServiceFactory
    {
        private readonly ISchedulingScriptSettings _settings;

        public InterviewFormDataServiceFactory(
            ISchedulingScriptSettings settings)
        {
            _settings = settings;
        }

        public IInterviewFormDataSourceService CreateFormService(int surveyId, int interviewId)
        {
            var result = _settings.UseDirectDbAccess
                ? (IInterviewFormDataSourceService)ServiceLocator.Resolve<IInterviewFormDataDatabaseSourceService>()
                : ServiceLocator.Resolve<IInterviewFormDataWebSourceService>();

            result.Initialize(surveyId, interviewId);

            return result;
        }

        public IInterviewRespondentDataSourceService CreateRespondentService(int surveyId, int interviewId)
        {
            var result = ServiceLocator.Resolve<IInterviewRespondentDataSourceService>();
            
            result.Initialize(surveyId, interviewId);
            
            return result;
        }
    }
}
