using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services
{
    public interface IInterviewTimings
    {
        BvInterviewTimings GetInterviewTimings(BvTasksEntity task, BvSurveyEntity survey);
    }
}
