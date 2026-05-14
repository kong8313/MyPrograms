using System.Runtime.Serialization;

namespace Confirmit.CATI.Core.Mail.Feedback
{
    [DataContract]
    public class FeedbackForm
    {
        public FeedbackForm()
        {
            Summary = string.Empty;
            Description = string.Empty;

            CompanyName = string.Empty;
            
            ContactName = string.Empty;
            ContactEmail = string.Empty;

            AuthorizedUserLogin = string.Empty;
            AuthorizedUserEmail = string.Empty;
            AuthorizedUserName = string.Empty;
        }

        [DataMember]
        public string Summary { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public FeedbackCategory Category { get; set; }

        [DataMember]
        public string CompanyName { get; set; }

        [DataMember]
        public string ContactName { get; set; }

        [DataMember]
        public string ContactEmail { get; set; }

        [DataMember]
        public string AuthorizedUserLogin { get; set; }

        [DataMember]
        public string AuthorizedUserEmail { get; set; }

        [DataMember]
        public string AuthorizedUserName { get; set; }

        [DataMember]
        public int CompanyId { get; set; }
    }
}