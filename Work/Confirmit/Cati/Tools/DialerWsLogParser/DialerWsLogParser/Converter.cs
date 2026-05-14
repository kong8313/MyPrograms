using System;
using DialerWsLogParserLibrary;

namespace DialerWsLogParser
{
    internal class Converter
    {
        public EventView EventToEventView(Event tableEntry)
        {
            return new EventView
            {
                Id = tableEntry.Id,
                RequestId = tableEntry.RequestId,
                Icon = GetIcon(tableEntry.Icon),
                Name = tableEntry.Name,
                Time = tableEntry.Time,
                CompanyId = tableEntry.CompanyId,
                DialerId = tableEntry.DialerId,
                CampaignId = tableEntry.CampaignId,
                AgentId = tableEntry.AgentId,
                CallId = tableEntry.CallId,
                InterviewId = tableEntry.InterviewId,
                Duration = tableEntry.Duration,
                AllInfo = tableEntry.AllInfo,
                IsMatchesCondition = tableEntry.IsHighlighted
            };
        }

        private Uri GetIcon(IconType tableEntryIcon)
        {
            switch (tableEntryIcon)
            {
                case IconType.Verbose:
                    return new Uri(System.IO.Path.GetFullPath(@"images\verbose.png"));
                case IconType.Info:
                    return new Uri(System.IO.Path.GetFullPath(@"images\info.png"));
                case IconType.Warning:
                    return new Uri(System.IO.Path.GetFullPath(@"images\warning.png"));
                case IconType.Error:
                    return new Uri(System.IO.Path.GetFullPath(@"images\error.png"));
                default:
                    return null;
            }
        }
    }
}
