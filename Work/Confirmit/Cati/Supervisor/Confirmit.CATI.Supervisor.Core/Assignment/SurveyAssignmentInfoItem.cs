namespace Confirmit.CATI.Supervisor.Core.Assignment
{
    /// <summary>
    /// Class contains single information item of survey assignments.
    /// </summary>
    public class SurveyAssignmentInfoItem
    {
        /// <summary>
        /// Gets or sets the person or person group SID.
        /// </summary>
        public int SID { get; set; }

        /// <summary>
        /// Gets or sets the person or person group name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is group.
        /// </summary>
        /// <value><c>true</c> if this item is group; otherwise, <c>false</c>.</value>
        public bool IsGroup { get; set; }

        /// <summary>
        /// Gets or sets the assigned calls count.
        /// </summary>
        public int AssignedCallsCount { get; set; }

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

            SurveyAssignmentInfoItem item = (SurveyAssignmentInfoItem)obj;
            return 
                SID == item.SID && 
                Name == item.Name && 
                IsGroup == item.IsGroup && 
                AssignedCallsCount == item.AssignedCallsCount;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return SID.GetHashCode() ^ Name.GetHashCode() ^ IsGroup.GetHashCode() ^ AssignedCallsCount.GetHashCode();
        }
    }
}