using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation
{
    /// <summary>
    /// This class contains conditions and sub filters
    /// all conditions and subfilters separate one and the same operation OR or AND
    /// </summary>
    public class SqlFilter
    {
        public AndOrOperator AndOrOperator { get; private set; }
        public List<SqlCondition> Conditions { get; private set; }
        public List<SqlFilter> SubFilters { get; private set; }

        public SqlFilter(AndOrOperator andOrOperator)
        {
            AndOrOperator = andOrOperator;
            Conditions = new List<SqlCondition>();
            SubFilters = new List<SqlFilter>();
        }

        public SqlFilter Clone()
        {
            var copy = new SqlFilter(AndOrOperator);
            
            copy.Conditions.AddRange(Conditions);
            copy.SubFilters.AddRange(SubFilters);
            
            return copy;
        }

        public bool IsEmpty()
        {
            if (Conditions.Any())
            {
                return false;
            }

            return !(SubFilters.Any(x => x.IsEmpty() == false));
        }

        /// <summary>
        /// Addition condition.
        /// Condition look like {column} {operator >, =, ... etc} {value}
        /// </summary>
        /// <param name="condition"></param>
        public void AddCondtion(SqlCondition condition)
        {
            Conditions.Add(condition);
        }

        /// <summary>
        /// Addition sub filter, which can contain some
        /// conditions and another subfilters.
        /// </summary>
        /// <param name="filter"></param>
        public void AddFilter(SqlFilter filter)
        {
            SubFilters.Add(filter);
        }

        public TableTypes GetUsedTables()
        {
            TableTypes result = 0;
            foreach (var subFilter in SubFilters)
            {
                result |= subFilter.GetUsedTables();
            }

            foreach (var condition in Conditions)
            {
                result |= condition.TableType;
            }

            return result;
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            result.Append(String.Join(" " + AndOrOperator + " ",
                Conditions.Select(x => x.ToString()).Where(x => !String.IsNullOrEmpty(x)).ToArray()));

            //this delimeter is necessary between conditions and filter.
            //result string looks like:
            //cond1 operator cond2 operator cond3 OPERATOR subfilter1 operator subfilter2
            //if we haven't got subfilters or conditions we heedn't separate them
            if (Conditions.Count > 0 && SubFilters.Count > 0)
            {
                result.Append(' ').
                    Append(AndOrOperator.ToString()).
                    Append(' ');
            }

            result.Append(String.Join(" " + AndOrOperator + " ",
                SubFilters.Select(x => x.ToString()).Where(x => !String.IsNullOrEmpty(x)).ToArray()));

            if (result.Length == 0)
                return null;

            return new StringBuilder("(").Append(result.ToString()).Append(')').ToString();
        }
    }
}