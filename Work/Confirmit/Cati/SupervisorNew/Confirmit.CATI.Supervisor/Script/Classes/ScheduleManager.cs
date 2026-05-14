using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.Schedules2007.BvSchScriptGen;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Script;
using Confirmit.CATI.Supervisor.Resources;
using Infragistics.Web.UI.GridControls;

namespace Confirmit.CATI.Supervisor.Script.Classes
{
    public class ScheduleManager : IScheduleManager
    {
        Schedule IScheduleManager.DeserializeSchedule(string xmlSchedule)
        {
            return DeserializeSchedule(xmlSchedule);
        }

        /// <summary>
        /// Deserialize schedule by incoming stream
        /// </summary>
        /// <param name="stream">incoming stream</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Occurs when file is invalid </exception>
        public static Schedule DeserializeSchedule(System.IO.Stream stream)
        {
            Schedule schedule = null;
            XmlSerializer serializer = new XmlSerializer(typeof(Schedule));
            schedule = (Schedule)serializer.Deserialize(stream);
            return schedule;
        }

        public static Schedule DeserializeSchedule(string xmlSchedule)
        {
            Schedule schedule = null;
            using (StringReader stringReader = new StringReader(xmlSchedule))
            {
                using (XmlTextReader xmlReader = new XmlTextReader(stringReader))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Schedule));
                    schedule = (Schedule)serializer.Deserialize(xmlReader);
                }
            }
            return schedule;
        }

        /// <summary>
        /// Serialize Schedule into the xml string.
        /// </summary>
        /// <param name="schedule">Incoming schedule</param>
        /// <returns></returns>
        public static String SerializeSchedule(Schedule schedule)
        {
            using (var stringWriter = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(Schedule));
                serializer.Serialize(stringWriter, schedule);
                return stringWriter.ToString();
            }
        }

        /// <summary>
        /// Returns Schedule by BvSchedule ID.
        /// </summary>
        /// <param name="scheduleId">Schedule ID.</param>
        /// <exception cref="ArgumentException">Script Id does not correspond to scheduling script.</exception>
        public static Schedule ScheduleById(int scheduleId)
        {
            Schedule schedule;
            var bvSchedule = ServiceLocator.Resolve<IScheduleRepository>().GetById(scheduleId);
            if (bvSchedule == null)
            {
                throw new ArgumentException(Strings.ScrIdDoesntCorrespondSchedulingScr, "schedule_id");
            }

            string xmlSchedule = bvSchedule.XmlUnderDev;
            if (!string.IsNullOrEmpty(xmlSchedule))
            {
                schedule = ServiceLocator.Resolve<IScheduleManager>().DeserializeSchedule(xmlSchedule);
            }
            else
            {
                schedule = new Schedule();
            }
            return schedule;
        }

        /// <summary>
        /// Returns all shift types by ShiftTypeCollection object
        /// </summary>
        /// <param name="shiftTypeCollection"></param>
        /// <param name="count">count of objects in array</param>
        /// <returns></returns>
        public static ShiftTypeInfo[] GetShiftTypes(ShiftTypeCollection shiftTypeCollection, out int count)
        {
            var shiftTypeList = new List<ShiftTypeInfo>();

            foreach (ShiftType shiftType in shiftTypeCollection)
            {
                var shiftTypeInfo = new ShiftTypeInfo
                                        {
                                            Id = shiftType.Id,
                                            Name = shiftType.Name,
                                            IsExclusion = shiftType.IsExclusionType,
                                            ColorName = shiftType.Color.HasValue ? ColorTranslator.FromHtml(shiftType.Color.Value.ToArgb().ToString()).Name : ""
                                        };
                shiftTypeList.Add(shiftTypeInfo);
            }
            count = shiftTypeList.Count;

            shiftTypeList.Sort();
            return shiftTypeList.ToArray();
        }

        /// <summary>
        /// Returns all shift types by CustomParameterCollection object
        /// </summary>
        /// <param name="paramsCollection"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static SchedulingParamInfo[] GetParameters(CustomParameterCollection paramsCollection, out int count)
        {
            SchedulingParamInfo paramInfo;

            var paramList = new List<SchedulingParamInfo>();

            foreach (CustomParameter param in paramsCollection)
            {
                paramInfo = new SchedulingParamInfo();
                paramInfo.Id = param.Id;
                paramInfo.Name = param.Name;
                paramInfo.Description = param.Description;
                if (param.Value != null) paramInfo.DefaultValue = (int) param.Value;
                paramInfo.Type = (int?) param.Type;
                paramList.Add(paramInfo);
            }
            count = paramList.Count;

            paramList.Sort();
            return paramList.ToArray();
        }

        /// <summary>
        /// Returns all shifts by TimeZone
        /// </summary>
        public static ShiftInfo[] GetShiftsByTimezone(int timeZone, ShiftCollection shiftCollection, out int totalCount)
        {
            var shiftList = new List<ShiftInfo>();

            foreach (Shift shift in shiftCollection)
            {
                var shiftInfo = new ShiftInfo {Id = shift.Id, ShiftTypeId = shift.ShiftTypeId.Value};

                ShiftData shiftData;
                if (shift.HasTimezone(timeZone))
                {
                    shiftData = shift.GetDataForTimezone(timeZone);
                    if (timeZone == Shift.RespondentTimezoneId)
                    {
                        shiftInfo.ShiftStatus = (shift.Timezones.Length == 1) ? ShiftStatus.Default : ShiftStatus.HasOverridden;
                    }
                    else
                    {
                        shiftInfo.ShiftStatus = ShiftStatus.Current;
                    }
                }
                else if (shift.HasTimezone(Shift.RespondentTimezoneId))
                {
                    shiftData = shift.GetDataForTimezone(Shift.RespondentTimezoneId);
                    shiftInfo.ShiftStatus = ShiftStatus.Default;
                }
                else
                {
                    continue;
                }

                shiftInfo.HasRespondentTimeZone = shift.HasTimezone(Shift.RespondentTimezoneId);
                shiftInfo.StartDay = shiftData.StartDayOfWeek.Value;
                shiftInfo.EndDay = shiftData.EndDayOfWeek.Value;
                shiftInfo.StartTime = shiftData.StartTime.Value;
                shiftInfo.EndTime = shiftData.EndTime.Value;
                shiftList.Add(shiftInfo);
            }

            totalCount = shiftList.Count;

            return shiftList.OrderBy(x => x.Id).ToArray();                        
        }

        /// <summary>
        /// Returns all exclusion by TimeZone
        /// </summary>        
        public static ExclusionInfo[] GetExclusionsByTimezone(int timeZone, ExclusionCollection exclusionCollection, out int totalCount)
        {
            var exclusionList = new List<ExclusionInfo>();

            foreach (Exclusion exclusion in exclusionCollection)
            {
                var exclusionInfo = new ExclusionInfo {
                                                        Id = exclusion.Id, 
                                                        ShiftTypeId = exclusion.ShiftTypeId.Value
                                                     };

                ExclusionData exclusionData;
                if (exclusion.HasTimezone(timeZone))
                {
                    exclusionData = exclusion.GetDataForTimezone(timeZone);
                    if (timeZone == Exclusion.RespondentTimezoneId)
                    {
                        exclusionInfo.ShiftStatus = (exclusion.Timezones.Length == 1) ? ShiftStatus.Default : ShiftStatus.HasOverridden;
                    }
                    else
                    {
                        exclusionInfo.ShiftStatus = ShiftStatus.Current;
                    }
                }
                else if (exclusion.HasTimezone(Exclusion.RespondentTimezoneId))
                {
                    exclusionData = exclusion.GetDataForTimezone(Exclusion.RespondentTimezoneId);
                    exclusionInfo.ShiftStatus = ShiftStatus.Default;
                }
                else
                {
                    continue;
                }

                DateTime startDateTime = exclusionData.StartDate.Value;
                DateTime endDateTime = exclusionData.EndDate.Value;

                exclusionInfo.HasRespondentTimeZone = exclusion.HasTimezone(Exclusion.RespondentTimezoneId);
                exclusionInfo.StartDay = startDateTime.Date;
                exclusionInfo.EndDay = endDateTime.Date;
                exclusionInfo.StartTime = startDateTime.TimeOfDay;
                exclusionInfo.EndTime = endDateTime.TimeOfDay;
                exclusionList.Add(exclusionInfo);
            }
            exclusionList.Sort();
            totalCount = exclusionList.Count;
            return exclusionList.ToArray();
        }

        /// <summary>
        /// Returns array of RuleInfo by RuleCollection
        /// </summary>
        public static RuleInfo[] GetRules(RuleCollection rules, out int count)
        {
            RuleInfo ruleInfo;
            SubRuleInfo subRuleInfo;
            ActionInfo actionInfo;
            List<RuleInfo> ruleList = new List<RuleInfo>();

            foreach (Rule rule in rules)
            {
                ruleInfo = new RuleInfo();
                ruleInfo.Id = rule.Id;
                ruleInfo.Description = rule.Description;
                ruleInfo.SampleUpdate = rule.SampleUpdate ? "Yes" : string.Empty;

                foreach (SubRule subRule in rule.SubRules)
                {
                    subRuleInfo = new SubRuleInfo();
                    subRuleInfo.Id = subRule.Id;
                    subRuleInfo.ItsId = subRule.ItsId.Value;
                    subRuleInfo.ShiftTypeId = subRule.ShiftTypeId.Value;
                    subRuleInfo.Filter = subRule.Filter;
                    subRuleInfo.Description = subRule.Description;
                    subRuleInfo.FilterEnabled = subRule.FilterEnabled;

                    foreach (SubRuleAction action in subRule.SubRuleActions)
                    {
                        actionInfo = new ActionInfo();
                        actionInfo.Id = action.Id;
                        actionInfo.ActionId = action.ActionId.Value;
                        actionInfo.Enabled = action.Enabled;
                        actionInfo.Filter = action.Filter;
                        actionInfo.FilterEnabled = action.FilterEnabled;
                        actionInfo.ParameterValue = action.Parameter.Value;
                        actionInfo.IsSchedulingParameter = (action.Parameter.Type==Parameter.ParamType.Parameter);
                        subRuleInfo.Actions.Add(actionInfo);
                    }
                    ruleInfo.SubRules.Add(subRuleInfo);
                }
                ruleList.Add(ruleInfo);
            }
            count = ruleList.Count;
            return ruleList.ToArray();
        }

        public static IEnumerable GetBothByTimezone(int timeZone, ShiftCollection shiftCollection, ExclusionCollection exclusionCollection)
        {
            int shiftCount, exclusionCount;
            var result = new List<IShiftInfo>();

            result.AddRange(GetShiftsByTimezone(timeZone, shiftCollection, out shiftCount));
            result.AddRange(GetExclusionsByTimezone(timeZone, exclusionCollection, out exclusionCount));

            return result.Select(info => new
            {
                Id = (info is ShiftInfo) ? info.Id.Value.ToString() : info.Id.Value.ToString() + "*",
                ShiftTypeId = info.ShiftTypeId,
                ShiftStatus = info.ShiftStatus,
                HasRespondentTimeZone = info.HasRespondentTimeZone,
                StartDayName = info.StartDayName,
                EndDayName = info.EndDayName,
                StartTimeToString = info.StartTimeToString,
                EndTimeToString = info.EndTimeToString
            }).ToList();                        
        }
        
        public static void FillObjectFromRow(object obj, GridRecord row)
        {
            string key;

            if (obj == null || row == null)
            {
                throw new ArgumentNullException();
            }

            Type objType = obj.GetType();
            foreach (PropertyInfo prop in objType.GetProperties())
            {
                RowReadAttribute rowReadAttribute = GetRowReadAttribute(prop);
                if (rowReadAttribute != null)
                {
                    key = rowReadAttribute.ColumnKey;
                    SetPropertyValue(prop, obj, row.Items.FindItemByKey(key).Value);
                }
            }
        }

        public static void FillObjectFromValues(object obj, Hashtable values)
        {
            string key;

            if (obj == null || values == null)
            {
                throw new ArgumentNullException();
            }

            Type objType = obj.GetType();
            foreach (PropertyInfo prop in objType.GetProperties())
            {
                RowReadAttribute rowReadAttribute = GetRowReadAttribute(prop);
                if (rowReadAttribute != null)
                {
                    key = rowReadAttribute.ColumnKey;
                    if(values.ContainsKey(key))
                    {
                        SetPropertyValue(prop, obj, values[key]);    
                    }                    
                }
            }
        }

        /// <summary>
        /// Returns RowReadAttribute binded with object property
        /// </summary>
        private static RowReadAttribute GetRowReadAttribute(PropertyInfo prop)
        {
            object[] attrs = prop.GetCustomAttributes(typeof(RowReadAttribute), true);
            return attrs.Length > 0 ? (RowReadAttribute)attrs[0] : null;
        }

        /// <summary>
        /// Set value for object property
        /// Support following types: int, bool, DateTime, TimeSpan, DayOfWeek
        /// </summary>
        ///<exception cref="ArgumentException">The value is not supported</exception>
        private static void SetPropertyValue(PropertyInfo prop, object obj, object value)
        {
            Int32 iValue;
            DateTime dt;
            TimeSpan ts;
            Boolean bValue;

            if (value != null)
            {
                if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(Nullable<int>))
                {
                    if (int.TryParse(value.ToString(), out iValue))
                    {
                        value = iValue;
                    }
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    if (bool.TryParse(value.ToString(), out bValue))
                    {
                        value = bValue;
                    }
                }
                else if (prop.PropertyType == typeof(DayOfWeek))
                {
                    value = Enum.Parse(typeof(DayOfWeek), value.ToString());
                }
                else if (prop.PropertyType == typeof(DateTime))
                {
                    if (DateTime.TryParse(value.ToString(), out dt))
                    {
                        value = dt;
                    }
                }
                else if (prop.PropertyType == typeof(TimeSpan))
                {
                    if (TimeSpan.TryParse(value.ToString(), out ts))
                    {
                        value = ts;
                    }
                }
            }
            else
            {
                if (prop.PropertyType != typeof(string) && prop.PropertyType != typeof(Nullable<int>) && prop.PropertyType != typeof(Nullable<Guid>))
                {
                    throw new ArgumentException(String.Format(Strings.PropertyNullValue, prop.Name));
                }
            }
            prop.SetValue(obj, value, null);
        }

        private static string GetResString(string key)
        {
            return (BaseForm.GetResString(key));
        }


        /// <summary>
        /// Returns DateTime by converting input string
        /// If direction FromClient: format should be dd/mm/yyyy HH:mm
        /// If direction ToClient: format should be yyyy-mm-ddThh:mm:ssZ
        /// </summary>
        public static string ConvertToDateTime(string inputValue, ConvertDirection direction)
        {
            if (string.IsNullOrEmpty(inputValue))
            {
                throw new ArgumentNullException("inputValue");
            }

            Match mc = null;

            if (direction == ConvertDirection.FromClient)
            {
                mc = Regex.Match(inputValue, @"(?<day>\d{2})[/](?<month>\d{2})[/](?<year>\d{4})\s+(?<hours>\d{2})[:](?<minutes>\d{2})");

                if (mc.Success)
                {
                    DateTime dt = new DateTime(
                        int.Parse(mc.Groups["year"].Value),
                        int.Parse(mc.Groups["month"].Value),
                        int.Parse(mc.Groups["day"].Value),
                        int.Parse(mc.Groups["hours"].Value),
                        int.Parse(mc.Groups["minutes"].Value),
                        0);
                    return string.Format("{0}Z", dt.ToString("s"));
                }
                else
                {
                    throw new ArgumentException(string.Format("{0}: {1}", Strings.errParameterValue, inputValue));
                }
            }
            else if (direction == ConvertDirection.ToClient)
            {
                DateTime dt = DateTime.Parse(inputValue.Substring(0, inputValue.Length - 1));
                return string.Format("{0}/{1}/{2} {3}:{4}",
                    dt.ToString("dd"),
                    dt.ToString("MM"),
                    dt.ToString("yyyy"),
                    dt.ToString("HH"),
                    dt.ToString("mm")
                    );
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets user-friendly message from UserMessageException when it occurs during script launch
        /// </summary>
        /// <param name="workingSchedule">Schedule object</param>
        /// <param name="compilerErrorCollection">CompilerErrorCollection</param>
        /// <returns>User-friendly exception message</returns>
        public static string GetLaunchExeptionMessage(Schedule workingSchedule, CompilerErrorCollection compilerErrorCollection)
        {
            if (compilerErrorCollection != null)
            {
                foreach (CompilerError cr in compilerErrorCollection)
                {
                    if (cr.IsWarning == false)
                    {
                        CustomCodeDescription customCodeDescription = CustomCodeDescription.Deserialize(cr.FileName);

                        var messageBuilder = new StringBuilder();

                        if (customCodeDescription.IsFilterDescription)
                        {
                            messageBuilder.Append(string.Format("{0}: ", Strings.ErrorInFilter));

                            if (customCodeDescription.RuleId.HasValue)
                            {
                                messageBuilder.AppendFormat(
                                    "{0}: {1}",
                                    Strings.Rule,
                                    workingSchedule.GetNumberByGuid(customCodeDescription.RuleId.Value));
                            }
                            if (customCodeDescription.SubRuleId.HasValue)
                            {
                                messageBuilder.AppendFormat(
                                    ", {0}: {1}",
                                    Strings.Subrule,
                                    workingSchedule.GetNumberByGuid(customCodeDescription.SubRuleId.Value));
                            }
                            if (customCodeDescription.ActionId.HasValue)
                            {
                                string name = "";
                                if( customCodeDescription.RuleId.HasValue && 
                                    customCodeDescription.SubRuleId.HasValue )
                                {
                                    var rule = workingSchedule.Rules.GetItemById(customCodeDescription.RuleId.Value);
                                    var subRule = rule.SubRules.GetItemById(customCodeDescription.SubRuleId.Value);
                                    var id = subRule.SubRuleActions.GetItemById(customCodeDescription.ActionId.Value).ActionId.Value;

                                    var scheduleService = ServiceLocator.Resolve<IScheduleService>();
                                    name = scheduleService.GetActions().GetActionById(id).Name;
                                }
                                messageBuilder.AppendFormat(
                                    ", {0}: {1} - {2}",
                                    Strings.Action,
                                    customCodeDescription.ActionId,
                                    name);
                            }

                            messageBuilder.AppendLine();
                            messageBuilder.AppendLine(cr.ErrorText);
                        }
                        else
                        {
                            messageBuilder.Append(string.Format("{0}.", Strings.ErrorInCustomScript));

                            messageBuilder.AppendLine();
                            messageBuilder.AppendLine(cr.ErrorText);
                            messageBuilder.AppendFormat("{0} {1}. ", Strings.Line, cr.Line);
                            messageBuilder.AppendFormat("{0} {1}. ", Strings.Column, cr.Column);
                        }

                        return messageBuilder.ToString();

                    }
                }
            }

            return null;
        }

        public static BvScheduleEntity AddSchedule(string name)
        {
            return AddSchedule(name, null);
        }

        public static BvScheduleEntity AddSchedule(string name, int? designStateGroupId)
        {
            var schedule = new BvScheduleEntity
            {
                Name = name,
                DesignStateGroupID = designStateGroupId
            };
            int id = ScheduleRepository.Insert(schedule);
            return ScheduleRepository.GetById(id);
        }
    }
}
