using System;
using Confirmit.CATI.Supervisor.Core.Assignment;

namespace Confirmit.CATI.Supervisor.Backend.Assignment
{
    public class PersonAssignmentInfoItemWithGroupName : PersonAssignmentInfoItem
    {
        #region Properties

        /// <summary>
        /// Gets/sets the name of parent group which is assigned to current survey.
        /// This value is set only for person implicit assignment. Otherwise it is 
        /// empty string.
        /// </summary>
        public string ParentGroupName { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            PersonAssignmentInfoItemWithGroupName tmp = (PersonAssignmentInfoItemWithGroupName)obj;
            return base.Equals(tmp) && (ParentGroupName == tmp.ParentGroupName);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return String.IsNullOrEmpty(ParentGroupName) ? base.GetHashCode() : base.GetHashCode() ^ ParentGroupName.GetHashCode();
        }

        #endregion
    }
}