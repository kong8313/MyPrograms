GO
PRINT N'Dropping [dbo].[BvSvySchedule].[IX_BvSvyScheduleMain]...';


GO
DROP INDEX [IX_BvSvyScheduleMain]
    ON [dbo].[BvSvySchedule];


GO
PRINT N'Dropping [dbo].[BvAppointment].[IX_app_State]...';


GO
DROP INDEX [IX_app_State]
    ON [dbo].[BvAppointment];


GO
PRINT N'Dropping DF_BvPerson_AssignmentsListMode...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [DF_BvPerson_AssignmentsListMode];


GO
PRINT N'Dropping Df_BvPerson_Description...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [Df_BvPerson_Description];


GO
PRINT N'Dropping Df_BvPerson_FullName...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [Df_BvPerson_FullName];


GO
PRINT N'Dropping Df_BvPerson_Name...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [Df_BvPerson_Name];


GO
PRINT N'Dropping DF_BvPerson_TotalSampleSize_AllowedChoices...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [DF_BvPerson_TotalSampleSize_AllowedChoices];


GO
PRINT N'Dropping DF_BvPerson_TotalSampleSize_DeskStationName...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [DF_BvPerson_TotalSampleSize_DeskStationName];


GO
PRINT N'Dropping DF_BvPerson_TotalSampleSize_DialerConnection...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [DF_BvPerson_TotalSampleSize_DialerConnection];


GO
PRINT N'Dropping DF_BvPerson_TotalSampleSize_DialerId...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [DF_BvPerson_TotalSampleSize_DialerId];


GO
PRINT N'Dropping DF_BvPerson_TotalSampleSize_ExtensionNumber...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [DF_BvPerson_TotalSampleSize_ExtensionNumber];


GO
PRINT N'Dropping DF_BvPerson_TotalSampleSize_FailedLoginAttempts...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [DF_BvPerson_TotalSampleSize_FailedLoginAttempts];


GO
PRINT N'Dropping DF_BvPerson_TotalSampleSize_HasNewMessage...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [DF_BvPerson_TotalSampleSize_HasNewMessage];


GO
PRINT N'Dropping DF_BvPerson_TotalSampleSize_IsDialerAgentLocal...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [DF_BvPerson_TotalSampleSize_IsDialerAgentLocal];


GO
PRINT N'Dropping DF_BvPerson_TotalSampleSize_IsLocked...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [DF_BvPerson_TotalSampleSize_IsLocked];


GO
PRINT N'Dropping DF_BvPerson_TotalSampleSize_Location...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [DF_BvPerson_TotalSampleSize_Location];


GO
PRINT N'Dropping DF_BvPerson_TotalSampleSize_MNDiallerUserId...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [DF_BvPerson_TotalSampleSize_MNDiallerUserId];


GO
PRINT N'Dropping DF_BvPerson_TotalSampleSize_PwdHashTxt...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [DF_BvPerson_TotalSampleSize_PwdHashTxt];


GO
PRINT N'Dropping DF_BvPerson_TotalSampleSize_PwdSaltTxt...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [DF_BvPerson_TotalSampleSize_PwdSaltTxt];


GO
PRINT N'Dropping DF_BvPerson_TotalSampleSize_StationExtensionNumber...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [DF_BvPerson_TotalSampleSize_StationExtensionNumber];


GO
PRINT N'Dropping DF_BvPerson_TotalSampleSize_TimezoneID...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [DF_BvPerson_TotalSampleSize_TimezoneID];


GO
PRINT N'Dropping DfBvPerson_IS...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [DfBvPerson_IS];


GO
PRINT N'Dropping DF_BvCachedCalls_OrderId...';


GO
ALTER TABLE [dbo].[BvCachedCalls] DROP CONSTRAINT [DF_BvCachedCalls_OrderId];


GO
PRINT N'Dropping DF_BvAggregateSurveyAlertStatus_ActiveCallsCount...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus] DROP CONSTRAINT [DF_BvAggregateSurveyAlertStatus_ActiveCallsCount];


GO
PRINT N'Dropping DF_BvAggregateSurveyAlertStatus_ActiveCallsCountPrev...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus] DROP CONSTRAINT [DF_BvAggregateSurveyAlertStatus_ActiveCallsCountPrev];


GO
PRINT N'Dropping DF_BvAggregateSurveyAlertStatus_AlertStatusOfActiveCallsCount...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus] DROP CONSTRAINT [DF_BvAggregateSurveyAlertStatus_AlertStatusOfActiveCallsCount];


GO
PRINT N'Dropping DF_BvTasks_LastKeepAliveTimeAlert...';


GO
ALTER TABLE [dbo].[BvTasks] DROP CONSTRAINT [DF_BvTasks_LastKeepAliveTimeAlert];


GO
PRINT N'Dropping DF_BvTasks_LastSubmissionAlert...';


GO
ALTER TABLE [dbo].[BvTasks] DROP CONSTRAINT [DF_BvTasks_LastSubmissionAlert];


GO
PRINT N'Dropping DF_BvTasks_SecondsSinceLastSubmission...';


GO
ALTER TABLE [dbo].[BvTasks] DROP CONSTRAINT [DF_BvTasks_SecondsSinceLastSubmission];


GO
PRINT N'Dropping FK_BvMessageToPerson_BvPerson...';


GO
ALTER TABLE [dbo].[BvMessageToPerson] DROP CONSTRAINT [FK_BvMessageToPerson_BvPerson];


GO
PRINT N'Dropping FK_BvPerson_BvSurvey...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [FK_BvPerson_BvSurvey];


GO
PRINT N'Dropping FK_BvPerson_CallGroupID...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [FK_BvPerson_CallGroupID];


GO
PRINT N'Dropping FK_BvPerson_TimezoneID...';


GO
ALTER TABLE [dbo].[BvPerson] DROP CONSTRAINT [FK_BvPerson_TimezoneID];


GO
PRINT N'Dropping FK_BvPersonMonitoring_BvPerson...';


GO
ALTER TABLE [dbo].[BvPersonMonitoring] DROP CONSTRAINT [FK_BvPersonMonitoring_BvPerson];


GO
PRINT N'Dropping [dbo].[BvAsyncOperations]...';


GO
DROP TABLE [dbo].[BvAsyncOperations];


GO
PRINT N'Dropping [dbo].[GetCallsForGroupForPredictiveSurvey]...';


GO
DROP FUNCTION [dbo].[GetCallsForGroupForPredictiveSurvey];

GO
PRINT N'Dropping [dbo].[GetCallByCondition]...';


GO
DROP FUNCTION [dbo].[GetCallByCondition];


GO
PRINT N'Dropping [dbo].[GetCallBySurvey]...';


GO
DROP FUNCTION [dbo].[GetCallBySurvey];

GO
PRINT N'Dropping [dbo].[BvSpCache_GetCalls]...';


GO
DROP PROCEDURE [dbo].[BvSpCache_GetCalls];


GO
PRINT N'Dropping [dbo].[BvSpCache_NotifyUpdated]...';


GO
DROP PROCEDURE [dbo].[BvSpCache_NotifyUpdated];


GO
PRINT N'Dropping [dbo].[BvSpCachedCalls_CallsCount_SaveToActiveCallsInfo]...';


GO
DROP PROCEDURE [dbo].[BvSpCachedCalls_CallsCount_SaveToActiveCallsInfo];


GO
PRINT N'Dropping [dbo].[BvSpCleanActiveCallsInfo]...';


GO
DROP PROCEDURE [dbo].[BvSpCleanActiveCallsInfo];


GO
PRINT N'Dropping [dbo].[BvSpGetActiveCallsDistribution]...';


GO
DROP PROCEDURE [dbo].[BvSpGetActiveCallsDistribution];


GO
PRINT N'Dropping [dbo].[BvSpPerson_updateDialerConnection]...';


GO
DROP PROCEDURE [dbo].[BvSpPerson_updateDialerConnection];


GO
PRINT N'Dropping [dbo].[BvSpPerson_updateMNDiallerUserId]...';


GO
DROP PROCEDURE [dbo].[BvSpPerson_updateMNDiallerUserId];


GO
PRINT N'Dropping [dbo].[BvSpSurvey_Close]...';


GO
DROP PROCEDURE [dbo].[BvSpSurvey_Close];


GO
PRINT N'Dropping [dbo].[BvSpSynchronizeAggregateData]...';


GO
DROP PROCEDURE [dbo].[BvSpSynchronizeAggregateData];


GO
PRINT N'Dropping [dbo].[BvSpTasks_LockByInterview]...';


GO
DROP PROCEDURE [dbo].[BvSpTasks_LockByInterview];


GO
PRINT N'Dropping [dbo].[BvActiveCallsInfo]...';


GO
DROP TABLE [dbo].[BvActiveCallsInfo];


GO
PRINT N'Dropping [dbo].[BvCachedCalls]...';


GO
DROP TABLE [dbo].[BvCachedCalls];


GO
PRINT N'Dropping [dbo].[BvCachedCallsSwapTable]...';


GO
DROP TABLE [dbo].[BvCachedCallsSwapTable];


GO
PRINT N'Dropping [dbo].[GetCallsForCacheTable]...';


GO
DROP FUNCTION [dbo].[GetCallsForCacheTable];

GO
PRINT N'Altering [dbo].[BvAggregateSurveyAlertStatus]...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyAlertStatus] DROP COLUMN [ActiveCallsCount], COLUMN [ActiveCallsCountPrev], COLUMN [AlertStatusOfActiveCallsCount];


GO
PRINT N'Altering [dbo].[BvHistory]...';


GO
ALTER TABLE [dbo].[BvHistory] ADD [CallCenterID] INT NOT NULL CONSTRAINT __Temp__DF_BvHistory_CallCenterID DEFAULT (0);
ALTER TABLE [dbo].[BvHistory] DROP CONSTRAINT __Temp__DF_BvHistory_CallCenterID

GO

PRINT N'Altering [dbo].[BvInterviewTimings]...';


GO
ALTER TABLE [dbo].[BvInterviewTimings] ADD [CallCenterID] INT NULL CONSTRAINT __Temp__DF_BvInterviewTimings_CallCenterID DEFAULT (0);
ALTER TABLE [dbo].[BvInterviewTimings] DROP CONSTRAINT __Temp__DF_BvInterviewTimings_CallCenterID


GO
/*
The column [dbo].[BvPerson].[DeskStationName] is being dropped, data loss could occur.

The column [dbo].[BvPerson].[DialerConnection] is being dropped, data loss could occur.

The column [dbo].[BvPerson].[DialerId] is being dropped, data loss could occur.

The column [dbo].[BvPerson].[ExtensionNumber] is being dropped, data loss could occur.

The column [dbo].[BvPerson].[FailedLoginAttempts] is being dropped, data loss could occur.

The column [dbo].[BvPerson].[IsDialerAgentLocal] is being dropped, data loss could occur.

The column [dbo].[BvPerson].[MNDiallerUserId] is being dropped, data loss could occur.

The column [dbo].[BvPerson].[StationExtensionNumber] is being dropped, data loss could occur.

The column [dbo].[BvPerson].[TimezoneID] is being dropped, data loss could occur.

The column [dbo].[BvPerson].[CallCenterID] on table [dbo].[BvPerson] must be added, but the column has no default value and does not allow NULL values. If the table contains data, the ALTER script will not work. To avoid this issue you must either: add a default value to the column, mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.
*/
GO
PRINT N'Creating [dbo].[BvPersonFailedLoginAttempts]...';


GO
CREATE TABLE [dbo].[BvPersonFailedLoginAttempts] (
    [PersonId] INT NOT NULL,
    [Count]    INT NOT NULL,
    CONSTRAINT [PK_BvPersonFailedLoginAttempts] PRIMARY KEY CLUSTERED ([PersonId] ASC)
);


GO
PRINT N'Starting rebuilding table [dbo].[BvPerson]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_BvPerson] (
    [SID]                 INT            NOT NULL,
    [Name]                NVARCHAR (255) CONSTRAINT [Df_BvPerson_Name] DEFAULT (' ') NOT NULL,
    [FullName]            NVARCHAR (255) CONSTRAINT [Df_BvPerson_FullName] DEFAULT (' ') NOT NULL,
    [Description]         NVARCHAR (255) CONSTRAINT [Df_BvPerson_Description] DEFAULT (' ') NOT NULL,
    [ManualSelection]     INT            CONSTRAINT [DfBvPerson_IS] DEFAULT ((0)) NOT NULL,
    [PwdHashTxt]          NVARCHAR (256) CONSTRAINT [DF_BvPerson_TotalSampleSize_PwdHashTxt] DEFAULT ('') NOT NULL,
    [PwdSaltTxt]          NVARCHAR (256) CONSTRAINT [DF_BvPerson_TotalSampleSize_PwdSaltTxt] DEFAULT ('') NOT NULL,
    [HasNewMessage]       BIT            CONSTRAINT [DF_BvPerson_TotalSampleSize_HasNewMessage] DEFAULT (NULL) NULL,
    [AutomaticSurveyID]   INT            NULL,
    [AllowedChoices]      INT            CONSTRAINT [DF_BvPerson_TotalSampleSize_AllowedChoices] DEFAULT (NULL) NULL,
    [IsLocked]            BIT            CONSTRAINT [DF_BvPerson_TotalSampleSize_IsLocked] DEFAULT (0) NOT NULL,
    [LockedDate]          DATETIME       NULL,
    [AssignmentsListMode] INT            CONSTRAINT [DF_BvPerson_AssignmentsListMode] DEFAULT 0 NOT NULL,
    [CallGroupID]         INT            NULL,
    [CallCenterID]        INT            NOT NULL,
    [Location]            NVARCHAR (256) CONSTRAINT [DF_BvPerson_TotalSampleSize_Location] DEFAULT (NULL) NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_BvPerson_SID] PRIMARY KEY CLUSTERED ([SID] ASC),
    CONSTRAINT [tmp_ms_xx_constraint_UQ_BvPerson_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[BvPerson])
    BEGIN
		INSERT INTO [dbo].BvPersonFailedLoginAttempts([PersonId],[Count]) 
		SELECT	 [SID], 
				 [FailedLoginAttempts] 
		FROM [dbo].[BvPerson]
		ORDER BY [SID] ASC

        INSERT INTO [dbo].[tmp_ms_xx_BvPerson] ([SID], [Name], [FullName], [Description], [ManualSelection], [PwdHashTxt], [PwdSaltTxt], [HasNewMessage], [AutomaticSurveyID], [AllowedChoices], [IsLocked], [LockedDate], [AssignmentsListMode], [CallGroupID], [Location], [CallCenterID])
        SELECT   [SID],
                 [Name],
                 [FullName],
                 [Description],
                 [ManualSelection],
                 [PwdHashTxt],
                 [PwdSaltTxt],
                 [HasNewMessage],
                 [AutomaticSurveyID],
                 [AllowedChoices],
                 [IsLocked],
                 [LockedDate],
                 [AssignmentsListMode],
                 [CallGroupID],
                 [Location],
				 0/*We will update this field bit later*/
        FROM     [dbo].[BvPerson]
        ORDER BY [SID] ASC;
    END

DROP TABLE [dbo].[BvPerson];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_BvPerson]', N'BvPerson';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_BvPerson_SID]', N'PK_BvPerson_SID', N'OBJECT';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_UQ_BvPerson_Name]', N'UQ_BvPerson_Name', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Altering [dbo].[BvPersonDeferredMonitoring]...';


GO
ALTER TABLE [dbo].[BvPersonDeferredMonitoring] ADD [CallCenterId] INT NOT NULL CONSTRAINT __Temp__DF_BvPersonDeferredMonitoring_CallCenterID DEFAULT (0);
ALTER TABLE [dbo].[BvPersonDeferredMonitoring] DROP CONSTRAINT __Temp__DF_BvPersonDeferredMonitoring_CallCenterID


GO
/*
The column [dbo].[BvPersonOrGroupAssignmentOnSurvey].[CallCenterID] on table [dbo].[BvPersonOrGroupAssignmentOnSurvey] must be added, but the column has no default value and does not allow NULL values. If the table contains data, the ALTER script will not work. To avoid this issue you must either: add a default value to the column, mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.
*/
GO
PRINT N'Starting rebuilding table [dbo].[BvPersonOrGroupAssignmentOnSurvey]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_BvPersonOrGroupAssignmentOnSurvey] (
    [Id]              INT IDENTITY (1, 1) NOT NULL,
    [CallCenterID]    INT NOT NULL,
    [PersonOrGroupId] INT NOT NULL,
    [SurveyId]        INT NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_PersonOrGroupAssignmentOnSurvey] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[BvPersonOrGroupAssignmentOnSurvey])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_BvPersonOrGroupAssignmentOnSurvey] ON;
        INSERT INTO [dbo].[tmp_ms_xx_BvPersonOrGroupAssignmentOnSurvey] ([Id], [CallCenterID], [PersonOrGroupId], [SurveyId])
        SELECT   [Id],
			     0,/*We will update this field bit later*/
                 [PersonOrGroupId],
                 [SurveyId]
        FROM     [dbo].[BvPersonOrGroupAssignmentOnSurvey]
        ORDER BY [Id] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_BvPersonOrGroupAssignmentOnSurvey] OFF;
    END

DROP TABLE [dbo].[BvPersonOrGroupAssignmentOnSurvey];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_BvPersonOrGroupAssignmentOnSurvey]', N'BvPersonOrGroupAssignmentOnSurvey';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_PersonOrGroupAssignmentOnSurvey]', N'PK_PersonOrGroupAssignmentOnSurvey', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Altering [dbo].[BvTasks]...';


GO
ALTER TABLE [dbo].[BvTasks] DROP COLUMN [LastKeepAliveTimeAlert], COLUMN [LastSubmissionAlert], COLUMN [SecondsSinceLastSubmission];


GO
ALTER TABLE [dbo].[BvTasks]
    ADD [DialerId]               INT            CONSTRAINT [DF_BvTasks_DialerId] DEFAULT (0) NOT NULL,
        [StationExtensionNumber] NVARCHAR (256) CONSTRAINT [DF_BvTasks_StationExtensionNumber] DEFAULT ('') NOT NULL,
        [IsDialerAgentLocal]     BIT            CONSTRAINT [DF_BvTasks_IsDialerAgentLocal] DEFAULT (0) NOT NULL,
        [CallCenterID]           INT            NOT NULL CONSTRAINT __Temp__DF_BvTasks_CallCenterID DEFAULT (0);
ALTER TABLE [dbo].[BvTasks] DROP CONSTRAINT __Temp__DF_BvTasks_CallCenterID

GO
PRINT N'Altering [dbo].[BvTimeBreaksHistory]...';


GO
ALTER TABLE [dbo].[BvTimeBreaksHistory] ADD [CallCenterId] INT NOT NULL CONSTRAINT __Temp__DF_BvTimeBreaksHistory_CallCenterID DEFAULT (0);
ALTER TABLE [dbo].[BvTimeBreaksHistory] DROP CONSTRAINT __Temp__DF_BvTimeBreaksHistory_CallCenterID

GO
PRINT N'Starting rebuilding table [dbo].[BvVersionHistory]...';


GO
DROP TABLE [dbo].[BvVersionHistory]


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
PRINT N'Creating [dbo].[BvAggregateSurveyDelta]...';


GO
CREATE TABLE [dbo].[BvAggregateSurveyDelta] (
    [ID]                          BIGINT IDENTITY (1, 1) NOT NULL,
    [SID]                         INT    NOT NULL,
    [ScheduledCallsCount]         INT    NOT NULL,
    [SuspendedCallsCount]         INT    NOT NULL,
    [MinutesSpentWorkingOnSurvey] INT    NOT NULL,
    CONSTRAINT [BvAggregateSurveyDelta_PK_ID] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
ALTER TABLE [dbo].[BvAggregateSurveyDelta] SET (LOCK_ESCALATION = DISABLE);


GO
PRINT N'Creating [dbo].[BvAsyncOperationQueue]...';


GO
CREATE TABLE [dbo].[BvAsyncOperationQueue] (
    [Id]                      INT            IDENTITY (1, 1) NOT NULL,
    [IsInitiatedBySystem]     BIT            NOT NULL,
    [Type]                    TINYINT        NOT NULL,
    [Title]                   NVARCHAR (255) NULL,
    [State]                   TINYINT        NOT NULL,
    [Parameters]              XML            NOT NULL,
    [SurveySid]               INT            NOT NULL,
    [Priority]                INT            NOT NULL,
    [QueuedDate]              DATETIME       NOT NULL,
    [StartedDate]             DATETIME       NULL,
    [FinishedDate]            DATETIME       NULL,
    [HeartBeat]               DATETIME       NULL,
    [TotalItemsCount]         INT            NOT NULL,
    [ProcessedItemsCount]     INT            NOT NULL,
    [FailedItemsCount]        INT            NOT NULL,
    [CreatedBySupervisorName] NVARCHAR (255) NULL,
    [AbortedBySupervisorName] NVARCHAR (255) NULL,
    [Server]                  NVARCHAR (256) NOT NULL,
    [Error]                   NVARCHAR (MAX) NULL,
    [Text]                    NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_BvAsyncOperationQueue_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[BvAsyncOperationQueue].[IX_BvAsyncOperationQueue_Priority_Id]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_BvAsyncOperationQueue_Priority_Id]
    ON [dbo].[BvAsyncOperationQueue]([Priority] ASC, [Id] ASC);


GO
PRINT N'Creating [dbo].[BvAsyncOperationQueue].[IX_BvAsyncOperationQueue_State]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvAsyncOperationQueue_State]
    ON [dbo].[BvAsyncOperationQueue]([State] ASC);


GO
PRINT N'Creating [dbo].[BvCallCenter]...';


GO
CREATE TABLE [dbo].[BvCallCenter] (
    [ID]              INT            IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (MAX) NOT NULL,
    [Description]     NVARCHAR (MAX) NULL,
    [IsDefault]       BIT            NOT NULL,
    [CanBeDeleted]    BIT            NOT NULL,
    [LocalTimezoneId] INT            NOT NULL,
    CONSTRAINT [PK_BvCallCenter] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating [dbo].[BvStartedServices]...';


GO
if NOT EXISTS (SELECT * FROM sysobjects WHERE name='BvStartedServices' AND xtype='U')
	CREATE TABLE [dbo].[BvStartedServices] (
		[MachineName] NVARCHAR (128) NOT NULL,
		[ServiceName] NVARCHAR (128) NOT NULL,
		CONSTRAINT [PK_BvStartedServices] PRIMARY KEY CLUSTERED ([MachineName] ASC, [ServiceName] ASC)
	);


GO
PRINT N'Creating [dbo].[BvSupervisorAssignment]...';


GO
CREATE TABLE [dbo].[BvSupervisorAssignment] (
    [Name]         NVARCHAR (256) NOT NULL,
    [CallCenterId] INT            NULL,
    CONSTRAINT [PK_BvSupervisorAssignment] PRIMARY KEY CLUSTERED ([Name] ASC)
);


GO
PRINT N'Creating [dbo].[BvSurveyAssignmentOnCallCenter]...';


GO
CREATE TABLE [dbo].[BvSurveyAssignmentOnCallCenter] (
    [SurveyId]     INT NOT NULL,
    [CallCenterId] INT NOT NULL,
    CONSTRAINT [PK_BvSurveyAssignmentOnCallCenter] PRIMARY KEY CLUSTERED ([SurveyId] ASC, [CallCenterId] ASC) WITH (IGNORE_DUP_KEY = ON)
);


GO
PRINT N'Creating [dbo].[BvSvySchedule].[IX_BvSvyScheduleMain]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_BvSvyScheduleMain]
    ON [dbo].[BvSvySchedule]([IsInActiveShiftType] ASC, [SurveySID] ASC, [Priority] DESC, [TimeInShift] ASC, [ExplicitType] DESC, [CallOrder] ASC, [InterviewID] ASC, [ExplicitSID] ASC)
    INCLUDE([ID], [CallState], [ApptID], [ConditionValue], [ExpireTime]);


GO
PRINT N'Creating [dbo].[BvSvySchedule].[IX_BvSvySchedule_Priority]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvSvySchedule_Priority]
    ON [dbo].[BvSvySchedule]([SurveySID] ASC, [Priority] ASC);


GO
PRINT N'Creating [dbo].[BvSvySChedule].[IX_BvSvySchedule_ShiftTypeID]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvSvySchedule_ShiftTypeID]
    ON [dbo].[BvSvySchedule]([SurveySID] ASC, [ShiftTypeID] ASC);


GO
PRINT N'Creating [dbo].[BvSvySChedule].[IX_BvSvySchedule_TimeInShift]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvSvySchedule_TimeInShift]
    ON [dbo].[BvSvySchedule]([SurveySID] ASC, [TimeInShift] ASC);


GO
PRINT N'Creating [dbo].[BvAppointment].[IX_BvAppointment_ExpTime]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvAppointment_ExpTime]
    ON [dbo].[BvAppointment]([SurveySID] ASC, [ExpTime] ASC);


GO
PRINT N'Creating [dbo].[BvAppointment].[IX_BvAppointment_Time]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvAppointment_Time]
    ON [dbo].[BvAppointment]([SurveySID] ASC, [Time] ASC);


GO
PRINT N'Creating [dbo].[BvInterview].[IX_BvInterview_LastCallTime]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvInterview_LastCallTime]
    ON [dbo].[BvInterview]([SurveySID] ASC, [LastCallTime] ASC);


GO
PRINT N'Creating [dbo].[BvInterview].[IX_BvInterview_RespondentName]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvInterview_RespondentName]
    ON [dbo].[BvInterview]([SurveySID] ASC, [RespondentName] ASC);


GO
PRINT N'Creating [dbo].[BvInterview].[IX_BvInterview_TelephoneNumber]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvInterview_TelephoneNumber]
    ON [dbo].[BvInterview]([SurveySID] ASC, [TelephoneNumber] ASC);


GO
PRINT N'Creating [dbo].[BvInterview].[IX_BvInterview_TimezoneID]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvInterview_TimezoneID]
    ON [dbo].[BvInterview]([SurveySID] ASC, [TimezoneID] ASC);


GO
PRINT N'Creating DF_BvAggregateSurveyDelta_MinutesSpentWorkingOnSurvey...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyDelta]
    ADD CONSTRAINT [DF_BvAggregateSurveyDelta_MinutesSpentWorkingOnSurvey] DEFAULT (0) FOR [MinutesSpentWorkingOnSurvey];


GO
PRINT N'Creating DF_BvAggregateSurveyDelta_ScheduledCallsCount...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyDelta]
    ADD CONSTRAINT [DF_BvAggregateSurveyDelta_ScheduledCallsCount] DEFAULT (0) FOR [ScheduledCallsCount];


GO
PRINT N'Creating DF_BvAggregateSurveyDelta_SuspendedCallsCount...';


GO
ALTER TABLE [dbo].[BvAggregateSurveyDelta]
    ADD CONSTRAINT [DF_BvAggregateSurveyDelta_SuspendedCallsCount] DEFAULT (0) FOR [SuspendedCallsCount];


GO
PRINT N'Creating DF_CanBeDeleted...';


GO
ALTER TABLE [dbo].[BvCallCenter]
    ADD CONSTRAINT [DF_CanBeDeleted] DEFAULT (0) FOR [CanBeDeleted];


GO
PRINT N'Creating DF_IsDefault...';


GO
ALTER TABLE [dbo].[BvCallCenter]
    ADD CONSTRAINT [DF_IsDefault] DEFAULT (0) FOR [IsDefault];


GO
PRINT N'Creating DF_BvPersonFailedLoginAttempts_Count...';


GO
ALTER TABLE [dbo].[BvPersonFailedLoginAttempts]
    ADD CONSTRAINT [DF_BvPersonFailedLoginAttempts_Count] DEFAULT (0) FOR [Count];


GO
PRINT N'Creating FK_BvMessageToPerson_BvPerson...';


GO
ALTER TABLE [dbo].[BvMessageToPerson] WITH NOCHECK
    ADD CONSTRAINT [FK_BvMessageToPerson_BvPerson] FOREIGN KEY ([InterviewerId]) REFERENCES [dbo].[BvPerson] ([SID]) ON DELETE CASCADE;


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
PRINT N'Creating FK_BvPersonMonitoring_BvPerson...';


GO
ALTER TABLE [dbo].[BvPersonMonitoring] WITH NOCHECK
    ADD CONSTRAINT [FK_BvPersonMonitoring_BvPerson] FOREIGN KEY ([PersonSID]) REFERENCES [dbo].[BvPerson] ([SID]) ON DELETE CASCADE;


GO
PRINT N'Altering [dbo].[BvTrBvHistory_HistoryInsert]...';


GO
ALTER TRIGGER [BvTrBvHistory_HistoryInsert] ON [dbo].[BvHistory]
FOR INSERT
AS 
BEGIN
	SET NOCOUNT ON
		
	INSERT INTO [BvAggregateSurveyDelta]								      
		SELECT 
			/*[SID]*/ SurveyId,
			/*[ScheduledCallsCount]*/ 0,
			/*[SuspendedCallsCount]*/ 0,
			/*[MinutesSpentWorkingOnSurvey]*/ ISNULL(SUM(WaitingTime), 0) + ISNULL(SUM(Duration), 0) MinutesSpentWorkingOnSurvey
		FROM inserted
		WHERE RoleId = 2
		GROUP BY SurveyId
END
GO
PRINT N'Altering [dbo].[BvTrBvInterview_InterviewsDelete]...';


GO
ALTER TRIGGER [BvTrBvInterview_InterviewsDelete] ON [dbo].[BvInterview] 
AFTER DELETE
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO [BvAggregateSurveyDelta]
		SELECT 
		    /*[SID]*/ SurveySID, 
			/*[ScheduledCallsCount]*/ 0, 
			/*[SuspendedCallsCount]*/ -COUNT(*) SuspendedCallsCount, 
			/*[MinutesSpentWorkingOnSurvey]*/ 0
        FROM deleted
        GROUP BY SurveySID


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
PRINT N'Altering [dbo].[BvTrBvInterview_InterviewsInsert]...';


GO
ALTER TRIGGER [BvTrBvInterview_InterviewsInsert] ON [dbo].[BvInterview] 
AFTER INSERT
AS
BEGIN
	SET NOCOUNT ON
    
	INSERT INTO [BvAggregateSurveyDelta]
		SELECT 
		    /*[SID]*/ SurveySID, 
			/*[ScheduledCallsCount]*/ 0, 
			/*[SuspendedCallsCount]*/ COUNT(*) SuspendedCallsCount, 
			/*[MinutesSpentWorkingOnSurvey]*/ 0
        FROM inserted
        GROUP BY SurveySID

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
PRINT N'Altering [dbo].[BvTrBvSvySchedule_CallsDelete]...';


GO
ALTER TRIGGER [BvTrBvSvySchedule_CallsDelete] ON [dbo].[BvSvySchedule]
FOR DELETE
AS 
BEGIN
	SET NOCOUNT ON
                                      
	INSERT INTO [BvAggregateSurveyDelta]
		SELECT
			/*[SID]*/ SurveySid, 
			/*[ScheduledCallsCount]*/ -COUNT(*) ScheduledCallsCount,
			/*[SuspendedCallsCount]*/ 0,
			/*[MinutesSpentWorkingOnSurvey]*/ 0
		FROM deleted
		WHERE CallState IN (2, -2)
		GROUP BY SurveySid

END
GO
PRINT N'Altering [dbo].[BvTrBvSvySchedule_CallsInsert]...';


GO
ALTER TRIGGER [BvTrBvSvySchedule_CallsInsert] ON [dbo].[BvSvySchedule]
AFTER INSERT
AS 
BEGIN
	SET NOCOUNT ON
	
	INSERT INTO [BvAggregateSurveyDelta]
		SELECT
			/*[SID]*/ SurveySid,
			/*[ScheduledCallsCount]*/ COUNT(*) ScheduledCallsCount,
			/*[SuspendedCallsCount]*/ 0,
			/*[MinutesSpentWorkingOnSurvey]*/ 0
		FROM inserted
		WHERE CallState IN (2, -2)
		GROUP BY SurveySid

END
GO
PRINT N'Altering [dbo].[BvTrBvSvySchedule_CallsUpdate]...';


GO
ALTER TRIGGER [BvTrBvSvySchedule_CallsUpdate] ON [dbo].[BvSvySchedule]
FOR UPDATE 
AS 
BEGIN
	SET NOCOUNT ON
	
	IF UPDATE( CallState )
	BEGIN								                          

		INSERT INTO [BvAggregateSurveyDelta]
		   SELECT 
			   /*[SID]*/                         inserted.SurveySid, 
			   /*[ScheduledCallsCount]*/         SUM(CASE WHEN inserted.CallState IN (2, -2) THEN 1 --call have been added
												          ELSE -1 --call have been deleted
											         END) ScheduledCallsCount,
				/*[SuspendedCallsCount]*/         0,
				/*[MinutesSpentWorkingOnSurvey]*/ 0
		   FROM 
			   inserted
		   INNER JOIN 
			   deleted 
		   ON 
			   inserted.id = deleted.id AND
			   (
				 (inserted.CallState IN (2,-2) AND         --call have been added
				  deleted.CallState NOT IN (2, -2)) OR     -- OR
				 (inserted.CallState NOT IN (2, -2) AND    --call have been deleted
				  deleted.CallState IN (2, -2)))
		   GROUP BY inserted.SurveySid

	END
END
GO
PRINT N'Refreshing [dbo].[GetLastTimeBreak]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[GetLastTimeBreak]';


GO
PRINT N'Creating [dbo].[GetCallsForGroupForPredictiveSurvey]...';


GO
CREATE FUNCTION dbo.GetCallsForGroupForPredictiveSurvey
(
    @rowCount AS INT,
    @SurveySid AS INT,
    @ObjectSid AS INT,
	@SuitableTimeForCalls DATETIME
)
RETURNS TABLE
AS RETURN(
          SELECT TOP (@rowCount) *
          FROM BvSvySchedule c
          WHERE SurveySid = @SurveySid AND
                ExplicitSID = @ObjectSid AND
                CallState = 2 AND
				IsInActiveShiftType = 1 AND
				TimeInShift <= @SuitableTimeForCalls
          ORDER BY priority DESC, TimeInShift, ExplicitType DESC, CallOrder )
GO
PRINT N'Creating [dbo].[BvFnPerson_Get]...';


GO
CREATE FUNCTION BvFnPerson_Get( @CallCenterId INT )
RETURNS TABLE
AS
RETURN 
(
	SELECT  * FROM BvPerson  a with( nolock ) WHERE CallCenterID = @CallCenterId
)
GO
PRINT N'Creating [dbo].[BvFnPerson_GetByTransferBatch]...';


GO
CREATE FUNCTION [dbo].[BvFnPerson_GetByTransferBatch]( @batchId INT )
RETURNS TABLE
AS
RETURN
(
	SELECT pr.PersonSID as Id FROM BvPersonRel pr 
	INNER JOIN BvTransferArrays ta ON (ta.BatchID = @batchId AND pr.ObjectSID = ta.ItemID)

	UNION ALL

	SELECT p.SID as Id FROM BvPerson p
	WHERE NOT EXISTS(SELECT 1 from BvTransferArrays ta WHERE ta.BatchID = @batchId)
)
GO
PRINT N'Creating [dbo].[BvFnPersonOrGroupAssignmentOnSurvey_Get]...';


GO
CREATE FUNCTION BvFnPersonOrGroupAssignmentOnSurvey_Get( @CallCenterId INT )
RETURNS TABLE
AS
RETURN 
(
	SELECT  * FROM BvPersonOrGroupAssignmentOnSurvey  a with( nolock ) WHERE CallCenterID = @CallCenterId
)
GO
PRINT N'Creating [dbo].[BvFnSurvey_GetByCallCenterId]...';


GO
CREATE FUNCTION [dbo].[BvFnSurvey_GetByCallCenterId]
(
	@CallCenterId int
)
RETURNS TABLE
AS
RETURN
(	
	SELECT s.* 
	FROM [BvSurvey]  s LEFT JOIN [BvSurveyAssignmentOnCallCenter] sa ON s.SID = sa.SurveyId AND sa.CallCenterId = @CallCenterID 
	WHERE @CallCenterID IS NULL OR sa.CallCenterId IS NOT NULL
)

GO
CREATE FUNCTION [dbo].[GetCallByCondition]
(   @SurveySid INT,
	@ExplicitSID INT,
	@ConditionValue INT,
	@Now DATETIME) 
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
				BvSvySchedule.ConditionValue <> 0 AND 
				BvSvySchedule.TimeInShift < @Now
		ORDER BY Priority DESC, TimeInShift, ExplicitType DESC, CallOrder )
GO
PRINT N'Creating [dbo].[GetCallBySurvey]...';


GO
CREATE FUNCTION [dbo].[GetCallBySurvey]
(   
    @SurveySid INT,
    @ExplicitSID INT,
	@Now DATETIME) 
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
                    ConditionValue <> 0 AND
					TimeInShift < @Now
            ORDER BY Priority DESC, TimeInShift, ExplicitType DESC, CallOrder )
GO
PRINT N'Altering [dbo].[BvSpLookUpByPerson_ForCallGroup]...';


GO
ALTER PROCEDURE [dbo].[BvSpLookUpByPerson_ForCallGroup]
	@SurveyID INT,
	@CallGroupID INT,
	@PersonID INT,
	@Now DATETIME
AS
	DECLARE @interviewId INT
	DECLARE @rowCount INT
	DECLARE @CallID INT
	DECLARE @ConditionValue INT
		    
	;WITH ExplicitSIDs AS
	(
		SELECT p.ObjectSID as ExplicitSID FROM BvLoginGroup p WHERE p.PersonSID = @personId AND p.ObjectSID IN ( @SurveyID, @personId )
	),
	conditions AS
	(
		SELECT ExplicitSID, ConditionValue, ConditionPriority, RotatePriority FROM ExplicitSIDs
		INNER JOIN BvCallGroupConditionPerSurvey cgc ON cgc.SurveyId = @SurveyID AND cgc.CallGroupId = @CallGroupID 
	),
	calls as
	(
		SELECT TOP(1) cc.* FROM conditions c
		CROSS APPLY dbo.GetCallByCondition( @surveyId, c.ExplicitSID, c.ConditionValue, @Now ) cc
		ORDER BY Priority DESC, ConditionPriority DESC, RotatePriority ASC, TimeInShift, ExplicitType DESC, CallOrder
	)
	UPDATE calls WITH(READPAST)
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
PRINT N'Altering [dbo].[BvSpLookUpByPerson_ForSurvey]...';


GO
ALTER PROCEDURE [dbo].[BvSpLookUpByPerson_ForSurvey]
      @surveyId int,
      @personId int,
	  @Now DATETIME
AS
    DECLARE @CallID INT
    DECLARE @interviewId INT
    DECLARE @rowCount INT
    
    ;WITH ExplicitSIDs AS
    (
            SELECT p.ObjectSID FROM BvLoginGroup p WHERE p.PersonSID = @personId
    )
    ,calls AS
      (
            SELECT TOP(1) cc.*
            FROM ExplicitSIDs e
            CROSS APPLY [dbo].[GetCallBySurvey](@surveyId, e.ObjectSID, @Now ) cc
            ORDER BY Priority DESC, TimeInShift, ExplicitType DESC, CallOrder
      )
      UPDATE calls WITH(READPAST)
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
PRINT N'Altering [dbo].[BvViewPersonAndGroup]...';


GO
ALTER VIEW BvViewPersonAndGroup AS
    SELECT  SID, 
		CallCenterID,
        Name, 
        0           IsGroup,
        FullName,
        Description
        FROM    BvPerson
    UNION
    SELECT  BvPersonGroup.SID, 
		0			CallCenterID,
        Name, 
        1           IsGroup,
        ''          FullName,
        ''          Description
    FROM    BvPersonGroup
GO
PRINT N'Altering [dbo].[BvSpAlert_RecalculateAll]...';


GO
ALTER PROCEDURE [dbo].[BvSpAlert_RecalculateAll]
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
            BvAggregateSurveyAlertStatus.ScheduledCallsCount = BvAggregateSurvey.ScheduledCallsCount,
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
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( BvAggregateSurvey.ScheduledCallsCount-ISNULL(/*CachedCalls.Cnt*/0, 0), @AmberOfScheduledCallsCount, @RedOfScheduledCallsCount ) as scc
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( BvAggregateSurvey.SuspendedCallsCount-BvAggregateSurvey.ScheduledCallsCount, @AmberOfSuspendedCallsCount, @RedOfSuspendedCallsCount ) as succ
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( BvAggregateSurvey.MinutesSpentWorkingOnSurvey, @AmberOfMinutesSpentWorkingOnSurvey, @RedOfMinutesSpentWorkingOnSurvey ) as mswos
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( AssignedInterviewers.cnt, @AmberOfAssignedInterviewersCount, @RedOfAssignedInterviewersCount ) as aic
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( tt.StrikeRate, @AmberOfStrikeRate, @RedOfStrikeRate ) as sr
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( tt.CountCalls, @AmberOfCountCalls, @RedOfCountCalls ) as cc	
RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpGetSurveyActivityWithAlerts]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetSurveyActivityWithAlerts]
   @BatchID int, @onlyActiveSurveys bit
AS  
    SELECT asas.[SID] as SurveySID,
               asas.[Name] as ProjectID,
           asas.[Description]  as SurveyName,
           asas.[InterviewersLoggedCount],
           asas.[InterviewersLoggedCountPrev],
           asas.[NextAppointmentTime],
           asas.[TotalSampleSize], -- count of interview with 'fresh sample' its
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
PRINT N'Altering [dbo].[BvSpGetSystemWideInfo]...';


GO
ALTER PROCEDURE BvSpGetSystemWideInfo
   @BatchID INT,
   @CallCenterID INT
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
                                                  
        SELECT @totalInterviewers = COUNT(DISTINCT Person.SID) FROM BvFnPerson_Get(@CallCenterID)  Person INNER JOIN 
					 BvMembership ON Person.SID = ObjectSID INNER JOIN 
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
PRINT N'Altering [dbo].[BvSpCallHistory_List]...';


GO
ALTER PROCEDURE [dbo].[BvSpCallHistory_List]
@InterviewID     INTEGER,
@SurveyID        INTEGER,
@CallCenterID	 INTEGER
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
          'IsHistoryItemForChildInterview' = CAST(0 AS BIT),
		  ISNULL( BvCallCenter.Name, '' ) as CallCenterName
     FROM BvHistory
     INNER JOIN BvSurvey ON BvSurvey.SID = BvHistory.SurveyId
     INNER JOIN BvState ON BvState.StateGroupID = BvSurvey.StateGroupID AND BvState.[StateID] = BvHistory.ITS
     LEFT JOIN BvPerson ON BvPerson.SID = BvHistory.PersonSID
     LEFT JOIN BvRole ON BvRole.RoleID = BvHistory.RoleID
     LEFT JOIN BvAppointment ON BvAppointment.[ID] = BvHistory.AppointmentID
	 LEFT JOIN BvCallCenter ON BvCallCenter.ID = BvHistory.CallCenterID
     INNER JOIN BvInterview  ON ( BvInterview.[ID] = @InterviewID ) AND
        BvInterview.SurveySID = @SurveyID
     LEFT JOIN BvTimezone ON BvTimezone.[ID] = BvInterview.TimezoneID
     WHERE BvHistory.InterviewID = BvInterview.[ID]
                      AND BvHistory.SurveyId = @SurveyID
     ORDER BY DATEADD( s, -ConfirmitDuration, FiredTime)

RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpCallHistoryData]...';


GO
ALTER PROCEDURE [dbo].[BvSpCallHistoryData]
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
         [h].[WaitingTime] AS [WaitingTime],
		 [vcc].Name AS [CallCenterName]

        FROM      [BvHistory] [h] 
        INNER JOIN [BvSurvey]  [s] ON [h].[SurveyId] = [s].[SID] AND [s].State in (0, 1)
		LEFT JOIN [BvCallCenter] [vcc] ON [h].[CallCenterID] = [vcc].ID
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
PRINT N'Altering [dbo].[BvSpHistory_CfData_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpHistory_CfData_Insert]
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
            [RoleID],
			[CallCenterID]
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
		@RoleID          /*RoleID*/,
		ISNULL( it.CallCenterID, 0 )
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
PRINT N'Altering [dbo].[BvSpInterviewTimings_Delete]...';


GO
ALTER PROCEDURE [dbo].[BvSpInterviewTimings_Delete]
	@InterviewId INT, 
    @SurveyId INT,
    @PersonId INT,
    @TaskStartTime DATETIME,
    @TaskDeliveredTime DATETIME,
	@CallCenterID INT
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

      INSERT INTO BvHistory(FiredTime, SurveyId, RoleID, PersonSID, WaitingTime, Duration, CallCenterID)
      VALUES(@UtcNow, @SurveyId, 2, @PersonId, @WaitingTime, @InterviewDuration, @CallCenterID)
   END

   DELETE FROM BvInterviewTimings
   WHERE InterviewID = @InterviewId AND
         SurveyID = @SurveyId

RETURN 0
GO
PRINT N'Altering [dbo].[BvSpInterviewTimings_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpInterviewTimings_Insert]
	@personID INT,
	@utcNow DATETIME,
	@startTime DATETIME OUTPUT,
	@timeCallDelivered DATETIME OUTPUT
AS
	DECLARE @InterviewID INT
	DECLARE @SurveySID INT
	DECLARE @CallCenterID INT

	UPDATE BvTasks
	SET @StartTime = StartTime,
	    @TimeCallDelivered = TimeCallDelivered,
	    @InterviewID = InterviewID,
	    @SurveySID = SurveySID,
		@CallCenterID = CallCenterID,
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
	
	INSERT INTO BvInterviewTimings(InterviewID, SurveyID, TimeCallDelivered, InterviewDuriationTime, WaitingTime, CallCenterID)
	VALUES(@InterviewID, @SurveySID, @TimeCallDelivered, @InterviewDuriationTime, @WaitingTime, @CallCenterID)
	
RETURN 0
GO
PRINT N'Altering [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]
	@SurveySID INT,
	@Count  INT,  --number of requested calls
	@SuitableTimeForCalls DATETIME
AS

	DECLARE @Groups TABLE(
		[ObjectSid] [int] NOT NULL,
		[GroupSize] [int] NOT NULL)
		
    DECLARE @MinDistributedCalls INT = 5
	
	INSERT INTO @Groups
    SELECT c.ExplicitSID, 
           COUNT(*) GroupSize --should we limit this value as it was limited during filling bvcachedcalls.
    FROM BvSvySchedule c
	INNER JOIN vLogins v on c.ExplicitSID = v.sid AND
	                        c.SurveySID = @SurveySID AND
                            c.CallState = 2 AND
							TimeInShift <= @SuitableTimeForCalls AND
		                    c.IsInActiveShiftType = 1
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
        [ObjectSid] [int] NOT NULL,
        [ID] [int] NOT NULL, 
        [Interview] [int] NOT NULL,
        [TimeInShift] [datetime] NOT NULL,
		[Priority] [INT] NOT NULL,
	    [CallOrder] [INT] NOT NULL,
		[ApptID] [INT])
        
	;WITH orderedUpdateTable as
	(    
		SELECT calls.*
		FROM @Groups groups
		CROSS APPLY dbo.GetCallsForGroupForPredictiveSurvey( 
			groups.GroupSize, @SurveySID, groups.ObjectSid, @SuitableTimeForCalls) calls
	)
	UPDATE orderedUpdateTable
    SET CallState = -2 
	OUTPUT inserted.ExplicitSid,
		   inserted.[ID],
		   inserted.[interviewID],
		   inserted.[TimeInShift],
		   inserted.[Priority],
		   inserted.[CallOrder],
		   inserted.[ApptID]
	INTO @usedCalls
    
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
    FROM @usedCalls c
    INNER JOIN BvInterview i ON Interview = i.ID AND   --we should avoid this join. this field can be stored in bvsvyschedule or somewhere else
                                SurveySID = @SurveySID
    LEFT JOIN BvPerson p on p.SID = ObjectSid
	ORDER BY Priority DESC,
             TimeInShift,
 			 CallOrder
	
RETURN (@@ROWCOUNT)
GO
PRINT N'Altering [dbo].[BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned]
 @SurveySID INT,
 @Count  INT,  --number of requested calls
 @SuitableTimeForCalls DATETIME
AS

SET NOCOUNT ON

	DECLARE @FixeNumberCallsPerPerson INT = 10

      --it should be at end of the SP. it uses for decrease recompilation
	DECLARE @CachedCalls TABLE (
	  [ExplicitSID] [int] NOT NULL,
	  [ID] [int] NOT NULL,
	  [InterviewID] [int] NOT NULL,
	  [TimeInShift] [datetime] NULL,
	  [Priority] [int] NOT NULL,
	  [CallOrder] [int] NOT NULL,
	  [ApptId] [int])
        
	;WITH orderedUpdateTable AS
	(
		SELECT calls.*, ROW_NUMBER() over (partition by ExplicitSid order by Priority DESC, TimeInShift, CallOrder) rn
		FROM BvSvySchedule calls
		where CallState = 2 AND 
		      SurveySID = @SurveySID AND
			  TimeInShift <= @SuitableTimeForCalls AND
			  IsInActiveShiftType = 1
	)
    UPDATE orderedUpdateTable 
    SET CallState = -2 
	OUTPUT inserted.ExplicitSid,
		   inserted.[ID],
		   inserted.[interviewID],
		   inserted.[TimeInShift],
		   inserted.[Priority],
		   inserted.[CallOrder],
		   inserted.[ApptID]
	INTO @CachedCalls
    where ExplicitSid in(select PersonSID from BvTasks where SurveySID = @SurveySID ) and rn <= @FixeNumberCallsPerPerson

    SELECT c.[ID], 
           c.[ExplicitSid], 
           @SurveySID SurveySid, 
           i.DialingMode DiallingMode,
		   c.[InterviewID], 
		   i.[TelephoneNumber],
		   (CASE WHEN c.ApptId > 0 THEN c.[TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
		   0 as [GroupID]
    FROM @CachedCalls c 
    INNER JOIN BvInterview i ON c.[Interviewid] = i.[ID] AND
                                i.[SurveySID] = @SurveySID
    ORDER BY Priority DESC, TimeInShift, CallOrder
 
RETURN (@@ROWCOUNT)
GO
PRINT N'Altering [dbo].[BvSpGetInterviewerBreaks]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetInterviewerBreaks]
	@StartDate DATETIME, @EndDate DATETIME, @MaxRows int
AS

IF(@StartDate IS NULL) SET @StartDate = '01-01-1753 00:00:00'
IF(@EndDate IS NULL) SET @EndDate = '12-31-9999 23:59:59.997'

SELECT TOP (@MaxRows)
	[h].[ID] AS [ID],
	[h].[Duration] AS [Duration],
	[h].[InterviewerId] AS [InterviewerId],
	[h].[StartTime] AS [StartTime],
	[p].[Name] AS [InterviewerName],
	[vcc].[Name] AS [CallCenterName]
FROM 
	BvTimeBreaksHistory [h]
LEFT JOIN BvPerson [p] ON [p].SID = [h].[InterviewerId]
LEFT JOIN [BvCallCenter] [vcc] on [vcc].[ID] = [h].[CallCenterId]
WHERE 
	[h].[StartTime] >= @StartDate AND
	[h].[StartTime] < @EndDate
          
RETURN 0
GO
PRINT N'Altering [dbo].[BvSpGetListSurveyTasks]...';


GO
ALTER PROCEDURE BvSpGetListSurveyTasks
   @surveysBatchID int,
   @interviewersBatchID int,   
   @TimeZoneID INT,
   @CallCenterID INT
AS
   DECLARE @currTime DATETIME
   SET @currTime = GETUTCDATE()
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
          (CASE WHEN t.InterviewID = 0 THEN NULL ELSE ISNULL(DATEDIFF(second, TimeStateChanged, GETUTCDATE()), 0) END) as SecondsSinceLastSubmission, 
          (CASE WHEN InterviewID > 0 
				THEN tsc.val
				ELSE 0
			END) LastSubmissionAlert, 
          t.LastKeepAliveTime,
          (CASE WHEN LastKeepAliveTime IS NULL 
				THEN 2 
				ELSE lkat.val
			END) LastKeepAliveTimeAlert,
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
   LEFT JOIN BvTransferArrays ta ON (ta.BatchID = @surveysBatchID AND
                                      t.SurveySID = ta.ItemID)
   INNER JOIN dbo.BvFnPerson_GetByTransferBatch(@interviewersBatchID) pta ON pta.Id = t.PersonSID
   OUTER APPLY dbo.GetLastTimeBreak(t.PersonSID) lb
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, LastKeepAliveTime, GETUTCDATE()), @AmberOfLastKeepAliveTime, @RedOfLastKeepAliveTime) as lkat
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, TimeStateChanged, GETUTCDATE()), @AmberOfLastSubmission, @RedOfLastSubmission ) as tsc
   WHERE (s.SID IS NOT NULL and ta.ItemID IS NOT NULL ) OR t.SurveySID = 0) as tsk
   INNER JOIN BvFnPerson_Get(@CallCenterID) p ON (tsk.PersonSID = p.SID)
   INNER JOIN BvTimezone tz ON ((CASE WHEN TzID = 0 THEN @TimeZoneID ELSE TzId END) = tz.ID)
   LEFT JOIN BvPersonMonitoring pm ON (pm.PersonSID = tsk.PersonSID)
GO
PRINT N'Altering [dbo].[BvSpGetPersonGroupsLevel]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetPersonGroupsLevel]
 @ParentSID INT,
 @Filter NVARCHAR(MAX) = NULL,
 @CallCenterID INT
AS
	SELECT
		[g].[SID] AS [SID],
		[g].[Name] AS [Name],
		(	
			SELECT COUNT(*)
				FROM [BvMembership] [m1]
				LEFT JOIN BvFnPerson_Get(@CallCenterID) [p] ON [p].[SID] = [m1].[ObjectSID]
				WHERE [m1].[ContainerSID] = [g].[SID] AND 
					[p].[Name] <> '' AND 
					(@Filter IS NULL OR [p].[Name] LIKE @Filter)
		) AS [Count]
		FROM [BvPersonGroup] [g]
		LEFT JOIN [BvMemberShip] [m] ON [g].[SID] = [m].[ObjectSID]
		WHERE [m].[ContainerSID] = @ParentSID AND  
		  [g].[Name] <> '' AND  
		  (@Filter IS NULL OR [g].[Name] LIKE @Filter)
GO
PRINT N'Altering [dbo].[BvSpGetPersonsLevel]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetPersonsLevel]
 @ParentSID INT,
 @Filter NVARCHAR(MAX) = NULL,
 @CallCenterID INT
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
 BvFnPerson_Get(@CallCenterID) [p]
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
PRINT N'Altering [dbo].[BvSpLookUpByPerson_ForManualMode]...';


GO
ALTER PROCEDURE [dbo].[BvSpLookUpByPerson_ForManualMode]
	@surveyId int,
	@interviewId int,
	@personId int
AS
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

	DECLARE @PersonAssignmentsListMode INT;
	SELECT @PersonAssignmentsListMode = AssignmentsListMode FROM BvPerson WHERE SID = @personId

	;WITH call AS
	(
		SELECT c.*
		FROM BvSvySchedule c
		INNER JOIN BvPersonRel p ON p.PersonSID = @personId
		WHERE CallState = 2 AND
		      SurveySid = @surveyId AND
		      InterviewId = @interviewId AND
			  (@PersonAssignmentsListMode = 1 OR p.ObjectSID = c.ExplicitSID)
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
	INTO @Call
	
	UPDATE BvAppointment 
	SET State = 2 
	WHERE State = 1 AND 
	      SurveysId = @surveyId AND 
	      InterviewSid = @interviewId
	      
	SELECT * FROM @Call
RETURN 0
GO
PRINT N'Altering [dbo].[BvSpPerson_Delete]...';


GO
ALTER PROCEDURE [dbo].[BvSpPerson_Delete]
 @SID int
AS
    EXEC BvSpMembership_Delete 0, @SID

    DELETE FROM BvNumber WHERE ObjectSID = @SID AND ClassID = 10

    DELETE  BvPerson WHERE SID = @SID

    DELETE FROM BvPersonRel WHERE PersonSID = @SID

	DELETE FROM BvPersonFailedLoginAttempts	WHERE PersonId = @SID

    -- delete implicit assigments
    DELETE FROM BvPersonOrGroupAssignmentOnSurvey WHERE PersonOrGroupId = @SID

    UPDATE BvSvySchedule 
    SET ExplicitSID = BvSvySchedule.SurveySID, 
        ExplicitType = 1
    WHERE ExplicitSID = @SID
GO
PRINT N'Altering [dbo].[BvSpPerson_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpPerson_Insert]
        @SID INT, 
        @Name NVARCHAR( 255 ),  
        @FullName NVARCHAR( 255 ),
        @Description NVARCHAR( 255 ),
        @ManualSelection INT,
        @AssignmentsListMode INT,
        @BvID INT,
        @PwdSaltTxt NVARCHAR(256),
		@CallGroupId INT,
		@CallCenterID INT,
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
		CallCenterID,
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
		@CallCenterID,
                @Location)

INSERT BvPersonFailedLoginAttempts( PersonId, Count ) VALUES( @SID, 0 )

RETURN 0
GO
PRINT N'Altering [dbo].[BvSpPerson_ListByParent]...';


GO
ALTER PROCEDURE [dbo].[BvSpPerson_ListByParent]
	@ParentSID int,
	@CallCenterID INT
AS
	SELECT  
        BvPerson.SID AS [SID],
        10 AS [ClassID], /* BVDBS_PERSON */
        BvPerson.[Name] AS [Name],
  ISNULL(BvTasks.[SurveySID], 0) AS [SurveySID],
  ISNULL(BvTasks.[InterviewID], 0) AS [InterviewID],
  2 AS [RoleID] /* always CATI */  
        FROM  BvPerson
  LEFT JOIN BvTasks
	ON BvTasks.PersonSID = BvPerson.SID
  INNER JOIN BvMembership 
	ON BvPerson.SID = BvMembership.ObjectSID
  WHERE BvMembership.ContainerSID = @ParentSID AND ( BvPerson.CallCenterID = @CallCenterID OR @CallCenterID = 0 )
  ORDER BY ClassID DESC
GO
PRINT N'Altering [dbo].[BvSpPerson_Update]...';


GO
-- TODO: remove this procedure at all 
ALTER PROCEDURE [dbo].[BvSpPerson_Update]
 @SID int, 
 @Name nvarchar( 255 ),  
 @FullName nvarchar( 255 ),
 @Description nvarchar( 255 ),
 @ManualSelection int,
 @BvID int,
 @AutoSurveyId int,
 @AllowedChoices INT = NULL,
 @CallCenterID INT
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
		CallCenterID = @CallCenterID
        WHERE   SID = @SID

IF ISNULL( @BvID, 0 ) > 0
 UPDATE BvNumber SET BvID = @BvID 
 WHERE ObjectSID = @SID AND ClassID = 10

RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpPersonAndGroups_List]...';


GO
ALTER PROCEDURE [dbo].[BvSpPersonAndGroups_List]
        @ParentSID int,
        @SurveySid int,
        @Filter nvarchar(max) = NULL, -- Part of person's or group's name to filter by.
		@CallCenterID INT
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
			(select count( distinct s.SID) from  BvSurvey s, BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID) a
				where  s.SID = a.SurveyId and a.PersonOrGroupId = p.SID  and s.State <> 2)
				as TotalAssignedSurveys 
            FROM   BvFnPerson_Get(@CallCenterID) p
            WHERE  p.SID IN (   SELECT  ObjectSID
                        FROM    BvMembership
                        WHERE   ContainerSID = @ParentSID )
                   AND (@Filter is NULL OR p.Name LIKE (@Filter) )

      UNION 

      select pg.sid     as SID,
             pg.name    as UserName,
             1          as isGroup,
       (SELECT COUNT(*) FROM BvMembership
              LEFT JOIN BvFnPerson_Get(@CallCenterID) p1 ON p1.SID = BvMembership.ObjectSID
     WHERE ContainerSID = pg.sid
           AND (@Filter is NULL OR p1.Name LIKE (@Filter) ) ) as MembersCount,
    1 as IsAssignedOnCurrentSurvey,
             0          as CurSurvAssign,
             0          as AllSurvAssign,
   (select count( distinct s.SID) from  BvSurvey s, BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID) a
    where  s.SID = a.SurveyId and a.PersonOrGroupId = pg.SID and s.State <> 2)
                       as TotalAssignedSurveys

      from BvPersonGroup pg
      where pg.Sid in ( SELECT  ObjectSID
                        FROM    BvMembership
                        WHERE   ContainerSID = @ParentSID ) 
							AND pg.SID <> 4 /* Exclude '[All]' group. */
							AND (@Filter is NULL OR pg.Name LIKE (@Filter) )
GO
PRINT N'Altering [dbo].[BvSpSendMessageToGroups]...';


GO
ALTER PROCEDURE [dbo].[BvSpSendMessageToGroups]
	@BatchId int,	
	@OnlineOnly bit,
    @MessageBody nvarchar(1024),	
	@MessageSupervisorName nvarchar(50),
	@CallCenterID INT
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
			BvFnPerson_Get(@CallCenterID) AS p
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
PRINT N'Altering [dbo].[BvSpSendMessageToInterviewers]...';


GO
ALTER PROCEDURE [dbo].[BvSpSendMessageToInterviewers]
	
	@BatchId int,	
	@OnlineOnly bit,
    @MessageBody nvarchar(1024),	
	@MessageSupervisorName nvarchar(50)    
AS

BEGIN

	DECLARE @MessageId int
	INSERT INTO BvMessages (Body, CreateTime, SupervisorName) VALUES(@MessageBody, GETUTCDATE(), @MessageSupervisorName);
	SET @MessageId = SCOPE_IDENTITY();

	DECLARE @MessageToPerson TABLE( MessageId INT, InterviewerId INT )

	UPDATE BvPerson SET HasNewMessage = 1 
		OUTPUT @MessageId, inserted.SID INTO @MessageToPerson (MessageId, InterviewerId) 
		FROM BvPerson p
		LEFT JOIN BvTasks t ON p.SID = t.PersonSID
		INNER JOIN bvTransferArrays ON (p.[SID] = ItemId AND BatchId = @BatchId)
		WHERE t.PersonSID IS NOT NULL OR @OnlineOnly <> 1

	INSERT INTO BvMessageToPerson (MessageId, InterviewerId)  SELECT  MessageId, InterviewerId FROM @MessageToPerson
END
GO
PRINT N'Altering [dbo].[BvSpSendMessageToSurveys]...';


GO
ALTER PROCEDURE  [dbo].[BvSpSendMessageToSurveys]
	@BatchId int,	
    @MessageBody nvarchar(1024),
	@MessageSupervisorName nvarchar(50),
	@CallCenterID INT
AS

BEGIN

	DECLARE @MessageId int
	INSERT INTO BvMessages (Body, CreateTime, SupervisorName) VALUES(@MessageBody, GETUTCDATE(), @MessageSupervisorName);
	SET @MessageId = SCOPE_IDENTITY();

	/* Survey group contains all interviewer working on survey*/
	BEGIN TRANSACTION
			DECLARE @MessageToPerson TABLE( MessageId INT, InterviewerId INT )

			UPDATE BvPerson 
				SET HasNewMessage = 1
			OUTPUT @MessageId, inserted.SID INTO @MessageToPerson (MessageId, InterviewerId)
			FROM											
				BvPerson as p
				INNER JOIN  bvTasks as t ON p.SID = t.PersonSID
				INNER JOIN 	bvTransferArrays a ON t.SurveySID = a.ItemId 
			WHERE p.CallCenterID = @CallCenterID AND a.BatchId = @BatchId

			INSERT INTO BvMessageToPerson(MessageId, InterviewerId) SELECT MessageId, InterviewerId FROM @MessageToPerson

	COMMIT TRANSACTION
	
END
GO
PRINT N'Altering [dbo].[BvSpSurvey_GetAssignedPersonList]...';


GO
ALTER PROCEDURE [dbo].[BvSpSurvey_GetAssignedPersonList]
    @SurveySID INT,
    @RoleID INT,
	@CallCenterID INT
AS
 SELECT 
      p.SID AS PersonId,
      p.Name AS PersonName
  FROM BvFnPerson_Get(@CallCenterID) p, BvPersonRel r with(
  nolock ), BvSurvey s with( nolock )
  where p.SID = r.PersonSID and r.Type = 2 and r.RoleID = @RoleID and
  r.ObjectSID = s.SID and s.SID = @SurveySID
  ORDER BY p.SID
GO
PRINT N'Altering [dbo].[BvSpTasks_InsertUpdate_2]...';


GO
ALTER PROCEDURE [dbo].[BvSpTasks_InsertUpdate_2]
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
        DiallingMode = @DiallingMode,
		StationExtensionNumber = @ExtensionNumber
WHERE PersonSID = @PersonSID

RETURN 0
GO
PRINT N'Altering [dbo].[BvSpTasks_SetTelephonyProblemForLoggedIn]...';


GO
ALTER PROCEDURE [dbo].[BvSpTasks_SetTelephonyProblemForLoggedIn]
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
	  AND DialerId = @DialerId
END
GO
PRINT N'Altering [dbo].[BvSpAssignment_List]...';


GO
ALTER PROCEDURE [dbo].[BvSpAssignment_List]
    @SurveySID INT,
	@CallCenterID INT
AS
SET NOCOUNT ON
    IF @SurveySID <> 0 
    BEGIN
        SELECT BvPersonOrGroupAssignmentOnSurvey.Id AS AssignmentSID,
                BvSurvey.SID AS SurveySID,
                BvSurvey.[Name] AS SurveyName,
                0 AS IsSurveyGroup,
                0 AS Counts,
                BvPersonOrGroupAssignmentOnSurvey.PersonOrGroupId AS PersonSID,
                BvViewPersonAndGroup.[Name] AS Name,
                BvViewPersonAndGroup.IsGroup AS IsPersonGroup
        FROM BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID) BvPersonOrGroupAssignmentOnSurvey, BvSurvey, BvViewPersonAndGroup
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
        SELECT BvPersonOrGroupAssignmentOnSurvey.Id AS AssignmentSID,
            BvSurvey.SID AS SurveySID,
            BvSurvey.[Name] AS SurveyName,
            0 AS IsSurveyGroup,
            0 AS Counts,
            BvPersonOrGroupAssignmentOnSurvey.PersonOrGroupId AS PersonSID,
            BvViewPersonAndGroup.[Name] AS Name,
            BvViewPersonAndGroup.IsGroup AS IsPersonGroup
        FROM    BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID) BvPersonOrGroupAssignmentOnSurvey, BvSurvey, BvViewPersonAndGroup
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
PRINT N'Altering [dbo].[BvSpPerson_GetAssignedSurveyList]...';


GO
ALTER PROCEDURE [dbo].[BvSpPerson_GetAssignedSurveyList]
@PersonSID INT, @UserName NVARCHAR (MAX)=NULL, @CallCenterID INT
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
  left join BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID) a on a.SurveyId = s.SID and a.PersonOrGroupId = @PersonSID 
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
PRINT N'Altering [dbo].[BvSpPerson_SpinUp]...';


GO
ALTER  PROCEDURE [dbo].[BvSpPerson_SpinUp]
    @PersonSID INT
AS
	--if person is not found then we use 0 call center id, because person group is global.
	DECLARE @CallCenterID TINYINT = ISNULL( (SELECT CallCenterID FROM BvPerson WHERE SID = @PersonSID ), 0 )
    
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
        select distinct a.SurveyId, 2, 2 from BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID) a
		inner join @temp temp
		ON a.PersonOrGroupId = temp.sid
        where a.CallCenterID = @CallCenterID
    
    delete from BvPersonRel where PersonSID = @PersonSID
    insert into BvPersonRel( PersonSID, ObjectSID, RoleID, Type )
        select @PersonSID, sid, role_id, type from @temp
            
    EXEC BvSpLogin_SpinUp @PersonSID
RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpPersonGroup_Delete]...';


GO
ALTER PROCEDURE [dbo].[BvSpPersonGroup_Delete]
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


RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpSurvey_Delete]...';


GO
ALTER PROCEDURE [dbo].[BvSpSurvey_Delete]
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
PRINT N'Altering [dbo].[BvSpSurvey_GetListByFolder]...';


GO
ALTER  procedure [dbo].[BvSpSurvey_GetListByFolder]
 @UserName NVARCHAR(MAX) = NULL,
 @Filter NVARCHAR(MAX) = NULL,
 @CallCenterId INT

as

SELECT  
        BvSurvey.SID    AS    [SID],
        BvSurvey.Name   AS    [ConfirmitID],
        BvSurvey.Description AS [Name], 
  (select count(distinct BvPersonOrGroupAssignmentOnSurvey.PersonOrGroupId) from BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID) BvPersonOrGroupAssignmentOnSurvey
        where BvPersonOrGroupAssignmentOnSurvey.SurveyId = BvSurvey.[SID]) 
         as TotalAssignedPersons 
FROM    [BvFnSurvey_GetByCallCenterId](@CallCenterId) [BvSurvey]
INNER JOIN [bvUserSurveyPermission] [p] ON BvSurvey.SID = [p].SurveySID
WHERE  p.UserName = @UserName AND 
       BvSurvey.[Description] <> '' AND 
       (@Filter IS NULL OR BvSurvey.[Description] LIKE @Filter + '%') AND
	   BvSurvey.State <> 2
GO
PRINT N'Altering [dbo].[BvSpDialer_Reset]...';


GO
ALTER PROCEDURE [dbo].[BvSpDialer_Reset]
    @ProblemID INT
AS  
    UPDATE BvSvySchedule 
    SET CallState = 2 
    WHERE CallState = -2

    UPDATE BvTasks 
    SET ProblemId = @ProblemID 
    WHERE LoggedInToDialerState = 1/*LOGGING_IN*/ OR 
          LoggedInToDialerState = 2/*LOGGED_IN*/
GO
PRINT N'Altering [dbo].[BvSpGetInterviewerPerformanceList]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetInterviewerPerformanceList] 
 @CallCenterId INT,
 @onlyLoggedIn bit
AS 

IF(@onlyLoggedIn = 0)	
		SELECT InterviewerId, 
			   InterviewerName,
			   InterviewingTime,
			   TotalInterviewCount, 
			   CompletedInterviewCount,
			   CompletedInLastHourCount 
		FROM BvInterviewerPerformance ip INNER JOIN BvFnPerson_Get(@CallCenterId) p ON ip.[InterviewerId] = p.[SID]
ELSE
		SELECT InterviewerId, 
			   InterviewerName,
			   InterviewingTime,
			   TotalInterviewCount, 
			   CompletedInterviewCount,
			   CompletedInLastHourCount 
		FROM BvTasks INNER JOIN BvInterviewerPerformance ip ON BvTasks.PersonSID = ip.[InterviewerId]
		             INNER JOIN BvFnPerson_Get(@CallCenterId) p ON ip.[InterviewerId] = p.[SID]
GO
PRINT N'Altering [dbo].[BvSpStartInterviewerBreak]...';


GO
ALTER  PROCEDURE [dbo].[BvSpStartInterviewerBreak]
    @InterviewerId INT    
AS
BEGIN
	DECLARE @CallCenterId INT = (SELECT CallCenterID FROM BvPerson WHERE SID = @InterviewerId)
	INSERT INTO BvTimeBreaksHistory (InterviewerId, StartTime, CallCenterId) VALUES (@InterviewerId, GETUTCDATE(), @CallCenterId)
END
GO
PRINT N'Altering [dbo].[BvSpTask_UpdateActiveQuestion]...';


GO
ALTER PROCEDURE BvSpTask_UpdateActiveQuestion
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
PRINT N'Altering [dbo].[BvSpTasks_Update_2]...';


GO
ALTER PROCEDURE [dbo].[BvSpTasks_Update_2]
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
PRINT N'Altering [dbo].[BvSpTasks_UpdateInterviewState]...';


GO
ALTER PROCEDURE [dbo].[BvSpTasks_UpdateInterviewState]
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
PRINT N'Altering [dbo].[BvSpAssignment_Insert2]...';


GO
ALTER PROCEDURE [dbo].[BvSpAssignment_Insert2]
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

    exec BvSpAddUniqueAssignment @PersonSID

RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpCall_ChangePriority]...';


GO
ALTER  PROCEDURE [dbo].[BvSpCall_ChangePriority]
    @SurveySID INTEGER,
    @Priority INTEGER,
    @BatchID INTEGER
AS
   UPDATE BvSvySchedule 
   SET Priority = @Priority,
       OldPriority = 0
   FROM BvTransferArrays ta
   WHERE ta.BatchID = @BatchID AND 
         ta.ItemID = [InterviewID] AND
		 [SurveySID] = @SurveySID AND
         CallState > 0
RETURN(0)
GO
PRINT N'Altering [dbo].[BvSpCall_ChangeShiftType]...';


GO
ALTER PROCEDURE [dbo].[BvSpCall_ChangeShiftType]
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
            FROM  BvTransferArrays ta
            INNER JOIN BvInterview i ON i.[ID] = ta.ItemID AND 
                                        i.SurveySID = @SurveySID
            LEFT JOIN AvailableTz atz ON atz.tz_id = i.TimezoneID OR 
                                         ( i.TimezoneID IS NULL AND atz.tz_id = @SiteTimeZoneID )
            WHERE atz.tz_id IS NULL ) i

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
		OUTPUT inserted.InterviewID, inserted.SurveySID
			INTO BvCachedCallsInsert
        FROM BvSvySchedule
        INNER JOIN BvTransferArrays ON BatchID = @BatchID
            AND ItemID = BvSvySchedule.[InterviewID] AND BvSvySchedule.SurveySID = @SurveySID
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
			OUTPUT inserted.InterviewID, inserted.SurveySID
			INTO BvCachedCallsInsert
            FROM BvSvySchedule
            INNER JOIN BvInterview ON BvInterview.SurveySID = @SurveySID
                AND BvSvySchedule.InterviewID = BvInterview.[ID]
            INNER JOIN BvShiftZones ON BvShiftZones.ShiftTypeID = @ShiftTypeID
                AND ISNULL(BvInterview.TimezoneID, 0 ) = BvShiftZones.TimeZoneID
            INNER JOIN BvTransferArrays ON BvTransferArrays.BatchID = @BatchID
                AND ItemID = BvSvySchedule.InterviewID AND BvSvySchedule.SurveySID = @SurveySID
            WHERE BvSvySchedule.CallState > 0
        ELSE--[None]
            UPDATE BvSvySchedule 
            SET ShiftTypeID = @ShiftTypeNone,
                Priority = CASE WHEN OldPriority > 0 THEN OldPriority ELSE Priority END,
                OldPriority = 0
			OUTPUT inserted.InterviewID, inserted.SurveySID
				INTO BvCachedCallsInsert
            FROM BvSvySchedule
            INNER JOIN BvTransferArrays ON BvTransferArrays.BatchID = @BatchID
                AND ItemID = BvSvySchedule.[InterviewID] AND BvSvySchedule.SurveySID = @SurveySID
            WHERE BvSvySchedule.CallState > 0
    END
    
RETURN(0)
GO
PRINT N'Altering [dbo].[BvSpCall_Get]...';


GO
ALTER PROCEDURE [dbo].[BvSpCall_Get]
    @SurveyID int,
    @InterviewID int,
    @Delete int,
    @GetLiveCall int = 0
AS
	DECLARE @OldCallState INT
	DECLARE @IsLockObtained INT = 0

	IF @Delete = 1
	BEGIN
       
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
PRINT N'Altering [dbo].[BvSpCall_MoveToITS]...';


GO
ALTER PROCEDURE [dbo].[BvSpCall_MoveToITS]
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

   EXEC BvSpDeleteTransfer @BatchID

RETURN @ProcessedCalls
GO
PRINT N'Altering [dbo].[BvSpCalls_Delete_Batch]...';


GO
ALTER PROCEDURE [dbo].[BvSpCalls_Delete_Batch]
	@surveySid INT,
	@batchId INT
AS    
 DECLARE @InterviewIds TABLE(Id INT)
    
 INSERT INTO @InterviewIds
 SELECT ItemID
 FROM BvTransferArrays ta
 WHERE BatchId = @batchID 
       
 UPDATE BvSvySchedule 
 SET CallState = 0
 FROM @InterviewIds iids
 WHERE SurveySID = @SurveySID AND
       iids.ID = InterviewId
GO
PRINT N'Altering [dbo].[BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnly]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnly]
 @SurveySID INT,
 @Count  INT, --number of requested calls
 @SuitableTimeForCalls DATETIME
AS
--best if it should be established at the connection level
--it may influence on count of recompilations
SET NOCOUNT ON

	DECLARE @FixeNumberCallsPerPerson INT = 10

      --it should be at end of the SP. it uses for decrease recompilation
	DECLARE @CachedCalls TABLE (
	  [ExplicitSID] [int] NOT NULL,
	  [ID] [int] NOT NULL,
	  [InterviewID] [int] NOT NULL,
	  [TimeInShift] [datetime] NULL,
	  [Priority] [INT] NOT NULL,
	  [CallOrder] [INT] NOT NULL,
	  [ApptID] [int] not null)
        
	;WITH orderedUpdateTable AS
	(
		SELECT TOP ( @Count ) *
		FROM BvSvySchedule
		WHERE SurveySID = @SurveySID AND
				ExplicitSid = @SurveySID AND 
				CallState = 2 AND
				TimeInShift <= @SuitableTimeForCalls AND
			    IsInActiveShiftType = 1
		ORDER BY Priority DESC,
                 TimeInShift,
				 CallOrder
	)
    UPDATE orderedUpdateTable
    SET CallState = -2 
	OUTPUT 0,
		   inserted.[ID],
		   inserted.[interviewID],
		   inserted.[TimeInShift],
		   inserted.[Priority],
		   inserted.[CallOrder],
		   inserted.[ApptID]
	INTO @CachedCalls

    SELECT c.[ID], 
           c.[ExplicitSid], 
           @SurveySID SurveySid, 
           i.DialingMode DiallingMode,
		   c.[InterviewID], 
		   i.[TelephoneNumber],
		   (CASE WHEN c.ApptId > 0 THEN c.[TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
		   0 as [GroupID]
    FROM @CachedCalls c 
    INNER JOIN BvInterview i ON c.[Interviewid] = i.[ID] AND
                                i.[SurveySID] = @SurveySID
    ORDER BY Priority DESC,
             TimeInShift,
 			 CallOrder
 
RETURN (@@ROWCOUNT)
GO
PRINT N'Altering [dbo].[BvSpGetCachedCallsForPredictiveSurveyByPersonGroup]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetCachedCallsForPredictiveSurveyByPersonGroup]
 @SurveySID INT,
 @GroupID INT,	
 @Count  INT, --number of requested calls
 @SuitableTimeForCalls DATETIME
AS
--best if it should be established at the connection level
--it may influence on count of recompilations
SET NOCOUNT ON

	DECLARE @FixeNumberCallsPerPerson INT = 10

      --it should be at end of the SP. it uses for decrease recompilation
	DECLARE @CachedCalls TABLE (
	  [ExplicitSID] [int] NOT NULL,
	  [ID] [int] NOT NULL,
	  [InterviewID] [int] NOT NULL,
	  [TimeInShift] [datetime] NULL,
	  [Priority] [INT] NOT NULL,
	  [CallOrder] [INT] NOT NULL,
	  [ApptID] [int] not null)
        
	;WITH orderedUpdateTable AS
	(
		SELECT TOP ( @Count ) *
		FROM BvSvySchedule 
		WHERE SurveySID = @SurveySID AND
				ExplicitSid = @groupID AND 
				CallState = 2 AND
				TimeInShift <= @SuitableTimeForCalls AND
			    IsInActiveShiftType = 1
		ORDER BY Priority DESC,
                 TimeInShift,
				 CallOrder
	)
    UPDATE orderedUpdateTable 
    SET CallState = -2 
	OUTPUT 0,
		   inserted.[ID],
		   inserted.[interviewID],
		   inserted.[TimeInShift],
		   inserted.[Priority],
		   inserted.[CallOrder],
		   inserted.[ApptID]
	INTO @CachedCalls

    SELECT c.[ID], 
           c.[ExplicitSid], 
           @SurveySID SurveySid, 
           i.DialingMode DiallingMode,
		   c.[InterviewID], 
		   i.[TelephoneNumber],
		   (CASE WHEN c.ApptId > 0 THEN c.[TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
		   @GroupID as [GroupID]
    FROM @CachedCalls c 
    INNER JOIN BvInterview i ON c.[Interviewid] = i.[ID] AND
                                i.[SurveySID] = @SurveySID
    ORDER BY Priority DESC,
             TimeInShift,
 			 CallOrder
 
RETURN (@@ROWCOUNT)
GO
PRINT N'Altering [dbo].[BvSpGetDeferredMonitoringListPage]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetDeferredMonitoringListPage] 
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
		'' AS TelephoneNumber,
		0 AS CallCenterId,
		'' AS CallCenterName
     
		RETURN 0;
	END
	
	DECLARE @StateGroupID INT
	SELECT @StateGroupID = MIN(ID) FROM BvStateGroup
	
	DECLARE @QueryBody as nvarchar(4000) = 'from 
		BvPersonDeferredMonitoring as def inner join BvSurvey as survey on def.SurveySID = survey.SID
		inner join BvUserSurveyPermission as perm on perm.SurveySID = def.SurveySID
		inner join BvPerson as person on person.SID = def.PersonSID
		inner join BvInterview as interview on interview.ID = def.InterviewID and interview.SurveySID = def.SurveySID
		left join BvCallCenter as vcc on def.[CallCenterId] = vcc.[ID]
		left join BvState as st on def.ExtendedStatus = st.StateID AND st.StateGroupID = '+ CAST(@StateGroupID AS NVARCHAR) +'
	where 
		def.IsComplete = 1 and perm.UserName = ''' + @userName + ''' and survey.State <> 2'

	DECLARE @Counter as nvarchar(4000) = 'select count(*) cnt '

	DECLARE @Query NVARCHAR(4000) = 'select def.ID, def.PersonSID, def.SurveySID, def.HasAudio, 
		def.InterviewID, def.ExtendedStatus, st.Name as ExtendedStatusName, def.TimeStamp, survey.Name as SurveyName, survey.Description as SurveyConfirmitName, 
		person.Name as PersonLogin,	person.FullName as PersonName, interview.RespondentName, interview.TelephoneNumber,
		def.CallCenterID, vcc.Name as CallCenterName ' + @QueryBody

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
PRINT N'Altering [dbo].[BvSpGetPersonsListPage]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetPersonsListPage]
 @ParentGroupsIDs NVARCHAR (MAX), 
 @PageIndex INT,
 @PageSize INT, 
 @OrderField NVARCHAR (64), 
 @IsOrderASC BIT, 
 @SearchCondition NVARCHAR (4000)=NULL,
 @CallCenterId INT
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
     FROM BvFnPerson_Get(' + CAST( @CallCenterId AS NVARCHAR(64)) + ') as BvPerson
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
PRINT N'Altering [dbo].[BvSpGetSurveys]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetSurveys]
 @Filter NVARCHAR(MAX) = NULL,
 @UserName NVARCHAR(MAX) = NULL,
 @CallCenterId INT
AS
SELECT DISTINCT
 [s].[SID] AS [SID],
 [s].[Name] AS [ConfirmitID],
 [s].[Description] AS [Name]
FROM    [BvFnSurvey_GetByCallCenterId](@CallCenterId) [s] 
left join [bvUserSurveyPermission] [p] on [s].[SID] = [p].[SurveySID]
INNER JOIN BvNumber n on n.ObjectSID = s.SID AND n.ClassID = 2
WHERE
     ( p.UserName = @UserName or @UserName is null)
 AND (@Filter IS NULL OR [s].[Description] LIKE @Filter)
 AND ( s.State <> 2)
GO
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpLookUpByPerson]...';


GO
ALTER PROCEDURE [dbo].[BvSpLookUpByPerson]
	@personId INT,
	@SuitableTimeForCalls DATETIME
AS
    IF @personId is null and @SuitableTimeForCalls is null
	begin
		select 0 CallID,
		       0 SurveySID,
			   0 iid
		where 1 = 0
		return 0
	end

    DECLARE @interviewId INT
    DECLARE @rowCount INT
    DECLARE @surveyId INT

	create table #output(CallID int,
						 SurveySID int,
						 iid int)
    
    ;WITH calls AS
	(
		SELECT TOP(1) c.*
		FROM BvSvySchedule c
		INNER JOIN BvPersonRel p ON p.PersonSID = @personId
		INNER JOIN BvSurvey on SID = SurveySid AND DialMode !=  4 AND State =1
		WHERE CallState = 2 AND
		      p.ObjectSID = c.ExplicitSID AND
			  TimeInShift <= @SuitableTimeForCalls AND
			  IsInActiveShiftType = 1
		ORDER BY Priority DESC,
                 TimeInShift,
				 ExplicitType DESC,
				 CallOrder
	)
	UPDATE calls
	SET CallState = -1,
	    ExpireTime = '9999-01-01 00:00:00.000',
		@interviewId = InterviewID,
		@surveyId = SurveySid
	OUTPUT
	   deleted.[ID] CallID,
	   deleted.SurveySID,
	   deleted.InterviewID iid
	INTO #output
	
	SET @rowCount = @@ROWCOUNT

	SELECT * FROM #output
	
	IF(@rowCount = 0) RETURN 0
	
	UPDATE BvAppointment 
	SET State = 2 
	WHERE State = 1 AND 
	      SurveysId = @surveyId AND 
	      InterviewSid = @interviewId
RETURN 0
GO
PRINT N'Altering [dbo].[BvSpLookUpByPerson_ForAssignmentMode]...';


GO
ALTER PROCEDURE [dbo].[BvSpLookUpByPerson_ForAssignmentMode]
	@surveyId INT,
	@personId INT,
	@SuitableTimeForCalls DATETIME
AS
    IF @personId is null and @SuitableTimeForCalls is null
	begin
		select 0 CallID,
		       0 SurveySID,
			   0 iid
		where 1 = 0
		return 0
	end

    DECLARE @interviewId INT
    DECLARE @rowCount INT

	create table #output(CallID int,
						 SurveySID int,
						 iid int)
    
    ;WITH calls AS
	(
		SELECT TOP(1) c.*
		FROM BvSvySchedule c
		INNER JOIN BvPersonRel p ON p.PersonSID = @personId
		WHERE CallState = 2 AND
			  TimeInShift <= @SuitableTimeForCalls AND
			  IsInActiveShiftType = 1 AND
		      SurveySid = @surveyId AND
			  p.ObjectSID = c.ExplicitSID
		ORDER BY Priority DESC,
                 TimeInShift,
				 ExplicitType DESC,
				 CallOrder
	)
	UPDATE calls
	SET CallState = -1,
	    ExpireTime = '9999-01-01 00:00:00.000',
		@interviewId = InterviewID,
		@surveyId = SurveySid
	OUTPUT
	   deleted.[ID] CallID,
	   deleted.SurveySID,
	   deleted.InterviewID
	INTO #output

	SET @rowCount = @@ROWCOUNT

	select * from #output
	
	IF(@rowCount = 0) RETURN 0
	
	UPDATE BvAppointment 
	SET State = 2 
	WHERE State = 1 AND 
	      SurveysId = @surveyId AND 
	      InterviewSid = @interviewId
RETURN 0
GO
PRINT N'Altering [dbo].[BvSpQueueUpSheduleTask3]...';


GO
ALTER PROCEDURE [dbo].[BvSpQueueUpSheduleTask3]
    @NowUTC           datetime,
    @DefaultTZ        INT
as
set nocount on

declare @rows int
 
    -- temp table for determine active shifts/survey
    create table #temp
    (
        [ID] int not null,
        SurveySID int not null
    )
 
    -- calculate live shifts 
    insert into #temp exec BvSpGetLiveShifts @NowUTC, @DefaultTZ

 
    -- insert into BvCachedCallsInsert calls id
    -- where shift type id is inserted, or deleted
    insert into BvCachedCallsInsert
    select c.InterviewID, c.SurveySID
    from BvSvySchedule c
    inner join ( select isnull( t.[ID], a.[ID] ) as [ID], 
                        isnull( t.SurveySID, a.SurveyId ) as SurveySID
                    from #temp t
                    full join BvActiveShiftTypeZone a on a.Id = t.[ID] and
                        a.SurveyId = t.SurveySID
                    where a.[ID] is null or t.[ID] is null ) s on
        s.[ID] = c.ShiftTypeID
        and s.SurveySID = c.SurveySID
 
        -- copy new shifts information
     truncate table BvActiveShiftTypeZone
     insert into BvActiveShiftTypeZone
     select [ID], SurveySID from #temp
 
     drop table #temp

	create table #tempCachedCallsInsert
	(
	   surveyId int,
	   interviewId int,
	   primary key(surveyId, interviewId)
	)

    delete from BvCachedCallsInsert
	output deleted.SurveySID,
	       deleted.InterviewID
    into #tempCachedCallsInsert

	if @@ROWCOUNT > 200000 
	begin 
        UPDATE BvSvySchedule
        SET IsInActiveShiftType = ISNULL(a.ID|1, 0)
        FROM BvSvySchedule c
        LEFT JOIN BvActiveShiftTypeZone a ON a.Id = c.ShiftTypeID AND
                                            a.SurveyId = c.SurveySID
        WHERE CallState != -3 AND--processed during sample loading
			    IsInActiveShiftType != CAST(ISNULL(a.ID|1, 0) AS BIT)
    end
    else 
	begin
		UPDATE BvSvySchedule
		SET IsInActiveShiftType = ISNULL(a.ID|1, 0)
		FROM BvSvySchedule c
		INNER JOIN #tempCachedCallsInsert i ON i.InterviewID = c.InterviewID AND  i.SurveyID = c.SurveySID
		LEFT JOIN BvActiveShiftTypeZone a ON a.Id = c.ShiftTypeID AND 
											a.SurveyId = c.SurveySID
		WHERE IsInActiveShiftType != CAST(ISNULL(a.ID|1, 0) AS BIT)
    END
 
return (0)
GO
PRINT N'Altering [dbo].[BvSpReleaseCall]...';


GO
ALTER PROCEDURE [dbo].[BvSpReleaseCall]
	@SurveySID		INT,
	@InterviewID	INT
AS

UPDATE BvSvySchedule
SET CallState = 2
WHERE InterviewID = @InterviewID AND 
	  SurveySID = @SurveySID AND
	  CallState <> 0
GO
PRINT N'Altering [dbo].[BvSpRemoveExpiredCalls]...';


GO
ALTER PROCEDURE [dbo].[BvSpRemoveExpiredCalls]
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
        
        --DELETE FROM BvSvySchedule 
        UPDATE BvSvySchedule
        SET CallState = -1
        WHERE ExpireTime < @NowUTC AND 
              CallState > 0 --TODO:
                        --WE should correct process call with CallState = -2
    END
RETURN 0
GO
PRINT N'Altering [dbo].[BvSpSchedule_Delete]...';


GO
ALTER PROCEDURE [dbo].[BvSpSchedule_Delete]
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

    --should we update calls with none shift type?
    UPDATE BvSvySchedule SET ShiftTypeID = -z.TimeZoneID
	OUTPUT inserted.InterviewID,
	       inserted.SurveySID
    into BvCachedCallsInsert
    FROM BvSvySchedule c
    INNER JOIN BvShiftZones z ON c.ShiftTypeID = z.[ID] 
    INNER JOIN BvShiftType t ON t.OwnerSID = @ScheduleID AND z.ShiftTypeID = t.ObjectID


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
PRINT N'Altering [dbo].[BvSpSetCallState]...';


GO
ALTER PROCEDURE [dbo].[BvSpSetCallState]
	@SurveySID		INT,
	@InterviewID	INT,
	@state			INT
AS

UPDATE BvSvySchedule
SET CallState = @state
WHERE InterviewID = @InterviewID AND 
	  SurveySID = @SurveySID
GO
PRINT N'Altering [dbo].[BvSpShiftType_Delete]...';


GO
ALTER PROCEDURE [dbo].[BvSpShiftType_Delete]
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

        DELETE FROM BvSvySchedule 
        OUTPUT DELETED.ApptID
        INTO @changingTable
        WHERE ShiftTypeID IN ( SELECT [ID] FROM BvShiftZones WHERE ShiftTypeID = @ObjectID ) AND
              (CallState > 0 OR CallState = -2)
        
        UPDATE BvAppointment
        SET State = 2
        FROM @changingTable c
        WHERE c.ApptID = BvAppointment.ID
    END
END

DELETE  BvShiftType
    WHERE   OwnerSID = @OwnerSID
    AND ID = @ID

DELETE FROM BvShiftZones WHERE ShiftTypeID = @ObjectID

RETURN 0
GO
PRINT N'Altering [dbo].[BvSpState_Update]...';


GO
ALTER PROCEDURE [dbo].[BvSpState_Update]
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

END

RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpSurvey_DeleteFiltered]...';


GO
ALTER PROCEDURE [dbo].[BvSpSurvey_DeleteFiltered]
@SurveySID INT,
@BatchID INT
AS
    
    DECLARE @deletedrecords table(ApptID INT)

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
PRINT N'Altering [dbo].[BvSpSurvey_ListPage]...';


GO
ALTER PROCEDURE [dbo].[BvSpSurvey_ListPage]
@CallCenterId INT,
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
        FROM    BvFnSurvey_GetByCallCenterId(' + cast(@CallCenterId AS NVARCHAR) + ') BvSurvey
        LEFT JOIN BvUserSurveyPermission ON ( BvUserSurveyPermission.UserName = '''+@userName+''' AND
                                              BvUserSurveyPermission.SurveySID = BvSurvey.SID)
        LEFT JOIN (SELECT COUNT(*) as Count, SurveySID FROM BvInterview group by SurveySid ) as sample on BvSurvey.SID = sample.SurveySID 
        WHERE
                  ((BvUserSurveyPermission.UserName IS NOT NULL) OR ('''+@userName+''' = '''')) AND BvSurvey.State <> 2'

DECLARE @TotalCount INT
exec @TotalCount = BvSpGetListPage @PageNumber, @PageSize, @OrderField, @IsOrderASC, @Query, @IDField, @SearchCondition
RETURN @TotalCount
GO
PRINT N'Altering [dbo].[BvSpSurveyModifyStateGroup]...';


GO
ALTER PROCEDURE [dbo].[BvSpSurveyModifyStateGroup]
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
     END

RETURN ( 0 )
GO
PRINT N'Altering [dbo].[BvSpTimezone_Delete]...';


GO
ALTER PROCEDURE [dbo].[BvSpTimezone_Delete]
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

IF EXISTS(SELECT * FROM BvCallCenter WHERE LocalTimezoneId = @ID)
BEGIN
	RAISERROR('Unable to delete timezone %i. The timezone is used in some call center', 12, 1, @ID)
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
PRINT N'Altering [dbo].[BvSpTimezone_DeleteUnused]...';


GO
ALTER PROCEDURE [dbo].[BvSpTimezone_DeleteUnused]
AS
	DELETE FROM [BvTimezone]
	WHERE [id] NOT IN (SELECT LocalTimezoneId FROM BvCallCenter)
	AND [id] NOT IN 
		( SELECT [TimezoneID] FROM [BvInterview] WHERE [TimezoneID] IS NOT NULL GROUP BY [TimezoneID] )
	AND [id] NOT IN ( SELECT z.[TimeZoneID] FROM [BvSvySchedule] sh
						JOIN [BvShiftZones] z
						ON sh.[ShiftTypeID] = z.[id] 
						GROUP BY z.[TimeZoneID] )
	AND [id] NOT IN ( SELECT [TimezoneID] FROM [BvTimezoneShift]
					 GROUP BY [TimezoneID] )
GO
PRINT N'Altering [dbo].[BvSpUpdateInProgressCallsToScheduled]...';


GO
ALTER PROCEDURE [dbo].[BvSpUpdateInProgressCallsToScheduled]
	@surveySID	INT,
	@its		INT
AS
	SET NOCOUNT ON

	-- 1st we release call in progress (CallState -1) and fill table variable
	-- with interview's ID's

	UPDATE BvSvySchedule
    SET CallState = 2
	WHERE SurveySID = @surveySID AND 
	      CallState = -1
GO
PRINT N'Creating [dbo].[BvSpAggregateSurveyProcessDelta]...';


GO
CREATE PROCEDURE [dbo].[BvSpAggregateSurveyProcessDelta]
AS
    DECLARE @BvAggregateSurveyDelta TABLE
	(
		[ID]                          INT,
		[SID]                         INT NOT NULL,
		[ScheduledCallsCount]         INT NOT NULL,
		[SuspendedCallsCount]         INT NOT NULL,
		[MinutesSpentWorkingOnSurvey] INT NOT NULL
	);

	DELETE FROM BvAggregateSurveyDelta WITH (READPAST)
	OUTPUT DELETED.* INTO @BvAggregateSurveyDelta

	UPDATE 
	    BvAggregateSurvey 
	SET 
	    BvAggregateSurvey.MinutesSpentWorkingOnSurvey += AggregatedDelta.MinutesSpentWorkingOnSurvey,
	    BvAggregateSurvey.ScheduledCallsCount += AggregatedDelta.ScheduledCallsCount,
		BvAggregateSurvey.SuspendedCallsCount += AggregatedDelta.SuspendedCallsCount
	FROM
	    BvAggregateSurvey
	INNER JOIN
	(
	    SELECT [SID],
		       SUM([ScheduledCallsCount]) ScheduledCallsCount,
		       SUM([SuspendedCallsCount]) SuspendedCallsCount,
			   SUM([MinutesSpentWorkingOnSurvey]) MinutesSpentWorkingOnSurvey
		FROM @BvAggregateSurveyDelta
		GROUP BY [SID]
	) AggregatedDelta
	ON BvAggregateSurvey.SID = AggregatedDelta.SID

RETURN 0
GO
PRINT N'Creating [dbo].[BvSpAsyncOperationQueue_AppendText]...';


GO
CREATE PROCEDURE [dbo].[BvSpAsyncOperationQueue_AppendText]
    @Id INT,
	@Text NVARCHAR(MAX)
AS
    UPDATE BvAsyncOperationQueue SET [Text] = [Text] + @Text, HeartBeat = GETUTCDATE() WHERE Id = @Id
RETURN
GO
PRINT N'Creating [dbo].[BvSpAsyncOperationQueue_Dequeue]...';


GO
CREATE PROCEDURE [dbo].[BvSpAsyncOperationQueue_Dequeue]
    @OperationsLimit INT,
	@QueueuedStateValue TINYINT, /*AsyncOperationState.Queued passed from C# to avoid copy paste*/
	@ExecutingStateValue TINYINT  /*AsyncOperationState.Executing passed from C# to avoid copy paste*/
AS
	DECLARE @executingAtTheMomentOperations INT;

	SELECT @executingAtTheMomentOperations = COUNT(*) FROM BvAsyncOperationQueue WHERE [State]=@ExecutingStateValue

	IF @executingAtTheMomentOperations < @OperationsLimit
	BEGIN
	    SELECT
		    Id
		FROM
		    BvAsyncOperationQueue
		WHERE
		    [State] = @QueueuedStateValue AND [SurveySid] NOT IN (SELECT SurveySid FROM BvAsyncOperationQueue WHERE [State]=@ExecutingStateValue)
		ORDER BY [Priority], [ID]
	END

RETURN
GO
PRINT N'Creating [dbo].[BvSpAsyncOperationQueue_UpdateProgress]...';


GO
CREATE PROCEDURE [dbo].[BvSpAsyncOperationQueue_UpdateProgress]
    @Id INT,
    @TotalItemsCount INT,
    @ProcessedItemsCount INT,
    @FailedItemsCount INT
AS
    UPDATE BvAsyncOperationQueue SET TotalItemsCount = @TotalItemsCount, ProcessedItemsCount = @ProcessedItemsCount, FailedItemsCount = @FailedItemsCount, HeartBeat = GETUTCDATE() WHERE Id = @Id
RETURN 0
GO
PRINT N'Creating [dbo].[BvSpAsyncOperations_ListPage]...';


GO
CREATE PROCEDURE [dbo].[BvSpAsyncOperations_ListPage]
@CallCenterId INT = NULL,
@PageNumber INT, 
@PageSize INT, 
@OrderField NVARCHAR (64), 
@IsOrderASC INT, 
@userName NVARCHAR (255), 
@SearchCondition NVARCHAR (4000) = NULL
AS
SET NOCOUNT ON

 IF @PageNumber IS NULL AND @PageSize IS NULL
 BEGIN
 /* Looks like we're generating code using FMTONLY. So lets return metadata*/

 SELECT  
		0                  AS   Id,
		''                 AS   InitiatorName,
        ''                 AS   ProjectId,
		0                  AS   CallCenterId,
		CAST(0 AS TINYINT) AS   OperationType,
		CAST(0 AS TINYINT) AS   OperationState,
		GETUTCDATE()       AS   InitiatedTime,
		GETUTCDATE()       AS   StartedTime,
		GETUTCDATE()       AS   FinishedTime,
		''                 AS   OperationTitle
     
     RETURN 0;
 END
 
DECLARE @Query as nvarchar(4000)
DECLARE @IDField as nvarchar(64) 
SET @IDField = 'Id'

SET @Query =
    'SELECT
		ao.Id                       AS   Id,
		ao.CreatedBySupervisorName  AS   InitiatorName,
		BvSurvey.Name               AS   ProjectId,
		1                           AS   CallCenterId,
		ao.Type                     AS   OperationType,
		ao.State                    AS   OperationState,
		ao.QueuedDate               AS   InitiatedTime,
		ao.StartedDate              AS   StartedTime,
		ao.FinishedDate             AS   FinishedTime,
		ao.Title		            AS   OperationTitle
        FROM    
			BvAsyncOperationQueue ao
			INNER JOIN BvFnSurvey_GetByCallCenterId(' + (case when @CallCenterId IS NULL Then 'NULL' else cast(@CallCenterId AS NVARCHAR) end) + ') BvSurvey ON ao.SurveySid = BvSurvey.SID			
			LEFT JOIN BvUserSurveyPermission ON ( BvUserSurveyPermission.UserName = '''+  @userName + ''' AND BvUserSurveyPermission.SurveySID = ao.SurveySid)
        WHERE
            ((BvUserSurveyPermission.UserName IS NOT NULL) OR ( ''' + @userName + ''' = '''' ))'
				  
DECLARE @TotalCount INT
exec @TotalCount = BvSpGetListPage @PageNumber, @PageSize, @OrderField, @IsOrderASC, @Query, @IDField, @SearchCondition
RETURN @TotalCount
GO
PRINT N'Creating [dbo].[BvSpCallCenter_Delete]...';


GO
CREATE PROCEDURE [dbo].[BvSpCallCenter_Delete]
	@CallCenterID INT,
	@DescCallCenterID INT,
	@PersonAction INT
AS
	DECLARE @IsCanBeDeleted BIT = ( SELECT CanBeDeleted FROM BvCallCenter WHERE ID = @CallCenterID )

	IF ISNULL( @IsCanBeDeleted, 1 ) = 1 
	BEGIN
		RAISERROR( 'Call center with ID = %d can''t be deleted, because call center doesn''t exists or is marked as can''t be deleted', 12, 1, @CallCenterID )
		RETURN (0)
	END

	DECLARE @Surveys TABLE( SurveyId INT )	

	DELETE BvSurveyAssignmentOnCallCenter 
		OUTPUT deleted.SurveyId INTO @Surveys
		WHERE CallCenterId = @CallCenterID

	INSERT INTO BvSurveyAssignmentOnCallCenter( SurveyId, CallCenterId )
		SELECT s.SurveyId, @DescCallCenterID FROM @Surveys s
		LEFT JOIN BvSurveyAssignmentOnCallCenter a
			ON s.SurveyId = a.SurveyId AND a.CallCenterId = @DescCallCenterID
		WHERE a.CallCenterId IS NULL
	
	UPDATE BvSupervisorAssignment SET CallCenterId = @DescCallCenterID WHERE CallCenterID = @CallCenterID

	DECLARE @PersonId INT

	IF @PersonAction = 0 --delete
	BEGIN
		
		DECLARE crPerson CURSOR FOR 
			SELECT SID FROM BvPerson WHERE CallCenterID = @CallCenterID
		
		OPEN crPerson
		FETCH NEXT FROM crPerson INTO @PersonId
		
		WHILE ( @@FETCH_STATUS = 0 ) 
		BEGIN
			EXEC BvSpPerson_Delete @PersonId
			FETCH NEXT FROM crPerson INTO @PersonId
		END

		CLOSE crPerson
		DEALLOCATE crPerson
	END
	ELSE IF @PersonAction = 1
	BEGIN
		DECLARE @Persons TABLE( SID INT )

		UPDATE BvPerson 
			SET CallCenterID = @DescCallCenterID
			OUTPUT deleted.SID INTO @Persons
			WHERE CallCenterID = @CallCenterID

		DELETE FROM BvPersonOrGroupAssignmentOnSurvey
			WHERE CallCenterID = @CallCenterID
		
		DECLARE crPerson CURSOR FOR 
			SELECT SID FROM @Persons
		
		OPEN crPerson
		FETCH NEXT FROM crPerson INTO @PersonId
		
		WHILE ( @@FETCH_STATUS = 0 ) 
		BEGIN
			EXEC BvSpPerson_SpinUp @PersonId
			FETCH NEXT FROM crPerson INTO @PersonId
		END

		CLOSE crPerson
		DEALLOCATE crPerson
	END
	ELSE
	BEGIN
		RAISERROR( 'Call center with ID = %d can''t be deleted, because wrong PersonAction = %d.', 12, 1, @PersonAction )
		RETURN 0
	END

	DELETE BvPersonOrGroupAssignmentOnSurvey WHERE CallCenterID = @CallCenterID
	DELETE BvCallCenter WHERE ID = @CallCenterID

	RETURN(0)
GO
PRINT N'Creating [dbo].[BvSpCallCenter_Insert]...';


GO
CREATE PROCEDURE [dbo].[BvSpCallCenter_Insert]
	@Name NVARCHAR(MAX),
	@Description NVARCHAR(MAX),
	@LocalTimezoneId INT
AS

	DECLARE @Count INT = (SELECT COUNT(*) FROM BvCallCenter )
	IF @Count >= 255
	BEGIN
		RAISERROR( 'Count of call centers can''t be greater 255', 12, 1 )
		RETURN 0
	END

	INSERT INTO BvCallCenter( Name, Description, LocalTimezoneId ) VALUES( @Name, @Description, @LocalTimezoneId )

	RETURN SCOPE_IDENTITY()
GO
PRINT N'Creating [dbo].[BvSpCallCenter_ListOfAssignedToSurvey]...';


GO
CREATE PROCEDURE [dbo].[BvSpCallCenter_ListOfAssignedToSurvey]
	@SurveyId INT
AS
	SELECT cs.* FROM BvSurveyAssignmentOnCallCenter a 
		INNER JOIN BvCallCenter cs
		ON a.CallCenterId = cs.ID
		WHERE a.SurveyId = @SurveyId

	RETURN(0)
GO
PRINT N'Creating [dbo].[BvSpCallCenter_Update]...';


GO
CREATE PROCEDURE [dbo].[BvSpCallCenter_Update]
	@CallCenterID INT,
	@Name NVARCHAR(MAX),
	@Description NVARCHAR(MAX),
	@LocalTimezoneId INT
AS
	UPDATE BvCallCenter 
		SET Name = @Name,
			Description = @Description,
			LocalTimezoneId = @LocalTimezoneId
		WHERE ID = @CallCenterID
	
	RETURN(0)
GO
PRINT N'Creating [dbo].[BvSpGetSurveyCallCenterAssignmentPage]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetSurveyCallCenterAssignmentPage]
	@CallCenterId INT,
    @PageIndex INT,
    @PageSize INT, 
    @OrderField NVARCHAR (64), 
    @IsOrderASC BIT, 
    @SearchCondition NVARCHAR (4000)=NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @PageIndex IS NULL AND @PageSize IS NULL
        BEGIN
        /* Looks like we're generating code using FMTONLY. So lets return metadata*/
         SELECT  
             0 AS SurveyId,
			 '' AS ProjectId,
             '' AS SurveyName, 
             '' AS CallCenterNames
     
        RETURN 0;
    END

	DECLARE @Query as nvarchar(4000)
	DECLARE @IDField as nvarchar(64)
	SET @IDField = 'SurveyId'
	SET @Query =
	'SELECT 
        s.SID as SurveyId,
        s.Name as ProjectId,
        s.Description as SurveyName,
        Stuff( (SELECT '', '' +  Name FROM BvCallCenter cs INNER JOIN BvSurveyAssignmentOnCallCenter a ON cs.ID = a.CallCenterID
        WHERE a.SurveyId = s.SID
        FOR XML PATH('''') ), 1, 2, '''' ) as CallCenterNames
    FROM BvSurvey s 
    WHERE s.State <> 2'

	IF @CallCenterId IS NOT NULL 
	BEGIN
        IF @SearchCondition IS NOT NULL AND LEN(@SearchCondition) > 0
		    SET @SearchCondition = @SearchCondition + ' AND '
	    
		SET @SearchCondition = @SearchCondition + ' SurveyId IN (SELECT SurveyID FROM BvSurveyAssignmentOnCallCenter WHERE CallCenterID = ' + CAST(@CallCenterId AS nvarchar) + ')'
	END

    DECLARE @TotalCount INT
    exec @TotalCount = BvSpGetListPage @PageIndex, @PageSize, @OrderField, @IsOrderASC, @Query, @IDField, @SearchCondition
    RETURN @TotalCount
END
GO
PRINT N'Creating [dbo].[BvSpSurvey_AssignToCallCenter]...';


GO
CREATE  PROCEDURE [dbo].[BvSpSurvey_AssignToCallCenter]
        @SurveyId INT,
        @CallCenterId INT
AS
SET NOCOUNT ON

	INSERT INTO BvSurveyAssignmentOnCallCenter(SurveyId, CallCenterId) 
		SELECT @SurveyId, @CallCenterId
			WHERE NOT EXISTS( SELECT 1 FROM BvSurveyAssignmentOnCallCenter WHERE SurveyId = @SurveyId AND CallCenterId = @CallCenterId )

	IF @@ROWCOUNT = 0 
	BEGIN
		RETURN (0)
	END
		
RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpSurvey_DeassignFromCallCenter]...';


GO
CREATE  PROCEDURE [dbo].[BvSpSurvey_DeassignFromCallCenter]
        @SurveyId INT,
        @CallCenterId INT
AS
SET NOCOUNT ON

	DELETE FROM BvSurveyAssignmentOnCallCenter 
		WHERE SurveyId = @SurveyId AND CallCenterId = @CallCenterId

	IF @@ROWCOUNT = 0 
	BEGIN
		RETURN (0)
	END

	DELETE FROM BvPersonOrGroupAssignmentOnSurvey
		WHERE SurveyId = @SurveyId AND CallCenterID = @CallCenterId

	DECLARE @deassignedPersons TABLE(personId INT PRIMARY KEY)

	DELETE BvPersonRel 
		OUTPUT deleted.PersonSID INTO @deassignedPersons
		WHERE ObjectSID = @SurveyId AND Type = 2 AND PersonSID IN ( SELECT SID FROM BvPerson WHERE CallCenterID = @CallCenterId )

	DELETE BvLoginGroup 
		FROM @deassignedPersons as dp
		WHERE BvLoginGroup.SurveySID = @SurveyId AND BvLoginGroup.PersonSID = dp.personId

RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpAssignment_Delete]...';


GO
ALTER PROCEDURE [dbo].[BvSpAssignment_Delete]
@SurveySID INT, 
@Count INT, 
@PersonSID INT, 
@RoleID INT,
@CallCenterID INT
AS
SET NOCOUNT ON

DECLARE @InsertedAssignmentsCount INTEGER = 0

 IF @Count > 0 
 BEGIN

    UPDATE BvSvySchedule SET ExplicitSID = @SurveySID, ExplicitType = 1
    WHERE ExplicitSID = @PersonSID AND
          SurveySID = @SurveySID AND
          CallState = 2 AND
          @RoleID = 2
    
    RETURN @InsertedAssignmentsCount
 END
 ELSE
 BEGIN
    DELETE FROM BvPersonOrGroupAssignmentOnSurvey
      WHERE PersonOrGroupId = @PersonSID AND SurveyId = @SurveySID AND CallCenterID = @CallCenterID
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
                     FROM BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID)
                     WHERE PersonOrGroupId = base.PersonSid AND
                           SurveyId = @SurveySID) AND
         NOT EXISTS (SELECT *                  --if person doesn't belong to others groups assigned to survey
                     FROM BvMemberShip
                     INNER JOIN BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID) ON PersonOrGroupId = ContainerSid AND
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
PRINT N'Altering [dbo].[BvSpAssignment_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpAssignment_Insert]
@SID INT, 
@SurveySID INT, 
@InterviewSID INT, 
@PersonSID INT, 
@RoleID INT, 
@FromCall INT=0,
@CallCenterID INT
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

            exec BvSpAddUniqueAssignment @PersonSID
END
ELSE
BEGIN
        
    IF NOT EXISTS ( SELECT * FROM BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID)
        WHERE PersonOrGroupId = @PersonSID AND SurveyId = @SurveySID)
          INSERT INTO BvPersonOrGroupAssignmentOnSurvey( PersonOrGroupId, SurveyId, CallCenterID )
              VALUES( @PersonSID, @SurveySID, @CallCenterID )
              
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
       SELECT r.PersonSid, @SurveySID, 2, 2
       FROM BVPersonRel r
	   LEFT JOIN BvPerson p 
		ON r.PersonSID = p.SID
       WHERE @PersonSID = r.ObjectSID AND
             ObjectSID != r.PersonSid AND
			 ( p.CallCenterID = @CallCenterID OR p.SID IS NULL )
       
       insert into BvLoginGroup 
       select personGroup.PersonSID, @SurveySID, lg.SurveySID
       from BvPersonRel personGroup
       inner join BvLoginGroup lg on lg.PersonSid = personGroup.PersonSID AND  --get surveySid from BvLoginGroup which should be set already
                                     lg.PersonSid = lg.ObjectSid
       inner join BvFnPerson_Get(@CallCenterID) ON sid = personGroup.PersonSID           --get only persons assigned to current group
       where personGroup.ObjectSID = @PersonSID AND
             personGroup.ObjectSID != personGroup.PersonSID			--we not need in fake records
   END
END

RETURN @InsertedAssignmentsCount

GO

DECLARE @DefaultTzId INT = CAST( ISNULL( (SELECT Value FROM [BvSystemSettings] WHERE SystemName='Site.TimeZoneID' ), '1') as NVARCHAR(64));

INSERT INTO BvCallCenter(Name, Description, IsDefault, CanBeDeleted, LocalTimezoneId ) VALUES( 'Default', 'Default call center', 1, 0, @DefaultTzId )

DECLARE @CallCenterID INT = @@IDENTITY;

UPDATE BvPerson SET CallCenterID = @CallCenterID;

UPDATE BvPersonOrGroupAssignmentOnSurvey SET CallCenterID = @CallCenterID;

UPDATE [dbo].[BvHistory] SET [CallCenterID] = @CallCenterID WHERE PersonSID <> 0 OR RoleID <> 0

UPDATE BvPersonDeferredMonitoring SET [CallCenterID] = @CallCenterID

UPDATE BvTasks SET [CallCenterID] = @CallCenterID

UPDATE BvTimeBreaksHistory SET [CallCenterID] = @CallCenterID

INSERT INTO BvSurveyAssignmentOnCallCenter( [SurveyId], [CallCenterId] ) SELECT [SID], @CallCenterID FROM BvSurvey

GO
DELETE FROM [BvThresholdTypes] WHERE id=5

GO
DELETE FROM [BvSystemSettings] WHERE SystemName='Site.TimeZoneID'

GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
	;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
	(
	SELECT 'Setup.SupervisorVirtualDirectoryName', 'SupervisorVirtualDirectoryName', 'Setup', 'Supervisor virtual directory name (don''t change)', 2, 0, NULL
	UNION ALL
	SELECT 'Setup.SupervisorAppPoolName', 'SupervisorAppPoolName', 'Setup', 'Supervisor app pool name (don''t change)', 2, 0, NULL
	UNION ALL
	SELECT 'Setup.SupervisorSiteName', 'SupervisorSiteName', 'Setup', 'Supervisor site name  (don''t change)', 2, 0, NULL
	UNION ALL
	SELECT 'Setup.IsDatabaseLoggingEnabled', 'IsDatabaseLoggingEnabled', 'Setup', 'Is database logging enabled or not. Possible values: 1 or empty. Will be applied after next installation', 2, 0, NULL
	UNION ALL
	SELECT 'Setup.IsEventlogLoggingEnabled', 'IsEventlogLoggingEnabled', 'Setup', 'Is eventlog logging enabled or not. Possible values: 1 or empty. Will be applied after next installation', 2, 0, NULL
	UNION ALL
	SELECT 'Setup.MinFreeSpaceOnDiskInMb', 'MinFreeSpaceOnDiskInMb', 'Setup', 'Min free space on disk in MB during a db update process for the database update utility (possible values: positive number). It shouldn''t be too small. Default: 1024', 2, 0, NULL
	UNION ALL
	SELECT 'Setup.DatabasesSnapshotFilePath', 'DatabasesSnapshotFilePath', 'Setup', 'Databases snapshot file path. Possible values: existed path on SQL server or empty. for the database update utility', 2, 0, NULL
	UNION ALL
	SELECT 'Setup.ConfirmitAuthoringServer', 'ConfirmitAuthoringServer', 'Setup', 'Confirmit authoring server (can be changed). Will be applied after next installation', 2, 0, NULL
	UNION ALL
	SELECT 'Setup.ConfirmitDeploymentServer', 'ConfirmitDeploymentServer', 'Setup', 'Confirmit deployment server (can be changed). Will be applied after next installation', 2, 0, NULL
	UNION ALL
	SELECT 'Setup.ConfirmitWebServiceServer', 'ConfirmitWebServiceServer', 'Setup', 'Confirmit web service server (can be changed). Will be applied after next installation', 2, 0, NULL
	UNION ALL
	SELECT 'Setup.SessionStateMode', 'SessionStateMode', 'Setup', 'Session state mode. Possible values: SQLMode or InProc. Will be applied after next installation', 2, 0, NULL
	UNION ALL
	SELECT 'Setup.EncryptedSessionStateConnectionString', 'EncryptedSessionStateConnectionString', 'Setup', 'Encrypted session state connection string (use a special tool to change this setting)', 2, 0, NULL
	UNION ALL
	SELECT 'Setup.SessionStateCookieName', 'SessionStateCookieName', 'Setup', 'Session state cookie name (can be changed). Will be applied after next installation', 2, 0, NULL
	UNION ALL
	SELECT 'Setup.EncryptedConfirmConnectionString', 'EncryptedConfirmConnectionString', 'Setup', 'Encrypted connection string to confirm database (use a special tool to change this setting)', 2, 0, NULL
	UNION ALL
	SELECT 'Setup.EncryptedConfirmlogConnectionString', 'EncryptedConfirmlogConnectionString', 'Setup', 'Encrypted connection string to confirmlog database (use a special tool to change this setting)', 2, 0, NULL
	UNION ALL
	SELECT 'Setup.IsSslAcceleratorUse', 'IsSslAcceleratorUse', 'Setup', 'Is ssl accelerator use. Possible values: true or false. Will be applied after next installation', 2, 0, NULL
	UNION ALL
	SELECT 'Setup.CertificateType', 'CertificateType', 'Setup', 'Certificate type. Possible values: Test or Real. Will be applied after next installation', 2, 0, NULL
	UNION ALL
	SELECT 'Setup.CertificateName', 'CertificateName', 'Setup', 'Certificate name (can be changed). Will be applied after next installation', 2, 0, NULL
	UNION ALL
	SELECT 'Setup.ConfirmitLinkedServerName', 'ConfirmitLinkedServerName', 'Setup', 'Confirmit linked server name (can be changed). This value can be used in update scripts during DB update process', 2, 0, NULL
	UNION ALL
	SELECT 'Setup.IsAliveHtmlLocation', 'IsAliveHtmlLocation', 'Setup', 'A location of IsAlive.html file. Reqired if ''IsSslAcceleratorUse'' parameter is true. Will be applied after next installation', 2, 0, NULL
	UNION ALL
	SELECT 'Setup.InstallLocation', 'InstallLocation', 'Setup', 'A root folder of CATI installation. Will be applied after next installation', 2, 0, NULL
	UNION ALL
	SELECT 'Setup.RealCertificateThumbprint', 'RealCertificateThumbprint', 'Setup', 'Real certificate thumbprint (can be changed). Will be applied after next installation', 2, 0, NULL
	)
	INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
		SELECT d.* FROM Data d LEFT JOIN BvSystemSettings ss ON d.[SystemName] = ss.[SystemName] WHERE ss.[SystemName] IS NULL
END

GO
PRINT N'Creating [dbo].[BvVersionHistory].[Description].[MS_Description]...';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'The description from ScriptDefinitionFile', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'BvVersionHistory', @level2type = N'COLUMN', @level2name = N'Description';


GO
PRINT N'Creating [dbo].[BvVersionHistory].[Duration].[MS_Description]...';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Time in milliseconds took to apply the script', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'BvVersionHistory', @level2type = N'COLUMN', @level2name = N'Duration';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpAggregateInterviewerPerformance]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAggregateInterviewerPerformance]';


GO
PRINT N'Refreshing [dbo].[BvSpAttemptsByDispositionReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAttemptsByDispositionReport]';


GO
PRINT N'Refreshing [dbo].[BvSpInterviewerProductivityReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterviewerProductivityReport]';


GO
PRINT N'Refreshing [dbo].[BvSpNumberOfAttemptsReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpNumberOfAttemptsReport]';


GO
PRINT N'Refreshing [dbo].[BvSpSample_Abandon]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSample_Abandon]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyOverviewReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyOverviewReport]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyProductivityReportCapi]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyProductivityReportCapi]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyProductivityReportCati]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyProductivityReportCati]';


GO
PRINT N'Refreshing [dbo].[BvSpAlertsHistoryAggregatedReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAlertsHistoryAggregatedReport]';


GO
PRINT N'Refreshing [dbo].[BvSpGetMessages]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetMessages]';


GO
PRINT N'Refreshing [dbo].[BvSpGetSurveyInterviews]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetSurveyInterviews]';


GO
PRINT N'Refreshing [dbo].[BvSpGetUserGroups]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetUserGroups]';


GO
PRINT N'Refreshing [dbo].[BvSpInterview_CfData_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterview_CfData_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpLogin_SpinUp]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLogin_SpinUp]';


GO
PRINT N'Refreshing [dbo].[BvSpPersonCheckForNewMessage]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonCheckForNewMessage]';


GO
PRINT N'Refreshing [dbo].[BvSpAlert_RecalculateAppointment]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAlert_RecalculateAppointment]';


GO
PRINT N'Refreshing [dbo].[BvSpPersonGroup_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonGroup_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpCleanDeferredMonitoring]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCleanDeferredMonitoring]';


GO
PRINT N'Refreshing [dbo].[BvSpGetDeferredMonitoringStartFile]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetDeferredMonitoringStartFile]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Clean]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Clean]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_IsPersonAssigned]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_IsPersonAssigned]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyCleanup_IsClean]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyCleanup_IsClean]';


GO
PRINT N'Refreshing [dbo].[BvSpGetLoggedInPersonsCount]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetLoggedInPersonsCount]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_InsertAnswerSubmissionAlertIfNeeded]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_InsertAnswerSubmissionAlertIfNeeded]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_LockByPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_LockByPerson]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UnLockByPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UnLockByPerson]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateCallOutcome]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateCallOutcome]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateKeepAlive]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateKeepAlive]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateLoggedInToDialerState]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateLoggedInToDialerState]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateProblemState]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateProblemState]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateStartTime]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateStartTime]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_UpdateStatusLogout]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_UpdateStatusLogout]';


GO
PRINT N'Refreshing [dbo].[BvSpFinishInterviewerBreak]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpFinishInterviewerBreak]';


GO
PRINT N'Refreshing [dbo].[BvSpGetInterviewerActiveBreak]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetInterviewerActiveBreak]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Update]';


GO
PRINT N'Checking existing data against newly created constraints';


GO
ALTER TABLE [dbo].[BvMessageToPerson] WITH CHECK CHECK CONSTRAINT [FK_BvMessageToPerson_BvPerson];

ALTER TABLE [dbo].[BvPerson] WITH CHECK CHECK CONSTRAINT [FK_BvPerson_BvSurvey];

ALTER TABLE [dbo].[BvPerson] WITH CHECK CHECK CONSTRAINT [FK_BvPerson_CallGroupID];

ALTER TABLE [dbo].[BvPersonMonitoring] WITH CHECK CHECK CONSTRAINT [FK_BvPersonMonitoring_BvPerson];


GO
PRINT N'Update complete.';