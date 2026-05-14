PRINT N'Altering [dbo].[GetPriorityCallByExplicitSidAndShiftTypeId]...';


GO
ALTER FUNCTION [dbo].[GetPriorityCallByExplicitSidAndShiftTypeId]
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
GO
PRINT N'Refreshing [dbo].[GetCallsForGroupForPredictiveSurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[GetCallsForGroupForPredictiveSurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnly]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnly]';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyByPersonGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyByPersonGroup]';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned]';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson]';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson_ForAssignmentMode]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson_ForAssignmentMode]';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]';


GO
PRINT N'Update complete.';


GO
