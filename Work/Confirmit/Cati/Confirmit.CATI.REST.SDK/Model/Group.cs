using System.Collections.Generic;

namespace Confirmit.CATI.REST.SDK.Model
{
    /// <summary>
    /// Class representing information about the group of interviewers
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Creates and initializes an instance of Group class
        /// </summary>
        public Group()
        {
            ParentGroups = new List<int> {Constants.Constants.CatiInterviewersRootGroupId};
            Description = "";
        }

        /// <summary>
        /// Unique identifier of the group 
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// Name of the group
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the group
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// List of unique identifiers of parent group. Deprecated in the new design.
        /// </summary>
        public List<int> ParentGroups { get; set; }
    }
}