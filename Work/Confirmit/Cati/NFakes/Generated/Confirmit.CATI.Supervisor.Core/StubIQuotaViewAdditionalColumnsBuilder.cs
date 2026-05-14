using System;
using System.Data;
using Confirmit.CATI.Supervisor.Core.Confirmit.QuotaViewExtension;
using Confirmit.CATI.Core.AuthoringService;

namespace Confirmit.CATI.Supervisor.Core.Confirmit.QuotaViewExtension.Fakes
{
    public class StubIQuotaViewAdditionalColumnsBuilder : IQuotaViewAdditionalColumnsBuilder 
    {
        private IQuotaViewAdditionalColumnsBuilder _inner;

        public StubIQuotaViewAdditionalColumnsBuilder()
        {
            _inner = null;
        }

        public IQuotaViewAdditionalColumnsBuilder Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void AddColumnsDataColumnCollectionDelegate(DataColumnCollection columns);
        public AddColumnsDataColumnCollectionDelegate AddColumnsDataColumnCollection;

        void IQuotaViewAdditionalColumnsBuilder.AddColumns(DataColumnCollection columns)
        {

            if (AddColumnsDataColumnCollection != null)
            {
                AddColumnsDataColumnCollection(columns);
            } else if (_inner != null)
            {
                ((IQuotaViewAdditionalColumnsBuilder)_inner).AddColumns(columns);
            }
        }

        public delegate void FillRowDataRowQuotaListQuotaRowDelegate(DataRow row, QuotaList quotaList, QuotaRow cell);
        public FillRowDataRowQuotaListQuotaRowDelegate FillRowDataRowQuotaListQuotaRow;

        void IQuotaViewAdditionalColumnsBuilder.FillRow(DataRow row, QuotaList quotaList, QuotaRow cell)
        {

            if (FillRowDataRowQuotaListQuotaRow != null)
            {
                FillRowDataRowQuotaListQuotaRow(row, quotaList, cell);
            } else if (_inner != null)
            {
                ((IQuotaViewAdditionalColumnsBuilder)_inner).FillRow(row, quotaList, cell);
            }
        }

        public delegate string GetSummaryInfoDelegate();
        public GetSummaryInfoDelegate GetSummaryInfo;

        string IQuotaViewAdditionalColumnsBuilder.GetSummaryInfo()
        {


            if (GetSummaryInfo != null)
            {
                return GetSummaryInfo();
            } else if (_inner != null)
            {
                return ((IQuotaViewAdditionalColumnsBuilder)_inner).GetSummaryInfo();
            }

            return default(string);
        }

    }
}