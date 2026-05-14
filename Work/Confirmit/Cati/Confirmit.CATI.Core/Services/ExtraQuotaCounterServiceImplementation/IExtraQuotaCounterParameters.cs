namespace Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation
{
    public interface IExtraQuotaCounterParameters
    {
        int SurveyId { get; }

        string[] QuotaFields { get; }
    }
}
