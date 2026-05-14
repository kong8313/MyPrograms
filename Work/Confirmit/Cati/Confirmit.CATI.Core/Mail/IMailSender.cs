namespace Confirmit.CATI.Core.Mail
{
    /// <summary>
    /// Define interface for sending e-mail.
    /// </summary>
    public interface IMailSender
    {
        /// <summary>
        /// Send e-mail.
        /// </summary>
        /// <param name="message">Message to be sent</param>
        void SendMail(MailMessage message);
    }

}
