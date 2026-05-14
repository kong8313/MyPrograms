using System;

namespace Confirmit.CATI.Console.LightweightTelephony
{
    public interface ICustomDialer
    {
        void Dial(string phoneNumber);

        void HangUp();

        event EventHandler<CallStatusChangedEventArgs> CallStatusChanged;
    }
}