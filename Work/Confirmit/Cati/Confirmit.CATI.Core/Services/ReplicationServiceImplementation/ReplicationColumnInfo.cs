using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation
{
    /// <summary>
    /// Database table column details for data replication.
    /// </summary>
    [DataContract(Namespace = "http://www.confirmit.com/ManagementService/18/08/2009/ReplicationColumnInfo")]
    public class ReplicationColumnInfo : ColumnInfo, IEquatable<ReplicationColumnInfo>
    {
        /// <summary>
        /// Column ID.
        /// Use TSQL method 
        /// <code>
        /// COLUMNPROPERTY(object_id('&lt;table name>'), '&lt;column name>', 'ColumnId')
        /// </code>
        /// to get Column ID.
        /// See http://msdn.microsoft.com/en-us/library/ms174968.aspx for details.
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// An array of identifiers of quotas, which belongs to the variable.
        /// </summary>
        [DataMember]
        public int[] QuotaIds { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ReplicationColumnInfo);
        }

        public bool Equals(ReplicationColumnInfo other)
        {
            return other != null &&
                   Name == other.Name &&
                   DataType == other.DataType &&
                   MaxLength == other.MaxLength &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            int hashCode = -1817077156;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + DataType.GetHashCode();
            hashCode = hashCode * -1521134295 + MaxLength.GetHashCode();
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            return hashCode;
        }
    }
}