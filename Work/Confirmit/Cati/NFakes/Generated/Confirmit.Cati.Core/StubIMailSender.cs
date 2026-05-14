using System;
using Confirmit.CATI.Core.Mail;

namespace Confirmit.CATI.Core.Mail.Fakes
{
    public class StubIMailSender : IMailSender 
    {
        private IMailSender _inner;

        public StubIMailSender()
        {
            _inner = null;
        }

        public IMailSender Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SendMailMailMessageDelegate(MailMessage message);
        public SendMailMailMessageDelegate SendMailMailMessage;

        void IMailSender.SendMail(MailMessage message)
        {

            if (SendMailMailMessage != null)
            {
                SendMailMailMessage(message);
            } else if (_inner != null)
            {
                ((IMailSender)_inner).SendMail(message);
            }
        }

    }
}