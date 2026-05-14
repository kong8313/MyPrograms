using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation
{
    /// <summary>
    /// Database table details for data replication.
    /// </summary>
    [DataContract(Namespace = "http://www.confirmit.com/ManagementService/18/08/2009/TableInfo")]
    public class TableInfo : IEquatable<TableInfo>
    {
        /// <summary>
        /// Simple table name without any prefixes and square brackets.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Array of table columns used for replication.
        /// </summary>
        [DataMember]
        public ReplicationColumnInfo[] ReplicationColumns { get; set; }

        /// <summary>
        /// Array of primary key columns of the table.
        /// </summary>
        [DataMember]
        public ColumnInfo[] PrimaryKeyColumns { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as TableInfo);
        }

        public bool Equals(TableInfo other)
        {
            return other != null &&
                   Name == other.Name &&
                   ReplicationColumns.SequenceEqual(other.ReplicationColumns);
        }

        public override int GetHashCode()
        {
            int hashCode = 1469036758;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<ReplicationColumnInfo[]>.Default.GetHashCode(ReplicationColumns);
            return hashCode;
        }
    }
}