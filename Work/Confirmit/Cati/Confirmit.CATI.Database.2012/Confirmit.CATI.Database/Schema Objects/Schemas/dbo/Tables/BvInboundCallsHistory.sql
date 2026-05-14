CREATE TABLE [dbo].[BvInboundCallsHistory]
(
	[Id] INT NOT NULL IDENTITY(1,1), 
	[FiredTime] DATETIME NOT NULL,
    [InboundTelNumber] VARCHAR(255) NULL, 
    [RespondentTelNumber] VARCHAR(255) NULL, 
    [SurveyId] INT NULL, 
    [InterviewId] INT NULL, 
    [OperationType] INT NOT NULL,
	[InboundCallId] VARCHAR(255) NOT NULL,
	CONSTRAINT [PK_BvInboundCallsHistory] PRIMARY KEY ([Id])
)

GO

CREATE NONCLUSTERED INDEX [IX_BvInboundCallsHistory_SurveyId_InterviewId] ON [dbo].[BvInboundCallsHistory] (SurveyId, InterviewId)
