using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data
{
    public interface IInterviewDataServiceFactory
    {
        IInterviewFormDataSourceService CreateFormService(int surveyId, int interviewId);
        IInterviewRespondentDataSourceService CreateRespondentService(int surveyId, int interviewId);
    }
}
