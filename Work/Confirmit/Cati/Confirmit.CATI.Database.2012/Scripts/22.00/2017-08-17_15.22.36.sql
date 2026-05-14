PRINT N'Altering [dbo].[BvInboundCallsHistory]...';

ALTER TABLE [dbo].[BvInboundCallsHistory] ADD [InboundCallId] VARCHAR(255) NOT NULL

GO

PRINT N'Creating [dbo].[BvInboundCallsHistory].[IX_BvInboundCallsHistory_SurveyId_InterviewId]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvInboundCallsHistory_SurveyId_InterviewId] ON [dbo].[BvInboundCallsHistory] (SurveyId, InterviewId)

GO
PRINT N'Update complete.';