using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIEmailSettings : IEmailSettings 
    {
        private IEmailSettings _inner;

        public StubIEmailSettings()
        {
            _inner = null;
        }

        public IEmailSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private string _AdministratorEmailAddress;
        public Func<string> AdministratorEmailAddressGet;
        public Action<string> AdministratorEmailAddressSetString;

        string IEmailSettings.AdministratorEmailAddress
        {
            get
            {
                if (AdministratorEmailAddressGet != null)
                {
                    return AdministratorEmailAddressGet();
                } else if (_inner != null)
                {
                    return ((IEmailSettings)_inner).AdministratorEmailAddress;
                }

                if (AdministratorEmailAddressSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AdministratorEmailAddress;
                }

                return default(string);
            }

            set
            {
                if (AdministratorEmailAddressSetString != null)
                {
                    AdministratorEmailAddressSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IEmailSettings)_inner).AdministratorEmailAddress = value;
                    return;
                }

                if (AdministratorEmailAddressGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AdministratorEmailAddress = value;
                }

            }
        }

        private string _FeedbackSupportEmailAddress;
        public Func<string> FeedbackSupportEmailAddressGet;
        public Action<string> FeedbackSupportEmailAddressSetString;

        string IEmailSettings.FeedbackSupportEmailAddress
        {
            get
            {
                if (FeedbackSupportEmailAddressGet != null)
                {
                    return FeedbackSupportEmailAddressGet();
                } else if (_inner != null)
                {
                    return ((IEmailSettings)_inner).FeedbackSupportEmailAddress;
                }

                if (FeedbackSupportEmailAddressSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _FeedbackSupportEmailAddress;
                }

                return default(string);
            }

            set
            {
                if (FeedbackSupportEmailAddressSetString != null)
                {
                    FeedbackSupportEmailAddressSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IEmailSettings)_inner).FeedbackSupportEmailAddress = value;
                    return;
                }

                if (FeedbackSupportEmailAddressGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _FeedbackSupportEmailAddress = value;
                }

            }
        }

        private string _NotificationEmailBCC;
        public Func<string> NotificationEmailBCCGet;
        public Action<string> NotificationEmailBCCSetString;

        string IEmailSettings.NotificationEmailBCC
        {
            get
            {
                if (NotificationEmailBCCGet != null)
                {
                    return NotificationEmailBCCGet();
                } else if (_inner != null)
                {
                    return ((IEmailSettings)_inner).NotificationEmailBCC;
                }

                if (NotificationEmailBCCSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _NotificationEmailBCC;
                }

                return default(string);
            }

            set
            {
                if (NotificationEmailBCCSetString != null)
                {
                    NotificationEmailBCCSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IEmailSettings)_inner).NotificationEmailBCC = value;
                    return;
                }

                if (NotificationEmailBCCGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _NotificationEmailBCC = value;
                }

            }
        }

        private string _NotificationEmailRecipients;
        public Func<string> NotificationEmailRecipientsGet;
        public Action<string> NotificationEmailRecipientsSetString;

        string IEmailSettings.NotificationEmailRecipients
        {
            get
            {
                if (NotificationEmailRecipientsGet != null)
                {
                    return NotificationEmailRecipientsGet();
                } else if (_inner != null)
                {
                    return ((IEmailSettings)_inner).NotificationEmailRecipients;
                }

                if (NotificationEmailRecipientsSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _NotificationEmailRecipients;
                }

                return default(string);
            }

            set
            {
                if (NotificationEmailRecipientsSetString != null)
                {
                    NotificationEmailRecipientsSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IEmailSettings)_inner).NotificationEmailRecipients = value;
                    return;
                }

                if (NotificationEmailRecipientsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _NotificationEmailRecipients = value;
                }

            }
        }

        private int _NotificationExceptionLimit;
        public Func<int> NotificationExceptionLimitGet;
        public Action<int> NotificationExceptionLimitSetInt32;

        int IEmailSettings.NotificationExceptionLimit
        {
            get
            {
                if (NotificationExceptionLimitGet != null)
                {
                    return NotificationExceptionLimitGet();
                } else if (_inner != null)
                {
                    return ((IEmailSettings)_inner).NotificationExceptionLimit;
                }

                if (NotificationExceptionLimitSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _NotificationExceptionLimit;
                }

                return default(int);
            }

            set
            {
                if (NotificationExceptionLimitSetInt32 != null)
                {
                    NotificationExceptionLimitSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IEmailSettings)_inner).NotificationExceptionLimit = value;
                    return;
                }

                if (NotificationExceptionLimitGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _NotificationExceptionLimit = value;
                }

            }
        }

    }
}