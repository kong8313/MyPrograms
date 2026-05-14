using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.Services.Survey.Data
{
    public class SurveyDataRowCache
    {
        public string TableName { get; set; }
        public bool IsExists { get; set; }
        public DataRow Row { get; private set; }
        public string LoopLevel { get; private set; }
        public string[] LoopPath{ get; private set; }
        public string[] LoopQualifyer{ get; private set; }

        public HashSet<string> ChangedColumns { get; private set; }
        public HashSet<string> ChangedForms { get; private set; }

        // Stores original values for fieldName (field-level tracking only)
        private readonly Dictionary<string, object> _originalFieldValues = new Dictionary<string, object>();

        public SurveyDataRowCache(string tableName, string loopLevel, string[] loopPath, string[] loopQualifyer, bool isExists, DataRow row)
        {
            TableName = tableName;
            IsExists = isExists;
            Row = row;
            LoopLevel = loopLevel;
            LoopPath = loopPath;
            LoopQualifyer = loopQualifyer;
            ChangedColumns = new HashSet<string>();
            ChangedForms = new HashSet<string>();
        }

        public bool IsChanged
        {
            get { return ChangedColumns.Any(); }
        }

        public void SetFieldValue(string formName, string fieldName, object value)
        {
            var ordinal = GetColumnOrdinal(fieldName);

            // Log original value for field if not already present
            if (!_originalFieldValues.ContainsKey(fieldName))
            {
                var original = Row.ItemArray[ordinal];
                _originalFieldValues[fieldName] = original is DBNull ? null : original;
            }

            var data = Row.ItemArray;
            data[ordinal] = value ?? DBNull.Value;
            Row.ItemArray = data;

            ChangedColumns.Add(fieldName);
            ChangedForms.Add(formName);
        }

        public object GetFieldValue(string fieldName)
        {
            var ordinal = GetColumnOrdinal(fieldName);
            var result = Row.ItemArray[ordinal];

            return (result is DBNull) ? null : result;
        }

        private int GetColumnOrdinal(string fieldName)
        {
            var column = Row.Table.Columns[fieldName];

            if (column == null)
            {
                throw new Exception(String.Format("Column with name '{0}' not found", fieldName));
            }

            return column.Ordinal;
        }

        // Expose original value log (read-only)
        public IReadOnlyDictionary<string, object> OriginalFieldValues => _originalFieldValues;
    }
}
