CREATE FUNCTION [dbo].[GetCallsPerGroup]
(
	@DialType as TINYINT,
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
				ExpireTime,
				a.ShiftPriority					
	FROM BvActiveShiftTypeZone a 
	CROSS JOIN
		(SELECT cc.CellId AS CellId from  BvClusteredQuotaCell cc
			WHERE  cc.SurveyId = @SurveySid
		 UNION 
		 SELECT 0 AS CellId
		 ) cells
	CROSS APPLY dbo.[GetTopCallsForShiftTypeGroupCell](@DialType, @rowCount, a.Id, @ExplicitSID, @SurveySid, cells.CellId, @TimeToRun) c
	WHERE a.SurveyId = @SurveySid
    ORDER BY Priority DESC,
				   a.ShiftPriority DESC,
                   TimeInShift,
                   ExplicitType DESC,
                   CallOrder )

