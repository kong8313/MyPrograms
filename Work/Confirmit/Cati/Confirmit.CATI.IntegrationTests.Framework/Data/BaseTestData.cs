using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Core.Activity;
using ConfirmitDialerInterface;
using DialerCommon;
using Microsoft.SqlServer.Management.Smo;
using DialType = Confirmit.CATI.Common.DialType;

namespace Confirmit.CATI.IntegrationTests.Framework.Data
{
    public abstract class BaseTestData
    {
        public SurveyData[] Surveys { get; set; }
        public PersonData[] Persons { get; set; }
        public DialerData[] Dialers { get; set; }
        public ScriptData[] Scripts { get; set; }
        public PersonGroupData[] PersonGroups { get; set; }
        public CallCenterData[] CallCenters { get; set; }
        public CallGroupData[] CallGroups { get; set; }
        public SupervisorData[] Supervisors { get; set; }
        public string[] TelephoneBlacklist { get; set; }
        public InboundCallHistoryData[] InboundCallHistories { get; set; }
        public ActiveDialData[] ActiveDials { get; set; }
        public BaseAlertData[] Alerts { get; set; }
        public Dictionary<string, object> SystemSettings { get; set; }
        public FilterData[] Filters { get; set; }
        public ExternalNumberData[] ExternalNumbers { get; set; }
        public StateData[] StateData { get; set; }

        protected BaseTestData()
        {
            Surveys = new SurveyData[] { };
            Persons = new PersonData[] { };
            Dialers = new DialerData[] { };
            Scripts = new ScriptData[] { };
            PersonGroups = new PersonGroupData[] { };
            CallCenters = new CallCenterData[] { };
            CallGroups = new CallGroupData[] { };
            Supervisors = new SupervisorData[] { };
            TelephoneBlacklist = new string[] { };
            InboundCallHistories = new InboundCallHistoryData[] { };
            Alerts = new BaseAlertData[] { };
            SystemSettings = new Dictionary<string, object>();
            Filters = new FilterData[] { };
            ExternalNumbers = new ExternalNumberData[] { };
            StateData = new StateData[] { };
        }
    }

    public class SurveyData : TaggedObjectData
    {
        public FormData[] Forms = { };
        public QuotaData[] Quotas = { };
        public InterviewData[] Interviews = { };
        public InboundTelephoneNumberData[] InboundTelephoneNumbers { get; set; }
        public string[] Assigns = { };
        public string AssignsS { set { Assigns = value.Split(','); } }

        public bool IsOpen = true;

        public bool IsSoftDeleted = false;

        public string ClusterQuota;
        public int ClusterQuotaThreshold;

        public bool IsUseDb;

        public string ProjectId;

        public DialingMode DialMode = DialingMode.Manual;

        public string SchedulingScript { get; set; }

        public bool IsCallGroupEnabled { get; set; }

        public string[] CallCenters { get; set; }

        public bool IsSupportBlackList { get; set; }

        public bool ScreenRecording { get; set; } = false;

        public bool OpenEndReview { get; set; }

        public bool? IsQuotaInCatiDb { get; set; }

        public InboundSurveyBehavior InboundBehavior { get; set; }

        public SurveyBalancingData Balancing = new SurveyBalancingData();
    }

    public class SurveyBalancingData
    {
        public string Quotas;
        public string Fields;
    }

    public class SupervisorData : TaggedObjectData
    {
        public string Name;
        public string CurrentCallCenter { get; set; }
        public string[] Surveys { get; set; }
    }

    public class InboundCallHistoryData : TaggedObjectData
    {
        public string InboundTelNumber { get; set; }
        public string RespondentTelNumber { get; set; }
        public string InboundCallId { get; set; }
        public int? SurveyId { get; set; }
        public int? InterviewId { get; set; }
        public int OperationType { get; set; }
    }

    public class ActiveDialData : TaggedObjectData
    {

    }

    public class CallCenterData : TaggedObjectData
    {
        public string Name;
        public string Dialer;
        public string Description;
    }

    public class InterviewData : TaggedObjectData
    {
        public int Count;
        public string Data;
        public CallOutcome ITS = CallOutcome.FreshSample;
        public CallData Call;
        public CallHistoryData[] CallHistory = { };
        public string LastCallPerson;
        public ReviewStatus ReviewStatus;
        public ExtendedCallHistoryData[] ExtendedCallHistory = { };
        public DialType DialType;
        public string TelephoneNumber = "01234567890";
        public string Sid;
        public string InterviewerId;
        public string RespondentName;
        public string DialMode;
        public string LastChannelId;
        public string ExtensionNumber;
        public string CallAttemptCount;
        public string TimeZoneId;
        public string CatiCallTime;
        public string CatiCallExpirationTime;
        public string CatiCallPriority;
        public string CatiShiftType;
        public string CatiCallState;
        public InterviewHisotryData[] History = { };

        public InterviewData(int count = 1)
        {
            Count = count;
        }
    }

    public class InterviewHisotryData
    {
        public string Time;
        public CallOutcome ITS;
        public int Duration;
        public Role Role = Role.Interviewer;
        public string Person = null;
        public string TelephoneNumber = "01234567890";
    }

    public class DialerData : TaggedObjectData
    {
        public int? Id;
        public string Name;
        public string Type = "BvTCI";
        public DialType DialType = DialType.Landline;
        public bool IsActive = true;
        public bool IsConnected = true;
        public int TenantId;
        public int? ReconnectionDuration = null;
        public int ExpectedState = (int)DialerStatus.DisconnectedAndDeactivated;

        public ReplyType ReplyType = ReplyType.Async;

        public string DialerVersion;
        public DialerFeatures Features;
    }

    public class CallGroupData : TaggedObjectData
    {
        public string Name;
        public string Description;
        public CallOutcome[] ITS { get; set; }
    }

    public class PersonGroupData : TaggedObjectData
    {
        public string Name;
        public string Description = "";
        public string Memberships { get; set; }
        public InboundGroupBehavior InboundBehavior = InboundGroupBehavior.DeliverCallsFromTheSameSurvey;
        public TransferGroupBehavior TransferBehavior = TransferGroupBehavior.Disabled;
    }

    public class PersonData : TaggedObjectData
    {
        public string Name;
        public string Description = "";
        public string Location = "";
        public string Password;
        public TaskChoiceMode TaskChoice;
        public TaskChoicePermissions? AllowedChoices;
        public string CallCenter;
        public string Memberships { get; set; }
        public string CallGroup { get; set; }
        public DialType DialType = DialType.Landline;
        public AgentType Type = AgentType.LiveAgent;
        public PersonAssignmentListMode AssignmentListMode;
    }

    public static class AllHoursSchedule
    {
        public const string Name = "AllHours";
        public const string Xml = "<Schedule xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Id xsi:nil=\"true\"/><Name/><Rules><Rule><Id>13c49088-ad96-476b-a6b4-b09ddf873ae1</Id><Description/><SubRules><SubRule><Id>ff32c0f5-5d1e-4726-9de7-ea95dc99c3ed</Id><ItsId>1</ItsId><ShiftTypeId>0</ShiftTypeId><Filter/><FilterEnabled>false</FilterEnabled><Description/><SubRuleActions><SubRuleAction><Id>1</Id><ActionId>27</ActionId><Filter/><Enabled>true</Enabled><Description/><ParameterValue>1000</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction><SubRuleAction><Id>2</Id><ActionId>5</ActionId><Filter/><Enabled>true</Enabled><Description/><ParameterValue>0</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction></SubRuleActions></SubRule><SubRule><Id>6cdc0632-15a2-4f4a-baf9-328eb9bb3b31</Id><ItsId>2</ItsId><ShiftTypeId>0</ShiftTypeId><Filter/><FilterEnabled>false</FilterEnabled><Description/><SubRuleActions><SubRuleAction><Id>1</Id><ActionId>2</ActionId><Filter/><Enabled>true</Enabled><Description/><ParameterValue>15</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction></SubRuleActions></SubRule><SubRule><Id>14aa9b11-236d-4473-8043-3557f9853c86</Id><ItsId>3</ItsId><ShiftTypeId>0</ShiftTypeId><Filter/><FilterEnabled>false</FilterEnabled><Description/><SubRuleActions><SubRuleAction><Id>1</Id><ActionId>3</ActionId><Filter/><Enabled>true</Enabled><Description/><ParameterValue>1</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction></SubRuleActions></SubRule><SubRule><Id>0d2081bd-80cd-4a0e-b3c3-70533863a712</Id><ItsId>16</ItsId><ShiftTypeId>0</ShiftTypeId><Filter/><FilterEnabled>false</FilterEnabled><Description/><SubRuleActions><SubRuleAction><Id>1</Id><ActionId>8</ActionId><Filter/><Enabled>true</Enabled><Description/><ParameterValue>0</ParameterValue><FilterEnabled>false</FilterEnabled></SubRuleAction></SubRuleActions></SubRule></SubRules></Rule></Rules><ShiftTypes><ShiftType><Id>1</Id><Name>Default</Name><ColorInt>-16751616</ColorInt></ShiftType></ShiftTypes><Shifts><Shift><Id>1</Id><ShiftTypeId>1</ShiftTypeId><Timezones><Timezone><Id xsi:nil=\"true\"/><Data><StartDayOfWeek>Monday</StartDayOfWeek><StartTime>00:00:00</StartTime><EndDayOfWeek>Tuesday</EndDayOfWeek><EndTime>00:00:00</EndTime></Data></Timezone></Timezones></Shift><Shift><Id>2</Id><ShiftTypeId>1</ShiftTypeId><Timezones><Timezone><Id xsi:nil=\"true\"/><Data><StartDayOfWeek>Tuesday</StartDayOfWeek><StartTime>00:00:00</StartTime><EndDayOfWeek>Wednesday</EndDayOfWeek><EndTime>00:00:00</EndTime></Data></Timezone></Timezones></Shift><Shift><Id>3</Id><ShiftTypeId>1</ShiftTypeId><Timezones><Timezone><Id xsi:nil=\"true\"/><Data><StartDayOfWeek>Wednesday</StartDayOfWeek><StartTime>00:00:00</StartTime><EndDayOfWeek>Thursday</EndDayOfWeek><EndTime>00:00:00</EndTime></Data></Timezone></Timezones></Shift><Shift><Id>4</Id><ShiftTypeId>1</ShiftTypeId><Timezones><Timezone><Id xsi:nil=\"true\"/><Data><StartDayOfWeek>Thursday</StartDayOfWeek><StartTime>00:00:00</StartTime><EndDayOfWeek>Friday</EndDayOfWeek><EndTime>00:00:00</EndTime></Data></Timezone></Timezones></Shift><Shift><Id>5</Id><ShiftTypeId>1</ShiftTypeId><Timezones><Timezone><Id xsi:nil=\"true\"/><Data><StartDayOfWeek>Friday</StartDayOfWeek><StartTime>00:00:00</StartTime><EndDayOfWeek>Saturday</EndDayOfWeek><EndTime>00:00:00</EndTime></Data></Timezone></Timezones></Shift><Shift><Id>6</Id><ShiftTypeId>1</ShiftTypeId><Timezones><Timezone><Id xsi:nil=\"true\"/><Data><StartDayOfWeek>Saturday</StartDayOfWeek><StartTime>00:00:00</StartTime><EndDayOfWeek>Sunday</EndDayOfWeek><EndTime>00:00:00</EndTime></Data></Timezone></Timezones></Shift><Shift><Id>7</Id><ShiftTypeId>1</ShiftTypeId><Timezones><Timezone><Id xsi:nil=\"true\"/><Data><StartDayOfWeek>Sunday</StartDayOfWeek><StartTime>00:00:00</StartTime><EndDayOfWeek>Monday</EndDayOfWeek><EndTime>00:00:00</EndTime></Data></Timezone></Timezones></Shift></Shifts><Exclusions/><CustomScript><Id>1</Id><LanguageName>JScript.Net</LanguageName><Body/></CustomScript></Schedule>";
    }

    public class ScriptData : TaggedObjectData
    {
        public string Name;
        public TestScript Script;
        public static ScriptData AllHours = new ScriptData() { Tag = AllHoursSchedule.Name, Name = AllHoursSchedule.Name };
        public static ScriptData DefaultSchedule = new ScriptData() { Tag = "#DefaultSchedule" };
    }

    public class CallData : TaggedObjectData
    {
        public BvCallEntity Model = new BvCallEntity();

        public int Priority
        {
            get => Model.Priority;
            set => Model.Priority = value;
        }

        public int ShiftType
        {
            get => Model.ShiftID;
            set => Model.ShiftID = value;
        }

        public int CallState
        {
            get => Model.CallState;
            set => Model.CallState = value;
        }

        public DateTime? TimeToExpire
        {
            get => Model.TimeToExpire;
            set => Model.TimeToExpire = value;
        }

        public DateTime? TimeInShift
        {
            get => Model.TimeInShift;
            set => Model.TimeInShift = value;
        }

        public string Resource { get; set; }
    }

    public class CallHistoryData : TaggedObjectData
    {
        public string Person;
        public CallOutcome ITS = CallOutcome.Completed;
        public DateTime FiredTime = DateTime.UtcNow;
        public string TelephoneNumber = null;
        public int? Duration = null;
        public int? WaitingTime = null;
        public int CallCenterId = 1;
    }

    public class ExtendedCallHistoryData : TaggedObjectData
    {
        public OperationType OperationType;
        public int OperationId = 0;
        public CallOutcome ITS = CallOutcome.Completed;
        public DateTime FiredTime = DateTime.UtcNow;
    }

    public abstract class TaggedObjectData
    {
        public string Tag;
    }

    public class SingleFormData : FormData
    {
        public string[] Precodes = { };
    }

    public class MultiFormData : FormData
    {
        public string[] Precodes = { };
        public bool IsOpen;
        public bool IsNumeric;

        public MultiFormData()
        {
            IsReplicated = false;
        }
    }

    public class FormData
    {
        public string Name;
        public string TableName = "response0";
        public SqlDataType SqlType = SqlDataType.Int;
        public bool IsReplicated = true;
        public int ColumnId = 0;
    }

    public class QuotaData
    {
        public int Id;
        public string Name;
        public string[] Fields;
        public CellData[] Cells = { };
        public bool IsOptimistic;
    }

    public class CellData
    {
        public int Id;
        public string Values;
        public int Counter;
        public int Limit;
        public bool IsDisabled;
        public QuotaLimitPriority Priority = QuotaLimitPriority.Medium;
    }

    public class InboundTelephoneNumberData
    {
        public string TelephoneNumber;
        public string Dialer;
    }

    public abstract class BaseAlertData
    {
        public int Amber;
        public int Red;
    }

    public class ExtendedStatusAlertData : BaseAlertData
    {
        public CallOutcome ITS;
    }

    public class AlertData : BaseAlertData
    {
        public BvThresholdType Type;
    }

    public enum FilterJoinType { And, Or }
    public class FilterData : TaggedObjectData
    {
        public FilterJoinType Join = FilterJoinType.And;
        public string[] Conditions = { };
    }

    public class ExternalNumberData : TaggedObjectData
    {
        public string Phone;
        public string Description;
        public bool Hidden;
        public string Assigns;
    }

    public class StateData
    {
        public int StateID;
        public string Name;
    }
}
