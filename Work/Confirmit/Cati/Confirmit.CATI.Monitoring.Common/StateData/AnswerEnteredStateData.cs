using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    /// <summary>
    /// Represents data of answer html element
    /// </summary>
    [Serializable]
    public class AnswerEnteredStateData : BaseStateData
    {
        /// <summary>
        /// Gets or sets Html element id
        /// </summary>
        public string ElementId
        {
            get;
            set;
        }       

        /// <summary>
        /// Gets or sets Html element value
        /// </summary>
        public string ElementValue
        {
            get;
            set;
        }
    }
}
