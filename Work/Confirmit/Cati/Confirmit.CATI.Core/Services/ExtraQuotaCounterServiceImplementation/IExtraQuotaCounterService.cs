namespace Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation
{
    public interface IExtraQuotaCounterService
    {
        IExtraQuotaCounterCalculator Create(IExtraQuotaCounterParameters parameters); 
    }
}