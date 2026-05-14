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
	INNER JOIN BvActiveShiftTypeZone a on a.SurveyId = @SurveySid
			CROSS APPLY dbo.GetCallsForPredictiveMode(c.cnt*@maxCallsPerGroup, a.Id, c.sid, @SurveySID, @SuitableTimeForCalls) cpg
)
GO
PRINT N'Update complete.';


GO
