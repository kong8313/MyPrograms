namespace Confirmit.CATI.Core.Mail.Feedback
{
    public interface IFeedbackMessageCreator
    {
        MailMessage GetMailMessage(FeedbackForm form);
    }
}