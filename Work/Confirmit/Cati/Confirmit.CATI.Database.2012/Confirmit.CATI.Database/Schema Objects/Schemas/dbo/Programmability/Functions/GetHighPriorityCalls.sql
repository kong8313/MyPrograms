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
		SELECT SUM(cnt) as cnt, SID, DialTypeId
		FROM vLogins WITH ( noexpand, INDEX([pk_vLogins]) )
		WHERE  SurveySID = @SurveySid OR SurveySID = 0
		GROUP BY SID, DialTypeId
	)
	SELECT TOP 500			--we need this to have ORDER BY plus we do not need more calls because we do paging
				[ID],
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
	FROM LoggedInGroups c
	CROSS APPLY dbo.GetCallsPerGroup(c.DialTypeId, c.cnt*@maxCallsPerGroup, c.sid, @SurveySID, @SuitableTimeForCalls) cpg
	ORDER BY Priority DESC,
				   ShiftPriority DESC,
                   TimeInShift,
                   ExplicitType DESC,
                   CallOrder
)
