namespace Confirmit.CATI.Supervisor.Classes.Quotas
{
    class SimpleQuotaCounterPercentageCssSelector : IQuotaCounterPercentageCssSelector
    {
        public string GetCssClass(int percentageValue)
        {
            var result = "quotas-counter-percentage-red";
            if (percentageValue >= 90 || percentageValue < 0)
            {
                result = "quotas-counter-percentage-green";
            }
            else if (percentageValue >= 11)
            {
                result = "quotas-counter-percentage-yellow";
            }

            return result;
        }
    }
}
