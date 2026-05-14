CREATE INDEX [IX_BvCallHistoryFiredTime_i_its_SurveyId]
	ON [dbo].BvCallHistory
(
	[FiredTime] ASC
) 
INCLUDE ([its], [SurveyId])
GO