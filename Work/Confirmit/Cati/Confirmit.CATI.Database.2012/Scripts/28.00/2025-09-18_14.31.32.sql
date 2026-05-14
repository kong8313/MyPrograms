GO
PRINT N'Altering Table [dbo].[BvHistoryCustomFields]...';

GO
ALTER TABLE [dbo].[BvHistoryCustomFields]
    ADD [IsActive] BIT NOT NULL 
		CONSTRAINT DF_BvHistoryCustomFields_IsActive DEFAULT (1); 

GO
PRINT N'Update complete.';

GO