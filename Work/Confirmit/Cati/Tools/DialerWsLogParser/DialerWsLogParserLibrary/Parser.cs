using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DialerWsLogParserLibrary
{
    public class Parser
    {
        public List<Event> Events { get; private set; }
        public List<EventsGroup> EventsGroups { get; private set; }
        public List<EventsGroup> FilteredEventsGroups { get; private set; }
        public Settings ParserSettings { get; private set; }

        private RegexParser _regexStringParser;

        public Parser()
        {
            Events = new List<Event>();
            EventsGroups = new List<EventsGroup>();
            FilteredEventsGroups = new List<EventsGroup>();
            ParserSettings = new Settings();

            _regexStringParser = new RegexParser();
        }

        public void Reset()
        {
            Events = new List<Event>();
            EventsGroups = new List<EventsGroup>();
            FilteredEventsGroups = new List<EventsGroup>();
        }

        public List<EventsGroup> ParseEventsAndGroups(List<string> source, ParseSettings parseSettings)
        {
            var isRidDetected = CheckSource(source);

            Events = ParseEvents(source);

            if (isRidDetected)
                EventsGroups = EventsToEventsGroupsWithRId(FilterEventsWithRid(Events, parseSettings));
            else
                EventsGroups = EventsToEventsGroupsWithoutRId(Events, parseSettings);

            return EventsGroups;
        }

        private List<Event> FilterEventsWithRid(List<Event> events, ParseSettings parseSettings)
        {
            return events.Where(@event => ShouldIncludeEvent(@event, parseSettings)).ToList();
        }

        private bool CheckSource(List<string> source)
        {
            return source.Any(l => l.Contains("[rid="));
        }

        private List<EventsGroup> EventsToEventsGroupsWithRId(List<Event> events)
        {
            var result = new List<EventsGroup>();

            var groupDictionary = new Dictionary<long, EventsGroup>();

            foreach (var entry in events)
            {
                long rid = entry.RequestId;

                if (!groupDictionary.ContainsKey(rid))
                    groupDictionary.Add(rid, new EventsGroup(entry.RequestId, entry.Name, entry.Time, entry.Time, entry.CompanyId,
                        entry.DialerId, entry.CampaignId, entry.AgentId, entry.CallId, entry.InterviewId, entry.Duration));
                else
                {
                    groupDictionary[rid].FinishTime = entry.Time;

                    if (groupDictionary[rid].CompanyId == string.Empty)
                        groupDictionary[rid].CompanyId = entry.CompanyId;
                    if (groupDictionary[rid].DialerId == string.Empty)
                        groupDictionary[rid].DialerId = entry.DialerId;
                    if (groupDictionary[rid].CampaignId == string.Empty)
                        groupDictionary[rid].CampaignId = entry.CampaignId;
                    if (groupDictionary[rid].AgentId == string.Empty)
                        groupDictionary[rid].AgentId = entry.AgentId;
                    if (groupDictionary[rid].CallId == string.Empty)
                        groupDictionary[rid].CallId = entry.CallId;
                    if (groupDictionary[rid].InterviewId == string.Empty)
                        groupDictionary[rid].InterviewId = entry.InterviewId;

                    if (isNecessaryChangeDuration(groupDictionary[rid].Duration, entry.Duration))
                        groupDictionary[rid].Duration = entry.Duration;
                }
            }

            result = new List<EventsGroup>(groupDictionary.Values);

            return result;
        }

        private List<EventsGroup> EventsToEventsGroupsWithoutRId(List<Event> events, ParseSettings parseSettings)
        {
            events = events.Where(evt => ShouldIncludeEvent(evt, parseSettings)).Select(e => { e.RequestId = e.Id; return e; }).ToList();

            return EventsToEventsGroupsWithRId(events);
        }

        private static bool ShouldIncludeEvent(Event evt, ParseSettings parseSettings)
        {
            return (!parseSettings.DialerServiceOnly || evt.Name.Contains("DialerService.")) 
                && (!parseSettings.ExcludeGetState || (!evt.Name.Contains("GetState") && !evt.AllInfo.Contains("NotifyDialerState")))
                && (!parseSettings.ExcludeOnHook || !evt.AllInfo.Contains("OffHook") && !evt.AllInfo.Contains("OnHook"))
                && (!parseSettings.ExcludeDuplicateNotifications || !( !evt.Name.Contains("DialerService.Notify") && string.IsNullOrWhiteSpace(evt.Duration) ));
        }

        public void FillFilteredEventsGroups(string name, string startTime, string finishTime, string companyId, string dialerId, string campaignId, string agentId, string callId,
            string interviewId, string duration, string all)
        {
            FilteredEventsGroups = new List<EventsGroup>();
            ParserSettings.SetColumnsFilter(name, startTime, finishTime, companyId, dialerId, campaignId, agentId, callId, interviewId, duration, all);
            foreach (var entry in EventsGroups)
                if (ParserSettings.IsParametersMatchCondition(entry))
                    FilteredEventsGroups.Add(entry);

            if (all != string.Empty)
            {
                FilterEventsGroupsByAllInfo(all);
                var sortedGroupTable = from entry in FilteredEventsGroups
                                       orderby entry.RequestId
                                       select entry;
                FilteredEventsGroups = sortedGroupTable.ToList<EventsGroup>();
            }
        }

        public void ResetMatchingCondition()
        {
            foreach (var entry in Events)
                entry.IsHighlighted = false;
        }

        private void FilterEventsGroupsByAllInfo(string allInfo)
        {
            var ridHashSet = new HashSet<long>();

            foreach (var entry in Events)
                if (entry.AllInfo.Contains(allInfo))
                    ridHashSet.Add(entry.RequestId);

            if (ParserSettings.IsConditionalOperatorAnd)
            {
                for (var i = 0; i < FilteredEventsGroups.Count; i++)
                    if (!ridHashSet.Contains(FilteredEventsGroups[i].RequestId))
                        FilteredEventsGroups.RemoveAt(i--);
            }
            else
            {
                foreach (var rid in ridHashSet)
                {
                    var entryInFilterGroupTable = (from entry in FilteredEventsGroups
                                                   where entry.RequestId == rid
                                                   select entry).FirstOrDefault();

                    if (entryInFilterGroupTable == null)
                    {
                        var entryInGroupTable = (from entry in EventsGroups
                                                 where entry.RequestId == rid
                                                 select entry).FirstOrDefault();
                        FilteredEventsGroups.Add(entryInGroupTable);
                    }
                }
            }
        }

        private bool isNecessaryChangeDuration(string groupTableEntryDuration, string logTableEntryDuration)
        {
            return groupTableEntryDuration == string.Empty ||
                   groupTableEntryDuration != string.Empty && logTableEntryDuration != string.Empty && int.Parse(logTableEntryDuration) > int.Parse(groupTableEntryDuration);
        }

        private List<Event> ParseEvents(List<string> source)
        {
            var result = new List<Event>();

            for (var i = 0; i < source.Count; i++)
            {
                if (source[i] == string.Empty)
                    continue;

                _regexStringParser = new RegexParser(source[i]);

                string name, time, companyId, dialerId, campaignId, agentId, callId, interviewId, duration, all;

                name = _regexStringParser.ExtractName();

                if (IsRowMustBeAddedToPreviousEvent(name, source[i], result))
                {
                    result[result.Count - 1].AllInfo += string.Format("\n{0}", source[i]);
                    continue;
                }

                long requestId = _regexStringParser.FindRequestId();

                time = _regexStringParser.FindTimeByRegex();
                companyId = _regexStringParser.FindParameterByRegex(@"companyId=\d+");
                dialerId = _regexStringParser.FindParameterByRegex(@"dialerId=\d+");
                campaignId = _regexStringParser.FindParameterByRegex(@"campaignId=\d+");
                agentId = _regexStringParser.FindManyParametersByRegex(@"agentId=\d+");
                callId = _regexStringParser.FindManyParametersByRegex(@"callId=\d+");
                interviewId = _regexStringParser.FindManyParametersByRegex(@"interviewId=\d+");
                duration = _regexStringParser.FindParameterByRegex(@"duration: \d+");
                all = source[i];

                var eventsEntry = new Event(result.Count + 1, requestId, name, time, companyId, dialerId, campaignId, agentId, callId, interviewId, duration, all);
                result.Add(eventsEntry);
            }

            return result;
        }

        public void SetSettings(Settings conditionHandler)
        {
            ParserSettings = conditionHandler;
        }

        private bool IsRowMustBeAddedToPreviousEvent(string name, string row, List<Event> events)
        {
            return events.Count > 0 &&
                   (name == "" || Regex.IsMatch(row, @"^   .+") || (Regex.IsMatch(events[events.Count - 1].AllInfo, @"Exception details:$")));
        }
    }

    public class ParseSettings
    {
        public bool DialerServiceOnly { get; set; }
        public bool ExcludeGetState { get; set; }
        public bool ExcludeDuplicateNotifications { get; set; }
        public bool ExcludeOnHook { get; set; }
    }
}
