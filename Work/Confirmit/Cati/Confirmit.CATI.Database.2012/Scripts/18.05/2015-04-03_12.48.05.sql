PRINT N'Altering [dbo].[GetCallsForPredictiveMode]...';


GO
ALTER FUNCTION [dbo].[GetCallsForPredictiveMode]
(   @rowCount AS INT,
    @ShiftTypeId INT,
    @ExplicitSID AS INT,
    @SurveySid AS INT,
    @TimeToRun AS DATETIME) 
RETURNS TABLE
AS RETURN(
          SELECT TOP(@rowCount) [ID],
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
	  FROM BvSvySchedule
          WHERE SurveySid = @SurveySid AND
                ExplicitSID = @ExplicitSID AND
				CellId = 0 AND
                CallState = 2 AND
                TimeInShift <= @TimeToRun AND
                ShiftTypeId = @ShiftTypeId
          ORDER BY Priority DESC,
                   TimeInShift,
                   ExplicitType DESC,
                   CallOrder )
GO
PRINT N'Creating [dbo].[GetHighPriorityCalls]...';


GO
CREATE FUNCTION [dbo].[GetHighPriorityCalls]
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
			INNER JOIN BvActiveShiftTypeZone a on a.SurveyId = @SurveySid
		WHERE  SurveySID = @SurveySid OR SurveySID = 0
		GROUP BY SID
	)
	SELECT  cpg.*
	FROM LoggedInGroups c
	INNER JOIN BvActiveShiftTypeZone a on a.SurveyId = @SurveySid
			CROSS APPLY dbo.GetCallsForPredictiveMode(c.cnt*@maxCallsPerGroup, a.Id, c.sid, @SurveySID, @SuitableTimeForCalls) cpg
)
GO
PRINT N'Refreshing [dbo].[BvSpGetActiveCallsForSurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetActiveCallsForSurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]';


GO
PRINT N'Update complete.';


GO
