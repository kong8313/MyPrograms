namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface IExternalTransferTelephoneNumberService
    {
        void InsertNumber(string telephoneNumber, string description, bool isHidden, int[] assignedSurveysIds);
        void UpdateNumber(int id, string telephoneNumber, string description, bool isHidden, int[] assignedSurveysIds);
        void DeleteNumbers(int[] ids);

        int[] GetAssignedSurveyIds(int externalTransferTelephoneNumberId);
        void SetAssignedSurveyIds(int externalTransferTelephoneNumberId, int[] surveyIds);
    }
}
