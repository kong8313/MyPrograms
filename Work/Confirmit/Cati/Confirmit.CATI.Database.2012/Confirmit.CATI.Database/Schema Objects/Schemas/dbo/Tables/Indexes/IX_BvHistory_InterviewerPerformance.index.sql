CREATE NONCLUSTERED INDEX [IX_BvHistory_InterviewerPerformance] ON [dbo].[BvHistory] 
(
	[FiredTime] ASC,
	[RoleID] ASC,
	[PersonSID] ASC,
	[ITS] ASC
)
INCLUDE 
( 
[WaitingTime],
[ConfirmitDuration],
[Duration],
[OpenEndReviewDuration]
)
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
