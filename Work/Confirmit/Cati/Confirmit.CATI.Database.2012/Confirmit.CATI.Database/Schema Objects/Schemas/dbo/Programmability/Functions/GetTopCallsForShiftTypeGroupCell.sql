CREATE FUNCTION [dbo].[GetTopCallsForShiftTypeGroupCell]
(   @DialTypeId as TINYINT,
    @rowCount AS INT,
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
				[Priority],
				ShiftTypeID, 
				ExpireTime					
	  FROM BvSvySchedule

          WHERE SurveySid = @SurveySid AND
                ExplicitSID = @ExplicitSID AND
				CellId = @CellId AND
				DialTypeId = @DialTypeId AND
                CallState = 2 AND
                TimeInShift <= @TimeToRun AND
                ShiftTypeId = @ShiftTypeId
          ORDER BY [Priority] DESC,
                   TimeInShift,
                   ExplicitType DESC,
                   CallOrder )
