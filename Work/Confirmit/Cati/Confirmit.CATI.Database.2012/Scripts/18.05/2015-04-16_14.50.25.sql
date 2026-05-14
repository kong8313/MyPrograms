PRINT N'Creating [dbo].[GetCallsPerGroup]...';


GO
CREATE FUNCTION [dbo].[GetCallsPerGroup]
(
    @rowCount AS INT,
    @ExplicitSID AS INT,
    @SurveySid AS INT,
    @TimeToRun AS DATETIME)
RETURNS TABLE
AS RETURN(
          SELECT TOP(@rowCount) 
		c.[ID],
        ExplicitSID,
		ExplicitType,
        SurveySID,
        InterviewID,
        CallState,
		ApptId,
		TimeInShift,
		CallOrder,
		Priority,
		ShiftTypeID, 
		ExpireTime					
    FROM BvActiveShiftTypeZone a 
	CROSS APPLY dbo.GetCallsForPredictiveMode(@rowCount, a.Id, @ExplicitSID, @SurveySid, @TimeToRun) c
    WHERE a.SurveyId = @SurveySid
    ORDER BY Priority DESC,
                   TimeInShift,
                   ExplicitType DESC,
                   CallOrder )
GO
PRINT N'Altering [dbo].[GetHighPriorityCalls]...';


GO
ALTER FUNCTION [dbo].[GetHighPriorityCalls]
(
	  @SurveySid AS INT,
	  @SuitableTimeForCalls DATETIME,
	  @maxCallsPerGroup AS INT
)
RETURNS TABLE
AS RETURN(

	WITH LoggedInGroups AS 
	(
		SELECT SUM(cnt) as cnt, SID
		FROM vLogins WITH ( noexpand, INDEX([pk_vLogins]) )
		WHERE  SurveySID = @SurveySid OR SurveySID = 0
		GROUP BY SID
	)
	SELECT  cpg.*
	FROM LoggedInGroups c
	CROSS APPLY dbo.GetCallsPerGroup(c.cnt*@maxCallsPerGroup, c.sid, @SurveySID, @SuitableTimeForCalls) cpg
)
GO
PRINT N'Update complete.';


GO
