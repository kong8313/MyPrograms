CREATE INDEX [IX_BvCallHistorySurveyID_InterviewID_i_FiredTime]
	ON [dbo].BvCallHistory
(
	SurveyId ASC, 
	InterviewId ASC
) 
INCLUDE ([FiredTime])
GO