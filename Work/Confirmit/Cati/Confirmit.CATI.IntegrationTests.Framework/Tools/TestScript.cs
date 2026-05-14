using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;

using System.Data.SqlClient;

using Confirmit.CATI.Core.ScheduleDom.Scheduling;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    public class Action
    {
        public static int _newID = 1;
        
        public enum Operation
        {
            [XmlEnum("0")]
            Invalidate = 0,
            [XmlEnum("1")]
            SuspendTheInterview = 1,
            [XmlEnum("2")]
            RecallAfterANumberOfMinutes = 2,
            [XmlEnum("3")]
            RecallAfterANumberOfShifts = 3,
            [XmlEnum("4")]
            RecallAfterNumberOfShiftsButChooseRandomTimeWithinShift = 4,
            [XmlEnum("5")]
            FulfillTheSpecifiedAppointment = 5,
            [XmlEnum("6")]
            TerminateTheInterview = 6,
            [XmlEnum("7")]
            RecallOnNextShiftOfSpecifiedType = 7,
            [XmlEnum("8")]
            SetTimeToNOW = 8,
            [XmlEnum("9")]
            RunCustomScript = 9,
            [XmlEnum("10")]
            IncrementQuantityVariable = 10,
            [XmlEnum("11")]
            SpecialIncrementQuantityVariable = 11,
            [XmlEnum("12")]
            DecrementQuantityVariable = 12,
            [XmlEnum("13")]
            SpecialDecrementQuantityVariable = 13,
            [XmlEnum("14")]
            ResetQuantityVariable = 14,
            [XmlEnum("15")]
            SpecialResetQuantityVariable = 15,
            [XmlEnum("16")]
            AssignValueToQuantityVariable = 16,
            [XmlEnum("17")]
            RecallAfterNumberOfShiftsSpecifiedByVariable = 17,
            [XmlEnum("18")]
            AssignFunctionCallResultToVariable = 18,
            [XmlEnum("19")]
            PlaceCallHistoryBookmark = 19,
            [XmlEnum("20")]
            RecallOnNextShiftOfTheTypeSpecifiedByVariable = 20,
            [XmlEnum("21")]
            PlaceCallHistoryBookmarkToNOW = 21,
            [XmlEnum("22")]
            StopExecution = 22,
            [XmlEnum("23")]
            GoTo = 23,
            [XmlEnum("24")]
            AssignVirtualExtension = 24,
            [XmlEnum("25")]
            SetNextRule = 25,
            [XmlEnum("26")]
            SetNewITS = 26,
            [XmlEnum("27")]
            SetNewCallPriority = 27,
            [XmlEnum("28")]
            IncrementPriority = 28,
            [XmlEnum("29")]
            DecrementPriority = 29,
            [XmlEnum("30")]
            AssignResource = 30,
            [XmlEnum("31")]
            SetCallExpirationTimeout = 31,
            [XmlEnum("32")]
            SetRole = 32,
            [XmlEnum("33")]
            SetTimeToCall = 33,
            [XmlEnum("34")]
            SetCallExpirationTime = 34,
            [XmlEnum("35")]
            RecallOnTheSpecificShift = 35,
            [XmlEnum("36")]
            RecallOnTheShiftSpecifiedByVariable = 36,
            [XmlEnum("37")]
            SetShiftType = 37,
            [XmlEnum("38")]
            SetDialingMode = 38,
            [XmlEnum("39")]
            EnableCall = 39,
            [XmlEnum("40")]
            DisableCall = 40,
            [XmlEnum("41")]
            AddAdditionalAssignmentOnGroup = 41,
            [XmlEnum("42")]
            RemoveAssignmentOnGroup = 42,
            [XmlEnum("43")]
            RestorePreviousCallState = 43,
            [XmlEnum("44")]
            AcceptInboundCall = 44
        }

        [XmlElement("Id")]
        public int Id = _newID++;
        [XmlElement("ActionId")]
        public Operation ActionId = Operation.SuspendTheInterview;
        [XmlElement("Filter")]
        public string Filter = "";
        [XmlElement("Enabled")]
        public bool Enabled = true;
        [XmlElement("Description")]
        public string Description = "";
        [XmlElement("ParameterValue")]
        public Parameter Parameter = new Parameter();
        [XmlElement("FilterEnabled")]
        public bool FilterEnabled;
        [XmlIgnore]
        public CustomParameter CustomParam;

        // DO not remove. Need for the tests
        private Action() { }

        public Action(Operation operation)
        {
            ActionId = operation;
        }
        public Action(Operation operation, string paramValue)
        {
            ActionId = operation;
            Parameter.Constant = paramValue;
        }

        public Action(Operation operation, CustomParameter param)
        {
            ActionId = operation;
            Parameter.ParameterID = param.Id;
            CustomParam = param;
        }

        public Action(Operation operation, CustomParameter param, string filter)
        {
            ActionId = operation;
            Parameter.ParameterID = param.Id;
            CustomParam = param;
            Filter = filter;
            FilterEnabled = true;
        }

        public Action(Operation operation, string paramValue, string filter)
        {
            ActionId = operation;
            Parameter.Constant = paramValue;
            Filter = filter;
            FilterEnabled = true;
        }

        public Action(Operation operation, string paramValue, string filter, bool filterEnabled)
        {
            ActionId = operation;
            Parameter.Constant = paramValue;
            Filter = filter;
            FilterEnabled = filterEnabled;
        }
        public Action(Operation operation, string paramValue, string filter, bool filterEnabled, bool actionAnabled)
        {
            ActionId = operation;
            Parameter.Constant = paramValue;
            Filter = filter;
            FilterEnabled = filterEnabled;
            Enabled = actionAnabled;
        }
    }

    public class SubRule
    {
        [XmlElement("Id")]
        public Guid Id = Guid.NewGuid();
        [XmlElement("ItsId")]
        public int ItsId;
        [XmlElement("ShiftTypeId")]
        public int ShiftTypeId;
        [XmlElement("CallState")]
        public int Phase;
        [XmlElement("Filter")]
        public string Filter = "";
        [XmlElement("FilterEnabled")]
        public bool FilterEnabled = true;
        [XmlElement("Description")]
        public string Description = "subrule";
        [XmlArray("SubRuleActions"), XmlArrayItem("SubRuleAction")]
        public List<Action> Actions = new List<Action>();

        public SubRule()
        {

        }
        public SubRule(Action action)
        {
            Actions.Add(action);
        }
        public SubRule(Action action, int itsId, int shiftTypeId, int phase, string filter, bool filterEnabled)
            : this (new List<Action> { action }, itsId, shiftTypeId, phase, filter, filterEnabled)
        {
        }

        public SubRule(IEnumerable<Action> actions, int itsId, int shiftTypeId, int phase, string filter, bool filterEnabled)
        {
            Actions.AddRange(actions);
            ItsId = itsId;
            ShiftTypeId = shiftTypeId;
            Phase = phase;
            Filter = filter;
            FilterEnabled = filterEnabled;
        }

        public SubRule(IEnumerable<Action> actions)
        {
            Actions.AddRange(actions);
        }

        public SubRule(params Action[] actions)
        {
            Actions.AddRange(actions);
        }

        public SubRule(Guid subruleId, IEnumerable<Action> actions)
        {
            Id = subruleId;
            Actions.AddRange(actions);
        }
    }

    [XmlRoot("Rule")]
    public class Rule
    {
        [XmlElement("Id")]
        public Guid Id = Guid.NewGuid();
        [XmlElement("Description")]
        public string Description = "rule";
        [XmlAttribute("SampleUpdate")]
        public bool SampleUpdate = false;
        [XmlArray("SubRules"), XmlArrayItem("SubRule")]
        public List<SubRule> SubRules = new List<SubRule>();

        public Rule()
        {
        }

        public Rule(SubRule subrule)
        {
            SubRules.Add(subrule);
        }

        public Rule(SubRule subrule, string desc)
        {
            Description = desc;
            SubRules.Add(subrule);
        }

        public Rule(SubRule subrule, bool sampleUpdate)
        {
            SampleUpdate = sampleUpdate;
            SubRules.Add(subrule);
        }

        public Rule(SubRule[] subrules)
        {
            SubRules.AddRange(subrules);
        }

        public Rule(Guid ruleId, Action action)
        {
            Id = ruleId;
            SubRules.Add(new SubRule(action));
        }


        public Rule(Guid id, string desc)
        {
            Id = id;
            Description = desc;
        }
    }

    public class ShiftType
    {
        [XmlElement("Id")]
        public int Id;
        [XmlElement("Name")]
        public string Name;
        [XmlElement("ColorInt")]
        public int ColorInt;


        public ShiftType(int id)
        {
            Id = id;
            Name = "shiftType " + id.ToString(CultureInfo.InvariantCulture);
            ColorInt = 0;
        }
        public ShiftType()
        {

        }
    }

    public class ShiftTimezone
    {
        [XmlElement("Id")]
        public int? Id;

        [XmlRoot("Data")]
        public class TimezoneData
        {
            [XmlElement("StartDayOfWeek")]
            public DayOfWeek StartDayOfWeek;
            [XmlElement("StartTime")]
            public string StartTime;
            [XmlElement("EndDayOfWeek")]
            public DayOfWeek EndDayOfWeek;
            [XmlElement("EndTime")]
            public string EndTime;
        }

        public TimezoneData Data = new TimezoneData();

        public ShiftTimezone(int? id, string start, string end)
        {
            Id = id;
            Data.StartTime = TimeSpanToTime(start, out Data.StartDayOfWeek);
            Data.EndTime = TimeSpanToTime(end, out Data.EndDayOfWeek);
        }

        public string TimeSpanToTime(string spanFormat, out DayOfWeek dayOfWeek)
        {
            TimeSpan span = TimeSpan.Parse(spanFormat);
            dayOfWeek = (DayOfWeek)span.Days;

            return span.Hours.ToString("00") + ":" + span.Minutes.ToString("00") + ":" + span.Seconds.ToString("00");
        }
        public ShiftTimezone()
        {

        }
    }

    [XmlRoot("Shift")]
    public class Shift
    {
        [XmlElement("Id")]
        public int Id;
        [XmlElement("ShiftTypeId")]
        public int ShiftTypeId;
        [XmlArray("Timezones"), XmlArrayItem("Timezone")]
        public List<ShiftTimezone> Timezones = new List<ShiftTimezone>();

        public Shift(int id, int typeID, params object[] timezones)
        {
            Id = id;
            ShiftTypeId = typeID;
            foreach (object tzObj in timezones)
            {
                if (tzObj is IEnumerable<ShiftTimezone>)
                    Timezones.AddRange((IEnumerable<ShiftTimezone>)tzObj);
                else if (tzObj is ShiftTimezone)
                    Timezones.Add((ShiftTimezone)tzObj);
                else
                    throw new NotSupportedException("Unexpected object");
            }
        }

        public Shift(int id, int typeID, string start, string end)
        {
            Id = id;
            ShiftTypeId = typeID;
            Timezones.Add(new ShiftTimezone(null, start, end));
        }
        public Shift()
        {

        }

        public static Shift Week = new Shift(1, 1, "0.00:00:00", "6.23:59:59");
        public static Shift Second = new Shift(1, 1, "0.00:00:00", "0.00:00:01");
    }

    public class ExclusionTimezone
    {
        [XmlElement("Id")]
        public int? Id;
        [XmlRoot("Data")]
        public class ExclusionData
        {
            [XmlElement("StartDate")]
            public string StartDate;
            [XmlElement("EndDate")]
            public string EndDate;
        }

        public ExclusionData Data = new ExclusionData();

        public ExclusionTimezone(int? tzID, string startDate, string endDate)
        {
            //only for Check
            DateTime.Parse(startDate);
            DateTime.Parse(endDate);
            //Init content
            Id = tzID;
            Data.StartDate = startDate;
            Data.EndDate = endDate;
        }
        public ExclusionTimezone()
        {

        }
    }

    [XmlRoot("Exclusion")]
    public class Exclusion
    {
        [XmlElement("Id")]
        public int Id;
        [XmlElement("ShiftTypeId")]
        public int ShiftTypeId;
        [XmlArray("Timezones"), XmlArrayItem("Timezone")]
        public List<ExclusionTimezone> Timezones = new List<ExclusionTimezone>();

        public Exclusion()
        {

        }

        public Exclusion(int id, params object[] timezones)
        {
            Id = id;
            foreach (object tzObj in timezones)
            {
                if (tzObj is IEnumerable<ExclusionTimezone>)
                    Timezones.AddRange((IEnumerable<ExclusionTimezone>)tzObj);
                else if (tzObj is ShiftTimezone)
                    Timezones.Add((ExclusionTimezone)tzObj);
                else
                    throw new NotSupportedException("Unexpected object");
            }
        }

        public Exclusion(int id, string start, string end)
        {
            Id = id;
            Timezones.Add(new ExclusionTimezone(null, start, end));
        }
    }

    [XmlRoot("CustomParameter")]
    public class CustomParameter
    {
        [XmlElement]
        public int Id;
        [XmlElement]
        public string Name;
        [XmlElement]
        public string Description;
        [XmlElement]
        public SchedulingParameterType Type;
        [XmlElement]
        public int Value;

    }

    public class TestScript
    {
        [XmlArray("Rules"), XmlArrayItem("Rule")]
        public List<Rule> Rules = new List<Rule>();
        [XmlArray("ShiftTypes"), XmlArrayItem("ShiftType")]
        public List<ShiftType> ShiftTypes = new List<ShiftType>();
        [XmlArray("Shifts"), XmlArrayItem("Shift")]
        public List<Shift> Shifts = new List<Shift>();
        [XmlArray("Exclusions"), XmlArrayItem("Exclusion")]
        public List<Exclusion> Exclusions = new List<Exclusion>();
        [XmlArray("CustomParameters"), XmlArrayItem("CustomParameter")]
        public List<CustomParameter> CustomParameters = new List<CustomParameter>();
        public string CustomScript = "";

        private readonly string _baseScheduleSciptPath;
        public int ScheduleID { get; set; }

        public TestScript(Action action, string baseScheduleSciptPath)
        {
            _baseScheduleSciptPath = baseScheduleSciptPath;
            Rules.Add(new Rule(new SubRule(action)));
        }

        public TestScript(Action action, params object[] shifts)
        {
            Rules.Add(new Rule(new SubRule(action)));
            FillShifts(shifts);
        }

        public TestScript(int its, Action.Operation operation, string param)
        {
            Rules.Add(new Rule(new SubRule(new Action(operation, param), its, 0, 0, null, true)));
            FillShifts(new[] { Shift.Week });
        }

        public TestScript(CallOutcome its, Action.Operation operation, string param)
            : this((int)its, operation, param)
        {
        }

        public TestScript(SubRule subrule, string baseScheduleSciptPath)
        {
            _baseScheduleSciptPath = baseScheduleSciptPath;
            Rules.Add(new Rule(subrule));
        }

        public TestScript(SubRule subrule, params object[] shifts)
        {
            Rules.Add(new Rule(subrule));
            FillShifts(shifts);
        }

        public TestScript(SubRule[] subrules, params object[] shifts)
        {
            Rules.Add(new Rule(subrules));
            FillShifts(shifts);
        }

        public TestScript(Rule rule)
        {
            Rules.Add(rule);
        }

        public TestScript(Rule rule, params object[] shifts)
        {
            Rules.Add(rule);
            FillShifts(shifts);
        }

        public TestScript(Rule rule, string baseScheduleSciptPath)
        {
            _baseScheduleSciptPath = baseScheduleSciptPath;
            Rules.Add(rule);
        }

        public TestScript(IEnumerable<Action> actions, string baseScheduleSciptPath)
        {
            _baseScheduleSciptPath = baseScheduleSciptPath;
            Rules.Add(new Rule(new SubRule(actions)));
        }

        public TestScript(IEnumerable<Action> actions, params object[] shifts)
        {
            Rules.Add(new Rule(new SubRule(actions)));
            FillShifts(shifts);
        }
        
        public TestScript(SubRule[] subrules, string baseScheduleSciptPath)
        {
            _baseScheduleSciptPath = baseScheduleSciptPath;
            Rules.Add(new Rule(subrules));
        }

        public TestScript(IEnumerable<Rule> rules, string baseScheduleSciptPath)
        {
            _baseScheduleSciptPath = baseScheduleSciptPath;
            Rules.AddRange(rules);
        }

        public TestScript(IEnumerable<Rule> rules, params object[] shifts)
        {
            Rules.AddRange(rules);
            FillShifts(shifts);
        }

        public void FillShifts(object[] shifts)
        {
            foreach (object shift in shifts)
            {
                if (shift is Shift)
                {
                    Shifts.Add((Shift)shift);
                }
                else if (shift is IEnumerable<Shift>)
                {
                    Shifts.AddRange((IEnumerable<Shift>)shift);
                }
                else if (shift is Exclusion)
                {
                    Exclusions.Add((Exclusion)shift);
                }
                else if (shift is IEnumerable<Exclusion>)
                {
                    Exclusions.AddRange((IEnumerable<Exclusion>) shift);
                }
                else
                {
                    throw new Exception(string.Format("Not supported object {0}", shift.GetType()));
                }
            }
        }        

        public string GenerateXML()
        {
            CustomParameters = PickCustomParameters();

            if (Shifts.Count > 0 || Exclusions.Count > 0)
                return GetnerateXMLWithShifts();

            string xml;

            using (Stream stream = new MemoryStream())
            {
                var serializer = new XmlSerializer(Rules.GetType());
                serializer.Serialize(stream, Rules);

                serializer = new XmlSerializer(CustomParameters.GetType());
                serializer.Serialize(stream, CustomParameters);

                stream.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(stream))
                {
                    xml = sr.ReadToEnd().Replace("ArrayOfRule", "Rules")
                                        .Replace("ArrayOfCustomParameter", "CustomParameters")
                                        .Replace("<?xml version=\"1.0\"?>", "");
                }
            }
            
            string baseScheduleScipt = File.ReadAllText(Path.Combine(IntegrationTestingFramework.Instance.Cfg.TestDataPath, _baseScheduleSciptPath));
            
            return String.Format(baseScheduleScipt, xml);
        }

        private string GetnerateXMLWithShifts()
        {
            // make ShiftTypes
            ShiftTypes.Clear();
            if (Exclusions.Count > 0)
                ShiftTypes.Add(new ShiftType(0));
            var ids = new Dictionary<int, int>();
            foreach (Shift shift in Shifts)
            {
                if (ids.ContainsKey(shift.ShiftTypeId))
                    continue;
                ids.Add(shift.ShiftTypeId, shift.ShiftTypeId);
                ShiftTypes.Add(new ShiftType(shift.ShiftTypeId));
            }

            string xml = "";

            using (var stream = new MemoryStream())
            {
                // Rules
                var serializer = new XmlSerializer(Rules.GetType());
                serializer.Serialize(stream, Rules);

                //ShiftTypes
                serializer = new XmlSerializer(ShiftTypes.GetType());
                serializer.Serialize(stream, ShiftTypes);


                //Shifts
                serializer = new XmlSerializer(Shifts.GetType());
                serializer.Serialize(stream, Shifts);

                //Exclusions
                serializer = new XmlSerializer(Exclusions.GetType());
                serializer.Serialize(stream, Exclusions);

                //Custom parameters
                serializer = new XmlSerializer(CustomParameters.GetType());
                serializer.Serialize(stream, CustomParameters);

                stream.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(stream))
                {
                    xml += sr.ReadToEnd();
                }
            }

            xml = xml.Replace("ArrayOfRule", "Rules").
                Replace("ArrayOfShiftType", "ShiftTypes").
                Replace("ArrayOfShift", "Shifts").
                Replace("ArrayOfExclusion", "Exclusions").
                Replace("ArrayOfCustomParameter", "CustomParameters").
                Replace("<?xml version=\"1.0\"?>", "");

            xml = "<?xml version=\"1.0\"?>" +
                            "<Schedule xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
                                "<Id xsi:nil=\"true\" />" +
                                    "<Name />" + xml;

            xml += @"<CustomScript>
                        <Id>1</Id>
                        <LanguageName>JScript.Net</LanguageName>
                        <Body>" + CustomScript + @"</Body>
                    </CustomScript>
                </Schedule>";


            return xml;
        }

        void AreEqual( CustomParameter expected, CustomParameter actual, string message )
        {
            Assert.AreEqual(expected.Name, actual.Name, message );
            Assert.AreEqual(expected.Description, actual.Description, message );
            Assert.AreEqual(expected.Type, actual.Type, message );
            Assert.AreEqual(expected.Value, actual.Value, message );
        }

        private List<CustomParameter> PickCustomParameters()
        {
            // make CustomParams
            var customParams = new Dictionary<int, CustomParameter>();
            foreach( var param in CustomParameters )
            {
                if( customParams.ContainsKey( param.Id ))
                    throw new Exception(String.Format("param with Id = {0} duplicated", param.Id));
                customParams.Add( param.Id, param );
            }

            foreach (var rule in Rules)
                foreach (var subrule in rule.SubRules)
                    foreach (var action in subrule.Actions)
                    {
                        if (action.Parameter.Type == Parameter.ParamType.Parameter)
                        {
                            CustomParameter customParam;
                            if (customParams.TryGetValue(action.CustomParam.Id, out customParam))
                            {
                                AreEqual( customParam, action.CustomParam, "Invalid scheduling script. scheduling script is used two different custom parameters with identical id");
                            }
                            else
                            {
                                customParams.Add(action.CustomParam.Id, action.CustomParam);
                                CustomParameters.Add(action.CustomParam);
                            }
                        }
                    }
            return customParams.Select(x => x.Value).ToList();
        }

        public static void Update(int scheduleID, TestScript script)
        {
            var schedule = ScheduleRepository.GetById(scheduleID);

            schedule.XmlUnderDev = script.GenerateXML();

            ScheduleRepository.Update(schedule);

            ScheduleService.Launch(scheduleID);
        }

        public int Create(string name)
        {
            if (String.IsNullOrEmpty(name))
                name = "schedulingScript " + Guid.NewGuid().ToString();

            var schedule = ScheduleRepository.GetByName(name);

            if (schedule == null) // does not exist, we should create new schedule
            {
                schedule = new BvScheduleEntity
                {
                    Name = name,
                };

                ScheduleRepository.Insert(schedule);
                schedule = ScheduleRepository.GetByName(name);
            }

            ScheduleID = schedule.ScheduleID;
            schedule.XmlUnderDev = GenerateXML();

            ScheduleRepository.Update(schedule);

            ScheduleService.Launch(ScheduleID);

            return ScheduleID;
        }

        public int GetShiftTypeWorkID(int shiftTypeID)
        {
            var shiftTypes = BvShiftTypeAdapter.GetByCondition("ID = @ID", new SqlParameter("@ID", shiftTypeID));
            if (shiftTypes.Count == 1)
                return shiftTypes[0].ObjectID;
            return (int)(CallShiftType.None);
        }
    }
}
