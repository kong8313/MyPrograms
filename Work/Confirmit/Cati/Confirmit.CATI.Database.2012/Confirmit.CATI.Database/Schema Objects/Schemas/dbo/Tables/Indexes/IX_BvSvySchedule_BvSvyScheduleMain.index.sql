CREATE UNIQUE NONCLUSTERED INDEX [IX_BvSvyScheduleMain] ON [dbo].[BvSvySchedule]
(
	[SurveySID] ASC,
    [ShiftTypeID] ASC,
	[ExplicitSID] ASC,
    [DialTypeId] ASC,
	[CellID] ASC,
	[Priority] DESC,
	[TimeInShift] ASC,
	[ExplicitType] DESC,
	[CallOrder] ASC,
	[InterviewID] ASC
)
INCLUDE ( 	[ID],
	[CallState],
	[ApptID],
	[ConditionValue],
	[ExpireTime])