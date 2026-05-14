GO
PRINT N'Altering [dbo].[BvSpTransfer_GetInternalTargets]...';


GO
ALTER PROCEDURE [dbo].[BvSpTransfer_GetInternalTargets]
        @PersonId INT,
        @SurveyId INT,
		@DialTypeId TINYINT,
		@DialerId INT

AS
	SELECT Name, Description, CallTransferBehavior, c.*
		FROM BvPersonGroup pg
		cross apply ( 
			SELECT count(*) AS CountOfTotalInterviewersLoggedIn,
				   ISNULL( SUM( CASE WHEN t.InterviewState IN( 0/*NO_CALLS*/, 1/*SELECTING*/, 2/*WAITING*/) THEN 1 ELSE 0 END ), 0) AS CountOfFreeInterviewersLoggedIn
			FROM BvLoginGroup lg 
			INNER JOIN BvTasks t ON lg.PersonSID = t.PersonSID AND lg.DialTypeId = @DialTypeId
			WHERE lg.ObjectSID = pg.SID AND lg.PersonSID <> @PersonId AND ( pg.CallTransferBehavior = 2 /*from other surveys*/ OR lg.SurveySID = @SurveyId OR lg.SurveySID = 0 ) 
			 ) AS c
	WHERE pg.CallTransferBehavior <> 0
GO
PRINT N'Update complete.';


GO
