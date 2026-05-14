using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    /// <summary>
    /// Represents data of scroll position in the responsive question
    /// </summary>
    [Serializable]
    public class ResponsiveQuestionScrolledData : BaseStateData
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
        /// Gets or sets Html element scroll position
        /// </summary>
        public double ScrollPosition
        {
            get;
            set;
        }
    }
}