using System.Runtime.Serialization;

namespace Confirmit.CATI.Core.Mail.Feedback
{
    [DataContract]
    public enum FeedbackCategory
    {
        [EnumMember]
        Suggestion,
        
        [EnumMember]
        Bug
    }
}