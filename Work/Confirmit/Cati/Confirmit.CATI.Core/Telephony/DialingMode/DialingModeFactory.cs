using System;

namespace Confirmit.CATI.Core.Telephony.DialingWorkflow
{
    public class DialingModeFactory
    {
        public static IDialingMode CreateDialingMode(ConfirmitDialerInterface.DialingMode dialingMode)
        {
            switch (dialingMode)
            {
                case ConfirmitDialerInterface.DialingMode.Manual:
                    return new ManualDialingMode();
                case ConfirmitDialerInterface.DialingMode.SpecialDial:
                    return new PreviewDialingMode(true);
                case ConfirmitDialerInterface.DialingMode.Preview:
                    return new PreviewDialingMode(false);
                case ConfirmitDialerInterface.DialingMode.Automatic:
                    return new AutomaticDialingMode();
                case ConfirmitDialerInterface.DialingMode.Predictive:
                    return new PredictiveDialingMode();
                default:
                    throw new Exception(string.Format("Unknown dialer mode: {0}.", dialingMode));
            }
        }
    }
}
