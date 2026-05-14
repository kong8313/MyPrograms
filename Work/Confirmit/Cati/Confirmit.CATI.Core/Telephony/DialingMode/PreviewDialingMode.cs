namespace Confirmit.CATI.Core.Telephony.DialingWorkflow
{
    public class PreviewDialingMode : DialingMode
    {
        public PreviewDialingMode(bool isSpecial)
            : base(isSpecial ? ConfirmitDialerInterface.DialingMode.SpecialDial : ConfirmitDialerInterface.DialingMode.Preview)
        {
            
        }


    }
}
