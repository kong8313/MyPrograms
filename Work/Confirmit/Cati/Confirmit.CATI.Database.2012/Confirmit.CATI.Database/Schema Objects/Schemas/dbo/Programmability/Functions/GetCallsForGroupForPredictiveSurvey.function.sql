CREATE FUNCTION dbo.GetCallsForGroupForPredictiveSurvey
(
    @DialTypeId TINYINT,
    @rowCount AS INT,
    @SurveySid AS INT,
    @ObjectSid AS INT,
	@SuitableTimeForCalls DATETIME
)
RETURNS TABLE
AS RETURN(
          SELECT TOP (@rowCount) c.*, a.ShiftPriority
          FROM BvActiveShiftTypeZone a
		  CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](@DialTypeId, @ObjectSid, a.Id, @SurveySID, @SuitableTimeForCalls, @rowCount) c
		  WHERE a.surveyid = @SurveySid
          ORDER BY priority DESC, a.ShiftPriority DESC, TimeInShift, ExplicitType DESC, CallOrder )