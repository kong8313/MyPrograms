using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    [Serializable]
    public class ShowInformationStateData : BaseStateData
    {
        #region Properties

        /// <summary>
        /// Text of message
        /// </summary>
        public string Message { get; set; }

        #endregion
    }
}