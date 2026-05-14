using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Confirmit.CATI.Supervisor.Core.Surveys;

namespace Confirmit.CATI.Supervisor.Core.UnitTests
{
    class SurveyQuotasExportInfoProviderStub : ISurveyQuotasExportInfoProvider
    {
        private readonly string _quotaName;
        private readonly string[] _columns;
        private readonly string[][] _values;

        public SurveyQuotasExportInfoProviderStub(string quotaName, string[] columns, string[][] values)
        {
            _quotaName = quotaName;
            _columns = columns;
            _values = values;
        }

        #region ISurveyQuotasExportInfoProvider Members

        public int SurveyId
        {
            get { return 0; }
        }

        public string[] GetQuotaNames()
        {
            return new[] { _quotaName };
        }

        public DataTable GetQuotaInfo(string quotaName)
        {
            var result = new DataTable();

            foreach (var c in _columns)
                result.Columns.Add(c);

            for (var rowIndex = 0; rowIndex < _values.GetLength(0); rowIndex++ )
            {
                var r = result.NewRow();

                for (var columnIndex = 0; columnIndex < _values[rowIndex].Length; columnIndex++)
                {
                    r[columnIndex] = _values[rowIndex][columnIndex];
                }

                result.Rows.Add(r);
            }

            return result;
        }

        #endregion
    }
}
