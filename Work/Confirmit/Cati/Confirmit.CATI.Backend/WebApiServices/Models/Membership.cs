using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Confirmit.CATI.Backend.WebApiServices.Models
{
    [Table("BvMembership")]
    public class Membership
    {
        [Key]
        public int id { get; set; }
        public int ContainerSID { get; set; }
        public int ObjectSID { get; set; }
    }
}
