PRINT N'Altering [dbo].[BvSchedule]...';


GO
ALTER TABLE [dbo].[BvSchedule]
    ADD [IsSampleUpdateRuleSet] BIT CONSTRAINT [DF_BvSchedule_IsSampleUpdateRuleSet] DEFAULT (0) NOT NULL;


GO

UPDATE BvSchedule SET RegenerateIsRequired = 1
GO