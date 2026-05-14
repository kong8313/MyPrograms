using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Common.Exceptions
{
    public class DialerStartCampaignException : UserMessageException
    {
        private readonly List<DialerInfo> _dialers;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialerStartCampaignException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="dialers">The dialers info.</param>
        public DialerStartCampaignException(string message, List<DialerInfo> dialers)
            : base(message)
        {
            _dialers = dialers;
        }

        /// <summary>
        /// Constructs the <see cref="FaultException"/> based on current exception details.
        /// </summary>
        /// <returns></returns>
        public override FaultException ToFault()
        {
            return new FaultException<DialerStartCampaignExceptionDetails>(
                new DialerStartCampaignExceptionDetails() {
                    Message = Message, MessageKey = MessageKey, Dialers = _dialers
                }, Message);
        }
    }

    public class DialerStartCampaignExceptionDetails : UserMessageExceptionDetails
    {
        public List<DialerInfo> Dialers;

        /// <summary>
        /// Constructs the <see cref="DialerStartCampaignException"/> based on current details.
        /// </summary>
        /// <returns></returns>
        public override UserMessageException ToException()
        {
            return new DialerStartCampaignException(Message, Dialers);
        }
    }

    public class DialerInfo
    {
        public int Id { get; set; }

        public DialerErrorCode ErrorCode { get; set; }

        public string Name { get; set; }
    }
}