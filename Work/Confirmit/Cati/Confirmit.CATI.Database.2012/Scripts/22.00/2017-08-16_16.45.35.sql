PRINT N'Altering [dbo].[BvTasks]...';


GO
ALTER TABLE [dbo].[BvTasks]
    ADD [CallType]       TINYINT CONSTRAINT [DF_BvTasks_CallType] DEFAULT (0) NOT NULL


GO
PRINT N'Update complete.';
