CREATE FUNCTION [dbo].[GetPriorityCallByExplicitSidAndShiftTypeIdClustered]
(   @DialTypeId TINYINT,
    @ExplicitSID INT,
    @ShiftTypeID INT,
	@SurveyID INT,
	@CellId INT,
    @SuitableTimeForCalls DATETIME,
	@TopCount INT)
RETURNS TABLE 
AS RETURN
(
	SELECT TOP (@TopCount) c.*
        FROM BvSvySchedule c with(readpast)
        WHERE DialTypeId = @DialTypeId AND
			  CallState = 2 AND
			  c.CellID = @CellID AND
			  c.ExplicitSID = @ExplicitSID and
			  c.ShiftTypeID = @ShiftTypeID and
			  TimeInShift <= @SuitableTimeForCalls AND
			  c.SurveySid = @SurveyID 
		ORDER BY Priority DESC,
				 TimeInShift,
				 ExplicitType DESC,
				 CallOrder 
)