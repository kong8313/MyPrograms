CREATE TABLE [dbo].[BvPersonDeferredMonitoring]
(
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PersonSID] [int] NOT NULL,
	[InterviewID] [int] NOT NULL,
	[SurveySID] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[HasAudio] [bit] NOT NULL,
	[EventsFile] [varbinary](max) NOT NULL,
	[StartingFile] [nvarchar](max) NULL,
	[IsRecording] [bit] NOT NULL,
	[IsComplete] [bit] NOT NULL,
	[ClientTimeUtc] [datetime] NOT NULL CONSTRAINT DF_BvPersonDeferredMonitoring_ClientTimeUtc DEFAULT GETUTCDATE(),
	[ServerTimeUtc] [datetime] NOT NULL CONSTRAINT DF_BvPersonDeferredMonitoring_ServerTimeUtc DEFAULT GETUTCDATE(),
	[RequestAudio] [bit] NOT NULL CONSTRAINT DF_BvPersonDeferredMonitoring_RequestAudio DEFAULT 0, 
    [CallID] [int] SPARSE NULL, 
    [ExtendedStatus] [int] NULL,
	[CallCenterId] [int] NOT NULL,
	[RespondentName] NVARCHAR (255) NULL,
	[TelephoneNumber] VARCHAR (255) NULL,
	[InterviewDuration] [int] NOT NULL CONSTRAINT DF_BvPersonDeferredMonitoring_InterviewDuration DEFAULT 0, 
    [RecordCreationTime] DATETIME NOT NULL CONSTRAINT DF_BvPersonDeferredMonitoring_RecordCreationTime DEFAULT GETUTCDATE(),
    [IsOldInterface] [bit] NOT NULL CONSTRAINT DF_BvPersonDeferredMonitoring_IsOldInterface DEFAULT 0, 
    [IsRetained] BIT NOT NULL CONSTRAINT DF_BvPersonDeferredMonitoring_IsRetained DEFAULT 0, 
    [Comment] NVARCHAR(1024) NULL
)
GO

EXEC sp_tableoption 'BvPersonDeferredMonitoring', 'large value types out of row', 1;
GO