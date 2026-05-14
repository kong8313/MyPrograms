PRINT N'Altering Procedure [dbo].[BvSpLookUpByPerson]...';


GO
ALTER PROCEDURE [dbo].[BvSpLookUpByPerson]
	@personId INT,
	@SuitableTimeForCalls DATETIME
AS
    IF @personId is null
    BEGIN
        SELECT 0 CallId, 0 SurveyId, 0 InterviewId, 0 ActiveDialId where 1 = 0
        RETURN 0
    END

    DECLARE @interviewId INT
    DECLARE @surveyId INT
	DECLARE @CallCenterId INT = (SELECT CallCenterID FROM BvPerson WHERE SID = @personId )

	create table #output(CallId int, SurveyId int, InterviewId int, ActiveDialId int)

	create table #surveySids(id int, objectSid int, dialType tinyint)

	insert into #surveySids
	select distinct s.SID, l.ObjectSid, l.DialTypeId
	FROM [BvFnSurvey_GetByCallCenterId](@CallCenterId) s
	CROSS JOIN BvLoginGroup l
	WHERE s.DialMode !=  4 AND State =1 AND l.PersonSid = @personId AND EXISTS
	      (select * from bvsvyschedule c
		   where c.SurveySID = s.SID and c.ExplicitSID = l.ObjectSID and c.DialTypeId = l.DialTypeId)
    
    ;WITH calls AS
	(
	    SELECT TOP(1) c.*
		FROM #surveySids s
		INNER JOIN BvActiveShiftTypeZone a on a.Surveyid = s.Id
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](s.dialType, s.ObjectSID, a.Id, s.Id, @SuitableTimeForCalls, 1) c
		ORDER BY Priority DESC,
				 a.ShiftPriority DESC,
                 TimeInShift,
				 ExplicitType DESC,
				 CallOrder
	)
	UPDATE calls
	SET CallState = -1,
		@interviewId = InterviewID,
		@surveyId = SurveySid
	OUTPUT
	   deleted.ID,
	   deleted.SurveySID,
	   deleted.InterviewID,
	   deleted.ActiveDialId
	INTO #output(CallId, SurveyId, InterviewId, ActiveDialId)
	
	SELECT * FROM #output
	
	IF @@ROWCOUNT = 0 RETURN 0
	
	UPDATE BvAppointment 
	SET State = 2 
	WHERE State = 1 AND 
	      SurveysId = @surveyId AND 
	      InterviewSid = @interviewId
RETURN 0
GO
PRINT N'Altering Procedure [dbo].[BvSpLookUpByPerson_ForAssignmentMode]...';


GO
ALTER PROCEDURE [dbo].[BvSpLookUpByPerson_ForAssignmentMode]
	@surveyId INT,
	@personId INT,
	@SuitableTimeForCalls DATETIME
AS
    IF @personId is null
    BEGIN
        SELECT 0 CallId, 0 SurveyId, 0 InterviewId, 0 ActiveDialId where 1 = 0
        RETURN 0
    END

    DECLARE @interviewId INT
    
	create table #output(CallId int, SurveyId int, InterviewId int, ActiveDialId int)
    
	;WITH calls AS
	(
	    SELECT TOP(1) c.*
        FROM BvLoginGroup t
		INNER JOIN BvActiveShiftTypeZone a on a.Surveyid = t.SurveySid and t.SurveySid = @surveyId and t.PersonSID = @personId
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](t.DialTypeId, t.ObjectSID, a.Id, @surveyId, @SuitableTimeForCalls, 1) c
		ORDER BY Priority DESC,
				 a.ShiftPriority DESC,
                 TimeInShift,
				 ExplicitType DESC,
				 CallOrder
	)
	UPDATE calls
	SET CallState = -1,
		@interviewId = InterviewID,
		@surveyId = SurveySid
	OUTPUT
	   deleted.ID,
	   deleted.SurveySID,
	   deleted.InterviewID,
	   deleted.ActiveDialId
	INTO #output(CallId, SurveyId, InterviewId, ActiveDialId)
	
	SELECT * FROM #output
	
	IF @@ROWCOUNT = 0 RETURN 0
	
	UPDATE BvAppointment 
	SET State = 2 
	WHERE State = 1 AND 
	      SurveysId = @surveyId AND 
	      InterviewSid = @interviewId
RETURN 0
GO
PRINT N'Altering Procedure [dbo].[BvSpLookUpByPerson_ForAssignmentModeClustered]...';


GO
ALTER PROCEDURE [dbo].[BvSpLookUpByPerson_ForAssignmentModeClustered]
	@surveyId INT,
	@personId INT,
	@SuitableTimeForCalls DATETIME
AS
    IF @personId is null
    BEGIN
        SELECT 0 CallId, 0 SurveyId, 0 InterviewId, 0 ActiveDialId where 1 = 0
        RETURN 0
    END

    DECLARE @interviewId INT

    create table #output(CallId int, SurveyId int, InterviewId int, ActiveDialId int)

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
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeIdClustered](t.DialTypeId, t.ObjectSID, a.Id, @surveyId, oc.CellId, @SuitableTimeForCalls, 1) c
		ORDER BY Priority DESC,
				 a.ShiftPriority DESC,
                 TimeInShift,
				 ExplicitType DESC,
				 CallOrder
	)
	UPDATE calls
	SET CallState = -1,
		@interviewId = InterviewID,
		@surveyId = SurveySid
	OUTPUT
	   deleted.ID,
	   deleted.SurveySID,
	   deleted.InterviewID,
	   deleted.ActiveDialId
	INTO #output(CallId, SurveyId, InterviewId, ActiveDialId)
	
	SELECT * FROM #output
	
	IF @@ROWCOUNT = 0 RETURN 0
	
	UPDATE BvAppointment 
	SET State = 2 
	WHERE State = 1 AND 
	      SurveysId = @surveyId AND 
	      InterviewSid = @interviewId
RETURN 0
GO
PRINT N'Altering Procedure [dbo].[BvSpSetNextInterviewForPerson]...';


GO
ALTER PROCEDURE [BvSpSetNextInterviewForPerson]
	@personId INT,
	@SurveySid INT,
	@InterviewId INT,
	@AssignmentMode INT
AS
    IF @personId is null
    BEGIN
        SELECT 0 CallId, 0 SurveyId, 0 InterviewId, 0 ActiveDialId where 1 = 0
        RETURN 0
    END


	DECLARE @CallCenterId INT = (SELECT CallCenterID FROM BvPerson WHERE SID = @personId )

    create table #output(CallId int, SurveyId int, InterviewId int, ActiveDialId int)

    if @AssignmentMode = 0		--assigned calls only mode
	BEGIN
		;WITH call AS
		(
			SELECT c.*
			FROM BvSvySchedule c
			JOIN [BvFnSurvey_GetByCallCenterId](@CallCenterId) s
				ON s.SID = c.SurveySID
			JOIN BvPersonRel p
				ON p.PersonSID = @personId AND p.ObjectSID = c.ExplicitSID
			WHERE c.SurveySID = @SurveySid AND c.InterviewID = @InterviewId 
				AND c.CallState IN (2 /* Scheduled */, 3 /* Disabled by user */) AND s.State = 1
		)
		UPDATE call
		SET CallState = -1
		OUTPUT
		   deleted.ID,
		   deleted.SurveySID,
		   deleted.InterviewID,
		   deleted.ActiveDialId
		INTO #output(CallId, SurveyId, InterviewId, ActiveDialId)
	END
	ELSE
	BEGIN
			;WITH call AS
		(
			SELECT c.*
			FROM BvSvySchedule c
			JOIN [BvFnSurvey_GetByCallCenterId](@CallCenterId) s
				ON s.SID = c.SurveySID
			JOIN BvPersonRel p
				ON p.PersonSID = @personId AND p.ObjectSID = @SurveySid
			WHERE c.SurveySID = @SurveySid AND c.InterviewID = @InterviewId 
			AND c.CallState IN (2 /* Scheduled */, 3 /* Disabled by user */) AND s.State = 1
		)
		UPDATE call
		SET CallState = -1
		OUTPUT
		   deleted.ID,
		   deleted.SurveySID,
		   deleted.InterviewID,
		   deleted.ActiveDialId
		INTO #output(CallId, SurveyId, InterviewId, ActiveDialId)
	END
	
	SELECT * FROM #output
	
	IF @@ROWCOUNT = 0 RETURN 0
	
	UPDATE BvAppointment 
	SET State = 2 
	WHERE State = 1 AND 
	      SurveysId = @SurveySId AND 
	      InterviewSid = @InterviewId
RETURN 0
GO
PRINT N'Altering Procedure [dbo].[BvSpCall_GetExpiredAndLock]...';


GO
ALTER PROCEDURE [dbo].[BvSpCall_GetExpiredAndLock]
	@LastId INT,
	@Now DATETIME
AS
	DECLARE @SurveyId INT
	DECLARE @InterviewId INT
	DECLARE @OldCallState INT

	;WITH data as (
	SELECT TOP(1) * FROM dbo.[BvSvySchedule] with(readpast, INDEX([IX_BvTime]))
	WHERE CallState > 0 AND ExpireTime < @Now AND ID > @LastId
          AND NOT EXISTS (SELECT 1 FROM [BvTasks] WHERE [CallID] = [BvSvySchedule].[ID])
	ORDER BY ID
	)
	UPDATE data SET @OldCallState = CallState, @SurveyId = SurveySID, @InterviewId = InterviewId, CallState = -1

	UPDATE BvAppointment
	SET STATE = 2
	WHERE SurveySID = @SurveyID AND InterviewSID = @InterviewID AND STATE = 1

	SELECT
		BvSvySchedule.[ID] callid,
		BvSvySchedule.ApptID,
		BvSvySchedule.SurveySID,
		BvSvySchedule.InterviewID iid,
		ISNULL( @OldCallState, BvSvySchedule.CallState ) as CallState,
		ISNULL( BvShiftZones.[ShiftTypeID], BvSvySchedule.[ShiftTypeID] ) ShiftID,
		BvSvySchedule.Priority,
		BvSvySchedule.TimeInShift,
		BvSvySchedule.ExpireTime TimeToExpire,
		CASE WHEN BvSvySchedule.ExplicitType = 2 THEN BvSvySchedule.ExplicitSID ELSE 0 END AS Resource,
		BvSvySchedule.ExplicitType Resource_Type,
		OldPriority,
		RuleNumber,
		ConditionValue,
		BvSvySchedule.CellId,
		BvSvySchedule.DialTypeId,
		BvSvySchedule.Type,
		BvSvySchedule.[DialerId],
		BvSvySchedule.[ActiveDialId]
	FROM BvSvySchedule
	LEFT JOIN BvShiftZones ON BvSvySchedule.ShiftTypeID = BvShiftZones.[ID]
	WHERE BvSvySchedule.SurveySID = @SurveyID AND BvSvySchedule.InterviewID = @InterviewID
GO
PRINT N'Update complete.';
