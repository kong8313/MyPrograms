using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Confirmit.CATI.Core.AuthoringService;

namespace Confirmit.CATI.Supervisor.Core.Confirmit.QuotaViewExtension
{
    public class AggregateQuotaViewAdditionalColumnsBuilder : IQuotaViewAdditionalColumnsBuilder
    {
        private readonly List<IQuotaViewAdditionalColumnsBuilder> extensions;

        public AggregateQuotaViewAdditionalColumnsBuilder(List<IQuotaViewAdditionalColumnsBuilder> extensions)
        {
            this.extensions = extensions;
        }

        public void AddColumns( DataColumnCollection columns)
        {
            foreach( var extension in extensions )
            {
                extension.AddColumns(columns);
            }
        }

        public void FillRow(DataRow row, QuotaList quota, QuotaRow cell)
        {
            foreach (var extension in extensions)
            {
                extension.
                    FillRow(row, quota, cell);
            }
        }

        public string GetSummaryInfo()
        {
            return String.Join(" ", extensions.Select(x => x.GetSummaryInfo()).Where(y => y != null).ToArray());
        }
    }
}