CREATE NONCLUSTERED INDEX [IX_GetCallByCondition] ON [dbo].[BvSvySchedule] 
(
	SurveySID ASC,
	[ShiftTypeId] ASC,
	ExplicitSID ASC,
    [DialTypeId] ASC,
	ConditionValue ASC,
	priority DESC, 
	TimeInShift ASC, 
	ExplicitType DESC, 
	CallOrder ASC
) INCLUDE( 	ID, ExpireTime, CallState )
WHERE ConditionValue <> 0
