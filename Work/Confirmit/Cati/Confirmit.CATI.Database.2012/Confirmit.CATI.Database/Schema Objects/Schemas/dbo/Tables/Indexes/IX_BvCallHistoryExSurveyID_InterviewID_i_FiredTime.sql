CREATE INDEX [IX_BvCallHistoryExSurveyID_InterviewID_i_FiredTime]
	ON [dbo].[BvCallHistoryEx]
(
	SurveyId ASC, 
	InterviewId ASC
) 
INCLUDE ([FiredTime])
GO