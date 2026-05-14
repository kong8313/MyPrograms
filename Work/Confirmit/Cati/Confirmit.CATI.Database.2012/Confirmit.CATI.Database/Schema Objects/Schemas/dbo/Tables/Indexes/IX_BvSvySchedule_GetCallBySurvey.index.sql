CREATE NONCLUSTERED INDEX [IX_GetCallBySurvey] ON [dbo].[BvSvySchedule] 
(
	SurveySID ASC,
	[ShiftTypeId] ASC,
	ExplicitSID ASC,
    [DialTypeId] ASC,
	priority DESC,
	TimeInShift ASC,
	ExplicitType DESC,
	CallOrder ASC
) INCLUDE( 	ID, ExpireTime, CallState )
WHERE ConditionValue <> 0