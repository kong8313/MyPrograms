namespace Confirmit.CATI.Supervisor.Classes.Quotas
{
    interface IQuotaCounterPercentageCssSelector
    {
        string GetCssClass(int percentageValue);
    }
}
