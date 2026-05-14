using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;

namespace Confirmit.CATI.Core.Tasks
{
    public interface ITaskExtension
    {
        void UpdateOnCallConnected(BvTasksEntity task, BvInterviewEntity interview, BvCallEntity call);
        void ProcessLinkedChain(BvTasksEntity task, BvTasksEntity originalTask);
        int GetFirstCampaignFromLinkedChain(BvTasksEntity task);
        int? SetLinkedInterviewSessionId(BvTasksEntity task);
        void AssignCallOnTask(BvTasksEntity task, BvSurveyEntity survey, BvInterviewEntity interview, BvCallEntity call, BvActiveDialEntity dial);
        void SetInterviewingState(BvTasksEntity task, BvActiveDialEntity dial);
    }
}
