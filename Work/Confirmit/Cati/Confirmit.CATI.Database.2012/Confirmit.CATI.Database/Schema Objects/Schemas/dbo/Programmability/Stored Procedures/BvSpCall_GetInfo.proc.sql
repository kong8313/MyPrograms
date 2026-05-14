CREATE PROCEDURE [dbo].[BvSpCall_GetInfo]
 @CallID INT
AS
 SELECT
   [ID] callId,
   [ApptID],
   [SurveySID],
   [InterviewID] iid,
   [CallState],
   [ShiftTypeID] ShiftID,
   [Priority],
   [TimeInShift],
   [ExpireTime] TimeToExpire,
   [ExplicitSID] Resource,
   [ExplicitType] Resource_Type,
   [OldPriority],
   [RuleNumber],
   [ConditionValue],
   [CellId],
   [DialTypeId],
   [Type],
   [DialerId],
   [ActiveDialId]
 FROM [dbo].[BvSvySchedule]
 WHERE [ID] = @CallID