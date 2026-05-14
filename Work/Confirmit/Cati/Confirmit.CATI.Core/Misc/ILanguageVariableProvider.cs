namespace Confirmit.CATI.Core.Misc
{
    public interface ILanguageVariableProvider
    {
        int? GetLanguageForInterview(int surveySid, int interviewId);
    }
}