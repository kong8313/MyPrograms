using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Paging;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation
{
    /// <summary>
    /// This class present condition from WHERE zone in sql query.
    /// It contains coulmn operator and value.
    /// </summary>
    public class SqlCondition
    {
        public string Column { get; private set; }
        public TableTypes TableType { get; private set; }
        public FilterOperator Operator { get; private set; }
        public string Value { get; private set; }
        public VariableTypes VariableType { get; private set; }
        public bool IsNeedCast { get; private set; }

        public SqlCondition(string column,
                            TableTypes tableType,
                            FilterOperator filterOperator,
                            string value,
                            VariableTypes variableType,
                            bool isNeedCast)
        {
            Column = column;
            TableType = tableType;
            Operator = filterOperator;
            Value = value;
            VariableType = variableType;
            IsNeedCast = isNeedCast;
        }


        internal static string GetTableName(TableTypes tableType)
        {
            switch (tableType)
            {
                case TableTypes.Appointment:
                {
                    return "BvAppointment";
                }
                case TableTypes.Call:
                {
                    return "BvCall";
                }
                case TableTypes.CFVariables:
                {
                    return "CFinterview";
                }
                case TableTypes.Interview:
                {
                    return "BvInterview";
                }
                case TableTypes.ShiftType:
                {
                    return "BvShiftType";
                }
                case TableTypes.Resource:
                {
                    return "BvViewPersonAndGroup";
                }
                case TableTypes.Person:
                {
                    return "BvPerson";
                }
                case TableTypes.Timezone:
                {
                    return "BvTimezone";
                }
                default:
                {
                    throw new IndexOutOfRangeException(String.Format(
                        "Unable to create where clause. There is no table type {0}",
                        tableType));
                }
            }
        }

        internal static string GetOperatorString(FilterOperator filterOperator)
        {
            switch (filterOperator)
            {
                case FilterOperator.Bigger:
                {
                    return ">";
                }
                case FilterOperator.BiggerEqual:
                {
                    return ">=";
                }
                case FilterOperator.Equal:
                {
                    return "=";
                }
                case FilterOperator.Less:
                {
                    return "<";
                }
                case FilterOperator.LessEqual:
                {
                    return "<=";
                }
                case FilterOperator.Like:
                {
                    return " LIKE ";
                }
                case FilterOperator.NotEqual:
                {
                    return "<>";
                }
                case FilterOperator.In:
                {
                    return " IN ";
                }
                case FilterOperator.NotIn:
                {
                    return " NOT IN ";
                }
                default:
                {
                    throw new IndexOutOfRangeException(String.Format(
                        "Unable to create where clause. There is no operator {0}",
                        filterOperator));
                }
            }
        }

        internal static string GetValueString(
            FilterOperator filterOperator, 
            string value,
            VariableTypes variableType)
        {
            if (filterOperator == FilterOperator.Like || filterOperator == FilterOperator.Not)
            {
                // we are trying to check if current condition was added as part of searching procedure.
                // if so, we should replace value which suitable for filters with value suitable for searching.
                string val = SearchManager.DecodeTextValue(value);
                if (String.IsNullOrEmpty(val) && filterOperator == FilterOperator.Like)
                {
                    return String.Format("\'{0}%\'", value);
                }
                
                return String.Format("\'{0}\'", val);
            }
            else if (filterOperator == FilterOperator.In || filterOperator == FilterOperator.NotIn)
            {
                // For IN and NOT IN operators, we need to format values for the SQL IN clause
                // Format: (value1, value2, value3)

                // Split the comma-separated string
                string[] values = value.Split(',');

                if (variableType == VariableTypes.String || variableType == VariableTypes.Date)
                {
                    // For string/date values, add single quotes around each value
                    string quotedValues = string.Join(
                        ",",
                        Array.ConvertAll(values, v => $"'{v.Trim()}'")
                    );
                    return $"({quotedValues})";
                }
                else
                {
                    // For numeric values, just use the values as-is
                    return $"({value})";
                }
            }
            else
            {
                // TODO:
                // we should wrap string value in ''
                // All background variables have type nvarchar(255). SQL performs implicit convertation
                // but numeric could have different precision and scale. That is why we have to
                // specify type of numeric. at this time we get default numeric. but there could be problems
                // if count of digit before delimiter is greater than 20 our cast will be incorrect.
                switch (variableType)
                {
                    case VariableTypes.String:
                    case VariableTypes.Date:
                    {
                        return String.Format("\'{0}\'", value);
                    }
                    case VariableTypes.Decimal:
                    {
                        return String.Format("CAST( {0} AS FLOAT)", value);
                    }
                    default:
                    {
                        return value;
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// tabletypes contains expression flag are not processed.
        /// Instead condition value from m_Column is returned.
        /// </remarks>
        /// <returns>string representation of sql condition</returns>
        public override string ToString()
        {
            string result = "{0}";

            
            //FilterOperator.IsNullOrEmpty has different pattern (no value is used for comparison : 
            //"NULLIF(column_name, '') IS NULL" vs  "column_name <sigh> value") so it is handled separately
            //NULLIF : If column_name is NULL then NULL is returned, if column_name is empty then NULL is returned as well
            if (Operator == FilterOperator.IsNullOrEmpty)
                return String.Format("NULLIF({0}.{1},'') IS NULL", GetTableName(TableType), Column);

            if (Operator == FilterOperator.Not)
            {
                var realValue = GetValueString(Operator, Value, VariableType);
                if (string.IsNullOrEmpty(realValue))
                {
                    return String.Format("{0}.{1} <> ''", GetTableName(TableType), Column);
                }

                return String.Format("NOT ({0}.{1} LIKE {2})", GetTableName(TableType), Column, realValue);
            }

            // all background variables have type nvarchar(255)
            // if there are columns which are empty then we get error on convertation from 
            // empty string to certain type. we should check this situation separatly.
            if (IsNeedCast)
            {
                result = $"({GetTableName(TableType)}.[{Column}] != '' AND {{0}})";
            }

            if ((TableType & TableTypes.Expression) == TableTypes.Expression)
            {
                result = String.Format(result, Column);
            }
            else
            {
                string fieldExp = VariableType == VariableTypes.String
                    ? $"ISNULL({GetTableName(TableType)}.[{Column}], '')"
                    : $"{GetTableName(TableType)}.[{Column}]";
                result = String.Format(result, $"({fieldExp}{GetOperatorString(Operator)}{GetValueString(Operator, Value, VariableType)})");
            }

            return result;
        }
    }
}
