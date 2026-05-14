using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.UsersApi
{
    public class ConfirmitUser
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int LanguageId { get; set; }
        public Dictionary<string, string> Links { get; set; }
        public string EncryptionKeyId { get; set; }
    }
}