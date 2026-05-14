using System;

namespace Confirmit.CATI.Console.LightweightTelephony
{
    public class CallStatusChangedEventArgs : EventArgs
    {
        public CustomCallOutcome CustomCallStatus;
    }
}