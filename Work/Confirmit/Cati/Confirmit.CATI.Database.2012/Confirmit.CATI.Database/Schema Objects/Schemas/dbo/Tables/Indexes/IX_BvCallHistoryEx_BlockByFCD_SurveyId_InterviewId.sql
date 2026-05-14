CREATE INDEX [IX_BvCallHistoryEx_BlockByFCD_SurveyId_InterviewId]
	ON [dbo].[BvCallHistoryEx] 
	(
		BlockedByFcd, 
		SurveyId, 
		InterviewId
	)
	INCLUDE(FiredTime)
