namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data
{
    public interface IInterviewRespondentDataService
    {
        object GetRespondentValue(string fieldName);
        void SetRespondentValue(string fieldName, object value);
        string GetDiff();
    }
}