/*At this time DB should be created*/
/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
SET DATEFORMAT YMD
SET ANSI_PADDING,ANSI_WARNINGS,CONCAT_NULL_YIELDS_NULL,ARITHABORT,QUOTED_IDENTIFIER,ANSI_NULLS ON
SET NUMERIC_ROUNDABORT OFF

GO
PRINT N'Creating [dbo].[BvTimeBreaksHistory]...';


GO
CREATE TABLE [dbo].[BvTimeBreaksHistory] (
    [ID]            INT      IDENTITY (1, 1) NOT NULL,
    [StartTime]     DATETIME NOT NULL,
    [InterviewerId] INT      NOT NULL,
    [Duration]      INT      NULL,
    CONSTRAINT [PK_BvTimeBreaksHistory_Id] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating [dbo].[BvTimeBreaksHistory].[IX_BvTimeBreaksHistory_Clustered]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvTimeBreaksHistory_Clustered]
    ON [dbo].[BvTimeBreaksHistory]([InterviewerId] ASC, [StartTime] ASC);


GO
PRINT N'Creating [dbo].[BvTelephoneBlacklist]...';


GO
CREATE TABLE [dbo].[BvTelephoneBlacklist] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [TelephoneNumber] VARCHAR (255) NOT NULL,
    CONSTRAINT [PK_BvTelephoneBlacklist] PRIMARY KEY NONCLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[BvTelephoneBlacklist].[IX_BvTelephoneBlacklist]...';


GO
CREATE UNIQUE CLUSTERED INDEX [IX_BvTelephoneBlacklist]
    ON [dbo].[BvTelephoneBlacklist]([TelephoneNumber] ASC) WITH (IGNORE_DUP_KEY = ON)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[BvSearchableFields]...';


GO
CREATE TABLE [dbo].[BvSearchableFields] (
    [SurveyId] INT NOT NULL,
    [ColumnId] INT NOT NULL,
    [TableId]  INT NOT NULL,
    [UseMode]  INT NOT NULL,
    CONSTRAINT [PK_BvSearchableFields] PRIMARY KEY CLUSTERED ([SurveyId] ASC, [UseMode] ASC, [ColumnId] ASC, [TableId] ASC)
);


GO
PRINT N'Creating [dbo].[BvScheduleParam]...';


GO
CREATE TABLE [dbo].[BvScheduleParam] (
    [ScheduleID]  INT            NOT NULL,
    [SurveySID]   INT            NOT NULL,
    [ParamID]     INT            NOT NULL,
    [Name]        NVARCHAR (256) NOT NULL,
    [Description] NVARCHAR (MAX) NULL,
    [Type]        INT            NOT NULL,
    [Value]       INT            NOT NULL,
    CONSTRAINT [PK_BvScheduleParam_SurveySIDParamID] PRIMARY KEY CLUSTERED ([ScheduleID] ASC, [SurveySID] ASC, [ParamID] ASC)
);


GO
PRINT N'Creating [dbo].[BvQuotaFilter]...';


GO
CREATE TABLE [dbo].[BvQuotaFilter] (
    [surveyId]  INT            NOT NULL,
    [FieldName] NVARCHAR (MAX) NOT NULL
);


GO
PRINT N'Creating [dbo].[BvQuotaFilter].[IX_BvQuotaFilter_Clustered_SurveyId]...';


GO
CREATE CLUSTERED INDEX [IX_BvQuotaFilter_Clustered_SurveyId]
    ON [dbo].[BvQuotaFilter]([surveyId] ASC);


GO
PRINT N'Creating [dbo].[BvQuotaBalancing]...';


GO
CREATE TABLE [dbo].[BvQuotaBalancing] (
    [surveyId]             INT  NOT NULL,
    [quotaId]              INT  NOT NULL,
    [priority]             INT  NOT NULL,
    [promotionThreshold]   INT  NOT NULL,
    [promotionCoefficient] REAL NOT NULL,
    CONSTRAINT [PK_BvQuotaBalancing] PRIMARY KEY CLUSTERED ([surveyId] ASC)
);


GO
PRINT N'Creating [dbo].[BvInterviewTimings]...';


GO
CREATE TABLE [dbo].[BvInterviewTimings] (
    [InterviewID]            INT      NOT NULL,
    [SurveyID]               INT      NOT NULL,
    [TimeCallDelivered]      DATETIME NULL,
    [InterviewDuriationTime] INT      NULL,
    [WaitingTime]            INT      NULL,
    CONSTRAINT [PK_BvInterviewTimings] PRIMARY KEY CLUSTERED ([SurveyID] ASC, [InterviewID] ASC)
);


GO
PRINT N'Creating [dbo].[BvDialers]...';


GO
CREATE TABLE [dbo].[BvDialers] (
    [Id]                                 INT            NOT NULL,
    [Name]                               NVARCHAR (255) NOT NULL,
    [ConnectionParameters]               NVARCHAR (MAX) NULL,
    [ConfigurationParameters]            NVARCHAR (MAX) NULL,
    [TenantId]                           INT            NOT NULL,
    [DialerOperationalStateNotification] BIT            NOT NULL,
    CONSTRAINT [PK_BvDialers_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[BvAsyncOperations]...';


GO
CREATE TABLE [dbo].[BvAsyncOperations] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [StartTime]        DATETIME       NULL,
    [EndTime]          DATETIME       NULL,
    [Text]             NVARCHAR (MAX) NULL,
    [ProcessedPercent] TINYINT        NULL,
    [Status]           INT            NOT NULL,
    [Type]             INT            NOT NULL,
    [SupervisorName]   NVARCHAR (255) NULL,
    CONSTRAINT [PK_BvAsyncOperations_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[BvActiveCallsInfo]...';


GO
CREATE TABLE [dbo].[BvActiveCallsInfo] (
    [Time]        DATETIME NOT NULL,
    [SurveySID]   INT      NOT NULL,
    [ExplicitSID] INT      NOT NULL,
    [CallsCount]  INT      NOT NULL
);


GO
PRINT N'Creating [dbo].[BvActiveCallsInfo].[IX_BvActiveCallsInfo_Time]...';


GO
CREATE CLUSTERED INDEX [IX_BvActiveCallsInfo_Time]
    ON [dbo].[BvActiveCallsInfo]([Time] ASC);


GO
PRINT N'Creating [dbo].[BvAppLocks]...';


GO
CREATE TABLE [dbo].[BvAppLocks] (
    [ResourceName]  NVARCHAR (255) NOT NULL,
    [TimeLockEnter] DATETIME       NULL,
    [TimeLockLeave] DATETIME       NULL,
    [IsLockHeld]    BIT            NULL,
    [ServerName]    NVARCHAR (255) NULL,
    [ResourceOwner] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_BvAppLocks_ResourceName] PRIMARY KEY CLUSTERED ([ResourceName] ASC)
);


GO
PRINT N'Creating [dbo].[BvAnswerSubmissionAlertHistory]...';


GO
CREATE TABLE [dbo].[BvAnswerSubmissionAlertHistory] (
    [PersonId]                   INT            NULL,
    [SubmissionTime]             DATETIME       NULL,
    [QuestionId]                 NVARCHAR (256) NULL,
    [SurveyId]                   INT            NULL,
    [InterviewId]                INT            NULL,
    [AnswerDuration]             INT            NULL,
    [AnswerSubmissionAlert]      BIT            NULL,
    [QuickAnswerSubmissionAlert] BIT            NULL,
    [InterviewState]             TINYINT        NOT NULL
);


GO
PRINT N'Creating [dbo].[BvAnswerSubmissionAlertHistory].[IX_BvAnswerSubmissionAlertHistory]...';


GO
CREATE CLUSTERED INDEX [IX_BvAnswerSubmissionAlertHistory]
    ON [dbo].[BvAnswerSubmissionAlertHistory]([SubmissionTime] ASC);


GO
PRINT N'Creating [dbo].[BvSystemSettings]...';


GO
CREATE TABLE [dbo].[BvSystemSettings] (
    [SystemName]  NVARCHAR (256) NOT NULL,
    [DisplayName] NVARCHAR (256) NULL,
    [Group]       NVARCHAR (256) NOT NULL,
    [Description] NVARCHAR (MAX) NOT NULL,
    [Type]        INT            NOT NULL,
    [Hidden]      BIT            NOT NULL,
    [Value]       NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_BvSystemSettings_SystemName] PRIMARY KEY CLUSTERED ([SystemName] ASC)
);


GO
PRINT N'Creating [dbo].[BvUserNotification]...';


GO
CREATE TABLE [dbo].[BvUserNotification] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Type]     INT            NOT NULL,
    [ObjectId] INT            NOT NULL,
    [SendDate] DATETIME       NOT NULL,
    [Subject]  NVARCHAR (MAX) NOT NULL,
    [Body]     NVARCHAR (MAX) NULL
);


GO
PRINT N'Creating [dbo].[BvInterviewerPerformance]...';


GO
CREATE TABLE [dbo].[BvInterviewerPerformance] (
    [InterviewerId]            INT            NOT NULL,
    [InterviewerName]          NVARCHAR (255) NOT NULL,
    [TotalInterviewCount]      INT            NOT NULL,
    [CompletedInterviewCount]  INT            NOT NULL,
    [CompletedInLastHourCount] INT            NOT NULL,
    [InterviewingTime]         INT            NOT NULL,
    CONSTRAINT [PK_BvInterviewerPerformance_InterviewerId] PRIMARY KEY CLUSTERED ([InterviewerId] ASC)
);


GO
PRINT N'Creating [dbo].[BvCachedCallsSwapTable]...';


GO
CREATE TABLE [dbo].[BvCachedCallsSwapTable] (
    [ID]          INT      NOT NULL,
    [ExplicitSID] INT      NOT NULL,
    [SurveySID]   INT      NOT NULL,
    [InterviewID] INT      NOT NULL,
    [CallState]   INT      NOT NULL,
    [TimeInShift] DATETIME NULL,
    [OrderId]     INT      NOT NULL,
    CONSTRAINT [PK_BvCachedCallsSwapTable_SurveySidInterviewId] PRIMARY KEY CLUSTERED ([SurveySID] ASC, [InterviewID] ASC)
);


GO
PRINT N'Creating [dbo].[BvPromotionHistory]...';


GO
CREATE TABLE [dbo].[BvPromotionHistory] (
    [ID]                  INT            IDENTITY (1, 1) NOT NULL,
    [QuotaId]             INT            NOT NULL,
    [SurveyId]            INT            NOT NULL,
    [FiredTime]           DATETIME       NOT NULL,
    [CallsToPromoteCount] INT            NOT NULL,
    [PromotedCallsCount]  INT            NOT NULL,
    [CellId]              INT            NOT NULL,
    [CellInfo]            NVARCHAR (MAX) NOT NULL
);


GO
PRINT N'Creating [dbo].[BvClosedCellHistory]...';


GO
CREATE TABLE [dbo].[BvClosedCellHistory] (
    [Id]                    INT            IDENTITY (1, 1) NOT NULL,
    [ClosingTime]           DATETIME       NOT NULL,
    [SurveySid]             INT            NOT NULL,
    [QuotaId]               INT            NOT NULL,
    [CellId]                INT            NOT NULL,
    [GeneratedWhereForCell] NVARCHAR (MAX) NOT NULL
);


GO
PRINT N'Creating [dbo].[BvClosedCellHistory].[IX_Date_ClosedCellHistory]...';


GO
CREATE NONCLUSTERED INDEX [IX_Date_ClosedCellHistory]
    ON [dbo].[BvClosedCellHistory]([SurveySid] ASC, [ClosingTime] ASC)
    INCLUDE([GeneratedWhereForCell]);


GO
PRINT N'Creating [dbo].[BvReplicationColumns]...';


GO
CREATE TABLE [dbo].[BvReplicationColumns] (
    [TableID]         INT            NOT NULL,
    [ColumnID]        INT            NOT NULL,
    [ColumnName]      NVARCHAR (255) NOT NULL,
    [ColumnType]      INT            NOT NULL,
    [ColumnMaxLength] INT            NULL,
    CONSTRAINT [PK_BvReplicationColumns] PRIMARY KEY CLUSTERED ([TableID] ASC, [ColumnID] ASC)
);


GO
PRINT N'Creating [dbo].[AudioMonitoring]...';


GO
CREATE TABLE [dbo].[AudioMonitoring] (
    [SupervisorName]  NVARCHAR (255) NOT NULL,
    [InterviewerSID]  INT            NOT NULL,
    [TelephoneNumber] NVARCHAR (255) NOT NULL,
    [SessionID]       NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_AudioMonitoring_SupervisorName] PRIMARY KEY CLUSTERED ([SupervisorName] ASC)
);


GO
PRINT N'Creating [dbo].[BvCallGroupConditionPerSurvey]...';


GO
CREATE TABLE [dbo].[BvCallGroupConditionPerSurvey] (
    [SurveyId]          INT       NOT NULL,
    [CallGroupId]       INT       NOT NULL,
    [ConditionValue]    INT       NOT NULL,
    [ConditionPriority] INT       NOT NULL,
    [RotatePriority]    TIMESTAMP NOT NULL,
    CONSTRAINT [PK_BvCallGroupConditionPerSurvey] PRIMARY KEY CLUSTERED ([SurveyId] ASC, [CallGroupId] ASC, [ConditionValue] ASC)
);


GO
PRINT N'Creating [dbo].[BvVersionHistory]...';


GO
CREATE TABLE [dbo].[BvVersionHistory] (
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
    [Major]                     INT            NOT NULL,
    [Minor]                     INT            NOT NULL,
    [BranchName]                NVARCHAR (MAX) NOT NULL,
    [ScriptNumber]              INT            NOT NULL,
    [Description]               NVARCHAR (MAX) NOT NULL,
    [ScriptAppliedDate]         DATETIME       NOT NULL,
    [Duration]                  INT            NOT NULL,
    [ScriptText]                NVARCHAR (MAX) NOT NULL,
    [ScriptOutput]              NVARCHAR (MAX) NOT NULL,
    [IsAppliedDuringDBCreation] BIT            NOT NULL,
    [DbUpateUtilityVersion]     NVARCHAR (MAX) NOT NULL,
    [ActiveUser]                NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_BvVersionHistory_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[BvScheduledEmailReports]...';


GO
CREATE TABLE [dbo].[BvScheduledEmailReports] (
    [ReportType] INT      NOT NULL,
    [LastSent]   DATETIME NULL,
    CONSTRAINT [PK_BvScheduledEmailReports_ReportType] PRIMARY KEY CLUSTERED ([ReportType] ASC)
);


GO
PRINT N'Creating [dbo].[BvCallGroupCondition]...';


GO
CREATE TABLE [dbo].[BvCallGroupCondition] (
    [CallGroupId]       INT       NOT NULL,
    [ConditionValue]    INT       NOT NULL,
    [ConditionPriority] INT       NOT NULL,
    [RotatePriority]    TIMESTAMP NOT NULL,
    CONSTRAINT [PK_BvCallGroupCondition] PRIMARY KEY CLUSTERED ([CallGroupId] ASC, [ConditionValue] ASC)
);


GO
PRINT N'Creating [dbo].[BvCallGroup]...';


GO
CREATE TABLE [dbo].[BvCallGroup] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (256) NOT NULL,
    [Description] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_BvCallGroup] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[BvUserSurveyPermission]...';


GO
CREATE TABLE [dbo].[BvUserSurveyPermission] (
    [UserName]  NVARCHAR (255) NOT NULL,
    [SurveySID] INT            NOT NULL,
    CONSTRAINT [pk_BvUserSurveyPermission] PRIMARY KEY CLUSTERED ([UserName] ASC, [SurveySID] ASC)
);


GO
PRINT N'Creating [dbo].[BvUniqueAssignments]...';


GO
CREATE TABLE [dbo].[BvUniqueAssignments] (
    [sid] INT NOT NULL,
    CONSTRAINT [pk_BvUniqueAssignments] PRIMARY KEY CLUSTERED ([sid] ASC)
);


GO
PRINT N'Creating [dbo].[BvTzUnPeriodicalShifts]...';


GO
CREATE TABLE [dbo].[BvTzUnPeriodicalShifts] (
    [shift_id]  INT           NOT NULL,
    [type_id]   INT           NOT NULL,
    [owner_id]  INT           NOT NULL,
    [tz_id]     INT           NOT NULL,
    [start_dt]  SMALLDATETIME NOT NULL,
    [finish_dt] SMALLDATETIME NOT NULL,
    CONSTRAINT [pk_BvTzUnPeriodicalShifts] PRIMARY KEY CLUSTERED ([tz_id] ASC, [start_dt] ASC, [finish_dt] ASC, [owner_id] ASC, [shift_id] ASC, [type_id] ASC)
);


GO
PRINT N'Creating [dbo].[BvTzPeriodicalShifts]...';


GO
CREATE TABLE [dbo].[BvTzPeriodicalShifts] (
    [shift_id]  INT NOT NULL,
    [type_id]   INT NOT NULL,
    [owner_id]  INT NOT NULL,
    [tz_id]     INT NOT NULL,
    [start_dt]  INT NOT NULL,
    [finish_dt] INT NOT NULL,
    CONSTRAINT [pk_BvTzPeriodicalShifts] PRIMARY KEY CLUSTERED ([tz_id] ASC, [start_dt] ASC, [finish_dt] ASC, [owner_id] ASC, [shift_id] ASC, [type_id] ASC)
);


GO
PRINT N'Creating [dbo].[BvTransferBatches]...';


GO
CREATE TABLE [dbo].[BvTransferBatches] (
    [LastBatchID] INT NOT NULL
);


GO
PRINT N'Creating [dbo].[BvTransferArrays]...';


GO
CREATE TABLE [dbo].[BvTransferArrays] (
    [BatchID] INT NOT NULL,
    [ItemID]  INT NOT NULL
);


GO
PRINT N'Creating [dbo].[BvTransferArrays].[IX_BvTransferArrays]...';


GO
CREATE CLUSTERED INDEX [IX_BvTransferArrays]
    ON [dbo].[BvTransferArrays]([BatchID] ASC, [ItemID] ASC);


GO
PRINT N'Creating [dbo].[BvTimezoneShift]...';


GO
CREATE TABLE [dbo].[BvTimezoneShift] (
    [OwnerSID]        INT      NOT NULL,
    [ShiftID]         INT      NOT NULL,
    [TimezoneID]      INT      NOT NULL,
    [StartDayOfWeek]  INT      NULL,
    [StartTime]       DATETIME NOT NULL,
    [FinishDayOfWeek] INT      NULL,
    [FinishTime]      DATETIME NOT NULL,
    CONSTRAINT [BvPkTimezoneShIFt] PRIMARY KEY CLUSTERED ([OwnerSID] ASC, [ShiftID] ASC, [TimezoneID] ASC)
);


GO
PRINT N'Creating [dbo].[BvTimezoneMaster]...';


GO
CREATE TABLE [dbo].[BvTimezoneMaster] (
    [ID]                INT            NOT NULL,
    [Name]              NVARCHAR (255) NOT NULL,
    [Bias]              INT            NOT NULL,
    [DaylightType]      INT            NOT NULL,
    [StandardName]      NVARCHAR (255) NOT NULL,
    [StandardStart]     DATETIME       NULL,
    [StandardDayOfWeek] INT            NULL,
    [StandardBias]      INT            NOT NULL,
    [DaylightName]      NVARCHAR (255) NOT NULL,
    [DaylightStart]     DATETIME       NULL,
    [DaylightDayOfWeek] INT            NULL,
    [DaylightBias]      INT            NOT NULL,
    CONSTRAINT [PK_BvTimezoneMaster_Id] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating [dbo].[BvTimezone]...';


GO
CREATE TABLE [dbo].[BvTimezone] (
    [ID]                INT            NOT NULL,
    [Name]              NVARCHAR (255) NOT NULL,
    [Bias]              INT            NOT NULL,
    [DaylightType]      INT            NOT NULL,
    [StandardName]      NVARCHAR (255) NOT NULL,
    [StandardStart]     DATETIME       NULL,
    [StandardDayOfWeek] INT            NULL,
    [StandardBias]      INT            NOT NULL,
    [DaylightName]      NVARCHAR (255) NOT NULL,
    [DaylightStart]     DATETIME       NULL,
    [DaylightDayOfWeek] INT            NULL,
    [DaylightBias]      INT            NOT NULL,
    CONSTRAINT [PK_BvTimezone_Id] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating [dbo].[BvThresholdTypes]...';


GO
CREATE TABLE [dbo].[BvThresholdTypes] (
    [ID]          INT            NOT NULL,
    [Description] NVARCHAR (255) NOT NULL,
    CONSTRAINT [Pk_BvThresholdTypes] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating [dbo].[BvThresholds]...';


GO
CREATE TABLE [dbo].[BvThresholds] (
    [ObjectSID]        INT NOT NULL,
    [ThresholdsTypeID] INT NOT NULL,
    [Amber]            INT NOT NULL,
    [Red]              INT NOT NULL,
    CONSTRAINT [PkBvThresholds] PRIMARY KEY CLUSTERED ([ObjectSID] ASC, [ThresholdsTypeID] ASC)
);


GO
PRINT N'Creating [dbo].[BvThresholdITS]...';


GO
CREATE TABLE [dbo].[BvThresholdITS] (
    [SurveySID] INT NOT NULL,
    [ITS]       INT NOT NULL,
    [Amber]     INT NOT NULL,
    [Red]       INT NOT NULL,
    CONSTRAINT [Pk_BvThresholdITS] PRIMARY KEY CLUSTERED ([SurveySID] ASC, [ITS] ASC)
);


GO
PRINT N'Creating [dbo].[BvTasks]...';


GO
CREATE TABLE [dbo].[BvTasks] (
    [SurveySID]                  INT              NOT NULL,
    [InterviewID]                INT              NOT NULL,
    [PersonSID]                  INT              NOT NULL,
    [TimeCallDelivered]          DATETIME         NULL,
    [State]                      NVARCHAR (256)   NULL,
    [TimeStateChanged]           DATETIME         NULL,
    [SecondsSinceLastSubmission] INT              NOT NULL,
    [LastSubmissionAlert]        INT              NOT NULL,
    [TzID]                       INT              NOT NULL,
    [DiallingMode]               INT              NOT NULL,
    [CallOutcome]                INT              NOT NULL,
    [InterviewState]             TINYINT          NOT NULL,
    [StatusLogout]               TINYINT          NOT NULL,
    [LoggedInToDialerState]      TINYINT          NOT NULL,
    [IsLoginRCToDialer]          BIT              NOT NULL,
    [CallID]                     INT              NULL,
    [LastKeepAliveTime]          DATETIME         NULL,
    [LastKeepAliveTimeAlert]     INT              NOT NULL,
    [ProblemId]                  INT              NOT NULL,
    [LockTime]                   SMALLDATETIME    NULL,
    [StationId]                  NVARCHAR (256)   NOT NULL,
    [StartTime]                  DATETIME         NULL,
    [AuthenticationKey]          UNIQUEIDENTIFIER NULL,
    [StartSessionTime]           DATETIME         NOT NULL,
    [EncryptionKey]              VARBINARY (64)   NOT NULL,
    [EncryptionIV]               VARBINARY (64)   NOT NULL,
    CONSTRAINT [PkBvTasks] PRIMARY KEY CLUSTERED ([PersonSID] ASC)
);


GO
PRINT N'Creating [dbo].[BvSvySchedule]...';


GO
CREATE TABLE [dbo].[BvSvySchedule] (
    [ID]                  INT              IDENTITY (1, 1) NOT NULL,
    [ApptID]              INT              NOT NULL,
    [ShiftTypeID]         INT              NOT NULL,
    [InterviewID]         INT              NULL,
    [SurveySID]           INT              NOT NULL,
    [CallState]           INT              NOT NULL,
    [Priority]            INT              NOT NULL,
    [TimeInShift]         DATETIME         NULL,
    [ExpireTime]          DATETIME         NOT NULL,
    [ExplicitSID]         INT              NOT NULL,
    [ExplicitType]        INT              NOT NULL,
    [RuleNumber]          UNIQUEIDENTIFIER NOT NULL,
    [IsInActiveShiftType] BIT              NOT NULL,
    [CallOrder]           INT              NOT NULL,
    [OldPriority]         INT              NOT NULL,
    [ConditionValue]      INT              NOT NULL
);


GO
PRINT N'Creating [dbo].[BvSvySchedule].[IX_Priority]...';


GO
CREATE UNIQUE CLUSTERED INDEX [IX_Priority]
    ON [dbo].[BvSvySchedule]([SurveySID] ASC, [InterviewID] ASC);


GO
PRINT N'Creating [dbo].[BvSvySchedule].[IX_BvSvyScheduleMain]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_BvSvyScheduleMain]
    ON [dbo].[BvSvySchedule]([IsInActiveShiftType] ASC, [ExplicitSID] ASC, [Priority] DESC, [TimeInShift] ASC, [SurveySID] ASC, [CallOrder] ASC, [InterviewID] ASC)
    INCLUDE([ID], [CallState], [ExplicitType], [ApptID], [ConditionValue]);


GO
PRINT N'Creating [dbo].[BvSvySchedule].[IX_GetCallBySurvey]...';


GO
CREATE NONCLUSTERED INDEX [IX_GetCallBySurvey]
    ON [dbo].[BvSvySchedule]([IsInActiveShiftType] ASC, [CallState] ASC, [SurveySID] ASC, [ExplicitSID] ASC, [Priority] DESC, [TimeInShift] ASC, [ExplicitType] DESC, [CallOrder] ASC)
    INCLUDE([ID], [ExpireTime]) WHERE [IsInActiveShiftType] = 1 AND CallState = 2 AND ConditionValue <> 0;


GO
PRINT N'Creating [dbo].[BvSvySchedule].[IX_GetCallByCondition]...';


GO
CREATE NONCLUSTERED INDEX [IX_GetCallByCondition]
    ON [dbo].[BvSvySchedule]([IsInActiveShiftType] ASC, [CallState] ASC, [SurveySID] ASC, [ExplicitSID] ASC, [ConditionValue] ASC, [Priority] DESC, [TimeInShift] ASC, [ExplicitType] DESC, [CallOrder] ASC)
    INCLUDE([ID], [ExpireTime]) WHERE [IsInActiveShiftType] = 1 AND CallState = 2 AND ConditionValue <> 0;


GO
PRINT N'Creating [dbo].[BvSvySchedule].[IX_BvTime]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvTime]
    ON [dbo].[BvSvySchedule]([ExpireTime] ASC);


GO
PRINT N'Creating [dbo].[BvSvySchedule].[IX_BvSvySchedule_Rel]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvSvySchedule_Rel]
    ON [dbo].[BvSvySchedule]([ExplicitSID] ASC);


GO
PRINT N'Creating [dbo].[BvSvySchedule].[IX_BvSvySchedule_CallState]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvSvySchedule_CallState]
    ON [dbo].[BvSvySchedule]([CallState] ASC);


GO
PRINT N'Creating [dbo].[BvSvySchedule].[IX_BvSvyScheduleCallID]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_BvSvyScheduleCallID]
    ON [dbo].[BvSvySchedule]([ID] ASC);


GO
PRINT N'Creating [dbo].[BvSurveyListAlertsViewConfiguration]...';


GO
CREATE TABLE [dbo].[BvSurveyListAlertsViewConfiguration] (
    [UpdatingTime]               INT      NOT NULL,
    [LastCall]                   DATETIME NULL,
    [SyncUpdatingTime]           INT      NOT NULL,
    [SyncLastCall]               DATETIME NULL,
    [IdlePeriodMaxCountOfChecks] INT      NOT NULL,
    [IdlePeriodCheckCounter]     INT      NOT NULL,
    [IdlePeriodMaxSeconds]       INT      NOT NULL
);


GO
PRINT N'Creating [dbo].[BvSurvey]...';


GO
CREATE TABLE [dbo].[BvSurvey] (
    [SID]                           INT            NOT NULL,
    [Number]                        INT            NOT NULL,
    [Name]                          NVARCHAR (255) NULL,
    [Description]                   NVARCHAR (255) NULL,
    [QuotaType]                     TINYINT        NOT NULL,
    [State]                         INT            NOT NULL,
    [ForceOpnRev]                   INT            NOT NULL,
    [StateGroupID]                  INT            NOT NULL,
    [RecWholeInt]                   INT            NOT NULL,
    [InterviewScreenRecording]      BIT            NOT NULL,
    [CfDbSchemaPath]                NVARCHAR (255) NOT NULL,
    [DestinationTableName]          NVARCHAR (255) NULL,
    [ReplicationStatus]             BIT            NULL,
    [ScheduleID]                    INT            NOT NULL,
    [DialerParameters]              NVARCHAR (MAX) NULL,
    [DialMode]                      TINYINT        NOT NULL,
    [IsTelephoneBlacklistSupported] BIT            NOT NULL,
    [NotificationEmail]             NVARCHAR (MAX) NULL,
    [IsRandomCallDeliveryEnabled]   BIT            NOT NULL,
    [EnforceHttps]                  BIT            NOT NULL,
    [LastTouchTime]                 SMALLDATETIME  NULL,
    [SurveySchedulingMode]          SMALLINT       NOT NULL,
    CONSTRAINT [PkBvSurvey] PRIMARY KEY CLUSTERED ([SID] ASC),
    CONSTRAINT [UQ_BvSurvey_1__33] UNIQUE NONCLUSTERED ([Name] ASC)
);


GO
PRINT N'Creating [dbo].[BvSurvey].[IX_BvSurvey_SurveyActivity]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_BvSurvey_SurveyActivity]
    ON [dbo].[BvSurvey]([State] ASC, [SID] ASC)
    INCLUDE([Name], [Description]);


GO
PRINT N'Creating [dbo].[BvStateGroup]...';


GO
CREATE TABLE [dbo].[BvStateGroup] (
    [ID]      INT            NOT NULL,
    [Name]    NVARCHAR (255) NOT NULL,
    [Order]   INT            NOT NULL,
    [Deleted] INT            NOT NULL,
    CONSTRAINT [PK_BvStateGroup] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating [dbo].[BvState]...';


GO
CREATE TABLE [dbo].[BvState] (
    [StateID]      INT            NOT NULL,
    [Name]         NVARCHAR (255) NOT NULL,
    [Priority]     INT            NOT NULL,
    [StateGroupID] INT            NOT NULL,
    [DA]           INT            NOT NULL,
    CONSTRAINT [PK_BvState] PRIMARY KEY CLUSTERED ([StateID] ASC, [StateGroupID] ASC, [DA] ASC)
);


GO
PRINT N'Creating [dbo].[BvSIDCounter]...';


GO
CREATE TABLE [dbo].[BvSIDCounter] (
    [SID] INT NOT NULL
);


GO
PRINT N'Creating [dbo].[BvShiftZones]...';


GO
CREATE TABLE [dbo].[BvShiftZones] (
    [ID]          INT IDENTITY (1, 1) NOT NULL,
    [TimeZoneID]  INT NOT NULL,
    [ShiftTypeID] INT NOT NULL,
    CONSTRAINT [PK_BvShiftZones] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_BvShiftZones] UNIQUE NONCLUSTERED ([TimeZoneID] ASC, [ShiftTypeID] ASC)
);


GO
PRINT N'Creating [dbo].[BvShiftType]...';


GO
CREATE TABLE [dbo].[BvShiftType] (
    [OwnerSID] INT           NOT NULL,
    [ID]       INT           NOT NULL,
    [Name]     VARCHAR (255) NOT NULL,
    [Color]    INT           NOT NULL,
    [ObjectID] INT           IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_BvShiftType] PRIMARY KEY CLUSTERED ([ObjectID] ASC),
    CONSTRAINT [IX_BvShiftType] UNIQUE NONCLUSTERED ([OwnerSID] ASC, [Name] ASC)
);


GO
PRINT N'Creating [dbo].[BvShift]...';


GO
CREATE TABLE [dbo].[BvShift] (
    [OwnerSID]        INT      NOT NULL,
    [ID]              INT      NOT NULL,
    [CycleType]       INT      NOT NULL,
    [StartDayOfWeek]  INT      NULL,
    [StartTime]       DATETIME NOT NULL,
    [FinishDayOfWeek] INT      NULL,
    [FinishTime]      DATETIME NOT NULL,
    [ShiftTypeID]     INT      NOT NULL,
    CONSTRAINT [BvPkShIFt] PRIMARY KEY CLUSTERED ([OwnerSID] ASC, [ID] ASC)
);


GO
PRINT N'Creating [dbo].[BvSchedule]...';


GO
CREATE TABLE [dbo].[BvSchedule] (
    [ScheduleID]           INT            NOT NULL,
    [XmlInUse]             NVARCHAR (MAX) NOT NULL,
    [XmlUnderDev]          NVARCHAR (MAX) NOT NULL,
    [ScriptSource]         NVARCHAR (MAX) NULL,
    [Name]                 NVARCHAR (255) NOT NULL,
    [CreateDate]           DATETIME       NOT NULL,
    [ModifyDate]           DATETIME       NOT NULL,
    [RegenerateIsRequired] BIT            NOT NULL,
    [DesignStateGroupID]   INT            NULL,
    CONSTRAINT [PK_BvSchedule] PRIMARY KEY CLUSTERED ([ScheduleID] ASC),
    CONSTRAINT [UQ_BvSchedule_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);


GO
PRINT N'Creating [dbo].[BvSampleStatusSummary]...';


GO
CREATE TABLE [dbo].[BvSampleStatusSummary] (
    [SurveySID]   INT NOT NULL,
    [ITS]         INT NOT NULL,
    [Cnt]         INT NOT NULL,
    [AlertStatus] INT NOT NULL,
    CONSTRAINT [Pk_BvSampleStatusSummary] PRIMARY KEY CLUSTERED ([SurveySID] ASC, [ITS] ASC)
);


GO
PRINT N'Creating [dbo].[BvSamples]...';


GO
CREATE TABLE [dbo].[BvSamples] (
    [BatchID]          INT            NOT NULL,
    [SurveySID]        INT            NOT NULL,
    [State]            INT            NOT NULL,
    [StateDescription] NVARCHAR (MAX) NOT NULL,
    [StartedTime]      DATETIME       NOT NULL,
    [FinishedTime]     DATETIME       NULL,
    [CountInterviews]  INT            NOT NULL,
    CONSTRAINT [Pk_BvSamples] PRIMARY KEY CLUSTERED ([BatchID] ASC)
);


GO
PRINT N'Creating [dbo].[BvSamples].[ix_BvSamples1]...';


GO
CREATE NONCLUSTERED INDEX [ix_BvSamples1]
    ON [dbo].[BvSamples]([State] ASC);


GO
PRINT N'Creating [dbo].[BvSamples].[ix_BvSamples]...';


GO
CREATE NONCLUSTERED INDEX [ix_BvSamples]
    ON [dbo].[BvSamples]([SurveySID] ASC, [StartedTime] ASC);


GO
PRINT N'Creating [dbo].[BvRole]...';


GO
CREATE TABLE [dbo].[BvRole] (
    [RoleID] INT            NOT NULL,
    [Name]   NVARCHAR (255) NOT NULL,
    CONSTRAINT [PkBvRole] PRIMARY KEY CLUSTERED ([RoleID] ASC)
);


GO
PRINT N'Creating [dbo].[BvReportParam]...';


GO
CREATE TABLE [dbo].[BvReportParam] (
    [BatchID] INT NOT NULL,
    [ParamID] INT NOT NULL,
    [Val]     INT NOT NULL
);


GO
PRINT N'Creating [dbo].[BvReportBatch]...';


GO
CREATE TABLE [dbo].[BvReportBatch] (
    [ID]          INT      IDENTITY (1, 1) NOT NULL,
    [PersonSID]   INT      NOT NULL,
    [ReportID]    INT      NOT NULL,
    [TimeCreated] DATETIME NOT NULL,
    CONSTRAINT [PK_BvReportBatch] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating [dbo].[BvReport]...';


GO
CREATE TABLE [dbo].[BvReport] (
    [Rpt_ID]             INT            IDENTITY (1, 1) NOT NULL,
    [Rpt_TargetClassID]  INT            NOT NULL,
    [Rpt_Name]           NVARCHAR (255) NOT NULL,
    [Rpt_FileName]       NVARCHAR (255) NOT NULL,
    [Rpt_DialogFileName] NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK__BvReport__267ABA7A] PRIMARY KEY CLUSTERED ([Rpt_ID] ASC)
);


GO
PRINT N'Creating [dbo].[BvReplicationTables]...';


GO
CREATE TABLE [dbo].[BvReplicationTables] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [SurveySid]   INT            NOT NULL,
    [TableName]   NVARCHAR (255) NOT NULL,
    [LastVersion] BIGINT         NULL,
    [PrimaryKey]  NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_BvReplicationTables] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating [dbo].[BvPersonRel]...';


GO
CREATE TABLE [dbo].[BvPersonRel] (
    [PersonSID] INT NOT NULL,
    [ObjectSID] INT NOT NULL,
    [RoleID]    INT NOT NULL,
    [Type]      INT NOT NULL
);


GO
PRINT N'Creating [dbo].[BvPersonRel].[Pk_BvPersonRel]...';


GO
CREATE UNIQUE CLUSTERED INDEX [Pk_BvPersonRel]
    ON [dbo].[BvPersonRel]([PersonSID] ASC, [ObjectSID] ASC) WITH (IGNORE_DUP_KEY = ON);


GO
PRINT N'Creating [dbo].[BvPersonMonitoringLastID]...';


GO
CREATE TABLE [dbo].[BvPersonMonitoringLastID] (
    [PersonSID]           INT    NOT NULL,
    [MonitoringSessionID] BIGINT NOT NULL,
    [LastSentID]          BIGINT NOT NULL,
    CONSTRAINT [PK_BvPersonMonitoringLastID] PRIMARY KEY NONCLUSTERED ([PersonSID] ASC)
);


GO
PRINT N'Creating [dbo].[BvPersonMonitoringLastID].[CLIDX_BvPersonMonitoringLastID_PersonSID]...';


GO
CREATE UNIQUE CLUSTERED INDEX [CLIDX_BvPersonMonitoringLastID_PersonSID]
    ON [dbo].[BvPersonMonitoringLastID]([PersonSID] ASC);


GO
PRINT N'Creating [dbo].[BvPersonMonitoringEvents]...';


GO
CREATE TABLE [dbo].[BvPersonMonitoringEvents] (
    [ID]                  BIGINT          IDENTITY (1, 1) NOT NULL,
    [PersonSID]           INT             NOT NULL,
    [MonitoringSessionID] BIGINT          NOT NULL,
    [TimeStamp]           DATETIME        NOT NULL,
    [MessageType]         INT             NOT NULL,
    [EventObject]         VARBINARY (MAX) NULL,
    CONSTRAINT [PK_BvPersonMonitoringEvents] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating [dbo].[BvPersonMonitoringEvents].[CLIDX_BvPersonMonitoringEvents_PersonSID_MonitoringSessionID]...';


GO
CREATE CLUSTERED INDEX [CLIDX_BvPersonMonitoringEvents_PersonSID_MonitoringSessionID]
    ON [dbo].[BvPersonMonitoringEvents]([PersonSID] ASC, [MonitoringSessionID] ASC);


GO
PRINT N'Creating [dbo].[BvPersonMonitoring]...';


GO
CREATE TABLE [dbo].[BvPersonMonitoring] (
    [PersonSID]           INT            NOT NULL,
    [supervisorName]      NVARCHAR (255) NOT NULL,
    [MonitoringSessionID] BIGINT         NOT NULL,
    CONSTRAINT [PK_BvPersonMonitoring] PRIMARY KEY NONCLUSTERED ([PersonSID] ASC)
);


GO
PRINT N'Creating [dbo].[BvPersonMonitoring].[CLIDX_BvPersonMonitoring_PersonSID]...';


GO
CREATE UNIQUE CLUSTERED INDEX [CLIDX_BvPersonMonitoring_PersonSID]
    ON [dbo].[BvPersonMonitoring]([PersonSID] ASC);


GO
PRINT N'Creating [dbo].[BvPersonMonitoring].[NCLIDX_BvPersonMonitoring_MonitoringSessionID]...';


GO
CREATE NONCLUSTERED INDEX [NCLIDX_BvPersonMonitoring_MonitoringSessionID]
    ON [dbo].[BvPersonMonitoring]([MonitoringSessionID] ASC);


GO
PRINT N'Creating [dbo].[BvPersonGroup]...';


GO
CREATE TABLE [dbo].[BvPersonGroup] (
    [SID]             INT            NOT NULL,
    [Name]            NVARCHAR (255) NOT NULL,
    [Description]     NVARCHAR (255) NOT NULL,
    [RoleID]          INT            NOT NULL,
    [ManualSelection] INT            NOT NULL,
    CONSTRAINT [PkBvPersonGroup] PRIMARY KEY CLUSTERED ([SID] ASC)
);


GO
PRINT N'Creating [dbo].[BvPersonGroup].[IX_BvPersonGroup_Name]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_BvPersonGroup_Name]
    ON [dbo].[BvPersonGroup]([Name] ASC);


GO
PRINT N'Creating [dbo].[BvPersonDeferredMonitoring]...';


GO
CREATE TABLE [dbo].[BvPersonDeferredMonitoring] (
    [ID]                  INT             IDENTITY (1, 1) NOT NULL,
    [PersonSID]           INT             NOT NULL,
    [MonitoringSessionID] BIGINT          NOT NULL,
    [InterviewID]         INT             NOT NULL,
    [SurveySID]           INT             NOT NULL,
    [TimeStamp]           DATETIME        NOT NULL,
    [HasAudio]            BIT             NOT NULL,
    [EventsFile]          VARBINARY (MAX) NOT NULL,
    [StartingFile]        NVARCHAR (MAX)  NULL,
    [IsRecording]         BIT             NOT NULL,
    [IsComplete]          BIT             NOT NULL,
    [ClientTimeUtc]       DATETIME        NOT NULL,
    [ServerTimeUtc]       DATETIME        NOT NULL,
    [RequestAudio]        BIT             NOT NULL,
    [CallID]              INT SPARSE      NULL,
    [ExtendedStatus]      INT             NULL,
    CONSTRAINT [PK_BvPersonDeferredMonitoring] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring_TimeStamp]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvPersonDeferredMonitoring_TimeStamp]
    ON [dbo].[BvPersonDeferredMonitoring]([TimeStamp] ASC)
    INCLUDE([PersonSID], [SurveySID], [HasAudio], [InterviewID], [IsComplete])
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvPersonDeferredMonitoring]
    ON [dbo].[BvPersonDeferredMonitoring]([PersonSID] ASC, [MonitoringSessionID] ASC, [IsRecording] ASC, [IsComplete] ASC)
    INCLUDE([ID])
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring_CallID]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvPersonDeferredMonitoring_CallID]
    ON [dbo].[BvPersonDeferredMonitoring]([CallID] ASC) WHERE [CallID] IS NOT NULL;


GO
PRINT N'Creating [dbo].[BvPerson]...';


GO
CREATE TABLE [dbo].[BvPerson] (
    [SID]                    INT            NOT NULL,
    [Name]                   NVARCHAR (255) NOT NULL,
    [FullName]               NVARCHAR (255) NOT NULL,
    [Description]            NVARCHAR (255) NOT NULL,
    [ManualSelection]        INT            NOT NULL,
    [TimezoneID]             INT            NULL,
    [PwdHashTxt]             NVARCHAR (256) NOT NULL,
    [PwdSaltTxt]             NVARCHAR (256) NOT NULL,
    [DialerId]               INT            NOT NULL,
    [ExtensionNumber]        NVARCHAR (256) NOT NULL,
    [MNDiallerUserId]        NVARCHAR (256) NOT NULL,
    [DialerConnection]       NVARCHAR (256) NOT NULL,
    [DeskStationName]        NVARCHAR (256) NOT NULL,
    [HasNewMessage]          BIT            NULL,
    [AutomaticSurveyID]      INT            NULL,
    [AllowedChoices]         INT            NULL,
    [StationExtensionNumber] NVARCHAR (256) NOT NULL,
    [IsDialerAgentLocal]     BIT            NOT NULL,
    [FailedLoginAttempts]    INT            NOT NULL,
    [IsLocked]               BIT            NOT NULL,
    [LockedDate]             DATETIME       NULL,
    [AssignmentsListMode]    INT            NOT NULL,
    [CallGroupID]            INT            NULL,
    [Location]               NVARCHAR (256) NULL,
    CONSTRAINT [PK_BvPerson_SID] PRIMARY KEY CLUSTERED ([SID] ASC),
    CONSTRAINT [UQ_BvPerson_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);


GO
PRINT N'Creating [dbo].[BvNumber]...';


GO
CREATE TABLE [dbo].[BvNumber] (
    [ObjectSID] INT    NOT NULL,
    [ClassID]   INT    NOT NULL,
    [BvID]      BIGINT NOT NULL,
    CONSTRAINT [PkBvNumber] PRIMARY KEY CLUSTERED ([ObjectSID] ASC)
);


GO
PRINT N'Creating [dbo].[BvNumber].[IX_BvNumber]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_BvNumber]
    ON [dbo].[BvNumber]([ClassID] ASC, [BvID] ASC);


GO
PRINT N'Creating [dbo].[BvMessageToPerson]...';


GO
CREATE TABLE [dbo].[BvMessageToPerson] (
    [MessageId]     INT NOT NULL,
    [InterviewerId] INT NOT NULL,
    CONSTRAINT [PK_BvMessageToPerson] PRIMARY KEY CLUSTERED ([InterviewerId] ASC, [MessageId] ASC) ON [PRIMARY]
);


GO
PRINT N'Creating [dbo].[BvMessages]...';


GO
CREATE TABLE [dbo].[BvMessages] (
    [Id]             INT             IDENTITY (1, 1) NOT NULL,
    [Body]           NVARCHAR (1024) NOT NULL,
    [CreateTime]     DATETIME        NOT NULL,
    [SupervisorName] NVARCHAR (50)   NOT NULL,
    CONSTRAINT [PK_BvMessages] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY]
);


GO
PRINT N'Creating [dbo].[BvMembership]...';


GO
CREATE TABLE [dbo].[BvMembership] (
    [ContainerSID] INT NOT NULL,
    [ObjectSID]    INT NOT NULL,
    [id]           INT IDENTITY (1, 1) NOT NULL
);


GO
PRINT N'Creating [dbo].[BvMembership].[IX_BvMembership_ObjectSID]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvMembership_ObjectSID]
    ON [dbo].[BvMembership]([ObjectSID] ASC) WITH (ALLOW_PAGE_LOCKS = OFF);


GO
PRINT N'Creating [dbo].[BvMembership].[IX_BvMembership_ContainerSID]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvMembership_ContainerSID]
    ON [dbo].[BvMembership]([ContainerSID] ASC) WITH (ALLOW_PAGE_LOCKS = OFF);


GO
PRINT N'Creating [dbo].[BvLoginGroup]...';


GO
CREATE TABLE [dbo].[BvLoginGroup] (
    [PersonSID] INT NOT NULL,
    [ObjectSID] INT NOT NULL,
    [SurveySID] INT NOT NULL
);


GO
PRINT N'Creating [dbo].[BvLoginGroup].[IX_BvLoginGroup]...';


GO
CREATE UNIQUE CLUSTERED INDEX [IX_BvLoginGroup]
    ON [dbo].[BvLoginGroup]([PersonSID] ASC, [ObjectSID] ASC, [SurveySID] ASC) WITH (IGNORE_DUP_KEY = ON);


GO
PRINT N'Creating [dbo].[BvInterview]...';


GO
CREATE TABLE [dbo].[BvInterview] (
    [ID]                INT            NOT NULL,
    [SurveySID]         INT            NOT NULL,
    [TelephoneNumber]   VARCHAR (255)  NULL,
    [RespondentName]    NVARCHAR (255) NULL,
    [TimezoneID]        INT            NULL,
    [TransientState]    INT            NULL,
    [LastCallTime]      DATETIME       NULL,
    [LastCallPersonSID] INT            NULL,
    [Duration]          INT            NULL,
    [ExtensionNumber]   VARCHAR (255)  NULL,
    [ConfirmitSid]      VARCHAR (64)   NOT NULL,
    [BatchID]           INT            NOT NULL,
    [LastChannelID]     TINYINT        NOT NULL,
    [DialingMode]       TINYINT        NOT NULL,
    [DialerId]          INT            NOT NULL,
    CONSTRAINT [BvPk_int] PRIMARY KEY CLUSTERED ([ID] ASC, [SurveySID] ASC)
);


GO
PRINT N'Creating [dbo].[BvInterview].[BvIx_int_State]...';


GO
CREATE NONCLUSTERED INDEX [BvIx_int_State]
    ON [dbo].[BvInterview]([SurveySID] ASC, [TransientState] ASC);


GO
PRINT N'Creating [dbo].[BvInterview].[BvIx_int_Batch]...';


GO
CREATE NONCLUSTERED INDEX [BvIx_int_Batch]
    ON [dbo].[BvInterview]([BatchID] ASC);


GO
PRINT N'Creating [dbo].[BvHistory]...';


GO
CREATE TABLE [dbo].[BvHistory] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [SurveyId]          INT            NOT NULL,
    [TelephoneNumber]   NVARCHAR (255) NULL,
    [FiredTime]         DATETIME       NOT NULL,
    [InterviewId]       INT            NULL,
    [ITS]               TINYINT        NULL,
    [AppointmentID]     INT            NULL,
    [WaitingTime]       INT            NULL,
    [ConfirmitDuration] INT            NULL,
    [Duration]          INT            NULL,
    [BatchId]           INT            NULL,
    [PersonSID]         INT            NULL,
    [RoleID]            TINYINT        NULL
);


GO
PRINT N'Creating [dbo].[BvHistory].[IX_History_Main]...';


GO
CREATE CLUSTERED INDEX [IX_History_Main]
    ON [dbo].[BvHistory]([SurveyId] ASC, [RoleID] ASC, [FiredTime] ASC, [ITS] ASC)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[BvHistory].[IX_BvHistory_InterviewerPerformance]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvHistory_InterviewerPerformance]
    ON [dbo].[BvHistory]([FiredTime] ASC, [RoleID] ASC, [PersonSID] ASC, [ITS] ASC)
    INCLUDE([WaitingTime], [ConfirmitDuration], [Duration])
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[BvFilters]...';


GO
CREATE TABLE [dbo].[BvFilters] (
    [SID]           INT            NOT NULL,
    [Name]          NVARCHAR (255) NOT NULL,
    [Description]   NVARCHAR (255) NOT NULL,
    [AndOrOperator] TINYINT        NOT NULL,
    [SurveySID]     INT            NOT NULL,
    [Hidden]        TINYINT        NOT NULL,
    CONSTRAINT [PK_BvFilters] PRIMARY KEY CLUSTERED ([SID] ASC),
    CONSTRAINT [IX_BvFilters] UNIQUE NONCLUSTERED ([Name] ASC)
);


GO
PRINT N'Creating [dbo].[BvFilterFields]...';


GO
CREATE TABLE [dbo].[BvFilterFields] (
    [ID]         INT            IDENTITY (1, 1) NOT NULL,
    [FilterSID]  INT            NOT NULL,
    [Table]      INT            NOT NULL,
    [Column]     NVARCHAR (255) NOT NULL,
    [Type]       INT            NOT NULL,
    [Sign]       INT            NOT NULL,
    [Value]      NVARCHAR (255) NOT NULL,
    [IsNeedCast] BIT            NOT NULL,
    CONSTRAINT [PK_BvFilterFields] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating [dbo].[BvFilterFields].[IX_BvFilterFieldsFilter]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvFilterFieldsFilter]
    ON [dbo].[BvFilterFields]([FilterSID] ASC);


GO
PRINT N'Creating [dbo].[BvConfirmitStatus]...';


GO
CREATE TABLE [dbo].[BvConfirmitStatus] (
    [StatusCode_Cnf]   NVARCHAR (256) NULL,
    [StatusName_Cnf]   NVARCHAR (256) NOT NULL,
    [StatusCode_BvFEE] INT            NOT NULL
);


GO
PRINT N'Creating [dbo].[BvConfirmitStatus].[IX_BvConfirmitStatus]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvConfirmitStatus]
    ON [dbo].[BvConfirmitStatus]([StatusCode_Cnf] ASC)
    INCLUDE([StatusCode_BvFEE]);


GO
PRINT N'Creating [dbo].[BvCallExpired]...';


GO
CREATE TABLE [dbo].[BvCallExpired] (
    [surveyID]    INT NULL,
    [interviewID] INT NULL,
    [CallState]   INT NULL
);


GO
PRINT N'Creating [dbo].[BvCachedCallsInsert]...';


GO
CREATE TABLE [dbo].[BvCachedCallsInsert] (
    [InterviewID] INT NOT NULL,
    [SurveySID]   INT NOT NULL,
    CONSTRAINT [pk_BvCachedCallsInsert] PRIMARY KEY CLUSTERED ([InterviewID] ASC, [SurveySID] ASC) WITH (IGNORE_DUP_KEY = ON)
);


GO
PRINT N'Creating [dbo].[BvCachedCalls]...';


GO
CREATE TABLE [dbo].[BvCachedCalls] (
    [ID]          INT      NOT NULL,
    [ExplicitSID] INT      NOT NULL,
    [SurveySID]   INT      NOT NULL,
    [InterviewID] INT      NOT NULL,
    [CallState]   INT      NOT NULL,
    [TimeInShift] DATETIME NULL,
    [OrderId]     INT      NOT NULL
);


GO
PRINT N'Creating [dbo].[BvCachedCalls].[IX_BvCachedCalls]...';


GO
CREATE UNIQUE CLUSTERED INDEX [IX_BvCachedCalls]
    ON [dbo].[BvCachedCalls]([OrderId] ASC);


GO
PRINT N'Creating [dbo].[BvCachedCalls].[IX_BvCachedCalls_2]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_BvCachedCalls_2]
    ON [dbo].[BvCachedCalls]([SurveySID] ASC, [ExplicitSID] ASC, [OrderId] DESC, [InterviewID] ASC);


GO
PRINT N'Creating [dbo].[BvCachedCalls].[IX_BvCachedCalls_1]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_BvCachedCalls_1]
    ON [dbo].[BvCachedCalls]([SurveySID] ASC, [InterviewID] ASC);


GO
PRINT N'Creating [dbo].[BvBackendInstance]...';


GO
CREATE TABLE [dbo].[BvBackendInstance] (
    [ServiceName] NVARCHAR (64) NOT NULL,
    CONSTRAINT [PK_BvBackendInstance] PRIMARY KEY NONCLUSTERED ([ServiceName] ASC)
);


GO
PRINT N'Creating [dbo].[BvPersonOrGroupAssignmentOnSurvey]...';


GO
CREATE TABLE [dbo].[BvPersonOrGroupAssignmentOnSurvey] (
    [Id]              INT IDENTITY (1, 1) NOT NULL,
    [PersonOrGroupId] INT NOT NULL,
    [SurveyId]        INT NOT NULL,
    CONSTRAINT [PK_PersonOrGroupAssignmentOnSurvey] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[BvAppointmentsAlertStatus]...';


GO
CREATE TABLE [dbo].[BvAppointmentsAlertStatus] (
    [ID]              INT            NOT NULL,
    [SurveySID]       INT            NOT NULL,
    [SurveyName]      NVARCHAR (255) NOT NULL,
    [ProjectID]       NVARCHAR (255) NOT NULL,
    [InterviewID]     INT            NOT NULL,
    [AppointmentTime] DATETIME       NOT NULL,
    [TZID]            INT            NULL,
    [Resource]        NVARCHAR (255) NULL,
    [Contact]         NVARCHAR (255) NOT NULL,
    [AlertStatus]     INT            NOT NULL,
    [CallID]          INT            NOT NULL
);


GO
PRINT N'Creating [dbo].[BvAppointmentsAlertStatus].[IX_BvAppointmentsAlertStatus]...';


GO
CREATE CLUSTERED INDEX [IX_BvAppointmentsAlertStatus]
    ON [dbo].[BvAppointmentsAlertStatus]([SurveySID] ASC, [AlertStatus] DESC, [AppointmentTime] DESC, [ID] ASC);


GO
PRINT N'Creating [dbo].[BvAppointmentCounters]...';


GO
CREATE TABLE [dbo].[BvAppointmentCounters] (
    [SurveySID]             INT            NOT NULL,
    [ProjectID]             NVARCHAR (255) NOT NULL,
    [SurveyName]            NVARCHAR (255) NOT NULL,
    [CountForShortInterval] INT            NOT NULL,
    [CountForLongInterval]  INT            NOT NULL,
    CONSTRAINT [PkBvAppointmentCounters] PRIMARY KEY CLUSTERED ([SurveySID] ASC)
);


GO
PRINT N'Creating [dbo].[BvAppointment]...';


GO
CREATE TABLE [dbo].[BvAppointment] (
    [SurveySID]      INT            NOT NULL,
    [InterviewSID]   INT            NOT NULL,
    [Time]           DATETIME       NOT NULL,
    [ExpTime]        DATETIME       NULL,
    [RespondentName] NVARCHAR (255) NULL,
    [ID]             INT            IDENTITY (1, 1) NOT NULL,
    [State]          INT            NOT NULL,
    [ContactName]    NVARCHAR (255) NOT NULL,
    [BatchID]        INT            NOT NULL,
    [TempID]         INT            NOT NULL,
    [TZID]           INT            NULL,
    CONSTRAINT [Pk_app] PRIMARY KEY CLUSTERED ([ID] ASC, [SurveySID] ASC),
    CONSTRAINT [UQ_BvAppointment_Id] UNIQUE NONCLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating [dbo].[BvAppointment].[IX_app_SurveySID_InterviewSID_State]...';


GO
CREATE NONCLUSTERED INDEX [IX_app_SurveySID_InterviewSID_State]
    ON [dbo].[BvAppointment]([SurveySID] ASC, [InterviewSID] ASC, [State] ASC);


GO
PRINT N'Creating [dbo].[BvAppointment].[IX_app_State]...';


GO
CREATE NONCLUSTERED INDEX [IX_app_State]
    ON [dbo].[BvAppointment]([State] ASC);


GO
PRINT N'Creating [dbo].[BvAppointment].[IX_app_BatchID]...';


GO
CREATE NONCLUSTERED INDEX [IX_app_BatchID]
    ON [dbo].[BvAppointment]([BatchID] ASC);


GO
PRINT N'Creating [dbo].[BvAppointment].[IX_app_alert]...';


GO
CREATE NONCLUSTERED INDEX [IX_app_alert]
    ON [dbo].[BvAppointment]([State] ASC, [Time] DESC);


GO
PRINT N'Creating [dbo].[BvAggregateSurveyAlertStatus]...';


GO
CREATE TABLE [dbo].[BvAggregateSurveyAlertStatus] (
    [SID]                                      INT            NOT NULL,
    [Name]                                     NVARCHAR (255) NOT NULL,
    [Description]                              NVARCHAR (255) NOT NULL,
    [InterviewersLoggedCount]                  INT            NOT NULL,
    [InterviewersLoggedCountPrev]              INT            NOT NULL,
    [NextAppointmentTime]                      DATETIME       NULL,
    [TotalSampleSize]                          INT            NOT NULL,
    [ActiveCallsCount]                         INT            NOT NULL,
    [ActiveCallsCountPrev]                     INT            NOT NULL,
    [ScheduledCallsCount]                      INT            NOT NULL,
    [ScheduledCallsCountPrev]                  INT            NOT NULL,
    [SuspendedCallsCount]                      INT            NOT NULL,
    [SuspendedCallsCountPrev]                  INT            NOT NULL,
    [MinutesSpentWorkingOnSurvey]              INT            NOT NULL,
    [AssignedInterviewersCount]                INT            NOT NULL,
    [StrikeRate]                               INT            NOT NULL,
    [CountCalls]                               INT            NOT NULL,
    [AvgDuration]                              INT            NOT NULL,
    [AlertStatusOfInterviewersLoggedCount]     INT            NOT NULL,
    [AlertStatusOfNextAppointmentTime]         INT            NOT NULL,
    [AlertStatusOfTotalSampleSize]             INT            NOT NULL,
    [AlertStatusOfActiveCallsCount]            INT            NOT NULL,
    [AlertStatusOfScheduledCallsCount]         INT            NOT NULL,
    [AlertStatusOfSuspendedCallsCount]         INT            NOT NULL,
    [AlertStatusOfMinutesSpentWorkingOnSurvey] INT            NOT NULL,
    [AlertStatusOfAssignedInterviewersCount]   INT            NOT NULL,
    [AlertStatusOfStrikeRate]                  INT            NOT NULL,
    [AlertStatusOfCountCalls]                  INT            NOT NULL,
    [MaxStatusOfITSAlerts]                     INT            NOT NULL,
    CONSTRAINT [PkBvAggregateSurveyAlertStatus] PRIMARY KEY CLUSTERED ([SID] ASC)
);


GO
PRINT N'Creating [dbo].[BvAggregateSurvey]...';


GO
CREATE TABLE [dbo].[BvAggregateSurvey] (
    [SID]                         INT NOT NULL,
    [ScheduledCallsCount]         INT NOT NULL,
    [SuspendedCallsCount]         INT NOT NULL,
    [MinutesSpentWorkingOnSurvey] INT NOT NULL,
    CONSTRAINT [PkBvAggregateSurvey] PRIMARY KEY CLUSTERED ([SID] ASC)
);


GO
PRINT N'Creating [dbo].[BvActiveShiftTypeZone]...';


GO
CREATE TABLE [dbo].[BvActiveShiftTypeZone] (
    [Id]       INT NOT NULL,
    [SurveyId] INT NOT NULL,
    CONSTRAINT [PK_BvActiveShiftTypeZone] PRIMARY KEY CLUSTERED ([Id] ASC, [SurveyId] ASC)
);


GO
PRINT N'Creating DF_BvSearchableFields_UseMode...';


GO
ALTER TABLE [dbo].[BvSearchableFields]
    ADD CONSTRAINT [DF_BvSearchableFields_UseMode] DEFAULT (0) FOR [UseMode];


GO
PRINT N'Creating DF_BvQuotaBalancing_priority...';


GO
ALTER TABLE [dbo].[BvQuotaBalancing]
    ADD CONSTRAINT [DF_BvQuotaBalancing_priority] DEFAULT (500) FOR [priority];


GO
PRINT N'Creating DF_BvQuotaBalancing_promotionCoefficient...';


GO
ALTER TABLE [dbo].[BvQuotaBalancing]
    ADD CONSTRAINT [DF_BvQuotaBalancing_promotionCoefficient] DEFAULT (0.8) FOR [promotionCoefficient];


GO
PRINT N'Creating DF_BvDialers_Name...';


GO
ALTER TABLE [dbo].[BvDialers]
    ADD CONSTRAINT [DF_BvDialers_Name] DEFAULT ('') FOR [Name];


GO
PRINT N'Creating DF_BvDialers_TenantId...';


GO
ALTER TABLE [dbo].[BvDialers]
    ADD CONSTRAINT [DF_BvDialers_TenantId] DEFAULT (0) FOR [TenantId];


GO
PRINT N'Creating DF_BvDialers_DialerOperationalStateNotification...';


GO
ALTER TABLE [dbo].[BvDialers]
    ADD CONSTRAINT [DF_BvDialers_DialerOperationalStateNotification] DEFAULT (0) FOR [DialerOperationalStateNotification];


GO
PRINT N'Creating DF_BvThresholdITS_Amber...';


GO
ALTER TABLE [dbo].[BvThresholdITS]
    ADD CONSTRAINT [DF_BvThresholdITS_Amber] DEFAULT (2147483647) FOR [Amber];


GO
PRINT N'Creating DF_BvThresholdITS_Red...';


GO
ALTER TABLE [dbo].[BvThresholdITS]
    ADD CONSTRAINT [DF_BvThresholdITS_Red] DEFAULT (2147483647) FOR [Red];


GO
PRINT N'Creating DF_BvTasks_SecondsSinceLastSubmission...';


GO
ALTER TABLE [dbo].[BvTasks]
    ADD CONSTRAINT [DF_BvTasks_SecondsSinceLastSubmission] DEFAULT (0) FOR [SecondsSinceLastSubmission];


GO
PRINT N'Creating DF_BvTasks_LastSubmissionAlert...';


GO
ALTER TABLE [dbo].[BvTasks]
    ADD CONSTRAINT [DF_BvTasks_LastSubmissionAlert] DEFAULT (0) FOR [LastSubmissionAlert];


GO
PRINT N'Creating DF_BvTasks_TzID...';


GO
ALTER TABLE [dbo].[BvTasks]
    ADD CONSTRAINT [DF_BvTasks_TzID] DEFAULT (0) FOR [TzID];


GO
PRINT N'Creating DF_BvTasks_DiallingMode...';


GO
ALTER TABLE [dbo].[BvTasks]
    ADD CONSTRAINT [DF_BvTasks_DiallingMode] DEFAULT (0) FOR [DiallingMode];


GO
PRINT N'Creating DF_BvTasks_CallOutcome...';


GO
ALTER TABLE [dbo].[BvTasks]
    ADD CONSTRAINT [DF_BvTasks_CallOutcome] DEFAULT (-1) FOR [CallOutcome];


GO
PRINT N'Creating DF_BvTasks_InterviewState...';


GO
ALTER TABLE [dbo].[BvTasks]
    ADD CONSTRAINT [DF_BvTasks_InterviewState] DEFAULT (0) FOR [InterviewState];


GO
PRINT N'Creating DF_BvTasks_StatusLogout...';


GO
ALTER TABLE [dbo].[BvTasks]
    ADD CONSTRAINT [DF_BvTasks_StatusLogout] DEFAULT (0) FOR [StatusLogout];


GO
PRINT N'Creating DF_BvTasks_LoggedInToDialerState...';


GO
ALTER TABLE [dbo].[BvTasks]
    ADD CONSTRAINT [DF_BvTasks_LoggedInToDialerState] DEFAULT (0) FOR [LoggedInToDialerState];


GO
PRINT N'Creating DF_BvTasks_IsLoginRCToDialer...';


GO
ALTER TABLE [dbo].[BvTasks]
    ADD CONSTRAINT [DF_BvTasks_IsLoginRCToDialer] DEFAULT (0) FOR [IsLoginRCToDialer];


GO
PRINT N'Creating DF_BvTasks_LastKeepAliveTimeAlert...';


GO
ALTER TABLE [dbo].[BvTasks]
    ADD CONSTRAINT [DF_BvTasks_LastKeepAliveTimeAlert] DEFAULT (0) FOR [LastKeepAliveTimeAlert];


GO
PRINT N'Creating DF_BvTasks_ProblemId...';


GO
ALTER TABLE [dbo].[BvTasks]
    ADD CONSTRAINT [DF_BvTasks_ProblemId] DEFAULT (0) FOR [ProblemId];


GO
PRINT N'Creating DF_BvTasks_StationId...';


GO
ALTER TABLE [dbo].[BvTasks]
    ADD CONSTRAINT [DF_BvTasks_StationId] DEFAULT ('') FOR [StationId];


GO
PRINT N'Creating DF_BvSvySchedule_ExpireTime...';


GO
ALTER TABLE [dbo].[BvSvySchedule]
    ADD CONSTRAINT [DF_BvSvySchedule_ExpireTime] DEFAULT ('9999-01-01 00:00:00.000') FOR [ExpireTime];


GO
PRINT N'Creating DF_BvSvySchedule_RuleNumber...';


GO
ALTER TABLE [dbo].[BvSvySchedule]
    ADD CONSTRAINT [DF_BvSvySchedule_RuleNumber] DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [RuleNumber];


GO
PRINT N'Creating DF_BvSvySchedule_IsInActiveShiftType...';


GO
ALTER TABLE [dbo].[BvSvySchedule]
    ADD CONSTRAINT [DF_BvSvySchedule_IsInActiveShiftType] DEFAULT (0) FOR [IsInActiveShiftType];


GO
PRINT N'Creating DF_BvSvySchedule_CallOrder...';


GO
ALTER TABLE [dbo].[BvSvySchedule]
    ADD CONSTRAINT [DF_BvSvySchedule_CallOrder] DEFAULT (0) FOR [CallOrder];


GO
PRINT N'Creating DF_BvSvySchedule_OldPriority...';


GO
ALTER TABLE [dbo].[BvSvySchedule]
    ADD CONSTRAINT [DF_BvSvySchedule_OldPriority] DEFAULT (0) FOR [OldPriority];


GO
PRINT N'Creating DF_BvSvySchedule_ConditionValue...';


GO
ALTER TABLE [dbo].[BvSvySchedule]
    ADD CONSTRAINT [DF_BvSvySchedule_ConditionValue] DEFAULT (0) FOR [ConditionValue];


GO
PRINT N'Creating DF_BvSurveyListAlertsViewConfiguration_UpdatingTime...';


GO
ALTER TABLE [dbo].[BvSurveyListAlertsViewConfiguration]
    ADD CONSTRAINT [DF_BvSurveyListAlertsViewConfiguration_UpdatingTime] DEFAULT (15) FOR [UpdatingTime];


GO
PRINT N'Creating DF_BvSurveyListAlertsViewConfiguration_SyncUpdatingTime...';


GO
ALTER TABLE [dbo].[BvSurveyListAlertsViewConfiguration]
    ADD CONSTRAINT [DF_BvSurveyListAlertsViewConfiguration_SyncUpdatingTime] DEFAULT (3600) FOR [SyncUpdatingTime];


GO
PRINT N'Creating DF_BvSurveyListAlertsViewConfiguration_IdlePeriodMaxCountOfChecks...';


GO
ALTER TABLE [dbo].[BvSurveyListAlertsViewConfiguration]
    ADD CONSTRAINT [DF_BvSurveyListAlertsViewConfiguration_IdlePeriodMaxCountOfChecks] DEFAULT (60) FOR [IdlePeriodMaxCountOfChecks];


GO
PRINT N'Creating DF_BvSurveyListAlertsViewConfiguration_IdlePeriodCheckCounter...';


GO
ALTER TABLE [dbo].[BvSurveyListAlertsViewConfiguration]
    ADD CONSTRAINT [DF_BvSurveyListAlertsViewConfiguration_IdlePeriodCheckCounter] DEFAULT (0) FOR [IdlePeriodCheckCounter];


GO
PRINT N'Creating DF_BvSurveyListAlertsViewConfiguration_IdlePeriodMaxSeconds...';


GO
ALTER TABLE [dbo].[BvSurveyListAlertsViewConfiguration]
    ADD CONSTRAINT [DF_BvSurveyListAlertsViewConfiguration_IdlePeriodMaxSeconds] DEFAULT (3600) FOR [IdlePeriodMaxSeconds];


GO
PRINT N'Creating DF__BvSurvey__State__4734D02B...';


GO
ALTER TABLE [dbo].[BvSurvey]
    ADD CONSTRAINT [DF__BvSurvey__State__4734D02B] DEFAULT ((0)) FOR [State];


GO
PRINT N'Creating DF__BvSurvey__Number__3ACEF946...';


GO
ALTER TABLE [dbo].[BvSurvey]
    ADD CONSTRAINT [DF__BvSurvey__Number__3ACEF946] DEFAULT ((0)) FOR [Number];


GO
PRINT N'Creating DfBvSurvey_QuotaType...';


GO
ALTER TABLE [dbo].[BvSurvey]
    ADD CONSTRAINT [DfBvSurvey_QuotaType] DEFAULT ((0)) FOR [QuotaType];


GO
PRINT N'Creating DfBvSurvey_Name...';


GO
ALTER TABLE [dbo].[BvSurvey]
    ADD CONSTRAINT [DfBvSurvey_Name] DEFAULT (' ') FOR [Name];


GO
PRINT N'Creating DfBvSurvey_Description...';


GO
ALTER TABLE [dbo].[BvSurvey]
    ADD CONSTRAINT [DfBvSurvey_Description] DEFAULT (' ') FOR [Description];


GO
PRINT N'Creating DF_BvSurvey_ForceOpnRev...';


GO
ALTER TABLE [dbo].[BvSurvey]
    ADD CONSTRAINT [DF_BvSurvey_ForceOpnRev] DEFAULT (0) FOR [ForceOpnRev];


GO
PRINT N'Creating DF_BvSurvey_StateGroupID...';


GO
ALTER TABLE [dbo].[BvSurvey]
    ADD CONSTRAINT [DF_BvSurvey_StateGroupID] DEFAULT (0) FOR [StateGroupID];


GO
PRINT N'Creating DF_BvSurvey_RecWholeInt...';


GO
ALTER TABLE [dbo].[BvSurvey]
    ADD CONSTRAINT [DF_BvSurvey_RecWholeInt] DEFAULT (0) FOR [RecWholeInt];


GO
PRINT N'Creating DF_BvSurvey_InterviewScreenRecording...';


GO
ALTER TABLE [dbo].[BvSurvey]
    ADD CONSTRAINT [DF_BvSurvey_InterviewScreenRecording] DEFAULT (0) FOR [InterviewScreenRecording];


GO
PRINT N'Creating DF_BvSurvey_CfDbSchemaPath...';


GO
ALTER TABLE [dbo].[BvSurvey]
    ADD CONSTRAINT [DF_BvSurvey_CfDbSchemaPath] DEFAULT (N'') FOR [CfDbSchemaPath];


GO
PRINT N'Creating DF_BvSurvey_IsTelephoneBlacklistSupported...';


GO
ALTER TABLE [dbo].[BvSurvey]
    ADD CONSTRAINT [DF_BvSurvey_IsTelephoneBlacklistSupported] DEFAULT (0) FOR [IsTelephoneBlacklistSupported];


GO
PRINT N'Creating DF_BvSurvey_IsRandomCallDeliveryEnabled...';


GO
ALTER TABLE [dbo].[BvSurvey]
    ADD CONSTRAINT [DF_BvSurvey_IsRandomCallDeliveryEnabled] DEFAULT (0) FOR [IsRandomCallDeliveryEnabled];


GO
PRINT N'Creating DF_BvSurvey_EnforceHttps...';


GO
ALTER TABLE [dbo].[BvSurvey]
    ADD CONSTRAINT [DF_BvSurvey_EnforceHttps] DEFAULT (0) FOR [EnforceHttps];


GO
PRINT N'Creating DF_BvSurvey_LastTouchTime...';


GO
ALTER TABLE [dbo].[BvSurvey]
    ADD CONSTRAINT [DF_BvSurvey_LastTouchTime] DEFAULT (GETUTCDATE()) FOR [LastTouchTime];


GO
PRINT N'Creating DF_BvSurvey_SurveySchedulingMode...';


GO
ALTER TABLE [dbo].[BvSurvey]
    ADD CONSTRAINT [DF_BvSurvey_SurveySchedulingMode] DEFAULT (0) FOR [SurveySchedulingMode];


GO
PRINT N'Creating DF_BvState_Priority...';


GO
ALTER TABLE [dbo].[BvState]
    ADD CONSTRAINT [DF_BvState_Priority] DEFAULT (1) FOR [Priority];


GO
PRINT N'Creating DF_BvState_StateGroupID...';


GO
ALTER TABLE [dbo].[BvState]
    ADD CONSTRAINT [DF_BvState_StateGroupID] DEFAULT (0) FOR [StateGroupID];


GO
PRINT N'Creating DF_BvState_DA...';


GO
ALTER TABLE [dbo].[BvState]
    ADD CONSTRAINT [DF_BvState_DA] DEFAULT (0) FOR [DA];


GO
PRINT N'Creating DF_BvSchedule_XmlInUse...';


GO
ALTER TABLE [dbo].[BvSchedule]
    ADD CONSTRAINT [DF_BvSchedule_XmlInUse] DEFAULT (N'') FOR [XmlInUse];


GO
PRINT N'Creating DF_BvSchedule_XmlUnderDev...';


GO
ALTER TABLE [dbo].[BvSchedule]
    ADD CONSTRAINT [DF_BvSchedule_XmlUnderDev] DEFAULT (N'') FOR [XmlUnderDev];


GO
PRINT N'Creating DF_BvSchedule_RegenerateIsRequired...';


GO
ALTER TABLE [dbo].[BvSchedule]
    ADD CONSTRAINT [DF_BvSchedule_RegenerateIsRequired] DEFAULT (0) FOR [RegenerateIsRequired];


GO
PRINT N'Creating DF_BvSampleStatusSummary_Cnt...';


GO
ALTER TABLE [dbo].[BvSampleStatusSummary]
    ADD CONSTRAINT [DF_BvSampleStatusSummary_Cnt] DEFAULT (0) FOR [Cnt];


GO
PRINT N'Creating DF_BvSampleStatusSummary_AlertStatus...';


GO
ALTER TABLE [dbo].[BvSampleStatusSummary]
    ADD CONSTRAINT [DF_BvSampleStatusSummary_AlertStatus] DEFAULT (0) FOR [AlertStatus];


GO
PRINT N'Creating DF__BvPersonG__RoleI__27BC24D2...';


GO
ALTER TABLE [dbo].[BvPersonGroup]
    ADD CONSTRAINT [DF__BvPersonG__RoleI__27BC24D2] DEFAULT ((0)) FOR [RoleID];


GO
PRINT N'Creating DF_BvPersonGroup_Manual_SELECTion...';


GO
ALTER TABLE [dbo].[BvPersonGroup]
    ADD CONSTRAINT [DF_BvPersonGroup_Manual_SELECTion] DEFAULT ((0)) FOR [ManualSelection];


GO
PRINT N'Creating DF_BvPersonDeferredMonitoring_IsRecording...';


GO
ALTER TABLE [dbo].[BvPersonDeferredMonitoring]
    ADD CONSTRAINT [DF_BvPersonDeferredMonitoring_IsRecording] DEFAULT ((1)) FOR [IsRecording];


GO
PRINT N'Creating DF_BvPersonDeferredMonitoring_IsComplete...';


GO
ALTER TABLE [dbo].[BvPersonDeferredMonitoring]
    ADD CONSTRAINT [DF_BvPersonDeferredMonitoring_IsComplete] DEFAULT ((0)) FOR [IsComplete];


GO
PRINT N'Creating DF_BvPersonDeferredMonitoring_HasAudio...';


GO
ALTER TABLE [dbo].[BvPersonDeferredMonitoring]
    ADD CONSTRAINT [DF_BvPersonDeferredMonitoring_HasAudio] DEFAULT ((0)) FOR [HasAudio];


GO
PRINT N'Creating DF_BvPersonDeferredMonitoring_ClientTimeUtc...';


GO
ALTER TABLE [dbo].[BvPersonDeferredMonitoring]
    ADD CONSTRAINT [DF_BvPersonDeferredMonitoring_ClientTimeUtc] DEFAULT GETUTCDATE() FOR [ClientTimeUtc];


GO
PRINT N'Creating DF_BvPersonDeferredMonitoring_ServerTimeUtc...';


GO
ALTER TABLE [dbo].[BvPersonDeferredMonitoring]
    ADD CONSTRAINT [DF_BvPersonDeferredMonitoring_ServerTimeUtc] DEFAULT GETUTCDATE() FOR [ServerTimeUtc];


GO
PRINT N'Creating DF_BvPersonDeferredMonitoring_RequestAudio...';


GO
ALTER TABLE [dbo].[BvPersonDeferredMonitoring]
    ADD CONSTRAINT [DF_BvPersonDeferredMonitoring_RequestAudio] DEFAULT 0 FOR [RequestAudio];


GO
PRINT N'Creating DF_BvPerson_AssignmentsListMode...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD CONSTRAINT [DF_BvPerson_AssignmentsListMode] DEFAULT 0 FOR [AssignmentsListMode];


GO
PRINT N'Creating Df_BvPerson_Name...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD CONSTRAINT [Df_BvPerson_Name] DEFAULT (' ') FOR [Name];


GO
PRINT N'Creating Df_BvPerson_FullName...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD CONSTRAINT [Df_BvPerson_FullName] DEFAULT (' ') FOR [FullName];


GO
PRINT N'Creating Df_BvPerson_Description...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD CONSTRAINT [Df_BvPerson_Description] DEFAULT (' ') FOR [Description];


GO
PRINT N'Creating DfBvPerson_IS...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD CONSTRAINT [DfBvPerson_IS] DEFAULT ((0)) FOR [ManualSelection];


GO
PRINT N'Creating DF_BvPerson_TotalSampleSize_TimezoneID...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD CONSTRAINT [DF_BvPerson_TotalSampleSize_TimezoneID] DEFAULT (NULL) FOR [TimezoneID];


GO
PRINT N'Creating DF_BvPerson_TotalSampleSize_PwdHashTxt...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD CONSTRAINT [DF_BvPerson_TotalSampleSize_PwdHashTxt] DEFAULT ('') FOR [PwdHashTxt];


GO
PRINT N'Creating DF_BvPerson_TotalSampleSize_PwdSaltTxt...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD CONSTRAINT [DF_BvPerson_TotalSampleSize_PwdSaltTxt] DEFAULT ('') FOR [PwdSaltTxt];


GO
PRINT N'Creating DF_BvPerson_TotalSampleSize_DialerId...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD CONSTRAINT [DF_BvPerson_TotalSampleSize_DialerId] DEFAULT (0) FOR [DialerId];


GO
PRINT N'Creating DF_BvPerson_TotalSampleSize_ExtensionNumber...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD CONSTRAINT [DF_BvPerson_TotalSampleSize_ExtensionNumber] DEFAULT ('') FOR [ExtensionNumber];


GO
PRINT N'Creating DF_BvPerson_TotalSampleSize_MNDiallerUserId...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD CONSTRAINT [DF_BvPerson_TotalSampleSize_MNDiallerUserId] DEFAULT ('') FOR [MNDiallerUserId];


GO
PRINT N'Creating DF_BvPerson_TotalSampleSize_DialerConnection...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD CONSTRAINT [DF_BvPerson_TotalSampleSize_DialerConnection] DEFAULT ('') FOR [DialerConnection];


GO
PRINT N'Creating DF_BvPerson_TotalSampleSize_DeskStationName...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD CONSTRAINT [DF_BvPerson_TotalSampleSize_DeskStationName] DEFAULT ('') FOR [DeskStationName];


GO
PRINT N'Creating DF_BvPerson_TotalSampleSize_HasNewMessage...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD CONSTRAINT [DF_BvPerson_TotalSampleSize_HasNewMessage] DEFAULT (NULL) FOR [HasNewMessage];


GO
PRINT N'Creating DF_BvPerson_TotalSampleSize_AllowedChoices...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD CONSTRAINT [DF_BvPerson_TotalSampleSize_AllowedChoices] DEFAULT (NULL) FOR [AllowedChoices];


GO
PRINT N'Creating DF_BvPerson_TotalSampleSize_StationExtensionNumber...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD CONSTRAINT [DF_BvPerson_TotalSampleSize_StationExtensionNumber] DEFAULT ('') FOR [StationExtensionNumber];


GO
PRINT N'Creating DF_BvPerson_TotalSampleSize_IsDialerAgentLocal...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD CONSTRAINT [DF_BvPerson_TotalSampleSize_IsDialerAgentLocal] DEFAULT (0) FOR [IsDialerAgentLocal];


GO
PRINT N'Creating DF_BvPerson_TotalSampleSize_FailedLoginAttempts...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD CONSTRAINT [DF_BvPerson_TotalSampleSize_FailedLoginAttempts] DEFAULT (0) FOR [FailedLoginAttempts];


GO
PRINT N'Creating DF_BvPerson_TotalSampleSize_IsLocked...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD CONSTRAINT [DF_BvPerson_TotalSampleSize_IsLocked] DEFAULT (0) FOR [IsLocked];


GO
PRINT N'Creating DF_BvPerson_TotalSampleSize_Location...';


GO
ALTER TABLE [dbo].[BvPerson]
    ADD CONSTRAINT [DF_BvPerson_TotalSampleSize_Location] DEFAULT (NULL) FOR [Location];


GO
PRINT N'Creating DF_BvInterview_ConfirmitSid...';


GO
ALTER TABLE [dbo].[BvInterview]
    ADD CONSTRAINT [DF_BvInterview_ConfirmitSid] DEFAULT ('') FOR [ConfirmitSid];


GO
PRINT N'Creating DF_BvInterview_DialingMode...';


GO
ALTER TABLE [dbo].[BvInterview]
    ADD CONSTRAINT [DF_BvInterview_DialingMode] DEFAULT (0) FOR [DialingMode];


GO
PRINT N'Creating DF_BvInterview_DialerId...';


GO
ALTER TABLE [dbo].[BvInterview]
    ADD CONSTRAINT [DF_BvInterview_DialerId] DEFAULT (0) FOR [DialerId];


GO
PRINT N'Creating DF_BvFilterFields_IsNeedCast...';


GO
ALTER TABLE [dbo].[BvFilterFields]
    ADD CONSTRAINT [DF_BvFilterFields_IsNeedCast] DEFAULT (0) FOR [IsNeedCast];


GO
PRINT N'Creating DF_BvCachedCalls_OrderId...';


GO
ALTER TABLE [dbo].[BvCachedCalls]
    ADD CONSTRAINT [DF_BvCachedCalls_OrderId] DEFAULT (0) FOR [OrderId];


GO
PRINT N'Creating DF_BvAppointmentsAlertStatus_AlertStatus...';


GO
ALTER TABLE [dbo].[BvAppointmentsAlertStatus]
    ADD CONSTRAINT [DF_BvAppointmentsAlertStatus_AlertStatus] DEFAULT (0) FOR [AlertStatus];


GO
PRINT N'Creating DF_BvAppointmentsAlertStatus_CallID...';


GO
ALTER TABLE [dbo].[BvAppointmentsAlertStatus]
    ADD CONSTRAINT [DF_BvAppointmentsAlertStatus_CallID] DEFAULT (0) FOR [CallID];


GO
PRINT N'Creating DF_BvAppointment_BatchID...';


GO
ALTER TABLE [dbo].[BvAppointment]
    ADD CONSTRAINT [DF_BvAppointment_BatchID] DEFAULT (0) FOR [BatchID];


GO
PRINT N'Creating DF_BvAppointment_TempID...';


GO
ALTER TABLE [dbo].[BvAppointment]
    ADD CONSTRAINT [DF_BvAppointment_TempID] DEFAULT (0) FOR [TempID];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_InterviewersLoggedCount...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_InterviewersLoggedCount] DEFAULT (0) FOR [InterviewersLoggedCount];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_InterviewersLoggedCountPrev...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_InterviewersLoggedCountPrev] DEFAULT (0) FOR [InterviewersLoggedCountPrev];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_TotalSampleSize...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_TotalSampleSize] DEFAULT (0) FOR [TotalSampleSize];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_ActiveCallsCount...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_ActiveCallsCount] DEFAULT (0) FOR [ActiveCallsCount];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_ActiveCallsCountPrev...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_ActiveCallsCountPrev] DEFAULT (0) FOR [ActiveCallsCountPrev];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_ScheduledCallsCount...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_ScheduledCallsCount] DEFAULT (0) FOR [ScheduledCallsCount];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_ScheduledCallsCountPrev...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_ScheduledCallsCountPrev] DEFAULT (0) FOR [ScheduledCallsCountPrev];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_SuspendedCallsCount...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_SuspendedCallsCount] DEFAULT (0) FOR [SuspendedCallsCount];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_SuspendedCallsCountPrev...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_SuspendedCallsCountPrev] DEFAULT (0) FOR [SuspendedCallsCountPrev];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_MinutesSpentWorkingOnSurvey...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_MinutesSpentWorkingOnSurvey] DEFAULT (0) FOR [MinutesSpentWorkingOnSurvey];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_AssignedInterviewersCount...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_AssignedInterviewersCount] DEFAULT (0) FOR [AssignedInterviewersCount];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_StrikeRate...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_StrikeRate] DEFAULT (0) FOR [StrikeRate];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_CountCalls...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_CountCalls] DEFAULT (0) FOR [CountCalls];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_AvgDuration...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_AvgDuration] DEFAULT (0) FOR [AvgDuration];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_AlertStatusOfInterviewersLoggedCount...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_AlertStatusOfInterviewersLoggedCount] DEFAULT (0) FOR [AlertStatusOfInterviewersLoggedCount];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_AlertStatusOfNextAppointmentTime...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_AlertStatusOfNextAppointmentTime] DEFAULT (0) FOR [AlertStatusOfNextAppointmentTime];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_AlertStatusOfTotalSampleSize...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_AlertStatusOfTotalSampleSize] DEFAULT (0) FOR [AlertStatusOfTotalSampleSize];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_AlertStatusOfActiveCallsCount...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_AlertStatusOfActiveCallsCount] DEFAULT (0) FOR [AlertStatusOfActiveCallsCount];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_AlertStatusOfScheduledCallsCount...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_AlertStatusOfScheduledCallsCount] DEFAULT (0) FOR [AlertStatusOfScheduledCallsCount];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_AlertStatusOfSuspendedCallsCount...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_AlertStatusOfSuspendedCallsCount] DEFAULT (0) FOR [AlertStatusOfSuspendedCallsCount];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_AlertStatusOfMinutesSpentWorkingOnSurvey...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_AlertStatusOfMinutesSpentWorkingOnSurvey] DEFAULT (0) FOR [AlertStatusOfMinutesSpentWorkingOnSurvey];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_AlertStatusOfAssignedInterviewersCount...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_AlertStatusOfAssignedInterviewersCount] DEFAULT (0) FOR [AlertStatusOfAssignedInterviewersCount];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_AlertStatusOfStrikeRate...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_AlertStatusOfStrikeRate] DEFAULT (0) FOR [AlertStatusOfStrikeRate];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_AlertStatusOfCountCalls...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_AlertStatusOfCountCalls] DEFAULT (0) FOR [AlertStatusOfCountCalls];


GO
PRINT N'Creating DF_BvAggregateSurveyAlertStatus_MaxStatusOfITSAlerts...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus]
    ADD CONSTRAINT [DF_BvAggregateSurveyAlertStatus_MaxStatusOfITSAlerts] DEFAULT (0) FOR [MaxStatusOfITSAlerts];


GO
PRINT N'Creating DF_BvAggregateSurvey_ScheduledCallsCount...';


GO
ALTER TABLE [dbo].[BvAggregateSurvey]
    ADD CONSTRAINT [DF_BvAggregateSurvey_ScheduledCallsCount] DEFAULT (0) FOR [ScheduledCallsCount];


GO
PRINT N'Creating DF_BvAggregateSurvey_SuspendedCallsCount...';


GO
ALTER TABLE [dbo].[BvAggregateSurvey]
    ADD CONSTRAINT [DF_BvAggregateSurvey_SuspendedCallsCount] DEFAULT (0) FOR [SuspendedCallsCount];


GO
PRINT N'Creating DF_BvAggregateSurvey_MinutesSpentWorkingOnSurvey...';


GO
ALTER TABLE [dbo].[BvAggregateSurvey]
    ADD CONSTRAINT [DF_BvAggregateSurvey_MinutesSpentWorkingOnSurvey] DEFAULT (0) FOR [MinutesSpentWorkingOnSurvey];


GO
PRINT N'Creating FK_BvQuotaFilter_surveyId...';


GO
ALTER TABLE [dbo].[BvQuotaFilter] WITH NOCHECK
    ADD CONSTRAINT [FK_BvQuotaFilter_surveyId] FOREIGN KEY ([surveyId]) REFERENCES [dbo].[BvSurvey] ([SID]) ON DELETE CASCADE;


GO
PRINT N'Creating FK_BvQuotaBalancing_surveyId...';


GO
ALTER TABLE [dbo].[BvQuotaBalancing] WITH NOCHECK
    ADD CONSTRAINT [FK_BvQuotaBalancing_surveyId] FOREIGN KEY ([surveyId]) REFERENCES [dbo].[BvSurvey] ([SID]) ON DELETE CASCADE;


GO
PRINT N'Creating ReferForeignField...';


GO
ALTER TABLE [dbo].[BvInterviewTimings] WITH NOCHECK
    ADD CONSTRAINT [ReferForeignField] FOREIGN KEY ([SurveyID]) REFERENCES [dbo].[BvSurvey] ([SID]) ON DELETE CASCADE;


GO
PRINT N'Creating FkBvUserSurveyPermission_Survey...';


GO
ALTER TABLE [dbo].[BvUserSurveyPermission] WITH NOCHECK
    ADD CONSTRAINT [FkBvUserSurveyPermission_Survey] FOREIGN KEY ([SurveySID]) REFERENCES [dbo].[BvSurvey] ([SID]) ON DELETE CASCADE;


GO
PRINT N'Creating FK_BvTimezoneShift_TimezoneID...';


GO
ALTER TABLE [dbo].[BvTimezoneShift] WITH NOCHECK
    ADD CONSTRAINT [FK_BvTimezoneShift_TimezoneID] FOREIGN KEY ([TimezoneID]) REFERENCES [dbo].[BvTimezone] ([ID]);


GO
PRINT N'Creating FkBvThresholds_ThresholdTypes...';


GO
ALTER TABLE [dbo].[BvThresholds] WITH NOCHECK
    ADD CONSTRAINT [FkBvThresholds_ThresholdTypes] FOREIGN KEY ([ThresholdsTypeID]) REFERENCES [dbo].[BvThresholdTypes] ([ID]) ON DELETE CASCADE;


GO
PRINT N'Creating FK_BvSurvey_Schedule...';


GO
ALTER TABLE [dbo].[BvSurvey] WITH NOCHECK
    ADD CONSTRAINT [FK_BvSurvey_Schedule] FOREIGN KEY ([ScheduleID]) REFERENCES [dbo].[BvSchedule] ([ScheduleID]);


GO
PRINT N'Creating FK_BvSchedule_BvStateGroup...';


GO
ALTER TABLE [dbo].[BvSchedule] WITH NOCHECK
    ADD CONSTRAINT [FK_BvSchedule_BvStateGroup] FOREIGN KEY ([DesignStateGroupID]) REFERENCES [dbo].[BvStateGroup] ([ID]) ON DELETE SET NULL;


GO
PRINT N'Creating FK_BvReportParam_BvReportBatch...';


GO
ALTER TABLE [dbo].[BvReportParam] WITH NOCHECK
    ADD CONSTRAINT [FK_BvReportParam_BvReportBatch] FOREIGN KEY ([BatchID]) REFERENCES [dbo].[BvReportBatch] ([ID]);


GO
PRINT N'Creating FK_BvReportBatch_BvReport...';


GO
ALTER TABLE [dbo].[BvReportBatch] WITH NOCHECK
    ADD CONSTRAINT [FK_BvReportBatch_BvReport] FOREIGN KEY ([ReportID]) REFERENCES [dbo].[BvReport] ([Rpt_ID]);


GO
PRINT N'Creating FK_BvPersonMonitoringLastID_BvPersonMonitoring...';


GO
ALTER TABLE [dbo].[BvPersonMonitoringLastID] WITH NOCHECK
    ADD CONSTRAINT [FK_BvPersonMonitoringLastID_BvPersonMonitoring] FOREIGN KEY ([PersonSID]) REFERENCES [dbo].[BvPersonMonitoring] ([PersonSID]) ON DELETE CASCADE;


GO
PRINT N'Creating FK_BvPersonMonitoringEvents_BvPersonMonitoring...';


GO
ALTER TABLE [dbo].[BvPersonMonitoringEvents] WITH NOCHECK
    ADD CONSTRAINT [FK_BvPersonMonitoringEvents_BvPersonMonitoring] FOREIGN KEY ([PersonSID]) REFERENCES [dbo].[BvPersonMonitoring] ([PersonSID]) ON DELETE CASCADE;


GO
PRINT N'Creating FK_BvPersonMonitoring_BvPerson...';


GO
ALTER TABLE [dbo].[BvPersonMonitoring] WITH NOCHECK
    ADD CONSTRAINT [FK_BvPersonMonitoring_BvPerson] FOREIGN KEY ([PersonSID]) REFERENCES [dbo].[BvPerson] ([SID]) ON DELETE CASCADE;


GO
PRINT N'Creating FK_BvPerson_BvSurvey...';


GO
ALTER TABLE [dbo].[BvPerson] WITH NOCHECK
    ADD CONSTRAINT [FK_BvPerson_BvSurvey] FOREIGN KEY ([AutomaticSurveyID]) REFERENCES [dbo].[BvSurvey] ([SID]) ON DELETE SET NULL;


GO
PRINT N'Creating FK_BvPerson_CallGroupID...';


GO
ALTER TABLE [dbo].[BvPerson] WITH NOCHECK
    ADD CONSTRAINT [FK_BvPerson_CallGroupID] FOREIGN KEY ([CallGroupID]) REFERENCES [dbo].[BvCallGroup] ([Id]) ON DELETE SET NULL;


GO
PRINT N'Creating FK_BvPerson_TimezoneID...';


GO
ALTER TABLE [dbo].[BvPerson] WITH NOCHECK
    ADD CONSTRAINT [FK_BvPerson_TimezoneID] FOREIGN KEY ([TimezoneID]) REFERENCES [dbo].[BvTimezone] ([ID]);


GO
PRINT N'Creating FK_BvMessageToPerson_BvPerson...';


GO
ALTER TABLE [dbo].[BvMessageToPerson] WITH NOCHECK
    ADD CONSTRAINT [FK_BvMessageToPerson_BvPerson] FOREIGN KEY ([InterviewerId]) REFERENCES [dbo].[BvPerson] ([SID]) ON DELETE CASCADE;


GO
PRINT N'Creating FK_BvMessageToPerson_BvMessages...';


GO
ALTER TABLE [dbo].[BvMessageToPerson] WITH NOCHECK
    ADD CONSTRAINT [FK_BvMessageToPerson_BvMessages] FOREIGN KEY ([MessageId]) REFERENCES [dbo].[BvMessages] ([Id]) ON DELETE CASCADE;


GO
PRINT N'Creating FkBvAppointmentsAlertStatus_Appointment...';


GO
ALTER TABLE [dbo].[BvAppointmentsAlertStatus] WITH NOCHECK
    ADD CONSTRAINT [FkBvAppointmentsAlertStatus_Appointment] FOREIGN KEY ([ID]) REFERENCES [dbo].[BvAppointment] ([ID]) ON DELETE CASCADE;


GO
PRINT N'Creating FkBvAppointmentCounters_Survey...';


GO
ALTER TABLE [dbo].[BvAppointmentCounters] WITH NOCHECK
    ADD CONSTRAINT [FkBvAppointmentCounters_Survey] FOREIGN KEY ([SurveySID]) REFERENCES [dbo].[BvSurvey] ([SID]) ON DELETE CASCADE;


GO
PRINT N'Creating FkBvAggregateSurveyAlertStatus_Survey...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus] WITH NOCHECK
    ADD CONSTRAINT [FkBvAggregateSurveyAlertStatus_Survey] FOREIGN KEY ([SID]) REFERENCES [dbo].[BvSurvey] ([SID]) ON DELETE CASCADE;


GO
PRINT N'Creating FkBvAggregateSurvey_Survey...';


GO
ALTER TABLE [dbo].[BvAggregateSurvey] WITH NOCHECK
    ADD CONSTRAINT [FkBvAggregateSurvey_Survey] FOREIGN KEY ([SID]) REFERENCES [dbo].[BvSurvey] ([SID]) ON DELETE CASCADE;


GO
PRINT N'Creating [dbo].[TrCallGroupCondition_Update]...';


GO
CREATE TRIGGER TrCallGroupCondition_Update ON BvCallGroupCondition FOR UPDATE
AS
    UPDATE BvCallGroupConditionPerSurvey
        SET ConditionPriority = inserted.ConditionPriority
    FROM BvCallGroupConditionPerSurvey 
    INNER JOIN inserted 
    ON	BvCallGroupConditionPerSurvey.CallGroupId = inserted.CallGroupId AND
        BvCallGroupConditionPerSurvey.ConditionValue = inserted.ConditionValue
GO
PRINT N'Creating [dbo].[TrCallGroupCondition_Delete]...';


GO
CREATE TRIGGER TrCallGroupCondition_Delete ON BvCallGroupCondition FOR DELETE
AS
    DELETE FROM BvCallGroupConditionPerSurvey 
        FROM BvCallGroupConditionPerSurvey cgc
        INNER JOIN deleted d 
        ON cgc.CallGroupId = d.CallGroupId AND cgc.ConditionValue = d.ConditionValue
GO
PRINT N'Creating [dbo].[TrCallGroupCondition_Insert]...';


GO
CREATE TRIGGER TrCallGroupCondition_Insert ON BvCallGroupCondition FOR INSERT
AS
    ;WITH surveys AS
    (
        SELECT SID FROM BvSurvey WHERE State = 1 AND SurveySchedulingMode = 1
    )
    INSERT INTO BvCallGroupConditionPerSurvey(SurveyId, CallGroupId, ConditionValue, ConditionPriority )
            SELECT SID, CallGroupId, ConditionValue, ConditionPriority FROM inserted, surveys
GO
PRINT N'Creating [dbo].[TrTimezoneShiftDelete]...';


GO
CREATE TRIGGER [dbo].[TrTimezoneShiftDelete] ON [dbo].[BvTimezoneShift] 
AFTER DELETE 
AS
declare @site_tz int
 
    delete from BvTzPeriodicalShifts
        from BvTzPeriodicalShifts t, deleted d
        where t.shift_id    = d.ShiftID
            and t.owner_id  = d.OwnerSID
            and t.tz_id     = d.TimezoneID
 
    delete from BvTzUnPeriodicalShifts
        from BvTzUnPeriodicalShifts t, deleted d
        where t.shift_id    = d.ShiftID
            and t.owner_id  = d.OwnerSID
            and t.tz_id     = d.TimezoneID
GO
PRINT N'Creating [dbo].[TrTimezoneInsert]...';


GO
CREATE TRIGGER dbo.TrTimezoneInsert ON dbo.BvTimezone 
AFTER INSERT 
AS
DECLARE @site_tz    int = ISNULL( ( SELECT Value FROM BvSystemSettings WHERE SystemName = 'Site.TimeZoneID' ), 1 )
 
    INSERT INTO BvTzPeriodicalShifts
        SELECT  ts.shift_id,
                ts.type_id,
                ts.owner_id,
                inserted.ID,
                ts.start_dt,
                ts.finish_dt
    FROM BvTzPeriodicalShifts ts, inserted
    WHERE ts.tz_id = @site_tz
 
    INSERT INTO BvTzUnPeriodicalShifts
        SELECT  ts.shift_id,
                ts.type_id,
                ts.owner_id,
                inserted.ID,
                ts.start_dt,
                ts.finish_dt
    FROM BvTzUnPeriodicalShifts ts, inserted
    WHERE ts.tz_id = @site_tz

    -- Insert shift type time zones
    INSERT INTO BvShiftZones 
      SELECT i.ID, BvShiftType.[ObjectID]
      FROM inserted i, BvShiftType
GO
PRINT N'Creating [dbo].[TrTimezoneDelete]...';


GO
CREATE TRIGGER dbo.TrTimezoneDelete ON dbo.BvTimezone 
AFTER DELETE 
AS
    DELETE FROM BvTzPeriodicalShifts WHERE tz_id IN
        ( SELECT ID FROM deleted )
 
    DELETE FROM BvTzUnPeriodicalShifts WHERE tz_id IN
        ( SELECT ID FROM deleted )

    DELETE FROM BvShiftZones WHERE TimeZoneID IN
        ( SELECT ID FROM deleted )
GO
PRINT N'Creating [dbo].[trBvTasksDelete]...';


GO
CREATE TRIGGER trBvTasksDelete ON BvTasks
AFTER DELETE
AS
 DELETE FROM BvPersonMonitoring
  WHERE PersonSID IN ( SELECT PersonSID FROM deleted )
GO
PRINT N'Creating [dbo].[BvTrBvSvySchedule_CallsUpdate]...';


GO
CREATE TRIGGER [BvTrBvSvySchedule_CallsUpdate] ON [dbo].[BvSvySchedule]
FOR UPDATE 
AS 
BEGIN
	SET NOCOUNT ON
	
	IF UPDATE( CallState )
	BEGIN								                          
       UPDATE BvAggregateSurvey
       SET ScheduledCallsCount += cnt
       FROM BvAggregateSurvey
       INNER JOIN (SELECT inserted.SurveySid, SUM(CASE WHEN inserted.CallState IN (2, -2) THEN 1 --call have been added
                                                       ELSE -1 --call have been deleted
                                                  END) cnt
                   FROM inserted
                   INNER JOIN deleted ON inserted.id = deleted.id AND
                                         ((inserted.CallState IN (2,-2) AND         --call have been added
                                           deleted.CallState NOT IN (2, -2)) OR     -- OR
                                          (inserted.CallState NOT IN (2, -2) AND    --call have been deleted
                                           deleted.CallState IN (2, -2)))
                   group by inserted.SurveySid) u ON SID = SurveySid
	END
END
GO
PRINT N'Creating [dbo].[BvTrBvSvySchedule_CallsInsert]...';


GO
CREATE TRIGGER [BvTrBvSvySchedule_CallsInsert] ON [dbo].[BvSvySchedule]
AFTER INSERT
AS 
BEGIN
	SET NOCOUNT ON
	
    UPDATE BvAggregateSurvey
    SET ScheduledCallsCount += cnt
    FROM BvAggregateSurvey
    INNER JOIN (SELECT SurveySid, COUNT(*) cnt
                FROM inserted
                WHERE CallState IN (2, -2)
                group by SurveySid) u ON SID = SurveySid
END
GO
PRINT N'Creating [dbo].[BvTrBvSvySchedule_CallsDelete]...';


GO
CREATE TRIGGER [BvTrBvSvySchedule_CallsDelete] ON [dbo].[BvSvySchedule]
FOR DELETE
AS 
BEGIN
	SET NOCOUNT ON
                                      
    UPDATE BvAggregateSurvey
    SET ScheduledCallsCount -= cnt
    FROM BvAggregateSurvey
    INNER JOIN (SELECT SurveySid, COUNT(*) cnt
                FROM deleted
                WHERE CallState IN (2, -2)
                group by SurveySid) u ON SID = SurveySid
END
GO
PRINT N'Creating [dbo].[TrSurvey_Changed]...';


GO
CREATE TRIGGER TrSurvey_Changed ON BvSurvey FOR INSERT, UPDATE, DELETE
AS
    --insert
    WITH activated AS
    (
        SELECT i.SID FROM inserted i 
        LEFT JOIN deleted d 
        ON i.SID = d.SID 
        WHERE ( i.State = 1 AND i.SurveySchedulingMode = 1 ) AND 
                (d.State <> 1 OR d.SurveySchedulingMode <> 1 OR d.SID IS NULL ) 
    )
    INSERT INTO BvCallGroupConditionPerSurvey(SurveyId, CallGroupId, ConditionValue, ConditionPriority )
        SELECT SID, CallGroupId, ConditionValue, ConditionPriority FROM activated, BvCallGroupCondition
                
    --delete
    ;WITH deactivated AS
    (
        SELECT d.SID FROM deleted d
        LEFT JOIN inserted i
        ON i.SID = d.SID 
        WHERE (d.State = 1 AND d.SurveySchedulingMode = 1 ) AND
                ( i.State <> 1 OR i.SurveySchedulingMode <> 1 OR i.SID IS NULL) 
    )
    DELETE FROM BvCallGroupConditionPerSurvey WHERE SurveyId IN ( SELECT SID FROM deactivated )
GO
PRINT N'Creating [dbo].[TrShiftDelete]...';


GO
CREATE TRIGGER [dbo].[TrShiftDelete] ON [dbo].[BvShift] 
AFTER DELETE 
AS
    delete from BvTzPeriodicalShifts
        from BvTzPeriodicalShifts t, deleted d
        where t.shift_id    = d.[ID]
            and t.owner_id  = d.OwnerSID
 
    delete from BvTzUnPeriodicalShifts
        from BvTzUnPeriodicalShifts t, deleted d
        where t.shift_id    = d.[ID]
            and t.owner_id  = d.OwnerSID
GO
PRINT N'Creating [dbo].[BvTrBvHistory_HistoryInsert]...';


GO
CREATE TRIGGER [BvTrBvHistory_HistoryInsert] ON [dbo].[BvHistory]
FOR INSERT
AS 
BEGIN
	SET NOCOUNT ON
										      
    UPDATE BvAggregateSurvey
    SET MinutesSpentWorkingOnSurvey += cnt
    FROM BvAggregateSurvey
    INNER JOIN (SELECT SurveyId, ISNULL(SUM(WaitingTime), 0) + ISNULL(SUM(Duration), 0) cnt
                FROM inserted
                WHERE RoleId = 2
                group by SurveyId) u ON SID = SurveyId
END
GO
PRINT N'Creating [dbo].[utilMaskQuote]...';


GO
CREATE FUNCTION [dbo].[utilMaskQuote]
(
 @InputString NVARCHAR(max) 
)
RETURNS NVARCHAR(max)  
AS      

BEGIN    
 DECLARE @ToReplace NVARCHAR(max)
 SET @ToReplace = N''''
 DECLARE @ReplaceWith NVARCHAR(max)
 SET @ReplaceWith = N''''''
 DECLARE @OutputString NVARCHAR(max)  
 SET @OutputString = REPLACE(@InputString, @ToReplace, @ReplaceWith)
 RETURN @OutputString
END
GO
PRINT N'Creating [dbo].[udf_CS]...';


GO
create function [dbo].[udf_CS] 
(
    @str     nvarchar( 256 )
)
returns nvarchar( 256 )
begin
    set @str = rtrim( @str )

    if len( @str ) > 0 and left( @str, 1 ) <> '?'
        return @str

    return NULL
end
GO
PRINT N'Creating [dbo].[udf_CodeToBase64]...';


GO
CREATE FUNCTION udf_CodeToBase64(
 @code TINYINT
)
RETURNS NCHAR(1)
AS
BEGIN

DECLARE @result NCHAR(1)

IF @code < 26
 SET @result = NCHAR( 65 + @code )
ELSE IF @code < 52
    SET @result = NCHAR( 97 + @code - 26 )
ELSE IF @code < 62
    SET @result = NCHAR( 48 + @code - 52 )
ELSE IF @code = 62
    SET @result = N'-'
ELSE
    SET @result = N'/'

RETURN @result

END
GO
PRINT N'Creating [dbo].[udf_CI]...';


GO
CREATE  function [dbo].[udf_CI] 
(
    @str nvarchar( 256 )
)
returns int
begin
    set @str = rtrim( @str )

    if len( @str ) > 0 and left( @str, 1 ) <> '?' begin
        return cast( @str as int )
    end

    return NULL
end
GO
PRINT N'Creating [dbo].[udf_CF]...';


GO
CREATE  function [dbo].[udf_CF] 
(
    @str     nvarchar( 256 )
)
returns float
begin
    set @str = rtrim( @str )

    if len( @str ) > 0 and left( @str, 1 ) <> '?'
        return cast( @str as float )

    return NULL
end
GO
PRINT N'Creating [dbo].[udf_CD9]...';


GO
CREATE function [dbo].[udf_CD9] 
(
    @str nvarchar( 256 )
)
returns decimal(10,9)
begin
    declare @len int
    set @str = rtrim( @str )

    set @len = len( @str )

    if @len > 0 and left( @str, 1 ) <> '?' begin
        if @len < 9 begin
            set @str = '00000000' + @str
            set @len = @len + 8
        end
        set @str = left( @str, @len - 9 ) + '.' + right( @str, 9 )
        return cast( @str as decimal( 10, 9 ) )
    end

    return NULL
end
GO
PRINT N'Creating [dbo].[udf_CD8]...';


GO
CREATE function [dbo].[udf_CD8] 
(
    @str nvarchar( 256 )
)
returns decimal(10,8)
begin
    declare @len int
    set @str = rtrim( @str )

    set @len = len( @str )

    if @len > 0 and left( @str, 1 ) <> '?' begin
        if @len < 8 begin
            set @str = '0000000' + @str
            set @len = @len + 7
        end
        set @str = left( @str, @len - 8 ) + '.' + right( @str, 8 )
        return cast( @str as decimal( 10, 8 ) )
    end

    return NULL
end
GO
PRINT N'Creating [dbo].[udf_CD7]...';


GO
CREATE function [dbo].[udf_CD7] 
(
    @str nvarchar( 256 )
)
returns decimal(10,7)
begin
    declare @len int
    set @str = rtrim( @str )

    set @len = len( @str )

    if @len > 0 and left( @str, 1 ) <> '?' begin
        if @len < 7 begin
            set @str = '000000' + @str
            set @len = @len + 6
        end
        set @str = left( @str, @len - 7 ) + '.' + right( @str, 7 )
        return cast( @str as decimal( 10, 7 ) )
    end

    return NULL
end
GO
PRINT N'Creating [dbo].[udf_CD6]...';


GO
CREATE function [dbo].[udf_CD6] 
(
    @str nvarchar( 256 )
)
returns decimal(10,6)
begin
    declare @len int
    set @str = rtrim( @str )

    set @len = len( @str )

    if @len > 0 and left( @str, 1 ) <> '?' begin
        if @len < 6 begin
            set @str = '00000' + @str
            set @len = @len + 5
        end
        set @str = left( @str, @len - 6 ) + '.' + right( @str, 6 )
        return cast( @str as decimal( 10, 6 ) )
    end

    return NULL
end
GO
PRINT N'Creating [dbo].[udf_CD5]...';


GO
CREATE function [dbo].[udf_CD5] 
(
    @str nvarchar( 256 )
)
returns decimal(10,5)
begin
    declare @len int
    set @str = rtrim( @str )

    set @len = len( @str )

    if @len > 0 and left( @str, 1 ) <> '?' begin
        if @len < 5 begin
            set @str = '0000' + @str
            set @len = @len + 4
        end
        set @str = left( @str, @len - 5 ) + '.' + right( @str, 5 )
        return cast( @str as decimal( 10, 5 ) )
    end

    return NULL
end
GO
PRINT N'Creating [dbo].[udf_CD4]...';


GO
CREATE function [dbo].[udf_CD4] 
(
    @str nvarchar( 256 )
)
returns decimal(10,4)
begin
    declare @len int
    set @str = rtrim( @str )

    set @len = len( @str )

    if @len > 0 and left( @str, 1 ) <> '?' begin
        if @len < 4 begin
            set @str = '000' + @str
            set @len = @len + 3
        end
        set @str = left( @str, @len - 4 ) + '.' + right( @str, 4 )
        return cast( @str as decimal( 10, 4 ) )
    end

    return NULL
end
GO
PRINT N'Creating [dbo].[udf_CD3]...';


GO
CREATE function [dbo].[udf_CD3] 
(
    @str nvarchar( 256 )
)
returns decimal(10,3)
begin
    declare @len int
    set @str = rtrim( @str )

    set @len = len( @str )

    if @len > 0 and left( @str, 1 ) <> '?' begin
        if @len < 3 begin
            set @str = '00' + @str
            set @len = @len + 2
        end
        set @str = left( @str, @len - 3 ) + '.' + right( @str, 3 )
        return cast( @str as decimal( 10, 3 ) )
    end

    return NULL
end
GO
PRINT N'Creating [dbo].[udf_CD2]...';


GO
CREATE function [dbo].[udf_CD2] 
(
    @str nvarchar( 256 )
)
returns decimal(10,2)
begin
    declare @len int
    set @str = rtrim( @str )

    set @len = len( @str )

    if @len > 0 and left( @str, 1 ) <> '?' begin
        if @len < 2 begin
            set @str = '0' + @str
            set @len = @len + 1
        end
        set @str = left( @str, @len - 2 ) + '.' + right( @str, 2 )
        return cast( @str as decimal( 10, 2 ) )
    end

    return NULL
end
GO
PRINT N'Creating [dbo].[udf_CD10]...';


GO
CREATE function [dbo].[udf_CD10] 
(
    @str nvarchar( 256 )
)
returns decimal(10,10)
begin
    declare @len int
    set @str = rtrim( @str )

    set @len = len( @str )

    if @len > 0 and left( @str, 1 ) <> '?' begin
        if @len < 10 begin
            set @str = '000000000' + @str
            set @len = @len + 9
        end
        set @str = left( @str, @len - 10 ) + '.' + right( @str, 10 )
        return cast( @str as decimal( 10, 10 ) )
    end

    return NULL
end
GO
PRINT N'Creating [dbo].[udf_CD1]...';


GO
create function [dbo].[udf_CD1] 
(
    @str nvarchar( 256 )
)
returns decimal(10,1)
begin
    declare @len int
    set @str = rtrim( @str )

    set @len = len( @str )

    if @len > 0 and left( @str, 1 ) <> '?' begin
        set @str = left( @str, @len - 1 ) + '.' + right( @str, 1 )
        return cast( @str as decimal( 10, 1 ) )
    end

    return NULL
end
GO
PRINT N'Creating [dbo].[udf_AlertStatus_INT]...';


GO
CREATE FUNCTION dbo.udf_AlertStatus_INT
(
    @Value INT,
    @Amber INT,
    @Red INT
)
RETURNS INT
BEGIN
    IF( (@Amber IS NULL) OR (@Red IS NULL) )
    BEGIN
       RETURN (0)
    END

    IF @Red = @Amber 
    BEGIN
        IF @Value = @Red
            RETURN (2)
    END
    ELSE IF @Red > @Amber 
    BEGIN
        IF @Value >= @Red
            RETURN (2)
        ELSE IF @Value >= @Amber
            RETURN (1)
    END
    ELSE --IF @Red < @Amber 
    BEGIN
        IF @Value <= @Red
            RETURN (2)
        ELSE IF @Value <= @Amber
            RETURN (1)
    END
    RETURN (0)
END
GO
PRINT N'Creating [dbo].[udf_AlertStatus_DATETIME]...';


GO
CREATE FUNCTION dbo.udf_AlertStatus_DATETIME
(
    @Value DATETIME,
    @Now DATETIME,
    @Amber INT,
    @Red INT
)
RETURNS INT
BEGIN

    DECLARE @RedDate DATETIME;
    DECLARE @AmberDate DATETIME;


    SET @Now = DATEADD(millisecond, -DATEPART(millisecond, @Now), @Now)
    SET @Value = DATEADD(millisecond, -DATEPART(millisecond, @Value), @Value)
    SET @RedDate = DATEADD(second, - @Red, @Now)
    SET @AmberDate = DATEADD(second, - @Amber, @Now)


    IF @Red = @Amber 
    BEGIN
        IF @Value = @RedDate
            RETURN (2)
    END
    ELSE IF @Red > @Amber 
    BEGIN
        IF @Value <= @RedDate
            RETURN (2)
        ELSE IF @Value <= @AmberDate
            RETURN (1)
    END
    ELSE --IF @Red < @Amber 
    BEGIN
        IF @Value >= @RedDate
            RETURN (2)
        ELSE IF @Value >= @AmberDate
            RETURN (1)
    END
    RETURN (0)
END
GO
PRINT N'Creating [dbo].[udfShiftStart]...';


GO
CREATE function dbo.udfShiftStart
-- return count of minutes from start of week
(
@week_day    int,
@time        smalldatetime
)
returns integer 
as
begin
 
    return @week_day * 1440 + datepart( hour, @time ) * 60 +
        datepart( minute, @time )
end
GO
PRINT N'Creating [dbo].[udfShiftFinish]...';


GO
create function dbo.udfShiftFinish
-- return count of minutes from start of week
(
@sweek_day    int,
@fweek_day    int,
@stime        smalldatetime,
@ftime        smalldatetime
)
returns integer 
as
begin
declare @week_day int
 
    set @week_day = @fweek_day
    
    if ( @fweek_day < @sweek_day ) or
        ( @fweek_day = @sweek_day and @sweek_day > @fweek_day )
        set @week_day = @week_day + 7
 
    return @week_day * 1440 + datepart( hour, @ftime ) * 60 +
        datepart( minute, @ftime )
end
GO
PRINT N'Creating [dbo].[GetCurrentBiasDate]...';


GO
CREATE FUNCTION dbo.GetCurrentBiasDate (
                @Date datetime,
                @ReferenceDate datetime,
                @ReferenceDOW int
)
RETURNS datetime
AS
BEGIN

                DECLARE @CurrentDay datetime
                DECLARE @CurrentDayDOW int
                DECLARE @Delta int
                
                SET @CurrentDay = 
                                CONVERT(CHAR(4), YEAR(@Date)) +
                                '-' +
                                RIGHT('0'+ CONVERT(VARCHAR(2), MONTH(@ReferenceDate)), 2) +
                                '-01 ' +
                                CONVERT(VARCHAR(8), @ReferenceDate, 108) 
                
                SET @CurrentDayDOW =
                                DATEPART(dw, @CurrentDay) 
                
                SET @Delta = DATEPART(day, @ReferenceDate)
                
                IF @CurrentDayDOW < (@ReferenceDOW + 1)
                                SET @CurrentDay = 
                                                DATEADD(day, 
                                                                @ReferenceDOW - @CurrentDayDOW + 1, 
                                                                @CurrentDay)
                ELSE IF @CurrentDayDOW > (@ReferenceDOW + 1)
                                SET @CurrentDay = 
                                                DATEADD(day, 
                                                                8 + @ReferenceDOW - @CurrentDayDOW, 
                                                                @CurrentDay)
                
                 
                SET @CurrentDay = 
                                DATEADD(week, @Delta - 1, @CurrentDay)
                
                WHILE DATEPART(month, @CurrentDay) > 
                                DATEPART(month, @ReferenceDate) 
                
                                                SET @CurrentDay = 
                                                                DATEADD(week, - 1, @CurrentDay)

                RETURN (@CurrentDay)   
END
GO
PRINT N'Creating [dbo].[TrTimezoneShiftUpdate]...';


GO
CREATE TRIGGER [dbo].[TrTimezoneShiftUpdate] ON [dbo].[BvTimezoneShift] 
AFTER UPDATE 
AS
 
-- first delete
    delete from BvTzUnPeriodicalShifts
    from BvTzUnPeriodicalShifts t, inserted d
    where t.shift_id    = d.ShiftID
        and t.owner_id  = d.OwnerSID
        and t.tz_id     = d.TimezoneID
 
    delete from BvTzPeriodicalShifts
    from BvTzPeriodicalShifts t, inserted d
    where t.shift_id    = d.ShiftID
        and t.owner_id  = d.OwnerSID
        and t.tz_id     = d.TimezoneID
 
-- second insert
insert into BvTzUnPeriodicalShifts
    select s.[ID] as ShiftID,
           s.ShiftTypeID,
           s.OwnerSID,
           t.TimeZoneID as timezone,
           t.StartTime  as Start,
           t.FinishTime as Finish
    from dbo.BvShift s
    inner join inserted t on s.[ID] = t.ShiftID 
        and s.OwnerSID = t.OwnerSID
    where s.CycleType = 2
 
insert into BvTzPeriodicalShifts
    select s.[ID] as ShiftID,
           s.ShiftTypeID,
           s.OwnerSID,
           t.TimeZoneID as timezone,
           dbo.udfShiftStart( t.StartDayOfWeek, t.StartTime ) as StartInMins,
           dbo.udfShiftFinish( t.StartDayOfWeek, t.FinishDayOfWeek, t.StartTime, t.FinishTime ) as FinishInMins
    from dbo.BvShift s
    inner join inserted t on s.[ID] = t.ShiftID and s.OwnerSID = t.OwnerSID
    where s.CycleType = 1
GO
PRINT N'Creating [dbo].[TrTimezoneShiftInsert]...';


GO
CREATE TRIGGER [dbo].[TrTimezoneShiftInsert] ON [dbo].[BvTimezoneShift] 
AFTER INSERT
AS
 
-- first delete
    delete from BvTzUnPeriodicalShifts
    from BvTzUnPeriodicalShifts t, inserted d
    where t.shift_id    = d.ShiftID
        and t.owner_id  = d.OwnerSID
        and t.tz_id     = d.TimezoneID
 
    delete from BvTzPeriodicalShifts
    from BvTzPeriodicalShifts t, inserted d
    where t.shift_id    = d.ShiftID
        and t.owner_id  = d.OwnerSID
        and t.tz_id     = d.TimezoneID
 
-- second insert
insert into BvTzUnPeriodicalShifts
    select s.[ID] as ShiftID,
           s.ShiftTypeID,
           s.OwnerSID,
           t.TimeZoneID as timezone,
           t.StartTime  as Start,
           t.FinishTime as Finish
    from dbo.BvShift s
    inner join inserted t on s.[ID] = t.ShiftID 
        and s.OwnerSID = t.OwnerSID
    where s.CycleType = 2
 
insert into BvTzPeriodicalShifts
    select s.[ID] as ShiftID,
           s.ShiftTypeID,
           s.OwnerSID,
           t.TimeZoneID as timezone,
           dbo.udfShiftStart( t.StartDayOfWeek, t.StartTime ) as StartInMins,
           dbo.udfShiftFinish( t.StartDayOfWeek, t.FinishDayOfWeek, t.StartTime, t.FinishTime ) as FinishInMins
    from dbo.BvShift s
    inner join inserted t on s.[ID] = t.ShiftID and s.OwnerSID = t.OwnerSID
    where s.CycleType = 1
GO
PRINT N'Creating [dbo].[TrShiftUpdate]...';


GO
CREATE TRIGGER [dbo].[TrShiftUpdate] ON [dbo].[BvShift] 
AFTER UPDATE
AS
    delete from BvTzPeriodicalShifts
        from BvTzPeriodicalShifts t, inserted d
        where t.shift_id    = d.[ID]
            and t.owner_id  = d.OwnerSID
 
    delete from BvTzUnPeriodicalShifts
        from BvTzUnPeriodicalShifts t, inserted d
        where t.shift_id    = d.[ID]
            and t.owner_id  = d.OwnerSID
 
-- insert un periodical shifts
    insert into BvTzUnPeriodicalShifts
        select s.[ID] as ShiftID,
           s.ShiftTypeID,
           s.OwnerSID,
           tz.[ID] as timezone,
           s.StartTime as Start,
           s.FinishTime as Finish
        from inserted s
        cross join dbo.BvTimezone tz
        where s.CycleType = 2
 
-- insert periodical shifts
    insert into BvTzPeriodicalShifts
        select s.[ID] as ShiftID,
           s.ShiftTypeID,
           s.OwnerSID,
           tz.[ID] as timezone,
           dbo.udfShiftStart( s.StartDayOfWeek, s.StartTime ),
           dbo.udfShiftFinish( s.StartDayOfWeek, s.FinishDayOfWeek, s.StartTime, s.FinishTime )
    from inserted s
    cross join dbo.BvTimezone tz
    where s.CycleType = 1
GO
PRINT N'Creating [dbo].[TrShiftInsert]...';


GO
CREATE TRIGGER [dbo].[TrShiftInsert] ON [dbo].[BvShift] 
AFTER INSERT
AS
 
-- insert un periodical shifts
    insert into BvTzUnPeriodicalShifts
        select s.[ID] as ShiftID,
           s.ShiftTypeID,
           s.OwnerSID,
           tz.[ID] as timezone,
           s.StartTime as Start,
           s.FinishTime as Finish
        from inserted s
        cross join dbo.BvTimezone tz
        where s.CycleType = 2
 
-- insert periodical shifts
 
    insert into BvTzPeriodicalShifts
        select s.[ID] as ShiftID,
           s.ShiftTypeID,
           s.OwnerSID,
           tz.[ID] as timezone,
           dbo.udfShiftStart( s.StartDayOfWeek, s.StartTime ),
           dbo.udfShiftFinish( s.StartDayOfWeek, s.FinishDayOfWeek, s.StartTime, s.FinishTime )
    from inserted s
    cross join dbo.BvTimezone tz
    where s.CycleType = 1
GO
PRINT N'Creating [dbo].[BvTrBvInterview_InterviewsUpdate]...';


GO
CREATE TRIGGER [BvTrBvInterview_InterviewsUpdate] ON [dbo].[BvInterview] 
AFTER UPDATE
AS
BEGIN
set nocount on
    IF UPDATE( TransientState )
    BEGIN
        UPDATE aggrTbl
            SET aggrTbl.Cnt = aggrTbl.Cnt + data.Dif,
                alertStatus = dbo.udf_AlertStatus_INT( aggrTbl.Cnt + data.Dif, ThresholdDef.Amber, ThresholdDef.Red )
        FROM BvSampleStatusSummary aggrTbl
        INNER JOIN ( 
            SELECT SurveySID, TransientState, SUM( Dif ) as Dif FROM (
                SELECT SurveySID, TransientState, COUNT(ID) as Dif FROM INSERTED GROUP BY SurveySID, TransientState 
                UNION ALL
                SELECT SurveySID, TransientState, -COUNT(ID) as Dif FROM DELETED GROUP BY SurveySID, TransientState 
            ) as t GROUP BY SurveySID, TransientState
                 ) as data
            ON aggrTbl.SurveySID = data.SurveySID AND aggrTbl.ITS = data.TransientState 
        LEFT JOIN BvThresholdITS as ThresholdDef
            ON ThresholdDef.SurveySID = 0 /*Use default thresholds, survey specific thresholds are not supported now*/ AND ThresholdDef.ITS = data.TransientState 
    END
END
GO
PRINT N'Creating [dbo].[BvTrBvInterview_InterviewsInsert]...';


GO
CREATE TRIGGER [BvTrBvInterview_InterviewsInsert] ON [dbo].[BvInterview] 
AFTER INSERT
AS
BEGIN
    UPDATE BvAggregateSurvey
    SET SuspendedCallsCount += cnt
    FROM BvAggregateSurvey
    INNER JOIN (SELECT SurveySID, COUNT(*) cnt
                FROM inserted
                group by SurveySID) u ON SID = SurveySID
                
                
    UPDATE aggrTbl
        SET aggrTbl.Cnt = aggrTbl.Cnt + data.Dif,
            alertStatus = dbo.udf_AlertStatus_INT( aggrTbl.Cnt + data.Dif, ThresholdDef.Amber, ThresholdDef.Red )
    FROM BvSampleStatusSummary aggrTbl
    INNER JOIN ( 
        SELECT SurveySID, TransientState, COUNT(ID) as Dif FROM INSERTED GROUP BY SurveySID, TransientState 
             ) as data
        ON aggrTbl.SurveySID = data.SurveySID AND aggrTbl.ITS = data.TransientState 
    LEFT JOIN BvThresholdITS as ThresholdDef
        ON ThresholdDef.SurveySID = 0 /*Use default thresholds, survey specific thresholds are not supported now*/ AND ThresholdDef.ITS = data.TransientState 
END
GO
PRINT N'Creating [dbo].[BvTrBvInterview_InterviewsDelete]...';


GO
CREATE TRIGGER [BvTrBvInterview_InterviewsDelete] ON [dbo].[BvInterview] 
AFTER DELETE
AS
BEGIN
    UPDATE BvAggregateSurvey
    SET SuspendedCallsCount -= cnt
    FROM BvAggregateSurvey
    INNER JOIN (SELECT SurveySID, COUNT(*) cnt
                FROM deleted
                group by SurveySID) u ON SID = SurveySID

    UPDATE aggrTbl
        SET aggrTbl.Cnt = aggrTbl.Cnt + data.Dif,
            alertStatus = dbo.udf_AlertStatus_INT( aggrTbl.Cnt + data.Dif, ThresholdDef.Amber, ThresholdDef.Red )
    FROM BvSampleStatusSummary aggrTbl
    INNER JOIN ( 
        SELECT SurveySID, TransientState, -COUNT(ID) as Dif FROM DELETED GROUP BY SurveySID, TransientState 
             ) as data
        ON aggrTbl.SurveySID = data.SurveySID AND aggrTbl.ITS = data.TransientState 
    LEFT JOIN BvThresholdITS as ThresholdDef
        ON ThresholdDef.SurveySID = 0 /*Use default thresholds, survey specific thresholds are not supported now*/ AND ThresholdDef.ITS = data.TransientState 
END
GO
PRINT N'Creating [dbo].[GetTZBias]...';


GO
CREATE FUNCTION [dbo].[GetTZBias]
(
	@Date datetime,
	@TZID INT
)
RETURNS INT
AS
BEGIN 
    DECLARE @RESULT INT

	DECLARE @DaylightDOW INT, 
			@StandardDOW INT, 
			@StandardStart datetime, 
			@DaylightStart datetime,
			@StandardBias INT, 
			@DaylightBias INT, 
			@Type INT,
			@OriginalBias int

	SELECT
		@Type			= DaylightType,
		@DaylightDOW	= DaylightDayOfWeek,
		@StandardDOW	= StandardDayOfWeek,
		@StandardStart	= StandardStart,
		@DaylightStart	= DaylightStart,
		@StandardBias	= StandardBias,
		@DaylightBias	= DaylightBias,
		@OriginalBias	= Bias
	FROM BvTimezone 
	WHERE ID = @TZID

	IF @Type = 1
	BEGIN
	   RETURN @OriginalBias 
	END

	-- Compute Start Date for Daylight

	DECLARE @CurrentDaylightStart datetime

	SET @CurrentDaylightStart = 
					dbo.GetCurrentBiasDate (@date,@DaylightStart, @DaylightDOW)

	-- Compute Start Date for Standard

	DECLARE @CurrentStandardStart datetime

	SET @CurrentStandardStart = 
					dbo.GetCurrentBiasDate (@date, @StandardStart, @StandardDOW)


	-- get Bias

	IF  @CurrentStandardStart >  @CurrentDaylightStart
	BEGIN
		IF @CurrentDaylightStart <= @Date AND @Date < @CurrentStandardStart 
			SET @RESULT = @OriginalBias + @DaylightBias
		ELSE 
			SET @RESULT = @OriginalBias + @StandardBias
	END
	ELSE 
	BEGIN
		IF @CurrentStandardStart <= @Date and @Date < @CurrentDaylightStart
			SET @RESULT = @OriginalBias + @StandardBias
		ELSE
			SET @RESULT = @OriginalBias + @DaylightBias
	END

	RETURN @RESULT
END
GO
PRINT N'Creating [dbo].[UTC2LT]...';


GO
CREATE  FUNCTION dbo.UTC2LT(
  @utc  SMALLDATETIME,
                @bias  INT,
                @type  INT,
                @stdDOW  INT, 
                @stdStart SMALLDATETIME,
  @stdBias INT,
                @dltDOW  INT, 
                @dltStart SMALLDATETIME,
  @dltBias INT
)
RETURNS SMALLDATETIME
AS
-- DATEFIRST must be set to 7 before calling the function
BEGIN

 SET @stdStart = DATEADD(minute, @bias, @stdStart)
 SET @dltStart = DATEADD(minute, @bias+@dltBias, @dltStart)

 IF @type = 2
 BEGIN
  DECLARE @stdStart1 SMALLDATETIME
  DECLARE @dltStart1 SMALLDATETIME
 
  SET @stdStart1 = dbo.GetCurrentBiasDate( @utc, @stdStart, @stdDOW )
  SET @dltStart1 = dbo.GetCurrentBiasDate( @utc, @dltStart, @dltDOW )
  IF  @stdStart1 >  @dltStart1
  BEGIN
   IF @dltStart1 <= @utc AND @utc < @stdStart1 
                  SET @bias = @bias + @dltBias
   ELSE 
                  SET @bias = @bias + @stdBias
  END
  ELSE
  BEGIN
   IF @stdStart1 <= @utc and @utc < @dltStart1
                  SET @bias = @bias + @stdBias
   ELSE 
                  SET @bias = @bias + @dltBias
  END
 END
 RETURN( DATEADD( minute, -@bias, @utc ) )
END
GO
PRINT N'Creating [dbo].[udf_UuidToBase64]...';


GO
CREATE FUNCTION dbo.udf_UuidToBase64(
 @uuid UNIQUEIDENTIFIER
)
RETURNS NVARCHAR(22)
AS
BEGIN

DECLARE @uuidbin BINARY(16)
SET @uuidbin = CAST( @uuid AS BINARY(16) )

DECLARE @pwd nvarchar(22)

DECLARE @byte TINYINT
DECLARE @byte2 TINYINT
DECLARE @tmp SMALLINT

DECLARE @byte01 TINYINT
DECLARE @byte02 TINYINT
DECLARE @byte03 TINYINT
DECLARE @byte04 TINYINT
DECLARE @byte05 TINYINT
DECLARE @byte06 TINYINT
DECLARE @byte07 TINYINT
DECLARE @byte08 TINYINT
DECLARE @byte09 TINYINT
DECLARE @byte10 TINYINT
DECLARE @byte11 TINYINT
DECLARE @byte12 TINYINT
DECLARE @byte13 TINYINT
DECLARE @byte14 TINYINT
DECLARE @byte15 TINYINT
DECLARE @byte16 TINYINT

SET @byte01 = SUBSTRING( @uuidbin, 1, 1 )
SET @byte02 = SUBSTRING( @uuidbin, 2, 1 )
SET @byte03 = SUBSTRING( @uuidbin, 3, 1 )
SET @byte04 = SUBSTRING( @uuidbin, 4, 1 )
SET @byte05 = SUBSTRING( @uuidbin, 5, 1 )
SET @byte06 = SUBSTRING( @uuidbin, 6, 1 )
SET @byte07 = SUBSTRING( @uuidbin, 7, 1 )
SET @byte08 = SUBSTRING( @uuidbin, 8, 1 )
SET @byte09 = SUBSTRING( @uuidbin, 9, 1 )
SET @byte10 = SUBSTRING( @uuidbin, 10, 1 )
SET @byte11 = SUBSTRING( @uuidbin, 11, 1 )
SET @byte12 = SUBSTRING( @uuidbin, 12, 1 )
SET @byte13 = SUBSTRING( @uuidbin, 13, 1 )
SET @byte14 = SUBSTRING( @uuidbin, 14, 1 )
SET @byte15 = SUBSTRING( @uuidbin, 15, 1 )
SET @byte16 = SUBSTRING( @uuidbin, 16, 1 )

-- start from last 8 bytes
--------------------------------------------

SET @byte = @byte16 & ( 0x3F )
SET @pwd = dbo.udf_CodeToBase64( @byte )

SET @byte = @byte16 / POWER( 2, 6 )
SET @tmp = @byte15 * POWER( 2, 2 )
SET @byte2 = @tmp & 0x3F
SET @byte = @byte | @byte2
SET @pwd = @pwd + dbo.udf_CodeToBase64( @byte )

SET @byte = @byte15 / POWER( 2, 4 )
SET @tmp = @byte14 * POWER( 2, 4 )
SET @byte2 = @tmp & 0x3F
SET @byte = @byte | @byte2
SET @pwd = @pwd + dbo.udf_CodeToBase64( @byte )

SET @byte = @byte14 / POWER( 2, 2 )
SET @pwd = @pwd + dbo.udf_CodeToBase64( @byte )

--------------------------------------------

SET @byte = @byte13 & 0x3F
SET @pwd = @pwd + dbo.udf_CodeToBase64( @byte )

SET @byte = @byte13 / POWER( 2, 6 )
SET @tmp = @byte12 * POWER( 2, 2)
SET @byte2 = @tmp & 0x3F
SET @byte = @byte | @byte2
SET @pwd = @pwd + dbo.udf_CodeToBase64( @byte )

SET @byte = @byte12 / POWER( 2, 4 )
SET @tmp = @byte11 * POWER( 2, 4 )
SET @byte2 = @tmp & 0x3F
SET @byte = @byte | @byte2
SET @pwd = @pwd + dbo.udf_CodeToBase64( @byte )

SET @byte = @byte11 / POWER( 2, 2 )
SET @pwd = @pwd + dbo.udf_CodeToBase64( @byte )

---------------------------------------------

SET @byte = @byte10 & 0x3F
SET @pwd = @pwd + dbo.udf_CodeToBase64( @byte )

SET @byte = @byte10 / POWER( 2, 6 )
SET @tmp = @byte09 * POWER( 2, 2 )
SET @byte2 = @tmp & 0x3F
SET @byte = @byte | @byte2
SET @pwd = @pwd + dbo.udf_CodeToBase64( @byte )

SET @byte = @byte09 / POWER( 2, 4 )
SET @pwd = @pwd + dbo.udf_CodeToBase64( @byte )

-- end of last 8 bytes, start of first 8 bytes
---------------------------------------------

SET @byte = @byte08 & 0x3F
SET @pwd = @pwd + dbo.udf_CodeToBase64( @byte )

SET @byte = @byte08 / POWER( 2, 6 )
SET @tmp = @byte07 * POWER( 2, 2)
SET @byte2 = @tmp & 0x3F
SET @byte = @byte | @byte2
SET @pwd = @pwd + dbo.udf_CodeToBase64( @byte )

SET @byte = @byte07 / POWER( 2, 4 )
SET @tmp = @byte06 * POWER( 2, 4 )
SET @byte2 = @tmp & 0x3F
SET @byte = @byte | @byte2
SET @pwd = @pwd + dbo.udf_CodeToBase64( @byte )

SET @byte = @byte06 / POWER( 2, 2 )
SET @pwd = @pwd + dbo.udf_CodeToBase64( @byte )

---------------------------------------------

SET @byte = @byte05 & 0x3F
SET @pwd = @pwd + dbo.udf_CodeToBase64( @byte )

SET @byte = @byte05 / POWER( 2, 6 )
SET @tmp = @byte04 * POWER( 2, 2)
SET @byte2 = @tmp & 0x3F
SET @byte = @byte | @byte2
SET @pwd = @pwd + dbo.udf_CodeToBase64( @byte )

SET @byte = @byte04 / POWER( 2, 4 )
SET @tmp = @byte03 * POWER( 2, 4 )
SET @byte2 = @tmp & 0x3F
SET @byte = @byte | @byte2
SET @pwd = @pwd + dbo.udf_CodeToBase64( @byte )

SET @byte = @byte03 / POWER( 2, 2 )
SET @pwd = @pwd + dbo.udf_CodeToBase64( @byte )

---------------------------------------------

SET @byte = @byte02 & 0x3F
SET @pwd = @pwd + dbo.udf_CodeToBase64( @byte )

SET @byte = @byte02 / POWER( 2, 6 )
SET @tmp = @byte01 * POWER( 2, 2)
SET @byte2 = @tmp & 0x3F
SET @byte = @byte | @byte2
SET @pwd = @pwd + dbo.udf_CodeToBase64( @byte )

-- last byte
SET @byte = @byte01 / POWER( 2, 4 )
SET @pwd = @pwd + dbo.udf_CodeToBase64( @byte )

RETURN @pwd

END
GO
PRINT N'Creating [dbo].[GetLastTimeBreak]...';


GO
CREATE FUNCTION [dbo].[GetLastTimeBreak]
(
	@personId INT
)
RETURNS TABLE AS RETURN 
(
	SELECT TOP(1) *
	FROM BvTimeBreaksHistory
	WHERE InterviewerId = @personId
	ORDER BY StartTime DESC
)
GO
PRINT N'Creating [dbo].[GetCallsForGroupForPredictiveSurvey]...';


GO
CREATE FUNCTION dbo.GetCallsForGroupForPredictiveSurvey
(
    @rowCount AS INT,
    @SurveySid AS INT,
    @ObjectSid AS INT
)
RETURNS TABLE
AS RETURN(
          SELECT TOP (@rowCount) *
          FROM BvCachedCalls c
          WHERE SurveySid = @SurveySid AND
                ExplicitSID = @ObjectSid AND
                CallState = 2
          ORDER BY OrderId )
GO
PRINT N'Creating [dbo].[GetCallsForCacheTable]...';


GO
CREATE FUNCTION dbo.GetCallsForCacheTable
(   @rowCount AS INT, 
    @ExplicitSID AS INT,
    @SurveySid AS INT,
    @TimeToRun AS DATETIME) 
RETURNS TABLE
AS RETURN(
          SELECT TOP(@rowCount) [ID],
                                ExplicitSID,
								ExplicitType,
                                SurveySID,
                                InterviewID,
                                CallState,
								ApptId,
								TimeInShift,
								CallOrder,
								Priority
          FROM BvSvySchedule
          WHERE ( @SurveySid = 0 OR SurveySid = @SurveySid ) AND
                ExplicitSID = @ExplicitSID AND
                CallState = 2 AND
                TimeInShift <= @TimeToRun AND
                IsInActiveShiftType = 1 AND
                ConditionValue = 0
          ORDER BY Priority DESC,
                   TimeInShift,
                   SurveySID,
                   CallOrder )
GO
PRINT N'Creating [dbo].[udf_GetParentFilters]...';


GO
CREATE FUNCTION dbo.udf_GetParentFilters
(
    @FilterSid INT
)
RETURNS TABLE
AS RETURN(
    WITH ParentFilters AS(
		--initialization
		SELECT SID
		FROM BvFilters
		WHERE SID = @FilterSid
		
		UNION ALL
		
		--recursive execution
		SELECT bff.FilterSid
		FROM BvFilterFields bff 
		INNER JOIN ParentFilters pf ON CAST(bff.Value AS INT) = pf.SID
		WHERE bff.Sign = 8 --sub filter
	)
	SELECT DISTINCT * FROM ParentFilters
)
GO
PRINT N'Creating [dbo].[udf_GetSubFilters]...';


GO
CREATE FUNCTION dbo.udf_GetSubFilters
(
    @FilterSid INT
)
RETURNS TABLE
AS RETURN(
    WITH SubFilters AS(
		--initialization
		SELECT SID
		FROM BvFilters
		WHERE SID = @FilterSid
		
		UNION ALL
		
		--recursive execution
		SELECT CAST( CASE WHEN bff.Sign = 8 THEN [Value] ELSE 0 END AS INT ) SubFilterSid
		FROM BvFilterFields bff 
		INNER JOIN SubFilters sf ON bff.FilterSid = sf.SID
		WHERE bff.Sign = 8 --sub filter
	)
	SELECT DISTINCT * FROM SubFilters  
)
GO
PRINT N'Creating [dbo].[GetCallBySurvey]...';


GO
CREATE FUNCTION [dbo].[GetCallBySurvey]
(   
    @SurveySid INT,
    @ExplicitSID INT) 
RETURNS TABLE WITH SCHEMABINDING
AS RETURN(
          SELECT TOP(1) [ID],
                        ExplicitSID,
                        ExplicitType,
                        SurveySID,
                        InterviewID,
                        CallState,
                        ApptId,
                        TimeInShift,
                        CallOrder,
                        Priority,
                        ConditionValue
          FROM [dbo].BvSvySchedule
          WHERE IsInActiveShiftType = 1 AND
                    CallState = 2 AND
                    SurveySid = @SurveySid AND
                    BvSvySchedule.ExplicitSID = @ExplicitSID AND
                    ConditionValue <> 0
            ORDER BY Priority DESC, TimeInShift, ExplicitType DESC, CallOrder )
GO
PRINT N'Creating [dbo].[GetCallByCondition]...';


GO
CREATE FUNCTION [dbo].[GetCallByCondition]
(   @SurveySid INT,
	@ExplicitSID INT,
	@ConditionValue INT) 
RETURNS TABLE WITH SCHEMABINDING
AS RETURN(
		    SELECT TOP(1) [ID],
		                ExplicitSID,
						ExplicitType,
		                SurveySID,
		                InterviewID,
		                CallState,
						ApptId,
						TimeInShift,
						CallOrder,
						Priority,
						ConditionValue
		    FROM [dbo].BvSvySchedule
		    WHERE IsInActiveShiftType = 1 AND
				CallState = 2 AND
				SurveySid = @SurveySid AND
				BvSvySchedule.ExplicitSID = @ExplicitSID AND
				BvSvySchedule.ConditionValue  = @ConditionValue AND
				BvSvySchedule.ConditionValue <> 0
		ORDER BY Priority DESC, TimeInShift, ExplicitType DESC, CallOrder )
GO
PRINT N'Creating [dbo].[udf_AlertStatus_TAB_INT]...';


GO
CREATE FUNCTION dbo.udf_AlertStatus_TAB_INT
(
    @Value INT,
    @Amber INT,
    @Red INT
)
returns table
as return(
    SELECT ( CASE WHEN ((@Amber IS NULL) OR (@Red IS NULL)) THEN 0
                  WHEN ((@Red = @Amber) AND (@Value = @Red)) THEN 2
                  WHEN (@Red > @Amber) THEN (CASE WHEN (@Value >= @Red) THEN 2
                                                  WHEN (@Value >= @Amber) THEN 1
                                                  ELSE 0
                                             END)
                  WHEN (@Red < @Amber) THEN (CASE WHEN (@Value <= @RED) THEN 2
                                                  WHEN (@Value <= @Amber) THEN 1
                                                  ELSE 0
                                             END)
                  ELSE 0
             END ) AS val
)
GO
PRINT N'Creating [dbo].[udf_AlertStatus_TAB_DATETIME]...';


GO
CREATE FUNCTION dbo.udf_AlertStatus_TAB_DATETIME
(
    @Value DATETIME,
    @Now DATETIME,
    @Amber INT,
    @Red INT
)
returns table
as return(
    SELECT ( CASE WHEN ((@Amber IS NULL) OR (@Red IS NULL)) THEN 0
                  WHEN ((@Red = @Amber) AND (@Value = DATEADD(second, - @Red, @Now))) THEN 2
                  WHEN (@Red > @Amber) THEN (CASE WHEN (@Value <= DATEADD(second, - @Red, @Now)) THEN 2
                                                  WHEN (@Value <= DATEADD(second, - @Amber, @Now)) THEN 1
                                                  ELSE 0
                                             END)
                  WHEN (@Red < @Amber) THEN (CASE WHEN (@Value >= DATEADD(second, - @Red, @Now)) THEN 2
                                                  WHEN (@Value >= DATEADD(second, - @Amber, @Now)) THEN 1
                                                  ELSE 0
                                             END)
                  ELSE 0
             END ) AS val
)
GO
PRINT N'Creating [dbo].[utilSplitNumbers]...';


GO
CREATE FUNCTION [dbo].[utilSplitNumbers]
(
 @ItemList VARCHAR(max), 
 @delimiter CHAR(1)
)
RETURNS @IDTable TABLE (Item int)  
AS      

BEGIN    
 DECLARE @tempItemList VARCHAR(max)
 SET @tempItemList = @ItemList

 DECLARE @i INT    
 DECLARE @Item VARCHAR(20)

 SET @tempItemList = REPLACE (@tempItemList, ' ', '')
 SET @i = CHARINDEX(@delimiter, @tempItemList)

 WHILE (LEN(@tempItemList) > 0)
 BEGIN
  IF @i = 0
   SET @Item = @tempItemList
  ELSE
   SET @Item = LEFT(@tempItemList, @i - 1)
  INSERT INTO @IDTable(Item) VALUES(cast(@Item as int))
  IF @i = 0
   SET @tempItemList = ''
  ELSE
   SET @tempItemList = RIGHT(@tempItemList, LEN(@tempItemList) - @i)
  SET @i = CHARINDEX(@delimiter, @tempItemList)
 END 
 RETURN
END
GO
PRINT N'Creating [dbo].[vLogins]...';


GO
create view dbo.vLogins
with schemabinding
as
    select ObjectSID as sid, SurveySID, count_big(*) as cnt
        from dbo.BvLoginGroup
    group by ObjectSID, SurveySID
GO
PRINT N'Creating [dbo].[vLogins].[pk_vLogins]...';


GO
CREATE UNIQUE CLUSTERED INDEX [pk_vLogins]
    ON [dbo].[vLogins]([sid] ASC, [SurveySID] ASC);


GO
PRINT N'Creating [dbo].[BvViewPersonAndGroup]...';


GO
CREATE VIEW BvViewPersonAndGroup AS
    SELECT  SID, 
        Name, 
        0           IsGroup,
        FullName,
        Description
        FROM    BvPerson
    UNION
    SELECT  BvPersonGroup.SID, 
        Name, 
        1           IsGroup,
        ''          FullName,
        ''          Description
    FROM    BvPersonGroup
GO
PRINT N'Creating [dbo].[BvSpCall_Enable]...';


GO
CREATE PROCEDURE BvSpCall_Enable
	@SurveySID INT,
	@BatchID INT,
	@Enable BIT
AS
UPDATE BvSvySchedule SET CallState = CASE WHEN @Enable = 1 THEN 2 ELSE 1 END
	FROM BvTransferArrays ta
	WHERE	BvSvySchedule.SurveySID = @SurveySID AND
			BvSvySchedule.InterviewID = ta.ItemID AND
			ta.BatchID = @BatchID AND
			BvSvySchedule.CallState > 0
GO
PRINT N'Creating [dbo].[BvSpLookUpByPerson_ForManualMode]...';


GO
CREATE PROCEDURE [dbo].[BvSpLookUpByPerson_ForManualMode]
	@surveyId int,
	@interviewId int,
	@personId int
AS
    DECLARE @callId INT
    
    DECLARE @Call TABLE
	(
		CallID INT,
		ApptID INT,
		SurveySID INT,
		iid INT,
		CallState INT,
		ShiftID INT,
		Priority INT,
		TimeInShift DATETIME,
		TimeToExpire DATETIME,
		Resource INT,
		Resource_Type INT,
		RuleNumber UNIQUEIDENTIFIER,
		roleid INT	
	);

	;WITH call AS
	(
		SELECT BvCachedCalls.*
		FROM BvCachedCalls
		INNER JOIN BvPersonRel ON BvPersonRel.PersonSID = @personId
		WHERE CallState = 2 AND
		      SurveySid = @surveyId AND
		      InterviewId = @interviewId AND
			  BvPersonRel.ObjectSID = BvCachedCalls.ExplicitSID
	)
	UPDATE call
	SET CallState = -1,
		@callId = call.[ID]
		
		
	IF @callId IS NOT NULL 
	BEGIN
		;WITH call AS
		(
			SELECT BvSvySchedule.*
			FROM BvSvySchedule
			WHERE ID = @callId
		)
		UPDATE call
		SET CallState = -1
		OUTPUT
		   deleted.[ID] CallID,
		   deleted.ApptID,
		   deleted.SurveySID,
		   deleted.InterviewID iid,
		   deleted.CallState,
		   deleted.ShiftTypeID ShiftID,
		   deleted.Priority,
		   deleted.TimeInShift,
		   deleted.ExpireTime TimeToExpire,
		   deleted.ExplicitSID Resource,
		   deleted.ExplicitType Resource_Type,
		   deleted.RuleNumber,
		   2 roleid	
		INTO @Call;
	END
	ELSE
	BEGIN
		DECLARE @PersonAssignmentsListMode INT;
		SELECT @PersonAssignmentsListMode = AssignmentsListMode FROM BvPerson WHERE SID = @personId
		;WITH call AS
		(
			SELECT BvSvySchedule.*
			FROM BvSvySchedule
			LEFT JOIN BvPersonRel ON BvPersonRel.PersonSID = @personId
			WHERE CallState = 2 AND
				  SurveySid = @surveyId AND
				  InterviewId = @interviewId AND
				  (@PersonAssignmentsListMode = 1 OR BvPersonRel.ObjectSID = BvSvySchedule.ExplicitSID)
		)
		UPDATE call
		SET CallState = -1,
			@callId = call.[ID]
		OUTPUT
		   deleted.[ID] CallID,
		   deleted.ApptID,
		   deleted.SurveySID,
		   deleted.InterviewID iid,
		   deleted.CallState,
		   deleted.ShiftTypeID ShiftID,
		   deleted.Priority,
		   deleted.TimeInShift,
		   deleted.ExpireTime TimeToExpire,
		   deleted.ExplicitSID Resource,
		   deleted.ExplicitType Resource_Type,
		   deleted.RuleNumber,
		   2 roleid	
		   INTO @Call
	END  
	
	UPDATE BvAppointment 
	SET State = 2 
	WHERE State = 1 AND 
	      SurveysId = @surveyId AND 
	      InterviewSid = @interviewId
	      
	SELECT * FROM @Call
RETURN 0
GO
PRINT N'Creating [dbo].[BvSpStartInterviewerBreak]...';


GO
CREATE  PROCEDURE [dbo].[BvSpStartInterviewerBreak]
    @InterviewerId INT    
AS
BEGIN
    UPDATE BvTasks
    SET StartTime = NULL
    WHERE PersonSid = @InterviewerId
    
	INSERT INTO BvTimeBreaksHistory (InterviewerId, StartTime) VALUES (@InterviewerId, GETUTCDATE())
END
GO
PRINT N'Creating [dbo].[BvSpCalls_Delete_Batch]...';


GO
CREATE PROCEDURE [dbo].[BvSpCalls_Delete_Batch]
	@surveySid INT,
	@batchId INT
AS    
 DECLARE @InterviewIds TABLE(Id INT)
    
 INSERT INTO @InterviewIds
 SELECT ItemID
 FROM BvTransferArrays ta
 WHERE BatchId = @batchID 
    
 -- Delete calls
 DELETE FROM BvCachedCalls 
 FROM @InterviewIds iids
 WHERE SurveySID = @SurveySID AND
       iids.Id = InterviewId
       
 UPDATE BvSvySchedule 
 SET CallState = 0
 FROM @InterviewIds iids
 WHERE SurveySID = @SurveySID AND
       iids.ID = InterviewId
GO
PRINT N'Creating [dbo].[BvSpInterviews_UpdateState_Batch]...';


GO
CREATE PROCEDURE [dbo].[BvSpInterviews_UpdateState_Batch]
@SurveySID INT, @BatchID INT, @StateID INT
AS
UPDATE BvInterview
   SET TransientState = @StateID 
   FROM BvInterview i
   INNER JOIN BvTransferArrays ta ON 
   i.ID = ta.ItemID AND
   i.SurveySID = @SurveySID AND
   ta.BatchID = @BatchID
GO
PRINT N'Creating [dbo].[BvSpScheduleParam_Prepare]...';


GO
CREATE PROCEDURE [dbo].[BvSpScheduleParam_Prepare]
	@ParamBatchID INT,
	@ParamID INT,
    @Name NVARCHAR(256),
	@Description NVARCHAR(MAX),
    @Type INT,
    @Value INT 
AS
    IF @ParamBatchID <= 0 
	BEGIN
		RAISERROR( '@ParamBatchID should be > 0 ', 16, 1 )
	END

	INSERT INTO BvScheduleParam( 
		ScheduleID, 
		SurveySID, 
		ParamID, 
		[Name], 
		Description, 
		Type, 
		Value ) 
    VALUES( 
		-@ParamBatchID, 
		0, 
		@ParamID, 
		@Name, 
		@Description, 
		@Type, 
		@Value )

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpScheduleParam_Launch]...';


GO
CREATE PROCEDURE [dbo].[BvSpScheduleParam_Launch]
	@ScheduleID INT,
	@ParamBatchID INT
AS
	DELETE FROM BvScheduleParam WHERE ScheduleID = @ScheduleID
	
	UPDATE BvScheduleParam SET ScheduleID = @ScheduleID WHERE ScheduleID = -@ParamBatchID
	
	INSERT INTO BvScheduleParam( ScheduleID, SurveySID, ParamID, Name, Description, Type, Value )  
		SELECT sp.ScheduleID, s.SID, sp.ParamID, sp.Name, sp.Description, sp.Type, sp.Value
                        FROM BvScheduleParam sp 
                INNER JOIN BvSurvey s 
                ON sp.ScheduleID = s.ScheduleID AND sp.ScheduleID = @ScheduleID
				WHERE s.State <> 2
                
RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpScheduleParam_Get]...';


GO
CREATE PROCEDURE [dbo].[BvSpScheduleParam_Get]
	@SurveySID INT,
	@ParamID INT
AS
	SELECT Value 
		FROM BvScheduleParam 
		WHERE	SurveySID = @SurveySID AND 
				ParamID = @ParamID 
RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpTask_UpdateActiveQuestion]...';


GO
CREATE PROCEDURE BvSpTask_UpdateActiveQuestion
 @projectId NVARCHAR(256),
 @catiInterviewerId INT,
 @qID NVARCHAR(256),
 @showTime DATETIME
AS

BEGIN TRY
    --Answer submission alert thresholds
    DECLARE @AmberOfAnswerSubmissionAlert INT
    DECLARE @RedOfAnswerSubmissionAlert INT
    SELECT @AmberOfAnswerSubmissionAlert = Amber, @RedOfAnswerSubmissionAlert = Red
    FROM BvThresholds 
    WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 1/*Task alert*/


    --Quick answer submission alert thresholds
    DECLARE @AmberOfQuickAnswerSubmissionAlert INT
    DECLARE @RedOfQuickAnswerSubmissionAlert INT
    SELECT @AmberOfQuickAnswerSubmissionAlert = Amber, @RedOfQuickAnswerSubmissionAlert = Red
    FROM BvThresholds 
    WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 17/*QuickAnswerSubmission alert*/


    DECLARE @AnswerDuration INT
    DECLARE @SubmissionTime DateTime
    DECLARE @surveyId INT
    DECLARE @interviewId INT
    DECLARE @personId INT
    DECLARE @questionId NVARCHAR(256)
    DECLARE @InterviewState TINYINT
    
    DECLARE @IsIncorrectOrder BIT = 0   --if previous question come in later.
   
    SET LOCK_TIMEOUT 500
    UPDATE BvTasks
      SET @IsIncorrectOrder = (CASE WHEN [TimeStateChanged] > @showTime THEN 1 ELSE 0 END),
        [State] = (CASE WHEN @IsIncorrectOrder = 1 THEN [State] ELSE @qID END),
        [TimeStateChanged] = (CASE WHEN @IsIncorrectOrder = 1 THEN [TimeStateChanged] ELSE @showTime END),
        [SecondsSinceLastSubmission] = 0,  --let recalculate sp set correct value
        [LastSubmissionAlert] = 0,         --let recalculate sp set correct value
        
        @AnswerDuration = DATEDIFF(s, TimeStateChanged, @showTime),   --in this case TimeStateChanged will be previous value not @showTime
        @SubmissionTime = TimeStateChanged,
        @surveyId = surveySid,
        @interviewId = interviewId,
        @personId = PersonSID,
        @questionId = State,
        @InterviewState = InterviewState
    WHERE PersonSID = @catiInterviewerId
    SET LOCK_TIMEOUT -1
                       
    IF @questionId IS NULL --first question
    BEGIN
       RETURN
    END
    
    IF @IsIncorrectOrder = 1
    BEGIN
       SET @questionId = @qID
       SET @AnswerDuration = -@AnswerDuration
       SET @SubmissionTime = @showTime
    END
                       
    DECLARE @AnswerSubmissionAlert BIT
    DECLARE @QuickAnswerSubmissionAlert BIT

    if ( @AnswerDuration >= @AmberOfAnswerSubmissionAlert  )
       SET @AnswerSubmissionAlert = 0
    if ( @AnswerDuration >= @RedOfAnswerSubmissionAlert  )
       SET @AnswerSubmissionAlert = 1
    if ( @AnswerDuration <= @AmberOfQuickAnswerSubmissionAlert )
       SET @QuickAnswerSubmissionAlert = 0
    if ( @AnswerDuration <= @RedOfQuickAnswerSubmissionAlert  )
       SET @QuickAnswerSubmissionAlert = 1


    if((@QuickAnswerSubmissionAlert IS NOT NULL OR @AnswerSubmissionAlert IS NOT NULL) AND @questionId != 'Internal_Stop')
    BEGIN
        INSERT INTO BvAnswerSubmissionAlertHistory
        VALUES(@personId, @SubmissionTime, @questionId, @surveyId, @interviewId, @AnswerDuration, @AnswerSubmissionAlert, @QuickAnswerSubmissionAlert, @InterviewState)
    END
END TRY
BEGIN CATCH
END CATCH;
GO
PRINT N'Creating [dbo].[BvSpTasks_UpdateStartTime]...';


GO
CREATE PROCEDURE [dbo].[BvSpTasks_UpdateStartTime]
	@personSid INT
AS
SET XACT_ABORT ON

	UPDATE [dbo].[BvTasks]
	SET StartTime = GETUTCDATE()
	WHERE PersonSID = @personSid AND
	      StartTime IS NULL
	
RETURN 0
GO
PRINT N'Creating [dbo].[BvSpTasks_InsertAnswerSubmissionAlertIfNeeded]...';


GO
CREATE PROCEDURE [dbo].[BvSpTasks_InsertAnswerSubmissionAlertIfNeeded]
	@PersonSid int
AS
    --Answer submission alert thresholds
    DECLARE @AmberOfAnswerSubmissionAlert INT
    DECLARE @RedOfAnswerSubmissionAlert INT
    
    SELECT 
        @AmberOfAnswerSubmissionAlert = Amber,
        @RedOfAnswerSubmissionAlert = Red
    FROM 
        BvThresholds 
    WHERE 
        ObjectSID = 0 /*Default value*/ AND 
        ThresholdsTypeID = 1/*Task alert*/
    
    DECLARE @AnswerDuration INT
    DECLARE @SubmissionTime DateTime
    DECLARE @surveyId INT
    DECLARE @interviewId INT
    DECLARE @personId INT
    DECLARE @questionId NVARCHAR(256)
    DECLARE @InterviewState TINYINT
    
    SELECT 
        @AnswerDuration = DATEDIFF(s, TimeStateChanged, GETUTCDATE()),   --in this case TimeStateChanged will be previous value not @showTime
        @SubmissionTime = TimeStateChanged,
        @surveyId = surveySid,
        @interviewId = interviewId,
        @personId = PersonSID,
        @questionId = State,
        @InterviewState = InterviewState
    FROM
        BvTasks
    WHERE
        PersonSID = @PersonSid
    
    IF @questionId IS NULL
    BEGIN
       RETURN
    END
    
    DECLARE @AnswerSubmissionAlert BIT

    if ( @AnswerDuration >= @AmberOfAnswerSubmissionAlert  )
       SET @AnswerSubmissionAlert = 0
    if ( @AnswerDuration >= @RedOfAnswerSubmissionAlert  )
       SET @AnswerSubmissionAlert = 1


    if(@AnswerSubmissionAlert IS NOT NULL)
    BEGIN
        INSERT INTO BvAnswerSubmissionAlertHistory
        VALUES(@personId, @SubmissionTime, @questionId, @surveyId, @interviewId, @AnswerDuration, @AnswerSubmissionAlert, NULL, @InterviewState)
    END
    
RETURN 0
GO
PRINT N'Creating [dbo].[BvSpSurveyProductivityReportCati]...';


GO
CREATE PROCEDURE [dbo].[BvSpSurveyProductivityReportCati]
	@SurveySids NVARCHAR(MAX), 
    @PersonSIDs NVARCHAR (MAX), 
    @ITS NVARCHAR (MAX), 
    @StartDate DATETIME, 
    @EndDate DATETIME
AS
IF(@SurveySids IS NULL AND 
   @PersonSIDs IS NULL AND
   @ITS IS NULL AND
   @StartDate IS NULL AND
   @EndDate IS NULL)
BEGIN
   SELECT 0 AS [PersonSID],
          '' AS [PersonCode],           
		  '' AS [PersonName],
		  0 AS [SurveySID],
          '' AS [SurveyCode],
          '' AS [SurveyName],
		  cast(0 as tinyint)  AS [StateID],
		  '' AS [StateName],
		  0 AS [InterviewCount],
		  0 AS [TotalInterviewCount],
          0 AS [InterviewTime]
          
   RETURN 0;
END
    
          
          
DECLARE @DefaultStateGroupID INTEGER
SELECT top(1) @DefaultStateGroupID = ID 
FROM BvStateGroup 
ORDER BY [Order] ASC;
 
CREATE TABLE #surveySids([SurveyId] int primary key, [SurveyCode] nvarchar(max), [Description] nvarchar(max))
insert into #surveySids 
SELECT [SID] AS [SurveyId],
          [Name] AS [SurveyCode],
          [Description]
FROM dbo.utilSplitNumbers( ISNULL(@SurveySids, ''), ',')
INNER JOIN BvSurvey ON SID = Item

CREATE TABLE #SelectedStatuses([StateID] tinyint primary key, [StateName] nvarchar(max))
insert into #SelectedStatuses
SELECT [s].[StateID],
       [s].[Name] [StateName]
FROM [BvState] [s]
LEFT JOIN dbo.utilSplitNumbers(ISNULL(@ITS, ''), ',') ON [s].[StateID] = [Item]
WHERE (@ITS IS NULL OR [Item] IS NOT NULL) AND [s].[StateGroupID] = @DefaultStateGroupID 
         
create table #persons(sid int primary key, [PersonCode] nvarchar(max), [PersonName] nvarchar(max))
insert into #persons
SELECT SID, 
       CAST([SID] AS NVARCHAR(MAX)) [PersonCode],
       [Name] [PersonName]
FROM BvPerson
LEFT JOIN dbo.utilSplitNumbers(ISNULL(@PersonSIDs, ''), ',') ON [SID] = [Item]
WHERE @PersonSIDs IS NULL OR [Item] IS NOT NULL AND EXISTS
  (
     SELECT * 
     FROM [BvPersonRel] 
     WHERE SID = PersonSid AND RoleId = 2
  )

;WITH BvHistory_CTE AS
(
   SELECT [history].*, [SurveyCode], [Description]
   FROM #surveySids [survey] 
   INNER JOIN [BvHistory] [history] ON [survey].[SurveyId] = [history].[SurveyId]
   WHERE [history].[FiredTime] BETWEEN @StartDate AND @EndDate AND
         [history].[RoleID] = 2
),
BvHistoryWithStates_CTE AS
(
   SELECT [history].*, [state].*
   FROM BvHistory_CTE [history]
   INNER JOIN #SelectedStatuses [state] ON [state].[StateID] = [history].[ITS]
)
 
SELECT 
 [person].[SID] AS [PersonSID],
 [person].[PersonCode],           
 [person].[PersonName],
                
 [history].[SurveyId] AS [SurveySID],
 [history].[SurveyCode] AS [SurveyCode],
 [history].[Description] AS [SurveyName],

 [history].[StateID] AS [StateID],
 [history].[StateName],
 
 COUNT(*) AS [InterviewCount], /* Interview count for status. */
    
 /* Total calls count for the selected person and survey (regardless to status). */
 (SELECT COUNT(*) 
  FROM [BvHistory_CTE] [h1] 
  WHERE [person].[SID] = [h1].[PersonSID] AND
        [history].[SurveyId] = [h1].[SurveyId] AND
        [h1].[ITS] IS NOT NULL ) AS [TotalInterviewCount],

 
 ISNULL(SUM([history].[Duration]), 0) AS [InterviewTime] /* Interview time in seconds. */

 FROM #persons [person]
 INNER JOIN BvHistoryWithStates_CTE [history] ON [history].[PersonSID] = [person].[SID]

 GROUP BY   [history].[SurveyId],
            [history].[SurveyCode],
            [history].[Description],
            [history].[StateId], 
            [history].[StateName],
            [person].[SID], 
            [person].[PersonCode], 
            [person].[PersonName]
 
 ORDER BY [person].[PersonCode], [history].[StateId]

RETURN 0
GO
PRINT N'Creating [dbo].[BvSpSurveyProductivityReportCapi]...';


GO
CREATE PROCEDURE [dbo].[BvSpSurveyProductivityReportCapi]
	@SurveySids NVARCHAR(MAX), 
    @PersonSIDs NVARCHAR (MAX), 
    @ITS NVARCHAR (MAX), 
    @StartDate DATETIME, 
    @EndDate DATETIME
AS

IF(@SurveySids IS NULL AND 
   @PersonSIDs IS NULL AND
   @ITS IS NULL AND
   @StartDate IS NULL AND
   @EndDate IS NULL)
BEGIN
   SELECT 0 AS [PersonSID],
          '' AS [PersonCode],           
		  '' AS [PersonName],
		  0 AS [SurveySID],
          '' AS [SurveyCode],
          '' AS [SurveyName],
		  cast(0 as tinyint)  AS [StateID],
		  '' AS [StateName],
		  0 AS [InterviewCount],
		  0 AS [TotalInterviewCount],
          0 AS [InterviewTime]
   RETURN 0;
END

DECLARE @DefaultStateGroupID INTEGER
SELECT top(1) @DefaultStateGroupID = ID 
FROM BvStateGroup 
ORDER BY [Order] ASC;
 
CREATE TABLE #surveySids([SurveyId] int primary key, [SurveyCode] nvarchar(max), [Description] nvarchar(max))
insert into #surveySids 
SELECT [SID] AS [SurveyId],
          [Name] AS [SurveyCode],
          [Description]
FROM dbo.utilSplitNumbers( ISNULL(@SurveySids, ''), ',')
INNER JOIN BvSurvey ON SID = Item

CREATE TABLE #SelectedStatuses([StateID] tinyint primary key, [StateName] nvarchar(max))
insert into #SelectedStatuses
SELECT [s].[StateID],
       [cs].[StatusName_Cnf] [StateName]
FROM [BvState] [s]
INNER JOIN [BvConfirmitStatus] [cs] ON [cs].[StatusCode_BvFEE] = [s].[StateId]
LEFT JOIN dbo.utilSplitNumbers(ISNULL(@ITS, ''), ',') ON [s].[StateID] = [Item]
WHERE (@ITS IS NULL OR [Item] IS NOT NULL) AND
     (
           CAST([cs].[StatusCode_BvFEE] AS NVARCHAR(MAX)) != [cs].[StatusCode_Cnf] 
           OR 
           [cs].[StatusCode_Cnf] is NULL 
     ) AND [s].[StateGroupID] = @DefaultStateGroupID 
         
create table #persons(sid int primary key, [PersonCode] nvarchar(max), [PersonName] nvarchar(max))
insert into #persons
SELECT SID, 
       [Name] [PersonCode],
       [Description] [PersonName]
FROM BvPerson
LEFT JOIN dbo.utilSplitNumbers(ISNULL(@PersonSIDs, ''), ',') ON [SID] = [Item]
WHERE @PersonSIDs IS NULL OR [Item] IS NOT NULL and exists
  (
     select * from  [BvPersonRel] where SID = PersonSid AND RoleId = 64
  )

;WITH BvHistory_CTE AS
(
   SELECT [history].*, [SurveyCode], [Description]
   FROM #surveySids [survey] 
   INNER JOIN [BvHistory] [history] ON [survey].[SurveyId] = [history].[SurveyId]
   WHERE [history].[FiredTime] BETWEEN @StartDate AND @EndDate AND
         [history].[RoleID] = 64
),
BvHistoryWithStates_CTE AS
(
   SELECT [history].*, [state].*
   FROM BvHistory_CTE [history]
   INNER JOIN #SelectedStatuses [state] ON [state].[StateID] = [history].[ITS]
)
 
SELECT 
 [person].[SID] AS [PersonSID],
 [person].[PersonCode],           
 [person].[PersonName],
                
 [history].[SurveyId] AS [SurveySID],
 [history].[SurveyCode] AS [SurveyCode],
 [history].[Description] AS [SurveyName],

 [history].[StateID] AS [StateID],
 [history].[StateName],
 
 COUNT(*) AS [InterviewCount], /* Interview count for status. */
    
 /* Total calls count for the selected person and survey (regardless to status). */
 (SELECT COUNT(*) 
  FROM [BvHistory_CTE] [h1] 
  WHERE [person].[SID] = [h1].[PersonSID] AND
        [history].[SurveyId] = [h1].[SurveyId] AND
        [h1].[ITS] IS NOT NULL ) AS [TotalInterviewCount],

 
 ISNULL(SUM([history].[Duration]), 0) AS [InterviewTime] /* Interview time in seconds. */

 FROM #persons [person]
 INNER JOIN BvHistoryWithStates_CTE [history] ON [history].[PersonSID] = [person].[SID]

 GROUP BY   [history].[SurveyId],
            [history].[SurveyCode],
            [history].[Description],
            [history].[StateId], 
            [history].[StateName],
            [person].[SID], 
            [person].[PersonCode], 
            [person].[PersonName]
 
 ORDER BY [person].[PersonCode], [history].[StateId]

RETURN 0
GO
PRINT N'Creating [dbo].[BvSpInterviewsAndAppointments_Delete_Batch]...';


GO
CREATE PROCEDURE [dbo].[BvSpInterviewsAndAppointments_Delete_Batch]
@surveySid INT, @batchId INT
AS
-- Delete appointments

 DELETE BvAppointment
 FROM BvTransferArrays
 WHERE SurveySID = @SurveySID AND
       BvTransferArrays.BatchId = @batchId AND
       ItemId = BvAppointment.InterviewSID

-- Delete interviews
 DELETE BvInterview 
 FROM BvTransferArrays
 WHERE SurveySID = @surveySid AND
       BvTransferArrays.BatchId = @batchId AND
       ID = ItemID
GO
PRINT N'Creating [dbo].[BvSpSurveyOverviewReport]...';


GO
CREATE PROCEDURE [dbo].[BvSpSurveyOverviewReport]
 @SurveySids NVARCHAR (MAX),
 @PersonSids NVARCHAR(MAX),
 @CompletedItses NVARCHAR(MAX),
 @UseDialer BIT,
 @HideEmpty BIT,
 @StartDateTime DATETIME,
 @EndDateTime DATETIME
 
 WITH RECOMPILE
AS 
 SET @StartDateTime = ISNULL(@StartDateTime, '1753-01-01T00:00:00.000')
 SET @EndDateTime = ISNULL(@EndDateTime, '9999-12-31T23:59:59.997');

 WITH Persons AS
 (
  SELECT p.SID AS PersonSid
  FROM dbo.utilSplitNumbers( ISNULL(@PersonSids, ''), ',') s
  INNER JOIN BvPerson p ON p.SID = s.Item

  UNION 

  SELECT p.Sid AS PersonSid
  FROM BvPerson p
  WHERE @PersonSids IS NULL

  UNION

  SELECT DialerSid AS PersonSid
  FROM (SELECT 0 AS DialerSid) dailerSids
  WHERE @UseDialer = 1
 ),
 CompletedItsList AS
 (
  SELECT Item AS CompletedIts 
  FROM dbo.utilSplitNumbers( ISNULL(@CompletedItses, ''), ',')
 ),
 
 Surveys AS
 (
  SELECT 
	s.SID AS SurveyId ,
	s.Name AS ProjectId,
	s.Description AS Title
  FROM dbo.utilSplitNumbers( ISNULL(@SurveySids, ''), ',') ss
  INNER JOIN BvSurvey s ON s.SID = ss.Item
 )
 
 SELECT
  s.SurveyId AS SurveyId,
  s.ProjectId AS ProjectId,
  s.Title AS Title,
  
  (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0)) AS LogOnTime,
  ISNULL(SUM(h.WaitingTime), 0) AS WaitingTime,
  COUNT(h.InterviewID) AS DialingsCount, --there is can be dummy record where Hst_Path1 is null (it is used when user was logged in but didn't get any calls)
  ISNULL(SUM(cil.CompletedIts/cil.CompletedIts), 0) AS Completes,
  ISNULL(AVG(CASE WHEN cil.CompletedIts IS NOT NULL THEN h.Duration ELSE NULL END), 0) AS AverageCompletedInterviewDuration
  FROM Surveys s 
    LEFT JOIN BvHistory h 
        ON s.SurveyId = h.SurveyId AND
           h.FiredTime >= @StartDateTime AND
           h.FiredTime <= @EndDateTime AND
           h.RoleID = 2 --we should not calced calls whuch were added during sample addition
        AND h.PersonSID IN (SELECT p.PersonSid FROM Persons p)
    LEFT JOIN CompletedItsList cil ON cil.CompletedIts = h.ITS
    GROUP BY s.SurveyId, s.ProjectId, s.Title
    HAVING (@HideEmpty = 0 OR (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0)) > 0)
GO
PRINT N'Creating [dbo].[BvSpSetCallDeliveryMode]...';


GO
CREATE PROCEDURE [dbo].[BvSpSetCallDeliveryMode]
    @SurveyId INT,
	@Mode BIT -- 0 - order by interview id, 1 - random order
AS
    DECLARE @PreviosMode BIT
    
	UPDATE BvSurvey
	SET IsRandomCallDeliveryEnabled = @Mode,
	    @PreviosMode = IsRandomCallDeliveryEnabled
	WHERE SID = @SurveyId
	
	IF @PreviosMode != @Mode
	BEGIN
	    UPDATE BvSvySchedule
	    SET CallOrder = CASE WHEN @Mode = 0 THEN InterviewId
	                         ELSE dbo.GetRandomValue(ID)
	                    END
	    WHERE SurveySid = @SurveyId
	END
RETURN 0
GO
PRINT N'Creating [dbo].[BvSpScheduleParam_Set]...';


GO
CREATE PROCEDURE [dbo].[BvSpScheduleParam_Set]
	@SurveySID INT,
	@ParamID INT,
	@Value INT
AS
	UPDATE BvScheduleParam
		SET Value = @Value
		WHERE	SurveySID = @SurveySID AND 
				ParamID = @ParamID 
				
	IF @@ROWCOUNT = 0 
		RAISERROR( 'Custom parameter with id = %d for survey with SID = %d not found', 12, 1, @ParamID, @SurveySID )
RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpScheduleParam_ResetParam]...';


GO
CREATE PROCEDURE [dbo].[BvSpScheduleParam_ResetParam] 
 @SurveySID INT
AS
 UPDATE s 
  SET s.Value = d.Value
  FROM BvScheduleParam d
  INNER JOIN BvScheduleParam s
  ON d.ScheduleID = s.ScheduleID AND s.ParamID = d.ParamID AND d.SurveySID = 0 AND s.SurveySID = @SurveySID
 
 RETURN 0
GO
PRINT N'Creating [dbo].[BvSpRemoveExpiredCalls]...';


GO
CREATE PROCEDURE [dbo].[BvSpRemoveExpiredCalls]
	@NowUTC           datetime
AS
    INSERT INTO BvCallExpired
    SELECT SurveySID,
           InterviewID,
           CallState
    FROM BvSvySchedule
    WHERE ExpireTime < @NowUTC AND 
          CallState > 0   --we shouldn't touch call with CallState = -3 (added during sample addition)
                      --call which are in interviewing process CallState = -1
                      --TODO:
                      --But we should process call from dialler correctly (CallState = -2)
 
    IF @@ROWCOUNT > 0 
    BEGIN
        -- update appointment status if call expired
        UPDATE BvAppointment
        SET State = 2
        FROM BvCallExpired
        WHERE BvAppointment.SurveySID = BvCallExpired.surveyID AND
			  BvAppointment.InterviewSID = BvCallExpired.interviewID
			  
	    --DELETE FROM BvCachedCalls
        UPDATE BvCachedCalls
		SET CallState = -1
        FROM BvCachedCalls cc
	INNER JOIN  BvCallExpired ce
        ON ce.surveyID = cc.SurveySID AND
           ce.interviewID = cc.InterviewID 
        WHERE cc.CallState > 0
        
        --DELETE FROM BvSvySchedule 
        UPDATE BvSvySchedule
        SET CallState = -1
        WHERE ExpireTime < @NowUTC AND 
              CallState > 0 --TODO:
                        --WE should correct process call with CallState = -2
    END
RETURN 0
GO
PRINT N'Creating [dbo].[BvSpNumberOfAttemptsReport]...';


GO
CREATE PROCEDURE [dbo].[BvSpNumberOfAttemptsReport]
   @SurveySid INT,
   @StartDateTime DATETIME,
   @EndDateTime DATETIME,
   @TotalSampleSize INT OUTPUT
AS
	SET @StartDateTime = ISNULL(@StartDateTime, '1753-01-01T00:00:00.000')
	SET @EndDateTime = ISNULL(@EndDateTime, '9999-12-31T23:59:59.997');

   IF @SurveySid IS NULL AND @StartDateTime IS NULL AND @EndDateTime IS NULL AND @TotalSampleSize IS NULL
   BEGIN
      SELECT 0 as Attempts, 0 as Records
    
      RETURN 0
   END

   --1) should we check state here?
   --2) should we that time is necessary for sample here?
   SELECT @TotalSampleSize = SUM(CountInterviews)
   FROM BvSamples
   WHERE SurveySID = @SurveySid;
   
   CREATE TABLE #temp( Attempts INT, Records INT);

   WITH NotEmptyAttempts AS
   (
      SELECT COUNT(*) AS Attempts, 
             1 AS InterviewCount 
      FROM BvHistory h
      WHERE h.SurveyId = @SurveySid AND
            h.RoleID = 2 AND --don't calc sample calls
            h.FiredTime BETWEEN @StartDateTime AND @EndDateTime AND
            h.InterviewId IS NOT NULL
      GROUP BY h.InterviewId
   ),
   NotEmptyOutputList AS
   (
	   SELECT nea.Attempts AS Attempts,
			  COUNT(nea.InterviewCount) AS Records
	   FROM NotEmptyAttempts nea
	   GROUP BY nea.Attempts
   )
   INSERT INTO #temp
   SELECT neol.Attempts Attempts,
          neol.Records Records
   FROM NotEmptyOutputList neol;
   
   WITH AllAttempts AS
   (
      SELECT MAX(Attempts) AS Attempts
      FROM #temp
      
      UNION ALL
      
      SELECT Attempts-1
      FROM AllAttempts
      WHERE Attempts > 1
   )
   SELECT aa.Attempts,
          ISNULL(t.Records, 0) Records
   FROM AllAttempts aa
   LEFT JOIN #temp t ON t.Attempts = aa.Attempts
   WHERE aa.Attempts IS NOT NULL
   OPTION (MAXRECURSION 500)
GO
PRINT N'Creating [dbo].[BvSpReleaseAppLock]...';


GO
CREATE PROCEDURE [dbo].[BvSpReleaseAppLock]
	@ResourceName NVARCHAR(255),
	@Succesfull BIT --if some errors was occured then last execution for lock is not changed
AS
	DECLARE @ReturnValue INT = 0
	
	IF @Succesfull = 1
		UPDATE BvAppLocks
		SET TimeLockLeave = GETUTCDATE(),
			IsLockHeld = 0
		WHERE ResourceName = @ResourceName
	
    EXEC @ReturnValue = sp_releaseapplock @ResourceName, N'Session'
	
RETURN @ReturnValue
GO
PRINT N'Creating [dbo].[BvSpPromoteCalls]...';


GO
CREATE PROCEDURE [dbo].[BvSpPromoteCalls]
	@surveyId INT,
	@quotaId INT,
	@cellId INT,
	@promotionPriority INT,
	@promotionCount INT,
	@promotionTime DATETIME
AS
    DECLARE @WhereCondition NVARCHAR(MAX)
    exec BvClr_QuotaService_GetWhereForFilteredCell @surveyId, @quotaId, @cellId, 'repl', @WhereCondition OUTPUT

    DECLARE @sql NVARCHAR(MAX) = '
	WITH PromotedRespID AS
	(
	   SELECT respId
	   FROM BvReplicatedData_' + CAST(@surveyId AS NVARCHAR(255)) + ' AS repl
	   WHERE (' + @WhereCondition + ')
	),
	PromotedCalls AS
	(
	   SELECT TOP(@promotionCoun)  BvSvySchedule.*
	   FROM PromotedRespID
	   INNER JOIN BvSvySchedule ON SurveySID = @surveyId AND respId = InterviewID
	   WHERE TimeInShift <= @promotionTime AND
	         Priority <= @promotionPriority AND
	         CallState > 0
	   ORDER BY Priority DESC,
                TimeInShift,
                SurveySID,
                CallOrder
	)
	UPDATE PromotedCalls
	SET OldPriority = CASE WHEN OldPriority > 0 THEN OldPriority ELSE Priority END, 
	    Priority = @promotionPriority
	'
	
	DECLARE @sqlQueryParams NVARCHAR(MAX) = N'@surveyId INT, @promotionTime DATETIME, @promotionPriority INT, @promotionCoun INT';
	
	EXEC sp_executesql @sql, @sqlQueryParams, @surveyId, @promotionTime, @promotionPriority, @promotionCount
	RETURN @@ROWCOUNT
GO
PRINT N'Creating [dbo].[BvSpPersonGroup_GetParentGroupForSpecificRole]...';


GO
CREATE PROCEDURE [dbo].[BvSpPersonGroup_GetParentGroupForSpecificRole]
	@Role int  --2 CATI, 64 - CAPI
AS
	SELECT pg.SID
	FROM BvPersonGroup pg
	LEFT JOIN BvMembership m ON pg.Sid = m.ObjectSID
	WHERE pg.RoleID = @Role AND
	      m.ObjectSID IS NULL
RETURN 0
GO
PRINT N'Creating [dbo].[BvSpLookUpByPerson]...';


GO
CREATE PROCEDURE [dbo].[BvSpLookUpByPerson]
	@personId int
AS
    DECLARE @interviewId INT
    DECLARE @rowCount INT
    DECLARE @surveyId INT
    
    ;WITH calls AS
	(
		SELECT TOP(1) BvCachedCalls.*
		FROM BvCachedCalls
		INNER JOIN BvPersonRel ON BvPersonRel.PersonSID = @personId
		WHERE CallState = 2 AND
		      BvPersonRel.ObjectSID = BvCachedCalls.ExplicitSID AND
			  SurveySID NOT IN ( SELECT SID  /* In BvCachedCalls we have calls for only open surveys */
                             FROM BvSurvey WHERE DialMode =  4 /*PREDICTIVE MODE*/ )
		ORDER BY OrderId
	)
	UPDATE calls
	SET CallState = -1,
		@interviewId = InterviewID,
		@surveyId = SurveySid
	OUTPUT
	   deleted.[ID] CallID,
	   deleted.SurveySID,
	   deleted.InterviewID iid
	
	SET @rowCount = @@ROWCOUNT
	
	IF(@rowCount = 0) RETURN 0
	
	UPDATE BvSvySchedule
	SET CallState = -1,
		ExpireTime = '9999-01-01 00:00:00.000'
	WHERE SurveysId = @surveyId AND 
	      Interviewid = @interviewId
	
	UPDATE BvAppointment 
	SET State = 2 
	WHERE State = 1 AND 
	      SurveysId = @surveyId AND 
	      InterviewSid = @interviewId
RETURN 0
GO
PRINT N'Creating [dbo].[BvSpLookUpByPerson_ForAssignmentMode]...';


GO
CREATE PROCEDURE [dbo].[BvSpLookUpByPerson_ForAssignmentMode]
	@surveyId int,
	@personId int
AS
    DECLARE @interviewId INT
    DECLARE @rowCount INT
    
    ;WITH calls AS
	(
		SELECT TOP(1) BvCachedCalls.*
		FROM BvCachedCalls WITH( INDEX(IX_BvCachedCalls) )
		INNER JOIN BvPersonRel ON BvPersonRel.PersonSID = @personId
		WHERE CallState = 2 AND
		      SurveySid = @surveyId AND
			  BvPersonRel.ObjectSID = BvCachedCalls.ExplicitSID
		ORDER BY OrderId
	)
	UPDATE calls
	SET CallState = -1,
		@interviewId = InterviewID,
		@surveyId = SurveySid
	OUTPUT
	   deleted.[ID] CallID,
	   deleted.SurveySID,
	   deleted.InterviewID iid

	SET @rowCount = @@ROWCOUNT
	
	IF(@rowCount = 0) RETURN 0
	
	UPDATE BvSvySchedule
	SET CallState = -1,
		ExpireTime = '9999-01-01 00:00:00.000'
	WHERE SurveysId = @surveyId AND 
	      Interviewid = @interviewId
	
	UPDATE BvAppointment 
	SET State = 2 
	WHERE State = 1 AND 
	      SurveysId = @surveyId AND 
	      InterviewSid = @interviewId
RETURN 0
GO
PRINT N'Creating [dbo].[BvSpInterview_UpdateRespondentFields]...';


GO
CREATE PROCEDURE [dbo].[BvSpInterview_UpdateRespondentFields]
    @projectId NVARCHAR(64),
    @respId INT,
    @TelephoneNumber NVARCHAR(255),
    @RespondentName NVARCHAR(255),
    @ExtensionNumber NVARCHAR(255),
    @TimeZoneId INT
AS

    DECLARE @SurveySID INT
    SELECT @SurveySID = SID FROM BvSurvey WHERE Name = @projectId
    IF @SurveySID IS NULL 
    BEGIN
        --RAISERROR( 'survey with projectID = ''%d'' not found', 16, 1, @projectId )
        RETURN (0)
    END

    UPDATE BvInterview
        SET TelephoneNumber = @TelephoneNumber,
            RespondentName = @RespondentName,
            ExtensionNumber = @ExtensionNumber,
            TimezoneId = @TimeZoneId
    WHERE ID = @respId AND
          SurveySID = @SurveySID
        
    UPDATE BvAppointment
    SET TZID = @TimeZoneId
    WHERE SurveySID = @SurveySID AND
          InterviewSID = @respId
GO
PRINT N'Creating [dbo].[BvSpInterviewTimings_Insert]...';


GO
CREATE PROCEDURE [dbo].[BvSpInterviewTimings_Insert]
	@personID INT,
	@utcNow DATETIME,
	@startTime DATETIME OUTPUT,
	@timeCallDelivered DATETIME OUTPUT
AS
	DECLARE @InterviewID INT
	DECLARE @SurveySID INT
	
	UPDATE BvTasks
	SET @StartTime = StartTime,
	    @TimeCallDelivered = TimeCallDelivered,
	    @InterviewID = InterviewID,
	    @SurveySID = SurveySID,
	    StartTime = @UtcNow,
	    TimeCallDelivered = null
	WHERE PersonSID = @personID
	
	DECLARE @InterviewDuriationTime INT = DATEDIFF(second, @TimeCallDelivered, @UtcNow)
	DECLARE @WaitingTime INT = DATEDIFF(second, @StartTime, @TimeCallDelivered)
	
	IF(@WaitingTime < 0)
	BEGIN
		-- Negative WaitingTime is possible in some cases for predictive surveys, see Cr 47039.
		-- In this case WaitingTime must be considered to be 0.
		DECLARE @DiallingMode INT
		SELECT @DiallingMode = DialMode FROM BvSurvey WHERE SID = @SurveySID
			
		SET @WaitingTime = 0;
	END
	
	INSERT INTO BvInterviewTimings(InterviewID, SurveyID, TimeCallDelivered, InterviewDuriationTime, WaitingTime)
	VALUES(@InterviewID, @SurveySID, @TimeCallDelivered, @InterviewDuriationTime, @WaitingTime)
	
RETURN 0
GO
PRINT N'Creating [dbo].[BvSpInterviewTimings_Delete]...';


GO
CREATE PROCEDURE [dbo].[BvSpInterviewTimings_Delete]
	@InterviewId INT, 
    @SurveyId INT,
    @PersonId INT,
    @TaskStartTime DATETIME,
    @TaskDeliveredTime DATETIME
AS
   IF(@TaskStartTime IS NOT NULL AND
      @SurveyId > 0)
   BEGIN
      DECLARE @UtcNow DATETIME = GETUTCDATE()
      DECLARE @WaitingTime INT = DATEDIFF(second, @TaskStartTime, @UtcNow)
      DECLARE @InterviewDuration INT;

      IF( @TaskDeliveredTime IS NOT NULL)
      BEGIN
         SET @WaitingTime = DATEDIFF(second, @TaskStartTime, @TaskDeliveredTime)
         SET @InterviewDuration = DATEDIFF(second, @TaskDeliveredTime, @UtcNow)
      END

      INSERT INTO BvHistory(FiredTime, SurveyId, RoleID, PersonSID, WaitingTime, Duration)
      VALUES(@UtcNow, @SurveyId, 2, @PersonId, @WaitingTime, @InterviewDuration)
   END

   DELETE FROM BvInterviewTimings
   WHERE InterviewID = @InterviewId AND
         SurveyID = @SurveyId

RETURN 0
GO
PRINT N'Creating [dbo].[BvSpInterviewerProductivityReport]...';


GO
CREATE PROCEDURE [dbo].[BvSpInterviewerProductivityReport]
 @SurveySids NVARCHAR (MAX),
 @PersonSids NVARCHAR(MAX),
 @CompletedItses NVARCHAR(MAX),
 @UseDialer BIT,
 @HideEmpty BIT,
 @StartDateTime DATETIME,
 @EndDateTime DATETIME
 
 WITH RECOMPILE
AS 
 DECLARE @DiallerName NVARCHAR(20)
 SET  @DiallerName = N'Dialer';
 
 SET @StartDateTime = ISNULL(@StartDateTime, '1753-01-01T00:00:00.000')
 SET @EndDateTime = ISNULL(@EndDateTime, '9999-12-31T23:59:59.997');

 WITH Persons AS
 (
  SELECT
   p.SID AS PersonSid,
   p.Name AS Name
  FROM dbo.utilSplitNumbers( ISNULL(@PersonSids, ''), ',') s
  INNER JOIN BvPerson p ON p.SID = s.Item

  UNION 

  SELECT p.Sid AS
   PersonSid,
   p.Name AS Name
  FROM BvPerson p
  WHERE @PersonSids IS NULL

  UNION

  SELECT
   DialerSid AS PersonSid,
   @DiallerName AS Name
  FROM (SELECT 0 AS DialerSid) dailerSids
  WHERE @UseDialer = 1
 ),
 CompletedItsList AS
 (
  SELECT Item AS CompletedIts 
  FROM dbo.utilSplitNumbers( ISNULL(@CompletedItses, ''), ',')
 ),
 
 SurveyIdsList AS
 (
  SELECT Item AS SurveyId 
  FROM dbo.utilSplitNumbers( ISNULL(@SurveySids, ''), ',')
 ),
 
 TimeBreaksHistory AS
 (
    SELECT ISNULL(SUM(Duration), 0) Duration, InterviewerId
    FROM BvTimeBreaksHistory
    WHERE StartTime >= @StartDateTime AND
          StartTime <= @EndDateTime
    GROUP BY InterviewerId
 )
 
 SELECT
  p.PersonSid AS PersonId,
  p.Name AS PersonName,
  (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0) + ISNULL(bh.Duration, 0)) AS LogOnTime,
  ISNULL(SUM(h.WaitingTime), 0) AS WaitingTime,
  ISNULL(bh.Duration, 0) AS OnBreakTime,
  COUNT(h.InterviewId) AS DialingsCount, --there is can be dummy record where Hst_Path1 is null (it is used when user was logged in but didn't get any calls)
  ISNULL(SUM(cil.CompletedIts/cil.CompletedIts), 0) AS Completes,
  ISNULL(AVG(CASE WHEN cil.CompletedIts IS NOT NULL THEN h.Duration ELSE NULL END), 0) AS AverageCompletedInterviewDuration
    FROM Persons p
 LEFT JOIN TimeBreaksHistory bh ON bh.InterviewerId = p.PersonSid
 LEFT JOIN BvHistory h ON p.PersonSid = h.PersonSid AND
        h.FiredTime >= @StartDateTime AND
        h.FiredTime <= @EndDateTime AND
        h.RoleID = 2 AND --we should not calced calls whuch were added during sample addition
         h.SurveyId IN (SELECT sil.SurveyId FROM SurveyIdsList sil)
 LEFT JOIN CompletedItsList cil ON cil.CompletedIts = h.ITS
    GROUP BY p.PersonSid, p.Name, bh.Duration
    HAVING (@HideEmpty = 0 OR (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0)) > 0 OR p.PersonSid = 0)
GO
PRINT N'Creating [dbo].[BvSpGetSurveyInterviews]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetSurveyInterviews]
@SurveySID INT, @PersonSID INT, @AssignmentsListmode INT, @ConfirmitVariablePrefix NVARCHAR(MAX),  @filterQuery NVARCHAR (MAX) = NULL, @InterviewsCountShownInManualMode INT
AS
SET NOCOUNT ON	
	
	DECLARE
		@sql          AS NVARCHAR(MAX),
		@selectSql    AS NVARCHAR(MAX),
		@whereSql     AS NVARCHAR(MAX),
		@replicatedColumns	 AS NVARCHAR(MAX),
		@replicatedColumnsAliases   AS NVARCHAR(MAX),
		@replicatedDataTable AS NVARCHAR(MAX)
    
	SET @replicatedColumns = ''
	SET @replicatedColumnsAliases = ''
	SET @replicatedDataTable = 'BvReplicatedData_'+ CAST( @SurveySID AS VARCHAR(10) )

	IF EXISTS ( SELECT 1 
                FROM BvTasks 
                INNER JOIN BvPerson ON PersonSid = SID
                WHERE PersonSID = @PersonSID AND ManualSelection != 1 ) 
    BEGIN
        RETURN (0)
    END
	
	CREATE TABLE #replicatedColumnsNames (
        [ColumnName] NVARCHAR(MAX) NOT NULL
    )
    
    INSERT INTO #replicatedColumnsNames SELECT ColumnName 
					FROM BvReplicationColumns
				    INNER JOIN BvSearchableFields ON 
					BvReplicationColumns.ColumnID = BvSearchableFields.ColumnId AND 
					BvReplicationColumns.TableID = BvSearchableFields.TableID
					WHERE SurveyId = @SurveySID AND
					BvSearchableFields.UseMode = 0 -- Use in Console
       	
       	UPDATE #replicatedColumnsNames 
       	SET    @replicatedColumns = @replicatedColumns+ ',' + @replicatedDataTable + '.' + '[' + ColumnName + ']' + ' AS ' + @ConfirmitVariablePrefix + ColumnName + ' '
       	FROM #replicatedColumnsNames
       	       	
       	UPDATE #replicatedColumnsNames 
       	SET    @replicatedColumnsAliases = @replicatedColumnsAliases+ ',' + @ConfirmitVariablePrefix + ColumnName + ' '
       	FROM #replicatedColumnsNames       	       	
                           
    SET @selectSql = 'SELECT BvSvySchedule.[InterviewID],
 BvInterview.[RespondentName],
 BvInterview.[TelephoneNumber], 
 BvState.[Name] as [ITSName],
 BvSvySchedule.[Priority] as [Priority]'+
  @replicatedColumns+
 'FROM BvSvySchedule
 INNER JOIN BvSurvey ON BvSurvey.SID = BvSvySchedule.SurveySID AND BvSurvey.SID = ' + CAST(@SurveySID AS VARCHAR(16)) +'
 INNER JOIN BvInterview ON BvInterview.SurveySID = BvSvySchedule.SurveySID  AND BvInterview.[ID] = BvSvySchedule.InterviewID  AND ( BvInterview.TransientState <> 13 )'
 IF @AssignmentsListmode = 0
 BEGIN
	SET @selectSql = @selectSql + ' INNER JOIN BvLoginGroup WITH (NOLOCK) ON BvLoginGroup.PersonSID = ' + CAST(@PersonSID AS VARCHAR(16)) + ' AND BvLoginGroup.ObjectSID = BvSvySchedule.ExplicitSID'
 END

 SET @selectSql = @selectSql + ' INNER JOIN BvState ON BvState.StateGroupID = BvSurvey.StateGroupID AND BvState.StateID = BvInterview.TransientState
 LEFT JOIN '+ @replicatedDataTable + ' ON respId = InterviewID 
 WHERE BvSvySchedule.CallState = 2 AND BvSvySchedule.SurveySID = ' + CAST(@SurveySID AS VARCHAR(16))
 
	
	IF(@filterQuery IS NOT NULL AND @filterQuery <> '')			
		SET @whereSql = ' WHERE ' + @filterQuery;
	ELSE
		SET @whereSql = '';
			
	--Need this construction to perform filtration using aliases
	SET @sql = 'SELECT TOP (' + cast(@InterviewsCountShownInManualMode as varchar(10)) + ')
				InterviewID, RespondentName, TelephoneNumber, ITSName ' + @replicatedColumnsAliases +
			   'FROM (' + @selectSql + ')S ' + @whereSql + 'ORDER BY Priority DESC'
print @sql
	EXECUTE sp_executesql @sql

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpGetReplicatedTable]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetReplicatedTable]
AS
    DECLARE @EnableChangeTracking BIT = 1

	SELECT tables.ID AS TableID,
           survey.CfDbSchemaPath,
           tables.TableName,
           tables.PrimaryKey,
           tables.LastVersion,
           survey.DestinationTableName,
           survey.ReplicationStatus,
           survey.SID AS SurveySid
    FROM BvReplicationTables tables
    INNER JOIN BvSurvey survey ON survey.SID = tables.SurveySid AND
                                  survey.ReplicationStatus = @EnableChangeTracking AND
                                  survey.State != 2
RETURN 0
GO
PRINT N'Creating [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]
	@SurveySID INT,
	@Count  INT  --number of requested calls
AS

	DECLARE @Groups TABLE(
		[ObjectSid] [int] NOT NULL,
		[GroupSize] [int] NOT NULL)
		
    DECLARE @MinDistributedCalls INT = 5
	
	INSERT INTO @Groups
    SELECT c.ExplicitSID, 
           COUNT(*) GroupSize
    FROM BvCachedCalls c
    WHERE c.SurveySID = @SurveySID AND 
          c.CallState = 2
    GROUP BY c.ExplicitSID
    
    DECLARE @totalCount INT 
    SELECT @totalCount = SUM(GroupSize) FROM @Groups
    DECLARE @part FLOAT = CAST(@Count AS FLOAT)/CAST(@totalCount AS FLOAT)
    DECLARE @current INT
    DECLARE @currentMinValue INT
    
    UPDATE @Groups
    SET @current = GroupSize*@part+0.5,
        @currentMinValue = CASE WHEN GroupSize < @MinDistributedCalls THEN GroupSize ELSE @MinDistributedCalls END,
        @current = CASE WHEN @current < @MinDistributedCalls THEN @currentMinValue ELSE @current END,
        GroupSize = @current
        
    DECLARE @usedCalls TABLE(
        [Interview] [int] NOT NULL,
		[OrderId] [int] NOT NULL)

	DECLARE @Calls TABLE (
	  [ObjectSid] [int] NOT NULL,
	  [ID] [int] NOT NULL,
	  [Interview] [int] NOT NULL,
	  [TimeInShift] [datetime] NULL,
	  [OrderId] [int] NOT NULL,
	  [ApptID] [int] not null)
        
	;WITH orderedUpdateTable as
	(    
		SELECT calls.*
		FROM @Groups groups
		CROSS APPLY dbo.GetCallsForGroupForPredictiveSurvey( 
			groups.GroupSize, @SurveySID, groups.ObjectSid) calls
	)
	UPDATE orderedUpdateTable
    SET CallState = -2 
	OUTPUT inserted.[interviewID],
		   inserted.[orderId]
	INTO @usedCalls
			  
    UPDATE BvSvySchedule  
    SET CallState = -2 
	OUTPUT inserted.ExplicitSid,
		   inserted.[ID],
		   inserted.[interviewID],
		   inserted.[TimeInShift],
		   uc.[orderId],
		   inserted.ApptId
	INTO @Calls
    FROM @usedCalls uc
    WHERE BvSvySchedule.InterviewID = uc.Interview AND
          SurveySID = @SurveySID
    
    SELECT c.ID, 
           ISNULL( p.Sid, 0 ) AS ExplicitSid, --person id (if person is assigned) or 0 (if survey or person group)
           @SurveySID AS SurveySid,
           i.DialingMode DiallingMode,
		   Interview AS InterviewID, 
		   TelephoneNumber,
		   (CASE WHEN c.ApptId > 0 THEN c.[TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
		   (CASE WHEN p.Sid IS NULL AND @SurveySID <> ObjectSid THEN ObjectSid
                 ELSE 0
            END) AS GroupID --Explicit Group ID or 0 in all other cases (when call is assigned to  implicit survey group or a user)
    FROM @Calls c
    INNER JOIN BvInterview i ON Interview = i.ID AND
                                SurveySID = @SurveySID
    LEFT JOIN BvPerson p on p.SID = ObjectSid
	ORDER BY orderid
	
RETURN (@@ROWCOUNT)
GO
PRINT N'Creating [dbo].[BvSpGetActiveCallsDistribution]...';


GO
--stored procedure dbo.BvSpGetActiveCallsDistribution returns a breakdown of active calls per ExplicitSid ( user/group )
--for 20 min starting from @StartTime for a specified suurvey @SurveySid
--dynamic PIVOT is used
CREATE PROCEDURE [dbo].[BvSpGetActiveCallsDistribution]
	@StartTime DATETIME,                                            -- expects UTC time
	@SurveySid INT,
	@DefaultTzId INT
AS
DECLARE
	@cols         AS NVARCHAR(MAX),
	@sql          AS NVARCHAR(MAX),
	@bias         AS INT;
	
set @StartTime = DATEADD(SECOND, -DATEPART(SECOND, @StartTime), @StartTime) --we need this to make further conversion to
																			--smalldatetime correct (do not shift minutes
																			-- of dates that have seconds > 30)
SELECT  @bias = DATEDIFF( [mi], @StartTime,  dbo.UTC2LT( @StartTime, Bias, DaylightType,
							StandardDayOfWeek, StandardStart, StandardBias,
							DaylightDayOfWeek, DaylightStart, DaylightBias ))
	FROM  [BvTimezone]
	WHERE [ID] = @DefaultTzId

-- Construct the column list for the IN clause
-- e.g., [12:00],[12:01],[12:03]
SET @cols = STUFF(
	(SELECT N',' + QUOTENAME(MN) AS [text()]
		FROM (  SELECT DISTINCT CONVERT(CHAR(5), DATEADD( mi, @bias, [time]), 108) AS MN 
				FROM [dbo].[BvActiveCallsInfo]
				WHERE [SurveySID] = @SurveySid 
				AND [time] >= @StartTime 
				AND [time] <= DATEADD( [mi], 20, @StartTime))  AS MN
		ORDER BY MN
		FOR XML PATH('')),
	1, 1, N'');
  
-- Construct the full T-SQL statement and execute dynamically. Query could look like this
/*
SELECT *
FROM (SELECT ISNULL ( g.Name,'*Survey Assignment*') as [Group/User Name], convert(char(5), DATEADD( mi,-300, [time]), 108) AS [minutes], [CallsCount]
          FROM dbo.BvActiveCallsInfo LEFT JOIN ( SELECT SID, Name FROM BvPerson UNION SELECT SID, Name from BvPersonGroup ) as g on ExplicitSid = g.SID 
          where surveysid=1 and [time] >='Dec  7 2009 11:50AM' and [time] <='Dec  7 2009 12:10PM' ) AS D
  PIVOT(MAX(CallsCount) FOR minutes IN([07:00],[07:01],[07:02],[07:03],[07:04],[07:05],[07:07],[07:08],[07:09],[07:10]) )  as  P order by [Group/User Name] ;
*/
SET @sql = N'SELECT *
FROM (SELECT ISNULL ( g.[Name],' + '''' + '*Survey Assignment*' + '''' + ') as [Group/User name], convert(char(5), DATEADD( mi,' 
		+ CAST( @bias AS VARCHAR(MAX)) + ', [time]), 108) AS [minutes], [CallsCount]
          FROM [dbo].[BvActiveCallsInfo] 
          LEFT JOIN ( 
			  SELECT [SID], [Name] FROM [BvPerson]
			  UNION 
			  SELECT [SID], [Name] from [BvPersonGroup] ) as g on [ExplicitSid] = g.[SID] 
			  WHERE [SurveySID]=' + CAST( @SurveySid  AS VARCHAR(32))+ ' AND [Time] >=' + '''' + 
			  + CAST(  @StartTime AS VARCHAR(MAX)) + '''' + ' AND [Time] <=' + '''' + 
			  CAST( DATEADD( [MI], 20, @StartTime ) AS VARCHAR(MAX)) + '''' + ') AS D
  PIVOT( MAX([CallsCount]) FOR [minutes] IN(' + @cols + N') ) AS P ORDER BY [Group/User name];';
  
EXEC sp_executesql @sql;
GO
PRINT N'Creating [dbo].[BvSpGetAppLock]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetAppLock]
	@ResourceName NVARCHAR(255),
	@LockMode NVARCHAR(32),
	@LockTimeout INT,
	@ServerName NVARCHAR(MAX),
	@WaitPeriod INT, --milliseconds
	@ResourceOwner NVARCHAR(MAX)
AS
    DECLARE @ReturnValue INT = 0;
    
    EXEC @ReturnValue = sp_getapplock @ResourceName, @LockMode, N'Session', @LockTimeout
    
    IF @ReturnValue >= 0
    BEGIN
		MERGE INTO BvAppLocks AS Target
		  USING ( SELECT @ResourceName AS ResourceName ) 
		  AS Source (ResourceName)
			ON Target.ResourceName = Source.ResourceName
		  WHEN MATCHED AND
			   (TimeLockLeave IS NULL OR
				DATEADD(millisecond, @WaitPeriod, TimeLockLeave) <= GETUTCDATE())
		  THEN
			 UPDATE SET TimeLockEnter = GETUTCDATE(),
						TimeLockLeave = NULL,
						ServerName = @ServerName,
						IsLockHeld = 1,
						ResourceOwner = @ResourceOwner
		  WHEN NOT MATCHED THEN
			 INSERT(ResourceName, TimeLockEnter, TimeLockLeave, IsLockHeld, ServerName, ResourceOwner)
			 VALUES(@ResourceName, GETUTCDATE(), NULL, 1, @ServerName, @ResourceOwner);
         
		IF(@@ROWCOUNT = 0)
		BEGIN
		   SET @ReturnValue = 2 --period is not expired
		   EXEC sp_releaseapplock @ResourceName, N'Session'
		END
    END
	   
	
RETURN @ReturnValue
GO
PRINT N'Creating [dbo].[BvSpGetInterviewerActiveBreak]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetInterviewerActiveBreak]
	@personId INT
AS
    WITH LastBreak AS
    (
		SELECT TOP(1) ID, StartTime, InterviewerId, Duration
		FROM BvTimeBreaksHistory
		WHERE InterviewerId = @personId
		ORDER BY StartTime DESC
	)
	SELECT ID, StartTime, InterviewerId
	FROM LastBreak
	WHERE Duration IS NULL
RETURN 0
GO
PRINT N'Creating [dbo].[BvSpFinishInterviewerBreak]...';


GO
CREATE PROCEDURE [dbo].[BvSpFinishInterviewerBreak]
	@InterviewerId INT    	
AS
    ;WITH TimeBreaksHistory AS
    (
       SELECT TOP(1) *
       FROM BvTimeBreaksHistory
       WHERE InterviewerId = @InterviewerId
       ORDER BY StartTime DESC
    )
	UPDATE TimeBreaksHistory 
	SET Duration = DATEDIFF(second, StartTime, GETUTCDATE())
	WHERE Duration IS NULL
	
RETURN 0
GO
PRINT N'Creating [dbo].[BvSpFilter_GetParentFilters]...';


GO
CREATE PROCEDURE [dbo].[BvSpFilter_GetParentFilters]
@ObjectSID INTEGER
AS
SET NOCOUNT ON
DECLARE @FilterSID INTEGER
DECLARE @Rows INTEGER

 IF @ObjectSID IS NULL
 BEGIN
 /* Looks like we're generating code using FMTONLY. So lets return metadata*/
	SELECT 0 as [SID]
    RETURN 0;
 END
 
    CREATE TABLE #temp (
        SID [int] NOT NULL
    )

    CREATE TABLE #look(
        SID [int] NOT NULL
    )

    CREATE TABLE #find(
        SID [int] NOT NULL
    )

    INSERT INTO #look 
        SELECT BvFilters.SID
        FROM BvFilters, BvFilterFields 
        WHERE BvFilterFields.FilterSID = BvFilters.SID
              AND BvFilterFields.[Sign] = 8 -- subfilter
              AND CAST( BvFilterFields.[Value] AS INTEGER ) = @ObjectSID

    INSERT INTO #find SELECT SID FROM #look

    SET @Rows = @@ROWCOUNT

    WHILE @Rows <> 0
    BEGIN
       INSERT INTO #temp 
           SELECT BvFilters.SID
           FROM BvFilterFields, #look, BvFilters
           WHERE BvFilterFields.FilterSID = BvFilters.SID
              AND BvFilterFields.[Sign] = 8 -- subfilter
              AND CAST( BvFilterFields.[Value] AS INTEGER ) = #look.SID
              AND BvFilters.SID NOT IN
              ( SELECT SID FROM #find )
       SET @Rows = @@ROWCOUNT

       INSERT INTO #find SELECT SID FROM #temp
       
       TRUNCATE TABLE #look

       INSERT INTO #look SELECT SID FROM #temp

       TRUNCATE TABLE #temp
    END

    DROP TABLE #temp
    DROP TABLE #look

    SELECT SID FROM #find

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpCleanActiveCallsInfo]...';


GO
CREATE PROCEDURE [dbo].[BvSpCleanActiveCallsInfo]
	@ExpirationPeriod INT
AS
BEGIN

	DELETE from BvActiveCallsInfo
	WHERE DateAdd(day, @ExpirationPeriod, [Time]) < GETUTCDATE()
 
END
GO
PRINT N'Creating [dbo].[BvSpCallHistoryData]...';


GO
create PROCEDURE [dbo].[BvSpCallHistoryData]
        @StartDate DATETIME, @EndDate DATETIME, @SurveySIDs nvarchar(max), @MaxRows int
        AS
        
		IF(@StartDate IS NULL) SET @StartDate = '01-01-1753 00:00:00'
		IF(@EndDate IS NULL) SET @EndDate = '12-31-9999 23:59:59.997'

		/* CTE for selected statuses. */
		;WITH SelectedSurveySIDs_CTE AS
		(
		 SELECT [Item] FROM dbo.utilSplitNumbers(ISNULL(@SurveySIDs, ''), ',')
		)

        SELECT TOP (@MaxRows)
         [h].[FiredTime] AS [FiredTime],
         [s].[Name] AS [ProjectID],
		 [s].[Description] AS [Name],
         [h].[InterviewId] AS [InterviewID],
         [h].[PersonSID] AS [InterviewerID],
		 (CASE WHEN [p].[SID] IS NOT NULL THEN [p].[Name]
			   WHEN [h].[PersonSID] = 0 THEN 'Dialer'
			   ELSE NULL END) [InterviewerName],
         [h].[TelephoneNumber] AS [TelephoneNumber], 
         [h].[ITS] AS [ExtendedStatus],
         [h].[Duration] AS [Duration],         /* Interview time in seconds. */
         [h].[WaitingTime] AS [WaitingTime]

        FROM      [BvHistory] [h] 
        INNER JOIN [BvSurvey]  [s] ON [h].[SurveyId] = [s].[SID] AND [s].State in (0, 1)
        LEFT JOIN SelectedSurveySIDs_CTE [ss] ON [s].[SID] = [ss].Item 
		LEFT JOIN BvPerson [p] ON [p].SID = [h].[PersonSID]

        WHERE 
              [h].[RoleID] = 2 /*CATI*/ 
          AND [h].[FiredTime] BETWEEN @StartDate AND @EndDate AND
              [h].[InterviewID] IS NOT NULL AND
			  ([ss].Item IS NOT NULL OR @SurveySIDs IS NULL)
          
        ORDER BY 
          [h].[SurveyId], [h].[FiredTime]

        RETURN 0
GO
PRINT N'Creating [dbo].[BvSpCachedCalls_CallsCount_SaveToActiveCallsInfo]...';


GO
CREATE PROCEDURE [dbo].[BvSpCachedCalls_CallsCount_SaveToActiveCallsInfo]
AS
	INSERT INTO [BvActiveCallsInfo]
		SELECT GETUTCDATE(), [SurveySID], [ExplicitSID], COUNT(*)
			FROM [BvCachedCalls]
			GROUP BY [SurveySID], [ExplicitSID]
GO
PRINT N'Creating [dbo].[BvSpAttemptsByDispositionReport]...';


GO
CREATE PROCEDURE [dbo].[BvSpAttemptsByDispositionReport]
   @SurveySid INT,
   @Itses NVARCHAR(MAX),
   @HideEmpty BIT,
   @StartDateTime DATETIME,
   @EndDateTime DATETIME
   
   WITH RECOMPILE
AS
    DECLARE @StateGroupId INT
    SELECT @StateGroupId = s.StateGroupID
    FROM BvSurvey s
    WHERE s.Sid = @SurveySid;
    
    IF(@StartDateTime IS NULL) SET @StartDateTime = '01-01-1753 00:00:00'
    IF(@EndDateTime IS NULL) SET @EndDateTime = '12-31-9999 23:59:59.997'

    ;WITH NecessaryItsList AS
    (
       SELECT s.StateID AS Its,
              s.Name AS [Name]
       FROM dbo.utilSplitNumbers( ISNULL(@Itses, ''), ',') i
       INNER JOIN BvState s ON (s.StateGroupID = @StateGroupId AND
                                s.StateID = i.Item)
       
       UNION 
       
       SELECT s.StateID AS Its,
              s.Name AS [Name]
       FROM BvState s
       WHERE @Itses IS NULL AND
             s.StateGroupID = @StateGroupId
    ),
	Attempts AS
	(
	   SELECT (ROW_NUMBER() over(partition by InterviewID order by FiredTime)) AS NumberAttempts,
	          h.InterviewID AS InterviewId,
	          s.StateId AS Its,
	          s.Name AS ItsName
	   FROM BvState s
	   LEFT JOIN BvHistory h ON s.StateId = h.ITS AND
	                            h.SurveyId = @SurveySid AND
	                            h.FiredTime >= @StartDateTime AND
	                            h.FiredTime <= @EndDateTime AND
	                            h.InterviewId IS NOT NULL AND
	                            h.RoleID = 2
	   WHERE s.StateGroupID = @StateGroupId
	),
	AttemptsByDesposition AS
	(
	   SELECT Its AS  Code,
	          ItsName AS Disposition,
              [1] AS Attempts1,
              [2] AS Attempts2,
              [3] AS Attempts3,
              [4] AS Attempts4,
              [5] AS Attempts5,
              [6] AS Attempts6,
              [7] AS Attempts7,
              [8] AS Attempts8,
              [9] AS Attempts9,
              [10] AS Attempts10
       FROM Attempts a
       PIVOT
       (
          COUNT(a.InterviewId) 
          FOR a.NumberAttempts in ( [1], [2], [3], [4], [5], [6], [7], [8], [9], [10])
       ) AS p
       WHERE (@HideEmpty = 0 OR
              [1]+[2]+[3]+[4]+[5]+[6]+[7]+[8]+[9] > 0)
    )
    SELECT abd.*
    FROM AttemptsByDesposition abd
    INNER JOIN NecessaryItsList il ON il.Its = abd.Code
GO
PRINT N'Creating [dbo].[BvSpAlertsHistoryAggregatedReport]...';


GO
CREATE PROCEDURE BvSpAlertsHistoryAggregatedReport
    @PersonIds NVARCHAR(MAX),
    @SurveyIds NVARCHAR(MAX),
    @StartDate DATETIME,
    @EndDate   DATETIME,
    @InterviewState TINYINT
 AS
	;WITH Persons AS
	(
		SELECT p.SID AS PersonId,
			   p.Name AS PersonName
		FROM dbo.utilSplitNumbers( ISNULL(@PersonIds, ''), ',') s
		INNER JOIN BvPerson p ON p.SID = s.Item
		
		UNION 

		SELECT p.SID AS PersonId,
		       p.Name AS PersonName
		FROM BvPerson p
		WHERE @PersonIds IS NULL OR @PersonIds = ''
	),
	Surveys AS
	(
		SELECT s.Item AS SurveyId
		FROM dbo.utilSplitNumbers( ISNULL(@SurveyIds, ''), ',') s
	)
	SELECT p.PersonId,
		   p.PersonName,
           ISNULL(SUM(h.AnswerSubmissionAlert^1), 0) AnswerSubmissionAmberCounts,
           ISNULL(SUM(h.AnswerSubmissionAlert^0), 0) AnswerSubmissionRedCounts,
           ISNULL(SUM(h.QuickAnswerSubmissionAlert^1), 0) QuickAnswerSubmissionAmberCounts,
           ISNULL(SUM(h.QuickAnswerSubmissionAlert^0), 0) QuickAnswerSubmissionRedCounts
    FROM BvAnswerSubmissionAlertHistory h
    INNER JOIN Persons p ON p.PersonId = h.PersonId
    INNER JOIN Surveys s ON s.SurveyID = h.SurveyId
    WHERE SubmissionTime >= @startDate AND
          SubmissionTime <= @endDate AND
          (InterviewState = @InterviewState OR @InterviewState IS NULL)
    GROUP BY p.PersonId, p.PersonName
GO
PRINT N'Creating [dbo].[BvSpPerson_updateDialerConnection]...';


GO
CREATE PROCEDURE [dbo].[BvSpPerson_updateDialerConnection]
	@SID int, 
	@DialerConnection nvarchar(256)
AS

UPDATE BvPerson 
	SET DialerConnection = @DialerConnection
		WHERE SID = @SID

RETURN (0)
GO
PRINT N'Creating [dbo].[SetDialerSurveyParametersWhereIsNull]...';


GO
CREATE PROCEDURE [dbo].[SetDialerSurveyParametersWhereIsNull]
	@dialerParameters nvarchar(max) 
AS
	UPDATE BvSurvey set DialerParameters = @dialerParameters
		WHERE DialerParameters IS NULL
RETURN 0
GO
PRINT N'Creating [dbo].[BvSpSystemSetting_Update]...';


GO
CREATE PROCEDURE [dbo].[BvSpSystemSetting_Update]
	@SystemName AS NVARCHAR(256),
	@Value AS NVARCHAR(MAX)
AS
	MERGE BvSystemSettings as target
	USING ( SELECT @SystemName ) AS source( SystemName )
	ON target.SystemName = source.SystemName
	WHEN MATCHED THEN 
        UPDATE SET Value = @Value
	WHEN NOT MATCHED THEN	
		INSERT ( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
		VALUES(  @SystemName, '<NULL>', '<NULL>', '<NULL>', 0, 0, @Value );
GO
PRINT N'Creating [dbo].[BvSpSurveyCleanup_IsClean]...';


GO
CREATE PROCEDURE [dbo].[BvSpSurveyCleanup_IsClean]
    @SurveyId INT
AS
     DECLARE @Cnt INT
     SELECT @Cnt = COUNT(*) FROM BvPersonOrGroupAssignmentOnSurvey WHERE SurveyId = @SurveyId
     
     IF @Cnt <> 0 
     BEGIN
         RETURN 0
     END
      
     SELECT @Cnt = COUNT(*) FROM BvSvySchedule WHERE SurveySid = @SurveyId
 
     IF @Cnt <> 0
     BEGIN
         RETURN 0
     END
     
     RETURN 1
GO
PRINT N'Creating [dbo].[BvSpSurveyCleanup_GetSurveysWhichAreReadyForNotice]...';


GO
CREATE PROCEDURE [dbo].[BvSpSurveyCleanup_GetSurveysWhichAreReadyForNotice]
    @LastTouchTime DATETIME
AS
    SELECT SID AS Id, Name, Description, NotificationEmail FROM BvSurvey s
        LEFT JOIN BvUserNotification n
        ON n.ObjectId = s.SID AND n.Type = 1/*UserNotificationType.SurveyCleanupNotificationWarning*/ AND  s.LastTouchTime < n.SendDate
        WHERE State = 0 AND LastTouchTime < @LastTouchTime AND n.Id IS NULL
GO
PRINT N'Creating [dbo].[BvSpSurveyCleanup_GetSurveysWhichAreReadyForCleanup]...';


GO
CREATE PROCEDURE [dbo].[BvSpSurveyCleanup_GetSurveysWhichAreReadyForCleanup]
        @LastTouchTime DATETIME,
        @LastSendNoticyTime DATETIME
AS
	SELECT SID AS Id, Name, Description, NotificationEmail FROM BvSurvey s
        LEFT JOIN BvUserNotification n
        ON n.ObjectId = s.SID AND n.Type = 1/*UserNotificationType.SurveyCleanupNotificationWarning*/ AND  s.LastTouchTime < n.SendDate
        WHERE State = 0 AND LastTouchTime < @LastTouchTime AND n.SendDate < @LastSendNoticyTime
GO
PRINT N'Creating [dbo].[BvSpSurvey_Clean]...';


GO
CREATE PROCEDURE [dbo].[BvSpSurvey_Clean]
    @SurveyId INT
AS
    DECLARE @CountOfDeletedAssignment INT
    DECLARE @CountOfDeletedCalls INT

    DELETE BvPersonOrGroupAssignmentOnSurvey 
    WHERE SurveyId = @SurveyId

    SET @CountOfDeletedAssignment = @@ROWCOUNT
    
    DELETE FROM bvpersonrel
    WHERE type = 2 AND objectsid = @SurveyId
    
    DELETE FROM bvlogingroup 
    WHERE surveysid = @surveyID OR objectsid = @surveyID
    

    DELETE FROM BvSvySchedule WHERE SurveySid = @SurveyId
    SET @CountOfDeletedCalls = @@ROWCOUNT

    SELECT @CountOfDeletedAssignment as CountOfDeletedAssignment, @CountOfDeletedCalls as CountOfDeletedCalls
GO
PRINT N'Creating [dbo].[BvSpGetInterviewerPerformanceList]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetInterviewerPerformanceList] 
 @onlyLoggedIn bit
AS 

IF(@onlyLoggedIn = 0)	
		SELECT InterviewerId, 
			   InterviewerName,
			   InterviewingTime,
			   TotalInterviewCount, 
			   CompletedInterviewCount,
			   CompletedInLastHourCount 
		FROM BvInterviewerPerformance	
ELSE
		SELECT InterviewerId, 
			   InterviewerName,
			   InterviewingTime,
			   TotalInterviewCount, 
			   CompletedInterviewCount,
			   CompletedInLastHourCount 
		FROM BvTasks inner join BvInterviewerPerformance ON 
			 BvTasks.PersonSID = BvInterviewerPerformance.InterviewerId
GO
PRINT N'Creating [dbo].[BvSpAggregateInterviewerPerformance]...';


GO
CREATE PROCEDURE [dbo].[BvSpAggregateInterviewerPerformance]

 @StartDateTime DATETIME,
 @EndDateTime DATETIME,
 @CompletedItses NVARCHAR(MAX) 
 
AS
 
Declare  @EndDateMinusOneHourTime DATETIME;
Set @EndDateMinusOneHourTime  = DATEADD(Hour,-1, @EndDateTime);

DELETE FROM BvInterviewerPerformance;
 
WITH Persons AS
	(
	SELECT 	
		p.SID AS PersonSid,
		p.Name as PersonName
		FROM BvPerson p 	  	  	
	),
	CompletedItsList AS
	(
	SELECT Item AS CompletedIts 
	FROM dbo.utilSplitNumbers( ISNULL(@CompletedItses, ''), ',')
	)
	INSERT INTO BvInterviewerPerformance(
	[InterviewerId],
	[InterviewerName],
	[InterviewingTime],
	[TotalInterviewCount],
	[CompletedInterviewCount],
	[CompletedInLastHourCount]
	)
	SELECT 
	p.PersonSid AS InterviewerId,
	p.PersonName AS InterviewerName,
	(ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0)) AS InterviewingTime,  
	COUNT(h.ITS) AS TotalInterviewCount, --there is can be dummy record where Hst_Path1 is null (it is used when user was logged in but didn't get any calls)  
	ISNULL(SUM(CASE WHEN cil.CompletedIts IS NOT NULL  THEN 1 ELSE 0 END), 0) AS CompletedInterviewCount,  
	ISNULL(SUM(CASE WHEN h.FiredTime >= @EndDateMinusOneHourTime and cil.CompletedIts IS NOT NULL THEN 1 ELSE 0 END), 0) AS CompletedInLastHourCount      
	FROM Persons p 
	INNER JOIN BvHistory h ON p.PersonSid = h.PersonSid AND
		h.FiredTime >= @StartDateTime AND
		h.FiredTime <= @EndDateTime AND
		h.RoleID = 2  --we should not calculate calls which were added during sample addition                          
	LEFT JOIN CompletedItsList cil ON cil.CompletedIts = h.ITS
	GROUP BY p.PersonSid, p.PersonName
GO
PRINT N'Creating [dbo].[BvSpTransferArray_Move]...';


GO
CREATE PROCEDURE [dbo].[BvSpTransferArray_Move]
	@srcBatchId int, 
	@dstBatchId int,
	@count int
AS
	UPDATE TOP(@count) BvTransferArrays
		SET BatchID = @dstBatchId
		WHERE BatchID = @srcBatchId
RETURN @@ROWCOUNT
GO
PRINT N'Creating [dbo].[BvSpFilter_MoveToSurvey]...';


GO
CREATE PROCEDURE [dbo].[BvSpFilter_MoveToSurvey]
   @SourceSurveySid INT,
   @TargetSurveySid INT
AS 

UPDATE [BvFilters]
SET SurveySID = @TargetSurveySid
WHERE SurveySID = @SourceSurveySid
GO
PRINT N'Creating [dbo].[BvSpGetSurveysWithSurveySpecificFilters]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetSurveysWithSurveySpecificFilters]
  @userName NVARCHAR (255)
AS
SELECT 
  [s].[SID] as [SurveySid],
  [s].[Name] as [ProjectId],
  [s].[Description] as [ProjectName],
  COUNT(*) as [FiltersCount]
FROM [BvSurvey] [s]
INNER JOIN [BvUserSurveyPermission] [p] on [s].[SID] = [p].SurveySID
INNER JOIN [BvFilters] [f] ON [s].[SID] = [f].[SurveySID]
WHERE [s].[State] != 2 -- Exclude soft-deleted surveys
	AND [p].[UserName] = @userName
GROUP BY [s].[SID], [s].[Name], [s].[Description]
GO
PRINT N'Creating [dbo].[BvSpPerson_GetAssignments]...';


GO
CREATE procedure [dbo].[BvSpPerson_GetAssignments]
@person_sid int
as
    select ObjectSID from BvPersonRel 
        where PersonSID = @person_sid

return (0)
GO
PRINT N'Creating [dbo].[BvSpPersonMonitoring_SetLastID]...';


GO
CREATE PROCEDURE [dbo].[BvSpPersonMonitoring_SetLastID] 
 -- Add the parameters for the stored procedure here
 @PersonSID INT = 0,
 @MonitoringSessionID BIGINT = 0,
 @LastSentID BIGINT = 0
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

    -- Insert statements for procedure here
 UPDATE BvPersonMonitoringLastID SET LastSentID = @LastSentID WHERE (PersonSID = @PersonSID) AND (MonitoringSessionID = @MonitoringSessionID)
END
GO
PRINT N'Creating [dbo].[BvSpPersonMonitoring_InsertEvent]...';


GO
CREATE PROCEDURE [dbo].[BvSpPersonMonitoring_InsertEvent] 
 -- Add the parameters for the stored procedure here
 @PersonSID INT, 
 @MonitoringSessionID BIGINT,
 @TimeStamp DATETIME,
 @MessageType INT,
 @EventObject VARBINARY(MAX)
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

 INSERT INTO BvPersonMonitoringEvents ([PersonSID], [MonitoringSessionID], [TimeStamp], MessageType, EventObject) VALUES(@PersonSID, @MonitoringSessionID, @TimeStamp, @MessageType, @EventObject)
 
 RETURN scope_identity()
END
GO
PRINT N'Creating [dbo].[BvSpPersonMonitoring_GetNewEvents]...';


GO
CREATE PROCEDURE [dbo].[BvSpPersonMonitoring_GetNewEvents] 
 -- Add the parameters for the stored procedure here
 @PersonSID INT = 0, 
 @MonitoringSessionID BIGINT = 0,
 @MaxEventID BIGINT = 0
AS
BEGIN

 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

 SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

 BEGIN TRANSACTION

 SELECT *
 FROM BvPersonMonitoringEvents
 WHERE (PersonSID = @PersonSID) AND (MonitoringSessionID = @MonitoringSessionID) AND (ID > @MaxEventID)
 ORDER BY [ID] ASC
 
 COMMIT TRANSACTION
END
GO
PRINT N'Creating [dbo].[BvSpPersonMonitoring_GetLastID]...';


GO
CREATE PROCEDURE [dbo].[BvSpPersonMonitoring_GetLastID] 
 -- Add the parameters for the stored procedure here
 @PersonSID INT = 0,
 @MonitoringSessionID BIGINT
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

    -- Insert statements for procedure here
 SELECT LastSentID FROM BvPersonMonitoringLastID WHERE (PersonSID = @PersonSID) AND (MonitoringSessionID = @MonitoringSessionID)
END
GO
PRINT N'Creating [dbo].[BvSpCfUpdateSurveyReplicationStatus]...';


GO
CREATE PROCEDURE [dbo].[BvSpCfUpdateSurveyReplicationStatus]
	@ProjectId NVARCHAR( 255 ),
	@IsReplicationEnabled BIT
AS
	SET NOCOUNT ON
	UPDATE BvSurvey SET
		ReplicationStatus = @IsReplicationEnabled
	WHERE [Name] = @ProjectId
	
RETURN 0
GO
PRINT N'Creating [dbo].[BvSpInterview_CfData_Insert]...';


GO
CREATE PROCEDURE [dbo].[BvSpInterview_CfData_Insert]
    @ProjectID NVARCHAR(256),
    @InterviewID INT,
    @Status_CF NVARCHAR(256),
    @RespondentName NVARCHAR(256),
    @RespondentPhone VARCHAR(256),
    @LastCallTime DATETIME,
    @TotalDuration INT,
    @InterviewerID INT,
    @RoleID INT,
    @LastChannelID TINYINT
AS
DECLARE @SurveySID INT
DECLARE @StatusBvFEE INT

    -- get survey sid and validate it
    SELECT @SurveySID = [Sid] FROM [BvSurvey] WHERE [Name] = @ProjectID
    
    IF @SurveySID IS NULL
    BEGIN
        RAISERROR('Survey for project %s does not exist', 16, 1, @ProjectID)
        RETURN -1
    END

    -- get interviewer and validate it
    IF ( @roleID = 2 /* CATI */ )
    BEGIN
        IF NOT EXISTS ( SELECT [Sid] FROM [BvPerson] WHERE [Sid] = @InterviewerID )
        BEGIN
            --We should ingnore wrong interviewer, because interviewer can be alredy deleted
            SET @InterviewerID = NULL
        END
    END
    ELSE
        SET @InterviewerID = NULL
    
    -- get BvFEE status by CfStatus and validate it
    SELECT @StatusBvFEE = [StatusCode_BvFEE] FROM [BvConfirmitStatus]
        WHERE [StatusCode_Cnf] = @Status_CF
        
    IF @StatusBvFEE IS NULL
    BEGIN
        SET @StatusBvFEE = 30 --ERROR ITS
    END
    
    SET @LastCallTime = DATEADD( MS, -DATEPART( MS, @LastCallTime ), @LastCallTime ) -- reset milliseconds to 000
    
    IF NOT EXISTS ( SELECT [Id] FROM [BvInterview] 
        WHERE [ID] = @InterviewID AND [SurveySID] = @SurveySID )
    BEGIN
        INSERT INTO [BvInterview]
        (
            [ID], 
            [SurveySID], 
            [BatchID],
            [LastChannelID],
            [TransientState],
            [RespondentName],
            [TelephoneNumber],
            [LastCallTime],
            [Duration],
            [LastCallPersonSID]
        )
        VALUES
        (
            @InterviewID,
            @SurveySID,
            0 /*[BatchID]*/,
            @LastChannelID,
            @StatusBvFEE,
            @RespondentName,
            @RespondentPhone,
            @LastCallTime,
            @TotalDuration,
            @InterviewerID
        )
    END
    ELSE
    BEGIN
        UPDATE [BvInterview] SET
            [TransientState] = @StatusBvFEE,
            [RespondentName] = ISNULL( @RespondentName, [RespondentName] ),
            [TelephoneNumber] = ISNULL( @RespondentPhone, [TelephoneNumber] ),
            [LastCallTime] = @LastCallTime,
            [Duration] = @TotalDuration,
            [LastCallPersonSID] = @InterviewerID,
            [LastChannelID] = @LastChannelID
        WHERE [ID] = @InterviewID AND [SurveySID] = @SurveySID
    END

RETURN 0
GO
PRINT N'Creating [dbo].[BvSpHistory_CfData_Insert]...';


GO
CREATE PROCEDURE [dbo].[BvSpHistory_CfData_Insert]
    @ProjectID NVARCHAR(256),
    @RespondentPhone NVARCHAR(256),
    @FiredTime DATETIME,
    @InterviewID INT,
    @Status_CF NVARCHAR(256),
    @AppointmentID INT,
    @NetDuration INT,
    @GrossDuration INT,
    @TotalDuration INT,
    @InterviewerID INT,
    @RoleID INT
AS
DECLARE @SurveySID INT
DECLARE @InterviewerID_BF INT
DECLARE @StatusBvFEE INT

    -- get survey sid and validate it
    SELECT @SurveySID = [Sid] FROM [BvSurvey] WHERE [Name] = @ProjectID
    
    IF @SurveySID IS NULL
    BEGIN
        RAISERROR('Survey for project %s does not exist', 16, 1, @ProjectID)
        RETURN -1
    END

    -- get interviewer and validate it
    IF ( @roleID = 2 /* CATI */ )
    BEGIN
        IF NOT EXISTS ( SELECT [Sid] FROM [BvPerson] WHERE [Sid] = @InterviewerID )
        BEGIN
            --We should ingnore wrong interviewer, because interviewer can be alredy deleted
            SET @InterviewerID_BF = 0
        END
        
        SET @InterviewerID_BF = @InterviewerID
    END
    ELSE IF ( @RoleID = 64 /* CAPI */ )
    BEGIN
        SELECT @InterviewerID_BF = [ObjectSID] FROM [BvNumber] 
            WHERE [BvID] = @InterviewerID AND [ClassID] = 10 /*BVSBS_PERSON*/
        
        IF @InterviewerID_BF IS NULL 
        BEGIN
            RAISERROR('CAPI interviewer %d does not exist', 16, 1, @InterviewerID)
            RETURN -1
        END
    END
    
    -- get BvFEE status by CfStatus and validate it
    SELECT @StatusBvFEE = [StatusCode_BvFEE] FROM [BvConfirmitStatus]
        WHERE [StatusCode_Cnf] = @Status_CF OR ( @Status_CF IS NULL AND [StatusCode_Cnf] IS NULL )
        
    IF @StatusBvFEE IS NULL
    BEGIN
        SET @StatusBvFEE = 30 --ERROR ITS
    END
    
    --if BvFEE status is appointment we should get latests active appointmentId for the interview
    --because CF does not pass appID but it should be stored in [Hst_Path3] field
    SELECT @AppointmentID = MAX([ID]) FROM [BvAppointment]
		WHERE [SurveySID] = @SurveySID AND InterviewSID = @InterviewID AND [State] = 0 /* has not call*/
  
	SET @AppointmentID = ISNULL(@AppointmentID, 0) --if appointment does not exist

    INSERT INTO [BvHistory]
    (
            [SurveyId],
            [TelephoneNumber],
            [FiredTime],
            [InterviewID],
            [ITS],
            [AppointmentID],
            [WaitingTime],
            [ConfirmitDuration],
            [Duration],
            BatchId,
            [PersonSID],
            [RoleID]
    )
    SELECT
		@SurveySID      /*Hst_ObjID*/,
		@RespondentPhone /*TelephoneNumber*/,
		@FiredTime       /*FiredTime*/,
		@InterviewID     /*InterviewID*/,
		@StatusBvFEE     /*ITS*/,
		@AppointmentID   /*AppointmentID*/,
		it.WaitingTime     /*WaitingTime*/,
		@GrossDuration /*ConfirmitDuration*/,
		ISNULL(it.InterviewDuriationTime, @TotalDuration) /*Duration*/,
		0               /*BatchId*/,
		@InterviewerID_BF /*PersonSID*/,
		@RoleID          /*RoleID*/
    FROM (
			SELECT @SurveySID SurveySID,
			       @InterviewID InterviewID
		 ) CfData
    LEFT JOIN BvInterviewTimings it ON CfData.SurveySID = it.SurveyID AND
                                       CfData.InterviewID = it.InterviewID
                                       
    DELETE FROM BvInterviewTimings
    WHERE InterviewID = @InterviewID AND
          SurveyID = @SurveySID

RETURN 0
GO
PRINT N'Creating [dbo].[BvSpGetVersion]...';


GO
 CREATE PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 17.5.0.0'
 RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpFilter_GetDependentFilters]...';


GO
CREATE PROCEDURE [dbo].[BvSpFilter_GetDependentFilters]
@FilterSID INTEGER
AS
 SELECT DISTINCT BvFilters.Name, BvFilters.SID
    FROM BvFilters, BvFilterFields  
        WHERE BvFilterFields.FilterSID = BvFilters.SID
  AND BvFilterFields.[Sign] = 8 -- subfilter
        AND CAST( BvFilterFields.[Value] AS INTEGER ) = @FilterSID
RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpClosedCellHistoryInsert]...';


GO
CREATE PROCEDURE [dbo].[BvSpClosedCellHistoryInsert]
	@SurveySid   AS INT,
	@QuotaId     AS INT,
	@CellId		 AS INT,
	@GeneratedWhereForCell AS NVARCHAR(MAX)
AS
    INSERT INTO BvClosedCellHistory(ClosingTime, SurveySid, QuotaId, CellId, GeneratedWhereForCell)
    VALUES(GETUTCDATE(), @SurveySid, @QuotaId, @CellId, @GeneratedWhereForCell)
GO
PRINT N'Creating [dbo].[BvSpInsertUpdateAudioMonitoringSession]...';


GO
CREATE PROCEDURE [dbo].[BvSpInsertUpdateAudioMonitoringSession]
	@SupervisorName nvarchar(255),
	@InterviewerId int,
	@TelephoneNumber nvarchar(255),
	@SessionId nvarchar(255)
AS
	UPDATE[AudioMonitoring]
		SET [InterviewerSID] = @InterviewerId,
			[TelephoneNumber] = @TelephoneNumber,
			[SessionID] = @SessionId
		WHERE [SupervisorName] = @SupervisorName

	IF @@ROWCOUNT = 0
	BEGIN
		INSERT INTO [AudioMonitoring] ([SupervisorName], [InterviewerSID], [TelephoneNumber], [SessionID])
			VALUES (@SupervisorName, @InterviewerId, @TelephoneNumber, @SessionId)
	END
	RETURN 0
GO
PRINT N'Creating [dbo].[BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned]
 @SurveySID INT,
 @Count  INT  --number of requested calls
AS

SET NOCOUNT ON

	DECLARE @FixeNumberCallsPerPerson INT = 10

      --it should be at end of the SP. it uses for decrease recompilation
	DECLARE @CachedCalls TABLE (
	  [InterviewID] [int] NOT NULL,
	  [OrderId] [int] NOT NULL)

	DECLARE @Calls TABLE (
	  [ExplicitSID] [int] NOT NULL,
	  [ID] [int] NOT NULL,
	  [InterviewID] [int] NOT NULL,
	  [TimeInShift] [datetime] NULL,
	  [OrderId] [int] NOT NULL,
	  [ApptID] [int] not null)
        
	;WITH orderedUpdateTable AS
	(
		SELECT calls.*, ROW_NUMBER() over (partition by ExplicitSid order by orderid) rn
		FROM BvCachedCalls calls
		where CallState = 2 AND SurveySID = @SurveySID
	)
    UPDATE orderedUpdateTable 
    SET CallState = -2 
	OUTPUT inserted.[interviewID],
		   inserted.[orderId]
	INTO @CachedCalls
    where ExplicitSid in(select sid from BvPerson ) and rn <= @FixeNumberCallsPerPerson
    
    UPDATE BvSvySchedule  
    SET CallState = -2 
	OUTPUT inserted.ExplicitSid,
		   inserted.ID,
		   inserted.InterviewID,
		   inserted.TimeInShift,
		   c.OrderId,
	       inserted.ApptId
    INTO @Calls
    FROM BvSvySchedule s 
    INNER JOIN @CachedCalls c ON (s.InterviewID = c.InterviewID AND
								  SurveySID = @SurveySID)

    SELECT c.[ID], 
           c.[ExplicitSid], 
           @SurveySID SurveySid, 
           i.DialingMode DiallingMode,
		   c.[InterviewID], 
		   i.[TelephoneNumber],
		   (CASE WHEN c.ApptId > 0 THEN c.[TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
		   0 as [GroupID]
    FROM @Calls c 
    INNER JOIN BvInterview i ON c.[Interviewid] = i.[ID] AND
                                i.[SurveySID] = @SurveySID
    ORDER BY orderid
 
RETURN (@@ROWCOUNT)
GO
PRINT N'Creating [dbo].[BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnly]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnly]
 @SurveySID INT,
 @Count  INT --number of requested calls
AS
--best if it should be established at the connection level
--it may influence on count of recompilations
SET NOCOUNT ON

	DECLARE @FixeNumberCallsPerPerson INT = 10

      --it should be at end of the SP. it uses for decrease recompilation
	DECLARE @CachedCalls TABLE (
	  [InterviewID] [int] NOT NULL,
	  [OrderId] [int] NOT NULL)

	DECLARE @Calls TABLE (
	  [ExplicitSID] [int] NOT NULL,
	  [ID] [int] NOT NULL,
	  [InterviewID] [int] NOT NULL,
	  [TimeInShift] [datetime] NULL,
	  [OrderId] [int] NOT NULL,
	  [ApptID] [int] not null)
        
	;WITH orderedUpdateTable AS
	(
		SELECT TOP ( @Count ) *
		FROM BvCachedCalls
		WHERE SurveySID = @SurveySID AND
				ExplicitSid = @SurveySID AND 
				CallState = 2
		ORDER BY OrderId
	)
    UPDATE orderedUpdateTable
    SET CallState = -2 
	OUTPUT inserted.[interviewID],
		   inserted.[orderId]
	INTO @CachedCalls
    
    UPDATE BvSvySchedule  
    SET CallState = -2 
	OUTPUT 0,
		   inserted.ID,
		   inserted.InterviewID,
		   inserted.TimeInShift,
		   c.OrderId,
	       inserted.ApptId
	INTO @Calls
    FROM BvSvySchedule s 
    INNER JOIN @CachedCalls c ON (s.InterviewID = c.InterviewID AND
								  SurveySID = @SurveySID)

    SELECT c.[ID], 
           c.[ExplicitSid], 
           @SurveySID SurveySid, 
           i.DialingMode DiallingMode,
		   c.[InterviewID], 
		   i.[TelephoneNumber],
		   (CASE WHEN c.ApptId > 0 THEN c.[TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
		   0 as [GroupID]
    FROM @Calls c
    INNER JOIN BvInterview i ON c.[Interviewid] = i.[ID] AND
                                i.[SurveySID] = @SurveySID
    ORDER BY orderid
 
RETURN (@@ROWCOUNT)
GO
PRINT N'Creating [dbo].[BvSpGetInterviewerBreaks]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetInterviewerBreaks]
	@StartDate DATETIME, @EndDate DATETIME, @MaxRows int
AS

IF(@StartDate IS NULL) SET @StartDate = '01-01-1753 00:00:00'
IF(@EndDate IS NULL) SET @EndDate = '12-31-9999 23:59:59.997'

SELECT TOP (@MaxRows)
	[h].[ID] AS [ID],
	[h].[Duration] AS [Duration],
	[h].[InterviewerId] AS [InterviewerId],
	[h].[StartTime] AS [StartTime],
	[p].[Name] AS [InterviewerName]
FROM 
	BvTimeBreaksHistory [h]
LEFT JOIN BvPerson [p] ON [p].SID = [h].[InterviewerId]
WHERE 
	[h].[StartTime] >= @StartDate AND
	[h].[StartTime] < @EndDate
          
RETURN 0
GO
PRINT N'Creating [dbo].[BvSpLookUpByPerson_ForSurvey]...';


GO
CREATE PROCEDURE [dbo].[BvSpLookUpByPerson_ForSurvey]
      @surveyId int,
      @personId int
AS
    DECLARE @CallID INT
    DECLARE @interviewId INT
    DECLARE @rowCount INT
    
    ;WITH ExplicitSIDs AS
    (
            SELECT BvPersonRel.ObjectSID FROM BvPersonRel WHERE BvPersonRel.PersonSID = @personId
    )
    ,calls AS
      (
            SELECT TOP(1) cc.*
            FROM ExplicitSIDs e
            CROSS APPLY [dbo].[GetCallBySurvey](@surveyId, e.ObjectSID ) cc
            ORDER BY Priority DESC, TimeInShift, ExplicitType DESC, CallOrder
      )
      UPDATE calls
      SET CallState = -1,
            @CallID = ID,
            @interviewId = InterviewID

      SET @rowCount = @@ROWCOUNT
      
      SELECT @CallID as CallID, @surveyId as SurveySID, @interviewId as iid WHERE @rowCount <> 0
      
      IF(@rowCount = 0) RETURN 0
      
      UPDATE BvAppointment 
      SET State = 2 
      WHERE State = 1 AND 
            SurveysId = @surveyId AND 
            InterviewSid = @interviewId
RETURN 0
GO
PRINT N'Creating [dbo].[BvSpLookUpByPerson_ForCallGroup]...';


GO
CREATE PROCEDURE [dbo].[BvSpLookUpByPerson_ForCallGroup]
	@SurveyID INT,
	@CallGroupID INT,
	@PersonID INT
AS
	DECLARE @interviewId INT
	DECLARE @rowCount INT
	DECLARE @CallID INT
	DECLARE @ConditionValue INT
		    
	;WITH ExplicitSIDs AS
	(
		SELECT BvPersonRel.ObjectSID as ExplicitSID FROM BvPersonRel WHERE BvPersonRel.PersonSID = @personId AND BvPersonRel.ObjectSID IN ( @SurveyID, @personId )
	),
	conditions AS
	(
		SELECT ExplicitSID, ConditionValue, ConditionPriority, RotatePriority FROM ExplicitSIDs
		INNER JOIN BvCallGroupConditionPerSurvey cgc ON cgc.SurveyId = @SurveyID AND cgc.CallGroupId = @CallGroupID 
	),
	calls as
	(
		SELECT TOP(1) cc.* FROM conditions c
		CROSS APPLY dbo.GetCallByCondition( @surveyId, c.ExplicitSID, c.ConditionValue ) cc
		ORDER BY Priority DESC, ConditionPriority DESC, RotatePriority ASC, TimeInShift, ExplicitType DESC, CallOrder
	)
	UPDATE calls
	SET CallState = -1,
		@CallID = ID,
		@interviewId = InterviewID,
		@surveyId = SurveySid,
		@ConditionValue = ConditionValue
	
	SET @rowCount = @@ROWCOUNT
			
	SELECT @CallID as CallID, @surveyId as SurveySID, @interviewId as iid WHERE @rowCount <> 0
		
	IF(@rowCount = 0) RETURN 0
			
	UPDATE BvCallGroupConditionPerSurvey 
		SET ConditionPriority = ConditionPriority 
		WHERE	SurveyId = @SurveyID AND
				CallGroupId = @CallGroupID AND 
				ConditionValue = @ConditionValue

	UPDATE BvAppointment 
	SET State = 2 
	WHERE State = 1 AND 
			SurveysId = @surveyId AND 
			InterviewSid = @interviewId

	
RETURN 0
GO
PRINT N'Creating [dbo].[BvSpUserSurveyPermission_Insert]...';


GO
CREATE PROCEDURE [dbo].[BvSpUserSurveyPermission_Insert]
   @UserName nvarchar( 255 ),
   @SurveyName nvarchar( 255 )
AS
   INSERT INTO BvUserSurveyPermission(UserName, SurveySID)
   SELECT @UserName, SID
   FROM BvSurvey
   WHERE Name = @SurveyName AND
         NOT EXISTS( SELECT * 
                     FROM BvUserSurveyPermission
                     WHERE UserName = @UserName AND
                           SurveySID = SID)
GO
PRINT N'Creating [dbo].[BvSpUserSurveyPermission_Get]...';


GO
CREATE PROCEDURE [dbo].[BvSpUserSurveyPermission_Get]
   @UserName nvarchar( 255 )
AS
   SELECT SurveySID
   FROM BvUserSurveyPermission
   WHERE UserName = @UserName
GO
PRINT N'Creating [dbo].[BvSpUserSurveyPermission_Delete]...';


GO
CREATE PROCEDURE [dbo].[BvSpUserSurveyPermission_Delete]
   @UserName nvarchar( 255 ),
   @SurveyName nvarchar( 255 ) = NULL
AS
   DELETE BvUserSurveyPermission
   WHERE  UserName = @UserName AND
      ((@SurveyName IS NULL) OR (SurveySID = (SELECT SID
                                              FROM BvSurvey
                                              WHERE Name = @SurveyName)))
GO
PRINT N'Creating [dbo].[BvSpUpdateInProgressCallsToScheduled]...';


GO
CREATE PROCEDURE [dbo].[BvSpUpdateInProgressCallsToScheduled]
	@surveySID	INT,
	@its		INT
AS
	SET NOCOUNT ON

	-- 1st we release call in progress (CallState -1) and fill table variable
	-- with interview's ID's
	
	UPDATE BvCachedCalls
	SET CallState = 2
	WHERE SurveySID = @surveySID AND 
	      CallState = -1

	UPDATE BvSvySchedule
    SET CallState = 2
	WHERE SurveySID = @surveySID AND 
	      CallState = -1
GO
PRINT N'Creating [dbo].[BvSpTimezone_Insert]...';


GO
CREATE PROCEDURE [dbo].[BvSpTimezone_Insert]
        @ID                 int,
        @Name               nvarchar( 255 ),
        @Bias               int,
        @DaylightType       int,
        @StandardName       nvarchar( 255 ),
        @StandardStart      datetime,
        @StandardDayOfWeek  int,
        @StandardBias       int,
        @DaylightName       nvarchar( 255 ),
        @DaylightStart      datetime,
        @DaylightDayOfWeek  int,
        @DaylightBias       int
AS
    BEGIN TRANSACTION
    IF @ID IS NULL BEGIN
        SELECT @ID = MAX( ID ) FROM BvTimezone
        IF @ID IS NULL BEGIN
            SELECT @ID = 0
        END
        SELECT @ID = @ID + 1
    END
    INSERT BvTimezone(
            ID,
            Name,
            Bias,
            DaylightType,
            StandardName,
            StandardStart,
            StandardDayOfWeek,
            StandardBias,
            DaylightName,
            DaylightStart,
            DaylightDayOfWeek,
            DaylightBias ) VALUES(
            @ID,
            @Name,
            @Bias,
            @DaylightType,
            @StandardName,
            @StandardStart,
            @StandardDayOfWeek,
            @StandardBias,
            @DaylightName,
            @DaylightStart,
            @DaylightDayOfWeek,
            @DaylightBias )

    COMMIT TRANSACTION

    RETURN @ID
GO
PRINT N'Creating [dbo].[BvSpTimezone_DeleteUnused]...';


GO
CREATE PROCEDURE [dbo].[BvSpTimezone_DeleteUnused]
    @DefaultTzId INT
AS
	DELETE FROM [BvTimezone]
	WHERE [id] <> @DefaultTzId
	AND [id] NOT IN 
		( SELECT [TimezoneID] FROM [BvInterview] WHERE [TimezoneID] IS NOT NULL GROUP BY [TimezoneID] )
	AND [id] NOT IN ( SELECT z.[TimeZoneID] FROM [BvSvySchedule] sh
						JOIN [BvShiftZones] z
						ON sh.[ShiftTypeID] = z.[id] 
						GROUP BY z.[TimeZoneID] )
	AND [id] NOT IN ( SELECT [TimezoneID] FROM [BvTimezoneShift]
					 GROUP BY [TimezoneID] )
GO
PRINT N'Creating [dbo].[BvSpTimezone_Delete]...';


GO
CREATE PROCEDURE [dbo].[BvSpTimezone_Delete]
        @ID     int,
        @Mode   int
AS

DECLARE @Rows int
DECLARE @res bit

SELECT @Rows = COUNT( * ) FROM BvTimezone WHERE ID = @ID

IF @Rows = 0 
  BEGIN
    RAISERROR( 'Timezone %i not exists', 16, 1, @ID )
    RETURN -1
  END

SELECT @Rows = COUNT( * ) FROM BvTimezoneShift WHERE TimezoneID = @ID

IF @Rows <> 0 
  BEGIN
    RAISERROR( 'Unable to delete timezone %i. Link exists on timezone shift', 12, 1, @ID )
    RETURN -1
  END

IF EXISTS( SELECT TOP 1 BvSvySchedule.[ID] 
             FROM BvSvySchedule, BvShiftZones
            WHERE BvShiftZones.TimeZoneID = @ID
                  AND BvSvySchedule.ShiftTypeID = BvShiftZones.[ID] )
BEGIN
    RAISERROR( 'Unable to delete timezone %i. Link exists on calls', 12, 1, @ID )
    RETURN -1
END

  
  SELECT @res = COUNT(*)
  FROM BvInterview
  WHERE TimezoneID = @ID
  
  IF @res <> 0
    BEGIN
      RAISERROR( 'Unable to delete timezone %i. Link exist on interview', 12, 1, @ID )
      RETURN -1
    END



BEGIN TRANSACTION

DELETE BvTimezone WHERE ID = @ID

COMMIT TRANSACTION

RETURN 0
GO
PRINT N'Creating [dbo].[BvSpTimezone_Activate]...';


GO
CREATE PROCEDURE [dbo].[BvSpTimezone_Activate]
    @TzID INT
AS
    IF NOT EXISTS( SELECT 1 FROM BvTimezoneMaster WHERE ID = @TzID )
    BEGIN
        RAISERROR( 'Timezone with ID = ''%d'' not found in master list', 16, 1, @TzID )
        RETURN -1
    END

    INSERT INTO BvTimezone 
        SELECT * FROM BvTimezoneMaster 
            WHERE ID = @TzID AND ID NOT IN( SELECT ID FROM BvTimezone )

    RETURN @@ROWCOUNT
GO
PRINT N'Creating [dbo].[BvSpTimezoneShift_Update]...';


GO
CREATE PROCEDURE [dbo].[BvSpTimezoneShift_Update]
        @OwnerSID int,
        @ShiftID int,
        @TimezoneID int,
        @StartDayOfWeek int,
        @StartTime datetime,
        @FinishDayOfWeek int,
        @FinishTime datetime,
        @Mode int

AS

DECLARE @Rows int

SELECT  @Rows = COUNT(*)
    FROM    BvShift
    WHERE   ID = @ShiftID
    AND OwnerSID = @OwnerSID
    
IF @Rows = 0
BEGIN
    RAISERROR('Shift with ShiftID = %i and OwnerSID = %i not found', 16, 1, @ShiftID, @OwnerSID)
    RETURN -1
END
IF @Rows <> 1
BEGIN
    RAISERROR('Multiple shifts with ShiftID = %i and OwnerSID = %i found', 16, 1, @ShiftID, @OwnerSID)
    RETURN -1
END
    
UPDATE  BvTimezoneShift
    SET StartDayOfWeek = @StartDayOfWeek, 
        StartTime = @StartTime,
        FinishDayOfWeek = @FinishDayOfWeek,
        FinishTime = @FinishTime
    WHERE   OwnerSID = @OwnerSID
    AND ShiftID = @ShiftID
    AND TimezoneID = @TimezoneID
return 0
GO
PRINT N'Creating [dbo].[BvSpTimezoneShift_Insert]...';


GO
CREATE PROCEDURE [dbo].[BvSpTimezoneShift_Insert]
        @OwnerSID int,
        @ShiftID int,
        @TimezoneID int,
        @StartDayOfWeek int,
        @StartTime datetime,
        @FinishDayOfWeek int,
        @FinishTime datetime

AS

DECLARE @Rows int

SELECT  @Rows = COUNT(*)
    FROM    BvTimezoneShift
    WHERE   TimezoneID = @TimezoneID
    AND ShiftID = @ShiftID
    AND OwnerSID = @OwnerSID
IF @Rows <> 0
--  return 50001    /* BVDBS_STORED_PROCEDURE_DUPLICATED_OBJECT */
    return 0

INSERT  BvTimezoneShift( 
        OwnerSID, 
        ShiftID, 
        StartDayOfWeek,
        StartTime,
        FinishDayOfWeek,
        FinishTime,
        TimezoneID )
    VALUES( @OwnerSID, 
        @ShiftID, 
        @StartDayOfWeek,
        @StartTime,
        @FinishDayOfWeek,
        @FinishTime,
        @TimezoneID )
return  @TimezoneID
GO
PRINT N'Creating [dbo].[BvSpTimezoneShift_Delete]...';


GO
CREATE PROCEDURE [dbo].[BvSpTimezoneShift_Delete]
        @OwnerSID int,
        @ShiftID int,
        @TimezoneID int,
        @Mode int

AS
DECLARE @Rows int
SELECT  @Rows = COUNT( * )
    FROM    BvTimezoneShift
    WHERE   OwnerSID = @OwnerSID
    AND ShiftID = @ShiftID
    AND TimezoneID = @TimezoneID
    
IF @Rows = 0
BEGIN
    RAISERROR('Shift with ShiftID = %i and OwnerSID = %i not found', 16, 1, @ShiftID, @OwnerSID)
    RETURN -1
END
IF @Rows <> 1
BEGIN
    RAISERROR('Multiple shifts with ShiftID = %i and OwnerSID = %i found', 16, 1, @ShiftID, @OwnerSID)
    RETURN -1
END
    
DELETE  BvTimezoneShift
    WHERE   OwnerSID = @OwnerSID
    AND ShiftID = @ShiftID
    AND     TimezoneID = @TimezoneID
return 0
GO
PRINT N'Creating [dbo].[BvSpTimezoneMaster_Insert]...';


GO
CREATE PROCEDURE [dbo].[BvSpTimezoneMaster_Insert]
        @ID                 int,
        @Name               nvarchar( 255 ),
        @Bias               int,
        @DaylightType       int,
        @StandardName       nvarchar( 255 ),
        @StandardStart      datetime,
        @StandardDayOfWeek  int,
        @StandardBias       int,
        @DaylightName       nvarchar( 255 ),
        @DaylightStart      datetime,
        @DaylightDayOfWeek  int,
        @DaylightBias       int
AS

    BEGIN TRANSACTION
    IF @ID IS NULL BEGIN
        SELECT @ID = MAX( ID ) FROM BvTimezoneMaster
        IF @ID IS NULL BEGIN
            SELECT @ID = 0
        END
        SELECT @ID = @ID + 1
    END
    INSERT BvTimezoneMaster(
            ID,
            Name,
            Bias,
            DaylightType,
            StandardName,
            StandardStart,
            StandardDayOfWeek,
            StandardBias,
            DaylightName,
            DaylightStart,
            DaylightDayOfWeek,
            DaylightBias ) VALUES(
            @ID,
            @Name,
            @Bias,
            @DaylightType,
            @StandardName,
            @StandardStart,
            @StandardDayOfWeek,
            @StandardBias,
            @DaylightName,
            @DaylightStart,
            @DaylightDayOfWeek,
            @DaylightBias )
    COMMIT TRANSACTION

    RETURN @ID
GO
PRINT N'Creating [dbo].[BvSpTimezoneMaster_Get]...';


GO
CREATE PROCEDURE [dbo].[BvSpTimezoneMaster_Get]
        @ID int
AS

IF @ID = 0 BEGIN
    SELECT  
  ID,
        Name,
        Bias,
        DaylightType,
        StandardName,
        StandardStart,
        StandardDayOfWeek,
        StandardBias,
        DaylightName,
        DaylightStart,
        DaylightDayOfWeek,
        DaylightBias
    FROM 
  BvTimezoneMaster 
 WHERE 
  ID NOT IN(
   SELECT ID
   FROM BvTimezone
   )
END
ELSE BEGIN
    SELECT
  ID,
        Name,
        Bias,
        DaylightType,
        StandardName,
        StandardStart,
        StandardDayOfWeek,
        StandardBias,
        DaylightName,
        DaylightStart,
        DaylightDayOfWeek,
        DaylightBias
    FROM
  BvTimezoneMaster 
    WHERE
  ID = @ID
END
GO
PRINT N'Creating [dbo].[BvSpThreshold_List]...';


GO
CREATE PROCEDURE [dbo].[BvSpThreshold_List]
    @ObjectSID INT
AS
    IF @ObjectSID <> 0
    BEGIN
        RAISERROR( 'ObjectSID reserved. Must be zero.', 16, 1 )
        RETURN(0)
    END

    SELECT  ObjectSID,
            ThresholdsTypeID,
            Amber,
            Red
        FROM dbo.BvThresholds WHERE ObjectSID = @ObjectSID

    RETURN( 0 )
GO
PRINT N'Creating [dbo].[BvSpThresholds_insert]...';


GO
CREATE PROCEDURE BvSpThresholds_insert
   @ObjectSID INT,
   @ThresholdsTypeID INT,
   @Amber INT,
   @Red INT
AS
   UPDATE BvThresholds
      SET Amber = @Amber,
          Red = @Red
      WHERE ObjectSID = @ObjectSID AND
            ThresholdsTypeID = @ThresholdsTypeID
   IF @@ROWCOUNT = 0
   INSERT INTO BvThresholds
   VALUES(@ObjectSID, @ThresholdsTypeID, @Amber, @Red)
GO
PRINT N'Creating [dbo].[BvSpThresholds_delete]...';


GO
CREATE PROCEDURE BvSpThresholds_delete
   @ObjectSID INT,
   @ThresholdsTypeID INT
AS
   DELETE BvThresholds
   WHERE ObjectSID = @ObjectSID AND
         ThresholdsTypeID = @ThresholdsTypeID
GO
PRINT N'Creating [dbo].[BvSpThresholdITS_Set]...';


GO
CREATE PROCEDURE [dbo].[BvSpThresholdITS_Set]
    @SurveySID INT,
    @ITS       INT,
    @Amber     INT,
    @Red       INT
AS
    IF @SurveySID <> 0 
    BEGIN
        RAISERROR( 'SurveySID reserved, must be zero', 16, 1 )
        RETURN (-1)
    END

    INSERT INTO BvThresholdITS( SurveySID, ITS, Amber, Red ) 
        SELECT @SurveySID, @ITS, @Amber, @Red 
            WHERE NOT EXISTS( SELECT 1 FROM BvThresholdITS WHERE SurveySID = @SurveySID AND ITS = @ITS )

    IF @@ROWCOUNT = 0 
        UPDATE BvThresholdITS
            SET Amber = @Amber,
                Red   = @Red
            WHERE SurveySID = @SurveySID AND ITS = @ITS

    UPDATE BvSampleStatusSummary
        SET alertStatus = dbo.udf_AlertStatus_INT( BvSampleStatusSummary.Cnt, @Amber, @Red )
        WHERE ITS = @ITS
GO
PRINT N'Creating [dbo].[BvSpThresholdITS_List]...';


GO
CREATE PROCEDURE [dbo].[BvSpThresholdITS_List]
    @SurveySID INT
AS

    IF @SurveySID <> 0 
    BEGIN
        RAISERROR( 'SurveySID reserved. Must be zero', 16, 1 )
        RETURN (-1 )
    END

    DECLARE @DefaultStateGroupSID INT
    SELECT @DefaultStateGroupSID = ID FROM BvStateGroup WHERE Name = 'Default group'

    SELECT  BvThresholdITS.SurveySID as SurveySID,
            BvThresholdITS.ITS AS ITS,
            BvState.Name as Name,
            BvThresholdITS.Amber as Amber,
            BvThresholdITS.Red as Red
        FROM BvThresholdITS 
        INNER JOIN BvState
        ON BvThresholdITS.ITS = BvState.StateID AND BvState.StateGroupID = @DefaultStateGroupSID
       WHERE SurveySID = @SurveySID

    RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpThresholdITS_Delete]...';


GO
CREATE PROCEDURE [dbo].[BvSpThresholdITS_Delete]
    @SurveySID INT,
    @ITS       INT
AS
    IF @SurveySID <> 0 
    BEGIN
        RAISERROR( 'SurveySID reserved, must be zero', 16, 1 )
        RETURN (-1)
    END

    IF @SurveySID = 0 
    BEGIN
        EXEC BvSpThresholdITS_Set 0, @ITS, 2147483647, 2147483647
    END
    ELSE
    BEGIN
        DELETE FROM BvThresholdITS 
            WHERE SurveySID = @SurveySID AND ITS = @ITS

        DECLARE @DefAmber INT
        DECLARE @DefRed INT

        SELECT @DefAmber = Amber, @DefRed = Red FROM BvThresholdITS 
            WHERE SurveySID = 0 AND ITS = @ITS

        UPDATE BvSampleStatusSummary
            SET alertStatus = dbo.udf_AlertStatus_INT( BvSampleStatusSummary.Cnt, @DefAmber, @DefRed )
            WHERE SurveySID = @SurveySID AND ITS = @ITS
    END
GO
PRINT N'Creating [dbo].[BvSpTasks_Update_2]...';


GO
CREATE PROCEDURE [dbo].[BvSpTasks_Update_2]
 @PersonSID int,
 @SurveySID int,
 @InterviewID int,
 @InterviewState tinyint,
 @TimeCallDelivered DATETIME,
 @CallOutcome int,
 @TzID int
AS

DECLARE @DialMode INT
SELECT @DialMode = DialingMode
FROM BvInterview
WHERE SurveySID = @SurveySID AND
      ID = @InterviewID

IF( @DialMode IS NULL OR @DialMode = 0)
BEGIN
	SELECT @DialMode = DialMode
	FROM BvSurvey WHERE SID = @SurveySID
	      
	SET @DialMode = ISNULL(@DialMode, 1) --BY DEFAULT (MANUAL)
END

IF( @SurveySID = 0 OR @InterviewID = 0 OR @InterviewState = 0)
BEGIN
 UPDATE [dbo].[BvTasks]
  SET SurveySID = @SurveySID,
   InterviewID = 0,
   InterviewState = @InterviewState,
   CallOutcome = (CASE WHEN @SurveySID = 0 THEN -1 ELSE @CallOutcome END),
   TzID = 0,
   CallID  = 0,
   SecondsSinceLastSubmission = 0,
   TimeStateChanged = GETUTCDATE(),
   TimeCallDelivered = @TimeCallDelivered,
   DiallingMode = @DialMode
 WHERE PersonSID = @PersonSID
END
ELSE BEGIN
 UPDATE [dbo].[BvTasks]
  SET SurveySID = @SurveySID,
   InterviewID = @InterviewID,
   TimeStateChanged = GETUTCDATE(),
   InterviewState = @InterviewState,
   TimeCallDelivered = @TimeCallDelivered,
   CallOutcome = @CallOutcome,
   TzID = @TzID,
   DiallingMode = @DialMode
 WHERE PersonSID = @PersonSID
END

SELECT @@ROWCOUNT AS [RowCount], CallId from BvTasks
 WHERE PersonSID = @PersonSID
GO
PRINT N'Creating [dbo].[BvSpTasks_UpdateStatusLogout]...';


GO
CREATE PROCEDURE [dbo].[BvSpTasks_UpdateStatusLogout]
 @PersonSID int,
 @StatusLogout tinyint
AS

DECLARE @PreviousStatusLogout TINYINT

UPDATE [dbo].[BvTasks]
SET StatusLogout = @StatusLogout,
    @PreviousStatusLogout = StatusLogout
WHERE PersonSID = @PersonSID

SELECT t.InterviewID, 
       t.LoggedInToDialerState, 
       t.IsLoginRCToDialer, 
       ISNULL(s.[Name], '') AS [ProjectID], 
       @PreviousStatusLogout PreviousStatusLogout,
       t.StartTime,
       t.SurveySid,
       t.DiallingMode
 FROM BvTasks t
 LEFT JOIN BvSurvey s
 ON t.SurveySID = s.[Sid]
 WHERE t.PersonSID = @PersonSID

RETURN 0
GO
PRINT N'Creating [dbo].[BvSpTasks_UpdateProblemState]...';


GO
CREATE PROCEDURE [dbo].[BvSpTasks_UpdateProblemState]
 @PersonSID int,
 @ProblemId int
AS

UPDATE [dbo].[BvTasks]
    SET ProblemId = @ProblemId
WHERE PersonSID = @PersonSID

RETURN 0
GO
PRINT N'Creating [dbo].[BvSpTasks_UpdateLoggedInToDialerState]...';


GO
CREATE PROCEDURE [dbo].[BvSpTasks_UpdateLoggedInToDialerState]
 @PersonSID int,
 @LoggedInToDialerState tinyint
AS

UPDATE [dbo].[BvTasks]
    SET LoggedInToDialerState = @LoggedInToDialerState
WHERE PersonSID = @PersonSID

RETURN 0
GO
PRINT N'Creating [dbo].[BvSpTasks_UpdateKeepAlive]...';


GO
CREATE PROCEDURE [dbo].[BvSpTasks_UpdateKeepAlive]
 @PersonSID int
AS

DECLARE @Now DATETIME
SET @Now = GETUTCdate()

UPDATE [dbo].[BvTasks]
    SET LastKeepAliveTime = @Now
WHERE PersonSID = @PersonSID

RETURN @@ROWCOUNT
GO
PRINT N'Creating [dbo].[BvSpTasks_UpdateInterviewState]...';


GO
CREATE PROCEDURE [dbo].[BvSpTasks_UpdateInterviewState]
 @PersonSID int,
 @InterviewState int
AS

IF @InterviewState = 0 --NO_CALLS
BEGIN

 UPDATE [dbo].[BvTasks]
     SET 
      InterviewID = 0, 
      CallID = 0,
      TzID = 0,
      LastSubmissionAlert = 0,
      SecondsSinceLastSubmission = 0,
      TimeStateChanged = GETUTCDATE(),
      TimeCallDelivered = NULL,
      InterviewState = @InterviewState,
      DiallingMode = ISNULL( (SELECT DialMode FROM BvSurvey
                  WHERE BvSurvey.SID = BvTasks.SurveySID ), 1 ) --BY DEFAULT (MANUAL)
 WHERE PersonSID = @PersonSID
END
ELSE 
IF @InterviewState = 6 --INTERVIEW_WRAP_UP 
BEGIN
 UPDATE [dbo].[BvTasks]
     SET InterviewState = @InterviewState,
      State = null,
      TimeStateChanged = GETUTCDATE()
 WHERE PersonSID = @PersonSID
END
ELSE
BEGIN
 UPDATE [dbo].[BvTasks]
     SET InterviewState = @InterviewState
 WHERE PersonSID = @PersonSID
END

RETURN @@ROWCOUNT
GO
PRINT N'Creating [dbo].[BvSpTasks_UpdateCallOutcome]...';


GO
CREATE PROCEDURE BvSpTasks_UpdateCallOutcome
   @PersonSID INT,
   @CallOutcome INT
AS
   UPDATE BvTasks
   SET CallOutcome = @CallOutcome
   WHERE PersonSID = @PersonSID

RETURN @@ROWCOUNT
GO
PRINT N'Creating [dbo].[BvSpTasks_UnLockByPerson]...';


GO
CREATE PROCEDURE [dbo].[BvSpTasks_UnLockByPerson]
        @PersonSID INT
    AS
       UPDATE BvTasks
          SET LockTime = NULL
        WHERE PersonSID = @PersonSID


       RETURN @@ROWCOUNT
GO
PRINT N'Creating [dbo].[BvSpTasks_SetTelephonyProblemForLoggedIn]...';


GO
CREATE PROCEDURE [dbo].[BvSpTasks_SetTelephonyProblemForLoggedIn]
@DialerId INT,
@ProblemCode INT 
AS
IF (@DialerId = 0)
BEGIN -- proceed for all dialers
	UPDATE BvTasks SET [ProblemId] = @ProblemCode
	 WHERE ([LoggedInToDialerState] = 2 /* LoginState.LOGGED_IN */
	  OR [LoggedInToDialerState] = 1 /* LoginState.LOGGING_IN */)
END
ELSE
BEGIN -- proceed for concrete dialer
	UPDATE BvTasks SET [ProblemId] = @ProblemCode
	 WHERE ([LoggedInToDialerState] = 2 /* LoginState.LOGGED_IN */
	  OR [LoggedInToDialerState] = 1 /* LoginState.LOGGING_IN */)
	  AND PersonSID IN (SELECT SID FROM BvPerson WHERE DialerId = @DialerId)
END
GO
PRINT N'Creating [dbo].[BvSpTasks_LockByPerson]...';


GO
CREATE PROCEDURE [dbo].[BvSpTasks_LockByPerson]
        @PersonSID INT,
        @OldLockTime DATETIME OUTPUT
    AS
       UPDATE BvTasks
          SET @OldLockTime = LockTime,
              LockTime = GETDATE()
        WHERE PersonSID = @PersonSID


       RETURN @@ROWCOUNT
GO
PRINT N'Creating [dbo].[BvSpTasks_LockByInterview]...';


GO
CREATE PROCEDURE [dbo].[BvSpTasks_LockByInterview]
        @SurveySID INT,
        @InterviewID INT,
        @PersonSID INT OUTPUT,
        @OldLockTime DATETIME OUTPUT
    AS
       UPDATE BvTasks
          SET LockTime = GETDATE(),
              @OldLockTime = LockTime,
              @PersonSID = PersonSID
          WHERE SurveySID = @SurveySID AND InterviewID = @InterviewID 


       RETURN @@ROWCOUNT
GO
PRINT N'Creating [dbo].[BvSpTasks_InsertUpdate_2]...';


GO
CREATE PROCEDURE [dbo].[BvSpTasks_InsertUpdate_2]
 @PersonSID int,
 @SurveySID int,
 @ExtensionNumber NVARCHAR(256),
 @LoggedInToDialerState tinyint,
 @IsLoginRCToDialer BIT
AS

DECLARE @Now DATETIME
SET @Now = GETUTCdate()

declare @DiallingMode int

SELECT @DiallingMode = DialMode
FROM BvSurvey WHERE SID = @SurveySID
SET @DiallingMode = ISNULL(@DiallingMode, 1)  --BY DEFAULT 1 (manual)

UPDATE [dbo].[BvTasks]
    SET TimeStateChanged = @Now,
	    SurveySID = @SurveySID,
	    InterviewID = 0,
        StatusLogout = 2, --LOGGED_IN
        LoggedInToDialerState = @LoggedInToDialerState,
        IsLoginRCToDialer = @IsLoginRCToDialer,
        DiallingMode = @DiallingMode
WHERE PersonSID = @PersonSID

UPDATE [dbo].[BvPerson]
 SET ExtensionNumber = @ExtensionNumber
WHERE SID = @PersonSID

SELECT MNDiallerUserId
 FROM BvPerson
  WHERE SID = @PersonSID

RETURN 0
GO
PRINT N'Creating [dbo].[BvSpSynchronizeAggregateData]...';


GO
CREATE PROCEDURE BvSpSynchronizeAggregateData
AS

SET DEADLOCK_PRIORITY -10

DECLARE @MaxIdlePeriod INT
SELECT @MaxIdlePeriod = IdlePeriodMaxSeconds
FROM BvSurveyListAlertsViewConfiguration

   IF EXISTS( SELECT *
			  FROM BvTasks
			  WHERE SecondsSinceLastSubmission != 0 AND
			        SecondsSinceLastSubmission <= @MaxIdlePeriod )
   BEGIN
      UPDATE BvSurveyListAlertsViewConfiguration
      SET IdlePeriodCheckCounter = 0
      RETURN;
   END
   ELSE BEGIN
      DECLARE @IdlePeriodCheckCounter INT
      DECLARE @IdlePeriodMaxCountOfChecks INT
      UPDATE BvSurveyListAlertsViewConfiguration
      SET IdlePeriodCheckCounter = IdlePeriodCheckCounter+1,
          @IdlePeriodCheckCounter = IdlePeriodCheckCounter+1,
          @IdlePeriodMaxCountOfChecks = IdlePeriodMaxCountOfChecks
          
      IF @IdlePeriodCheckCounter < @IdlePeriodMaxCountOfChecks
      BEGIN
		RETURN
	  END
	  
	  UPDATE BvSurveyListAlertsViewConfiguration
      SET IdlePeriodCheckCounter = 0
   END
			        

   UPDATE BvAggregateSurvey
   SET ScheduledCallsCount = ISNULL(scheduledCalls.CallsCount, 0),
       MinutesSpentWorkingOnSurvey = ISNULL((SELECT ISNULL(SUM(WaitingTime), 0) + ISNULL(SUM(Duration), 0)
                                      FROM BvHistory h
                                      WHERE h.SurveyId = SID AND
                                            h.RoleId = 2), 0)
   FROM BvAggregateSurvey
   LEFT JOIN (SELECT ss.SurveySID SurveySID, count(ss.ID) CallsCount
              FROM BvSvySchedule ss
              WHERE ss.CallState > 0 or ss.CallState = -2 --predictive mode
              GROUP BY ss.SurveySID) as scheduledCalls ON (SID = scheduledCalls.SurveySID)



   UPDATE BvAggregateSurvey
   SET SuspendedCallsCount = cnt
   FROM BvAggregateSurvey, 
        (SELECT SurveySID, COUNT(ID) cnt
         FROM BvInterview
         GROUP BY SurveySID) as i
   WHERE SID = SurveySID
GO
PRINT N'Creating [dbo].[BvSpSvySch_Delete]...';


GO
CREATE PROCEDURE [dbo].[BvSpSvySch_Delete]
@SurveySID      INTEGER,
@InterviewID    INTEGER
AS
	-- delete calls
	UPDATE BvSvySchedule 
	SET CallState = 0
	WHERE SurveySID = @SurveySID AND
			InterviewID = @InterviewID

	UPDATE BvAppointment
	SET STATE = 2
	WHERE SurveySID = @SurveySID AND
		InterviewSID = @InterviewID

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpSurvey_IsPersonAssigned]...';


GO
CREATE PROCEDURE [dbo].[BvSpSurvey_IsPersonAssigned]
    @SurveySID INT,
 @PersonSID INT
AS
  
SELECT a.Id from dbo.BvPersonOrGroupAssignmentOnSurvey a 
 WHERE PersonOrGroupId = @PersonSID and SurveyId = @SurveySID

return (0)
GO
PRINT N'Creating [dbo].[BvSpSurvey_GetOpened]...';


GO
CREATE PROCEDURE [dbo].[BvSpSurvey_GetOpened]
AS
 
 SELECT
     BvSurvey.SID
 FROM
     BvSurvey
    WHERE
        BvSurvey.State = 1

RETURN 0
GO
PRINT N'Creating [dbo].[BvSpSurvey_GetListByFolder]...';


GO
CREATE  procedure [dbo].[BvSpSurvey_GetListByFolder]
 @UserName NVARCHAR(MAX) = NULL,
 @Filter NVARCHAR(MAX) = NULL

as

SELECT  
        BvSurvey.SID    AS    [SID],
        BvSurvey.Name   AS    [ConfirmitID],
        BvSurvey.Description AS [Name], 
  (select count(distinct BvPersonOrGroupAssignmentOnSurvey.PersonOrGroupId) from BvPersonOrGroupAssignmentOnSurvey
        where BvPersonOrGroupAssignmentOnSurvey.SurveyId = BvSurvey.[SID]) 
         as TotalAssignedPersons 
FROM    BvSurvey
INNER JOIN [bvUserSurveyPermission] [p] ON BvSurvey.SID = [p].SurveySID
WHERE  p.UserName = @UserName AND 
       BvSurvey.[Description] <> '' AND 
       (@Filter IS NULL OR BvSurvey.[Description] LIKE @Filter + '%') AND
	   BvSurvey.State <> 2
GO
PRINT N'Creating [dbo].[BvSpSurvey_GetAssignedPersonList]...';


GO
CREATE PROCEDURE [dbo].[BvSpSurvey_GetAssignedPersonList]
    @SurveySID INT,
    @RoleID INT
AS
 SELECT 
      p.SID AS PersonId,
      p.Name AS PersonName
  FROM BvPerson p with( nolock ), BvPersonRel r with(
  nolock ), BvSurvey s with( nolock )
  where p.SID = r.PersonSID and r.Type = 2 and r.RoleID = @RoleID and
  r.ObjectSID = s.SID and s.SID = @SurveySID
  ORDER BY p.SID
GO
PRINT N'Creating [dbo].[BvSpSurvey_DeleteFiltered]...';


GO
CREATE PROCEDURE [dbo].[BvSpSurvey_DeleteFiltered]
@SurveySID INT,
@BatchID INT
AS
    
    DECLARE @deletedrecords table(ApptID INT)
    
    DELETE FROM BvCachedCalls
    FROM BvTransferArrays
    WHERE BvTransferArrays.BatchID = @BatchID AND 
          BvTransferArrays.ItemID = BvCachedCalls.[ID] AND
          (CallState > 0 OR CallState = -2)

    DELETE FROM BvSvySchedule
    OUTPUT DELETED.ApptID
    INTO @deletedrecords
    FROM BvTransferArrays
    WHERE BvTransferArrays.BatchID = @BatchID AND 
          BvTransferArrays.ItemID = BvSvySchedule.[ID] AND
          (CallState > 0 OR CallState = -2)

   UPDATE BvAppointment
   SET State = 2
   FROM @deletedRecords d
   WHERE d.ApptID = BvAppointment.ID

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpSurvey_Close]...';


GO
CREATE PROCEDURE [dbo].[BvSpSurvey_Close]
@SurveySID INTEGER
AS
DELETE FROM BvCachedCalls 
WHERE SurveySID = @SurveySID AND
      (CallState > 0 OR CallState = -2)
RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpSurveyState_Update]...';


GO
CREATE PROCEDURE [dbo].[BvSpSurveyState_Update]
 @ObjectID INT,
 @StateGroupID INT,
 @Priority INT
AS
 UPDATE BvSvySchedule SET Priority = @Priority
  FROM BvSvySchedule c
        INNER JOIN BvSurvey s 
   ON c.SurveySID = s.SID
            AND s.StateGroupID = @StateGroupID
        INNER JOIN BvInterview i 
   ON i.SurveySID = c.SurveySID
            AND i.[ID] = c.InterviewID
            AND i.TransientState = @ObjectID

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpSurveyModifyStateGroup]...';


GO
CREATE PROCEDURE [dbo].[BvSpSurveyModifyStateGroup]
@SurveySID     INTEGER,
@StateGroupID  INTEGER
AS
DECLARE @OldStateGroupID  INTEGER

     SELECT @OldStateGroupID = StateGroupID FROM BvSurvey WHERE SID = @SurveySID
     IF @OldStateGroupID <> @StateGroupID
     BEGIN
          UPDATE BvSvySchedule SET Priority = st.Priority
          FROM BvSvySchedule c
          INNER JOIN BvInterview i ON c.SurveySID = i.SurveySID
            AND c.InterviewID = i.[ID]
          INNER JOIN BvState st ON st.StateGroupID = @StateGroupID
            AND i.TransientState = st.StateID
          WHERE c.SurveySID = @SurveySID


          insert into BvCachedCallsInsert
          select c.InterviewID, @SurveySID
          from BvSvySchedule c
          where c.SurveySID = @SurveySID
     END

RETURN ( 0 )
GO
PRINT N'Creating [dbo].[BvSpState_ListBySurvey]...';


GO
CREATE PROCEDURE [dbo].[BvSpState_ListBySurvey]
	@SurveySID int
AS

SELECT [StateID], [Name], [Priority], [DA] FROM [BvState]
     WHERE [StateGroupID] = (
		SELECT [StateGroupID] FROM [BvSurvey] WHERE [SID] = @SurveySID )
     ORDER BY [StateID]
GO
PRINT N'Creating [dbo].[BvSpState_ListByGroup]...';


GO
CREATE PROCEDURE [dbo].[BvSpState_ListByGroup]
	@StateGroupID int
AS

-- if default group
IF @StateGroupID = 0
BEGIN
     DECLARE @MinOrder INTEGER
     SELECT @MinOrder = MIN([Order]) FROM [BvStateGroup] 
     SELECT @StateGroupID = [ID] FROM [BvStateGroup] WHERE [Order] = @MinOrder
END

SELECT [StateID], [Name], [Priority], [DA]  FROM [BvState] 
     WHERE [StateGroupID] = @StateGroupID  
     ORDER BY [StateID]
GO
PRINT N'Creating [dbo].[BvSpState_List]...';


GO
CREATE PROCEDURE [dbo].[BvSpState_List]
@ObjectID     INTEGER
AS
DECLARE @StateGroupID INTEGER

SET @StateGroupID = 0

-- if default group
IF @ObjectID = 0
BEGIN
     DECLARE @MinOrder INTEGER
     SELECT @MinOrder = MIN([Order]) FROM BvStateGroup 
     SELECT @StateGroupID = [ID] FROM BvStateGroup WHERE [Order] = @MinOrder
END
-- if @ObjectID is a SurveySID
ELSE IF EXISTS( SELECT * FROM BvSurvey WHERE SID = @ObjectID AND State <> 2 )
     SELECT @StateGroupID = StateGroupID FROM BvSurvey WHERE SID = @ObjectID
-- if bad id
ELSE IF NOT EXISTS( SELECT * FROM BvStateGroup WHERE [ID] = @ObjectID )
     RETURN -1
ELSE
    SET @StateGroupID = @ObjectID

SELECT StateID, [Name], Priority, [DA]  FROM BvState 
     WHERE StateGroupID = @StateGroupID  ORDER BY StateID
GO
PRINT N'Creating [dbo].[BvSpStateGroup_Update]...';


GO
CREATE PROCEDURE [dbo].[BvSpStateGroup_Update]
@ObjectSID INTEGER,
@Name      VARCHAR(255)
AS

     UPDATE BvStateGroup SET [Name] = @Name WHERE [ID] = @ObjectSID

RETURN ( 0 )
GO
PRINT N'Creating [dbo].[BvSpStateGroup_Insert]...';


GO
CREATE PROCEDURE [dbo].[BvSpStateGroup_Insert]
    @SID     INT,
    @CopyID  INT,
    @Name    VARCHAR(255)
AS
DECLARE @Order INTEGER

    IF NOT EXISTS( SELECT * FROM BvStateGroup )
    BEGIN
		RAISERROR('Default state group not found.', 16, 1)
		RETURN -1
	END

    -- if @ParentSID = 0 then find default group
    IF @CopyID = 0
    BEGIN
        SELECT @Order = MIN([Order] ) FROM BvStateGroup
        SELECT @CopyID = ISNULL( ID, 0 ) FROM BvStateGroup WHERE [Order] =@Order
    END

     SELECT @Order = MAX([Order] ) FROM BvStateGroup    
     SET @Order = @Order + 1

    -- Insert new state group
    INSERT INTO BvStateGroup(
        [ID],
        [Name],
        [Order],
        [Deleted])
    VALUES (
        @SID, 
        @Name,
        @Order,
        0)

    -- Copy States   
     INSERT INTO BvState( StateID, [Name], StateGroupID, Priority, DA )
         SELECT StateID, [Name], @SID, Priority, DA FROM BvState WHERE StateGroupID = @CopyID

RETURN 0
GO
PRINT N'Creating [dbo].[BvSpShift_Update]...';


GO
CREATE PROCEDURE [dbo].[BvSpShift_Update]
        @OwnerSID int,
        @OldID int,
        @NewID int,
        @CycleType int,
        @StartDayOfWeek int,
        @StartTime datetime,
        @FinishDayOfWeek int,
        @FinishTime datetime,
        @ShiftTypeID int,
        @Mode int

AS

IF ( @OldID <> @NewID ) BEGIN
    UPDATE  BvTimezoneShift
        SET ShiftID = @NewID
        WHERE   OwnerSID = @OwnerSID
        AND ShiftID = @OldID
    UPDATE  BvShift
        SET ID = @NewID,
            CycleType = @CycleType,
            StartDayOfWeek = @StartDayOfWeek, 
            StartTime = @StartTime,
            FinishDayOfWeek = @FinishDayOfWeek,
            FinishTime = @FinishTime,
            ShiftTypeID = @ShiftTypeID
        WHERE   OwnerSID = @OwnerSID
        AND ID = @OldID
END
ELSE BEGIN
    UPDATE  BvShift
        SET CycleType = @CycleType,
            StartDayOfWeek = @StartDayOfWeek, 
            StartTime = @StartTime,
            FinishDayOfWeek = @FinishDayOfWeek,
            FinishTime = @FinishTime,
            ShiftTypeID = @ShiftTypeID
        WHERE   OwnerSID = @OwnerSID
        AND ID = @OldID
END

return 0
GO
PRINT N'Creating [dbo].[BvSpShift_List]...';


GO
CREATE PROCEDURE [dbo].[BvSpShift_List]
        @OwnerSID int,
        @ID int,
        @TimezoneID int      
AS

IF @TimezoneID = 0 BEGIN    
    IF @ID = 0 BEGIN
        SELECT  ID,
            CycleType,
            StartDayOfWeek,
            StartTime,
            FinishDayOfWeek,
            FinishTime,
            (   SELECT  COUNT(*) 
                    FROM    BvTimezoneShift 
                    WHERE   OwnerSID = @OwnerSID 
                    AND     ShiftID = ID ) TimezoneID,
            ShiftTypeID
            FROM    BvShift
            WHERE   OwnerSID = @OwnerSID
            ORDER BY BvShift.ID
    END
    ELSE BEGIN
        SELECT  @ID ID,
            CycleType,
            StartDayOfWeek,
            StartTime,
            FinishDayOfWeek,
            FinishTime,
            0  TimezoneID,
            ShiftTypeID
            FROM    BvShift
            WHERE   OwnerSID = @OwnerSID
            AND     ID = @ID
        UNION
        SELECT  @ID ID,
            BvShift.CycleType,
            BvTimezoneShift.StartDayOfWeek,
            BvTimezoneShift.StartTime,
            BvTimezoneShift.FinishDayOfWeek,
            BvTimezoneShift.FinishTime,
            BvTimezoneShift.TimezoneID TimezoneID,
            BvShift.ShiftTypeID
            FROM    BvShift 
            JOIN    BvTimezoneShift     
                ON  BvShift.ID = BvTimezoneShift.ShiftID
                AND BvTimezoneShift.OwnerSID = @OwnerSID
            WHERE   BvShift.ID = @ID
                AND BvShift.OwnerSID = @OwnerSID
        ORDER   BY  TimezoneID
    END 
END ELSE IF @TimezoneID > 0 BEGIN
    IF @ID = 0 BEGIN
        SELECT  BvShift.ID,
            BvShift.CycleType,
            ISNULL( BvTimezoneShift.StartDayOfWeek, BvShift.StartDayOfWeek ) StartDayOfWeek,
            ISNULL( BvTimezoneShift.StartTime, BvShift.StartTime ) StartTime,
            ISNULL( BvTimezoneShift.FinishDayOfWeek, BvShift.FinishDayOfWeek ) FinishDayOfWeek,
            ISNULL( BvTimezoneShift.FinishTime, BvShift.FinishTime ) FinishTime,
            ISNULL( BvTimezoneShift.TimezoneID, 0 ) TimezoneID,
            BvShift.ShiftTypeID
            FROM    BvShift
            LEFT JOIN BvTimezoneShift ON BvShift.ID = BvTimezoneShift.ShiftID 
                        AND BvShift.OwnerSID = BvTimezoneShift.OwnerSID 
                        AND BvTimezoneShift.TimezoneID = @TimezoneID        
            WHERE   BvShift.OwnerSID = @OwnerSID
            ORDER BY BvShift.ID
    END
    ELSE BEGIN
        SELECT  ID,
            CycleType,
            StartDayOfWeek,
            StartTime,
            FinishDayOfWeek,
            FinishTime,
            0 TimezoneID,
            ShiftTypeID
            FROM    BvShift
            WHERE   OwnerSID = @OwnerSID
            AND     ID = @ID
        UNION
        SELECT  BvTimezoneShift.ShiftID ID,
            BvShift.CycleType,
            BvTimezoneShift.StartDayOfWeek,
            BvTimezoneShift.StartTime,
            BvTimezoneShift.FinishDayOfWeek,
            BvTimezoneShift.FinishTime,
            BvTimezoneShift.TimezoneID,
            BvShift.ShiftTypeID
            FROM    BvTimezoneShift
            JOIN    BvShift ON BvTimezoneShift.ShiftID = BvShift.ID
                    AND BvTimezoneShift.OwnerSID = BvShift.OwnerSID
            WHERE   BvTimezoneShift.OwnerSID = @OwnerSID
            AND     BvTimezoneShift.ShiftID = @ID
            AND     BvTimezoneShift.TimezoneID = @TimezoneID    
            ORDER BY TimezoneID 
    END
END ELSE BEGIN
    SELECT ID,
        CycleType,
        StartDayOfWeek,
        StartTime,
        FinishDayOfWeek,
        FinishTime,
        0 TimezoneID,
        ShiftTypeID
        FROM BvShift
        WHERE OwnerSID = @OwnerSID
    UNION
    SELECT BvTimezoneShift.ShiftID ID,
        BvShift.CycleType,
        BvTimezoneShift.StartDayOfWeek,
        BvTimezoneShift.StartTime,
        BvTimezoneShift.FinishDayOfWeek,
        BvTimezoneShift.FinishTime,
        BvTimezoneShift.TimezoneID,
        BvShift.ShiftTypeID
        FROM BvTimezoneShift
        JOIN BvShift ON
                BvTimezoneShift.ShiftID = BvShift.ID AND
                BvTimezoneShift.OwnerSID = BvShift.OwnerSID
        WHERE BvShift.OwnerSID = @OwnerSID
        ORDER   BY  TimezoneID
END
GO
PRINT N'Creating [dbo].[BvSpShift_Insert]...';


GO
CREATE PROCEDURE [dbo].[BvSpShift_Insert]
        @OwnerSID int,
        @ID int,
        @CycleType int,
        @StartDayOfWeek int,
        @StartTime datetime,
        @FinishDayOfWeek int,
        @FinishTime datetime,
        @ShiftTypeID int

AS
DECLARE @Rows int

SELECT  @Rows = COUNT(*)
    FROM    BvShift
    WHERE   ID = @ID
    AND OwnerSID = @OwnerSID
IF @Rows <> 0
--  return 50001    /* BVDBS_STORED_PROCEDURE_DUPLICATED_OBJECT */
    return 0

SELECT  @Rows = COUNT(*)
    FROM    BvShiftType
    WHERE   ObjectID = @ShiftTypeID
    AND OwnerSID = @OwnerSID
IF @Rows <> 1
--  return 50002    /* BVDBS_STORED_PROCEDURE_OBJECT_NOT_EXIST */
    return 0

INSERT  BvShift( 
        OwnerSID, 
        ID, 
        CycleType,
        StartDayOfWeek,
        StartTime,
        FinishDayOfWeek,
        FinishTime,
        ShiftTypeID )
    VALUES( @OwnerSID, 
        @ID, 
        @CycleType,
        @StartDayOfWeek,
        @StartTime,
        @FinishDayOfWeek,
        @FinishTime,
        @ShiftTypeID )
return  @ID
GO
PRINT N'Creating [dbo].[BvSpShift_Delete]...';


GO
CREATE PROCEDURE [dbo].[BvSpShift_Delete]
        @OwnerSID int,
        @ID int,
        @Mode int

AS
DECLARE @Rows int

SELECT  @Rows = COUNT( * )
    FROM    BvShift
    WHERE   OwnerSID = @OwnerSID
    AND ID = @ID
    
IF @Rows = 0
BEGIN
    RAISERROR( 'Shift with ID = %i and OwnerSID = %i not found', 16, 1, @ID, @OwnerSID)
    RETURN -1
END
IF @Rows <> 1
BEGIN
    RAISERROR( 'Multiple shifts with ID = %i and OwnerSID = %i found', 16, 1, @ID,@OwnerSID )
    RETURN -1
END
    
DELETE  BvTimezoneShift
    WHERE   OwnerSID = @OwnerSID
    AND ShiftID = @ID
DELETE  BvShift
    WHERE   OwnerSID = @OwnerSID
    AND ID = @ID
return 0
GO
PRINT N'Creating [dbo].[BvSpShiftType_Update]...';


GO
CREATE PROCEDURE [dbo].[BvSpShiftType_Update]
        @OwnerSID int,
        @OldID int,
        @NewID int,
        @Name nvarchar(255),
        @Color int,
        @Mode int,
        @ObjectID int
AS
DECLARE @Rows int

SELECT  @Rows = COUNT(*), @ObjectID = MIN( ObjectID )
    FROM    BvShiftType
    WHERE   ID = @OldID
    AND OwnerSID = @OwnerSID
    
IF @Rows = 0
BEGIN
	RAISERROR('Shift type with ID = %i and OwnerSID = %i not found', 16, 1, @OldID, @OwnerSID)
	RETURN -1
END
IF @Rows <> 1
BEGIN
    RAISERROR('Multiple shift types with ID = %i and OwnerSID = %i found', 16, 1, @OldID, @OwnerSID)
    RETURN -1
END
    
IF ( @OldID <> @NewID ) 
BEGIN

    SELECT  @Rows = COUNT(*)
        FROM    BvShiftType
        WHERE   ID = @NewID
        AND OwnerSID = @OwnerSID

    IF @Rows <> 0
	BEGIN
		RAISERROR('Shift type with ID = %i and OwnerSID = %i  already exists', 16, 1, @NewID, @OwnerSID)
		RETURN -1
	END

    UPDATE  BvShiftType
        SET ID = @NewID,
            Name = @Name,
            Color = @Color
        WHERE   OwnerSID = @OwnerSID
        AND ID = @OldID
END
ELSE BEGIN
    UPDATE  BvShiftType
        SET Name = @Name,
            Color = @Color
        WHERE   OwnerSID = @OwnerSID
        AND ID = @OldID
END
RETURN ( 0 )
GO
PRINT N'Creating [dbo].[BvSpShiftType_List]...';


GO
CREATE PROCEDURE [dbo].[BvSpShiftType_List]
        @OwnerSID int 

AS

IF @OwnerSID = 0
    SELECT  ID,  [Name],  [Color], ObjectID
        FROM    BvShiftType
ELSE
    SELECT  ID,  [Name],  [Color], ObjectID
        FROM    BvShiftType
        WHERE   OwnerSID = @OwnerSID

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpShiftType_Insert]...';


GO
CREATE PROCEDURE [dbo].[BvSpShiftType_Insert]
        @OwnerSID int,
        @ID int,
        @Name nvarchar(255),
        @Color int,
        @ObjectID int
AS
SET NOCOUNT ON

DECLARE @Rows int

SELECT  @Rows = COUNT(*)
    FROM    BvShiftType
    WHERE   ID = @ID
    AND OwnerSID = @OwnerSID

IF @Rows <> 0
--  return 50001    /* BVDBS_STORED_PROCEDURE_DUPLICATED_OBJECT */
    return 0

    INSERT BvShiftType( OwnerSID, ID, Name, Color )
      VALUES( @OwnerSID, @ID, @Name, @Color )

    SET @Rows = @@IDENTITY

    -- Insert shift type time zones
    INSERT INTO BvShiftZones VALUES( 0, @Rows )
    INSERT INTO BvShiftZones 
      SELECT BvTimeZone.[ID], @Rows
      FROM BvTimeZone

RETURN @Rows
GO
PRINT N'Creating [dbo].[BvSpShiftType_GetID]...';


GO
CREATE PROCEDURE [dbo].[BvSpShiftType_GetID]
@OwnerID INT,
@ID      INT
AS
DECLARE @SID INT

    SELECT @SID = ObjectID 
      FROM BvShiftType
      WHERE OwnerSID = @OwnerID
           AND [ID] = @ID

    IF @SID IS NULL
        RAISERROR( 'Could not find shift type with owner id %i and id %i', 16, 1, @OwnerID, @ID )

RETURN ISNULL(@SID, -1)
GO
PRINT N'Creating [dbo].[BvSpSetObjectNumber]...';


GO
CREATE  PROCEDURE BvSpSetObjectNumber
 @ObjectSID INT,
 @ClassID INT,
 @BvID   BIGINT
AS
IF @BvID <> 0
BEGIN
   IF EXISTS( SELECT * FROM BvNumber WHERE ClassID = @ClassID AND BvID = @BvID )
   BEGIN
       RAISERROR( 'The custom number specified has been used previously, please specify another number.', 16, 1 )
       RETURN( -1 )
   END

 INSERT INTO BvNumber ( 
  ObjectSID, 
  ClassID, 
  BvID ) 
 VALUES ( 
  @ObjectSID,
  @ClassID, 
  @BvID )
 END

RETURN( 1 )
GO
PRINT N'Creating [dbo].[BvSpSetCallState]...';


GO
CREATE PROCEDURE [dbo].[BvSpSetCallState]
	@SurveySID		INT,
	@InterviewID	INT,
	@state			INT
AS

UPDATE BvCachedCalls
SET CallState = @state
WHERE InterviewID = @InterviewID AND 
	  SurveySID = @SurveySID

UPDATE BvSvySchedule
SET CallState = @state
WHERE InterviewID = @InterviewID AND 
	  SurveySID = @SurveySID
GO
PRINT N'Creating [dbo].[BvSpSendMessageToSurveys]...';


GO
CREATE PROCEDURE  [dbo].[BvSpSendMessageToSurveys]
	@BatchId int,	
    @MessageBody nvarchar(1024),
	@MessageSupervisorName nvarchar(50)    
AS

BEGIN

	DECLARE @MessageId int
	INSERT INTO BvMessages (Body, CreateTime, SupervisorName) VALUES(@MessageBody, GETUTCDATE(), @MessageSupervisorName);
	SET @MessageId = SCOPE_IDENTITY();

	/* Survey group contains all interviewer working on survey*/
	BEGIN TRANSACTION

			INSERT INTO BvMessageToPerson (MessageId, InterviewerId) 
					SELECT @MessageId, t.PersonSID 
						FROM bvTasks as t
						WHERE t.SurveySID IN (SELECT ItemId FROM bvTransferArrays WHERE BatchId = @BatchId)

			UPDATE BvPerson SET HasNewMessage = 1
					WHERE SID IN (SELECT SID FROM											
								BvPerson as p
								INNER JOIN  bvTasks as t ON p.SID = t.PersonSID
								INNER JOIN 	bvTransferArrays ON (t.SurveySID = ItemId AND BatchId = @BatchId)
					)			
					
	COMMIT TRANSACTION
	
END
GO
PRINT N'Creating [dbo].[BvSpSendMessageToInterviewers]...';


GO
CREATE PROCEDURE [dbo].[BvSpSendMessageToInterviewers]
	
	@BatchId int,	
	@OnlineOnly bit,
    @MessageBody nvarchar(1024),	
	@MessageSupervisorName nvarchar(50)    
AS

BEGIN

	DECLARE @MessageId int
	INSERT INTO BvMessages (Body, CreateTime, SupervisorName) VALUES(@MessageBody, GETUTCDATE(), @MessageSupervisorName);
	SET @MessageId = SCOPE_IDENTITY();

IF(@OnlineOnly = 1)/* for online only persons*/
	
	BEGIN
		INSERT INTO BvMessageToPerson (MessageId, InterviewerId) 
			SELECT @MessageId, PersonSID 
			FROM BvTasks 
			INNER JOIN bvTransferArrays ON (PersonSID = ItemId AND BatchId = @BatchId)

		UPDATE BvPerson SET HasNewMessage = 1 
			WHERE SID IN (SELECT PersonSID 
			              FROM BvTasks 
			              INNER JOIN bvTransferArrays ON (PersonSID = ItemId AND BatchId = @BatchId) )
	END

ELSE /* for all persons*/
	BEGIN
		INSERT INTO BvMessageToPerson (MessageId, InterviewerId) 
			SELECT @MessageId, SID FROM 
				bvPerson INNER JOIN bvTransferArrays ON (SID = ItemId AND BatchId = @BatchId)

		UPDATE BvPerson SET HasNewMessage = 1  
				WHERE SID IN (SELECT ItemId FROM bvTransferArrays WHERE BatchId = @BatchId)
	END
END
GO
PRINT N'Creating [dbo].[BvSpSendMessageToGroups]...';


GO
CREATE PROCEDURE [dbo].[BvSpSendMessageToGroups]
	@BatchId int,	
	@OnlineOnly bit,
    @MessageBody nvarchar(1024),	
	@MessageSupervisorName nvarchar(50)    
AS

BEGIN

	DECLARE @MessageId int
	INSERT INTO BvMessages (Body, CreateTime, SupervisorName) VALUES(@MessageBody, GETUTCDATE(), @MessageSupervisorName);
	SET @MessageId = SCOPE_IDENTITY();

	/* Interviewer group contains all interviewers including ones in nested groups */	
	WITH CTE (ObjectSID) 
	AS
	(
		SELECT m.ObjectSID
		FROM bvMembership as m
		Inner join bvTransferArrays ON BatchId = @BatchId 
		WHERE
		 [m].[ContainerSID] = ItemId 

	UNION ALL
		
		SELECT m.ObjectSID
		FROM bvMembership as m
		INNER JOIN CTE as c
		ON m.ContainerSID = c.ObjectSID
	),
	CTE_ALL_INTERVIEWERS AS
	(
		SELECT DISTINCT p.[SID] FROM
			bvPerson AS p
			INNER Join CTE AS c
				ON c.ObjectSID = p.SID		
	)

	/* Save into temporary table all interviewers for whom we should send message.
	If flag @OnlineOnly is true save only online interviewers otherwise all interviewers */
	SELECT SID INTO #INTERVIEWERS 
	FROM CTE_ALL_INTERVIEWERS as C
	LEFT JOIN BvTasks as L ON C.SID = L.PersonSID
	WHERE (@OnlineOnly = 0 OR (@OnlineOnly=1 AND L.PersonSID IS NOT NULL))

	BEGIN TRANSACTION

		INSERT INTO BvMessageToPerson (MessageId, InterviewerId) 
					SELECT @MessageId, I.SID FROM #INTERVIEWERS as I

			UPDATE BvPerson SET HasNewMessage = 1 
				WHERE SID IN (SELECT #INTERVIEWERS.SID FROM #INTERVIEWERS )					
					
	COMMIT TRANSACTION

	DROP TABLE #INTERVIEWERS

END
GO
PRINT N'Creating [dbo].[BvSpSchedule_Insert]...';


GO
CREATE PROCEDURE [dbo].[BvSpSchedule_Insert]
       @ScheduleID INT,
       @Name NVARCHAR(255),
       @XmlUnderDev NVARCHAR(MAX),
       @ScriptSource NVARCHAR(MAX),
       @DesignStateGroupID INT
AS

INSERT INTO BvSchedule (
       [ScheduleID],
       [Name],
       [CreateDate],
       [ModifyDate],
       [XmlUnderDev],
       [ScriptSource],
       [DesignStateGroupID] )
    VALUES (
       @ScheduleID,
       @Name,
       GETUTCDATE(),
       GETUTCDATE(),
       @XmlUnderDev,
       @ScriptSource,
       @DesignStateGroupID )
GO
PRINT N'Creating [dbo].[BvSpSample_Finalize]...';


GO
CREATE  PROCEDURE BvSpSample_Finalize
    @BatchID INT,
    @BatchSize INT,
    @SurveySID INT
AS

DECLARE @IsRandomCallDeliveryEnabled BIT

SELECT @IsRandomCallDeliveryEnabled = IsRandomCallDeliveryEnabled
FROM BvSurvey
WHERE SID = @SurveySID

UPDATE BvSvySchedule
SET CallState = 2,
    CallOrder = CASE WHEN @IsRandomCallDeliveryEnabled = 0 THEN InterviewId
                     ELSE dbo.GetRandomValue(InterviewID)
                END
FROM BvInterview
WHERE BvInterview.SurveySid = BvSvySchedule.SurveySid AND
      BvInterview.ID = BvSvySchedule.InterviewID AND
      BvInterview.BatchID = @BatchID AND
      CallState = -3

insert into BvCachedCallsInsert
select c.InterviewID, @SurveySID
from BvSvySchedule c
inner join BvInterview i on i.SurveySID = @SurveySID
    and i.[ID] = c.InterviewID
    and i.BatchID = @BatchID
where c.SurveySID = @SurveySID

TRUNCATE TABLE BvUniqueAssignments
INSERT INTO BvUniqueAssignments
SELECT DISTINCT ExplicitSID FROM BvSvySchedule
   
RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpSample_Abandon]...';


GO
CREATE PROCEDURE [dbo].[BvSpSample_Abandon]
    @BatchID INT,
    @SurveySID INT
AS

DELETE BvHistory 
WHERE BatchId = @BatchID AND 
      SurveyId = @SurveySID

DELETE BvSvySchedule 
FROM BvInterview
WHERE BvInterview.BatchID = @BatchID AND
      BvSvySchedule.InterviewID = BvInterview.ID AND 
      BvSvySchedule.SurveySID = BvInterview.SurveySID

DELETE FROM BvInterview
WHERE BvInterview.BatchID = @BatchID

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpSampleStatusSummary_Get]...';


GO
CREATE PROCEDURE [dbo].[BvSpSampleStatusSummary_Get]
@SurveySID INT
AS
    DECLARE @StateGroupID INT
    SELECT @StateGroupID = StateGroupID FROM BvSurvey WHERE SID = @SurveySID

    SELECT  BvSampleStatusSummary.SurveySID as SurveySID,
            BvSampleStatusSummary.ITS  as StateID,
            BvState.Name as StateName,
            BvSampleStatusSummary.Cnt as Cnt,
            BvSampleStatusSummary.AlertStatus as AlertStatus
        FROM BvSampleStatusSummary 
        INNER JOIN BvState
            ON  BvState.StateID = BvSampleStatusSummary.ITS AND 
                BvSampleStatusSummary.SurveySID = @SurveySID AND 
                BvState.StateGroupID = @StateGroupID

    RETURN(0)
GO
PRINT N'Creating [dbo].[BvSpReportSSS]...';


GO
CREATE  PROCEDURE BvSpReportSSS
@SurveySID INT, @SelectInterviewsQuery NVARCHAR (MAX)
AS
 IF @SurveySID IS NULL AND @SelectInterviewsQuery IS NULL
 BEGIN
 /* Looks like we're generating code using FMTONLY. So lets return metadata*/
 SELECT
     0  AS id,
     '' AS name,
     0  AS count,
     0  AS sample_size
     RETURN 0;
 END
 
DECLARE @Query NVARCHAR(MAX) ='
         SELECT
             allInterviews.TransientState            ''id'',
             allInterviews.StateName                 ''name'',
             count( allInterviews.TransientState )   ''count'',
             (SELECT count( * ) FROM BvInterview
              WHERE SurveySID = @SurveySID)          ''sample_size''
         FROM ( '+ @SelectInterviewsQuery + ' ) as allInterviews
         GROUP BY allInterviews.TransientState, allInterviews.StateName
         ORDER BY allInterviews.TransientState'
     
     EXEC sp_executesql  @Query, N'@SurveySID INT',
     @SurveySID = @SurveySID
GO
PRINT N'Creating [dbo].[BvSpReportSampleStatusSummary]...';


GO
CREATE PROCEDURE [dbo].[BvSpReportSampleStatusSummary]
@SurveySID INT, 
@PersonsSIDs NVARCHAR (2000), 
@ITSIDs NVARCHAR (1000)
AS
IF @SurveySID IS NULL AND @PersonsSIDs IS NULL AND @ITSIDs IS NULL
BEGIN
    SELECT 
    0 as [StateID],
    '' as [StateName],
    0 as [Count],
    '' as [SurveyName],
    0 as [SampleSize],
    0 as [Calls],
    '' as [Person]
    
    RETURN 0
END

DECLARE @StrSurveySID NVARCHAR (16)
SET @StrSurveySID = CAST(@SurveySID AS NVARCHAR(16))

DECLARE @SurveyQreName NVARCHAR (255), @SurveyDescription NVARCHAR (255)
SELECT @SurveyQreName = ISNULL(Name, '''') FROM BvSurvey WHERE SID = @SurveySID AND State <> 2
SELECT @SurveyDescription = ISNULL(Description, '''') FROM BvSurvey WHERE SID = @SurveySID AND State <> 2
Set @SurveyDescription = REPLACE(@SurveyDescription,'''','''''') --escape single apostrophe

SET @SurveyQreName = @SurveyDescription + ' (' + @SurveyQreName + ')'

DECLARE @PersonsStatement NVARCHAR (1000)
DECLARE @PersonsFilter NVARCHAR (4000)
DECLARE @PersonsGroup NVARCHAR (255)
IF @PersonsSIDs = '' BEGIN
 SET @PersonsStatement = ' ''ALL_PERSONS'' '
 SET @PersonsFilter = ''
 SET @PersonsGroup = ''
END
ELSE BEGIN
 SET @PersonsStatement = 
  ' IsNull((SELECT Name FROM BvPerson WHERE SID = 
  BvInterview.LastCallPersonSID), ''NO_CALLS'') '
 SET @PersonsFilter = 
  ' AND BvInterview.LastCallPersonSID in (' +
  @PersonsSIDs + ') '
 SET @PersonsGroup = ', BvInterview.LastCallPersonSID '
END

DECLARE @ITSFilter NVARCHAR (2000)
IF @ITSIDs = ''
 SET @ITSFilter = ''
ELSE
 SET @ITSFilter = ' AND bvstate.stateid IN (' + @ITSIDs + ') '

DECLARE @Query NVARCHAR (4000)
SET @Query=
 'SELECT
  bvstate.stateid ''StateID'',
  bvstate.name ''StateName'',
  count( BvInterview.transientstate ) ''Count'',
  ''' + @SurveyQreName + ''' ''SurveyName'',
  (SELECT count(*) 
   FROM BvInterview 
   WHERE (SurveySID = ' + @StrSurveySID + ') ' +
   ') ''SampleSize'',
  0 ''Calls'',
   ' + @PersonsStatement + ' ''Person''
 FROM bvstate LEFT JOIN BvInterview 
 ON (bvstate.stateid = BvInterview.transientstate) 
 AND (SurveySID = ' + @StrSurveySID + ') ' +
 'LEFT JOIN BvSurvey ON
 bvsurvey.SID = ' + @StrSurveySID + '
 WHERE bvstate.StateGroupID = bvsurvey.StateGroupID 
  ' + @PersonsFilter + ' 
  ' + @ITSFilter + ' 
 GROUP BY bvstate.stateid, bvstate.name ' + @PersonsGroup + ' 
 ORDER BY BvState.StateID'
/*print @Query*/
exec sp_executesql @Query
GO
PRINT N'Creating [dbo].[BvSpReleaseCall]...';


GO
CREATE PROCEDURE [dbo].[BvSpReleaseCall]
	@SurveySID		INT,
	@InterviewID	INT
AS

UPDATE BvCachedCalls
SET CallState = 2
WHERE InterviewID = @InterviewID AND 
	  SurveySID = @SurveySID AND
	  CallState <> 0

UPDATE BvSvySchedule
SET CallState = 2
WHERE InterviewID = @InterviewID AND 
	  SurveySID = @SurveySID AND
	  CallState <> 0
GO
PRINT N'Creating [dbo].[BvSpPerson_updateMNDiallerUserId]...';


GO
CREATE PROCEDURE [dbo].[BvSpPerson_updateMNDiallerUserId]
 @SID int, 
 @MNDiallerUserId int
AS

UPDATE BvPerson 
 SET MNDiallerUserId = @MNDiallerUserId
  WHERE SID = @SID

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpPerson_Update]...';


GO
-- TODO: remove this procedure at all 
CREATE PROCEDURE [dbo].[BvSpPerson_Update]
 @SID int, 
 @Name nvarchar( 255 ),  
 @FullName nvarchar( 255 ),
 @Description nvarchar( 255 ),
 @ManualSelection int,
 @BvID int,
 @AutoSurveyId int,
 @AllowedChoices INT = NULL,
 @StationExtensionNumber NVARCHAR (255),
 @IsDialerAgentLocal BIT
AS
DECLARE @Rows int

SELECT  @Rows = COUNT(*)
    FROM    BvPerson

    WHERE   SID = @SID
IF @Rows = 0
BEGIN
    RAISERROR( 'Person with SID %i not found', 16, 1, @SID)
    RETURN -1
END
IF @Rows <> 1
BEGIN
    RAISERROR( 'Multiple persons with SID %i found ', 16, 1, @SID)
    RETURN -1
END

IF ISNULL( @BvID, 0 ) > 0
BEGIN
    IF EXISTS( 
     SELECT 1 FROM BvNumber 
     WHERE BvID = @BvID AND ClassID = 10 AND ObjectSID != @SID
    )
    BEGIN
     RAISERROR( 'BvID = %u already exists', 16, 1, @BvID )
     RETURN -1
    END
END
    
    UPDATE  BvPerson
    SET [Name] = @Name, 
        FullName = @FullName,
        [Description] = @Description,
        ManualSelection = @ManualSelection,
        AutomaticSurveyID = @AutoSurveyId,
        AllowedChoices = @AllowedChoices,
        StationExtensionNumber = @StationExtensionNumber,
        IsDialerAgentLocal = @IsDialerAgentLocal
        WHERE   SID = @SID

IF ISNULL( @BvID, 0 ) > 0
 UPDATE BvNumber SET BvID = @BvID 
 WHERE ObjectSID = @SID AND ClassID = 10

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpPerson_ListByParent]...';


GO
CREATE PROCEDURE [dbo].[BvSpPerson_ListByParent]
	@ParentSID int
AS
	SELECT  
        BvPerson.SID AS [SID],
        10 AS [ClassID], /* BVDBS_PERSON */
        BvPerson.[Name] AS [Name],
  ISNULL(BvTasks.[SurveySID], 0) AS [SurveySID],
  ISNULL(BvTasks.[InterviewID], 0) AS [InterviewID],
  2 AS [RoleID] /* always CATI */  
        FROM    BvPerson
  LEFT JOIN BvTasks
  ON BvTasks.PersonSID = BvPerson.SID
        WHERE   BvPerson.SID IN (   SELECT  ObjectSID
                        FROM    BvMembership
                        WHERE   ContainerSID = @ParentSID )
    ORDER BY ClassID DESC
GO
PRINT N'Creating [dbo].[BvSpPerson_Insert]...';


GO
CREATE PROCEDURE [dbo].[BvSpPerson_Insert]
        @SID INT, 
        @Name NVARCHAR( 255 ),  
        @FullName NVARCHAR( 255 ),
        @Description NVARCHAR( 255 ),
        @ManualSelection INT,
        @AssignmentsListMode INT,
        @BvID INT,
        @PwdSaltTxt NVARCHAR(256),
		@CallGroupId INT,
		@Location NVARCHAR(256)
AS

DECLARE @Rows int

IF ( @BvID > 0 )
BEGIN
 EXEC @BvID = BvSpSetObjectNumber @SID, 10, @BvID
 IF @BvID = -1
     RETURN ( 50006 )
END

IF (EXISTS(SELECT 1 FROM BvPerson WHERE [Name]=@Name))
BEGIN
    RAISERROR( 'Person with name %s already exists', 12, 1, @Name )
    RETURN -1
END

INSERT  BvPerson( 
        SID,
        [Name], 
        FullName,
        [Description],
        ManualSelection, 
        AssignmentsListMode,
        PwdSaltTxt,
		CallGroupID,
		Location)
    VALUES ( 
        @SID,
        @Name, 
        @FullName,
        @Description,
        @ManualSelection,
        @AssignmentsListMode, 
        @PwdSaltTxt,
		@CallGroupId,
		@Location)

RETURN 0
GO
PRINT N'Creating [dbo].[BvSpPerson_GetAssignedSurveyList]...';


GO
CREATE PROCEDURE [dbo].[BvSpPerson_GetAssignedSurveyList]
@PersonSID INT, @UserName NVARCHAR (MAX)=NULL
AS
IF @PersonSID IS NULL AND @UserName IS NULL
BEGIN
  SELECT 
    0 as [SID],
    '' as [Name],
    '' as [Description],
    0 as [AssignedCallsCount],
    0 as [AssignmentType]
  RETURN(0)
END

SELECT DISTINCT
  [s].[SID],
  [s].[Name],
  [s].[Description],
  0 AS [AssignedCallsCount],
  [AssignmentType] =
    CASE
      WHEN a.[Id] IS NULL THEN 0 -- 0 for implicit assignment by group
      ELSE 1 -- 1 for explicit assignment
    END
 FROM BvSurvey s 
  inner join BvUserSurveyPermission p on p.UserName = @UserName and s.SID = p.SurveySID
  left join BvPersonRel r on r.ObjectSID = s.SID and r.Type = 2 and r.RoleID = 2 and r.PersonSID = @PersonSID
  left join BvPersonOrGroupAssignmentOnSurvey a on a.SurveyId = s.SID and a.PersonOrGroupId = @PersonSID
 WHERE s.State <> 2 AND ( a.Id is not null or r.ObjectSID is not null )
  

UNION

 SELECT 
  BvSurvey.[SID],
  BvSurvey.[Name],
  BvSurvey.[Description],
  COUNT(*) AS [AssignedCallsCount],
  2 AS [AssignmentType] -- implicit assignment by call
    FROM BvSvySchedule WITH(NOLOCK), 
  BvSurvey, 
  BvViewPersonAndGroup, 
  BvUserSurveyPermission
 WHERE
        BvSvySchedule.ExplicitSID = BvViewPersonAndGroup.SID AND
		BvSvySchedule.ExplicitSID = @PersonSID AND
        BvSurvey.SID = BvSvySchedule.SurveySID AND
        BvSurvey.SID = BvUserSurveyPermission.SurveySID AND
        BvUserSurveyPermission.UserName = @UserName AND
        BvSurvey.State <> 2
    GROUP BY 
  BvSurvey.SID, 
  BvSurvey.[Name],
  BvSurvey.[Description]

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpPersonMonitoring_Stop]...';


GO
CREATE PROCEDURE [dbo].[BvSpPersonMonitoring_Stop]
        @PersonSID INT,
  @MonitoringSessionID BIGINT
AS
DECLARE @Count INT

SELECT @Count = COUNT(*) FROM BvPersonMonitoring WHERE (PersonSID = @PersonSID) AND (MonitoringSessionID = @MonitoringSessionID)

IF @Count <> 0
BEGIN
 DELETE FROM BvPersonMonitoring WHERE (PersonSID = @PersonSID) AND (MonitoringSessionID = @MonitoringSessionID)

 DELETE FROM BvPersonMonitoringLastID WHERE (PersonSID = @PersonSID) AND (MonitoringSessionID = @MonitoringSessionID)

 DELETE FROM BvPersonMonitoringEvents WHERE (PersonSID = @PersonSID) AND (MonitoringSessionID = @MonitoringSessionID)

    RETURN (1)
END

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpPersonMonitoring_Start]...';


GO
CREATE PROCEDURE [dbo].[BvSpPersonMonitoring_Start]
        @PersonSID INT,
        @SupervisorName NVARCHAR(256),
  @MonitoringSessionID BIGINT
AS

INSERT INTO BvPersonMonitoring 
    SELECT @PersonSID, @SupervisorName, @MonitoringSessionID
        WHERE NOT EXISTS( 
            SELECT 1 
            FROM BvPersonMonitoring 
            WHERE PersonSID = @PersonSID )

IF @@ROWCOUNT <> 0
BEGIN
 DELETE FROM BvPersonMonitoringLastID WHERE (PersonSID = @PersonSID)

 INSERT INTO BvPersonMonitoringLastID
  SELECT @PersonSID, @MonitoringSessionID, 0
   WHERE EXISTS(
    SELECT 1 FROM BvPersonMonitoring WHERE PersonSID = @PersonSID)

    SELECT 1 as result, '' as supervisorNameAlreadyMonitoring, 0 as monitoringSessionID
    RETURN (1)
END
ELSE
BEGIN
    SELECT 0 as result, supervisorName as supervisorNameAlreadyMonitoring, monitoringSessionID FROM BvPersonMonitoring WHERE PersonSID = @PersonSID
    RETURN (0)
END

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpPersonMonitoring_IsStart]...';


GO
CREATE PROCEDURE [dbo].[BvSpPersonMonitoring_IsStart]
        @PersonSID INT
AS
DECLARE @supervisorNameAlreadyMonitoring NVARCHAR( 256 )
DECLARE @monitoringSessionID BIGINT
SELECT @supervisorNameAlreadyMonitoring = supervisorName, @monitoringSessionID = MonitoringSessionID FROM BvPersonMonitoring WHERE PersonSID = @PersonSID

IF @supervisorNameAlreadyMonitoring IS NULL
BEGIN
	/*
		we need it to get correct type for DAL generated entities
	*/
	SET @monitoringSessionID = 0

    SELECT 0 as result, '' as supervisorNameAlreadyMonitoring, @monitoringSessionID as monitoringSessionID
    RETURN (0)
END
ELSE
BEGIN
    SELECT 1 as result, @supervisorNameAlreadyMonitoring as supervisorNameAlreadyMonitoring, @monitoringSessionID as monitoringSessionID
    RETURN (1)
END

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpPersonGroup_Update]...';


GO
CREATE PROCEDURE [dbo].[BvSpPersonGroup_Update]
        @SID                int,
        @Name               nvarchar( 255 ),
        @Description        nvarchar( 255 ),
        @RoleID             int,
        @ManualSelection    int,
  @BvID    int
AS

IF EXISTS ( SELECT [SID] FROM BvPersonGroup WHERE [Name] = @Name AND [SID] != @SID )
BEGIN
 RAISERROR('Person group %s already exists', 12, 2, @Name)
 RETURN -1
END

DECLARE @Rows int
SELECT  @Rows = COUNT(*)
    FROM    BvPersonGroup
    WHERE   SID = @SID

IF @Rows = 0
  BEGIN
    RAISERROR('Person group with SID %i not exists', 16, 2, @SID)
    RETURN -1
  END
IF @Rows <> 1
  BEGIN
    RAISERROR('Multiple person groups with SID %i found', 16, 2, @SID)
    RETURN -1
  END

IF ISNULL( @BvID, 0 ) > 0
BEGIN
    IF EXISTS( 
     SELECT 1 FROM BvNumber 
     WHERE BvID = @BvID AND ClassID = 65546 AND ObjectSID != @SID
    )
    BEGIN
     RAISERROR( 'BvID = %u already exists', 16, 1, @BvID )
     RETURN -1
    END
END

UPDATE  BvPersonGroup
    SET [Name] = @Name,
    [Description] = @Description,
    RoleID = @RoleID,
    ManualSelection = @ManualSelection
    WHERE SID = @SID

IF ISNULL( @BvID, 0 ) > 0
 UPDATE BvNumber SET BvID = @BvID 
 WHERE ObjectSID = @SID AND ClassID = 65546
ELSE
    DELETE FROM BvNumber 
    WHERE ObjectSID = @SID AND ClassID = 65546

RETURN 0
GO
PRINT N'Creating [dbo].[BvSpPersonGroup_List]...';


GO
CREATE PROCEDURE [dbo].[BvSpPersonGroup_List]
        @ParentGroupId int 

AS

IF @ParentGroupId = 0 --only root groups
	SELECT DISTINCT
	   BvPersonGroup.SID,
	   BvPersonGroup.Name,
	   BvPersonGroup.Description,
	   BvPersonGroup.RoleID,
	   BvPersonGroup.ManualSelection
	FROM BvPersonGroup
	LEFT JOIN BvMembership ON BvPersonGroup.SID = BvMembership.ObjectSID AND
							  BvMembership.ContainerSID = @ParentGroupId
	WHERE BvMembership.ObjectSID IS NULL
ELSE --child groups for passed parent group
	SELECT DISTINCT
	   BvPersonGroup.SID,
	   BvPersonGroup.Name,
	   BvPersonGroup.Description,
	   BvPersonGroup.RoleID,
	   BvPersonGroup.ManualSelection
	FROM BvPersonGroup
	INNER JOIN BvMembership ON BvPersonGroup.SID = BvMembership.ObjectSID AND
							  BvMembership.ContainerSID = @ParentGroupId
GO
PRINT N'Creating [dbo].[BvSpPersonCheckForNewMessage]...';


GO

CREATE PROCEDURE [dbo].[BvSpPersonCheckForNewMessage]
@PersonSID INT
AS

BEGIN
 
	SELECT ISNULL(HasNewMessage, 0) AS HasNewMessage
	FROM bvPerson 
	WHERE SID = @PersonSID

END
GO
PRINT N'Creating [dbo].[BvSpPersonAndGroups_List]...';


GO
CREATE PROCEDURE [dbo].[BvSpPersonAndGroups_List]
        @ParentSID int,
        @SurveySid int,
        @Filter nvarchar(max) = NULL -- Part of person's or group's name to filter by.
AS

    SELECT  p.SID  as SID,
            p.Name as UserName,
            0      as isGroup,    
   0      as MembersCount,
   (SELECT COUNT(*) FROM BvPersonRel r with ( nolock )
     WHERE r.PersonSID = p.SID AND r.ObjectSID = @SurveySid )
    as IsAssignedOnCurrentSurvey,
            (SELECT COUNT(*) FROM BvSvySchedule sv where p.Sid = sv.ExplicitSid
                  and sv.SurveySid = @SurveySid)
               as CurSurvAssign,
            (SELECT COUNT(*) FROM BvSvySchedule sv where p.Sid = sv.ExplicitSid) 
               as AllSurvAssign,          
   (select count( distinct s.SID) from  BvSurvey s, BvPersonOrGroupAssignmentOnSurvey a with( nolock )
    where  s.SID = a.SurveyId and a.PersonOrGroupId = p.SID  and s.State <> 2)
    as TotalAssignedSurveys 

            FROM   BvPerson p
            WHERE  p.SID IN (   SELECT  ObjectSID
                        FROM    BvMembership
                        WHERE   ContainerSID = @ParentSID )
                   AND (@Filter is NULL OR p.Name LIKE (@Filter) )

      UNION 

      select pg.sid     as SID,
             pg.name    as UserName,
             1          as isGroup,
       (SELECT COUNT(*) FROM BvMembership
              LEFT JOIN BvPerson p1 ON p1.SID = BvMembership.ObjectSID
     WHERE ContainerSID = pg.sid
           AND (@Filter is NULL OR p1.Name LIKE (@Filter) ) ) as MembersCount,
    1 as IsAssignedOnCurrentSurvey,
             0          as CurSurvAssign,
             0          as AllSurvAssign,
   (select count( distinct s.SID) from  BvSurvey s, BvPersonOrGroupAssignmentOnSurvey a with( nolock )
    where  s.SID = a.SurveyId and a.PersonOrGroupId = pg.SID and s.State <> 2)
                       as TotalAssignedSurveys

      from BvPersonGroup pg
      where pg.Sid in ( SELECT  ObjectSID
                        FROM    BvMembership
                        WHERE   ContainerSID = @ParentSID ) 
							AND pg.SID <> 4 /* Exclude '[All]' group. */
							AND (@Filter is NULL OR pg.Name LIKE (@Filter) )
GO
PRINT N'Creating [dbo].[BvSpMembership_Insert]...';


GO
CREATE PROCEDURE [dbo].[BvSpMembership_Insert]
        @ContainerSID int,
        @ObjectSID int
AS

DECLARE @Rows int

SELECT  @Rows = COUNT(*)
    FROM    BvMembership
    WHERE   ContainerSID = @ContainerSID
    AND ObjectSID = @ObjectSID

IF @Rows <> 0
BEGIN
	RAISERROR( 'Duplicated object', 16, 1)
    RETURN -1
END

INSERT  BvMembership( 
        ContainerSID, 
        ObjectSID ) 
    VALUES( 
        @ContainerSID, 
        @ObjectSID )
GO
PRINT N'Creating [dbo].[BvSpMembership_Delete]...';


GO
CREATE PROCEDURE [dbo].[BvSpMembership_Delete]
        @ContainerSID int,
        @ObjectSID int

AS

IF  @ContainerSID = 0
    DELETE  BvMembership WITH(ROWLOCK)
        WHERE   ObjectSID = @ObjectSID
ELSE
    IF  @ObjectSID = 0
        DELETE  BvMembership WITH(ROWLOCK)
            WHERE   ContainerSID = @ContainerSID
    ELSE
        DELETE  BvMembership WITH(ROWLOCK)
            WHERE   ContainerSID = @ContainerSID
            AND ObjectSID = @ObjectSID
GO
PRINT N'Creating [dbo].[BvSpLogin_SpinUp]...';


GO
CREATE  PROCEDURE [dbo].[BvSpLogin_SpinUp]
@PersonSID INTEGER
AS
declare @SurveySID int
declare @PersonMode int

    select @PersonMode = ManualSelection from BvPerson where sid = @PersonSID
    
	select @SurveySID = SurveySID
	from BvTasks where PersonSID = @PersonSID
    
    if @SurveySID is not null 
    begin
        if(@PersonMode != 2) --is not survey selection
           SET @SurveySID = 0
    
        delete from BvLoginGroup with (tablockx) where PersonSID = @PersonSID
        insert into BvLoginGroup select PersonSID, ObjectSID, @SurveySID
            from BvPersonRel where PersonSID = @PersonSID
    end
 
return (0)
GO
PRINT N'Creating [dbo].[BvSpInterview_Update]...';


GO
CREATE PROCEDURE [dbo].[BvSpInterview_Update]
        @ID                         int,
        @SurveySID                  int,        
        @TimeZoneID                 int,
        @TransientState             int, 
        @LastCallPersonSID          int,
        @Duration                   int,
        @TelephoneNumber            varchar( 255 ),
        @RespondentName             nvarchar( 255 ),
        @LastCallTime               datetime,
        @ExtensionNumber            varchar( 255 ),
        @LastChannelID              tinyint,
        @DialingMode                tinyint,
		@DialerId					int
AS

 IF (@TimeZoneID > 0)
    IF NOT EXISTS (SELECT 1 FROM BvTimezone WHERE ID = @TimeZoneID)
       BEGIN
         RAISERROR( 'Unrecognized time zone assigned to respondent, ensure the time zone is available from the active time zone list', 16, 1 )
         RETURN (-1)  
       END 
       
UPDATE  BvInterview SET
        TimezoneID                  = CASE WHEN @TimeZoneID = 0 THEN NULL ELSE @TimeZoneID END,
        TransientState              = CASE WHEN @TransientState = 0 THEN TransientState ELSE @TransientState END,
        LastCallPersonSID           = @LastCallPersonSID,
        Duration                    = @Duration,
        TelephoneNumber             = @TelephoneNumber,
        RespondentName              = @RespondentName,
        LastCallTime                = @LastCallTime,
        ExtensionNumber             = @ExtensionNumber,
        LastChannelID               = @LastChannelID,
        DialingMode                 = @DialingMode,
		DialerId					= @DialerId
        WHERE SurveySID = @SurveySID AND ID = @ID
       
RETURN 0
GO
PRINT N'Creating [dbo].[BvSpInterview_Insert]...';


GO
CREATE PROCEDURE [dbo].[BvSpInterview_Insert]
	    @ID                         int,
        @SurveySID                  int,        
        @TimeZoneID                 int,
        @TransientState             int,
        @LastCallPersonSID          int,
        @Duration                   int,
        @TelephoneNumber            varchar( 255 ),
        @RespondentName             nvarchar( 255 ),
        @LastCallTime               datetime,
        @ExtensionNumber            varchar( 255 ),
        @LastChannelID              tinyint,
        @ConfirmitSid               varchar(64) = '',
        @DialingMode                tinyint
AS

 IF (@TimeZoneID > 0)
    IF NOT EXISTS (SELECT TOP (1) 1 FROM BvTimezone WHERE ID = @TimeZoneID)
       BEGIN
         RAISERROR( 'Unrecognized time zone assigned to respondent, ensure the time zone is available from the active time zone list', 16, 1 )
         RETURN (-1)  
       END 


IF @TimeZoneID = 0 
        SET @TimeZoneID = NULL

INSERT BvInterview( 
		ID,
        SurveySID,        
        TimezoneID,
        TransientState,
        LastCallPersonSID,
        Duration,
        TelephoneNumber,
        RespondentName,
        LastCallTime,
        ExtensionNumber,
        BatchID,
        LastChannelID,
        ConfirmitSid,
        DialingMode )
        VALUES(
			@ID,
            @SurveySID,            
            @TimeZoneID,
            @TransientState,
            @LastCallPersonSID,
            @Duration,
            @TelephoneNumber,
            @RespondentName,
            @LastCallTime,
            @ExtensionNumber,
            0,
            @LastChannelID,
            @ConfirmitSid,
            @DialingMode )
            
RETURN @ID
GO
PRINT N'Creating [dbo].[BvSpGetUserGroups]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetUserGroups]
    @PersonSID INT
AS
    IF NOT EXISTS( SELECT 1 FROM BvPerson WHERE SID = @PersonSID )
    BEGIN
        RAISERROR( 'The person with SID="%u" not found', 16, 1, @PersonSID )
        RETURN -1
    END

    SELECT ObjectSID as GroupSID FROM bvpersonrel 
        WHERE PersonSID = @PersonSID and RoleID = 2 and Type = 1
    
    RETURN @@ROWCOUNT
GO
PRINT N'Creating [dbo].[BvSpGetUserError]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetUserError]
        @ErrNo int
AS
IF  @ErrNo = 0
return  0
IF  @ErrNo = 2627
return  50001
IF  @ErrNo = 2601
return  50001
return  @ErrNo
GO
PRINT N'Creating [dbo].[BvSpGetSystemWideInfo]...';


GO
CREATE PROCEDURE BvSpGetSystemWideInfo
   @BatchID int
AS  
        --1. InterviewersLoggedCount thresholds
        DECLARE @AmberOfInterviewersLoggedCountSWI INT
        DECLARE @RedOfInterviewersLoggedCountSWI INT
        SELECT @AmberOfInterviewersLoggedCountSWI = Amber, @RedOfInterviewersLoggedCountSWI = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 12/*SystemWideInfo.LoggedInterviewersCount alert*/

        --2. OpenSurveysCount thresholds
        DECLARE @AmberOfOpenSurveysCount INT
        DECLARE @RedOfOpenSurveysCount INT
        SELECT @AmberOfOpenSurveysCount = Amber, @RedOfOpenSurveysCount = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 13/*SystemWideInfo.OpenSurveysCount alert*/

        --3. CallsCount thresholds
        DECLARE @AmberOfCallsCount INT
        DECLARE @RedOfCallsCount INT
        SELECT @AmberOfCallsCount = Amber, @RedOfCallsCount = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 14/*SystemWideInfo.CallsCount alert*/


        DECLARE @count INT;
		DECLARE @countOpenSurveys INT
        DECLARE @totalInterviewers INT
        DECLARE @loggedinterviewers INT        
		DECLARE @totalInterviewersWorkedToday INT

        SELECT @count = ISNULL(SUM(StrikeRate),0)
        FROM BvAggregateSurveyAlertStatus asas
        INNER JOIN BvSurvey s ON (s.SID = asas.SID)
        INNER JOIN BvTransferArrays ta ON (ta.BatchID = @BatchID AND
                                           ta.ItemID = s.SID)
                                                  
        SELECT @totalInterviewers = COUNT(DISTINCT BvPerson.SID) FROM BvPerson INNER JOIN 
					 BvMembership ON BvPerson.SID = ObjectSID INNER JOIN 
					 BvPersonGroup ON BvMembership.ContainerSID = BvPersonGroup.SID AND BvPersonGroup.RoleID = 2
        
		SELECT @totalInterviewersWorkedToday = COUNT(DISTINCT BvInterviewerPerformance.InterviewerId) FROM BvInterviewerPerformance

        SELECT @loggedinterviewers = COUNT(*)
        FROM BvTasks
        WHERE StatusLogout != 0 --logged out

        SELECT @countOpenSurveys = COUNT(*)
        FROM BvSurvey s
        INNER JOIN BvTransferArrays ta ON (ta.BatchID = @BatchID AND
                                           ta.ItemID = s.SID)
        WHERE s.State = 1 /*open*/
               
        SELECT         
			@totalInterviewers as TotalInterviewersCount,
			@loggedinterviewers as LoggedInterviewersCount,
            @countOpenSurveys as OpenSurveysCount,
			@totalInterviewersWorkedToday as TotalInterviewersWorkedTodayCount,
            @count as CallsCount,
            dbo.udf_AlertStatus_INT(@loggedinterviewers, @AmberOfInterviewersLoggedCountSWI, @RedOfInterviewersLoggedCountSWI) as AlertStatusOfLoggedInterviewersCount,
            dbo.udf_AlertStatus_INT(@countOpenSurveys, @AmberOfOpenSurveysCount, @RedOfOpenSurveysCount) as AlertStatusOfOpenSurveysCount,
            dbo.udf_AlertStatus_INT(@count, @AmberOfCallsCount, @RedOfCallsCount) as AlertStatusOfCallsCount
GO
PRINT N'Creating [dbo].[BvSpGetSurveys]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetSurveys]
 @Filter NVARCHAR(MAX) = NULL,
 @UserName NVARCHAR(MAX) = NULL
AS
SELECT DISTINCT
 [s].[SID] AS [SID],
 [s].[Name] AS [ConfirmitID],
 [s].[Description] AS [Name]
FROM    [BvSurvey] [s] 
left join [bvUserSurveyPermission] [p] on [s].[SID] = [p].[SurveySID]
INNER JOIN BvNumber n on n.ObjectSID = s.SID AND n.ClassID = 2
WHERE
     ( p.UserName = @UserName or @UserName is null)
 AND (@Filter IS NULL OR [s].[Description] LIKE @Filter)
 AND ( s.State <> 2)
GO
PRINT N'Creating [dbo].[BvSpGetSurveyActivityWithAlerts]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetSurveyActivityWithAlerts]
   @BatchID int, @onlyActiveSurveys bit
AS  
    SELECT asas.[SID] as SurveySID,
               asas.[Name] as ProjectID,
           asas.[Description]  as SurveyName,
           asas.[InterviewersLoggedCount],
           asas.[InterviewersLoggedCountPrev],
           asas.[NextAppointmentTime],
           asas.[TotalSampleSize], -- count of interview with 'fresh sample' its
           asas.[ActiveCallsCount],
           asas.[ActiveCallsCountPrev],
           asas.[ScheduledCallsCount],
           asas.[ScheduledCallsCountPrev],
           asas.[SuspendedCallsCount],
           asas.[SuspendedCallsCountPrev],
           asas.[MinutesSpentWorkingOnSurvey],
           asas.[AssignedInterviewersCount],
           asas.[StrikeRate],
           asas.[CountCalls],
           asas.[AvgDuration],
           asas.[AlertStatusOfInterviewersLoggedCount],
           asas.[AlertStatusOfNextAppointmentTime],
           asas.[AlertStatusOfTotalSampleSize],
           asas.[AlertStatusOfActiveCallsCount],
           asas.[AlertStatusOfScheduledCallsCount],
           asas.[AlertStatusOfSuspendedCallsCount],
           asas.[AlertStatusOfMinutesSpentWorkingOnSurvey],
           asas.[AlertStatusOfAssignedInterviewersCount],
           asas.[AlertStatusOfStrikeRate],
           asas.[AlertStatusOfCountCalls],
           asas.[MaxStatusOfITSAlerts]
    FROM BvTransferArrays ta
    INNER JOIN BvAggregateSurveyAlertStatus asas
        ON ta.ItemID = asas.SID
    INNER JOIN BvSurvey 
        ON (BvSurvey.SID = asas.SID)
    WHERE ta.BatchID = @BatchID
	AND BvSurvey.State <> 2
	AND	InterviewersLoggedCount >= @onlyActiveSurveys
GO
PRINT N'Creating [dbo].[BvSpGetPersonsLevel]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetPersonsLevel]
 @ParentSID INT,
 @Filter NVARCHAR(MAX) = NULL
AS
DECLARE @ParentRole INT
SELECT 
 @ParentRole = [g].[RoleID]
FROM
 [BvPersonGroup] [g]
WHERE
 [g].[SID] = @ParentSID


IF @ParentRole = 2
BEGIN
/*FOR CATI PERSON*/
SELECT
 [p].[SID] AS [SID],
 [p].[Name] AS [Name]
FROM   
 [BvPerson] [p]
 LEFT JOIN [BvMembership] [m] ON [p].[SID] = [m].[ObjectSID]
WHERE
 [m].[ContainerSID] = @ParentSID
 AND (@Filter IS NULL OR [p].[Name] LIKE @Filter)
END
ELSE IF @ParentRole = 64
BEGIN
/*FOR CAPI PERSON*/
SELECT
 [p].[SID] AS [SID],
 [p].[Description] AS [Name]
FROM   
 [BvPerson] [p]
 LEFT JOIN [BvMembership] [m] ON [p].[SID] = [m].[ObjectSID]
WHERE
 [m].[ContainerSID] = @ParentSID
 AND [p].[Description] <> ''
 AND (@Filter IS NULL OR [p].[Description] LIKE @Filter)
END
GO
PRINT N'Creating [dbo].[BvSpGetPersonGroupsLevel]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetPersonGroupsLevel]
 @ParentSID INT,
 @Filter NVARCHAR(MAX) = NULL
AS
 SELECT
  [g].[SID] AS [SID],
  [g].[Name] AS [Name],
  (SELECT COUNT(*)
  FROM [BvMembership] [m1]
  LEFT JOIN [BvPerson] [p] ON [p].[SID] = [m1].[ObjectSID]
  WHERE
   [m1].[ContainerSID] = [g].[SID]
  AND  [p].[Name] <> ''
  AND  (@Filter IS NULL OR [p].[Name] LIKE @Filter)
  ) AS [Count]
 FROM  [BvPersonGroup] [g]
 LEFT JOIN [BvMemberShip] [m] ON [g].[SID] = [m].[ObjectSID]
 WHERE
  [m].[ContainerSID] = @ParentSID
  AND  [g].[Name] <> ''
  AND  (@Filter IS NULL OR [g].[Name] LIKE @Filter)
GO
PRINT N'Creating [dbo].[BvSpGetOpenedSurveys]...';


GO
CREATE  PROCEDURE [dbo].[BvSpGetOpenedSurveys]
   @PersonSID INT
AS
SET NOCOUNT ON

    SELECT com.SID, com.Name
    FROM (
         SELECT DISTINCT BvSurvey.SID, BvSurvey.[Name]
         FROM BvSurvey
         INNER JOIN BvPersonRel p ON (p.PersonSID = @PersonSID AND
                                      p.ObjectSID = BvSurvey.SID AND
                                      p.Type = 2)
         WHERE BvSurvey.State = 1
 
         UNION

         SELECT DISTINCT BvSurvey.SID, BvSurvey.[Name]
         FROM BvSurvey
         INNER JOIN BvPersonRel p ON (p.PersonSID = @PersonSID AND
                                      p.Type = 1)
         INNER JOIN BvSvySchedule ss ON (p.ObjectSID = ss.ExplicitSID AND
                                         ss.ExplicitType = 2 AND
                                         ss.SurveySID = BvSurvey.SID AND
                                         ss.CallState > 0 AND
                                         ss.IsInActiveShiftType = 1)
         WHERE BvSurvey.State = 1) com
      ORDER BY com.Name

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpGetNewSID]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetNewSID]
AS
DECLARE @SID int

UPDATE BvSIDCounter 
    SET @SID = SID, SID = SID + 1

return @SID
GO
PRINT N'Creating [dbo].[BvSpGetMessages]...';


GO
CREATE PROCEDURE BvSpGetMessages 
	@InterviewerId INT
AS
BEGIN

   BEGIN TRANSACTION

        UPDATE bvPerson SET HasNewMessage = 0 WHERE SID = @InterviewerId

		DELETE bvMessageToPerson 
			OUTPUT bvMessages.Body, bvMessages.CreateTime, bvMessages.SupervisorName
			FROM bvMessages INNER JOIN bvMessageToPerson 
			ON MessageId = bvMessages.Id And InterviewerId = @InterviewerId		

	COMMIT TRANSACTION
	
END
GO
PRINT N'Creating [dbo].[BvSpGetLoggedInPersonsCount]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetLoggedInPersonsCount]
AS

SELECT COUNT (*) FROM BvTasks

RETURN 0
GO
PRINT N'Creating [dbo].[BvSpGetLiveShifts]...';


GO
create procedure [dbo].[BvSpGetLiveShifts]
@utc smalldatetime,    -- in utc time
@tz_local INT
as
set nocount on
declare @date1 int
DECLARE @ShiftTypeNone INT = -2147483648; --None constant for bvSvySchedule.ShiftTypeID

 
set @date1 = @@DATEFIRST
set DATEFIRST 7
 
    create table #temp_tz ( 
        tz_id    int,
        ltStart  smalldatetime,
        minStart int
    )
 
    create table #active
    (
        [ID] int not null,
        ScheduleID int not null,
                tz_id int not null
    )
 
 
    -- check in future
    set @utc = dateadd( minute, 1, @utc )
 
    -- insert into temp normalize date by all timezone
    -- normalize date - time in minute from start of week
    -- = day_of_week * 24 * 60 + hour * 60 + minute
    insert into #temp_tz select [ID] AS TzID, ltStart,
                        ( DATEPART( dw, ltStart ) - 1 ) * 1440 + 
                        DATEPART( hour, ltStart ) * 60 + 
                        DATEPART( minute, ltStart ) as minStart
                      from ( select  [ID], dbo.UTC2LT( @utc, Bias, DaylightType,
                                StandardDayOfWeek, StandardStart, StandardBias,
                                DaylightDayOfWeek, DaylightStart, DaylightBias ) as ltStart
                             from BvTimezone ) s1
 
        --select * from #temp_tz
 
 
 
    -- insert periodical active shifts info
    insert into #active
        select distinct z.[ID], tzs.owner_id, tzs.tz_id
        from #temp_tz
        inner join BvTzPeriodicalShifts tzs on
            #temp_tz.tz_id = tzs.tz_id
              and ( #temp_tz.minStart >= tzs.start_dt 
              and #temp_tz.minStart < tzs.finish_dt OR 
              #temp_tz.minStart + 10080/*week*/ >= tzs.start_dt 
              and #temp_tz.minStart + 10080/*week*/ < tzs.finish_dt)
        inner join BvShiftZones z on
              ( z.TimeZoneID = tzs.tz_id or
              ( z.TimeZoneID = 0 and tzs.tz_id = @tz_local ) )
              and z.ShiftTypeID = tzs.type_id
 
    -- delete shifts which fits exclusions
        delete from #active 
        from  #active a 
                        join BvTzUnPeriodicalShifts utzs on
                                a.tz_id = utzs.tz_id
                                 and a.ScheduleID = utzs.owner_id
                        join #temp_tz on #temp_tz.tz_id = utzs.tz_id
                        
                        where 
                                #temp_tz.ltStart >= utzs.start_dt and #temp_tz.ltStart < utzs.finish_dt
                        
                                                        
 
    set DATEFIRST @date1
    drop table #temp_tz

    -- insert timezones for [AnyValid] calls
    insert into #active
        select distinct -z.TimeZoneID, a.ScheduleID, a.tz_id
        from #active a, BvShiftZones z
        where a.[ID] = z.[ID]
    -- insert fictive shift for [None] calls
    insert into #active
        select @ShiftTypeNone, ScheduleID, 0 FROM BvSchedule
 
    select a.[ID], b.SID
        from #active a, BvSurvey b
        where a.ScheduleID = b.ScheduleID
        and b.State = 1 /* survey opened */
 
return (0)
GO
PRINT N'Creating [dbo].[BvSpGetListSurveyTasks]...';


GO
CREATE PROCEDURE BvSpGetListSurveyTasks
   @batchID int,
   @TimeZoneID INT
AS
   DECLARE @currTime DATETIME
   SET @currTime = GETUTCDATE()
   
   SELECT tsk.InterviewID, 
          tsk.PersonSID, 
          p.Name as InterviewerName, 
          tsk.SurveySID, 
          tsk.ProjectID, 
          tsk.SurveyName,
          tsk.TimeCallDelivered, 
          tsk.State, 
          tsk.SecondsSinceLastSubmission, 
          tsk.LastSubmissionAlert, 
          tsk.LastKeepAliveTime,
          tsk.LastKeepAliveTimeAlert,
          tsk.InterviewState,
          tsk.LoggedInToDialerState,
          tsk.TzID, 
          tsk.DiallingMode, -- if no survey assigned to task - use manual dialing mode
          tsk.CallOutcome, 
          tsk.StatusLogout,
          tsk.ProblemId,
          tz.Bias, 
          pm.supervisorName,
          pm.MonitoringSessionID,
          tsk.StationId
   FROM
   (SELECT t.InterviewID, 
          t.PersonSID, 
          t.SurveySID, 
          ISNULL(s.Name, '') as ProjectID, 
          ISNULL(s.Description, '') as SurveyName,
          (CASE WHEN t.StatusLogout != 6 /*BREAK*/ THEN t.TimeCallDelivered 
                ELSE lb.StartTime
           END) as TimeCallDelivered, 
          t.State, 
          (CASE WHEN t.InterviewID = 0 THEN NULL ELSE t.SecondsSinceLastSubmission END) as SecondsSinceLastSubmission, 
          t.LastSubmissionAlert, 
          t.LastKeepAliveTime,
          t.LastKeepAliveTimeAlert,
          t.InterviewState,
          t.LoggedInToDialerState,
          t.TzID, 
          t.DiallingMode, 
          t.CallOutcome, 
          t.StatusLogout,
          t.ProblemId,
          t.StationId
   FROM BvTasks t
   LEFT JOIN BvSurvey s ON (t.SurveySID = s.SID)
   LEFT JOIN BvTransferArrays ta ON (ta.BatchID = @batchID AND
                                      t.SurveySID = ta.ItemID)
   OUTER APPLY dbo.GetLastTimeBreak(t.PersonSID) lb
   WHERE (s.SID IS NOT NULL and ta.ItemID IS NOT NULL ) OR t.SurveySID = 0) as tsk
   INNER JOIN BvPerson p ON (tsk.PersonSID = p.SID)
   INNER JOIN BvTimezone tz ON ((CASE WHEN TzID = 0 THEN @TimeZoneID ELSE TzId END) = tz.ID)
   LEFT JOIN BvPersonMonitoring pm ON (pm.PersonSID = tsk.PersonSID)
GO
PRINT N'Creating [dbo].[BvSpGetListRange]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetListRange]
@StartIndex INT, 
@ObjectCount INT, 
@OrderField NVARCHAR (64), 
@IsOrderASC INT, 
@Query NVARCHAR (MAX), 
@IDField NVARCHAR (64), 
@SearchCondition NVARCHAR (4000)=NULL,
@CounterQuery NVARCHAR (MAX) = NULL
AS

print @StartIndex
print @ObjectCount
print @OrderField
print @IsOrderASC
print @Query
print @IDField
print @SearchCondition

IF (@SearchCondition IS NOT NULL AND @SearchCondition <> '')
BEGIN
 SET @Query = 'SELECT * FROM (' + @Query + ') t WHERE ' + @SearchCondition
END

DECLARE @TotalCount INT
DECLARE @CountQuery NVARCHAR(MAX)

IF(@CounterQuery IS NOT NULL)
	SET @CountQuery = N'with T as (' + @CounterQuery + ') select @TotalCountOut = cnt from T'
ELSE
	SET @CountQuery = N'with T as (' + @Query + ') select @TotalCountOut = count(1) from T'

EXEC sp_executesql @CountQuery, N'@TotalCountOut int output', @TotalCountOut = @TotalCount output
  
DECLARE @OrderClause AS NVARCHAR(500)
DECLARE @OrderDirection AS NVARCHAR(6)

IF (@IsOrderASC = 1)
BEGIN
   SET @OrderDirection = ' ASC '
END
ELSE
BEGIN
   SET @OrderDirection = ' DESC '
END

IF (UPPER(@OrderField) != UPPER(@IDField))
BEGIN
    SET @OrderClause = ' ORDER BY ' + @OrderField + @OrderDirection + ',' + @IDField + @OrderDirection
END
ELSE
BEGIN
    SET @OrderClause = ' ORDER BY ' + @OrderField + @OrderDirection
END

DECLARE @SQL AS NVARCHAR(MAX)
IF @ObjectCount = 2147483647
BEGIN
    -- request all records
    SET @SQL = 'SELECT * FROM (' + @Query + ') S ' + @OrderClause
END
ELSE
BEGIN
    SET @SQL = 'SELECT * FROM (SELECT S.*, ROW_NUMBER() OVER(' + @OrderClause + ') AS SpecialTempRowNumberForPaging
      FROM (' + @Query + ') S ) S
      WHERE SpecialTempRowNumberForPaging BETWEEN ' + STR(@StartIndex) + ' AND ' + STR(@StartIndex + @ObjectCount - 1) +
      @OrderClause
END

EXEC sp_executesql @SQL
RETURN ISNULL(@TotalCount, 0)
GO
PRINT N'Creating [dbo].[BvSpGetListPage]...';


GO
CREATE  PROCEDURE [dbo].[BvSpGetListPage]
@PageNumber INT, 
@PageSize INT, 
@OrderField NVARCHAR (64), 
@IsOrderASC INT,
@Query NVARCHAR (MAX), 
@IDField NVARCHAR (64), 
@SearchCondition NVARCHAR (4000)=NULL
AS
 DECLARE @StartIndex INT
 IF @PageSize != 2147483647
 BEGIN
  SET @StartIndex = (@PageNumber - 1) * @PageSize + 1
 END
 
 DECLARE @TotalCount INT
 exec @TotalCount = BvSpGetListRange @StartIndex, @PageSize, @OrderField, @IsOrderASC, @Query, @IDField, @SearchCondition
 RETURN @TotalCount
GO
PRINT N'Creating [dbo].[BvSpGetExpiredCalls]...';


GO
--TODO: we should rename this procedure on [BvSpGetExpiredCall] or [BvSpGetNextExpiredCall]
CREATE PROCEDURE [dbo].[BvSpGetExpiredCalls] 
	@MaxCount INT
AS
    DELETE TOP(@MaxCount) FROM BvCallExpired 
        OUTPUT DELETED.surveyID,
               DELETED.interviewID,
               DELETED.CallState
RETURN (@@ROWCOUNT)
GO
PRINT N'Creating [dbo].[BvSpGetDeferredMonitoringStartFile]...';


GO
CREATE PROCEDURE BvSpGetDeferredMonitoringStartFile
	@RecordID INT
AS
BEGIN
	SELECT [StartingFile] FROM [BvPersonDeferredMonitoring] WHERE [ID] = @RecordID
END
GO
PRINT N'Creating [dbo].[BvSpGetCachedCallsForPredictiveSurveyByPersonGroup]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetCachedCallsForPredictiveSurveyByPersonGroup]
 @SurveySID INT,
 @GroupID INT,	
 @Count  INT --number of requested calls
AS
--best if it should be established at the connection level
--it may influence on count of recompilations
SET NOCOUNT ON

	DECLARE @FixeNumberCallsPerPerson INT = 10

      --it should be at end of the SP. it uses for decrease recompilation
	DECLARE @CachedCalls TABLE (
	  [InterviewID] [int] NOT NULL,
	  [OrderId] [int] NOT NULL)

	DECLARE @Calls TABLE (
	  [ExplicitSID] [int] NOT NULL,
	  [ID] [int] NOT NULL,
	  [InterviewID] [int] NOT NULL,
	  [TimeInShift] [datetime] NULL,
	  [OrderId] [int] NOT NULL,
	  [ApptID] [int] not null)
        
	;WITH orderedUpdateTable AS
	(
		SELECT TOP ( @Count ) *
		FROM BvCachedCalls 
		WHERE SurveySID = @SurveySID AND
				ExplicitSid = @groupID AND 
				CallState = 2
		ORDER BY OrderId
	)
    UPDATE orderedUpdateTable 
    SET CallState = -2 
	OUTPUT inserted.[interviewID],
		   inserted.[orderId]
	INTO @CachedCalls
    
    UPDATE BvSvySchedule  
    SET CallState = -2 
	OUTPUT 0,
		   inserted.ID,
		   inserted.InterviewID,
		   inserted.TimeInShift,
		   c.OrderId,
	       inserted.ApptId
    INTO @Calls
    FROM BvSvySchedule s 
    INNER JOIN @CachedCalls c ON (s.InterviewID = c.InterviewID AND
								  SurveySID = @SurveySID)

    SELECT c.[ID], 
           c.[ExplicitSid], 
           @SurveySID SurveySid, 
           i.DialingMode DiallingMode,
		   c.[InterviewID], 
		   i.[TelephoneNumber],
		   (CASE WHEN c.ApptId > 0 THEN c.[TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
		   @GroupID as [GroupID]
    FROM @Calls c 
    INNER JOIN BvInterview i ON c.[Interviewid] = i.[ID] AND
                                i.[SurveySID] = @SurveySID
    ORDER BY orderid
 
RETURN (@@ROWCOUNT)
GO
PRINT N'Creating [dbo].[BvSpGetAppointmentCount]...';


GO
CREATE PROCEDURE BvSpGetAppointmentCount
      @batchID int
AS
   SELECT SurveySID,
          SurveyName,
          ProjectID,
          CountForShortInterval,
          CountForLongInterval
   FROM BvAppointmentCounters ac
   INNER JOIN BvTransferArrays ta ON (ta.BatchID = @batchID AND
                                      ta.ItemID = ac.SurveySID)
GO
PRINT N'Creating [dbo].[BvSpGetAppointmentActivity]...';


GO
CREATE PROCEDURE BvSpGetAppointmentActivity
   @batchID int,
   @top int = 100
AS
   SET @top = ISNULL(@top, 100)
   SELECT TOP(@top) 
         aas.[ID],
         aas.[SurveySID],
         aas.[SurveyName],
         aas.[ProjectID],
         aas.[InterviewID],
         aas.[AppointmentTime],
         aas.[TZID],
         tz.[Bias],
         aas.[Resource] InterviewerName,
         aas.[Contact],
         aas.[AlertStatus],
         aas.[CallID]
   FROM BvTransferArrays
   INNER JOIN BvAppointmentsAlertStatus aas ON (ItemID = aas.SurveySID)
   INNER JOIN BvTimezone tz ON (aas.TZID = tz.ID)
   WHERE aas.AlertStatus > 0 AND
         @batchID = BatchID
   ORDER BY aas.AppointmentTime DESC
GO
PRINT N'Creating [dbo].[BvSpGetAllAppointmentsForUser]...';


GO
CREATE PROCEDURE BvSpGetAllAppointmentsForUser
   @PersonSID INT
AS
   SELECT a.ID, a.InterviewSID, a.ContactName, a.Time, a.ExpTime, s.Name as ProjectID, s.Description as projectName, a.TZID
   FROM BvSurvey s
   INNER JOIN BvAppointment a ON ( a.State = 1 AND --call was created
                                   a.SurveySID = s.SID )
   INNER JOIN BvSvySchedule ss ON ( ss.SurveySID = s.SID AND
                                    ss.InterviewID = a.InterviewSID AND
                                    ss.CallState > 0 AND
                                    ss.ExplicitSID = @PersonSID )
   WHERE s.State = 1 --open survey
   ORDER BY a.Time

RETURN @@ROWCOUNT
GO
PRINT N'Creating [dbo].[BvSpGetActiveShiftsInRelativeTime]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetActiveShiftsInRelativeTime]
@dtStart DATETIME, @dtFinish DATETIME, @DefaultTZ INT
AS

    DECLARE @TimeSpanInMin INT = DATEDIFF( "n", @dtStart, @dtFinish );
    
    DECLARE @TimeInWeek INT = ( DATEPART( dw, @dtStart ) - 1 ) * 1440 + 
                            DATEPART( hour, @dtStart ) * 60 + 
                            DATEPART( minute, @dtStart )
                            
    CREATE TABLE #tzshift (
        [ID] [int] NOT NULL,
        [OwnerID] [int] NOT NULL,
        [ShiftTypeID] [int] NOT NULL,
        [TimeZoneID] [int] NOT NULL
    )

    DECLARE @WeekSizeInMinutes INT = 7 * 24 * 60
		
    ;WITH 
    -- select using shifts
    ShiftByTZ( owner_id, shift_id, type_id, tz_id, start_wo, finish_wo ) AS
    (
        SELECT owner_id, shift_id, type_id, tz_id, start_dt, finish_dt from BvTzPeriodicalShifts where start_dt != finish_dt /*ignore fictitious shifts*/
    ),
    --calc first future time for shift by TZ
    MatchingShiftByTZ( owner_id, shift_id, type_id, tz_id) as
    (
        SELECT s.owner_id, s.shift_id, s.type_id, s.tz_id
            FROM ShiftByTZ as s
           WHERE ( s.start_wo <= @TimeInWeek AND s.finish_wo >= (@TimeInWeek + @TimeSpanInMin) ) 
               OR ( s.finish_wo > 10080/*60*24*7*/ AND	(s.start_wo - 10080/*60*24*7*/) <= @TimeInWeek AND
														(s.finish_wo - 10080/*60*24*7*/) >= (@TimeInWeek + @TimeSpanInMin) )
    )
    INSERT INTO #tzshift
    SELECT	shift_id,
			owner_id,
			type_id, 
			tz_id
		FROM MatchingShiftByTZ ms WHERE NOT EXISTS( 
                SELECT 1 FROM BvTzUnPeriodicalShifts ex 
					WHERE ex.owner_id = ms.owner_id AND ex.tz_id = ms.tz_id AND
						ex.start_dt <= @dtStart AND ex.finish_dt > @dtStart )

	INSERT INTO #tzshift 
		SELECT ID, OwnerID, ShiftTypeID, 0 FROM  #tzshift WHERE TimeZoneID = @DefaultTZ;
		
	SELECT	[ID] as [ID],
			[OwnerID] as [OwnerID],
			[ShiftTypeID] as [ShiftTypeID],
			[TimeZoneID] as [TimeZoneID]
		FROM #tzshift
	
	DROP TABLE #tzshift
	
RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpGetActiveShifts]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetActiveShifts]
@dtStart    DATETIME,--UTC
@dtFinish   DATETIME,--UTC
@SelectType INT = 1, -- 1 - ShiftID, OwnerID, ShiftType, TimeZoneID
                     -- 2 - ShiftType, TimeZone
                     -- 3 - BvShiftZones.ID, ShiftTypeID
@DefaultTZID INT
--WITH ENCRYPTION
AS
    DECLARE @ShiftTypeNone INT = -2147483648; --None constant for bvSvySchedule.ShiftTypeID

    CREATE TABLE #tzshift (
        [ID] [int] NOT NULL,
        [OwnerID] [int] NOT NULL,
        [ShiftTypeID] [int] NOT NULL,
        [TimeZoneID] [int] NOT NULL
    )

    DECLARE @TimeSpanInMin INT
    --select @dtStart, @dtFinish
    
    SET @TimeSpanInMin = DATEDIFF( "n", @dtStart, @dtFinish );

    WITH 
    -- select using shifts
    ShiftByTZ( owner_id, shift_id, type_id, tz_id, start_wo, finish_wo ) AS
    (
        SELECT owner_id, shift_id, type_id, tz_id, start_dt, finish_dt from BvTzPeriodicalShifts where start_dt != finish_dt /*ignore fictitious shifts*/
    ),
    -- select offset from begin week in minutes by TZ for current time
    TimeAndWeekOffsetByTZ( tz_id, cur_ut, cur_lt, cur_week_start_lt, cur_tz_wo ) AS
    (
        SELECT ID, cur_ut, cur_lt, DATEADD( minute, -cur_tz_wo, cur_lt), cur_tz_wo FROM (
        SELECT [ID], @dtStart as cur_ut, cur_lt, ( DATEPART( dw, cur_lt ) - 1 ) * 1440 + 
                            DATEPART( hour, cur_lt ) * 60 + 
                            DATEPART( minute, cur_lt ) as cur_tz_wo                      
                                from ( select  [ID], dbo.UTC2LT( @dtStart, Bias, DaylightType,
                                    StandardDayOfWeek, StandardStart, StandardBias,
                                    DaylightDayOfWeek, DaylightStart, DaylightBias ) as cur_lt
                                 from BvTimezone ) s1 ) s2
    ),
    --calc first future time for shift by TZ
    MatchingShiftByTZ( owner_id, shift_id, type_id, tz_id, start_lt, finish_lt) as
    (
        SELECT s.owner_id, s.shift_id, s.type_id, s.tz_id, 
            cur_lt,--trim shift time
            DATEADD( minute, @TimeSpanInMin , cur_lt )--trim shift time
            FROM ShiftByTZ as s
            INNER JOIN TimeAndWeekOffsetByTZ two
            ON s.tz_id = two.tz_id
           WHERE ( s.start_wo <= two.cur_tz_wo AND s.finish_wo >= (two.cur_tz_wo + @TimeSpanInMin) ) 
               OR ( s.finish_wo > 10080/*60*24*7*/ AND (s.start_wo - 10080/*60*24*7*/) <= two.cur_tz_wo AND (s.finish_wo - 10080/*60*24*7*/) >= (two.cur_tz_wo + @TimeSpanInMin) )
    )
    INSERT INTO #tzshift --OUTPUT INSERTED.* 
    SELECT shift_id, owner_id, type_id, tz_id FROM MatchingShiftByTZ ms where NOT EXISTS( 
                SELECT 1 FROM BvTzUnPeriodicalShifts ex WHERE ms.start_lt >= ex.start_dt and ms.finish_lt < ex.finish_dt and ex.owner_id = ms.owner_id and ex.tz_id = ms.tz_id )

  -- Prepare default timezone
  INSERT INTO #tzshift 
    SELECT [ID], OwnerID, ShiftTypeID, 0
    FROM #tzshift WHERE TimeZoneID = @DefaultTZID

  IF @SelectType = 1
      SELECT [ID], OwnerID, ShiftTypeID, TimeZoneID FROM #tzshift
  ELSE IF @SelectType = 2
      SELECT DISTINCT ShiftTypeID, TimeZoneID FROM #tzshift
  ELSE IF @SelectType = 3
      SELECT DISTINCT BvShiftZones.[ID] , #tzshift.OwnerID
        FROM BvShiftZones, #tzshift
        WHERE BvShiftZones.TimeZoneID = #tzshift.TimeZoneID
          AND BvShiftZones.ShiftTypeID = #tzshift.ShiftTypeID
      UNION ALL
      SELECT DISTINCT -TimeZoneID, OwnerID FROM #tzshift
      UNION ALL
      SELECT @ShiftTypeNone, ScheduleID FROM BvSchedule
    drop table #tzshift
RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpFilter_Update]...';


GO
CREATE PROCEDURE [dbo].[BvSpFilter_Update]
@SID           INTEGER,
@Name          NVARCHAR(255),
@Description   NVARCHAR(255),
@AndOrOperator TINYINT,
@SurveySID     INTEGER
AS
    IF EXISTS ( SELECT * FROM BvFilters WHERE [Name] = @Name AND 
        [SID] <> @SID)
    BEGIN
        RAISERROR( N'Filter with name %s already exists', 12, 1, @Name )
        RETURN (-1)
    END
    
    IF @SurveySID > 0
    
		BEGIN		
			IF EXISTS( SELECT 1
					   FROM BvFilters f
					   LEFT JOIN dbo.udf_GetSubFilters(@SID) subFilters ON subFilters.SID = f.SID
					   LEFT JOIN dbo.udf_GetParentFilters(@SID) parentFilters ON parentFilters.SID = f.SID
					   WHERE f.SurveySID > 0 AND
							 f.SurveySID != @SurveySID AND
							 ( subFilters.SID IS NOT NULL OR
							   parentFilters.SID IS NOT NULL ) )
			BEGIN
				RAISERROR( N'Cannot update filter %s because it is used for another survey(s).', 12, 1, @Name )
				RETURN (-1)
			END
			
			UPDATE BvFilters
			SET SurveySID = @SurveySID
			FROM dbo.udf_GetParentFilters(@SID) pe
			WHERE BvFilters.SID = pe.SID
		END
	
	ELSE	
		BEGIN
		
			UPDATE BvFilters
			SET SurveySID = 0
			FROM dbo.udf_GetParentFilters(@SID) parentFilters
			WHERE BvFilters.SID = parentFilters.SID AND
				  NOT EXISTS( SELECT 1
							  FROM dbo.udf_GetSubFilters(parentFilters.SID) subFilters
							  INNER JOIN BvFilters f ON f.SID = subFilters.SID
							  INNER JOIN BvFilterFields ff ON ff.FilterSid = f.Sid AND
															  ff.[Table] = 512 --cf table
							  WHERE f.SID != @SID)
		END
		                  
	UPDATE BvFilters 
		SET    [Name] = @Name,
			   [Description] = @Description,
			   [AndOrOperator] = @AndOrOperator,
			   [SurveySID] = @SurveySID
		WHERE [SID] = @SID
		                  
RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpFilter_Insert]...';


GO
CREATE PROCEDURE [dbo].[BvSpFilter_Insert]
@Name          NVARCHAR(255),
@Description   NVARCHAR(255),
@AndOrOperator INTEGER,
@SurveySID     INTEGER,
@Hidden        INTEGER
AS
DECLARE @SID INTEGER

    IF NOT EXISTS( SELECT * FROM BvFilters WHERE [Name] = @Name )
    BEGIN
        EXEC @SID = BvSpGetNewSID

        INSERT INTO BvFilters( [SID],
           [Name],
           [Description],
           [AndOrOperator],
           [SurveySID],
           [Hidden])
        VALUES( @SID, @Name, @Description, @AndOrOperator, @SurveySID, @Hidden )
    END
    ELSE
    BEGIN
        -- GetSurveyName
        DECLARE @SurveyName NVARCHAR(255)
        
        SELECT @SurveyName = BvSurvey.[Name]
         FROM BvSurvey, BvFilters
         WHERE BvFilters.[Name] = @Name AND
           BvSurvey.SID = BvFilters.SurveySID

        IF @SurveyName IS NULL
          RAISERROR( N'Filter with such name already exists', 12, 1 )
        ELSE
          RAISERROR( N'The name you entered reserved for "%s" survey', 12, 1, @SurveyName )

        RETURN (-1)
    END

RETURN (@SID)
GO
PRINT N'Creating [dbo].[BvSpFilter_DeleteFields]...';


GO
/* This sp don't change survey specification of filter.
   We should execute this sp change filter fields (delete/insert)
 */
CREATE PROCEDURE [dbo].[BvSpFilter_DeleteFields]
@FilterSID   INTEGER
AS
	DELETE FROM BvFilterFields WHERE FilterSID = @FilterSID
RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpFilter_Delete]...';


GO
CREATE PROCEDURE [dbo].[BvSpFilter_Delete] 
@SID    INTEGER
AS

    DECLARE @SurveySID INT
    SELECT @SurveySID = SurveySID
    FROM BvFilters
    WHERE SID = @SID
    
    DECLARE @changedFilters TABLE(SID INT)

    DELETE FROM BvFilterFields 
    OUTPUT DELETED.FilterSid
    INTO @changedFilters
    WHERE [Sign] = 8 AND 
          CAST( Value AS INTEGER ) = @SID
    
    IF(@SurveySID > 0)
       UPDATE BvFilters
       SET SurveySID = 0
       FROM @changedFilters changes
       CROSS APPLY dbo.udf_GetParentFilters(changes.SID) parentFilters
       WHERE BvFilters.SID = parentFilters.SID AND
		     NOT EXISTS( SELECT 1
		                 FROM dbo.udf_GetSubFilters(parentFilters.SID) subFilters
		                 INNER JOIN BvFilters f ON f.SID = subFilters.SID
		                 INNER JOIN BvFilterFields ff ON ff.FilterSid = f.Sid AND
		                                                  ff.[Table] = 512 --cf table
		                 WHERE f.SID != @SID)
    
    DELETE FROM BvFilterFields WHERE FilterSID = @SID    
    
    DELETE FROM BvFilters WHERE SID = @SID

RETURN(0)
GO
PRINT N'Creating [dbo].[BvSpFilter_CheckSurveyMismatch]...';


GO
CREATE PROCEDURE [dbo].[BvSpFilter_CheckSurveyMismatch]
@FilterSID    INTEGER,
@SubFilterSID    INTEGER
AS
SET NOCOUNT ON
        
	DECLARE @Ret  INT

    SELECT @Ret = COUNT( DISTINCT SurveySID )
    FROM BvFilters f
    LEFT JOIN dbo.udf_GetSubFilters(@FilterSID) subFilters ON f.SID = subFilters.SID
    LEFT JOIN dbo.udf_GetSubFilters(@SubFilterSID) subFiltersForSubFilter ON f.SID = subFiltersForSubFilter.SID
    LEFT JOIN dbo.udf_GetParentFilters(@FilterSID) parentFilters ON f.SID = parentFilters.SID
    WHERE SurveySID != 0 AND
          ( subFilters.SID IS NOT NULL OR subFiltersForSubFilter.SID IS NOT NULL OR parentFilters.SID IS NOT NULL)
 
    IF @Ret > 1
        SET @Ret = 1
    ELSE
        SET @Ret = 0

RETURN (@Ret)
GO
PRINT N'Creating [dbo].[BvSpFilter_CheckCircle]...';


GO
CREATE PROCEDURE [dbo].[BvSpFilter_CheckCircle]
	@FilterSID    INTEGER,
	@SubFilterSID INTEGER
AS
SET NOCOUNT ON

	DECLARE @Ret INT = 0
    SELECT @Ret = COUNT(*) FROM dbo.udf_GetSubFilters(@SubFilterSID) WHERE SID = @FilterSID
    IF @Ret > 1
       SET @Ret = 1

RETURN @Ret
GO
PRINT N'Creating [dbo].[BvSpExecuteForAllSurveys]...';


GO
create procedure [dbo].[BvSpExecuteForAllSurveys]
@cm varchar(8000),
@tables int,          -- 1 - BvInterview 
                      -- 2 - BvSvySchedule 
                      -- 4 - BvOpenend not used
                      -- 8 - BvAppointment 
                      -- 16 - BvHistory
@sid_str varchar(30),
@sid    int
as
set xact_abort on
declare @cm2 varchar(8000)
declare @to_sid varchar(12)

declare crSurvey cursor local for select [SID] from BvSurvey 
 inner join BvNumber on BvSurvey.SID = BvNumber.ObjectSID
 

declare @SurveySID int
declare @survey varchar(12)
declare @rows int

    set @to_sid = cast( @sid as varchar( 12 ) )

    set @cm2 = replace( @cm, @sid_str, @to_sid )

    set @cm = @cm2

    set @rows = 0

    open crSurvey
    fetch next from crSurvey into @SurveySID

    while ( @@fetch_status = 0 ) begin

        set @cm2 = @cm
        set @survey = cast( @SurveySID as varchar(12) )

        execute( @cm2 )

        set @rows = @rows + @@rowcount

        fetch next from crSurvey into @SurveySID
    end

    close crSurvey
    deallocate crSurvey

return (@rows)
GO
PRINT N'Creating [dbo].[BvSpDialer_Reset]...';


GO
CREATE PROCEDURE [dbo].[BvSpDialer_Reset]
    @ProblemID INT
AS    
    UPDATE BvCachedCalls
    SET CallState = 2 
    WHERE CallState = -2
    
    UPDATE BvSvySchedule 
    SET CallState = 2 
    WHERE CallState = -2

    UPDATE BvTasks 
    SET ProblemId = @ProblemID 
    WHERE LoggedInToDialerState = 1/*LOGGING_IN*/ OR 
          LoggedInToDialerState = 2/*LOGGED_IN*/
GO
PRINT N'Creating [dbo].[BvSpDeleteTransfer]...';


GO
CREATE PROCEDURE [dbo].[BvSpDeleteTransfer]
@BatchID INTEGER
AS

    DELETE FROM BvTransferArrays WHERE BatchID = @BatchID

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpCreateTransferBatchID]...';


GO
CREATE PROCEDURE [dbo].[BvSpCreateTransferBatchID]
@bibb INT
AS
DECLARE @BatchID INTEGER

    UPDATE BvTransferBatches 
    SET LastBatchID = LastBatchID + 1,
        @BatchID = LastBatchID + 1

RETURN (@BatchID)
GO
PRINT N'Creating [dbo].[BvSpConfirmitStatus_List]...';


GO
CREATE PROCEDURE [dbo].[BvSpConfirmitStatus_List]
AS
BEGIN
SELECT 
 [BvConfirmitStatus].[StatusCode_Cnf] AS [ConfirmitCode],
 [BvConfirmitStatus].[StatusName_Cnf] AS [ConfirmitName],
 [BvConfirmitStatus].[StatusCode_BvFEE] AS [FusionCode]
FROM
 [BvConfirmitStatus]
WHERE cast([StatusCode_BvFEE] as nvarchar(256)) != [StatusCode_Cnf] OR
      [StatusCode_Cnf] IS NULL
END
GO
PRINT N'Creating [dbo].[BvSpCleanMessages]...';


GO
CREATE PROCEDURE [dbo].[BvSpCleanMessages]
@ExpirationPeriod INT
AS
BEGIN

	DELETE from bvMessages
	WHERE DateAdd(day, @ExpirationPeriod, bvMessages.CreateTime) < GETUTCDATE()
 
END
GO
PRINT N'Creating [dbo].[BvSpCleanDeferredMonitoring]...';


GO
CREATE PROCEDURE [BvSpCleanDeferredMonitoring]
	@ExpirationPeriodInDays INT,
	@DeleteTopRows INT
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @EpirationTime DATETIME = DateAdd(day, -@ExpirationPeriodInDays, GETUTCDATE())
	DECLARE @DeletedRowCount INT
	DELETE TOP(@DeleteTopRows) FROM [BvPersonDeferredMonitoring]
	WHERE [TimeStamp] < @EpirationTime
	SET @DeletedRowCount = @@ROWCOUNT;
	RETURN @DeletedRowCount	
END
GO
PRINT N'Creating [dbo].[BvSpCheckCallOnShifts]...';


GO
CREATE PROCEDURE [dbo].[BvSpCheckCallOnShifts]
@TimeZoneID     INT,
	/* 
	 * @ShiftTypeID > 0 means specific shift type id( BvShiftType.ID )
	 * @ShiftTypeID = 0 means [None]
	 * @ShiftTypeID =-1 @ShiftTypeID means [Any valid]
	 */

@ShiftTypeID    INT, 
@TimeInShift    DATETIME,   -- In UTC
@SurveySID      INT,
@DefaultTimeZoneID INT
AS
DECLARE @Bias INT
DECLARE @OwnerID INT

      IF @ShiftTypeID IS NULL OR @ShiftTypeID = 0
          RETURN (0)

      SELECT @OwnerID = [ScheduleID] FROM BvSurvey
            WHERE [SID] = @SurveySID
            
      IF @TimeZoneID = 0
	  SET @TimeZoneID = @DefaultTimeZoneID
            
      IF NOT EXISTS ( SELECT 1 FROM BvTzPeriodicalShifts
					  WHERE (type_id = @ShiftTypeID or @ShiftTypeID = -1) AND
				             owner_id = @OwnerID and tz_id = @TimeZoneID and start_dt != finish_dt )
	   BEGIN	
			IF(@ShiftTypeID = -1)				
				RAISERROR( 'Scheduling script does not contain any shift types', 12, 1)
			ELSE
				BEGIN		
					DECLARE @ShiftTypeName as nvarchar(20)
					SET @ShiftTypeName = (select Name from BvShiftType where ObjectID = @ShiftTypeID)
				
					RAISERROR( 'Scheduling script does not contain specific shift type with name %s', 12, 1, @ShiftTypeName)
				END
			RETURN (-1)
	   END

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpCall_MoveToITS]...';


GO
CREATE PROCEDURE [dbo].[BvSpCall_MoveToITS]
@SurveySID   INTEGER,
@BatchID    INTEGER,
@StateID     INTEGER
AS
   DECLARE @CfDbSchemaPath NVARCHAR(255)
   DECLARE @ProcessedCalls INT = 0
   DECLARE @SurveySchedulingMode INT 
   SELECT @CfDbSchemaPath = CfDbSchemaPath,
		  @SurveySchedulingMode = SurveySchedulingMode
   FROM BvSurvey
   WHERE SID = @SurveySID
   
   CREATE TABLE #InterviewIds(Id INT)
   
   UPDATE BvInterview
   SET TransientState = @StateID 
   OUTPUT inserted.ID
   INTO #InterviewIds
   FROM BvInterview i
   INNER JOIN BvTransferArrays ta ON i.ID = ta.ItemID AND
									 ta.BatchID = @BatchID AND
									 i.SurveySID = @SurveySID
   LEFT JOIN BvSvySchedule s ON i.Id = s.InterviewId AND
                                s.SurveySid = @SurveySID
   WHERE ISNULL(s.CallState, 1) > 0
         
   SET @ProcessedCalls = @@ROWCOUNT
   
   UPDATE BvSvySchedule 
   SET Priority = BvState.Priority,
       OldPriority = 0,
	   ConditionValue = CASE WHEN @SurveySchedulingMode = 1 THEN @StateID ELSE 0 END
   FROM #InterviewIds ids
   INNER JOIN BvState ON BvState.StateID = @StateID
   INNER JOIN BvSurvey ON BvSurvey.SID = @SurveySID AND
                          BvState.StateGroupID = BvSurvey.StateGroupID
   WHERE BvSvySchedule.SurveySID = @SurveySID AND 
         BvSvySchedule.InterviewId = ids.Id AND
         BvSvySchedule.CallState > 0
   
   IF((@ProcessedCalls != 0) AND (@CfDbSchemaPath IS NOT NULL) AND (@CfDbSchemaPath != ''))
   BEGIN
	   DECLARE @Query NVARCHAR(1024)
	   SET @Query = 'UPDATE '+@CfDbSchemaPath+'.response_control '+
					'SET ITS = '+cast(@StateID as nvarchar(10))+ ' ' +
					'FROM #InterviewIds as ids '+
					'WHERE respid = ids.ID '
	   EXECUTE( @Query )
   END

   INSERT INTO BvCachedCallsInsert
   SELECT ids.Id, @SurveySID 
   FROM #InterviewIds ids

   EXEC BvSpDeleteTransfer @BatchID

RETURN @ProcessedCalls
GO
PRINT N'Creating [dbo].[BvSpCall_GetInfo]...';


GO
CREATE PROCEDURE [dbo].[BvSpCall_GetInfo]
 @CallID INT
AS
 SELECT
   [ID] callId,
   [ApptID],
   [SurveySID],
   [InterviewID] iid,
   [CallState],
   [ShiftTypeID] ShiftID,
   [Priority],
   [TimeInShift],
   [ExpireTime] TimeToExpire,
   [ExplicitSID] Resource,
   [ExplicitType] Resource_Type,
   [OldPriority],
   [RuleNumber],
   [ConditionValue]
 FROM [dbo].[BvSvySchedule]
 WHERE [ID] = @CallID
GO
PRINT N'Creating [dbo].[BvSpCall_Get]...';


GO
CREATE PROCEDURE [dbo].[BvSpCall_Get]
    @SurveyID int,
    @InterviewID int,
    @Delete int,
    @GetLiveCall int = 0
AS
	DECLARE @OldCallState INT
	DECLARE @IsLockObtained INT = 0

	IF @Delete = 1
	BEGIN
		UPDATE BvCachedCalls
		SET CallState = -1 
		WHERE SurveySID = @SurveyID AND 
             InterviewID = @InterviewID AND
             CallState > 0
       
       UPDATE BvSvySchedule 
       SET	@OldCallState = CallState,
			CallState = -1
       WHERE SurveySID = @SurveyID AND 
             InterviewID = @InterviewID AND
             CallState > 0
             
        SET @IsLockObtained = @@ROWCOUNT
             
		UPDATE BvAppointment
		SET STATE = 2
		WHERE SurveySID = @SurveyID AND
			  InterviewSID = @InterviewID AND
			  STATE = 1
    END

	SELECT
		BvSvySchedule.[ID] callid,
		BvSvySchedule.ApptID,
		BvSvySchedule.SurveySID,
		BvSvySchedule.InterviewID iid,
		ISNULL( @OldCallState, BvSvySchedule.CallState ) as CallState,
		ISNULL( BvShiftZones.[ShiftTypeID], BvSvySchedule.[ShiftTypeID] ) ShiftID,
		BvSvySchedule.Priority,
		BvSvySchedule.TimeInShift,
		BvSvySchedule.ExpireTime TimeToExpire,
		CASE WHEN BvSvySchedule.ExplicitType = 2 THEN BvSvySchedule.ExplicitSID ELSE 0 END AS Resource,
		BvSvySchedule.ExplicitType Resource_Type,
		OldPriority,
		RuleNumber,
		ConditionValue
	FROM BvSvySchedule 
	LEFT JOIN BvShiftZones ON BvSvySchedule.ShiftTypeID = BvShiftZones.[ID]
	WHERE BvSvySchedule.SurveySID = @SurveyID AND 
		 BvSvySchedule.InterviewID = @InterviewID AND
		 ( ISNULL( @OldCallState, BvSvySchedule.CallState ) > 0 OR ( @GetLiveCall <> 0 AND ISNULL( @OldCallState, BvSvySchedule.CallState ) < 0 AND ISNULL( @OldCallState, BvSvySchedule.CallState ) > -3) )
			 
RETURN @IsLockObtained
GO
PRINT N'Creating [dbo].[BvSpCall_ChangeShiftType]...';


GO
CREATE PROCEDURE [dbo].[BvSpCall_ChangeShiftType]
@SurveySID   INTEGER,
	/* 
	 * @ShiftTypeID > 0 means specific shift type id( BvShiftType.ID ) and should be resolved to ShiftZoneId in bvSvySchedule.ShiftTypeID
	 * @ShiftTypeID = Int32.MinValue(-2147483648) meens [None] and should ne resolved to Int32.MinValue in BvSvySchedule.ShiftTypeID
	 * @ShiftTypeID =-1 @ShiftTypeID means [Any valid] and should be resolved to -Timezone in BvSvySchedule.ShiftTypeID
	 */
@ShiftTypeID INTEGER,
@BatchID     INTEGER,
@SiteTimeZoneID INTEGER
AS

	DECLARE @ShiftTypeNone INT = -2147483648; --None constant for bvSvySchedule.ShiftTypeID

    /*
     * Get next matching shifts by TZ
     */
    IF @ShiftTypeID <> @ShiftTypeNone
    BEGIN
        DECLARE @owner_id INT

        SELECT @owner_id = [ScheduleID] FROM BvSurvey
                WHERE [SID] = @SurveySID

        IF @owner_id IS NULL
        BEGIN
            RAISERROR( 'Scheduling script not found.', 16, 1 )
            RETURN(-1)
        END
     
        DECLARE @ErrorTimezoneList NVARCHAR(MAX)
        SET @ErrorTimezoneList = ''
        
        IF NOT EXISTS( SELECT 1 FROM BvShiftType WHERE OwnerSID = @owner_id AND ( ObjectID = @ShiftTypeID OR @ShiftTypeID = -1 ))
        BEGIN
			IF @ShiftTypeID = -1
				RAISERROR( 'Shceduling script doesn''t contain any shifttypes', 12, 1)
			ELSE
				RAISERROR( 'Scheduling script does not contain specific shift type with ID = %d', 12, 1, @ShiftTypeID)
			RETURN(-1)
        END
        
        ;WITH AvailableTz( tz_id ) AS
        (
			SELECT DISTINCT tz_id FROM BvTzPeriodicalShifts
				WHERE (type_id = @ShiftTypeID OR  @ShiftTypeID = -1) and start_dt <> finish_dt
        )
        SELECT @ErrorTimezoneList = CASE WHEN LEN(@ErrorTimezoneList) > 0 THEN @ErrorTimezoneList + ',' ELSE '' END + CAST( ISNULL(i.TimezoneID,0) AS NVARCHAR(MAX) ) FROM (
            SELECT DISTINCT TimezoneID 
            FROM  BvSvySchedule
            INNER JOIN BvTransferArrays ON BatchID = @BatchID AND 
                                           ItemID = BvSvySchedule.[ID]
            INNER JOIN BvInterview i ON i.[ID] = BvSvySchedule.InterviewID AND 
                                        i.SurveySID = @SurveySID
            LEFT JOIN AvailableTz atz ON atz.tz_id = i.TimezoneID OR 
                                         ( i.TimezoneID IS NULL AND atz.tz_id = @SiteTimeZoneID )
            WHERE atz.tz_id IS NULL AND
                  BvSvySchedule.CallState > 0 ) i

        IF LEN( @ErrorTimezoneList ) > 0
        BEGIN
            RAISERROR( 'Operation cannot be completed, the assigned scheduling script does not support the following timezone ID(s): "%s" for the selected shift type.To resolve this, in scheduling either add a default shift(s) or add the specific timezone shift(s) for this shift type.', 12, 1, @ErrorTimezoneList )
            RETURN -1
        END
    END

    -- [Any Valid] 
    IF @ShiftTypeID = -1 
    BEGIN
        UPDATE BvSvySchedule 
            SET BvSvySchedule.ShiftTypeID = -ISNULL(i.TimezoneID, 0 ),
                Priority = CASE WHEN OldPriority > 0 THEN OldPriority ELSE Priority END,
                OldPriority = 0
        FROM BvSvySchedule
        INNER JOIN BvTransferArrays ON BatchID = @BatchID
            AND ItemID = BvSvySchedule.[ID]
        INNER JOIN BvInterview i ON i.[ID] = BvSvySchedule.InterviewID
            AND i.SurveySID = @SurveySID
        WHERE BvSvySchedule.CallState > 0
    END
    ELSE 
    BEGIN
        IF @ShiftTypeID > 0 --Specific shift
            UPDATE BvSvySchedule 
                SET ShiftTypeID = BvShiftZones.[ID],
                    Priority = CASE WHEN OldPriority > 0 THEN OldPriority ELSE Priority END,
                    OldPriority = 0
            FROM BvSvySchedule
            INNER JOIN BvInterview ON BvInterview.SurveySID = @SurveySID
                AND BvSvySchedule.InterviewID = BvInterview.[ID]
            INNER JOIN BvShiftZones ON BvShiftZones.ShiftTypeID = @ShiftTypeID
                AND ISNULL(BvInterview.TimezoneID, 0 ) = BvShiftZones.TimeZoneID
            INNER JOIN BvTransferArrays ON BvTransferArrays.BatchID = @BatchID
                AND ItemID = BvSvySchedule.[ID]
            WHERE BvSvySchedule.CallState > 0
        ELSE--[None]
            UPDATE BvSvySchedule 
            SET ShiftTypeID = @ShiftTypeNone,
                Priority = CASE WHEN OldPriority > 0 THEN OldPriority ELSE Priority END,
                OldPriority = 0
            FROM BvSvySchedule
            INNER JOIN BvTransferArrays ON BvTransferArrays.BatchID = @BatchID
                AND ItemID = BvSvySchedule.[ID]
            WHERE BvSvySchedule.CallState > 0
    END

    INSERT INTO BvCachedCallsInsert
    SELECT c.InterviewID, @SurveySID 
	FROM BvSvySchedule c
    INNER JOIN BvTransferArrays a ON a.BatchID = @BatchID AND
									 a.ItemID = c.[ID]
    WHERE c.SurveySID = @SurveySID
    
RETURN(0)
GO
PRINT N'Creating [dbo].[BvSpCall_ChangePriority]...';


GO
CREATE  PROCEDURE [dbo].[BvSpCall_ChangePriority]
    @SurveySID INTEGER,
    @Mode INTEGER,
    @Priority INTEGER,
    @BatchID INTEGER
AS
   DECLARE @Temp TABLE(callId int)

   UPDATE BvSvySchedule 
   SET Priority = @Priority,
       OldPriority = 0
   OUTPUT inserted.ID
   INTO @Temp
   FROM BvTransferArrays ta
   WHERE ta.BatchID = @BatchID AND 
         ta.ItemID = [ID] AND
         CallState > 0
   
   IF(@MODE != 6 AND @MODE != 7)
   BEGIN
      RAISERROR( '@Mode parameter is incorrect', 16, 1 )
      RETURN(0);
   END

   INSERT INTO BvCachedCallsInsert
   SELECT s.InterviewID, s.SurveySID
   FROM @Temp
   INNER JOIN BvSvySchedule s ON s.ID = CallID

RETURN(0)
GO
PRINT N'Creating [dbo].[BvSpCallHistory_List]...';


GO
CREATE PROCEDURE [dbo].[BvSpCallHistory_List]
@InterviewID     INTEGER,
@SurveyID        INTEGER
AS
SET NOCOUNT OFF
     SELECT
          BvHistory.ID AS [ID],
          BvHistory.SurveyId AS SurveyID,
          BvHistory.FiredTime AS EndTime,
          BvHistory.InterviewID AS InterviewID,
          BvState.[StateID] AS ITS_ID,
          BvState.[Name] AS TransientState,
          BvHistory.WaitingTime AS WaitingTime,
          BvHistory.Duration AS Duration,
          ISNULL( BvRole.[Name], '' ) AS Role,
          ISNULL( BvPerson.[Name], '' ) AS Person,
          BvHistory.AppointmentID AS AppointmentID,
          ISNULL(BvAppointment.ContactName, '' ) AS ContactName,
          BvAppointment.[Time] AS TimeToCall,
          BvAppointment.ExpTime AS TimeToExpire,
          ISNULL(BvInterview.TelephoneNumber, '' ) AS TelephoneNumber,
          ISNULL(BvInterview.RespondentName, '' ) AS RespondentName,
          ISNULL(BvInterview.TimezoneID, 0 ) AS TimeZoneID,
          ISNULL(BvTimezone.[Name], '' ) AS TimeZone,
          'IsHistoryItemForChildInterview' = CAST(0 AS BIT)
     FROM BvHistory
     INNER JOIN BvSurvey ON BvSurvey.SID = BvHistory.SurveyId
     INNER JOIN BvState ON BvState.StateGroupID = BvSurvey.StateGroupID AND BvState.[StateID] = BvHistory.ITS
     LEFT JOIN BvPerson ON BvPerson.SID = BvHistory.PersonSID
     LEFT JOIN BvRole ON BvRole.RoleID = BvHistory.RoleID
     LEFT JOIN BvAppointment ON BvAppointment.[ID] = BvHistory.AppointmentID
     INNER JOIN BvInterview  ON ( BvInterview.[ID] = @InterviewID ) AND
        BvInterview.SurveySID = @SurveyID
     LEFT JOIN BvTimezone ON BvTimezone.[ID] = BvInterview.TimezoneID
     WHERE BvHistory.InterviewID = BvInterview.[ID]
                      AND BvHistory.SurveyId = @SurveyID
     ORDER BY DATEADD( s, -ConfirmitDuration, FiredTime)

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpCache_NotifyUpdated]...';


GO
create procedure [dbo].[BvSpCache_NotifyUpdated]
as
    truncate table BvActiveShiftTypeZone
return (0)
GO
PRINT N'Creating [dbo].[BvSpCache_GetCalls]...';


GO
CREATE PROCEDURE dbo.BvSpCache_GetCalls
   @TimeToRun datetime,
   @InterviewsCountPerPerson INT
as
   
   TRUNCATE TABLE [BvCachedCallsSwapTable]

   ;WITH OrderedCallsForCache AS
   (
	   SELECT DISTINCT ca.*
	   FROM ( SELECT v.sid AS ObjectSID, v.SurveySID AS SurveySID, sum(v.cnt) * @InterviewsCountPerPerson AS CNT
			  FROM vLogins v with ( noexpand, INDEX([pk_vLogins]) )
			  INNER JOIN BvUniqueAssignments a on a.sid = v.sid
			  LEFT JOIN BvSurvey s on s.SID = v.sid AND ( s.State <> 1 OR s.SurveySchedulingMode <> 0/*Normal mode*/ )
			  WHERE s.SID IS NULL
			  GROUP BY v.sid, v.SurveySID ) AS assignment
	   CROSS APPLY dbo.GetCallsForCacheTable( assignment.CNT, assignment.ObjectSID, assignment.SurveySID,  @TimeToRun) ca
	),
	CallsForCache AS
	(
		SELECT [ID],
			[ExplicitSID],
			[SurveySID],
			[InterviewID],
			[CallState],
			(CASE WHEN [ApptID] > 0 THEN [TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
			 ROW_NUMBER() OVER(order by priority DESC, TimeInShift, ExplicitType DESC, CallOrder) OrderId 
		 FROM OrderedCallsForCache
	)
	INSERT INTO [BvCachedCallsSwapTable]
	SELECT *
	FROM CallsForCache

	DELETE FROM [BvCachedCallsSwapTable]
	FROM [BvCachedCalls] WITH(TABLOCKX)
	WHERE [BvCachedCallsSwapTable].SurveySID = [BvCachedCalls].SurveySID AND
		  [BvCachedCallsSwapTable].InterviewID = [BvCachedCalls].InterviewID AND
		  [BvCachedCalls].CallState <= 0

	DELETE FROM [BvCachedCalls]

	INSERT INTO [BvCachedCalls]
	SELECT * FROM [BvCachedCallsSwapTable]
			
	UPDATE STATISTICS BvCachedCalls; 

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpBvID_Delete]...';


GO
CREATE PROCEDURE [dbo].[BvSpBvID_Delete]
        @ClassID        INT,
        @ObjectSID      INT
AS
    SET NOCOUNT ON

    DECLARE @bvID INT

    DELETE FROM BvNumber 
 WHERE ClassID = @ClassID AND ObjectSID = @ObjectSID

RETURN( 0 )
GO
PRINT N'Creating [dbo].[BvSpAssignment_List]...';


GO
CREATE PROCEDURE [dbo].[BvSpAssignment_List]
    @SurveySID INT,
    @PersonSID INT
AS
SET NOCOUNT ON
    IF @SurveySID <> 0 
    BEGIN
        IF @PersonSID <> 0
            SELECT BvPersonOrGroupAssignmentOnSurvey.Id AS AssignmentSID,
                   BvSurvey.SID AS SurveySID,
                   BvSurvey.[Name] AS SurveyName,
                   0 AS IsSurveyGroup,
                   0 AS Counts,
                   BvPersonOrGroupAssignmentOnSurvey.PersonOrGroupId AS PersonSID,
                   BvViewPersonAndGroup.[Name] AS Name,
                   BvViewPersonAndGroup.IsGroup AS IsPersonGroup
            FROM BvPersonOrGroupAssignmentOnSurvey, BvSurvey, BvViewPersonAndGroup, BvPersonRel
            WHERE   BvPersonOrGroupAssignmentOnSurvey.PersonOrGroupId = BvViewPersonAndGroup.SID
                AND BvPersonOrGroupAssignmentOnSurvey.SurveyId = @SurveySID
                AND BvSurvey.SID = @SurveySID
                AND BvViewPersonAndGroup.SID = BvPersonRel.ObjectSID
                AND BvPersonRel.PersonSID = @PersonSID
          UNION ALL
            SELECT BvSvySchedule.ExplicitSID AS AssignmentSID,
                   BvSurvey.SID AS SurveySID,
                   BvSurvey.[Name] AS SurveyName,
                   0 AS IsSurveyGroup,
                   COUNT(*) AS Counts,
                   BvSvySchedule.ExplicitSID AS PersonSID,
                   BvViewPersonAndGroup.[Name] AS Name,
                   BvViewPersonAndGroup.IsGroup AS IsPersonGroup
             FROM BvSvySchedule WITH(NOLOCK), BvSurvey, BvPersonRel, BvViewPersonAndGroup
             WHERE --BvSvySchedule.ExplicitType = 2
               BvSvySchedule.SurveySID = @SurveySID
               AND BvSvySchedule.ExplicitSID = BvPersonRel.ObjectSID
               AND BvSurvey.SID = @SurveySID
               AND BvPersonRel.PersonSID = @PersonSID
               AND BvViewPersonAndGroup.SID = BvPersonRel.ObjectSID
               AND BvSvySchedule.CallState > 0
            GROUP BY BvSurvey.SID, BvSurvey.[Name], BvSvySchedule.CallState,
                BvSvySchedule.ExplicitSID, BvViewPersonAndGroup.[Name],
                BvViewPersonAndGroup.IsGroup
        ELSE
            SELECT BvPersonOrGroupAssignmentOnSurvey.Id AS AssignmentSID,
                   BvSurvey.SID AS SurveySID,
                   BvSurvey.[Name] AS SurveyName,
                   0 AS IsSurveyGroup,
                   0 AS Counts,
                   BvPersonOrGroupAssignmentOnSurvey.PersonOrGroupId AS PersonSID,
                   BvViewPersonAndGroup.[Name] AS Name,
                   BvViewPersonAndGroup.IsGroup AS IsPersonGroup
            FROM BvPersonOrGroupAssignmentOnSurvey, BvSurvey, BvViewPersonAndGroup
            WHERE   BvPersonOrGroupAssignmentOnSurvey.PersonOrGroupId = BvViewPersonAndGroup.SID
                AND BvPersonOrGroupAssignmentOnSurvey.SurveyId = @SurveySID
                AND BvSurvey.SID = @SurveySID
          UNION ALL
            SELECT BvSvySchedule.ExplicitSID AS AssignmentSID,
                   BvSurvey.SID AS SurveySID,
                   BvSurvey.[Name] AS SurveyName,
                   0 AS IsSurveyGroup,
                   COUNT(*) AS Counts,
                   BvSvySchedule.ExplicitSID AS PersonSID,
                   BvViewPersonAndGroup.[Name] AS Name,
                   BvViewPersonAndGroup.IsGroup AS IsPersonGroup
             FROM BvSvySchedule WITH(NOLOCK), BvSurvey, BvViewPersonAndGroup
             WHERE --BvSvySchedule.ExplicitType = 2
               BvSvySchedule.SurveySID = @SurveySID
               AND BvSvySchedule.ExplicitSID = BvViewPersonAndGroup.SID
               AND BvSurvey.SID = @SurveySID
               AND BvSvySchedule.CallState > 0
            GROUP BY BvSurvey.SID, BvSurvey.[Name], BvSvySchedule.CallState,
                BvSvySchedule.ExplicitSID, BvViewPersonAndGroup.[Name],
                BvViewPersonAndGroup.IsGroup
    END
    ELSE
    BEGIN
        IF @PersonSID <> 0  
            SELECT BvPersonOrGroupAssignmentOnSurvey.Id AS AssignmentSID,
                BvSurvey.SID AS SurveySID,
                BvSurvey.[Name] AS SurveyName,
                0 AS IsSurveyGroup,
                0 AS Counts,
                BvPersonOrGroupAssignmentOnSurvey.PersonOrGroupId AS PersonSID,
                BvViewPersonAndGroup.[Name] AS Name,
                BvViewPersonAndGroup.IsGroup AS IsPersonGroup
            FROM  BvPersonOrGroupAssignmentOnSurvey, BvSurvey, BvViewPersonAndGroup, BvPersonRel
            WHERE BvPersonOrGroupAssignmentOnSurvey.PersonOrGroupId = BvViewPersonAndGroup.SID
                AND BvPersonOrGroupAssignmentOnSurvey.PersonOrGroupId = BvPersonRel.ObjectSID
                AND BvPersonOrGroupAssignmentOnSurvey.SurveyId = @SurveySID
                AND BvPersonRel.PersonSID = @PersonSID
				AND BvSurvey.State <> 2
          UNION ALL
            SELECT BvSvySchedule.ExplicitSID AS AssignmentSID,
                BvSurvey.SID AS SurveySID,
                BvSurvey.[Name] AS SurveyName,
                0 AS IsSurveyGroup,
                COUNT(*) AS Counts,
                BvSvySchedule.ExplicitSID AS PersonSID,
                BvViewPersonAndGroup.[Name] AS Name,
                BvViewPersonAndGroup.IsGroup AS IsPersonGroup
            FROM BvSvySchedule WITH(NOLOCK), BvSurvey, BvViewPersonAndGroup, BvPersonRel
            WHERE --BvSvySchedule.ExplicitType = 2
               BvSvySchedule.ExplicitSID = BvViewPersonAndGroup.SID
               AND BvSvySchedule.ExplicitSID = BvPersonRel.ObjectSID
               AND BvSurvey.SID = BvSvySchedule.SurveySID
--               AND BvSvySchedule.RoleID = BvPersonRel.RoleID
               AND BvPersonRel.PersonSID = @PersonSID
               AND BvSvySchedule.CallState > 0
			   AND BvSurvey.State <> 2
            GROUP BY BvSurvey.SID, BvSurvey.[Name], BvSvySchedule.CallState,
                BvSvySchedule.ExplicitSID, BvViewPersonAndGroup.[Name],
                BvViewPersonAndGroup.IsGroup
        ELSE
            SELECT BvPersonOrGroupAssignmentOnSurvey.Id AS AssignmentSID,
                BvSurvey.SID AS SurveySID,
                BvSurvey.[Name] AS SurveyName,
                0 AS IsSurveyGroup,
                0 AS Counts,
                BvPersonOrGroupAssignmentOnSurvey.PersonOrGroupId AS PersonSID,
                BvViewPersonAndGroup.[Name] AS Name,
                BvViewPersonAndGroup.IsGroup AS IsPersonGroup
            FROM    BvPersonOrGroupAssignmentOnSurvey WITH(NOLOCK), BvSurvey, BvViewPersonAndGroup
            WHERE   BvPersonOrGroupAssignmentOnSurvey.PersonOrGroupId = BvViewPersonAndGroup.SID
                AND BvPersonOrGroupAssignmentOnSurvey.SurveyId = BvSurvey.SID
				AND BvSurvey.State <> 2
          UNION ALL
            SELECT BvSvySchedule.ExplicitSID AS AssignmentSID,
                BvSurvey.SID AS SurveySID,
                BvSurvey.[Name] AS SurveyName,
                0 AS IsSurveyGroup,
                COUNT(*) AS Counts,
                BvSvySchedule.ExplicitSID AS PersonSID,
                BvViewPersonAndGroup.[Name] AS Name,
                BvViewPersonAndGroup.IsGroup AS IsPersonGroup
            FROM BvSvySchedule WITH(NOLOCK), BvSurvey, BvViewPersonAndGroup
            WHERE --BvSvySchedule.ExplicitType = 2
               BvSvySchedule.ExplicitSID = BvViewPersonAndGroup.SID
               AND   BvSurvey.SID = BvSvySchedule.SurveySID
               AND BvSvySchedule.CallState > 0
			   AND BvSurvey.State <> 2
            GROUP BY BvSurvey.SID, BvSurvey.[Name], BvSvySchedule.CallState,
                BvSvySchedule.ExplicitSID, BvViewPersonAndGroup.[Name], 
                BvViewPersonAndGroup.IsGroup
    END

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpAppointmentUpdate]...';


GO
CREATE PROCEDURE [dbo].[BvSpAppointmentUpdate]
        @apptID         INT,
        @surveySID      INT,
        @interviewID    INT,
        @time           DATETIME,
        @expired        DATETIME,
        @contact        NVARCHAR( 255 ),
        @state          INT,
        @TZID           INT
AS
    SET NOCOUNT ON

    IF @apptID = 0
    BEGIN
        INSERT INTO BvAppointment
        (
            SurveySID, 
            InterviewSID, 
            Time, 
            ExpTime,
            State, 
            ContactName,
            TZID
        )
        VALUES
        (
            @surveySID, 
            @interviewID, 
            @time, 
            @expired,
            0, 
            @contact,
            @TZID
        )
        SET @apptID = @@IDENTITY
    END
    ELSE
    BEGIN
        UPDATE BvAppointment SET
            SurveySID = @surveySID,
            InterviewSID = @interviewID,
            Time = @time, 
            ExpTime = @expired,
            ContactName = @contact,
            State = @state,
            TZID = @TZID
        WHERE [ID] = @apptID
    END
    SELECT @apptID
    RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpAppointmentGet2]...';


GO
CREATE Procedure [dbo].[BvSpAppointmentGet2]
    @SurveySID int,
    @InterviewID int
AS
    SELECT 
        SurveySID,
        InterviewSID,
        Appt.Time,
        ExpTime,
        RespondentName,
        ID,
        State,
        ContactName
    FROM BvAppointment Appt
    WHERE 
        SurveySID = @SurveySID AND 
        InterviewSID = @InterviewID AND 
        State = 0
    return (0)
GO
PRINT N'Creating [dbo].[BvSpAlert_RecalculateAppointment]...';


GO
CREATE PROCEDURE BvSpAlert_RecalculateAppointment
	@AppointmentAlert_ShortInterval INT,
	@AppointmentAlert_LongInterval INT,
	@defaultTimeZone INT
AS
   DECLARE @Now DATETIME
   SET @Now = GETUTCDATE()

   DECLARE @Red INT
   DECLARE @Amber INT 
   DECLARE @StartDate DATETIME

   SELECT @Red = Red, @Amber = Amber
   FROM BvThresholds
   WHERE ObjectSID = 0 AND
         ThresholdsTypeID = 15
         
   
   SET @StartDate = DATEADD(millisecond, -DATEPART(millisecond, @Now), @Now)
   SET @StartDate = DATEADD(second, -DATEPART(second, @StartDate), @StartDate)
   SET @StartDate = DATEADD(minute, -DATEPART(minute, @StartDate), @StartDate)
   SET @StartDate = DATEADD(Hour, -DATEPART(hour, @StartDate), @StartDate)

   ----------------------BvAppointmentCounters----------------------
   UPDATE BvAppointmentCounters
   SET CountForShortInterval = (SELECT COUNT(*)
                                FROM BvAppointment a
                                WHERE a.State = 1 AND/*with call*/
                                      a.SurveySID = BvAppointmentCounters.SurveySID AND
                                      a.Time >= @Now AND
                                      a.Time <= DateAdd(second, @AppointmentAlert_ShortInterval, @Now)),
       CountForLongInterval = (SELECT COUNT(*)
                               FROM BvAppointment a
                               WHERE a.State = 1 AND/*with call*/
                                     a.SurveySID = BvAppointmentCounters.SurveySID AND
                                     a.Time >= (CASE WHEN @AppointmentAlert_LongInterval >= 0
                                                THEN @Now
                                                ELSE @StartDate
                                                END) AND
                                     a.Time <= (CASE WHEN @AppointmentAlert_LongInterval >= 0
                                                THEN DateAdd(hour, @AppointmentAlert_LongInterval, @Now)
                                                ELSE DateAdd(day, -@AppointmentAlert_LongInterval, @StartDate)
                                                END))
   ----------------------BvAppointmentsAlertStatus----------------------
   TRUNCATE TABLE BvAppointmentsAlertStatus
  
   INSERT INTO BvAppointmentsAlertStatus( 
     [ID],
     [SurveySID],
     [SurveyName],
     [ProjectID],
     [InterviewID],
     [AppointmentTime],
     [TZID],
     [Resource],
     [Contact],
     [AlertStatus],
     [CallID])
   SELECT a.ID,
          a.SurveySID,
          s.Description,
          s.Name,
          a.InterviewSID,
          a.Time,
          ISNULL(a.TZID, @defaultTimeZone),
          pag.Name,
          a.ContactName,
          (case WHEN ((dbo.udf_AlertStatus_DATETIME(a.Time, @Now, @Amber, @Red) = 1) AND (@NOW >= a.Time))
          THEN 0
          ELSE dbo.udf_AlertStatus_DATETIME(a.Time, @Now, @Amber, @Red)
          END),
         ss.ID
   FROM BvAppointment a
   INNER JOIN BvSurvey s ON (s.SID = a.SurveySID and s.State = 1)
   INNER JOIN BvSvySchedule ss ON(a.SurveySID = ss.SurveySID AND
                                  a.InterviewSID = ss.InterviewID)
   LEFT JOIN BvViewPersonAndGroup pag ON(ss.ExplicitType = 2 AND
                                         pag.SID = ss.ExplicitSID)
   WHERE a.State = 1 AND/*with call*/
         ((a.Time >= DateAdd(second, @Amber, @Now)) OR (a.Time < @Now))
GO
PRINT N'Creating [dbo].[BvSpAlert_RecalculateAll]...';


GO
CREATE PROCEDURE [dbo].[BvSpAlert_RecalculateAll]
   @Now DATETIME
AS 

    CREATE TABLE #tempTable(SurveySID int NOT NULL,
              StrikeRate int NOT NULL DEFAULT(0),
              CountCalls int NOT NULL DEFAULT(0),
              AvgDuration float NOT NULL DEFAULT(0))


    DECLARE @needTime DATETIME;
    SET @needTime = DATEADD(minute, -15, @Now);


    INSERT INTO #tempTable
    SELECT BvSurvey.SID, 
            4*ISNULL(sum(case when h.ITS = 13 then 1 else 0 end), 0), 4*count(h.SurveyId), ISNULL(avg(Duration), 0)
    FROM BvSurvey 
    left join BvHistory h on h.FiredTime >= @needTime AND
                          h.SurveyId = BvSurvey.SID AND
                          h.RoleID = 2
	WHERE State <> 2
    group by SID


    --2. InterviewersLoggedCount thresholds
    DECLARE @AmberOfInterviewersLoggedCount INT
    DECLARE @RedOfInterviewersLoggedCount INT
    SELECT @AmberOfInterviewersLoggedCount = Amber, @RedOfInterviewersLoggedCount = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 2/*SurveyActivityView.InterviewersLoggedCount alert*/


    --3. NextAppointmentTime thresholds
    DECLARE @AmberOfNextAppointmentTime INT
    DECLARE @RedOfNextAppointmentTime INT
    SELECT @AmberOfNextAppointmentTime = Amber, @RedOfNextAppointmentTime = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 3/*SurveyActivityView.NextAppointmentTime alert*/


    --4. NextAppointmentTime thresholds
    DECLARE @AmberOfTotalSampleSize INT
    DECLARE @RedOfTotalSampleSize INT
    SELECT @AmberOfTotalSampleSize = Amber, @RedOfTotalSampleSize = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 4/*SurveyActivityView.TotalSampleSize alert*/


    --5. ActiveCallsCount thresholds
    DECLARE @AmberOfActiveCallsCount INT
    DECLARE @RedOfActiveCallsCount INT
    SELECT @AmberOfActiveCallsCount = Amber, @RedOfActiveCallsCount = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 5/*SurveyActivityView.ActiveCallsCount alert*/


    --6. Scheduled thresholds
    DECLARE @AmberOfScheduledCallsCount INT
    DECLARE @RedOfScheduledCallsCount INT
    SELECT @AmberOfScheduledCallsCount = Amber, @RedOfScheduledCallsCount = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 6/*SurveyActivityView.ScheduledCallsCount alert*/


    --7. SuspendedCallsCount thresholds
    DECLARE @AmberOfSuspendedCallsCount INT
    DECLARE @RedOfSuspendedCallsCount INT
    SELECT @AmberOfSuspendedCallsCount = Amber, @RedOfSuspendedCallsCount = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 7/*SurveyActivityView.SuspendedCallsCount alert*/


    --8. MinutesSpentWorkingOnSurvey thresholds
    DECLARE @AmberOfMinutesSpentWorkingOnSurvey INT
    DECLARE @RedOfMinutesSpentWorkingOnSurvey INT
    SELECT @AmberOfMinutesSpentWorkingOnSurvey = Amber, @RedOfMinutesSpentWorkingOnSurvey = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 8/*SurveyActivityView.SuspendedCallsCount alert*/


    --9. AssignedInterviewersCount thresholds
    DECLARE @AmberOfAssignedInterviewersCount INT
    DECLARE @RedOfAssignedInterviewersCount INT
    SELECT @AmberOfAssignedInterviewersCount = Amber, @RedOfAssignedInterviewersCount = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 9/*SurveyActivityView.AssignedInterviewersCount alert*/


    --10. StrikeRate thresholds
    DECLARE @AmberOfStrikeRate INT
    DECLARE @RedOfStrikeRate INT
    SELECT @AmberOfStrikeRate = Amber, @RedOfStrikeRate = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 10/*SurveyActivityView.StrikeRate alert*/


    --11. CountCalls thresholds
    DECLARE @AmberOfCountCalls INT
    DECLARE @RedOfCountCalls INT
    SELECT @AmberOfCountCalls = Amber, @RedOfCountCalls = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 11/*SurveyActivityView.CountCalls alert*/
    
    SET @Now = DATEADD(millisecond, -DATEPART(millisecond, @Now), @Now)

    UPDATE BvAggregateSurveyAlertStatus
        SET BvAggregateSurveyAlertStatus.InterviewersLoggedCount = ISNULL(logs.cnt, 0),
            BvAggregateSurveyAlertStatus.InterviewersLoggedCountPrev = BvAggregateSurveyAlertStatus.InterviewersLoggedCount,
            BvAggregateSurveyAlertStatus.NextAppointmentTime = Appointment.minTime,
            BvAggregateSurveyAlertStatus.TotalSampleSize = BvSampleStatusSummary.Cnt,
            BvAggregateSurveyAlertStatus.ActiveCallsCount = ISNULL(CachedCalls.cnt, 0),
            BvAggregateSurveyAlertStatus.ActiveCallsCountPrev = BvAggregateSurveyAlertStatus.ActiveCallsCount,
            BvAggregateSurveyAlertStatus.ScheduledCallsCount = BvAggregateSurvey.ScheduledCallsCount-ISNULL(CachedCalls.cnt, 0),
            BvAggregateSurveyAlertStatus.ScheduledCallsCountPrev = BvAggregateSurveyAlertStatus.ScheduledCallsCount,
            BvAggregateSurveyAlertStatus.SuspendedCallsCount = BvAggregateSurvey.SuspendedCallsCount-BvAggregateSurvey.ScheduledCallsCount,
            BvAggregateSurveyAlertStatus.SuspendedCallsCountPrev = BvAggregateSurveyAlertStatus.SuspendedCallsCount,
            BvAggregateSurveyAlertStatus.MinutesSpentWorkingOnSurvey = BvAggregateSurvey.MinutesSpentWorkingOnSurvey,
            BvAggregateSurveyAlertStatus.AssignedInterviewersCount = ISNULL(AssignedInterviewers.cnt, 0),
            BvAggregateSurveyAlertStatus.StrikeRate = tt.StrikeRate,
            BvAggregateSurveyAlertStatus.CountCalls = tt.CountCalls,
            BvAggregateSurveyAlertStatus.AvgDuration = tt.AvgDuration,
            
            AlertStatusOfInterviewersLoggedCount = ilg.val,
            AlertStatusOfNextAppointmentTime = nat.val,
            AlertStatusOfTotalSampleSize = tss.val,
            AlertStatusOfActiveCallsCount = acc.val,
            AlertStatusOfScheduledCallsCount = scc.val,
            AlertStatusOfSuspendedCallsCount = succ.val,
            AlertStatusOfMinutesSpentWorkingOnSurvey = mswos.val,
            AlertStatusOfAssignedInterviewersCount = aic.val,
            AlertStatusOfStrikeRate = sr.val,
            AlertStatusOfCountCalls = cc.val,
            MaxStatusOfITSAlerts = ( SELECT MAX( AlertStatus ) FROM dbo.BvSampleStatusSummary WHERE SurveySID = BvAggregateSurveyAlertStatus.SID )
        FROM BvAggregateSurveyAlertStatus
        
        INNER JOIN BvSampleStatusSummary ON ( BvSampleStatusSummary.SurveySID = BvAggregateSurveyAlertStatus.SID AND
                                              BvSampleStatusSummary.ITS = 16)
                                              
        INNER JOIN #tempTable tt ON tt.SurveySID = BvAggregateSurveyAlertStatus.SID 
            
        INNER JOIN BvAggregateSurvey WITH(ROWLOCK) 
            ON (tt.SurveySID=BvAggregateSurvey.SID)
            
        LEFT JOIN (SELECT SurveySID, COUNT(*) as cnt
                   FROM BvTasks
                   WHERE SurveySID > 0
                   GROUP BY SurveySID) logs ON (tt.SurveySID = logs.SurveySID)
                   
        LEFT JOIN (SELECT COUNT(*) cnt, BvPersonrel.ObjectSid SurveySID
				   FROM BvPersonrel WHERE BvPersonrel.Type = 2
				   GROUP BY BvPersonrel.ObjectSid) AS AssignedInterviewers ON AssignedInterviewers.SurveySID = BvAggregateSurveyAlertStatus.SID
				   
	    LEFT JOIN (SELECT COUNT(*) cnt, SurveySid
                   FROM BvCachedCalls
                   WHERE  CallState = 2 OR CallState = -2
                   GROUP BY  SurveySid) AS CachedCalls ON CachedCalls.SurveySid = BvAggregateSurveyAlertStatus.SID
                   
        LEFT JOIN (SELECT MIN([Time]) minTime, SurveySID
                   FROM BvAppointment
                   WHERE State = 1
                   GROUP BY SurveySID) Appointment ON Appointment.SurveySID = BvAggregateSurveyAlertStatus.SID
                   
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT(ISNULL(logs.cnt, 0), @AmberOfInterviewersLoggedCount, @RedOfInterviewersLoggedCount ) as ilg
        CROSS APPLY dbo.udf_AlertStatus_TAB_DATETIME(
          DATEADD(millisecond, 
                  -DATEPART(millisecond, Appointment.minTime),
                  Appointment.minTime), 
          @Now, 
          @AmberOfNextAppointmentTime, 
          @RedOfNextAppointmentTime ) as nat
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT(BvSampleStatusSummary.Cnt, @AmberOfTotalSampleSize, @RedOfTotalSampleSize ) as tss
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( CachedCalls.Cnt, @AmberOfActiveCallsCount, @RedOfActiveCallsCount ) as acc
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( BvAggregateSurvey.ScheduledCallsCount-ISNULL(CachedCalls.Cnt, 0), @AmberOfScheduledCallsCount, @RedOfScheduledCallsCount ) as scc
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( BvAggregateSurvey.SuspendedCallsCount-BvAggregateSurvey.ScheduledCallsCount, @AmberOfSuspendedCallsCount, @RedOfSuspendedCallsCount ) as succ
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( BvAggregateSurvey.MinutesSpentWorkingOnSurvey, @AmberOfMinutesSpentWorkingOnSurvey, @RedOfMinutesSpentWorkingOnSurvey ) as mswos
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( AssignedInterviewers.cnt, @AmberOfAssignedInterviewersCount, @RedOfAssignedInterviewersCount ) as aic
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( tt.StrikeRate, @AmberOfStrikeRate, @RedOfStrikeRate ) as sr
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( tt.CountCalls, @AmberOfCountCalls, @RedOfCountCalls ) as cc
        
        /*-----------------Task list alert-----------------*/
        
        DECLARE @AmberOfLastSubmission INT
  DECLARE @RedOfLastSubmission INT
  DECLARE @AmberOfLastKeepAliveTime INT
  DECLARE @RedOfLastKeepAliveTime INT
  
  SELECT @AmberOfLastSubmission = Amber, @RedOfLastSubmission = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 1/*Task alert*/
        
  SELECT @AmberOfLastKeepAliveTime = Amber, @RedOfLastKeepAliveTime = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 16/*Last keep alive alert*/

		BEGIN TRY
			SET LOCK_TIMEOUT 3000

			UPDATE BvTasks
			SET SecondsSinceLastSubmission = (CASE WHEN InterviewID > 0 
											  THEN ISNULL(DATEDIFF(second, TimeStateChanged, GETUTCDATE()), 0)
											  ELSE 0
											  END),
				LastSubmissionAlert = (CASE WHEN InterviewID > 0 
									   THEN tsc.val
									   ELSE 0
									   END),
				LastKeepAliveTimeAlert = (CASE WHEN LastKeepAliveTime IS NULL 
										  THEN 2 
										  ELSE lkat.val
										  END)
		   FROM BvTasks
		   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, TimeStateChanged, GETUTCDATE()), @AmberOfLastSubmission, @RedOfLastSubmission ) as tsc
		   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, LastKeepAliveTime, GETUTCDATE()), @AmberOfLastKeepAliveTime, @RedOfLastKeepAliveTime) as lkat

		END TRY
		BEGIN CATCH
		END CATCH;
		SET LOCK_TIMEOUT -1
	

       SET @Now = GETUTCDATE()
       UPDATE BvSurveyListAlertsViewConfiguration
       SET SyncLastCall = @Now
       WHERE ((DateAdd(second, SyncUpdatingTime, SyncLastCall) <= @Now) OR
             (SyncLastCall IS NULL)) OR
             IdlePeriodCheckCounter != 0
  
       IF(@@ROWCOUNT = 1)
       BEGIN
          EXEC BvSpSynchronizeAggregateData
       END

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpAddUniqueAssignment]...';


GO
create procedure dbo.BvSpAddUniqueAssignment
@sid int
as
    insert into BvUniqueAssignments
        select a.sid
        from ( select @sid as sid ) a
        left join BvUniqueAssignments a2 on a2.sid = a.sid
        where a2.sid is null
return (0)
GO
PRINT N'Creating [dbo].[BvSpGetCallAttemptsReport_ListPage]...';


GO
CREATE PROCEDURE BvSpGetCallAttemptsReport_ListPage 
	@SupervisorName NVARCHAR(255),
	@PageNumber INT, 
	@PageSize INT, 
	@OrderField NVARCHAR (64), 
	@IsOrderASC INT,
	@SearchCondition NVARCHAR (4000) = NULL
AS
BEGIN
	IF @SupervisorName IS NULL AND @PageNumber IS NULL AND @PageSize IS NULL
	BEGIN
	/* Looks like we're generating code using FMTONLY. So lets return metadata*/
		SELECT
		0 as [ID],
		GETDATE() as [EventDate],
		0 as [SurveySID],
		'' as [ProjectID],
		'' as [ProjectName],
		'' as [InterviewerName],
		0 as [InterviewID],
		0 as [CallDuration],
		CAST( 0 as TINYINT) as [ExtendedStatus],
		'' as [ExtendedStatusName],
		'' as [TelephoneNumber]
     
		RETURN 0;
	END
 
	DECLARE @StateGroupID INT
	SELECT @StateGroupID = ID FROM [BvStateGroup] WHERE [Order] = (SELECT MIN([Order]) FROM [BvStateGroup])
	
	DECLARE @Query NVARCHAR(MAX) = 'SELECT
		hist.[ID] as [ID],
		hist.[FiredTime] as [EventDate],
		survey.[SID] as [SurveySID],
		survey.[Name] as [ProjectID],
		survey.[Description] as [ProjectName],
		person.[Name] as [InterviewerName],
		hist.[InterviewId] as [InterviewID],
		hist.[Duration] as [CallDuration],
		hist.[ITS] as [ExtendedStatus],
		states.[Name] as [ExtendedStatusName],
		hist.[TelephoneNumber] as [TelephoneNumber]
		FROM
		[BvHistory] hist INNER JOIN [BvSurvey] survey ON hist.SurveyId = survey.[SID]
		INNER JOIN [BvUserSurveyPermission] perm ON (perm.SurveySID = survey.[SID] AND perm.UserName = ''' + @SupervisorName + ''')
		INNER JOIN [BvPerson] person ON person.[SID] = hist.[PersonSID] 
		INNER JOIN [BvState] states ON states.StateID = hist.[ITS] AND states.StateGroupID = ' + CAST(@StateGroupID AS NVARCHAR(20)) +
		' WHERE hist.[RoleID] = 2 AND hist.InterviewId IS NOT NULL AND survey.State <> 2'

	DECLARE @TotalCount INT
	exec @TotalCount = BvSpGetListPage @PageNumber, @PageSize, @OrderField, @IsOrderASC, @Query, 'ID', @SearchCondition
	RETURN @TotalCount
END
GO
PRINT N'Creating [dbo].[BvSpSchedule_ListPage]...';


GO
CREATE PROCEDURE [dbo].[BvSpSchedule_ListPage]
    @PageNumber INT, 
    @PageSize INT, 
    @OrderField NVARCHAR (64), 
    @IsOrderASC INT,  
    @SearchCondition NVARCHAR (4000)=NULL
AS
SET NOCOUNT ON

IF @PageNumber IS NULL AND @PageSize IS NULL
BEGIN
/* Looks like we're generating code using FMTONLY. So lets return metadata*/
 SELECT  
        0 AS [SID],
        '' AS [Name],
        getdate() AS [CreateDate],
        getdate() AS [ModifyDate],
        0 AS [State],
        0 AS [DesignStateGroupID],
        '' AS [DesignStateGroupName]
END

DECLARE @Query AS NVARCHAR(4000)
DECLARE @IDField AS NVARCHAR(64)
DECLARE @DefaultStateGroupID AS INT

SET @IDField = 'SID';
SELECT @DefaultStateGroupID = MIN(ID) FROM [BvStateGroup] 

SET @Query =
    'SELECT  
        ScheduleID      AS SID,
        sch.Name            AS Name,
        CreateDate      AS CreateDate,
        ModifyDate      AS ModifyDate,
        CASE WHEN LEN( XmlInUse ) = 0 THEN 0 --Not launched
			 WHEN XmlInUse <> XmlUnderDev THEN 1 -- Pending synchronized
			 ELSE 2 -- Synchronized
		END as State,
		sch.DesignStateGroupID,
		gr.Name as DesignStateGroupName
    FROM BvSchedule sch inner join BvStateGroup as gr on gr.ID = isnull(sch.DesignStateGroupID, ' + CONVERT(NVARCHAR, @DefaultStateGroupID) + ')'

DECLARE @TotalCount INT
exec @TotalCount = BvSpGetListPage @PageNumber, @PageSize, @OrderField, @IsOrderASC, @Query, @IDField, @SearchCondition
RETURN @TotalCount
GO
PRINT N'Creating [dbo].[BvSpSvySch_Insert]...';


GO
CREATE  PROCEDURE [dbo].[BvSpSvySch_Insert]
        @ID                 int,
        @ApptID             int,
        @SurveySID          int,
        @InterviewID        int,
        @CallState          int,
        /* 
         * @ShiftTypeID > 0 means specific shift type id( BvShiftType.ID ) and should be resolved to ShiftZoneId in bvSvySchedule.ShiftTypeID
         * @ShiftTypeID = Int32.MinValue(-2147483648) meens [None] and should ne resolved to Int32.MinValue in BvSvySchedule.ShiftTypeID
         * @ShiftTypeID =-1 @ShiftTypeID means [Any valid] and should be resolved to -Timezone in BvSvySchedule.ShiftTypeID
         */
        @ShiftTypeID        int,
        @Priority           int,
        @TimeInShift        datetime,
        @ExpireTime         datetime,
        @Resource           int,
        @RuleNumber         uniqueidentifier,
        @DefaultTimeZoneID  INT,
        @ConditionValue     INT
AS
SET NOCOUNT ON
DECLARE @Rows INTEGER
DECLARE @ExplicitSID INTEGER
DECLARE @ExplicitType INTEGER
DECLARE @CallTZ INT

DECLARE @sqlQueryParams NVARCHAR(MAX)
DECLARE @sqlQuery NVARCHAR(MAX)
DECLARE @whereCondition NVARCHAR(MAX)
DECLARE @ROWCOUNT INT = 0
DECLARE @alias NVARCHAR(25) = 'repl'
DECLARE @ShiftTypeNone INT = -2147483648; --None constant for bvSvySchedule.ShiftTypeID

    SET @InterviewID = ABS( @InterviewID )

    -- Get call TZ
    SELECT @CallTZ = TimezoneID 
    FROM BvInterview
    WHERE SurveySID = @SurveySID AND 
         [ID] = @InterviewID
         
    SET @CallTZ = ISNULL( @CallTZ, 0 )

    IF  @ShiftTypeID <> @ShiftTypeNone --Not [None]
    BEGIN
        DECLARE @ret INT       
        DECLARE @actualShiftTypeID INT
 
		/*
			@ShiftTypeID can contain negative timezone value
			but BvSpCheckCallOnShifts does not understand such values -
			in this case it should think that @ShiftTypeID = -1 [Any Valid] 
		*/
        IF @ShiftTypeID > 0
			SET @actualShiftTypeID = @ShiftTypeID
		ELSE
			SET @actualShiftTypeID = -1
        
        /*
			Note: we remove "Checking call time to be Out of Shifts", because if time in shift isn't hit to 
			shift of specific shift type, call will be delivered bit late.
			But we should call BvSpCheckCallOnShifts without TimeInShift, because we should check that 
			specific shifttype have somoething available shifts for specific timezone.
		*/
        EXEC @ret = BvSpCheckCallOnShifts @CallTZ, @actualShiftTypeID, NULL/*@TimeInShift*/, @SurveySID, @DefaultTimeZoneID
        IF @ret <> 0
            RETURN @ret
    END

    IF @Resource = 0
    BEGIN
        SET @ExplicitSID = @SurveySID

        SET @ExplicitType = 1
        IF @ExplicitSID IS NULL
        BEGIN
            RAISERROR( 'Could not find assignment group, %i', 16, 1, @ExplicitSID )
            RETURN -50002
        END
    END
    ELSE
    BEGIN
        SET @ExplicitSID = @Resource
        SET @ExplicitType = 2
    END

    IF @ShiftTypeID > 0--meens specific shift type id
    BEGIN
        SELECT @ShiftTypeID = [ID]
            FROM BvShiftZones WHERE ShiftTypeID = @ShiftTypeID
                AND TimeZoneID = @CallTZ
    END
    ELSE IF @ShiftTypeID <> @ShiftTypeNone -- means [Any valid]
    BEGIN
		SET @ShiftTypeID = -@CallTZ
    END
    --ELSE/*@ShiftTypeID = @ShiftTypeNone*/ -- means [None]
    --BEGIN
	--	SET @ShiftTypeID = @ShiftTypeNone
    --END

    DECLARE @ExpirationTime DATETIME = @ExpireTime
    DECLARE @TimeInShift1 DATETIME = @TimeInShift
    
    IF @ExpireTime IS NULL
        SET @ExpirationTime = '9999-01-01 00:00:00.000'
    
    IF @TimeInShift IS NULL
        SET @TimeInShift1 = '1899-12-30 00:00:00.000'

    DECLARE @oldApptID INT = NULL
    
    DECLARE @IsRandomCallDeliveryEnabled BIT
	SELECT @IsRandomCallDeliveryEnabled = IsRandomCallDeliveryEnabled
	FROM BvSurvey
	WHERE SID = @SurveySID
      
    EXEC BvClr_QuotaService_GetWhereForAllClosedSurveyCells @SurveySID, @alias, @whereCondition OUTPUT

    SET @sqlQuery = 
      N'SET @refID = 0
        MERGE BvSvySchedule as target
        USING( SELECT @SurveySID, 
                      @InterviewId, 
                      @ApptID, 
                      (SELECT COUNT(*)
                       FROM BvReplicatedData_' + CAST(@SurveySID AS NVARCHAR(255)) + ' as repl
                       WHERE respid = @InterviewID AND
                             (' + @whereCondition + '))) AS source (SurveySid, InterviewId, Appt, IsClosed)
        ON target.SurveySID = source.SurveySID AND
           target.InterviewID = source.InterviewID
        WHEN MATCHED
        THEN
			  UPDATE
			  SET @refoldApptID     = ApptID,
			      @refID            = CASE WHEN Appt > 0 OR IsClosed = 0 THEN ID ELSE 0 END,
				  ApptID            = @ApptID,
				  CallState         = CASE WHEN Appt > 0 OR IsClosed = 0 THEN @CallState ELSE 0 END,
				  Priority          = @Priority,
				  TimeInShift       = @TimeInShift1,
				  ExpireTime        = @ExpirationTime,
				  ShiftTypeID       = @ShiftTypeID,
				  ExplicitSID       = @ExplicitSID,
				  ExplicitType      = @ExplicitType,
				  RuleNumber        = @RuleNumber,
                  ConditionValue    = @ConditionValue,
				  OldPriority       = 0
        WHEN NOT MATCHED AND ( Appt > 0 OR IsClosed = 0 )
        THEN
              INSERT(ShiftTypeID,
                     ApptID,
                     InterviewID,
                     SurveySID,
                     CallState,
                     Priority,
                     TimeInShift,
                     ExpireTime,
                     ExplicitSID,
                     ExplicitType,
                     RuleNumber,
                     CallOrder,
					 ConditionValue )
              VALUES(@ShiftTypeID,
                     @ApptID,
                     @InterviewID,
                     @SurveySID,
                     @CallState,
                     @Priority,
                     @TimeInShift1,
                     @ExpirationTime,
                     @ExplicitSID,
                     @ExplicitType,
                     @RuleNumber,
                     CASE WHEN @IsRandomCallDeliveryEnabled = 0 THEN InterviewId
						  ELSE dbo.GetRandomValue(@InterviewID)
					 END,
					 @ConditionValue);
         
        IF( (@@ROWCOUNT > 0) AND (@refoldApptID IS NULL))
           SET @refID = @@IDENTITY'
        
SET @sqlQueryParams = N'@SurveySID INT, @IsRandomCallDeliveryEnabled BIT, @CallState INT, @ApptID INT, @Priority INT, ' +
       '@TimeInShift1 DATETIME, @ExpirationTime DATETIME, @ShiftTypeID INT, @ExplicitSID INT, ' +
       '@ExplicitType INT, @RuleNumber uniqueidentifier, @InterviewID INT, @ConditionValue INT, ' +
       '@refID INT OUTPUT, @refoldApptID INT OUTPUT'
       
    EXEC sp_executesql @sqlQuery, @sqlQueryParams, @SurveySID, @IsRandomCallDeliveryEnabled, @CallState, @ApptID, @Priority,
       @TimeInShift1, @ExpirationTime, @ShiftTypeID, @ExplicitSID, @ExplicitType, @RuleNumber,
       @InterviewID, @ConditionValue, @refID = @ID OUTPUT, @refoldApptID = @oldApptID OUTPUT

    IF @ID > 0 --call was updated or inserted
    BEGIN         
		IF @oldApptID > 0
		  UPDATE BvAppointment
		  SET State = 2
		  WHERE ID = @oldApptID

		  -- insert call id in touch table
		INSERT INTO BvCachedCallsInsert 
		values(@InterviewID, @SurveySID)
		
	    IF @ApptID > 0
		  UPDATE BvAppointment SET State = 1 WHERE ID = @ApptID 

	  exec BvSpAddUniqueAssignment @ExplicitSID
	END
  
RETURN (@ID)
GO
PRINT N'Creating [dbo].[BvSpSurvey_Update]...';


GO
CREATE PROCEDURE [dbo].[BvSpSurvey_Update]
        @SID            int,
        @Name           nvarchar( 255 ),
        @Description        nvarchar( 255 ),
        @QuotaType      tinyint,
		@DialMode tinyint,
        @forceOpnRev int,
        @StateGroupID int,
        @RecWholeInt int,
		@InterviewScreenRecording bit,
        @BvID bigint,
		@DestinationTableName NVARCHAR (255), 
		@ReplicationStatus BIT,
		@ScheduleID INT,
		@DialerParameters NVARCHAR(MAX),
		@IsTelephoneBlacklistSupported BIT,
		@NotificationEmail NVARCHAR(MAX),
		@EnforceHttps BIT,
		@LastTouchTime SMALLDATETIME,
		@SurveySchedulingMode SMALLINT
AS
SET NOCOUNT ON

EXEC   BvSpSurveyModifyStateGroup @SID, @StateGroupID

IF ISNULL( @BvID, 0 ) > 0
BEGIN
    IF EXISTS( 
        SELECT 1 FROM BvNumber 
        WHERE BvID = @BvID AND ClassID = 2 AND ObjectSID != @SID
    )
    BEGIN
        RAISERROR( 'BvID = %I64d already exists', 16, 1, @BvID )
        RETURN -1
    END
END

DECLARE @OldSurveyDescription NVARCHAR( 255 )
DECLARE @OldScheduleID INT
DECLARE @OldSurveySchedulingMode INT

UPDATE  BvSurvey
    SET [Name]               = @Name,     
        @OldSurveyDescription = [Description],
        [Description]        = @Description,       
        QuotaType            = @QuotaType,
		DialMode             = @DialMode,         
        ForceOpnRev          = @forceOpnRev,
        StateGroupID         = @StateGroupID,
        RecWholeInt          = @RecWholeInt,
		InterviewScreenRecording = @InterviewScreenRecording,
        DestinationTableName = @DestinationTableName,
        ReplicationStatus    = @ReplicationStatus,
        ScheduleID           = @ScheduleID,
        @OldScheduleID       = ScheduleID,
        DialerParameters	 = @DialerParameters,
        IsTelephoneBlacklistSupported = @IsTelephoneBlacklistSupported,
        NotificationEmail	=	@NotificationEmail,
		[EnforceHttps]       = @EnforceHttps,
        [LastTouchTime]      = @LastTouchTime,
		@OldSurveySchedulingMode = [SurveySchedulingMode],
        [SurveySchedulingMode] = @SurveySchedulingMode
    WHERE SID = @SID

-- SL. Should we use such optimization here? It works incorrectly with NULLs. BvSurvey allows NULL for the Description field.
IF (@OldSurveyDescription != @Description) 
BEGIN
   UPDATE BvAggregateSurveyAlertStatus
   SET Description = @Description
   WHERE SID = @SID
   
   UPDATE BvAppointmentsAlertStatus
   SET SurveyName = @Description
   WHERE SurveySID = @SID
   
   UPDATE BvAppointmentCounters
   SET SurveyName = @Description
   WHERE SurveySID = @SID
END

IF ISNULL( @BvID, 0 ) > 0
BEGIN
    IF EXISTS ( SELECT 1 FROM BvNumber WHERE ObjectSID = @SID )
        UPDATE BvNumber SET BvID = @BvID 
        WHERE ObjectSID = @SID AND ClassID = 2
    ELSE
        INSERT INTO BvNumber ( ObjectSID, ClassID, BvID )
            VALUES ( @SID, 2, @BvID )
END
ELSE
    EXEC BvSpBvID_Delete 2, @SID

EXEC    BvSpMembership_Delete 0, @SID


IF @OldScheduleID <> @ScheduleID
BEGIN
    /*
     * change scheduling parameters
     */
    --delete specific survey schedule params
    DELETE FROM BvScheduleParam WHERE SurveySID = @SID
    -- Add default schedule param of current scheduling script to BvScheduleParam table
    INSERT INTO BvScheduleParam( ScheduleID, SurveySID, ParamID, [Name], Description, Type, Value ) 
        SELECT sp.ScheduleID, @SID, sp.ParamID, sp.[Name], sp.Description, sp.Type, sp.Value
            FROM BvScheduleParam sp 
                WHERE sp.SurveySID = 0 AND sp.ScheduleID = @ScheduleID
END

IF @OldSurveySchedulingMode <> @SurveySchedulingMode
BEGIN
	IF @SurveySchedulingMode = 0 
	BEGIN
		UPDATE BvSvySchedule SET ConditionValue = 0 WHERE SurveySID = @SID
	END
	ELSE
	BEGIN
		UPDATE BvSvySchedule 
			SET ConditionValue = TransientState
		FROM BvInterview 
			WHERE BvSvySchedule.SurveySID = @SID AND BvInterview.SurveySID = @SID AND BvSvySchedule.InterviewID = BvInterview.ID
	END
END

return 0
GO
PRINT N'Creating [dbo].[BvSpSurvey_ListPage]...';


GO
CREATE PROCEDURE [dbo].[BvSpSurvey_ListPage]
@PageNumber INT, 
@PageSize INT, 
@OrderField NVARCHAR (64), 
@IsOrderASC INT, 
@userName NVARCHAR (255), 
@userID INT=0, 
@accessMask INT=2147483647, 
@SearchCondition NVARCHAR (4000)=NULL
AS
SET NOCOUNT ON

 IF @PageNumber IS NULL AND @PageSize IS NULL
 BEGIN
 /* Looks like we're generating code using FMTONLY. So lets return metadata*/

 SELECT  
        0 AS SID,
        '' AS Name, 
        0 AS SampleSize,
        0 AS State,
        '' AS Description
     
     RETURN 0;
 END

DECLARE @Query as nvarchar(4000)
DECLARE @IDField as nvarchar(64)
SET @IDField = 'SID'
SET @Query =
    'SELECT  
        BvSurvey.SID        SID,
        BvSurvey.Name       Name, 
        ISNULL(sample.Count, 0) SampleSize,
        BvSurvey.State,
        BvSurvey.Description
        FROM    BvSurvey
        LEFT JOIN BvUserSurveyPermission ON ( BvUserSurveyPermission.UserName = '''+@userName+''' AND
                                              BvUserSurveyPermission.SurveySID = BvSurvey.SID)
        LEFT JOIN (SELECT COUNT(*) as Count, SurveySID FROM BvInterview group by SurveySid ) as sample on BvSurvey.SID = sample.SurveySID 
        WHERE
                  ((BvUserSurveyPermission.UserName IS NOT NULL) OR ('''+@userName+''' = '''')) AND BvSurvey.State <> 2'

DECLARE @TotalCount INT
exec @TotalCount = BvSpGetListPage @PageNumber, @PageSize, @OrderField, @IsOrderASC, @Query, @IDField, @SearchCondition
RETURN @TotalCount
GO
PRINT N'Creating [dbo].[BvSpSurvey_Insert]...';


GO
CREATE  PROCEDURE [dbo].[BvSpSurvey_Insert]
        @SID int,
        @Name nvarchar( 255 ),
        @Description nvarchar( 255 ),
        @QuotaType tinyint,
		@DialMode tinyint,
        @State int,
        @forceOpnRev int,
        @StateGroupID int,
        @RecWholeInt int,
		@InterviewScreenRecording bit,
        @BvID bigint,
        @RouteAddress NVARCHAR(255),
        @CfDbSchemaPath NVARCHAR(255),
        @DestinationTableName NVARCHAR (255), 
		@ReplicationStatus BIT,
		@ScheduleID INT,
		@DialerParameters NVARCHAR(MAX),
		@IsTelephoneBlacklistSupported BIT,
		@NotificationEmail NVARCHAR(MAX),
		@EnforceHttps BIT,
		@SurveySchedulingMode SMALLINT
AS
SET NOCOUNT ON


IF @StateGroupID = 0
BEGIN
    DECLARE @MinOrder INTEGER
    SELECT @MinOrder = MIN([Order]) FROM BvStateGroup
    SELECT @StateGroupID = [ID] FROM BvStateGroup WHERE [Order] = @MinOrder
END


IF ISNULL( @BvID, 0 ) > 0
BEGIN
    EXEC @BvID = BvSpSetObjectNumber @SID, 2, @BvID
    IF @BvID = -1
 BEGIN
        RETURN ( 50006 )
 END
END

IF ISNULL( @ScheduleID, 0 ) = 0
BEGIN
    SELECT @ScheduleID = MIN( ScheduleID ) FROM BvSchedule
END

INSERT  BvSurvey( 
        SID, 
        [Name], 
        [Description],
        QuotaType,
		DialMode,
        State,
        ForceOpnRev,
        StateGroupID,
        RecWholeInt,
		InterviewScreenRecording,
        CfDbSchemaPath,
        DestinationTableName, 
        ReplicationStatus,
        ScheduleID,
        DialerParameters,
        IsTelephoneBlacklistSupported,
        [NotificationEmail],
		[EnforceHttps],
		SurveySchedulingMode
        )
    VALUES
    (
        @SID,
        @Name,
        @Description,
        @QuotaType,
		@DialMode,
        @State,
        @forceOpnRev,
        @StateGroupID,
        @RecWholeInt,
		@InterviewScreenRecording,
        @CfDbSchemaPath,
        @DestinationTableName, 
        @ReplicationStatus,
        @ScheduleID,
        @DialerParameters,
        @IsTelephoneBlacklistSupported,
        @NotificationEmail,
		@EnforceHttps,
		@SurveySchedulingMode	
	)
        
INSERT BvAggregateSurvey (SID) VALUES(@SID)
INSERT BvAggregateSurveyAlertStatus (SID, Name, Description) VALUES(@SID, @Name, @Description)

INSERT BvAppointmentCounters (SurveySID, SurveyName, ProjectID, CountForShortInterval, CountForLongInterval)
VALUES(@SID, @Description, @Name, 0, 0)

INSERT INTO BvSampleStatusSummary( SurveySID, ITS ) 
        SELECT @SID, StateID FROM BvState WHERE StateGroupID = @StateGroupID

-- Add default schedule param of current scheduling script to BvScheduleParam table
INSERT INTO BvScheduleParam( ScheduleID, SurveySID, ParamID, [Name], Description, Type, Value ) 
    SELECT sp.ScheduleID, @SID, sp.ParamID, sp.Name, sp.Description, sp.Type, sp.Value
                 FROM BvScheduleParam sp 
                        WHERE sp.SurveySID = 0 AND sp.ScheduleID = @ScheduleID

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpSurvey_Delete]...';


GO
CREATE PROCEDURE [dbo].[BvSpSurvey_Delete]
        @surveyID int
AS
    DECLARE @State INTEGER

	IF EXISTS( SELECT 1 FROM BvTasks WHERE SurveySID = @surveyID )
	BEGIN
		DECLARE @Name NVARCHAR(MAX) 
		SELECT @Name = name FROM BvSurvey WHERE SID = @surveyID
		RAISERROR( 'Survey with name = ''%s'' can''t be deleted, because active users exist for it survey', 16, 1, @name )
		RETURN -1
	END

    DELETE FROM BvThresholdITS WHERE SurveySID = @surveyID

    DELETE FROM BvMembership WITH(ROWLOCK)
    WHERE ObjectSID = @surveyID
    
    DELETE BvAppointment 
    WHERE SurveySID = @surveyID
    
    DELETE FROM BvCachedCalls 
    WHERE SurveySID = @surveyID
    
    DELETE FROM BvSvySchedule 
    WHERE SurveySID = @surveyID

    DELETE BvPersonOrGroupAssignmentOnSurvey WHERE SurveyId = @surveyID 

    DELETE BvInterview WHERE SurveySID = @surveyID
    
    EXEC BvSpMembership_Delete 0, @surveyID
    
    --delete specific survey schedule params
    DELETE FROM BvScheduleParam WHERE SurveySID = @surveyID

    EXEC BvSpBvID_Delete 2, @surveyID

    DELETE  BvSurvey WHERE SID = @surveyID
    DELETE FROM BvSampleStatusSummary WHERE SurveySID = @surveyID
    
    DECLARE @FilterSID INTEGER
    SELECT @FilterSID = SID FROM BvFilters WHERE [Name] = CAST( @surveyID AS NVARCHAR(255) )
    IF @FilterSID IS NOT NULL
    BEGIN
        DELETE FROM BvFilterFields WHERE FilterSID = @FilterSID
        DELETE FROM BvFilters WHERE SID = @FilterSID
    END
    
    DELETE FROM BvFilterFields
    FROM BvFilterFields
    INNER JOIN BvFilters ON ( SID = FilterSid )
    WHERE SurveySID = @surveyID

    DELETE FROM BvFilters WHERE SurveySID = @surveyID
    
    delete from bvpersonrel where type = 2 and objectsid = @surveyID
    
    delete from bvlogingroup where surveysid = @surveyID OR objectsid = @surveyID

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpState_Update]...';


GO
CREATE PROCEDURE [dbo].[BvSpState_Update]
 @ObjectID INT,
 @StateGroupID INT,
 @Name VARCHAR(255),
 @Priority INT,
 @DA BIT
AS

DECLARE @OldPriority INT

SELECT @OldPriority = Priority
 FROM BvState 
 WHERE StateID = @ObjectID AND StateGroupID = @StateGroupID

UPDATE BvState 
 SET Priority = @Priority, [Name] = @Name, DA = @DA 
 WHERE StateID = @ObjectID AND StateGroupID = @StateGroupID

IF ( @OldPriority <> @Priority )
BEGIN

 DECLARE crSurveys CURSOR LOCAL FOR SELECT [SID] FROM [BvSurvey]
  INNER JOIN [BvNumber] ON [BvSurvey].[SID] = [BvNumber].[ObjectSID]

 DECLARE @SurveySID INT
 DECLARE @SurveyProcedureName NVARCHAR(128)
 DECLARE @SurveysProcessed INT

 OPEN crSurveys
 FETCH NEXT FROM crSurveys INTO @SurveySID

 WHILE ( @@fetch_status = 0 )
 BEGIN
  SET @SurveyProcedureName = 'BvSpSurveyState_Update'

  EXEC @SurveyProcedureName @ObjectID, @StateGroupID, @Priority

  SET @SurveysProcessed = @SurveysProcessed + 1

  FETCH NEXT FROM crSurveys INTO @SurveySID
 END

 CLOSE crSurveys
 DEALLOCATE crSurveys

 IF ( @SurveysProcessed > 0 )
  EXEC BvSpCache_NotifyUpdated 
END

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpStateGroup_Delete]...';


GO
CREATE PROCEDURE [dbo].[BvSpStateGroup_Delete]
@ObjectSID INTEGER
AS
DECLARE @Deleted INTEGER
DECLARE @MinOrder INTEGER
DECLARE @Order       INTEGER
DECLARE @GroupName NVARCHAR(MAX)
DECLARE @SurveyName NVARCHAR(MAX)
 
     -- Dont delete state group if it default group
     SELECT @MinOrder   = MIN( [Order] ) FROM BvStateGroup
     SELECT @Order = [Order] FROM BvStateGroup WHERE [ID] = @ObjectSID

     IF @MinOrder = @Order
     BEGIN
         RAISERROR( 'Could not delete default state group.', 12, 1)
         RETURN -1
     END

     -- Dont delete state group if link exist
     IF EXISTS( SELECT * FROM BvSurvey WHERE StateGroupID = @ObjectSID AND State <> 2 )
     BEGIN
		SELECT @GroupName = Name FROM BvStateGroup WHERE [ID] = @ObjectSID
		SELECT TOP(1) @SurveyName = name FROM BvSurvey WHERE StateGroupID = @ObjectSID AND State <> 2
		
        RAISERROR( 'The state group "%s" can not be deleted because survey "%s" references it.', 12, 1, @GroupName, @SurveyName )
		RETURN( -1 )
     END

DECLARE @DefaultStateGroupID INTEGER
SELECT top(1) @DefaultStateGroupID = ID 
FROM BvStateGroup 
ORDER BY [Order] ASC;

	 IF EXISTS( SELECT * FROM BvSurvey WHERE StateGroupID = @ObjectSID AND State = 2 )
	 BEGIN
		UPDATE BvSurvey 
		SET StateGroupID = @DefaultStateGroupID 
		WHERE StateGroupID = @ObjectSID AND State = 2
	 END

     EXEC BvSpMembership_Delete 0, @ObjectSID
     DELETE FROM BvStateGroup WHERE [ID] = @ObjectSID
     DELETE FROM BvState WHERE StateGroupID = @ObjectSID

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpShiftType_Delete]...';


GO
CREATE PROCEDURE [dbo].[BvSpShiftType_Delete]
@OwnerSID INTEGER,
@ID       INTEGER,
@Mode     INTEGER
AS

DECLARE @Rows     INTEGER
DECLARE @Rows2    INTEGER
DECLARE @ObjectID INTEGER

SELECT  @Rows = COUNT( * ), @ObjectID = MIN( ObjectID )
    FROM    BvShiftType
    WHERE   OwnerSID = @OwnerSID
    AND ID = @ID

IF @Rows = 0
  BEGIN
    RAISERROR('Shift type with ID %i not exists', 16, 2, @ID)
    RETURN -1
  END
IF @Rows <> 1
  BEGIN
    RAISERROR('Multiple shift types with ID %i found', 16, 2, @ID)
    RETURN -1
  END

SELECT  @Rows = COUNT( * )
    FROM    BvShift
    WHERE   OwnerSID = @OwnerSID
    AND ShiftTypeID = @ObjectID

SELECT @Rows2 = COUNT( BvSvySchedule.ShiftTypeID )
    FROM BvSvySchedule, BvShiftZones
    WHERE BvShiftZones.ShiftTypeID = @ObjectID
      AND BvShiftZones.[ID] = BvSvySchedule.ShiftTypeID

IF @Rows <> 0 OR @Rows2 <> 0 BEGIN
    IF @Mode <> 2 /* BVDBS_ACTION_MODE_STRONG */
    BEGIN
        IF @Rows <> 0
          RAISERROR( 'Unable to delete shift type. Link exists on shifts', 12, 1 )
        ELSE 
          RAISERROR( 'Unable to delete shift type. Link exists on calls', 12, 1 )
        return -1
    END
    ELSE BEGIN
        DELETE  BvTimezoneShift
            WHERE   OwnerSID = @OwnerSID
            AND ShiftID IN ( SELECT  ID
                            FROM    BvShift
                            WHERE   OwnerSID = @OwnerSID
                            AND ShiftTypeID = @ObjectID )
        DELETE  BvShift
            WHERE   OwnerSID = @OwnerSID
            AND ShiftTypeID = @ObjectID
            
        DECLARE @changingTable table(ApptID INT NOT NULL)
        
        DELETE FROM BvCachedCalls
		FROM BvSvySchedule s
        WHERE s.ShiftTypeID IN ( SELECT [ID] FROM BvShiftZones WHERE ShiftTypeID = @ObjectID ) AND
              (s.CallState > 0 OR s.CallState = -2) AND
			  s.SurveySID = BvCachedCalls.SurveySID AND	
			  s.InterviewID = BvCachedCalls.InterviewID

        DELETE FROM BvSvySchedule 
        OUTPUT DELETED.ApptID
        INTO @changingTable
        WHERE ShiftTypeID IN ( SELECT [ID] FROM BvShiftZones WHERE ShiftTypeID = @ObjectID ) AND
              (CallState > 0 OR CallState = -2)
        
        UPDATE BvAppointment
        SET State = 2
        FROM @changingTable c
        WHERE c.ApptID = BvAppointment.ID
        
        if @@rowcount > 0
            exec BvSpCache_NotifyUpdated
    END
END

DELETE  BvShiftType
    WHERE   OwnerSID = @OwnerSID
    AND ID = @ID

DELETE FROM BvShiftZones WHERE ShiftTypeID = @ObjectID

RETURN 0
GO
PRINT N'Creating [dbo].[BvSpSchedule_Delete]...';


GO
CREATE PROCEDURE [dbo].[BvSpSchedule_Delete]
       @ScheduleID int 
AS
DECLARE @rows INT

DECLARE @allHourSID INT
SELECT @allHourSID = MIN( ScheduleID ) FROM BvSchedule

/* Don't allow to delete 'All hours' schedule */
IF @allHourSID = @ScheduleID
BEGIN
	RAISERROR( 'Could not delete default scheduling script.', 12, 1)
    RETURN -1
END

IF EXISTS ( SELECT SID FROM BvSurvey WHERE ScheduleID = @ScheduleID AND State <> 2 )
BEGIN
	RAISERROR( 'Could not delete scheduling script that used by survey(s)', 12, 1)
	RETURN -1
END

BEGIN TRAN

    UPDATE BvSvySchedule SET ShiftTypeID = -z.TimeZoneID
    FROM BvSvySchedule c
    INNER JOIN BvShiftZones z 
    ON c.ShiftTypeID = z.[ID] 
    INNER JOIN BvShiftType t ON t.OwnerSID = @ScheduleID
    AND z.ShiftTypeID = t.ObjectID

    SET @rows = @@ROWCOUNT
    if @rows > 0
        exec BvSpCache_NotifyUpdated

    DELETE FROM BvScheduleParam WHERE ScheduleID = @ScheduleID

    DELETE FROM BvShiftZones
        WHERE ShiftTypeID IN ( 
            SELECT ObjectID FROM BvShiftType
            WHERE OwnerSID = @ScheduleID )

    DELETE  BvShift
        WHERE OwnerSID = @ScheduleID

    DELETE  BvShiftType
        WHERE OwnerSID = @ScheduleID

    DELETE  BvTimezoneShift
        WHERE OwnerSID = @ScheduleID

    DELETE FROM BvSchedule 
        WHERE   ScheduleID = @ScheduleID

	IF EXISTS ( SELECT SID FROM BvSurvey WHERE ScheduleID = @ScheduleID AND State = 2 )
	BEGIN
		UPDATE BvSurvey 
		SET ScheduleID = @allHourSID 
		WHERE ScheduleID = @ScheduleID AND State = 2
	END

COMMIT

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpQueueUpSheduleTask3]...';


GO
CREATE PROCEDURE [dbo].[BvSpQueueUpSheduleTask3]
    @NowUTC           datetime,
    @DefaultTZ        INT
as
set nocount on

declare @rows int
declare @tb table (
        InterviewID int not null,
        SurveySID int not null,
        primary key ( InterviewID, SurveySID ) )
 
    -- temp table for determine active shifts/survey
    create table #temp
    (
        [ID] int not null,
        SurveySID int not null
    )
 
    -- calculate live shifts 
    insert into #temp exec BvSpGetLiveShifts @NowUTC, @DefaultTZ

    -- use temp buffer
    insert into @tb select InterviewID, SurveySID from BvCachedCallsInsert

    set @rows = @@rowcount

 
    -- insert into BvCachedCallsInsert calls id
    -- where shift type id is inserted, or deleted
    insert into @tb
        select c.InterviewID, c.SurveySID
        from BvSvySchedule c
        left join @tb ci on ci.InterviewID = c.InterviewID
            and ci.SurveySID = c.SurveySID
        inner join ( select isnull( t.[ID], a.[ID] ) as [ID], 
                            isnull( t.SurveySID, a.SurveyId ) as SurveySID
                     from #temp t
                     full join BvActiveShiftTypeZone a on a.Id = t.[ID] and
                         a.SurveyId = t.SurveySID
                     where a.[ID] is null or t.[ID] is null ) s on
           s.[ID] = c.ShiftTypeID
           and s.SurveySID = c.SurveySID
     where ci.SurveySID is null
 
        -- save rowcount
     set @rows = @rows + @@rowcount
 
        -- copy new shifts information
     truncate table BvActiveShiftTypeZone
        insert into BvActiveShiftTypeZone
            select [ID], SurveySID from #temp
 
        drop table #temp
 
        if @rows > 200000 begin 
            UPDATE BvSvySchedule
            SET IsInActiveShiftType = ISNULL(a.ID|1, 0)
            FROM BvSvySchedule c
            LEFT JOIN BvActiveShiftTypeZone a ON a.Id = c.ShiftTypeID AND
                                              a.SurveyId = c.SurveySID
            WHERE CallState != -3 --processed during sample loading
        end
        else begin
        UPDATE BvSvySchedule
        SET IsInActiveShiftType = ISNULL(a.ID|1, 0)
        FROM BvSvySchedule c
        INNER JOIN @tb i ON i.InterviewID = c.InterviewID AND 
                            i.SurveySID = c.SurveySID
        LEFT JOIN BvActiveShiftTypeZone a ON a.Id = c.ShiftTypeID AND 
                                         a.SurveyId = c.SurveySID
        END
        
        delete from BvCachedCallsInsert
        from @tb i
        where i.InterviewID = BvCachedCallsInsert.InterviewID and 
			  i.SurveySID = BvCachedCallsInsert.SurveySID
 
return (0)
GO
PRINT N'Creating [dbo].[BvSpPerson_SpinUp]...';


GO
CREATE  PROCEDURE [dbo].[BvSpPerson_SpinUp]
    @PersonSID INT
AS
    declare @temp table
    (
        sid int not null,
        role_id int not null,
        type int not null
    )

    insert into @temp
        select distinct m.ContainerSID, g.RoleID, 1
        from BvMemberShip m
        inner join BvPersonGroup g on g.SID = m.ContainerSID
        where m.ObjectSID = @PersonSID

    insert into @temp values ( @PersonSID, 0, 1 )

    insert into @temp
        select distinct a.SurveyId, 2, 2 from BvPersonOrGroupAssignmentOnSurvey a
        where a.PersonOrGroupId in (
            select sid from @temp )
    
    delete from BvPersonRel where PersonSID = @PersonSID
    insert into BvPersonRel( PersonSID, ObjectSID, RoleID, Type )
        select @PersonSID, sid, role_id, type from @temp
            
    EXEC BvSpLogin_SpinUp @PersonSID
RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpPerson_Delete]...';


GO
CREATE PROCEDURE [dbo].[BvSpPerson_Delete]
 @SID int
AS
    EXEC BvSpMembership_Delete 0, @SID

    DELETE FROM BvNumber WHERE ObjectSID = @SID AND ClassID = 10

    DELETE  BvPerson WHERE SID = @SID

    DELETE FROM BvPersonRel
    FROM BvPersonRel
    WHERE PersonSID = @SID

    -- delete implicit assigments
    DELETE FROM BvPersonOrGroupAssignmentOnSurvey WHERE PersonOrGroupId = @SID

    UPDATE BvSvySchedule 
    SET ExplicitSID = BvSvySchedule.SurveySID, 
        ExplicitType = 1
    WHERE ExplicitSID = @SID

    if @@ROWCOUNT > 0
        exec BvSpCache_NotifyUpdated
GO
PRINT N'Creating [dbo].[BvSpPersonGroup_Insert]...';


GO
CREATE PROCEDURE [dbo].[BvSpPersonGroup_Insert]
        @SID                int,
        @Name               nvarchar( 255 ),
        @Description        nvarchar( 255 ),
        @ManualSelection    int,
        @RoleID             int, /* attrs FOR insertion into container */           
        @IsUser             int,
        @IsSelection        int,
        @BvID               int

AS
IF EXISTS ( SELECT [SID] FROM BvPersonGroup WHERE [Name] = @Name )
BEGIN
 RAISERROR('Person group with name %s already exists', 12, 2, @Name)
 RETURN -1
END

IF ISNULL( @BvID, 0 ) > 0
BEGIN
    EXEC @BvID = BvSpSetObjectNumber @SID, 65546, @BvID
    IF @BvID = -1
        RETURN ( 50006 )
END

INSERT  BvPersonGroup( 
        SID,
        [Name],
        [Description], 
        RoleID,
        ManualSelection )
    VALUES( 
        @SID, 
        @Name,
        @Description, 
        @RoleID,
        @ManualSelection )

EXEC BvSpPerson_SpinUp @SID
GO
PRINT N'Creating [dbo].[BvSpPersonGroup_Delete]...';


GO
CREATE PROCEDURE [dbo].[BvSpPersonGroup_Delete]
 @SID int
AS
DECLARE @GroupName NVARCHAR(MAX)

    IF EXISTS( SELECT 1 FROM BvMembership WHERE ContainerSID = @SID )
    BEGIN
        SELECT @GroupName = Name FROM BvPersonGroup WHERE SID = @SID
        RAISERROR( 'The person group "%s" cannot be deleted because it is not empty', 12, 1, @GroupName )
        RETURN (-1)
    END

    DELETE  BvMembership
        WHERE ContainerSID = @SID OR ObjectSID = @SID

    -- delete implicit assigments
    DELETE FROM BvPersonOrGroupAssignmentOnSurvey WHERE PersonOrGroupId = @SID
        
    DELETE FROM BvPersonRel
    FROM BvPersonRel
    WHERE PersonSID = @SID

    DELETE FROM BvNumber WHERE ObjectSID = @SID AND ClassID = 65546

    DELETE  BvPersonGroup
        WHERE SID = @SID
         
    UPDATE BvSvySchedule 
        SET ExplicitSID = BvSvySchedule.SurveySID, 
            ExplicitType = 1
        WHERE ExplicitSID = @SID

    if @@ROWCOUNT > 0
        exec BvSpCache_NotifyUpdated


RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpGetObjectsRange]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetObjectsRange]
@StartIndex INT, 
@ObjectCount INT, 
@OrderField NVARCHAR (64), 
@IsOrderASC BIT, 
@Query NVARCHAR (MAX), 
@IDField NVARCHAR (64), 
@SearchCondition NVARCHAR (4000)=NULL,
@CounterQuery NVARCHAR (MAX) = NULL
AS
if @OrderField = ''
  set @OrderField = 'ID'

  DECLARE @TotalCount INT
  exec @TotalCount = BvSpGetListRange @StartIndex, @ObjectCount, @OrderField, @IsOrderASC, @Query, @IDField, @SearchCondition, @CounterQuery
  return @TotalCount
GO
PRINT N'Creating [dbo].[BvSpGetObjectsPage]...';


GO
CREATE procedure [dbo].[BvSpGetObjectsPage]
 @PageIndex int,
 @PageSize int,
 @OrderField nvarchar(64),
 @IsOrderASC bit,
 @Query nvarchar(MAX),
 @IDField nvarchar(64),
 @SearchCondition NVARCHAR(4000) = NULL,
 @CounterQuery NVARCHAR (MAX) = NULL
as
	DECLARE @StartIndex INT
	DECLARE @TotalCount INT
	IF @PageSize != 2147483647
	BEGIN
		SET @StartIndex = (@PageIndex - 1) * @PageSize + 1
	END
	exec @TotalCount = BvSpGetObjectsRange @StartIndex, @PageSize, @OrderField, @IsOrderASC, @Query, @IDField, @SearchCondition, @CounterQuery
	return @TotalCount
GO
PRINT N'Creating [dbo].[BvSpGetDeferredMonitoringListPage]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetDeferredMonitoringListPage] 
	@PageIndex INT, 
	@PageSize INT, 
	@OrderField NVARCHAR (64),
	@IsOrderASC BIT, 
	@userName NVARCHAR (255),
	@SearchCondition NVARCHAR(4000) = NULL
AS
BEGIN
	IF @userName IS NULL AND @PageIndex IS NULL
	BEGIN
		/* Looks like we're generating code using FMTONLY. So lets return metadata*/
		SELECT
		0  AS ID,
		0 AS PersonSID,
		0 AS SurveySID,
		CAST(0 as bit) AS HasAudio,
		0 AS InterviewID,
		0 as ExtendedStatus,
		'' as ExtendedStatusName,
		GETDATE() AS TimeStamp,
		'' AS SurveyName,
		'' AS SurveyConfirmitName,
		'' AS PersonLogin,
		'' AS PersonName,
		'' AS RespondentName,
		'' AS TelephoneNumber
     
		RETURN 0;
	END
	
	DECLARE @StateGroupID INT
	SELECT @StateGroupID = MIN(ID) FROM BvStateGroup
	
	DECLARE @QueryBody as nvarchar(4000) = 'from 
		BvPersonDeferredMonitoring as def inner join BvSurvey as survey on def.SurveySID = survey.SID
		inner join BvUserSurveyPermission as perm on perm.SurveySID = def.SurveySID
		inner join BvPerson as person on person.SID = def.PersonSID
		inner join BvInterview as interview on interview.ID = def.InterviewID and interview.SurveySID = def.SurveySID
		left join BvState as st on def.ExtendedStatus = st.StateID AND st.StateGroupID = '+ CAST(@StateGroupID AS NVARCHAR) +'
	where 
		def.IsComplete = 1 and perm.UserName = ''' + @userName + ''' and survey.State <> 2'

	DECLARE @Counter as nvarchar(4000) = 'select count(*) cnt '

	DECLARE @Query NVARCHAR(4000) = 'select def.ID, def.PersonSID, def.SurveySID, def.HasAudio, 
		def.InterviewID, def.ExtendedStatus, st.Name as ExtendedStatusName, def.TimeStamp, survey.Name as SurveyName, survey.Description as SurveyConfirmitName, 
		person.Name as PersonLogin,	person.FullName as PersonName, interview.RespondentName, interview.TelephoneNumber ' + @QueryBody

	IF CHARINDEX('RespondentName', @SearchCondition) > 0 OR CHARINDEX('TelephoneNumber', @SearchCondition) > 0
	BEGIN
		SET @Counter = @Counter + @QueryBody
	END
	ELSE
	BEGIN
		SET @Counter = @Counter + 'from 
		BvPersonDeferredMonitoring as def inner join BvSurvey as survey on def.SurveySID = survey.SID
		inner join BvUserSurveyPermission as perm on perm.SurveySID = def.SurveySID
		inner join BvPerson as person on person.SID = def.PersonSID
	where 
		def.IsComplete = 1 and perm.UserName = ''' + @userName + ''' and survey.State <> 2'
	END
	

	DECLARE @TotalCount INT

	EXEC @TotalCount = BvSpGetObjectsPage @PageIndex, @PageSize, @OrderField, @IsOrderASC, @Query, 'ID', @SearchCondition, @Counter
	RETURN @TotalCount
END
GO
PRINT N'Creating [dbo].[BvSpFilter_InsertField]...';


GO
CREATE PROCEDURE [dbo].[BvSpFilter_InsertField]
@FilterSID    INTEGER,
@Table        INTEGER,
@Column       NVARCHAR(255),
@Type         INTEGER,
@Sign         INTEGER,
@Value        NVARCHAR(255),
@IsNeedCast BIT
AS
DECLARE @Ret INTEGER
DECLARE @f1 NVARCHAR(255)
DECLARE @f2 NVARCHAR(255)

    IF @Sign = 8 -- subfilter
    BEGIN
        DECLARE @Find         INTEGER
        DECLARE @SubFilterSID INTEGER

        SET @SubFilterSID = CAST( @Value AS INTEGER )

        IF NOT EXISTS ( SELECT * FROM BvFilters WHERE SID = @SubFilterSID )
        BEGIN
            RAISERROR( N'Filter with SID = %u not found.', 16, 1, @SubFilterSID )
            RETURN(-1)
        END
 
        EXEC @Find = BvSpFilter_CheckCircle @FilterSID, @SubFilterSID
        IF @Find <> 0
        BEGIN
            SELECT @f1 = [Name] FROM BvFilters WHERE SID = @SubFilterSID
            SELECT @f2 = [Name] FROM BvFilters WHERE SID = @FilterSID

            RAISERROR( N'Cannot insert subfilter %s into filter %s : circular reference found.', 12, 1, @f1, @f2 )
            RETURN (-1)
        END

        EXEC @Find = BvSpFilter_CheckSurveyMismatch @FilterSID, @SubFilterSID
        IF @Find <> 0
        BEGIN
            SELECT @f1 = [Name] FROM BvFilters WHERE SID = @SubFilterSID
            SELECT @f2 = [Name] FROM BvFilters WHERE SID = @FilterSID

            RAISERROR( N'Cannot insert subfilter %s into filter %s because it is used for another survey(s).', 12, 1, @f1, @f2 )
            RETURN (-1)
        END
        
        DECLARE @SurveySID INT
        SELECT @SurveySID = SurveySID 
        FROM BvFilters
        WHERE SID = @SubFilterSID
        
        IF @SurveySID > 0
			UPDATE BvFilters
			SET SurveySID = @SurveySID
			FROM dbo.udf_GetParentFilters(@FilterSID) parentFilters
			WHERE parentFilters.SID = BvFilters.SID
    END

    INSERT INTO BvFilterFields( [FilterSID],
        [Table],
        [Column],
        [Type],
        [Sign],
        [Value],
        IsNeedCast )
    VALUES( @FilterSID,
            @Table,
            @Column,
            @Type,
            @Sign,
            @Value,
            @IsNeedCast )
 
    SET @Ret = @@IDENTITY

RETURN ( @Ret )
GO
PRINT N'Creating [dbo].[BvSpCall_Activate]...';


GO
CREATE PROCEDURE [dbo].[BvSpCall_Activate]
	@SurveySID INT,
	@Mode INT,
	@BatchID INT, 
	@Priority INT,
	@PersonSID INT, 
	/* 
	 * @ShiftTypeID > 0 means specific  shift type id( BvShiftType.ID ) and should be resolved to ShiftZoneId in bvSvySchedule.ShiftTypeID
	 * @ShiftTypeID = Int32.MinValue(-2147483648) meens [None] and should ne resolved to Int32.MinValue in BvSvySchedule.ShiftTypeID
	 * @ShiftTypeID =-1 @ShiftTypeID means [Any valid] and should be resolved to -Timezone in BvSvySchedule.ShiftTypeID
	 */
	@ShiftTypeID INT,
	@TimeToCall DATETIME,
	@EnableDisabledCalls BIT,
	@DefaultTZID INT
AS
SET NOCOUNT ON

    DECLARE @ActivateScheduledCalls INT = 8 -- activate prepared scheduled calls ( FilterGenerateMode: SCHEDULEDINTERVIEWID = 8 )
    DECLARE @ActivateSuspendedCalls INT = 9 -- activate prepared suspended calls ( FilterGenerateMode: SUSPENDEDINTERVIEWID = 9 )
    DECLARE @ActivateAllCalls INT = 1 -- activate prepared suspended calls ( FilterGenerateMode: INTERVIEWID = 1 )
	DECLARE @ShiftTypeNone INT = -2147483648; --None constant for bvSvySchedule.ShiftTypeID
	DECLARE @TimeToCallNow DATETIME = '1899-12-30T00:00:00.000'
	DECLARE @TimeToCallMinute DATETIME = DATEADD( minute, 1, @TimeToCall )
	DECLARE @ExplicitType INT = 2;
	DECLARE @sqlQuery NVARCHAR(MAX)
	DECLARE @sqlQueryParams NVARCHAR(MAX)
	DECLARE @whereCondition NVARCHAR(MAX)
	DECLARE @alias NVARCHAR(25) = 'repl'

	DECLARE @IsRandomCallDeliveryEnabled BIT
	DECLARE @OwnerID INT
	DECLARE @SurveySchedulingMode INT

	SELECT @IsRandomCallDeliveryEnabled = IsRandomCallDeliveryEnabled,
           @SurveySchedulingMode = SurveySchedulingMode,
	       @OwnerID = [ScheduleID]
	FROM BvSurvey
	WHERE SID = @SurveySID

	IF (@PersonSID = 0 )
	BEGIN
	    SET @ExplicitType = 1;

		SET @PersonSID = @SurveySID
	END

	DECLARE @DisableActivationITSTable TABLE( ITS INT )

	INSERT INTO @DisableActivationITSTable
	SELECT StateID
	FROM BvState 
	INNER JOIN BvSurvey ON BvState.StateGroupID = BvSurvey.StateGroupID AND
						   BvSurvey.SID = @SurveySID
	WHERE DA = 1


	CREATE TABLE #InterviewTimeZoneTable
	(
		[ID] [int] NOT NULL,
		TimeZoneID [int] NOT NULL,
		Bias [int] NULL,
		ShiftTypeID [int] NOT NULL,
		ConditionValue [int] NOT NULL
	)

	DECLARE @CurFirstDOW INT = @@DATEFIRST
	SET DATEFIRST 7
	INSERT INTO #InterviewTimeZoneTable
	SELECT BvInterview.[ID], 
		   ISNULL(BvInterview.TimezoneID, 0), 
		   ISNULL(dbo.GetTZBias(ISNULL(@TimeToCall, GETUTCDATE()), CASE WHEN ISNULL(TimezoneID, 0) = 0 THEN @DefaultTZID ELSE TimeZoneID END), 0) Bias, 
		   CASE WHEN @ShiftTypeID = @ShiftTypeNone THEN @ShiftTypeID ELSE -ISNULL(BvInterview.TimezoneID, 0) END,
		   CASE WHEN @SurveySchedulingMode = 1 THEN TransientState ELSE 0 END
	FROM BvInterview
	INNER JOIN BvTransferArrays ta ON ta.ItemId = BvInterview.[ID] AND
									  ta.BatchID = @BatchID
	WHERE BvInterview.SurveySID = @SurveySID AND
		  BvInterview.TransientState NOT IN (SELECT * FROM @DisableActivationITSTable)
	SET DATEFIRST @CurFirstDOW


	DECLARE @DistinctTimeZonesTable TABLE
	(
		TimeZoneID [int] NOT NULL
	)
  
	INSERT INTO @DistinctTimeZonesTable 
	SELECT DISTINCT TimeZoneID 
	FROM #InterviewTimeZoneTable


	IF (	@ShiftTypeID <> @ShiftTypeNone ) --[any valid] or specific shift we should chek too
	BEGIN 
		DECLARE @ErrorTimezoneList NVARCHAR(MAX);

		IF ISNULL( @TimeToCall,  @TimeToCallNow ) <>  @TimeToCallNow/*equal zero for DATE type(meens Set to NOW)*/
		BEGIN
			DECLARE @activeshift TABLE
			(
				ShiftID INT NOT NULL, 
				OwnerID INT NOT NULL,
				[ShiftTypeID] INT NOT NULL,
				[TimezoneID] INT
			)
	        
			INSERT INTO @activeshift EXEC BvSpGetActiveShiftsInRelativeTime @TimeToCall, @TimeToCallMinute, @DefaultTZID

			;WITH ActiveTz( TimeZoneID ) AS
			(
				SELECT DISTINCT TimeZoneID
				FROM @activeshift
				WHERE OwnerID = @OwnerID AND 
				      (ShiftTypeID = @ShiftTypeID OR @ShiftTypeID = -1)
			)
			SELECT @ErrorTimezoneList = CASE WHEN @ErrorTimezoneList IS NULL THEN ''
											 ELSE @ErrorTimezoneList + ',' 
										END + CAST( ct.TimeZoneID AS NVARCHAR(64) )
			FROM @DistinctTimeZonesTable ct 
			LEFT JOIN ActiveTz at ON ct.TimezoneID = at.TimezoneID
			WHERE at.TimezoneID IS NULL
		END
		ELSE --@TimeToCall is NULL or @TimeToCallNow
		BEGIN
			SELECT @ErrorTimezoneList = CASE WHEN @ErrorTimezoneList IS NULL THEN ''
											 ELSE @ErrorTimezoneList + ',' 
										END + CAST( ct.TimeZoneID AS NVARCHAR(64) )
			FROM @DistinctTimeZonesTable ct 
			LEFT JOIN BvTzPeriodicalShifts s ON	( ct.TimezoneID = s.tz_id OR ( ct.TimezoneID = 0 AND @DefaultTZID = s.tz_id) ) AND 
												( s.type_id = @ShiftTypeID OR @ShiftTypeID = -1 ) AND
												s.start_dt <> s.finish_dt AND 
												s.owner_id = @OwnerID
			WHERE s.shift_id IS NULL
		END
		
		IF @ErrorTimezoneList IS NOT NULL
		BEGIN
			DELETE BvTransferArrays WHERE BatchID = @BatchID

			RAISERROR( 'Operation cannot be completed, Time specified is out of shifts of selected type in following Tz: %s.', 12, 1, @ErrorTimezoneList )
			RETURN -1
		END
	END

	IF @ShiftTypeID > 0 
	BEGIN
		UPDATE #InterviewTimeZoneTable
		SET ShiftTypeID = BvShiftZones.[ID]
		FROM BvShiftZones 
		WHERE BvShiftZones.ShiftTypeID = @ShiftTypeID AND 
			  BvShiftZones.TimeZoneID = #InterviewTimeZoneTable.TimeZoneID
	END

	IF (@Mode = @ActivateScheduledCalls OR @Mode = @ActivateAllCalls)
	BEGIN
		IF @EnableDisabledCalls <> 0
		BEGIN
			UPDATE  BvSvySchedule
			SET TimeInShift = ( CASE WHEN @TimeToCall = @TimeToCallNow THEN @TimeToCallNow
									 ELSE DATEADD( minute, #InterviewTimeZoneTable.Bias, @TimeToCall ) 
								END),
				Priority = @Priority,
				CallState = 2,
				ShiftTypeID = #InterviewTimeZoneTable.ShiftTypeID,
				ExplicitSID = @PersonSID,
				ExplicitType = @ExplicitType,
				OldPriority = 0,
				ConditionValue = #InterviewTimeZoneTable.ConditionValue
			FROM BvSvySchedule 
			INNER JOIN #InterviewTimeZoneTable ON BvSvySchedule.[InterviewID] = #InterviewTimeZoneTable.[ID] AND BvSvySchedule.SurveySID = @SurveySID
			WHERE CallState > 0
		END
		ELSE
		BEGIN
			UPDATE  BvSvySchedule
			SET TimeInShift = ( CASE WHEN @TimeToCall = @TimeToCallNow THEN @TimeToCallNow
									 ELSE DATEADD( minute, #InterviewTimeZoneTable.Bias, @TimeToCall ) 
								END),
				Priority = @Priority,
				ShiftTypeID = #InterviewTimeZoneTable.ShiftTypeID,
				ExplicitSID = @PersonSID,
				ExplicitType = @ExplicitType,
				OldPriority = 0,
				ConditionValue = #InterviewTimeZoneTable.ConditionValue
			FROM BvSvySchedule 
			INNER JOIN #InterviewTimeZoneTable ON BvSvySchedule.[InterviewID] = #InterviewTimeZoneTable.[ID] AND BvSvySchedule.SurveySID = @SurveySID
			WHERE CallState > 0
		END
	END
		  
	IF (@Mode = @ActivateSuspendedCalls OR @Mode = @ActivateAllCalls)
	BEGIN
		EXEC BvClr_QuotaService_GetWhereForAllClosedSurveyCells @SurveySID, @alias, @whereCondition OUTPUT
     
		SET @sqlQuery = 
		N'INSERT INTO BvSvySchedule
			SELECT
				0,-- ApptID
				#InterviewTimeZoneTable.ShiftTypeID,-- ShiftTypeID
				#InterviewTimeZoneTable.[ID],
				@SurveySID,
				2 as CallStateCurrent,
				@Priority,
				(CASE WHEN @TimeToCall = @TimeToCallNow THEN @TimeToCallNow
					  ELSE DATEADD( minute, #InterviewTimeZoneTable.Bias, @TimeToCall )
				END),-- TimeInShift
				''9999-01-01 00:00:00.000'',-- ExpireTime
				@PersonSID,
				@ExplicitType,
				''00000000-0000-0000-0000-000000000000'',
				0,
				(CASE WHEN @IsRandomCallDeliveryEnabled = 0 THEN #InterviewTimeZoneTable.[ID]
					 ELSE dbo.GetRandomValue(#InterviewTimeZoneTable.[ID])
				END),
				0 /*old priority*/,
				#InterviewTimeZoneTable.ConditionValue
			FROM #InterviewTimeZoneTable
			LEFT JOIN BvReplicatedData_' + CAST(@SurveySID AS NVARCHAR(255)) + ' AS repl ON repl.respid = #InterviewTimeZoneTable.ID AND
						(' + @whereCondition + ')
			WHERE repl.respid IS NULL AND
				  NOT EXISTS ( SELECT [ID] 
							   FROM BvSvySchedule
							   WHERE BvSvySchedule.SurveySID = @SurveySID AND 
									 BvSvySchedule.InterviewID = #InterviewTimeZoneTable.[ID] )'
   
		SET @sqlQueryParams = N'@ShiftTypeID INT, @Priority INT, @PersonSID INT, @ExplicitType INT, '+
			'@SurveySID INT, @TimeToCall DATETIME, @IsRandomCallDeliveryEnabled BIT, @TimeToCallNow DATETIME';
   
		EXEC sp_executesql @sqlQuery, @sqlQueryParams, @ShiftTypeID, @Priority, @PersonSID, @ExplicitType,
			@SurveySID, @TimeToCall, @IsRandomCallDeliveryEnabled, @TimeToCallNow
	END

	INSERT INTO BvCachedCallsInsert
	SELECT ta.ItemId, @SurveySID
	FROM BvTransferArrays ta
	WHERE ta.BatchID = @BatchId
       
	DELETE BvTransferArrays WHERE BatchID = @BatchID
	EXEC BvSpAddUniqueAssignment @PersonSID
       
RETURN 0
GO
PRINT N'Creating [dbo].[BvSpAssignment_Insert2]...';


GO
CREATE PROCEDURE [dbo].[BvSpAssignment_Insert2]
@SurveySID INT, 
@PersonSID INT,
@BatchID INT
AS
SET NOCOUNT ON

    UPDATE BvSvySchedule 
    SET ExplicitSID = @PersonSID, 
        ExplicitType = 2, --Person type
        Priority = CASE WHEN OldPriority > 0 THEN OldPriority ELSE Priority END,
        OldPriority = 0
    FROM BvTransferArrays
    WHERE BvTransferArrays.BatchID = @BatchID AND
          BvSvySchedule.SurveySID = @SurveySID AND
          BvSvySchedule.InterviewID = BvTransferArrays.ItemID AND
          BvSvySchedule.CallState > 0

    INSERT INTO BvCachedCallsInsert
    SELECT ItemID, @SurveySID 
	FROM BvTransferArrays
    WHERE BvTransferArrays.BatchID = @BatchID

    exec BvSpAddUniqueAssignment @PersonSID

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpAssignment_Insert]...';


GO
CREATE PROCEDURE [dbo].[BvSpAssignment_Insert]
@SID INT, 
@SurveySID INT, 
@InterviewSID INT, 
@PersonSID INT, 
@RoleID INT, 
@FromCall INT=0
AS
SET NOCOUNT ON
DECLARE @InsertedAssignmentsCount INTEGER = 0

IF @InterviewSID > 0 OR @FromCall > 0 
BEGIN

            UPDATE BvSvySchedule SET
                ExplicitSID = @PersonSID, 
                ExplicitType = 2, --Person type
                Priority = CASE WHEN OldPriority > 0 THEN OldPriority ELSE Priority END,
                OldPriority = 0
            WHERE SurveySID = @SurveySID AND 
                  InterviewID = @InterviewSID AND
                  CallState > 0

            insert into BvCachedCallsInsert 
			values(@InterviewSID, @SurveySID)

            exec BvSpAddUniqueAssignment @PersonSID
END
ELSE
BEGIN
        
    IF NOT EXISTS ( SELECT * FROM BvPersonOrGroupAssignmentOnSurvey
        WHERE PersonOrGroupId = @PersonSID AND SurveyId = @SurveySID )
          INSERT INTO BvPersonOrGroupAssignmentOnSurvey( PersonOrGroupId, SurveyId )
              VALUES( @PersonSID, @SurveySID )
              
    SET @InsertedAssignmentsCount = @@ROWCOUNT          
   
   IF EXISTS ( SELECT SID FROM BvPerson WHERE SID = @PersonSID )
   BEGIN
	   INSERT INTO BvPersonRel( PersonSID, ObjectSID, RoleID, Type )
	   VALUES(@PersonSID, @SurveySID, 2, 2)
		   
	   EXEC BvSpLogin_SpinUp @PersonSID
   END
   ELSE
   BEGIN
       INSERT INTO BvPersonRel( PersonSID, ObjectSID, RoleID, Type )
       SELECT PersonSid, @SurveySID, 2, 2
       FROM BVPersonRel
       WHERE @PersonSID = ObjectSID AND
             ObjectSID != PersonSid
       
       insert into BvLoginGroup 
       select personGroup.PersonSID, @SurveySID, lg.SurveySID
       from BvPersonRel personGroup
       inner join BvLoginGroup lg on lg.PersonSid = personGroup.PersonSID AND  --get surveySid from BvLoginGroup which should be set already
                                     lg.PersonSid = lg.ObjectSid
       inner join BvPerson ON sid = personGroup.PersonSID           --get only persons assigned to current group
       where personGroup.ObjectSID = @PersonSID AND
             personGroup.ObjectSID != personGroup.PersonSID			--we not need in fake records
   END
END

RETURN @InsertedAssignmentsCount
GO
PRINT N'Creating [dbo].[BvSpAssignment_Delete]...';


GO
CREATE PROCEDURE [dbo].[BvSpAssignment_Delete]
@SurveySID INT, 
@Count INT, 
@PersonSID INT, 
@RoleID INT
AS
SET NOCOUNT ON

DECLARE @InsertedAssignmentsCount INTEGER = 0

 IF @Count > 0 
 BEGIN

    CREATE TABLE #tc
    (
        [ID] [int] not null
    )
    INSERT INTO #tc
       SELECT [InterviewID] FROM BvSvySchedule
       WHERE ExplicitSID = @PersonSID AND
          SurveySID = @SurveySID AND
          CallState = 2 AND
          @RoleID = 2

    UPDATE BvSvySchedule SET ExplicitSID = @SurveySID, ExplicitType = 1
        WHERE SurveySID = @SurveySID AND InterviewID IN 
            ( SELECT [ID] FROM #tc )
        

    insert into BvCachedCallsInsert 
    select [ID], @SurveySID 
	from #tc

    DROP TABLE #tc
    
    RETURN @InsertedAssignmentsCount
 END
 ELSE
 BEGIN
    DELETE FROM BvPersonOrGroupAssignmentOnSurvey
      WHERE PersonOrGroupId = @PersonSID AND SurveyId = @SurveySID
    SET @InsertedAssignmentsCount = @@ROWCOUNT
 END

-- recalculate login cache
IF EXISTS ( SELECT SID FROM BvPerson WHERE SID = @PersonSID )
   EXEC BvSpPerson_SpinUp @PersonSID
ELSE
BEGIN
   DECLARE @deassignmentPersons TABLE(personId int)
   DELETE BvPersonRel
   OUTPUT deleted.PersonSid
   INTO @deassignmentPersons
   FROM BvPersonRel base
   WHERE ObjectSid = @SurveySID AND    --look at assignments to survey only
         Type = 2 AND                          
         PersonSid IN (SELECT PersonSid        --look at all persons inside current group
                       FROM BvPersonRel pr
                       WHERE Type = 1 AND
                             ObjectSid = @PersonSID) AND
         NOT EXISTS (SELECT *                  --if person doesn't assign directly to survey
                     FROM BvPersonOrGroupAssignmentOnSurvey
                     WHERE PersonOrGroupId = base.PersonSid AND
                           SurveyId = @SurveySID) AND
         NOT EXISTS (SELECT *                  --if person doesn't belong to others groups assigned to survey
                     FROM BvMemberShip
                     INNER JOIN BvPersonOrGroupAssignmentOnSurvey ON PersonOrGroupId = ContainerSid AND
                                                                     SurveyId = @SurveySID
                     WHERE ObjectSid = base.PersonSid)
                     
   INSERT INTO @deassignmentPersons VALUES(@PersonSID)
                     
   DELETE BvLoginGroup
   FROM @deassignmentPersons dp
   WHERE PersonSid = personID AND
         ObjectSID = @SurveySID
END

RETURN @InsertedAssignmentsCount
GO
PRINT N'Creating [dbo].[BvSpInterviewerBreaksReport]...';


GO
CREATE PROCEDURE [dbo].[BvSpInterviewerBreaksReport]
    @personIds NVARCHAR(MAX),
	@SearchCondition NVARCHAR(MAX),
	@PageIndex INT,
	@PageSize INT,
	@OrderField NVARCHAR(64),
	@IsOrderASC BIT
AS
	IF @personIds IS NULL AND @PageIndex IS NULL AND @PageSize IS NULL
	BEGIN
		SELECT  '' AS PersonName,
				CAST(NULL AS DATETIME) AS StartTime,
				0 AS Duration
		WHERE 1 = 0
		RETURN 0;
	END
	
	DECLARE @query NVARCHAR(MAX) = '
	SELECT Name PersonName,
	       StartTime,
		   Duration
	FROM BvTimeBreaksHistory
	INNER JOIN dbo.utilSplitNumbers( ISNULL(''' + ISNULL(@PersonIds, '') + ''', ''''), '','') s1 ON s1.Item = InterviewerId
	INNER JOIN BvPerson ON SID = InterviewerId'
	      
	DECLARE @TotalCount INT

    EXEC @TotalCount = BvSpGetObjectsPage @PageIndex, @PageSize, @OrderField, @IsOrderASC, @Query, @OrderField, @SearchCondition
    RETURN @TotalCount
RETURN 0
GO
PRINT N'Creating [dbo].[BvSpAlertsHistoryReport]...';


GO
CREATE PROCEDURE BvSpAlertsHistoryReport
	@personIds NVARCHAR(MAX),
	@surveyIds NVARCHAR(MAX),
	@SearchCondition NVARCHAR(MAX),
	@PageIndex INT,
	@PageSize INT,
	@OrderField NVARCHAR(64),
	@IsOrderASC BIT
 AS
 
	IF @personIds IS NULL AND @surveyIds IS NULL AND @PageIndex IS NULL AND @PageSize IS NULL
	BEGIN
	/* Looks like we're generating code using FMTONLY. So lets return metadata*/
	SELECT  0 AS PersonId,
			'' AS PersonName,
			0 AS SurveyId,
			'' AS ProjectId,
			'' AS SurveyName,
			0 AlertType,
			cast(0 as bit) Alert,
			0 AS AnswerDuration,
			'' AS QuestionId,
			CAST(NULL AS DATETIME) AS SubmissionTime,
			0 AS InterviewId,
			CAST(0 AS TINYINT) AS InterviewState
     WHERE 1 = 0
	 RETURN 0;
	END
 
    DECLARE @query NVARCHAR(MAX) = '
    SELECT p.Sid AS PersonId,
           p.Name AS PersonName,
           s.SID AS SurveyId,
           s.Name AS ProjectId,
           s.Description AS SurveyName,
           (CASE WHEN h.AnswerSubmissionAlert IS NULL THEN 2 ELSE 1 END) AlertType,
           (CASE WHEN h.AnswerSubmissionAlert IS NULL THEN h.QuickAnswerSubmissionAlert ELSE h.AnswerSubmissionAlert END) Alert,
           h.AnswerDuration,
           h.QuestionId,
           h.SubmissionTime,
           h.InterviewId,
           h.InterviewState
    FROM BvAnswerSubmissionAlertHistory h
    LEFT JOIN dbo.utilSplitNumbers( ISNULL(''' + ISNULL(@PersonIds, '') + ''', ''''), '','') s1 ON s1.Item = h.PersonId
    INNER JOIN BvPerson p ON p.Sid = h.PersonId
    INNER JOIN dbo.utilSplitNumbers( ISNULL(''' + @SurveyIds + ''', ''''), '','') s2 ON s2.Item = h.SurveyId
    INNER JOIN BvSurvey s ON s.SID = h.SurveyId
    WHERE '''' = ''' + ISNULL(@PersonIds, '') + ''' OR s1.Item IS NOT NULL'

    DECLARE @TotalCount INT

    EXEC @TotalCount = BvSpGetObjectsPage @PageIndex, @PageSize, @OrderField, @IsOrderASC, @Query, @OrderField, @SearchCondition
    RETURN @TotalCount
GO
PRINT N'Creating [dbo].[BvSpGetPersonsListPage]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetPersonsListPage]
 @ParentGroupsIDs NVARCHAR (MAX), 
 @PageIndex INT,
 @PageSize INT, 
 @OrderField NVARCHAR (64), 
 @IsOrderASC BIT, 
 @SearchCondition NVARCHAR (4000)=NULL
AS
BEGIN
 IF @ParentGroupsIDs IS NULL AND @PageIndex IS NULL
 BEGIN
 /* Looks like we're generating code using FMTONLY. So lets return metadata*/
 SELECT
     0  AS PersonSID,
     '' AS PersonName,
     '' AS PersonDescription,
     CAST(0 as BIT)  AS LoggedIn,
     0 as ManualSelection,
     0 as AllowedChoices,
     '' as SurveyID,
     CAST(0 as BIT)  AS IsLocked,
     CAST(NULL AS DATETIME) AS LockedDate,
	 '' AS CallGroupName,
     '' AS PersonLocation
     RETURN 0;
 END
 
 DECLARE @Query nvarchar(max)
 DECLARE @IDField nvarchar(64)
 SET @IDField = 'PersonSID'
   
 SET @Query = 
   N'SELECT DISTINCT [BvPerson].[SID] PersonSID,
    [BvPerson].[Name] PersonName,
    [BvPerson].[Description] PersonDescription,
    cast((case when t.[PersonSID] is null 
       then 0
       else 1 
     end) as bit) as [LoggedIn],
     [BvPerson].[ManualSelection] as ManualSelection,
     [BvPerson].[AllowedChoices] as AllowedChoices,
     ISNULL ( s.Name, '''' ) as [SurveyID],
     [BvPerson].[IsLocked] as IsLocked,
     [BvPerson].[LockedDate] as LockedDate,
	 ISNULL( [BvCallGroup].[Name], '''' ) as CallGroupName,
	 [BvPerson].[Location] as PersonLocation
     FROM [dbo].[BvPerson]
	 LEFT JOIN [BvCallGroup]
	  ON [BvPerson].[CallGroupID] = [BvCallGroup].ID
     LEFT JOIN [dbo].[BvMembership]
      ON [BvMembership].[ObjectSID] = [BvPerson].[SID]
     LEFT JOIN dbo.BvTasks t
      on [BvPerson].SID = t.PersonSID
     LEFT JOIN  dbo.bvsurvey s
       on s.SID = t.SurveySID and s.State <> 2
    WHERE [BvPerson].[SID] = [BvMembership].[ObjectSID] 
    AND [BvMembership].[ContainerSID] in (' + @ParentGroupsIDs + ')'

   
   IF @OrderField = '' OR @OrderField = null
   SET @OrderField = 'PersonSID' 
   
   DECLARE @TotalCount INT

   EXEC @TotalCount = BvSpGetObjectsPage @PageIndex, @PageSize, @OrderField, @IsOrderASC, @Query, @IDField, @SearchCondition
   RETURN @TotalCount
END
GO
PRINT N'Creating [dbo].[BvTimeBreaksHistory].[Duration].[MS_Description]...';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'Duration is measured in seconds', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'BvTimeBreaksHistory', @level2type = N'COLUMN', @level2name = N'Duration';


GO
PRINT N'Creating [dbo].[BvVersionHistory].[Description].[MS_Description]...';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'The description from ScriptDefinitionFile', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'BvVersionHistory', @level2type = N'COLUMN', @level2name = N'Description';


GO
PRINT N'Creating [dbo].[BvVersionHistory].[Duration].[MS_Description]...';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Time in milliseconds took to apply the script', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'BvVersionHistory', @level2type = N'COLUMN', @level2name = N'Duration';


GO
PRINT N'Creating [dbo].[BvSurvey].[NotificationEmail].[MS_Description]...';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Email address to receive emails triggered by scripting errors in an interview.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'BvSurvey', @level2type = N'COLUMN', @level2name = N'NotificationEmail';


GO
PRINT N'Checking existing data against newly created constraints';


GO



GO
ALTER TABLE [dbo].[BvQuotaFilter] WITH CHECK CHECK CONSTRAINT [FK_BvQuotaFilter_surveyId];

ALTER TABLE [dbo].[BvQuotaBalancing] WITH CHECK CHECK CONSTRAINT [FK_BvQuotaBalancing_surveyId];

ALTER TABLE [dbo].[BvInterviewTimings] WITH CHECK CHECK CONSTRAINT [ReferForeignField];

ALTER TABLE [dbo].[BvUserSurveyPermission] WITH CHECK CHECK CONSTRAINT [FkBvUserSurveyPermission_Survey];

ALTER TABLE [dbo].[BvTimezoneShift] WITH CHECK CHECK CONSTRAINT [FK_BvTimezoneShift_TimezoneID];

ALTER TABLE [dbo].[BvThresholds] WITH CHECK CHECK CONSTRAINT [FkBvThresholds_ThresholdTypes];

ALTER TABLE [dbo].[BvSurvey] WITH CHECK CHECK CONSTRAINT [FK_BvSurvey_Schedule];

ALTER TABLE [dbo].[BvSchedule] WITH CHECK CHECK CONSTRAINT [FK_BvSchedule_BvStateGroup];

ALTER TABLE [dbo].[BvReportParam] WITH CHECK CHECK CONSTRAINT [FK_BvReportParam_BvReportBatch];

ALTER TABLE [dbo].[BvReportBatch] WITH CHECK CHECK CONSTRAINT [FK_BvReportBatch_BvReport];

ALTER TABLE [dbo].[BvPersonMonitoringLastID] WITH CHECK CHECK CONSTRAINT [FK_BvPersonMonitoringLastID_BvPersonMonitoring];

ALTER TABLE [dbo].[BvPersonMonitoringEvents] WITH CHECK CHECK CONSTRAINT [FK_BvPersonMonitoringEvents_BvPersonMonitoring];

ALTER TABLE [dbo].[BvPersonMonitoring] WITH CHECK CHECK CONSTRAINT [FK_BvPersonMonitoring_BvPerson];

ALTER TABLE [dbo].[BvPerson] WITH CHECK CHECK CONSTRAINT [FK_BvPerson_BvSurvey];

ALTER TABLE [dbo].[BvPerson] WITH CHECK CHECK CONSTRAINT [FK_BvPerson_CallGroupID];

ALTER TABLE [dbo].[BvPerson] WITH CHECK CHECK CONSTRAINT [FK_BvPerson_TimezoneID];

ALTER TABLE [dbo].[BvMessageToPerson] WITH CHECK CHECK CONSTRAINT [FK_BvMessageToPerson_BvPerson];

ALTER TABLE [dbo].[BvMessageToPerson] WITH CHECK CHECK CONSTRAINT [FK_BvMessageToPerson_BvMessages];

ALTER TABLE [dbo].[BvAppointmentsAlertStatus] WITH CHECK CHECK CONSTRAINT [FkBvAppointmentsAlertStatus_Appointment];

ALTER TABLE [dbo].[BvAppointmentCounters] WITH CHECK CHECK CONSTRAINT [FkBvAppointmentCounters_Survey];

ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus] WITH CHECK CHECK CONSTRAINT [FkBvAggregateSurveyAlertStatus_Survey];

ALTER TABLE [dbo].[BvAggregateSurvey] WITH CHECK CHECK CONSTRAINT [FkBvAggregateSurvey_Survey];


GO
/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
/* Data loading */

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__RefactorLog]') AND type in (N'U'))
	DROP TABLE [dbo].[__RefactorLog]
GO


PRINT 'Data loading:'
GO
;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
(
SELECT 'ActivityLogging.InterviewerActivityEventTimingsThreshold' as SystemName, 'Threshold of interviewer activity event timings' as DisplayName, 'Logging' as [Group], 'If Interviewer Activity Event takes longer than Threshold time then it will contain timings information, otherwise it will not.' as [Description], 4 as [Type], 0 as Hidden, '00:00:00' as Value
UNION ALL 
SELECT 'ActivityLogging.ManagementActivityEventTimingsThreshold' as SystemName, 'Threshold of management activity event timings' as DisplayName, 'Logging' as [Group], 'If Management Activity Event takes longer than Threshold time then it will contain timings information, otherwise it will not.' as [Description], 4 as [Type], 0 as Hidden, '00:00:00' as Value
UNION ALL 
SELECT 'Site.StartSurveyURL' as SystemName, 'StartSurveyURL' as DisplayName, 'System' as [Group], 'URL which is used for generation of interview url.' as [Description], 2 as [Type], 0 as Hidden, 'http://localhost/wix/cati_' as Value
UNION ALL 
SELECT 'SQLServer.DefaultSqlCommandTimeout', 'DefaultSqlCommandTimeout', 'System', 'This timeout is used in Data Access Layer.', 1, 0, '120'
UNION ALL
SELECT 'SQLServer.DefaultSqlConnectionTimeout', 'DefaultSqlConnectionTimeout', 'System', 'This timeout is used in Data Access Layer.', 1, 0, '120'
UNION ALL
SELECT 'SQLServer.TemplateDBFileName', 'TemplateDBFileName', 'System', 'Path (with file name) to the backup which is used for the new CATI instance creation.', 2, 0, 'C:\backupConfirmitCATIV15.bak'
UNION ALL 
SELECT 'SQLServer.SqlServerDataPath', 'SqlServerDataPath', 'System', 'Path to the master DB file (used in Backend instance registrator). Either both SqlServerDataPath and SqlServerLogPath should be empty or both should contain path.', 2, 0, ''
UNION ALL
SELECT 'SQLServer.SqlServerLogPath', 'SqlServerLogPath', 'System', 'Path to save DB logs. Either both SqlServerDataPath and SqlServerLogPath should be empty or both should contain path.', 2, 0, ''
UNION ALL
SELECT 'Server.AccessAllowedIPAddresses', 'Access Allowed IP Addresses', 'System', 'IP addresses which internal WCF services are allowed for.', 2, 0, '127.0.0.1'
UNION ALL 
SELECT 'Server.ServiceStartTimeout', 'ServiceStartTimeout', 'System', 'Start CATI instance service timeout (in sec).', 1, 0, '60'
UNION ALL
SELECT 'WebServiceUrl.Authoring', 'Confirmit Authoring WS Url', 'System', 'Confirmit Authoring WS Url.', 2, 0, ''
UNION ALL
SELECT 'WebServiceUrl.SurveyData', 'Confirmit SurveyData WS Url', 'System', 'Confirmit SurveyData WS Url.', 2, 0, ''
UNION ALL
SELECT 'Debug.BackendStartup', 'Break point on backend startup', 'Debug', 'Breakpoint on backend startup', 3, 0, 'False'
UNION ALL 
SELECT 'Debug.PublishMetadataForExternalWCFServices', 'Publish Metadata For External WCF Services', 'Debug', 'Should metadata for external services be published or not.', 3, 0, 'True'
UNION ALL
SELECT 'Debug.PublishMetadataForInternalWCFServices', 'Publish Metadata For Internal WCF Services', 'Debug', 'Should metadata for internal services be published or not.', 3, 0, 'True'
UNION ALL
SELECT 'Email.NotificationEmailRecipients', 'NotificationEmailRecipients', 'Logging', 'Email address(es) to send an email if the connection to the dialer is lost or the local dialer component is restarted.', 2, 0, NULL
UNION ALL
SELECT 'Email.AdministratorEmailAddress', 'AdministratorEmailAddress', 'Logging', 'Email address(es) to send an email whenever an interviewer account is locked out.', 2, 0, NULL
UNION ALL
SELECT 'Email.NotificationEmailBCC', 'NotificationEmailBCC', 'Logging', 'BCC address(es) to send dialer unavailable notification, Dialer WS started notification or notifications about errors during scheduling script execution.', 2, 0, ''
UNION ALL
SELECT 'Email.NotificationExceptionLimit', 'EmailNotificationExceptionLimit', 'Logging', 'Limit of errors which will be detailed in the mail about errors while sample upload with Full scheduling.', 1, 0, '5'
UNION ALL
SELECT 'Logging.TraceVerbose', 'TraceVerbose', 'Logging', 'Switch the logging of Verbose messages on/off.', 3, 0, 'False'
UNION ALL 
SELECT 'Logging.EnableReceivingClientErrors', 'EnableReceivingClientErrors', 'Logging', 'Turn the logging of errors from CATI Console and CATI Monitoring Player on the server on/off.', 3, 0, 'True'
UNION ALL
SELECT 'Dialer.DialerType', 'Dialer Type', 'Telephony', 'Type of the Dialer(s) which currently used with the CATI company.', 2, 0, 'NoDialler'
UNION ALL
SELECT 'Dialer.DefaultSurveyParameters', 'DialerDefaultSurveyParameters', 'Telephony', 'Set of parameters to configure the way the dialing system handles situations related to dialing routines.', 2, 0, NULL
UNION ALL
SELECT 'Dialer.AudioRecordingsPageSize', 'AudioRecordingsPageSize', 'Telephony', 'Size of pages to obtain audio for interviews.', 1, 0, '100'
UNION ALL
SELECT 'Dialer.ServiceCallsRetryLimit', 'ServiceCallsRetryLimit', 'Telephony', 'Number of attempts which Backend makes to get successful answer from Dialer WS.', 1, 0, '6'
UNION ALL
SELECT 'Dialer.HealthControlStopWaitTime', 'DialerHealthControlStopWaitTime', 'Telephony', 'DialerHealthControl thread waiting time (in ms).', 1, 0, '7000'
UNION ALL
SELECT 'Dialer.HealthControlCheckPeriod', 'DialerHealthControlCheckPeriod', 'Telephony', 'Dialer Get State interval (in ms).', 1, 0, '60000'
UNION ALL
SELECT 'Dialer.HealthControlUnavailableTimeoutInMs', 'DialerHealthControlUnavailableTimeoutInMs', 'Telephony', 'Period to wait for successful response from Dialer (in ms).', 1, 0, '180000'
UNION ALL
SELECT 'Dialer.InterviewerPredictiveSafeBreakWaitTimeout', 'InterviewerPredictiveSafeBreakWaitTimeout', 'Telephony', 'The timeout (in ms) is needed to be sure that the call won''t be delivered to interviewer in predictive mode after ''GoNotReady'' was called.', 1, 0, '5000'
UNION ALL
SELECT 'Dialer.DelayForGetAudioRecordsMs', 'DelayForGetAudioRecordsMs', 'Telephony', 'Time to wait (in ms) for audio file creation.', 1, 0, '5000'
UNION ALL
SELECT 'DeferredMonitoring.DeferredRecordsExpirationPeriodInDays', 'DeferredRecordsExpirationPeriodInDays', 'Deferred Monitoring', 'Number of days after which the deferred record will be deleted.', 1, 0, '30'
UNION ALL 
SELECT 'DeferredMonitoring.EnableDeferredRecordsCleanup', 'EnableDeferredRecordsCleanup', 'Deferred Monitoring', 'Cleaning of deferred records is switched on/off.', 3, 0, 'True'
UNION ALL 
SELECT 'DeferredMonitoring.DeferredRecordsAudioObtainingPeriodInHours', 'DeferredRecordsAudioObtainingPeriodInHours', 'Deferred Monitoring', 'For deferred records which have been made during these last hours system tries to get audio files.', 1, 0, '-1'
UNION ALL 
SELECT 'DeferredMonitoring.DeferredMonitoringCleanupDeleteTopRows', 'DeferredMonitoringCleanupDeleteTopRows', 'Deferred Monitoring', 'Max number of deferred records which to delete at a time.', 1, 0, '300'
UNION ALL 
SELECT 'DeferredMonitoring.DeferredMonitoringCleanupRunPeriodInMinutes', 'DeferredMonitoringCleanupRunPeriodInMinutes', 'Deferred Monitoring', 'Interval (in min) to run thread which cleans deferred records.', 1, 0, '30'
UNION ALL 
SELECT 'DeferredMonitoring.DeferredMonitoringCleanupDelayBetweenDeletesInMs', 'DeferredMonitoringCleanupDelayBetweenDeletesInMs', 'Deferred Monitoring', 'Delay (in ms) between deferred records portions deletion.', 1, 0, '0'
UNION ALL 
SELECT 'Quotas.MaxQuestionsPerQuota', 'MaxQuestionsPerQuota', 'Quotas', 'Maximum number of questions which CATI quota can be based on.', 1, 0, '5'
UNION ALL
SELECT 'QuotaBalancing.TotalPeriodIsSec', 'Promotion period', 'Quotas', 'Total time (in sec) allotted for running promotion procedure for all ''quota balanced'' surveys (opened surveys with the quota chosen for balancing).', 1, 0, '900'
UNION ALL
SELECT 'QuotaBalancing.MaxCellsCount', 'Max cells to promote', 'Quotas', 'Maximal number of cells which can be promoted during one promotion session.', 1, 0, '5'
UNION ALL
SELECT 'QuotaBalancing.MinDelayInSec', 'Min delay between calls of promotion process', 'Quotas', 'Minimal delay (in sec) between calls of promotion process.', 1, 0, '10'
UNION ALL
SELECT 'QuotaBalancing.PromotionHistoryCleanPeriod', 'PromotionHistoryCleanPeriod', 'Quotas', 'Number of days after which the promotion history record will be deleted.', 1, 0, '7'
UNION ALL 
SELECT 'AppointmentAlert.ShortInterval', 'AppointmentAlert.ShortInterval', 'Supervisor', 'Short interval for appointment counters (Appointment List from the Activity Views).', 1, 0, '3600'
UNION ALL
SELECT 'AppointmentAlert.LongInterval', 'AppointmentAlert.LongInterval', 'Supervisor', 'Long interval for appointment counters (Appointment List from the Activity Views).', 1, 0, '-1'
UNION ALL 
SELECT 'Site.TimeZoneID', 'TimeZoneID', 'Supervisor', 'ID of the time zone which is currently set as Local.', 1, 0, '1'
UNION ALL 
SELECT 'AsyncOperation.ActivatePortionSize', 'Asynchronous ''Activate'' portion size', 'Supervisor', 'Portion size for asynchronous activate call operation.', 1, 0, '1000'
UNION ALL
SELECT 'AsyncOperation.MovePortionSize', 'Asynchronous ''Move'' portion size', 'Supervisor', 'Portion size for asynchronous move call operation.', 1, 0, '1000'
UNION ALL
SELECT 'AsyncOperation.AsyncOperationCleanTimeoutInHours', 'AsyncOperationCleanTimeoutInHours', 'Supervisor', 'Number of hours after which the asynchronous operation record will be deleted.', 1, 0, '720'
UNION ALL
SELECT 'AnswerSubmissionAlert.AnswerSubmissionAlertHistoryCleanPeriod', 'AnswerSubmissionAlertHistoryCleanPeriod', 'Supervisor', 'All records made earlier this period (in days) will be deleted from BvAnswerSubmissionAlertHistory table.', 1, 0, '30'
UNION ALL
SELECT 'Replication.BackgroundReplicationSleepPeriod', 'BackgroundReplicationSleepPeriod', 'Supervisor', 'ReplicationThread interval (in ms).', 1, 0, '60000'
UNION ALL 
SELECT 'Replication.ForceReplicationLockTimeout', 'ForceReplicationLockTimeout', 'Supervisor', 'Timeout to get an exclusive lock (in ms).', 1, 0, '120000'
UNION ALL
SELECT 'TelephoneBlacklist.TelephoneBlacklistLimit', 'TelephoneBlacklistLimit', 'Supervisor', 'Limit of numbers of phone numbers in the blacklist.', 1, 0, '350000'
UNION ALL 
SELECT 'AccountLocking.Enabled', 'AccountLocking.Enabled', 'Interviewing', 'Automatic locking of interviewers account functionality is switched on/off.', 3, 0, 'True'
UNION ALL
SELECT 'AccountLocking.MaxFailedLoginAttempts', 'AccountLocking.MaxFailedLoginAttempts', 'Interviewing', 'Number of consecutive unsuccessful login attempts after which the account will be locked automatically.', 1, 0, '3'
UNION ALL
SELECT 'Console.StateServiceSessionTimeoutInMinutes', 'StateServiceSessionTimeoutInMinutes', 'Interviewing', 'Period (in min) after which StateService sessions expire.', 1, 0, '600'
UNION ALL
SELECT 'Console.KeepAliveInterval', 'Keep alive interval', 'Interviewing', 'Keep alive interval (in ms).', 1, 0, '10'
UNION ALL
SELECT 'CacheCalls.InterviewsCountPerPerson', 'Interviews count per person', 'Interviewing', 'Number of calls in cache per interviewer logged into the console (number of calls appear in Active calls view per logged in console person).', 1, 0, '20'
UNION ALL
SELECT 'Console.InterviewsCountShownInManualMode', 'Interviews count shown in manual mode', 'Interviewing', 'Interviews count shown to an interviewer that is logged in CATI Console in manual mode.', 1, 0, '100'
UNION ALL
SELECT 'AutoLogout.AutoLogoutThreadSleepPeriod', 'AutoLogoutThreadSleepPeriod', 'Interviewing', 'AutoLogoutThread interval (in ms).', 1, 0, '3600000'
UNION ALL 
SELECT 'AutoLogout.AutoLogoutTimeout', 'AutoLogoutTimeout', 'Interviewing', 'Time (in ms) after which the person will be logged out automatically.', 1, 0, '7200000'
UNION ALL
SELECT 'SurveyCleanup.NotificationTimeout', 'Survey cleanup notification timeout', 'Supervisor', 'Period of inactivity after which a notification that survey is going to be cleaned is sent.', 4, 0, '90.00:00:00'
UNION ALL
SELECT 'SurveyCleanup.CleanupTimeout', 'Survey cleanup timeout', 'Supervisor', 'The time which passes after the warning notification was sent before the survey is really cleaned.', 4, 0, '10.00:00:00'
UNION ALL
SELECT 'RoutineMaintenance.WeeklyTime', 'Weekly time of routine maintenance', 'Supervisor', 'The day of week and time at which the routine maintenance (automatic survey cleanup procedure) starts.', 4, 0, '6.00:00:00'
UNION ALL 
SELECT 'RoutineMaintenance.Duration', 'Duration of routine maintenance', 'Supervisor', 'Routine maintenance duration (survey cleaning procedure duration).', 4, 0, '0.06:00:00'
UNION ALL 
SELECT 'CallGroup.Enabled', 'Enabled call group functionality', 'Supervisor', 'Call group functionality is switched on/off.', 3, 0, 'False'
UNION ALL 
SELECT 'CallGroup.EnabledForNewSurveys', 'Enabled call group for new surveys', 'Supervisor', 'Default call group value for newly created surveys', 3, 0, 'False'
UNION ALL 
SELECT 'Reports.CallHistoryReportEnabled', 'CallHistoryReportEnabled', 'Supervisor', 'Is scheduled call history report enabled?', 3, 0, 'False'
UNION ALL
SELECT 'Reports.CallHistoryReportHour', 'CallHistoryReportHour', 'Supervisor', 'Hour when scheduled call history report must be sent.', 1, 0, '0'
UNION ALL
SELECT 'Reports.CallHistoryReportRecepients', 'CallHistoryReportRecepients', 'Supervisor', 'Email address(es) to send scheduled call history report.', 2, 0, NULL
UNION ALL
SELECT 'Reports.CallHistoryReportCallHistoryRowsLimit', 'CallHistoryReportCallHistoryRowsLimit', 'Supervisor', 'Limit for call history data rows exported.', 1, 0, '1000000'
UNION ALL
SELECT 'Reports.CallHistoryReportInterviewerBreaksRowsLimit', 'CallHistoryReportInterviewerBreaksRowsLimit', 'Supervisor', 'Limit for interviewer breaks data rows exported.', 1, 0, '100000'
UNION ALL
SELECT 'Reports.SurveyOverviewReportEnabled', 'SurveyOverviewReportEnabled', 'Supervisor', 'Is scheduled survey overview report enabled?', 3, 0, 'False'
UNION ALL
SELECT 'Reports.SurveyOverviewReportHour', 'SurveyOverviewReportHour', 'Supervisor', 'Hour when scheduled survey overview report must be sent.', 1, 0, '0'
UNION ALL
SELECT 'Reports.SurveyOverviewReportRecepients', 'SurveyOverviewReportRecepients', 'Supervisor', 'Email address(es) to send scheduled survey overview report.', 2, 0, NULL
UNION ALL
SELECT 'Reports.SurveyProductivityReportEnabled', 'SurveyProductivityReportEnabled', 'Supervisor', 'Is scheduled survey productivity report enabled?', 3, 0, 'False'
UNION ALL
SELECT 'Reports.SurveyProductivityReportHour', 'SurveyProductivityReportHour', 'Supervisor', 'Hour when scheduled survey productivity report must be sent.', 1, 0, '0'
UNION ALL
SELECT 'Reports.SurveyProductivityReportRecepients', 'SurveyProductivityReportRecepients', 'Supervisor', 'Email address(es) to send scheduled survey productivity report.', 2, 0, NULL
UNION ALL
SELECT 'Reports.InterviewerProductivityReportEnabled', 'InterviewerProductivityReportEnabled', 'Supervisor', 'Is scheduled interviewer productivity report enabled?', 3, 0, 'False'
UNION ALL
SELECT 'Reports.InterviewerProductivityReportHour', 'InterviewerProductivityReportHour', 'Supervisor', 'Hour when scheduled interviewer productivity report must be sent.', 1, 0, '0'
UNION ALL
SELECT 'Reports.InterviewerProductivityReportRecepients', 'InterviewerProductivityReportRecepients', 'Supervisor', 'Email address(es) to send scheduled interviewer productivity report.', 2, 0, NULL
UNION ALL
SELECT 'Monitoring.LaunchFileAllowedTimeLifeInHours', 'LaunchFileAllowedTimeLifeInHours', 'Supervisor', 'Launch file allowed time life in hours.', 1, 0, '2'
)	
INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
	SELECT * FROM Data
GO


PRINT 'Loading initial RoleID to BvRole...'
GO

INSERT INTO BvRole VALUES( 0x0000 , 'System' )
INSERT INTO BvRole VALUES( 0x0001 , 'Supervisor' )
INSERT INTO BvRole VALUES( 0x0002 , 'Interviewer' )
INSERT INTO BvRole VALUES( 0x0004 , 'Coder' )
INSERT INTO BvRole VALUES( 0x0008 , 'Consultant' )
INSERT INTO BvRole VALUES( 0x0010 , 'Key-entry Clerk' )
INSERT INTO BvRole VALUES( 0x0020 , 'Web-respondents' )
INSERT INTO BvRole VALUES( 0x0040 , 'CAPI Intervievers' )
GO

PRINT 'Loading initial states to BvState...'
GO
INSERT INTO BvState (StateID, Name, Priority) VALUES( 1, 'Appointment', 1000 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 2, 'Busy', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 3, 'No reply', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 4, 'Quota failure', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 5, 'Refusal', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 6, 'Terminated', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 7, 'Answer phone', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 8, 'Modem', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 9, 'Fax', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 10, 'Congestion', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 11, 'Unobtainable', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 12, 'Nuisance', 1 )
INSERT INTO BvState (StateID, Name, Priority, DA) VALUES( 13, 'Completed', 1, 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 14, 'Screened', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 15, 'Returned not dialled', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 16, 'Fresh sample', 1 )
INSERT INTO BvState (StateID, Name, Priority, DA) VALUES( 17, 'Blacklist', 1, 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 18, 'Not automatically dialled (ie manual dialling)', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 19, 'Status not sensed', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 20, 'Transfer to Web', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 21, 'Transfer to CATI', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 22, 'Transfer to CAPI', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 23, 'Transfer to IVR', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 24, 'Interrupted by interviewer', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 25, 'Returned dialler expired', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 26, 'Interrupted by system', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 27, 'Filtered by call delivery', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 28, 'Stopped', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 29, 'Telephony failure', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 30, 'Error', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 31, 'Custom1', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 32, 'Custom2', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 33, 'Custom3', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 34, 'Custom4', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 35, 'Custom5', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 36, 'Custom6', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 37, 'Custom7', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 38, 'Custom8', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 39, 'Custom9', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 40, 'Custom10', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 41, 'Custom11', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 42, 'Custom12', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 43, 'Custom13', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 44, 'Custom14', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 45, 'Custom15', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 46, 'Custom16', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 47, 'Custom17', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 48, 'Custom18', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 49, 'Custom19', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 50, 'Custom20', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 51, 'Custom21', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 52, 'Custom22', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 53, 'Custom23', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 54, 'Custom24', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 55, 'Custom25', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 56, 'Custom26', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 57, 'Custom27', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 58, 'Custom28', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 59, 'Custom29', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 60, 'Custom30', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 61, 'Custom31', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 62, 'Custom32', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 63, 'Custom33', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 64, 'Custom34', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 65, 'Custom35', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 66, 'Custom36', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 67, 'Custom37', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 68, 'Custom38', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 69, 'Custom39', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 70, 'Custom40', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 71, 'Custom41', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 72, 'Custom42', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 73, 'Custom43', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 74, 'Custom44', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 75, 'Custom45', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 76, 'Custom46', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 77, 'Custom47', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 78, 'Custom48', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 79, 'Custom49', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 80, 'Custom50', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 81, 'Custom51', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 82, 'Custom52', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 83, 'Custom53', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 84, 'Custom54', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 85, 'Custom55', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 86, 'Custom56', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 87, 'Custom57', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 88, 'Custom58', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 89, 'Custom59', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 90, 'Custom60', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 91, 'Custom61', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 92, 'Custom62', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 93, 'Custom63', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 94, 'Custom64', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 95, 'Custom65', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 96, 'Custom66', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 97, 'Custom67', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 98, 'Custom68', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 99, 'Custom69', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 100, 'Custom70', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 101, 'Custom71', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 102, 'Custom72', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 103, 'Custom73', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 104, 'Custom74', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 105, 'Custom75', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 106, 'Custom76', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 107, 'Custom77', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 108, 'Custom78', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 109, 'Custom79', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 110, 'Custom80', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 111, 'Custom81', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 112, 'Custom82', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 113, 'Custom83', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 114, 'Custom84', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 115, 'Custom85', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 116, 'Custom86', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 117, 'Custom87', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 118, 'Custom88', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 119, 'Custom89', 1 )
INSERT INTO BvState (StateID, Name, Priority) VALUES( 120, 'Custom90', 1 )
GO

PRINT 'Loading initial timezones...'
GO

exec BvSpTimezoneMaster_Insert 1, '(GMT) Greenwich Mean Time : Dublin, Edinburgh, Lisbon, London', 0, 2, 'GMT Standard Time', '2000-10-05 02:00:00.000', 0, 0, 'GMT Daylight Time', '2000-03-05 01:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 2, '(GMT) Monrovia, Reykjavik', 0, 1, 'Greenwich Standard Time', NULL, NULL, 0, 'Greenwich Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 3, '(GMT+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna', -60, 2, 'W. Europe Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'W. Europe Daylight Time', '2000-03-05 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 4, '(GMT+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague', -60, 2, 'Central Europe Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'Central Europe Daylight Time', '2000-03-05 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 5, '(GMT+01:00) Brussels, Copenhagen, Madrid, Paris', -60, 2, 'Romance Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'Romance Daylight Time', '2000-03-05 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 6, '(GMT+01:00) Sarajevo, Skopje, Warsaw, Zagreb', -60, 2, 'Central European Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'Central European Daylight Time', '2000-03-05 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 7, '(GMT+01:00) West Central Africa', -60, 1, 'W. Central Africa Standard Time', NULL, NULL, 0, 'W. Central Africa Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 8, '(GMT+02:00) Athens, Bucharest', -120, 2, 'GTB Standard Time', '2000-10-05 04:00:00.000', 0, 0, 'GTB Daylight Time', '2000-03-05 03:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 9, '(GMT+02:00) Nicosia', -120, 2, 'E. Europe Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'E. Europe Daylight Time', '2000-03-05 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 10, '(GMT+02:00) Cairo', -120, 1, 'Egypt Standard Time', NULL, NULL, 0, 'Egypt Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 11, '(GMT+02:00) Harare, Pretoria', -120, 1, 'South Africa Standard Time', NULL, NULL, 0, 'South Africa Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 12, '(GMT+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius', -120, 2, 'FLE Standard Time', '2000-10-05 04:00:00.000', 0, 0, 'FLE Daylight Time', '2000-03-05 03:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 13, '(GMT+02:00) Jerusalem', -120, 2, 'Jerusalem Standard Time', '2000-09-04 02:00:00.000', 0, 0, 'Jerusalem Daylight Time', '2000-03-05 02:00:00.000', 5, -60
exec BvSpTimezoneMaster_Insert 14, '(GMT+03:00) Baghdad', -180, 1, 'Arabic Standard Time', NULL, NULL, 0, 'Arabic Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 15, '(GMT+03:00) Kuwait, Riyadh', -180, 1, 'Arab Standard Time', NULL, NULL, 0, 'Arab Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 16, '(GMT+04:00) Moscow, St. Petersburg, Volgograd', -240, 1, 'Russian Standard Time', NULL, NULL, 0, 'Russian Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 17, '(GMT+03:00) Nairobi', -180, 1, 'E. Africa Standard Time', NULL, NULL, 0, 'E. Africa Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 18, '(GMT+03:30) Tehran', -210, 2, 'Iran Standard Time', '2000-09-03 23:59:00.000', 1, 0, 'Iran Daylight Time', '2000-03-03 23:59:00.000', 6, -60
exec BvSpTimezoneMaster_Insert 19, '(GMT+04:00) Abu Dhabi, Muscat', -240, 1, 'Arabian Standard Time', NULL, NULL, 0, 'Arabian Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 20, '(GMT+04:00) Yerevan', -240, 2, 'Caucasus Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'Caucasus Daylight Time', '2000-03-05 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 21, '(GMT+04:30) Kabul', -270, 1, 'Afghanistan Standard Time', NULL, NULL, 0, 'Afghanistan Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 22, '(GMT+06:00) Ekaterinburg', -360, 1, 'Ekaterinburg Standard Time', NULL, NULL, 0, 'Ekaterinburg Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 23, '(GMT+05:00) Tashkent', -300, 1, 'West Asia Standard Time', NULL, NULL, 0, 'West Asia Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 24, '(GMT+05:30) Chennai, Kolkata, Mumbai, New Delhi', -330, 1, 'India Standard Time', NULL, NULL, 0, 'India Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 25, '(GMT+05:45) Kathmandu', -345, 1, 'Nepal Standard Time', NULL, NULL, 0, 'Nepal Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 26, '(GMT+07:00) Novosibirsk', -420, 1, 'N. Central Asia Standard Time', NULL, NULL, 0, 'N. Central Asia Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 27, '(GMT+06:00) Astana', -360, 1, 'Central Asia Standard Time', NULL, NULL, 0, 'Central Asia Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 28, '(GMT+05:30) Sri Jayawardenepura', -330, 1, 'Sri Lanka Standard Time', NULL, NULL, 0, 'Sri Lanka Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 29, '(GMT+06:30) Yangon (Rangoon)', -390, 1, 'Myanmar Standard Time', NULL, NULL, 0, 'Myanmar Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 30, '(GMT+07:00) Bangkok, Hanoi, Jakarta', -420, 1, 'SE Asia Standard Time', NULL, NULL, 0, 'SE Asia Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 31, '(GMT+08:00) Krasnoyarsk', -480, 1, 'North Asia Standard Time', NULL, NULL, 0, 'North Asia Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 32, '(GMT+08:00) Beijing, Chongqing, Hong Kong, Urumqi', -480, 1, 'China Standard Time', NULL, NULL, 0, 'China Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 33, '(GMT+09:00) Irkutsk', -540, 1, 'North Asia East Standard Time', NULL, NULL, 0, 'North Asia East Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 34, '(GMT+08:00) Kuala Lumpur, Singapore', -480, 1, 'Malay Peninsula Standard Time', NULL, NULL, 0, 'Malay Peninsula Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 35, '(GMT+08:00) Perth', -480, 1, 'W. Australia Standard Time', NULL, NULL, 0, 'W. Australia Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 36, '(GMT+08:00) Taipei', -480, 1, 'Taipei Standard Time', NULL, NULL, 0, 'Taipei Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 37, '(GMT+09:00) Osaka, Sapporo, Tokyo', -540, 1, 'Tokyo Standard Time', NULL, NULL, 0, 'Tokyo Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 38, '(GMT+09:00) Seoul', -540, 1, 'Korea Standard Time', NULL, NULL, 0, 'Korea Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 39, '(GMT+10:00) Yakutsk', -600, 1, 'Yakutsk Standard Time', NULL, NULL, 0, 'Yakutsk Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 40, '(GMT+09:30) Adelaide', -570, 2, 'Cen. Australia Standard Time', '2000-04-01 03:00:00.000', 0, 0, 'Cen. Australia Daylight Time', '2000-10-01 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 41, '(GMT+09:30) Darwin', -570, 1, 'AUS Central Standard Time', NULL, NULL, 0, 'AUS Central Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 42, '(GMT+10:00) Brisbane', -600, 1, 'E. Australia Standard Time', NULL, NULL, 0, 'E. Australia Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 43, '(GMT+10:00) Canberra, Melbourne, Sydney', -600, 2, 'AUS Eastern Standard Time', '2000-04-01 03:00:00.000', 0, 0, 'AUS Eastern Daylight Time', '2000-10-01 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 44, '(GMT+10:00) Guam, Port Moresby', -600, 1, 'West Pacific Standard Time', NULL, NULL, 0, 'West Pacific Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 45, '(GMT+10:00) Hobart', -600, 2, 'Tasmania Standard Time', '2000-04-01 03:00:00.000', 0, 0, 'Tasmania Daylight Time', '2000-10-01 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 46, '(GMT+11:00) Vladivostok', -660, 1, 'Vladivostok Standard Time', NULL, NULL, 0, 'Vladivostok Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 47, '(GMT+11:00) Solomon Is., New Caledonia', -660, 1, 'Central Pacific Standard Time', NULL, NULL, 0, 'Central Pacific Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 48, '(GMT+12:00) Auckland, Wellington', -720, 2, 'New Zealand Standard Time', '2000-04-01 03:00:00.000', 0, 0, 'New Zealand Daylight Time', '2000-09-05 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 49, '(GMT+12:00) Fiji', -720, 2, 'Fiji Standard Time', '2000-01-04 03:00:00.000', 0, 0, 'Fiji Daylight Time', '2000-10-04 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 50, '(GMT+13:00) Nuku''alofa', -780, 1, 'Tonga Standard Time', NULL, NULL, 0, 'Tonga Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 51, '(GMT-01:00) Azores', 60, 2, 'Azores Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'Azores Daylight Time', '2000-03-05 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 52, '(GMT-01:00) Cape Verde Is.', 60, 1, 'Cape Verde Standard Time', NULL, NULL, 0, 'Cape Verde Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 53, '(GMT-02:00) Mid-Atlantic', 120, 2, 'Mid-Atlantic Standard Time', '2000-09-05 02:00:00.000', 0, 0, 'Mid-Atlantic Daylight Time', '2000-03-05 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 54, '(GMT-03:00) Brasilia', 180, 2, 'E. South America Standard Time', '2000-02-04 23:59:00.000', 6, 0, 'E. South America Daylight Time', '2000-10-03 23:59:00.000', 6, -60
exec BvSpTimezoneMaster_Insert 55, '(GMT-03:00) Cayenne, Fortaleza', 180, 1, 'SA Eastern Standard Time', NULL, NULL, 0, 'SA Eastern Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 56, '(GMT-03:00) Greenland', 180, 2, 'Greenland Standard Time', '2000-10-05 23:00:00.000', 6, 0, 'Greenland Daylight Time', '2000-03-04 22:00:00.000', 6, -60
exec BvSpTimezoneMaster_Insert 57, '(GMT-03:30) Newfoundland', 210, 2, 'Newfoundland Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Newfoundland Daylight Time', '2000-03-02 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 58, '(GMT-04:00) Atlantic Time (Canada)', 240, 2, 'Atlantic Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Atlantic Daylight Time', '2000-03-02 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 59, '(GMT-04:00) Georgetown, La Paz, Manaus, San Juan', 240, 1, 'SA Western Standard Time', NULL, NULL, 0, 'SA Western Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 60, '(GMT-04:00) Santiago', 240, 2, 'Pacific SA Standard Time', '2000-03-02 23:59:00.000', 6, 0, 'Pacific SA Daylight Time', '2000-10-02 23:59:00.000', 6, -60
exec BvSpTimezoneMaster_Insert 61, '(GMT-05:00) Bogota, Lima, Quito', 300, 1, 'SA Pacific Standard Time', NULL, NULL, 0, 'SA Pacific Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 62, '(GMT-05:00) Eastern Time (US & Canada)', 300, 2, 'Eastern Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Eastern Daylight Time', '2000-03-02 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 63, '(GMT-05:00) Indiana (East)', 300, 2, 'US Eastern Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'US Eastern Daylight Time', '2000-03-02 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 64, '(GMT-06:00) Central America', 360, 1, 'Central America Standard Time', NULL, NULL, 0, 'Central America Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 65, '(GMT-06:00) Central Time (US & Canada)', 360, 2, 'Central Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Central Daylight Time', '2000-03-02 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 66, '(GMT-06:00) Guadalajara, Mexico City, Monterrey', 360, 2, 'Mexico Standard Time', '2000-10-05 02:00:00.000', 0, 0, 'Mexico Daylight Time', '2000-04-01 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 67, '(GMT-06:00) Saskatchewan', 360, 1, 'Canada Central Standard Time', NULL, NULL, 0, 'Canada Central Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 68, '(GMT-07:00) Arizona', 420, 1, 'US Mountain Standard Time', NULL, NULL, 0, 'US Mountain Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 69, '(GMT-07:00) Chihuahua, La Paz, Mazatlan', 420, 2, 'Mexico Standard Time 2', '2000-10-05 02:00:00.000', 0, 0, 'Mexico Daylight Time 2', '2000-04-01 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 70, '(GMT-07:00) Mountain Time (US & Canada)', 420, 2, 'Mountain Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Mountain Daylight Time', '2000-03-02 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 71, '(GMT-08:00) Pacific Time (US & Canada)', 480, 2, 'Pacific Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Pacific Daylight Time', '2000-03-02 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 72, '(GMT-09:00) Alaska', 540, 2, 'Alaskan Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Alaskan Daylight Time', '2000-03-02 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 73, '(GMT-10:00) Hawaii', 600, 1, 'Hawaiian Standard Time', NULL, NULL, 0, 'Hawaiian Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 74, '(GMT+13:00) Samoa', -780, 2, 'Samoa Standard Time', '2000-04-01 01:00:00.000', 0, 0, 'Samoa Daylight Time', '2000-09-05 00:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 75, '(GMT-12:00) International Date Line West', 720, 1, 'Dateline Standard Time', NULL, NULL, 0, 'Dateline Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 76, '(GMT-03:00) Buenos Aires', 180, 1, 'Argentina Standard Time', NULL, NULL, 0, 'Argentina Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 77, '(GMT+04:00) Baku', -240, 2, 'Azerbaijan Standard Time', '2000-10-05 05:00:00.000', 0, 0, 'Azerbaijan Daylight Time', '2000-03-05 04:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 78, '(GMT+06:00) Dhaka', -360, 1, 'Bangladesh Standard Time', NULL, NULL, 0, 'Bangladesh Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 79, '(GMT-04:00) Cuiaba', 240, 2, 'Central Brazilian Standard Time', '2000-02-04 23:59:00.000', 6, 0, 'Central Brazilian Daylight Time', '2000-10-03 23:59:00.000', 6, -60
exec BvSpTimezoneMaster_Insert 80, '(GMT-06:00) Guadalajara, Mexico City, Monterrey', 360, 2, 'Central Standard Time (Mexico)', '2000-10-05 02:00:00.000', 0, 0, 'Central Daylight Time (Mexico)', '2000-04-01 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 81, '(GMT) Coordinated Universal Time', 0, 1, 'Coordinated Universal Time', NULL, NULL, 0, 'Coordinated Universal Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 82, '(GMT+04:00) Tbilisi', -240, 1, 'Georgian Standard Time', NULL, NULL, 0, 'Georgian Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 83, '(GMT+02:00) Amman', -120, 2, 'Jordan Standard Time', '2000-10-05 01:00:00.000', 5, 0, 'Jordan Daylight Time', '2000-03-05 23:59:00.000', 4, -60
exec BvSpTimezoneMaster_Insert 84, '(GMT+03:00) Kaliningrad, Minsk', -180, 1, 'Kaliningrad Standard Time', NULL, NULL, 0, 'Kaliningrad Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 85, '(GMT+12:00) Petropavlovsk-Kamchatsky - Old', -720, 2, 'Kamchatka Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'Kamchatka Daylight Time', '2000-03-05 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 86, '(GMT+12:00) Magadan', -720, 1, 'Magadan Standard Time', NULL, NULL, 0, 'Magadan Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 87, '(GMT+04:00) Port Louis', -240, 1, 'Mauritius Standard Time', NULL, NULL, 0, 'Mauritius Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 88, '(GMT+02:00) Beirut', -120, 2, 'Middle East Standard Time', '2000-10-05 23:59:00.000', 6, 0, 'Middle East Daylight Time', '2000-03-04 23:59:00.000', 6, -60
exec BvSpTimezoneMaster_Insert 89, '(GMT-03:00) Montevideo', 180, 2, 'Montevideo Standard Time', '2000-03-02 02:00:00.000', 0, 0, 'Montevideo Daylight Time', '2000-10-01 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 90, '(GMT) Casablanca', 0, 1, 'Morocco Standard Time', NULL, NULL, 0, 'Morocco Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 91, '(GMT-07:00) Chihuahua, La Paz, Mazatlan', 420, 2, 'Mountain Standard Time (Mexico)', '2000-10-05 02:00:00.000', 0, 0, 'Mountain Daylight Time (Mexico)', '2000-04-01 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 92, '(GMT+01:00) Windhoek', -60, 2, 'Namibia Standard Time', '2000-04-01 02:00:00.000', 0, 0, 'Namibia Daylight Time', '2000-09-01 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 93, '(GMT-08:00) Baja California', 480, 2, 'Pacific Standard Time (Mexico)', '2000-10-05 02:00:00.000', 0, 0, 'Pacific Daylight Time (Mexico)', '2000-04-01 02:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 94, '(GMT+05:00) Islamabad, Karachi', -300, 1, 'Pakistan Standard Time', NULL, NULL, 0, 'Pakistan Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 95, '(GMT-04:00) Asuncion', 240, 2, 'Paraguay Standard Time', '2000-04-01 23:59:00.000', 6, 0, 'Paraguay Daylight Time', '2000-10-01 23:59:00.000', 6, -60
exec BvSpTimezoneMaster_Insert 96, '(GMT+02:00) Damascus', -120, 2, 'Syria Standard Time', '2000-10-05 23:59:00.000', 4, 0, 'Syria Daylight Time', '2000-04-01 23:59:00.000', 4, -60
exec BvSpTimezoneMaster_Insert 97, '(GMT+02:00) Istanbul', -120, 2, 'Turkey Standard Time', '2000-10-05 04:00:00.000', 0, 0, 'Turkey Daylight Time', '2000-03-05 03:00:00.000', 0, -60
exec BvSpTimezoneMaster_Insert 98, '(GMT+08:00) Ulaanbaatar', -480, 1, 'Ulaanbaatar Standard Time', NULL, NULL, 0, 'Ulaanbaatar Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 99, '(GMT+12:00) Coordinated Universal Time+12', -720, 1, 'UTC+12', NULL, NULL, 0, 'UTC+12', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 100, '(GMT-02:00) Coordinated Universal Time-02', 120, 1, 'UTC-02', NULL, NULL, 0, 'UTC-02', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 101, '(GMT-11:00) Coordinated Universal Time-11', 660, 1, 'UTC-11', NULL, NULL, 0, 'UTC-11', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 102, '(GMT-04:30) Caracas', 270, 1, 'Venezuela Standard Time', NULL, NULL, 0, 'Venezuela Daylight Time', NULL, NULL, -60
exec BvSpTimezoneMaster_Insert 103, '(GMT-03:00) Salvador', 180, 2, 'Bahia Standard Time', '2000-02-04 23:59:00.000', 6, 0, 'Bahia Daylight Time', '2000-10-03 23:59:00.000', 6, -60


exec BvSpTimezone_Activate 1/*GMT*/

PRINT 'Loading initial site...'
DECLARE @SID int

SELECT @SID = 1

/*SELECT 'Loading initial questionnaires'*/

PRINT 'Loading initial containers...'

DECLARE @SiteSID int       SELECT @SiteSID      =  @SID  SELECT @SID = @SID + 1
DECLARE @SurveysSID int    SELECT @SurveysSID   =  @SID  SELECT @SID = @SID + 1
DECLARE @ResourcesSID int  SELECT @ResourcesSID =  @SID  SELECT @SID = @SID + 1
DECLARE @PersonsSID int    SELECT @PersonsSID   =  @SID  SELECT @SID = @SID + 1
DECLARE @QnairesSID int    SELECT @QnairesSID   =  @SID  SELECT @SID = @SID + 1
DECLARE @ServersSID int    SELECT @ServersSID   =  @SID  SELECT @SID = @SID + 1
DECLARE @ServicesSID int   SELECT @ServicesSID  =  @SID  SELECT @SID = @SID + 1
DECLARE @SServicesSID int  SELECT @SServicesSID =  @SID  SELECT @SID = @SID + 1
DECLARE @DServicesSID int  SELECT @DServicesSID =  @SID  SELECT @SID = @SID + 1

DECLARE @Group1SID int     SELECT @Group1SID    =  @SID  SELECT @SID = @SID + 1
DECLARE @Group2SID int     SELECT @Group2SID    =  @SID  SELECT @SID = @SID + 1
DECLARE @Group3SID int     SELECT @Group3SID    =  @SID  SELECT @SID = @SID + 1
DECLARE @SuperSID int      SELECT @SuperSID     =  @SID  SELECT @SID = @SID + 1
DECLARE @InterSID int      SELECT @InterSID     =  @SID  SELECT @SID = @SID + 1
DECLARE @CoderSID int      SELECT @CoderSID     =  @SID  SELECT @SID = @SID + 1
DECLARE @KeyEntrySID int   SELECT @KeyEntrySID  =  @SID  SELECT @SID = @SID + 1
DECLARE @WebRespSID int    SELECT @WebRespSID   =  @SID  SELECT @SID = @SID + 1
DECLARE @CAPISID int       SELECT @CAPISID      =  @SID  SELECT @SID = @SID + 1
DECLARE @Custom1SID int    SELECT @Custom1SID   =  @SID  SELECT @SID = @SID + 1
DECLARE @Custom2SID int    SELECT @Custom2SID   =  @SID  SELECT @SID = @SID + 1
DECLARE @Custom3SID int    SELECT @Custom3SID   =  @SID  SELECT @SID = @SID + 1
DECLARE @AutoSID int       SELECT @AutoSID      =  @SID  SELECT @SID = @SID + 1
DECLARE @SchedScrSID int   SELECT @SchedScrSID  =  @SID  SELECT @SID = @SID + 1
DECLARE @SampleScrSID int  SELECT @SampleScrSID =  @SID  SELECT @SID = @SID + 1
DECLARE @LibraryScrSID int SELECT @LibraryScrSID = @SID  SELECT @SID = @SID + 1
DECLARE @StateGroupRootSID int SELECT @StateGroupRootSID = @SID SELECT @SID = @SID + 1
DECLARE @StateGroupSID int SELECT @StateGroupSID = @SID SELECT @SID = @SID + 1
DECLARE @TCISID int        SELECT @TCISID       =  @SID  SELECT @SID = @SID + 1
DECLARE @DialersSID int    SELECT @DialersSID   =  @SID  SELECT @SID = @SID + 1
DECLARE @GarbageSID int    SELECT @GarbageSID   =  @SID  SELECT @SID = @SID + 1
DECLARE @GateSID int       SELECT @GateSID      =  @SID  SELECT @SID = @SID + 1
DECLARE @SServerSID int    SELECT @SServerSID   =  @SID  SELECT @SID = @SID + 1

PRINT 'Update BvState table'
UPDATE BvState SET StateGroupID = @StateGroupSID

PRINT 'Load BvThresholdITS table'
INSERT INTO BvThresholdITS ( SurveySID, ITS ) 
        SELECT 0, StateID FROM BvState WHERE StateGroupID = @StateGroupSID

PRINT 'Insert default state group'
INSERT INTO BvStateGroup( [ID], [Name], [Order], Deleted  ) VALUES( @StateGroupSID, 'Default group', 1, 0 )


PRINT 'Loading information about person groups...'

insert into BvPersonGroup(SID, Name, Description, RoleID, ManualSelection)
values(@InterSID,   'CATI Interviewers', '', 0x0002, 0)

insert into BvPersonGroup(SID, Name, Description, RoleID, ManualSelection)
values(@CAPISID,   'CAPI interviewers',   '', 0x0040, 1)

PRINT 'Loading initial SID...'

INSERT INTO BvSIDCounter( SID )
VALUES( @SID )

DECLARE @AllHoursID INT
EXEC @AllHoursID = BvSpGetNewSID

EXEC BvSpSchedule_Insert @AllHoursID, 'All hours', '<?xml version="1.0"?>
<Schedule xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
    <Id xsi:nil="true" />
    <Name />
    <Rules>
        <Rule>
            <Id>13c49088-ad96-476b-a6b4-b09ddf873ae1</Id>
            <Description />
            <SubRules>
                <SubRule>
                    <Id>ff32c0f5-5d1e-4726-9de7-ea95dc99c3ed</Id>
                    <ItsId>1</ItsId>
                    <ShiftTypeId>0</ShiftTypeId>
                    <Filter />
                    <FilterEnabled>false</FilterEnabled>
                    <Description />
                    <SubRuleActions>
                        <SubRuleAction>
                            <Id>1</Id>
                            <ActionId>27</ActionId>
                            <Filter />
                            <Enabled>true</Enabled>
                            <Description />
                            <ParameterValue>1000</ParameterValue>
                            <FilterEnabled>false</FilterEnabled>
                        </SubRuleAction>
                        <SubRuleAction>
                            <Id>2</Id>
                            <ActionId>5</ActionId>
                            <Filter />
                            <Enabled>true</Enabled>
                            <Description />
                            <ParameterValue>0</ParameterValue>
                            <FilterEnabled>false</FilterEnabled>
                        </SubRuleAction>
                    </SubRuleActions>
                </SubRule>
                <SubRule>
                    <Id>6cdc0632-15a2-4f4a-baf9-328eb9bb3b31</Id>
                    <ItsId>2</ItsId>
                    <ShiftTypeId>0</ShiftTypeId>
                    <Filter />
                    <FilterEnabled>false</FilterEnabled>
                    <Description />
                    <SubRuleActions>
                        <SubRuleAction>
                            <Id>1</Id>
                            <ActionId>2</ActionId>
                            <Filter />
                            <Enabled>true</Enabled>
                            <Description />
                            <ParameterValue>15</ParameterValue>
                            <FilterEnabled>false</FilterEnabled>
                        </SubRuleAction>
                    </SubRuleActions>
                </SubRule>
                <SubRule>
                    <Id>14aa9b11-236d-4473-8043-3557f9853c86</Id>
                    <ItsId>3</ItsId>
                    <ShiftTypeId>0</ShiftTypeId>
                    <Filter />
                    <FilterEnabled>false</FilterEnabled>
                    <Description />
                    <SubRuleActions>
                        <SubRuleAction>
                            <Id>1</Id>
                            <ActionId>3</ActionId>
                            <Filter />
                            <Enabled>true</Enabled>
                            <Description />
                            <ParameterValue>1</ParameterValue>
                            <FilterEnabled>false</FilterEnabled>
                        </SubRuleAction>
                    </SubRuleActions>
                </SubRule>
                <SubRule>
                    <Id>0d2081bd-80cd-4a0e-b3c3-70533863a712</Id>
                    <ItsId>16</ItsId>
                    <ShiftTypeId>0</ShiftTypeId>
                    <Filter />
                    <FilterEnabled>false</FilterEnabled>
                    <Description />
                    <SubRuleActions>
                        <SubRuleAction>
                            <Id>1</Id>
                            <ActionId>8</ActionId>
                            <Filter />
                            <Enabled>true</Enabled>
                            <Description />
                            <ParameterValue>0</ParameterValue>
                            <FilterEnabled>false</FilterEnabled>
                        </SubRuleAction>
                    </SubRuleActions>
                </SubRule>
            </SubRules>
        </Rule>
    </Rules>
    <ShiftTypes>
        <ShiftType>
            <Id>1</Id>
            <Name>Default</Name>
            <ColorInt>-16751616</ColorInt>
        </ShiftType>
    </ShiftTypes>
    <Shifts>
        <Shift>
            <Id>1</Id>
            <ShiftTypeId>1</ShiftTypeId>
            <Timezones>
                <Timezone>
                    <Id xsi:nil="true" />
                    <Data>
                        <StartDayOfWeek>Monday</StartDayOfWeek>
                        <StartTime>00:00:00</StartTime>
                        <EndDayOfWeek>Tuesday</EndDayOfWeek>
                        <EndTime>00:00:00</EndTime>
                    </Data>
                </Timezone>
            </Timezones>
        </Shift>
        <Shift>
            <Id>2</Id>
            <ShiftTypeId>1</ShiftTypeId>
            <Timezones>
                <Timezone>
                    <Id xsi:nil="true" />
                    <Data>
                        <StartDayOfWeek>Tuesday</StartDayOfWeek>
                        <StartTime>00:00:00</StartTime>
                        <EndDayOfWeek>Wednesday</EndDayOfWeek>
                        <EndTime>00:00:00</EndTime>
                    </Data>
                </Timezone>
            </Timezones>
        </Shift>
        <Shift>
            <Id>3</Id>
            <ShiftTypeId>1</ShiftTypeId>
            <Timezones>
                <Timezone>
                    <Id xsi:nil="true" />
                    <Data>
                        <StartDayOfWeek>Wednesday</StartDayOfWeek>
                        <StartTime>00:00:00</StartTime>
                        <EndDayOfWeek>Thursday</EndDayOfWeek>
                        <EndTime>00:00:00</EndTime>
                    </Data>
                </Timezone>
            </Timezones>
        </Shift>
        <Shift>
            <Id>4</Id>
            <ShiftTypeId>1</ShiftTypeId>
            <Timezones>
                <Timezone>
                    <Id xsi:nil="true" />
                    <Data>
                        <StartDayOfWeek>Thursday</StartDayOfWeek>
                        <StartTime>00:00:00</StartTime>
                        <EndDayOfWeek>Friday</EndDayOfWeek>
                        <EndTime>00:00:00</EndTime>
                    </Data>
                </Timezone>
            </Timezones>
        </Shift>
        <Shift>
            <Id>5</Id>
            <ShiftTypeId>1</ShiftTypeId>
            <Timezones>
                <Timezone>
                    <Id xsi:nil="true" />
                    <Data>
                        <StartDayOfWeek>Friday</StartDayOfWeek>
                        <StartTime>00:00:00</StartTime>
                        <EndDayOfWeek>Saturday</EndDayOfWeek>
                        <EndTime>00:00:00</EndTime>
                    </Data>
                </Timezone>
            </Timezones>
        </Shift>
        <Shift>
            <Id>6</Id>
            <ShiftTypeId>1</ShiftTypeId>
            <Timezones>
                <Timezone>
                    <Id xsi:nil="true" />
                    <Data>
                        <StartDayOfWeek>Saturday</StartDayOfWeek>
                        <StartTime>00:00:00</StartTime>
                        <EndDayOfWeek>Sunday</EndDayOfWeek>
                        <EndTime>00:00:00</EndTime>
                    </Data>
                </Timezone>
            </Timezones>
        </Shift>
        <Shift>
            <Id>7</Id>
            <ShiftTypeId>1</ShiftTypeId>
            <Timezones>
                <Timezone>
                    <Id xsi:nil="true" />
                    <Data>
                        <StartDayOfWeek>Sunday</StartDayOfWeek>
                        <StartTime>00:00:00</StartTime>
                        <EndDayOfWeek>Monday</EndDayOfWeek>
                        <EndTime>00:00:00</EndTime>
                    </Data>
                </Timezone>
            </Timezones>
        </Shift>
    </Shifts>
    <Exclusions />
    <CustomScript>
        <Id>1</Id>
        <LanguageName>JScript.Net</LanguageName>
        <Body />
    </CustomScript>
</Schedule>',
NULL, -- ScriptSource
NULL  -- DesignStateGroupId

PRINT 'Loading information about reports...'
GO

INSERT INTO BvReport VALUES (2,  'Sample Status Summary',   'SampleStatusSummary.rpt',      'bv7rptu.dll')
INSERT INTO BvReport VALUES (2,  'Sample Disposition',      'SampleDisposition.rpt',      'bv7prodrptu.dll')
INSERT INTO BvReport VALUES (2,  'Survey/Person',           'SurveyPersonReport.rpt',       'bv7rptu.dll')
INSERT INTO BvReport VALUES (2,  'Production By Interviewer',   'SurveyProductionByInterviewer.rpt', 'bv7prodrptu.dll')
INSERT INTO BvReport VALUES (2,  'Production Details',      'ProductionDetails.rpt',      'bv7prodrptu.dll')
INSERT INTO BvReport VALUES (2,  'Time Outcome',         'SurveyTimeOutcome.rpt',      'bv7prodrptu.dll')
INSERT INTO BvReport VALUES (2,  'Summary of Openends',     'Summary_of_Openends.rpt',      'bv7rptu.dll')
INSERT INTO BvReport VALUES (10, 'Survey/Person',           'SurveyPersonReport.rpt',       'bv7rptu.dll')
INSERT INTO BvReport VALUES (10, 'Person Production',    'InterviewerProduction.rpt',    'bv7prodrptu.dll')
INSERT INTO BvReport VALUES (10, 'Time Outcome',         'PersonTimeOutcome.rpt',      'bv7prodrptu.dll')
INSERT INTO BvReport VALUES (12, 'Production Summary',      'ProductionSummary.rpt',      'bv7prodrptu.dll')
INSERT INTO BvReport VALUES (12, 'Surveys By Outcome',      'SurveysByOutcome.rpt',       'bv7prodrptu.dll')
GO

PRINT 'Loading information about RD property'
GO

GO

-- insert 0 
INSERT INTO BvTransferBatches VALUES( 0 )
GO

-------------------------------------------------------------------------------
-- UPDATE FOR PERSON GROUPS
-------------------------------------------------------------------------------
declare cr cursor local for select SID from BvPersonGroup
declare @sid int

    open cr

    fetch next from cr into @sid
    while ( @@FETCH_STATUS = 0 )
    begin
        exec BvSpPerson_SpinUp @sid
        fetch next from cr into @sid
    end

    close cr
    deallocate cr
GO

INSERT INTO [BvThresholdTypes] VALUES(1, 'Task alert')
INSERT INTO [BvThresholdTypes] VALUES(2, 'SurvayActivityView.InterviewersLoggedCount alert')
INSERT INTO [BvThresholdTypes] VALUES(3, 'SurvayActivityView.NextAppointmentTime alert')
INSERT INTO [BvThresholdTypes] VALUES(4, 'SurvayActivityView.TotalSampleSize alert')
INSERT INTO [BvThresholdTypes] VALUES(5, 'SurvayActivityView.ActiveCallsCount alert')
INSERT INTO [BvThresholdTypes] VALUES(6, 'SurvayActivityView.ScheduledCallsCount alert')
INSERT INTO [BvThresholdTypes] VALUES(7, 'SurvayActivityView.SuspendedCallsCount alert')
INSERT INTO [BvThresholdTypes] VALUES(8, 'SurvayActivityView.MinutesSpentWorkingOnSurvey alert')
INSERT INTO [BvThresholdTypes] VALUES(9, 'SurvayActivityView.AssignedInterviewersCount alert')
INSERT INTO [BvThresholdTypes] VALUES(10, 'SurvayActivityView.StrikeRate alert')
INSERT INTO [BvThresholdTypes] VALUES(11, 'SurvayActivityView.CountCalls alert')
INSERT INTO [BvThresholdTypes] VALUES(12, 'SystemWideInfo.LoggedInterviewersCount')
INSERT INTO [BvThresholdTypes] VALUES(13, 'SystemWideInfo.OpenSurveysCount')
INSERT INTO [BvThresholdTypes] VALUES(14, 'SystemWideInfo.CallsCount')
INSERT INTO [BvThresholdTypes] VALUES(15, 'AppointmentList alert')
INSERT INTO [BvThresholdTypes] VALUES(16, 'TasksAlert.LastKeepAliveTime alert')
INSERT INTO [BvThresholdTypes] VALUES(17, 'QuickAnswerSubmission alert')

GO

INSERT INTO [BvConfirmitStatus] VALUES( 'complete', 'Complete', 	13 )
INSERT INTO [BvConfirmitStatus] VALUES( 'screened', 'Screened', 	14 )
INSERT INTO [BvConfirmitStatus] VALUES( 'quotafull','Quota Full', 	4 )
INSERT INTO [BvConfirmitStatus] VALUES( 'error',    'Error', 	    30 )
INSERT INTO [BvConfirmitStatus] VALUES( NULL,       'Incomplete', 	1 )
INSERT INTO [BvConfirmitStatus] VALUES( '1', 'Appointment', 1 )
INSERT INTO [BvConfirmitStatus] VALUES( '2', 'Busy', 2 )
INSERT INTO [BvConfirmitStatus] VALUES( '3', 'No reply', 3 )
INSERT INTO [BvConfirmitStatus] VALUES( '4', 'Quota failure', 4 )
INSERT INTO [BvConfirmitStatus] VALUES( '5', 'Refusal', 5 )
INSERT INTO [BvConfirmitStatus] VALUES( '6', 'Terminated', 6 )
INSERT INTO [BvConfirmitStatus] VALUES( '7', 'Answer phone', 7 )
INSERT INTO [BvConfirmitStatus] VALUES( '8', 'Modem', 8 )
INSERT INTO [BvConfirmitStatus] VALUES( '9', 'Fax', 9 )
INSERT INTO [BvConfirmitStatus] VALUES( '10', 'Congestion', 10 )
INSERT INTO [BvConfirmitStatus] VALUES( '11', 'Unobtainable', 11 )
INSERT INTO [BvConfirmitStatus] VALUES( '12', 'Nuisance', 12 )
INSERT INTO [BvConfirmitStatus] VALUES( '13', 'Completed', 13 )
INSERT INTO [BvConfirmitStatus] VALUES( '14', 'Screened', 14 )
INSERT INTO [BvConfirmitStatus] VALUES( '15', 'Returned not dialled', 15 )
INSERT INTO [BvConfirmitStatus] VALUES( '16', 'Fresh sample', 16 )
INSERT INTO [BvConfirmitStatus] VALUES( '17', 'Blacklist', 17 )
INSERT INTO [BvConfirmitStatus] VALUES( '18', 'Not automatically dialled (ie manual dialling)', 18 )
INSERT INTO [BvConfirmitStatus] VALUES( '19', 'Status not sensed', 19 )
INSERT INTO [BvConfirmitStatus] VALUES( '20', 'Transfer to Web', 20 )
INSERT INTO [BvConfirmitStatus] VALUES( '21', 'Transfer to CATI', 21 )
INSERT INTO [BvConfirmitStatus] VALUES( '22', 'Transfer to CAPI', 22 )
INSERT INTO [BvConfirmitStatus] VALUES( '23', 'Transfer to IVR', 23 )
INSERT INTO [BvConfirmitStatus] VALUES( '24', 'Interrupted by interviewer', 24 )
INSERT INTO [BvConfirmitStatus] VALUES( '25', 'Returned dialler expired', 25 )
INSERT INTO [BvConfirmitStatus] VALUES( '26', 'Interrupted by system', 26 )
INSERT INTO [BvConfirmitStatus] VALUES( '27', 'Filtered by call delivery', 27 )
INSERT INTO [BvConfirmitStatus] VALUES( '28', 'Stopped', 28 )
INSERT INTO [BvConfirmitStatus] VALUES( '29', 'Telephony failure', 29 )
INSERT INTO [BvConfirmitStatus] VALUES( '30', 'Error', 30 )
INSERT INTO [BvConfirmitStatus] VALUES( '31', 'Custom1', 31 )
INSERT INTO [BvConfirmitStatus] VALUES( '32', 'Custom2', 32 )
INSERT INTO [BvConfirmitStatus] VALUES( '33', 'Custom3', 33 )
INSERT INTO [BvConfirmitStatus] VALUES( '34', 'Custom4', 34 )
INSERT INTO [BvConfirmitStatus] VALUES( '35', 'Custom5', 35 )
INSERT INTO [BvConfirmitStatus] VALUES( '36', 'Custom6', 36 )
INSERT INTO [BvConfirmitStatus] VALUES( '37', 'Custom7', 37 )
INSERT INTO [BvConfirmitStatus] VALUES( '38', 'Custom8', 38 )
INSERT INTO [BvConfirmitStatus] VALUES( '39', 'Custom9', 39 )
INSERT INTO [BvConfirmitStatus] VALUES( '40', 'Custom10', 40 )
INSERT INTO [BvConfirmitStatus] VALUES( '41', 'Custom11', 41 )
INSERT INTO [BvConfirmitStatus] VALUES( '42', 'Custom12', 42 )
INSERT INTO [BvConfirmitStatus] VALUES( '43', 'Custom13', 43 )
INSERT INTO [BvConfirmitStatus] VALUES( '44', 'Custom14', 44 )
INSERT INTO [BvConfirmitStatus] VALUES( '45', 'Custom15', 45 )
INSERT INTO [BvConfirmitStatus] VALUES( '46', 'Custom16', 46 )
INSERT INTO [BvConfirmitStatus] VALUES( '47', 'Custom17', 47 )
INSERT INTO [BvConfirmitStatus] VALUES( '48', 'Custom18', 48 )
INSERT INTO [BvConfirmitStatus] VALUES( '49', 'Custom19', 49 )
INSERT INTO [BvConfirmitStatus] VALUES( '50', 'Custom20', 50 )
INSERT INTO [BvConfirmitStatus] VALUES( '51', 'Custom21', 51 )
INSERT INTO [BvConfirmitStatus] VALUES( '52', 'Custom22', 52 )
INSERT INTO [BvConfirmitStatus] VALUES( '53', 'Custom23', 53 )
INSERT INTO [BvConfirmitStatus] VALUES( '54', 'Custom24', 54 )
INSERT INTO [BvConfirmitStatus] VALUES( '55', 'Custom25', 55 )
INSERT INTO [BvConfirmitStatus] VALUES( '56', 'Custom26', 56 )
INSERT INTO [BvConfirmitStatus] VALUES( '57', 'Custom27', 57 )
INSERT INTO [BvConfirmitStatus] VALUES( '58', 'Custom28', 58 )
INSERT INTO [BvConfirmitStatus] VALUES( '59', 'Custom29', 59 )
INSERT INTO [BvConfirmitStatus] VALUES( '60', 'Custom30', 60 )
INSERT INTO [BvConfirmitStatus] VALUES( '61', 'Custom31', 61 )
INSERT INTO [BvConfirmitStatus] VALUES( '62', 'Custom32', 62 )
INSERT INTO [BvConfirmitStatus] VALUES( '63', 'Custom33', 63 )
INSERT INTO [BvConfirmitStatus] VALUES( '64', 'Custom34', 64 )
INSERT INTO [BvConfirmitStatus] VALUES( '65', 'Custom35', 65 )
INSERT INTO [BvConfirmitStatus] VALUES( '66', 'Custom36', 66 )
INSERT INTO [BvConfirmitStatus] VALUES( '67', 'Custom37', 67 )
INSERT INTO [BvConfirmitStatus] VALUES( '68', 'Custom38', 68 )
INSERT INTO [BvConfirmitStatus] VALUES( '69', 'Custom39', 69 )
INSERT INTO [BvConfirmitStatus] VALUES( '70', 'Custom40', 70 )
INSERT INTO [BvConfirmitStatus] VALUES( '71', 'Custom41', 71 )
INSERT INTO [BvConfirmitStatus] VALUES( '72', 'Custom42', 72 )
INSERT INTO [BvConfirmitStatus] VALUES( '73', 'Custom43', 73 )
INSERT INTO [BvConfirmitStatus] VALUES( '74', 'Custom44', 74 )
INSERT INTO [BvConfirmitStatus] VALUES( '75', 'Custom45', 75 )
INSERT INTO [BvConfirmitStatus] VALUES( '76', 'Custom46', 76 )
INSERT INTO [BvConfirmitStatus] VALUES( '77', 'Custom47', 77 )
INSERT INTO [BvConfirmitStatus] VALUES( '78', 'Custom48', 78 )
INSERT INTO [BvConfirmitStatus] VALUES( '79', 'Custom49', 79 )
INSERT INTO [BvConfirmitStatus] VALUES( '80', 'Custom50', 80 )
INSERT INTO [BvConfirmitStatus] VALUES( '81', 'Custom51', 81 )
INSERT INTO [BvConfirmitStatus] VALUES( '82', 'Custom52', 82 )
INSERT INTO [BvConfirmitStatus] VALUES( '83', 'Custom53', 83 )
INSERT INTO [BvConfirmitStatus] VALUES( '84', 'Custom54', 84 )
INSERT INTO [BvConfirmitStatus] VALUES( '85', 'Custom55', 85 )
INSERT INTO [BvConfirmitStatus] VALUES( '86', 'Custom56', 86 )
INSERT INTO [BvConfirmitStatus] VALUES( '87', 'Custom57', 87 )
INSERT INTO [BvConfirmitStatus] VALUES( '88', 'Custom58', 88 )
INSERT INTO [BvConfirmitStatus] VALUES( '89', 'Custom59', 89 )
INSERT INTO [BvConfirmitStatus] VALUES( '90', 'Custom60', 90 )
INSERT INTO [BvConfirmitStatus] VALUES( '91', 'Custom61', 91 )
INSERT INTO [BvConfirmitStatus] VALUES( '92', 'Custom62', 92 )
INSERT INTO [BvConfirmitStatus] VALUES( '93', 'Custom63', 93 )
INSERT INTO [BvConfirmitStatus] VALUES( '94', 'Custom64', 94 )
INSERT INTO [BvConfirmitStatus] VALUES( '95', 'Custom65', 95 )
INSERT INTO [BvConfirmitStatus] VALUES( '96', 'Custom66', 96 )
INSERT INTO [BvConfirmitStatus] VALUES( '97', 'Custom67', 97 )
INSERT INTO [BvConfirmitStatus] VALUES( '98', 'Custom68', 98 )
INSERT INTO [BvConfirmitStatus] VALUES( '99', 'Custom69', 99 )
INSERT INTO [BvConfirmitStatus] VALUES( '100', 'Custom70', 100 )
INSERT INTO [BvConfirmitStatus] VALUES( '101', 'Custom71', 101 )
INSERT INTO [BvConfirmitStatus] VALUES( '102', 'Custom72', 102 )
INSERT INTO [BvConfirmitStatus] VALUES( '103', 'Custom73', 103 )
INSERT INTO [BvConfirmitStatus] VALUES( '104', 'Custom74', 104 )
INSERT INTO [BvConfirmitStatus] VALUES( '105', 'Custom75', 105 )
INSERT INTO [BvConfirmitStatus] VALUES( '106', 'Custom76', 106 )
INSERT INTO [BvConfirmitStatus] VALUES( '107', 'Custom77', 107 )
INSERT INTO [BvConfirmitStatus] VALUES( '108', 'Custom78', 108 )
INSERT INTO [BvConfirmitStatus] VALUES( '109', 'Custom79', 109 )
INSERT INTO [BvConfirmitStatus] VALUES( '110', 'Custom80', 110 )
INSERT INTO [BvConfirmitStatus] VALUES( '111', 'Custom81', 111 )
INSERT INTO [BvConfirmitStatus] VALUES( '112', 'Custom82', 112 )
INSERT INTO [BvConfirmitStatus] VALUES( '113', 'Custom83', 113 )
INSERT INTO [BvConfirmitStatus] VALUES( '114', 'Custom84', 114 )
INSERT INTO [BvConfirmitStatus] VALUES( '115', 'Custom85', 115 )
INSERT INTO [BvConfirmitStatus] VALUES( '116', 'Custom86', 116 )
INSERT INTO [BvConfirmitStatus] VALUES( '117', 'Custom87', 117 )
INSERT INTO [BvConfirmitStatus] VALUES( '118', 'Custom88', 118 )
INSERT INTO [BvConfirmitStatus] VALUES( '119', 'Custom89', 119 )
INSERT INTO [BvConfirmitStatus] VALUES( '120', 'Custom90', 120 )
GO

INSERT INTO BvSurveyListAlertsViewConfiguration VALUES(15, NULL, 3600, NULL, 60, 0, 3600)

PRINT 'All done.'

