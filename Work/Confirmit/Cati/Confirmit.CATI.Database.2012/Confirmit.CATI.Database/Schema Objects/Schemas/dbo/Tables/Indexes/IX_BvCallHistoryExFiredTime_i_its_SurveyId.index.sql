CREATE INDEX [IX_BvCallHistoryExFiredTime_i_its_SurveyId]
	ON [dbo].[BvCallHistoryEx]
(
	[FiredTime] ASC
) 
INCLUDE ([its], [SurveyId])
GO