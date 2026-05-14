GO
PRINT N'Creating [dbo].[BvScheduleError]...';


GO
CREATE TABLE [dbo].[BvScheduleError] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [ScheduleID]     INT            NOT NULL,
    [SurveySid]      INT            NOT NULL,
    [InterviewId]    INT            NOT NULL,
    [Timestamp]      DATETIME       NOT NULL,
    [TriggeredBy]    NVARCHAR (255) NOT NULL,
    [ExtendedStatus] NVARCHAR (255) NOT NULL,
    [RuleNumber]     NVARCHAR (255) NOT NULL,
    [Action]         NVARCHAR (255) NOT NULL,
    [Message]        NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_BvScheduleError_ID] PRIMARY KEY NONCLUSTERED ([Id] ASC)
);

GO
CREATE CLUSTERED INDEX [IX_BvScheduleError_ScheduleID_Timestamp]
    ON [dbo].[BvScheduleError]([ScheduleID] ASC, [Timestamp] ASC);


GO
ALTER TABLE [dbo].[BvScheduleError] WITH NOCHECK
    ADD CONSTRAINT [FK_BvScheduleError_BvSchedule.table] FOREIGN KEY ([ScheduleID]) REFERENCES [dbo].[BvSchedule] ([ScheduleID]) ON DELETE CASCADE;

GO
PRINT N'Update complete.';


GO
PRINT N'Adding SchedulingScript.ErrorLogSize system setting';

GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
    SELECT 'SchedulingScript.ErrorLogSize', 'Size of Log Table for scheduling script errors', 'Scheduling script', 'Limit for amount of rows in BvScheduleError table', 1, 0, '100'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO