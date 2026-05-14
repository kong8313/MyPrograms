using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.Paging
{
    /// <summary>
    /// Represents parameter for single search condition for lists search.
    /// </summary>
    [Serializable]
    public class SearchParameter : ICloneable
    {
        #region Properties

        /// <summary>
        /// Gets/sets name of column to search in.
        /// </summary>
        public string ColumnName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets value to search for.
        /// </summary>
        [JsonConverter(typeof(TypedObjectConverter))]
        public object Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets type of column to search.
        /// </summary>
        public SearchColumnType ColumnType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets search operator.
        /// </summary>
        public SearchOperator Operator
        {
            get;
            set;
        }

        #endregion

        public SearchParameter()
        {
        }
        
        public SearchParameter(SearchParameter param)
        {
            ColumnName = param.ColumnName;
            ColumnType = param.ColumnType;
            Operator = param.Operator;
            Value = param.Value;
        }
        
        #region Methods

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            SearchParameter tmp = (SearchParameter)obj;
            return (ColumnName == tmp.ColumnName &&
                ColumnType == tmp.ColumnType &&
                Operator == tmp.Operator &&
                Value == tmp.Value);
        }

        public override int GetHashCode()
        {
            int result = ColumnName.GetHashCode() ^ ColumnType.GetHashCode() ^ Operator.GetHashCode();

            if (Value != null)
            {
                return result ^ Value.GetHashCode();
            }

            return result;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion
    }

    /// <summary>
    /// Represents collection of SearchParameter items.
    /// </summary>
    [Serializable]
    public class SearchParameterCollection : List<SearchParameter>, ICloneable
    {
        public SearchParameterCollection()
        {
        }

        public SearchParameterCollection(IEnumerable<SearchParameter> collection)
            : base(collection)
        {
        }

        public object Clone()
        {
            return new SearchParameterCollection(this.Select(x => (SearchParameter)x.Clone()));
        }
    }
}