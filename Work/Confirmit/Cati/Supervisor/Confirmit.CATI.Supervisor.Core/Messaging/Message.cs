using System;

namespace Confirmit.CATI.Supervisor.Core.Messaging
{
    /// <summary>
    /// Represents message 
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Message body
        /// </summary>
        public string Body
        {
            get;
            set;
        }
        
        /// <summary>
        /// Name of supervisor who send message
        /// </summary>
        public string SupervisorName
        {
            get;
            set;
        }
    }
}
