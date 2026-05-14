using System;

namespace Confirmit.CATI.Core.Telephony.Connection
{
    public class DialerConnectionState
    {
        public enum ConnectionState
        {
            Alive,
            EventNotificationsDown,
            DialerWebserviceDown
        }

        public ConnectionState CurrentState { get; set; }
            
        public string Error { get; set; }
        public bool IsAlive => CurrentState == ConnectionState.Alive;

        public override string ToString()
        {
            switch (CurrentState)
            {
                case ConnectionState.Alive:
                    return "Dialer connection alive";
                case ConnectionState.EventNotificationsDown:
                    return "Connection to dialer is established, but dialer events do not arrive to Confirmit CATI. " 
                        + "It may indicate temporary network problems or wrong configuration of dialer webservice. " + Error;
                case ConnectionState.DialerWebserviceDown:
                    return "Cannot establish connection to dialer." + Error;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}