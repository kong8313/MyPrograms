namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface ILoginGroupRepository
    {
        bool IsResourceLoggedIntoSurvey(int resourceId, int surveySid);
        bool IsResourceReadyForCallInSurvey(int resourceId, int surveySid);
        bool IsAnyoneLoggedIntoSurvey(int surveySid);
        bool IsAnyoneLoggedIntoSurvey(int surveySid, int agentTypeIndex);
        bool IsAnyoneReadyForCallInSurvey(int surveySid, int agentTypeIndex);
    }
}