using BvDotNetScript.ScriptObjects.Cache;

namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data
{
    public interface IInterviewFormDataService
    {
        string GetFormValue(FormDescBase desc, string category, string[] loopQualifyer);
        void SetFormValue(FormDescBase desc, string category, string[] loopQualifyer, string value);
    }
}
