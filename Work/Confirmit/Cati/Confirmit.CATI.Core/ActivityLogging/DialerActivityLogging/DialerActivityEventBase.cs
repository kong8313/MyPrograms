using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.Logger;
using Confirmit.Logging;

namespace Confirmit.CATI.Core.ActivityLogging.DialerActivityLogging
{
    public abstract class DialerActivityEventBase : ActivityEventBase
    {
        public int DialerId { get; }
        protected CustomField[] EventFields { get; set; }
        protected Dictionary<string, string> Details { get; }

        protected DialerActivityEventBase(int dialerId, string eventName) : base(eventName,
            "Dialer activity: " + eventName)
        {
            DialerId = dialerId;
            EventFields = new CustomField[0];
            Details = new Dictionary<string, string>();
        }

        protected override void Save(LogLevel logLevel, CustomField[] eventFields)
        {
            var fields = GetDialerActivityCommonFields().ConcatWithReplace(eventFields);

            base.Save(logLevel, fields);

            CustomMetrics.OnActivityEvent("Dialer", EventName, TimeSpan.FromMilliseconds(Duration));
        }

        private CustomField[] GetDialerActivityCommonFields()
        {
            var commonFields = new[]
            {
                new CustomField("ActivityType", "Dialer"),
                new CustomField("DialerId", DialerId),
            };

            return commonFields;
        }

        protected void WriteLog(LogLevel logLevel)
        {
            AddDetailsField();
            Save(logLevel, EventFields);
        }

        private void AddDetailsField()
        {
            var fields = new[]
            {
                new CustomField("Details", DetailsToYaml() ?? "-"),
            };

            EventFields = EventFields.ConcatWithReplace(fields);
        }

        private string DetailsToYaml()
        {
            if (!Details.Any()) return null;

            return Details.ToYaml();
        }
    }
}