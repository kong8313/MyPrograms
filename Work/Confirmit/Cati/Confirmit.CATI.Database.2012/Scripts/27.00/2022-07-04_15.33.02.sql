GO
PRINT N'Altering [dbo].[BvScheduleError]...';


GO
ALTER TABLE [dbo].[BvScheduleError]
    ADD [NotificationSent] BIT CONSTRAINT [DF_BvScheduleError_NotificationSent] DEFAULT (0) NOT NULL;

GO
UPDATE BvScheduleError SET NotificationSent = 1;


GO
PRINT N'Update complete.';


GO

