PRINT N'Altering [dbo].[BvInterview]...';

ALTER TABLE [dbo].[BvInterview]
    ADD [ReviewStatus] TINYINT NOT NULL CONSTRAINT DF_BvInterview_ReviewStatus DEFAULT (0) 

GO

UPDATE [dbo].[BvInterview] SET [ReviewStatus] = 1 where [IsSentToReview] = 1

GO
PRINT N'Creating [dbo].[BvSystemState]...';


GO
CREATE TABLE [dbo].[BvSystemState] (
    [SystemName] NVARCHAR (256) NOT NULL,
    [Value]      NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_BvSystemState_SystemName] PRIMARY KEY CLUSTERED ([SystemName] ASC)
);

