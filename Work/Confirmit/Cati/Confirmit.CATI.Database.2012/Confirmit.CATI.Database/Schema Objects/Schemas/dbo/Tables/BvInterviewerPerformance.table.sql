CREATE TABLE [dbo].[BvInterviewerPerformance](
	[InterviewerId] [int] NOT NULL,
	[InterviewerName] [nvarchar](255) NOT NULL,
	[SurveyId] [int] NOT NULL,
	[TotalInterviewCount] [int] NOT NULL,
	[CompletedInterviewCount] [int] NOT NULL,
	[CompletedInLastHourCount] [int] NOT NULL,
	[InterviewingTime] [int] NOT NULL,
 CONSTRAINT [PK_BvInterviewerPerformance_InterviewerId_SurveyId] PRIMARY KEY CLUSTERED 
(
	[InterviewerId] ASC,
	[SurveyId]
))