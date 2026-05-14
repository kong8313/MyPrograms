PRINT N'Dropping [dbo].[BvSpUpdateInProgressCallsToScheduled]...';


GO
DROP PROCEDURE [dbo].[BvSpUpdateInProgressCallsToScheduled];


GO
PRINT N'Creating [dbo].[BvSpSurvey_Shutdown]...';


GO
CREATE PROCEDURE [dbo].[BvSpSurvey_Shutdown]
    @SurveyId INT
AS
    UPDATE BvSvySchedule 
		SET CallState = 2 
		WHERE SurveySID = @SurveyId AND CallState BETWEEN -2 AND -1
GO
PRINT N'Update complete.';


GO
