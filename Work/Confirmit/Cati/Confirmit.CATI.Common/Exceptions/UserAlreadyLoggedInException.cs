using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Confirmit.CATI.Common.Exceptions
{
    [Serializable]
    public class UserAlreadyLoggedInException : UserMessageException
    {
        /// <summary>
        /// StationId from which user already logged in - first stationId
        /// </summary>
        public string FirstStationId { get; set; }

        /// <summary>
        /// StationId from which user tries to login again - second stationId
        /// </summary>
        public string SecondStationId { get; set; }

        public UserAlreadyLoggedInException()
        {
        }

        public UserAlreadyLoggedInException(string message, string firstStationId, string secondStationId)
            : base(message)
        {
            FirstStationId = firstStationId;
            SecondStationId = secondStationId;
        }

        public UserAlreadyLoggedInException(
            string message, string messageKey, string firstStationId, string secondStationId)
            : base(message)
        {
            MessageKey = messageKey;
            FirstStationId = firstStationId;
            SecondStationId = secondStationId;
        }

        public UserAlreadyLoggedInException(
            string message, string firstStationId, string secondStationId, Exception innerException)
            : base(message, innerException)
        {
            FirstStationId = firstStationId;
            SecondStationId = secondStationId;
        }

        protected UserAlreadyLoggedInException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
            {
                FirstStationId = info.GetString("FirstStationId");
                SecondStationId = info.GetString("SecondStationId");
            }
        }

        public override FaultException ToFault()
        {
            return new FaultException<UserAlreadyLoggedInExceptionDetails>(
                new UserAlreadyLoggedInExceptionDetails
                {
                    Message = Message, 
                    MessageKey = MessageKey,
                    FirstStationId = FirstStationId,
                    SecondStationId = SecondStationId
                },
                Message);
        }
    }

    /// <summary>
    /// Details of UserAlreadyLoggedInException fault.
    /// </summary>
    public class UserAlreadyLoggedInExceptionDetails : UserMessageExceptionDetails
    {
        public string FirstStationId { get; set; }
        public string SecondStationId { get; set; }

        /// <summary>
        /// Constructs the <see cref="UserAlreadyLoggedInException"/> based on current details.
        /// </summary>
        /// <returns></returns>
        public override UserMessageException ToException()
        {
            return new UserAlreadyLoggedInException(
                Message, MessageKey, FirstStationId, SecondStationId);
        }
    }
}