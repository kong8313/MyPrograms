CREATE VIEW [dbo].[BvViewBothCallHistories]
AS
SELECT
    [Id], [FiredTime], [ApptID], [ShiftTypeID], [InterviewID], [SurveyId], [ITS], [DialingMode], [CallState], [Priority], 
    [TimeInShift], [ExpireTime], [ExplicitSID], [ExplicitType], [CellId], [OperationId], [OperationType], [CallCenterId],
    [BlockedByFcd], [DialTypeId]
FROM dbo.[BvCallHistory]
UNION
SELECT
    [Id], [FiredTime], [ApptID], [ShiftTypeID], [InterviewID], [SurveyId], [ITS], [DialingMode], [CallState], [Priority],
    [TimeInShift], [ExpireTime], [ExplicitSID], [ExplicitType], [CellId], [OperationId], [OperationType], [CallCenterId],
    [BlockedByFcd], [DialTypeId]
FROM dbo.[BvCallHistoryEx]
