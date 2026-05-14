CREATE FUNCTION [dbo].[GetPriorityCallByExplicitSidAndShiftTypeId]
(   @DialTypeId TINYINT,
    @ExplicitSID INT,
    @ShiftTypeID INT,
	@SurveyID INT,
    @SuitableTimeForCalls DATETIME,
	@TopCount INT)
RETURNS TABLE 
AS RETURN
(
	    SELECT TOP(@TopCount) c.*
        FROM BvSvySchedule c with(readpast, INDEX(IX_BvSvyScheduleMain))
		WHERE DialTypeId = @DialTypeId AND
		      CallState = 2 AND
			  c.ExplicitSID = @ExplicitSID and
			  c.ShiftTypeID = @ShiftTypeID and
			  c.CellId = 0 and
			  TimeInShift <= @SuitableTimeForCalls AND
			  c.SurveySid = @SurveyID
		ORDER BY Priority DESC,
				 TimeInShift,
				 ExplicitType DESC,
				 CallOrder 
)