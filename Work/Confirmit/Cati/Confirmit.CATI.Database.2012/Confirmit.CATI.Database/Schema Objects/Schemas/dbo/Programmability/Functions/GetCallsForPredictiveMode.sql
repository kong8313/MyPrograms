CREATE FUNCTION [dbo].[GetCallsForPredictiveMode]
(   @DialTypeId TINYINT,
	@rowCount AS INT,
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
	      FROM BvSvySchedule with(readpast, INDEX(IX_BvSvyScheduleMain))
          WHERE DialTypeId = @DialTypeId AND
		        SurveySid = @SurveySid AND
                ExplicitSID = @ExplicitSID AND
				CellId = 0 AND
                CallState = 2 AND
                TimeInShift <= @TimeToRun AND
                ShiftTypeId = @ShiftTypeId
          ORDER BY Priority DESC,
                   TimeInShift,
                   ExplicitType DESC,
                   CallOrder )

