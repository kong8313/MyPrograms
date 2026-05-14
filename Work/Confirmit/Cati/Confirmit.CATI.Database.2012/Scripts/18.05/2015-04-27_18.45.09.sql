PRINT N'Creating [dbo].[GetTopCallsForShiftTypeGroupCell]...';


GO
CREATE FUNCTION [dbo].[GetTopCallsForShiftTypeGroupCell]
(   @rowCount AS INT,
    @ShiftTypeId INT,
    @ExplicitSID AS INT,
    @SurveySid AS INT,
    @CellId AS INT,
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
		CellId = @CellId AND
                CallState = 2 AND
                TimeInShift <= @TimeToRun AND
                ShiftTypeId = @ShiftTypeId
          ORDER BY Priority DESC,
                   TimeInShift,
                   ExplicitType DESC,
                   CallOrder )
GO
PRINT N'Altering [dbo].[GetCallsPerGroup]...';


GO
ALTER FUNCTION [dbo].[GetCallsPerGroup]
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
	CROSS JOIN
		(SELECT cc.CellId AS CellId from  BvClusteredQuotaCell cc
			WHERE  cc.SurveyId = @SurveySid
		 UNION 
		 SELECT 0 AS CellId
		 ) cells
	CROSS APPLY dbo.[GetTopCallsForShiftTypeGroupCell](@rowCount, a.Id, @ExplicitSID, @SurveySid, cells.CellId, @TimeToRun) c
	WHERE a.SurveyId = @SurveySid
    ORDER BY Priority DESC,
                   TimeInShift,
                   ExplicitType DESC,
                   CallOrder )
GO
PRINT N'Refreshing [dbo].[GetHighPriorityCalls]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[GetHighPriorityCalls]';


GO
PRINT N'Update complete.';


GO
