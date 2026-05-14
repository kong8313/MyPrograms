namespace Confirmit.CATI.Supervisor.Core.Assignment
{
    /// <summary>
    /// Class contains single information item of person assignments.
    /// </summary>
    public class PersonAssignmentInfoItem
    {
        /// <summary>
        /// Gets or sets the survey SID.
        /// </summary>
        public int SurveySID { get; set; }

        /// <summary>
        /// Gets or sets Confirmit project ID.
        /// </summary>
        public string ProjectID { get; set; }

        /// <summary>
        /// Gets or sets Confirmit project name.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the assigned calls count.
        /// </summary>
        public int AssignedCallsCount { get; set; }

        /// <summary>
        /// Gets/sets type of assignment. 
        /// 0 means implicit assignment by group,
        /// 1 means explicit assignment.
        /// 2 means implicit assignment by call.
        /// </summary>
        public int AssignmentType { get; set; }

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

            PersonAssignmentInfoItem item = (PersonAssignmentInfoItem)obj;
            return 
                SurveySID == item.SurveySID && 
                ProjectID == item.ProjectID && 
                ProjectName == item.ProjectName && 
                AssignedCallsCount == item.AssignedCallsCount &&
                AssignmentType == item.AssignmentType;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return SurveySID.GetHashCode() ^ 
                ProjectID.GetHashCode() ^ 
                ProjectName.GetHashCode() ^ 
                AssignedCallsCount.GetHashCode() ^ 
                AssignmentType.GetHashCode();
        }
    }
}