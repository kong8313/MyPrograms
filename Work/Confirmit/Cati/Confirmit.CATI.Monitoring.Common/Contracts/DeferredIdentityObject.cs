using System;
using System.Runtime.Serialization;

namespace Confirmit.CATI.Monitoring.Common.Contracts
{
    /// <summary>
    /// Class of CATI Console identity for deferred monitoring.
    /// </summary>
    [DataContract(Namespace = "http://www.confirmit.com/")]
    public class DeferredIdentityObject : ICloneable
    {
        /// <summary>
        /// Gets/sets deferred session identifier.
        /// </summary>
        [DataMember]
        public int DeferredRecordId { get; set; }

        /// <summary>
        /// Gets/sets survey identifier.
        /// </summary>
        [DataMember]
        public int SurveySID { get; set; }

        /// <summary>
        /// Gets/sets interview identifier.
        /// </summary>
        [DataMember]
        public int InterviewID { get; set; }

        /// <summary>
        /// Determines whether the specified DeferredIdentityObject is equal to the current DeferredIdentityObject.
        /// </summary>
        /// <param name="obj">The DeferredIdentityObject to compare with current.</param>
        /// <returns>true, if objects are equal; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is DeferredIdentityObject))
            {
                return false;
            }

            var second = (DeferredIdentityObject)obj;

            return (DeferredRecordId == second.DeferredRecordId &&
                    SurveySID == second.SurveySID &&
                    InterviewID == second.InterviewID);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return DeferredRecordId.GetHashCode() ^ SurveySID.GetHashCode() ^ InterviewID;
        }

        /// <summary>
        /// Overloading of equality operator.
        /// </summary>
        /// <param name="x">First argument.</param>
        /// <param name="y">Second argument.</param>
        /// <returns>True, if both arguments are equal; false, otherwise.</returns>
        public static bool operator ==(DeferredIdentityObject x, DeferredIdentityObject y)
        {
            if (ReferenceEquals(x, null))
            {
                if (ReferenceEquals(y, null))
                {
                    return true;
                }
                
                return false;
            }
            
            return x.Equals(y);
        }

        /// <summary>
        /// Overloading of inequality operator.
        /// </summary>
        /// <param name="x">First argument.</param>
        /// <param name="y">Second argument.</param>
        /// <returns>True, if both arguments are not equal; false, otherwise.</returns>
        public static bool operator !=(DeferredIdentityObject x, DeferredIdentityObject y)
        {
            return !(x == y);
        }

        /// <summary>
        /// Clones object of identity for deferred monitoring.
        /// </summary>
        /// <returns>Cloned object of identity for deferred monitoring.</returns>
        public object Clone()
        {
            var clone = new DeferredIdentityObject
            {
                DeferredRecordId = DeferredRecordId,
                InterviewID = InterviewID,
                SurveySID = SurveySID
            };

            return clone;
        }
    }
}