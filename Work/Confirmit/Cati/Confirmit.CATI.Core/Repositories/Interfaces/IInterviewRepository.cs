using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IInterviewRepository
    {
        BvInterviewWithOriginEntity GetById(int surveySid, int interviewId);
        BvInterviewWithOriginEntity GetByIdWithCheck(int surveySid, int interviewId);
        BvInterviewWithOriginEntity GetByTelephoneNumber(int surveyId, string telephoneNumber);

        void Update([NotNull] BvInterviewWithOriginEntity interview, SchedulingScriptExecutionOptions schedulingOptions);
        void InsertOnly(BvInterviewEntity interview);
        void Insert([NotNull] BvInterviewWithOriginEntity interview, SchedulingScriptExecutionOptions schedulingOptions, ISampleDataStorage sampleStorage = null);
        
    }
}
