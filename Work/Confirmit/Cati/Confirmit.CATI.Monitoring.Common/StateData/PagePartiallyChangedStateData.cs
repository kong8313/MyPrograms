using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    [Serializable]
    public class PagePartiallyChangedStateData : BaseStateData
    {
        public string ElementId { get; set; }

        public string ElementOuterHtml { get; set; }

        public string ActiveElementId { get; set; }
    }
}