

GO
PRINT N'Creating [dbo].[BvSchedulingScriptLog]...';


GO
CREATE TABLE [dbo].[BvSchedulingScriptLog] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [ScheduleID]  INT            NOT NULL,
    [SurveySid]   INT            NOT NULL,
    [InterviewId] INT            NOT NULL,
    [Timestamp]   DATETIME       NOT NULL,
    [LogMessages] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_BvSchedulingScriptLog_ID] PRIMARY KEY NONCLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[BvSchedulingScriptLog].[IX_BvSchedulingScriptLog_SurveySid_InterviewId]...';


GO
CREATE CLUSTERED INDEX [IX_BvSchedulingScriptLog_SurveySid_InterviewId]
    ON [dbo].[BvSchedulingScriptLog]([SurveySid] ASC, [InterviewId] ASC);


GO
PRINT N'Creating [dbo].[BvSchedulingScriptLog].[IX_BvSchedulingScriptLog_TimeStamp]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvSchedulingScriptLog_TimeStamp]
    ON [dbo].[BvSchedulingScriptLog]([Timestamp] ASC);


GO
PRINT N'Update complete.';


GO


GO
PRINT N'RoutineMaintenance.Actions.SchedulingScriptLogTableCleanup.ShiftType system setting';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'RoutineMaintenance.Actions.SchedulingScriptLogTableCleanup.ShiftType', 'Scheduling script execution log table cleanup maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '1'
	UNION ALL 
	SELECT 'RoutineMaintenance.Actions.SchedulingScriptLogTableCleanup.ExpirationPeriod', 'Scheduling script execution log table cleanup expiration period', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '90.00:00:00'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';