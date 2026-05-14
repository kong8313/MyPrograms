namespace Confirmit.CATI.Core.Misc
{
    public interface IRedialNumberSaver
    {
        void SaveAlternativeNumber(int surveySid, string currentPhoneNumber, int interviewId);
    }
}