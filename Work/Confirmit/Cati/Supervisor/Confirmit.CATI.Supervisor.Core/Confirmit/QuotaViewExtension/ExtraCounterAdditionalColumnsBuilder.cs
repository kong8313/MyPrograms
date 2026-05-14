using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation;

namespace Confirmit.CATI.Supervisor.Core.Confirmit.QuotaViewExtension
{
    public class ExtraCounterAdditionalColumnsBuilder : IQuotaViewAdditionalColumnsBuilder
    {
        private readonly IExtraQuotaCounterParameters _parameters;
        private readonly Dictionary<string, int> _extraCounters;

        private readonly string _extraStatus;

        public ExtraCounterAdditionalColumnsBuilder(IExtraQuotaCounterParameters parameters)
        {
            IExtraQuotaCounterService extraQuotaCounterService = ServiceLocator.Resolve<IExtraQuotaCounterService>();
            _parameters = parameters;
            var calculator = extraQuotaCounterService.Create(parameters);
            _extraCounters = calculator.GetCellCounter().ToDictionary(x => x.Descriptor, y => y.Value);
            _extraStatus = calculator.GetFormatedTotalCounter();
        }
        #region IQuotaViewAdditionalColumnsBuilder Members

        public void AddColumns(DataColumnCollection columns)
        {
            columns.Add(QuotaManager.ExtraCounter, typeof(int));
        }

        public void FillRow(DataRow row, QuotaList quota, QuotaRow cell)
        {
            string cellDescriptor = ExtraQuotaCounterService.CreateCellDescriptor(_parameters.QuotaFields, quota.FieldNames, cell.FieldPrecodes);
            
            int value;
            
            _extraCounters.TryGetValue(cellDescriptor, out value);
            row[QuotaManager.ExtraCounter] = value;
        }

        public string GetSummaryInfo()
        {
            return _extraStatus;
        }

        #endregion
    }
}