using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation;

namespace Confirmit.CATI.Supervisor.Core.Confirmit.QuotaViewExtension
{
    public class UsedCallsAdditionalColumnsBuilder : IQuotaViewAdditionalColumnsBuilder
    {
        private readonly IExtraQuotaCounterParameters _parameters;
        private readonly Dictionary<string, int> _counters;

        public UsedCallsAdditionalColumnsBuilder(IExtraQuotaCounterParameters parameters)
        {
            IUsedCallsCalculator usedCallsCalculator = ServiceLocator.Resolve<IUsedCallsCalculator>();
            _parameters = parameters;
            _counters = usedCallsCalculator.GetCountersOfNotScheduledExcludingCompletes(parameters).
                                            ToDictionary(x => x.Descriptor, y => y.Value);
        }

        public void AddColumns(DataColumnCollection columns)
        {
            columns.Add(QuotaManager.UsedCalls, typeof(int));
            columns.Add(QuotaManager.BurnRate, typeof(double));
        }

        public void FillRow(DataRow row, QuotaList quota, QuotaRow cell)
        {
            int value;
            string cellDescriptor = ExtraQuotaCounterService.CreateCellDescriptor(_parameters.QuotaFields, quota.FieldNames, cell.FieldPrecodes);
            _counters.TryGetValue(cellDescriptor, out value);

            row[QuotaManager.UsedCalls] = value;
            row[QuotaManager.BurnRate] = (cell.Counter != 0) ? ((double)value / cell.Counter) : 0;
        }

        public string GetSummaryInfo()
        {
            return null;
        }
    }
}