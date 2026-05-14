CREATE INDEX [IX_BvCallHistory_BlockByFCD_SurveyId_InterviewId]
	ON [dbo].[BvCallHistory] 
	(
		BlockedByFcd, 
		SurveyId, 
		InterviewId
	)
	INCLUDE(FiredTime)
