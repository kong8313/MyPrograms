using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    /// <summary>
    /// Represents state data of dialing start operation. Constains phone and message.
    /// </summary>
    [Serializable]
    public class DialStartData : BaseStateData
    {
        #region Constructors

        /// <summary>
        /// Intializes new instance of DialStartData class.
        /// </summary>
        public DialStartData()
            : base()
        {
        }

        /// <summary>
        /// Intialiazes new instance of DialStartData and fills it with given data.
        /// </summary>
        /// <param name="phone">Phone number.</param>
        /// <param name="message">Message.</param>
        public DialStartData(string phone, string message)
            : base()
        {
            Phone = phone;
            Message = message;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets dialing phone.
        /// </summary>
        public string Phone
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets dialing message.
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        #endregion
    }
}
