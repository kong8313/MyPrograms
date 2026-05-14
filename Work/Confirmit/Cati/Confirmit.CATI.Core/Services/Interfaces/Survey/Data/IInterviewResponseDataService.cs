
namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data
{
    public interface IInterviewResponseDataService
    {
        string GetInterviewVariableValue(string projectId, int interviewId, string variableName);
    }
}
