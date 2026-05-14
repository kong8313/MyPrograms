using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Misc;
using Confirmit.Logging;

namespace Confirmit.CATI.Core.ActivityLogging
{
    public abstract class ActivityEventBase 
    {
        protected string EventName { get; }
        private string Message { get; }
        private readonly Stopwatch _stopwatch;
        protected long Duration { get; private set; }
        private ICompanyInfo CompanyInfo { get; }

        protected ActivityEventBase(string eventName, string message) 
        {
            EventName = eventName;
            Message = message;
            _stopwatch = Stopwatch.StartNew();
            CompanyInfo = ServiceLocator.Resolve<ICompanyInfo>();
        }

        protected virtual void Save(LogLevel logLevel, CustomField[] eventFields)
        {
            Duration = _stopwatch.ElapsedMilliseconds;
            
            var logWriter = ServiceLocator.Resolve<ILogWriter>();

            var fields = LogData.ToCustomFields().ConcatWithReplace(GetEventCommonFields());
            fields = fields.ConcatWithReplace(eventFields);

            logWriter.Write(logLevel, Message, fields);
        }

        private CustomField[] GetEventCommonFields()
        {
            var commonFields = new[]
            {
                new CustomField("ActivityName", EventName),
                new CustomField("CompanyId", CompanyInfo.CompanyId),
                new CustomField("CompanyName", CompanyInfo.CompanyName),
                new CustomField("Duration", (int) Duration),
            };

            return commonFields;
        }
    }

    public static class CustomFieldsExtension
    {
        public static CustomField[] ConcatWithReplace(this CustomField[] origin, CustomField[] adding)
        {
            if (origin == null && adding == null) return Array.Empty<CustomField>();
            if (origin == null) return adding;
            if (adding == null) return origin;

            var originWithoutNewFieldDuplicates = new List<CustomField>();
            originWithoutNewFieldDuplicates.AddRange(origin.Where(o => adding.All(a => a.Name != o.Name)));

            return originWithoutNewFieldDuplicates.Concat(adding).ToArray();
        }
    }


}