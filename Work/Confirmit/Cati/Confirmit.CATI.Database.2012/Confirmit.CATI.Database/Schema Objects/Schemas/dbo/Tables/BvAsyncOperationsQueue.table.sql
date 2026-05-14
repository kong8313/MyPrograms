CREATE TABLE [dbo].[BvAsyncOperationQueue]
(
	[Id] INT IDENTITY NOT NULL,
	[IsInitiatedBySystem] BIT NOT NULL,
	[Type] TINYINT NOT NULL,
	[Title] NVARCHAR(255) NULL,
	[State] TINYINT NOT NULL,
	[Parameters] XML NOT NULL,
	[SurveySid] INT NOT NULL,
	[Priority] INT NOT NULL,
	[QueuedDate] DateTime NOT NULL,
	[StartedDate] DateTime NULL,
	[FinishedDate] DateTime NULL,
	[HeartBeat] DateTime NULL,
	[TotalItemsCount] INT NOT NULL,
	[ProcessedItemsCount] INT NOT NULL,
	[FailedItemsCount] INT NOT NULL,
	[CreatedBySupervisorName] NVARCHAR(255) NULL,
	[AbortedBySupervisorName] NVARCHAR(255) NULL,
	[Server] NVARCHAR(256) NOT NULL,
	[Error] NVARCHAR(MAX) NULL,
	[Text] NVARCHAR(MAX) NULL,
	[CallCenterId] INT NOT NULL
	CONSTRAINT PK_BvAsyncOperationQueue_Id PRIMARY KEY CLUSTERED (Id),
);
GO

CREATE UNIQUE NONCLUSTERED INDEX IX_BvAsyncOperationQueue_Priority_Id ON [dbo].[BvAsyncOperationQueue] ([Priority], [Id]);
GO

CREATE NONCLUSTERED INDEX IX_BvAsyncOperationQueue_State_Priority_Id ON [dbo].[BvAsyncOperationQueue] ([State], [Priority], [Id]);
