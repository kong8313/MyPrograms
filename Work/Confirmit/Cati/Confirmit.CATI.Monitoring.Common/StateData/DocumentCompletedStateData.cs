using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    /// <summary>
    /// Contains data for DocumentCompleted event
    /// </summary>
    [Serializable]
    public class DocumentCompletedStateData : BaseStateData
    {
        /// <summary>
        /// Gets or sets current page content
        /// </summary>
        public string PageContent
        {
            get;
            set;
        }

    }
}
