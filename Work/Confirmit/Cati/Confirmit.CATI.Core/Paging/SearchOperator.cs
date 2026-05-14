using System;

namespace Confirmit.CATI.Core.Paging
{
    /// <summary>
    /// Represents list of operator supported by our list search procedure.
    /// </summary>
    public enum SearchOperator
    {
        /// <summary>
        /// Operator ==
        /// </summary>
        Equal,

        /// <summary>
        /// Operator !=
        /// </summary>
        NotEqual,

        /// <summary>
        /// Operator &lt;
        /// </summary>
        Less,

        /// <summary>
        /// Operator >
        /// </summary>
        Greater,

        /// <summary>
        /// Operator &lt;=
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// Operator >=
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// Operator LIKE
        /// </summary>
        Like,

        IsNullOrEmpty       // will translate to NULLIF( expression, '' ) IS NULL  in SQL
    }
}
