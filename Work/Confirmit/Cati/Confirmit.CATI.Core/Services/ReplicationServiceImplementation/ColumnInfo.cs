using System.Runtime.Serialization;
using Microsoft.SqlServer.Management.Smo;

namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation
{
    /// <summary>
    /// Information about database table column.
    /// </summary>
    [DataContract(Namespace = "http://www.confirmit.com/ManagementService/18/08/2009/ColumnInfo")]
    public class ColumnInfo
    {
        /// <summary>
        /// Simple column name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// SQL type of column as <see cref="Microsoft.SqlServer.Management.Smo.SqlDataType"/>.
        /// </summary>
        [DataMember]
        public SqlDataType DataType { get; set; }

        /// <summary>
        /// Maximum length of SQL type of column. Used for string types only. Ignored for integer types.
        /// </summary>
        [DataMember]
        public int MaxLength { get; set; }

        /// <summary>
        /// Gets or sets the numeric precision of the data type.
        /// </summary>
        [DataMember]
        public int NumericPrecision { get; set; }

        /// <summary>
        /// Gets or sets the numeric scale of the data type.
        /// </summary>
        [DataMember]
        public int NumericScale { get; set; }
    }
}