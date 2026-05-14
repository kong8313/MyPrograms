using System.Threading;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    internal class PreviewCall
    {
        public ManualResetEvent HangupEvent { get; set; }
    }
}