using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Confirmit.CATI.Backend.WebApiServices.Models
{
    /// <summary>
    /// Class representing information about the group of interviewers
    /// </summary>
    [Table("RestView_Group")]
    public class Group
    {
        public Group()
        {
            ParentGroups = new List<int>();
        }

        /// <summary>
        /// Unique identifier of the group
        /// </summary>
        [Key]
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
