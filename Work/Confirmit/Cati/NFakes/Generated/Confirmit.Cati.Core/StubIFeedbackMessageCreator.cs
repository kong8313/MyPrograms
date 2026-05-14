using System;
using Confirmit.CATI.Core.Mail.Feedback;
using Confirmit.CATI.Core.Mail;

namespace Confirmit.CATI.Core.Mail.Feedback.Fakes
{
    public class StubIFeedbackMessageCreator : IFeedbackMessageCreator 
    {
        private IFeedbackMessageCreator _inner;

        public StubIFeedbackMessageCreator()
        {
            _inner = null;
        }

        public IFeedbackMessageCreator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate MailMessage GetMailMessageFeedbackFormDelegate(FeedbackForm form);
        public GetMailMessageFeedbackFormDelegate GetMailMessageFeedbackForm;

        MailMessage IFeedbackMessageCreator.GetMailMessage(FeedbackForm form)
        {


            if (GetMailMessageFeedbackForm != null)
            {
                return GetMailMessageFeedbackForm(form);
            } else if (_inner != null)
            {
                return ((IFeedbackMessageCreator)_inner).GetMailMessage(form);
            }

            return default(MailMessage);
        }

    }
}