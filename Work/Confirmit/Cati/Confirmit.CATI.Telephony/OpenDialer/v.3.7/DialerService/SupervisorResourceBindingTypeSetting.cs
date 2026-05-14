using System;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony.DialerService
{
    public class SupervisorResourceBindingTypeSetting
    {
        private readonly ResourceBindingType _supervisorResourceBindingType;
        public bool IsSet { get; private set; }

        public SupervisorResourceBindingTypeSetting(ILogger logger)
        {
            IsSet = Enum.TryParse(Settings.Default.SupervisorResourceBindingType, out _supervisorResourceBindingType);

            if (IsSet)
            {
                IsSet = Enum.IsDefined(typeof(ResourceBindingType), _supervisorResourceBindingType);
            }

            if (!IsSet && (Settings.Default.SupervisorResourceBindingType != "NotDefined"))
            {
                logger.Warning(
                    "SupervisorResourceBindingTypeSetting.Ctor",
                    "Incorrect value is set for SupervisorResourceBindingType: [{0}]",
                    Settings.Default.SupervisorResourceBindingType);
            }
        }

        public ResourceBindingType Get()
        {
            if (!IsSet)
            {
                throw new DialerException(DialerErrorCode.Exception, "Supervisor resource binding type is not specified.");
            }

            return _supervisorResourceBindingType;
        }
    }
}