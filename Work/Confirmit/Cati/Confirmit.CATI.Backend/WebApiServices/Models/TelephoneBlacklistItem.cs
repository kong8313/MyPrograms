using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Backend.WebApiServices.Models
{
    /// <summary>
    /// Class representing information about the item in the blacklist
    /// </summary>
    [Table("BvTelephoneBlacklist")]
    public class TelephoneBlacklistItem
    {
        /// <summary>
        /// Unique identifier of the blacklist item
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Telephone number
        /// </summary>
        public string TelephoneNumber { get; set; }

        /// <summary>
        /// Type of the telephone number
        /// </summary>
        [Column(TypeName = "tinyint")]
        public BlacklistPatternType Type { get; set; }
    }
}
