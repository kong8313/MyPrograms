using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Backend.WcfServices.Internal.ManagementService
{
    public interface IInterviewTimings
    {
        BvInterviewTimings GetInterviewTimings(BvTasksEntity task, BvSurveyEntity survey);
    }
}
