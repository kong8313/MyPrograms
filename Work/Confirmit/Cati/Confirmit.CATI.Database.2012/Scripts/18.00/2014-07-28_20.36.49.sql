GO
PRINT N'Dropping [dbo].[BvSvySchedule].[IX_BvSvyScheduleMain]...';


GO
DROP INDEX [IX_BvSvyScheduleMain]
    ON [dbo].[BvSvySchedule];


GO
PRINT N'Dropping [dbo].[GetPriorityCallByExplicitSidAndShiftTypeIdClustered]...';


GO
DROP FUNCTION [dbo].[GetPriorityCallByExplicitSidAndShiftTypeIdClustered];


GO
PRINT N'Creating [dbo].[BvSvySchedule].[IX_BvSvyScheduleMain]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_BvSvyScheduleMain]
    ON [dbo].[BvSvySchedule]([SurveySID] ASC, [ShiftTypeID] ASC, [ExplicitSID] ASC, [CellId] ASC, [Priority] DESC, [TimeInShift] ASC, [ExplicitType] DESC, [CallOrder] ASC, [InterviewID] ASC)
    INCLUDE([ID], [CallState], [ApptID], [ConditionValue], [ExpireTime]);


GO
PRINT N'Altering [dbo].[GetCallsForPredictiveMode]...';


GO
ALTER FUNCTION [dbo].[GetCallsForPredictiveMode]
(   @rowCount AS INT,
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
								Priority
          FROM BvSvySchedule
          WHERE SurveySid = @SurveySid AND
                ExplicitSID = @ExplicitSID AND
				CellId = 0 AND
                CallState = 2 AND
                TimeInShift <= @TimeToRun AND
                ShiftTypeId = @ShiftTypeId
          ORDER BY Priority DESC,
                   TimeInShift,
                   ExplicitType DESC,
                   CallOrder )
GO
PRINT N'Altering [dbo].[GetPriorityCallByExplicitSidAndShiftTypeId]...';


GO
ALTER FUNCTION [dbo].[GetPriorityCallByExplicitSidAndShiftTypeId]
(   @ExplicitSID INT,
    @ShiftTypeID INT,
	@SurveyID INT,
    @SuitableTimeForCalls DATETIME,
	@TopCount INT)
RETURNS TABLE 
AS RETURN
(
	    SELECT TOP(@TopCount) c.*
        FROM BvSvySchedule c with(readpast)
		WHERE CallState = 2 AND
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
PRINT N'Creating [dbo].[GetPriorityCallByExplicitSidAndShiftTypeIdClustered]...';


GO
CREATE FUNCTION [dbo].[GetPriorityCallByExplicitSidAndShiftTypeIdClustered]
(   @ExplicitSID INT,
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
        WHERE CallState = 2 AND
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
GO
PRINT N'Altering [dbo].[BvSpLookUpByPerson_ForAssignmentModeClustered]...';


GO
ALTER PROCEDURE [dbo].[BvSpLookUpByPerson_ForAssignmentModeClustered]
	@surveyId INT,
	@personId INT,
	@SuitableTimeForCalls DATETIME
AS
    IF @personId is null and @SuitableTimeForCalls is null
	begin
		select 0 CallID,
		       0 SurveySID,
			   0 iid
		where 1 = 0
		return 0
	end

    DECLARE @interviewId INT
	DECLARE @callId INT
    DECLARE @rowCount INT
    
    ;WITH opennedCells as
	(
		SELECT 0 as CellId
		UNION 
		SELECT CellId FROM BvClusteredQuotaCell WHERE SurveyId = @SurveyID AND LiveCount < LiveLimit 
	),
	calls AS
	(
	    SELECT TOP(1) c.*
        FROM BvLoginGroup t
		INNER JOIN BvActiveShiftTypeZone a on a.Surveyid = t.SurveySid and t.SurveySid = @surveyId and t.PersonSID = @personId
		INNER JOIN opennedCells oc ON 1 = 1
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeIdClustered](t.ObjectSID, a.Id, @surveyId, oc.CellId, @SuitableTimeForCalls, 1) c
		ORDER BY Priority DESC,
                 TimeInShift,
				 ExplicitType DESC,
				 CallOrder
	)
	UPDATE calls
	SET CallState = -1,
	    ExpireTime = '9999-01-01 00:00:00.000',
		@interviewId = InterviewID,
		@surveyId = SurveySid,
		@callId = Id

	SET @rowCount = @@ROWCOUNT

	select @callID CallID, @surveyId SurveySID, @interviewId iid
	where @callID is not null

	IF(@rowCount = 0) RETURN 0
	
	UPDATE BvAppointment 
	SET State = 2 
	WHERE State = 1 AND 
	      SurveysId = @surveyId AND 
	      InterviewSid = @interviewId
RETURN 0
GO
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Refreshing [dbo].[BvSpGetActiveCallsForSurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetActiveCallsForSurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]';


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
PRINT N'Refreshing [dbo].[BvSpGetOpenedSurveys]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetOpenedSurveys]';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson]';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson_ForAssignmentMode]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpLookUpByPerson_ForAssignmentMode]';


GO
PRINT N'Update complete.';


GO
