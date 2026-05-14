using System;
using Confirmit.CATI.Monitoring.Common.StateData;

namespace Confirmit.CATI.Core.Services.RecordsMigration
{
    public class MonitoringEvent
    {
        public DateTime TimeStamp { get; set; }

        public int MessageType { get; set; }

        public BaseStateData Data { get; set; }
    }
}