using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Confirmit.CATI.Monitoring.Common.Contracts
{
    /// <summary>
    /// Class of CATI Console identity.
    /// </summary>
    [DataContract( Namespace = "http://www.confirmit.com/" )]
    public class IdentityObject : ICloneable
    {
        /// <summary>
        /// ID of company.
        /// </summary>
        [DataMember]
        public int CompanyID { get; set; }

        /// <summary>
        /// Alias of company.
        /// </summary>
        [DataMember]
        public string CompanyAlias { get; set; }

        /// <summary>
        /// ID of interviewer.
        /// </summary>
        [DataMember]
        public int InterviewerID { get; set; }

        /// <summary>
        /// ID of monitoring session.
        /// </summary>
        [DataMember]
        public long MonitoringSessionID { get; set; }

        /// <summary>
        /// ID of deferred monitoring session.
        /// </summary>
        [DataMember]
        public DeferredIdentityObject DeferredIdentity { get; set; }

        /// <summary>
        /// Returns string representation of identity object.
        /// </summary>
        /// <returns>String representation of identity object.</returns>
        [DebuggerStepThrough]
        public override string ToString()
        {
            return string.Format( 
                "CompanyID: {0}, InterviewerID: {1}, MonitoringSessionID: {2}, DeferredMonitoringRecordID: {3}", 
                CompanyID, 
                InterviewerID, 
                MonitoringSessionID,
                (DeferredIdentity == null) ? "None" : DeferredIdentity.DeferredRecordId.ToString(CultureInfo.InvariantCulture) );
        }

        /// <summary>
        /// Returns hash code of object.
        /// </summary>
        /// <returns>Hash code of object.</returns>
        [DebuggerStepThrough]
        public override int GetHashCode()
        {
            var hashCode = string.Format( "{0}:{1}:{2}", CompanyID, InterviewerID, MonitoringSessionID ).GetHashCode();

            return DeferredIdentity == null ? hashCode : hashCode ^ DeferredIdentity.GetHashCode();
        }

        /// <summary>
        /// Are to given object equal to this.
        /// </summary>
        /// <param name="obj">Another object.</param>
        /// <returns>True is another object equal to this. False, otherwise.</returns>
        [DebuggerStepThrough]
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is IdentityObject))
            {
                return false;
            }

            var another = (IdentityObject) obj;

            if (CompanyID != another.CompanyID)
            {
                return false;
            }

            if (InterviewerID != another.InterviewerID)
            {
                return false;
            }

            if (MonitoringSessionID != another.MonitoringSessionID)
            {
                return false;
            }

            if (DeferredIdentity != another.DeferredIdentity)
            {
                return false;
            }
			
            return true;
        }

        /// <summary>
        /// Clones object of identity.
        /// </summary>
        /// <returns>Cloned object of identity.</returns>
        public object Clone()
        {
            var clone = new IdentityObject
            {
                CompanyID = CompanyID,
                InterviewerID = InterviewerID,
                MonitoringSessionID = MonitoringSessionID,
                DeferredIdentity = (DeferredIdentityObject) DeferredIdentity.Clone()
            };

            return clone;
        }
    }
}