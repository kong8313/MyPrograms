PRINT N'Altering [dbo].[BvTasks]...';
GO

ALTER TABLE [dbo].[BvTasks]
    ADD [NewSurveySID] INT CONSTRAINT [DF_BvTasks_NewSurveySID] DEFAULT (0) NOT NULL;
GO

PRINT N'Update complete.';
GO
