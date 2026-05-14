GO
PRINT N'Altering [dbo].[BvSpActiveDial_Update]...';


GO
ALTER PROCEDURE [dbo].[BvSpActiveDial_Update]
 @Id BIGINT,
 @Type TINYINT,
 @State TINYINT,
 @AnswerTime DATETIME,
 @TransferId NVARCHAR(MAX),
 @SurveyId INT,
 @CampaignId BIGINT,
 @InterviewId INT,
 @CallId INT,
 @MainPersonId INT,
 @JsonTransferState NVARCHAR(MAX),
 @TransferType TINYINT
AS
	DECLARE @OldCallId INT
	DECLARE @DialerId INT
	UPDATE BvActiveDial
		SET @OldCallId = CallId,
		    @DialerId = DialerId,
			Type = @Type,
			State = @State,
			AnswerTime = @AnswerTime,
			TransferId = @TransferId,
			SurveyId = @SurveyId,
			CampaignId = @CampaignId,
			InterviewId = @InterviewId,
			CallId = @CallId,
			MainPersonId = @MainPersonId,
			JsonTransferState = @JsonTransferState,
			TransferType = @TransferType
		WHERE Id = @Id
	IF ISNULL( @OldCallId, 0 ) <> ISNULL( @CallId, 0 )
	BEGIN
		IF @OldCallId IS NOT NULL
			UPDATE BvSvySchedule SET DialerId = 0, ActiveDialId = 0 WHERE ID = @OldCallId
		IF @CallId IS NOT NULL
			UPDATE BvSvySchedule SET DialerId = @DialerId, ActiveDialId = @Id WHERE ID = @CallId
	END
GO
PRINT N'Droping [dbo].[BvSpLookUpByPerson_ForSurvey]...';


GO
DROP PROCEDURE [dbo].[BvSpLookUpByPerson_ForSurvey];
GO
PRINT N'Altering [dbo].[GetCallByCondition]...';


GO
ALTER FUNCTION [dbo].[GetCallByCondition]
(   @DialTypeId TINYINT,
	@ShiftTypeId INT,
    @SurveySid INT,
	@ExplicitSID INT,
	@ConditionValue INT,
	@Now DATETIME) 
RETURNS TABLE
AS RETURN(
		    SELECT TOP(1) BvSvySchedule.*
		    FROM [dbo].BvSvySchedule
		    WHERE 
				DialTypeId = @DialTypeId AND
			    ShiftTypeId = @ShiftTypeId AND
				CallState = 2 AND
				SurveySid = @SurveySid AND
				BvSvySchedule.ExplicitSID = @ExplicitSID AND
				BvSvySchedule.ConditionValue  = @ConditionValue AND
				BvSvySchedule.ConditionValue <> 0 AND 
				BvSvySchedule.TimeInShift < @Now
		ORDER BY Priority DESC, TimeInShift, ExplicitType DESC, CallOrder )
GO
PRINT N'Altering [dbo].[BvSpLookUpByPerson_ForCallGroup]...';


GO
ALTER PROCEDURE [dbo].[BvSpLookUpByPerson_ForCallGroup]
	@SurveyID INT,
	@CallGroupID INT,
	@PersonID INT,
	@Now DATETIME
AS
    IF @PersonID is null
    BEGIN
        SELECT 0 CallId, 0 SurveyId, 0 InterviewId, 0 ActiveDialId where 1 = 0
        RETURN 0
    END
	DECLARE @interviewId INT
	DECLARE @ConditionValue INT
    create table #output(CallId int, SurveyId int, InterviewId int, ActiveDialId int)
	;WITH conditions AS
	(
		SELECT p.ObjectSID as ExplicitSID, a.Id as ShiftTypeId, ConditionValue, ConditionPriority, RotatePriority, p.DialTypeId, a.ShiftPriority FROM BvLoginGroup p
		INNER JOIN BvActiveShiftTypeZone a on a.SurveyId = @SurveyId
		INNER JOIN BvSvyScheduleRuntimeStatistics s ON s.SurveyId = @SurveyId AND s.ShiftTypeID = a.Id AND s.ExplicitSID = p.ObjectSID
		INNER JOIN BvCallGroupConditionPerSurvey cgc ON cgc.SurveyId = @SurveyID AND cgc.CallGroupId = @CallGroupID 
		WHERE p.PersonSID = @personId 
	),
	calls as
	(
		SELECT TOP(1) cc.* FROM conditions c
		CROSS APPLY dbo.GetCallByCondition( c.DialTypeId, c.ShiftTypeID, @surveyId, c.ExplicitSID, c.ConditionValue, @Now ) cc
		ORDER BY Priority DESC, ConditionPriority DESC, RotatePriority ASC, c.ShiftPriority DESC, TimeInShift, ExplicitType DESC, CallOrder
	)
	UPDATE calls WITH(READPAST)
	SET CallState = -1,
		@interviewId = InterviewID,
		@surveyId = SurveySid,
		@ConditionValue = ConditionValue
	OUTPUT
	   deleted.ID,
	   deleted.SurveySID,
	   deleted.InterviewID,
	   deleted.ActiveDialId
	INTO #output(CallId, SurveyId, InterviewId, ActiveDialId)
	SELECT * FROM #output
	IF @@ROWCOUNT = 0 RETURN 0
	UPDATE BvCallGroupConditionPerSurvey 
		SET ConditionPriority = ConditionPriority 
		WHERE	SurveyId = @SurveyID AND
				CallGroupId = @CallGroupID AND 
				ConditionValue = @ConditionValue
	UPDATE BvAppointment 
	SET State = 2 
	WHERE State = 1 AND 
			SurveysId = @surveyId AND 
			InterviewSid = @interviewId
RETURN 0
GO
PRINT N'Altering [dbo].[BvSpLookUpByPerson]...';


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
	    ExpireTime = '9999-01-01 00:00:00.000',
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
PRINT N'Altering [dbo].[BvSpLookUpByPerson_ForAssignmentMode]...';


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
	    ExpireTime = '9999-01-01 00:00:00.000',
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
PRINT N'Altering [dbo].[BvSpLookUpByPerson_ForAssignmentModeClustered]...';


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
	    ExpireTime = '9999-01-01 00:00:00.000',
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
PRINT N'Altering [dbo].[BvSpLookUpByPerson_ForManualMode]...';


GO
ALTER PROCEDURE [dbo].[BvSpLookUpByPerson_ForManualMode]
	@surveyId int,
	@interviewId int,
	@personId int
AS
    IF @personId is null
    BEGIN
        SELECT 0 CallId, 0 SurveyId, 0 InterviewId, 0 ActiveDialId where 1 = 0
        RETURN 0
    END
	DECLARE @rowCount INT
	DECLARE @PersonAssignmentsListMode INT;
	SELECT @PersonAssignmentsListMode = AssignmentsListMode FROM BvPerson WHERE SID = @personId
	create table #output(CallId int, SurveyId int, InterviewId int, ActiveDialId int)
	;WITH call AS
	(
		SELECT c.*
		FROM BvSvySchedule c WITH(READPAST)
		INNER JOIN BvLoginGroup p ON p.PersonSID = @personId AND p.DialTypeId = c.DialTypeId
		WHERE CallState = 2 AND
		      c.SurveySid = @surveyId AND
		      InterviewId = @interviewId AND
			  (@PersonAssignmentsListMode = 1 OR p.ObjectSID = c.ExplicitSID)
	)
	UPDATE call
	SET CallState = -1
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
PRINT N'Altering [dbo].[BvSpSetNextInterviewForPerson]...';


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
			WHERE c.SurveySID = @SurveySid AND c.InterviewID = @InterviewId AND c.CallState = 2 AND s.State = 1
		)
		UPDATE call
		SET CallState = -1,
			ExpireTime = '9999-01-01 00:00:00.000'
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
			WHERE c.SurveySID = @SurveySid AND c.InterviewID = @InterviewId AND c.CallState = 2 AND s.State = 1
		)
		UPDATE call
		SET CallState = -1,
			ExpireTime = '9999-01-01 00:00:00.000'
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
PRINT N'Altering [dbo].[BvSpCall_Get]...';


GO
ALTER PROCEDURE [dbo].[BvSpCall_Get]
    @SurveyID int,
    @InterviewID int,
    @LockMode int, --TryLockNotLive = 1, TryLockAny = 2, NoLock = 0
    @GetLiveCall int = 0
AS
	DECLARE @OldCallState INT
	DECLARE @IsLockObtained INT = 0

	IF @LockMode > 0
	BEGIN
       
       UPDATE BvSvySchedule 
       SET	@OldCallState = CallState,
			CallState = -1
       WHERE SurveySID = @SurveyID AND 
             InterviewID = @InterviewID AND
             ( CallState > 0 OR @LockMode = 2 AND CallState NOT IN (0,-1) )
             
        SET @IsLockObtained = @@ROWCOUNT
             
		UPDATE BvAppointment
		SET STATE = 2
		WHERE SurveySID = @SurveyID AND
			  InterviewSID = @InterviewID AND
			  STATE = 1
    END

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
		BvSvySchedule.DialerId,
		BvSvySchedule.ActiveDialId
	FROM BvSvySchedule 
	LEFT JOIN BvShiftZones ON BvSvySchedule.ShiftTypeID = BvShiftZones.[ID]
	WHERE BvSvySchedule.SurveySID = @SurveyID AND 
		 BvSvySchedule.InterviewID = @InterviewID AND
		 ( ISNULL( @OldCallState, BvSvySchedule.CallState ) > 0 OR ( @GetLiveCall <> 0 AND ISNULL( @OldCallState, BvSvySchedule.CallState ) < 0 ) )
			 
RETURN @IsLockObtained
GO
PRINT N'Inserting new ITS into [BvConfirmitStatus]...';


GO

;WITH data( StateId, Name, Priority, StateGroupID, DA, FcdAction )
AS
(
    SELECT s.StateId, s.Name, s.Priority, sg.ID, s.DA, s.FcdAction FROM BvStateGroup sg CROSS JOIN 
    (
        SELECT 1012 as StateId, 'Canceled Transfer' as Name, 1 as Priority, 0 as DA, 0 as FcdAction
    ) as s
)
INSERT INTO [dbo].[BvState] (StateID, Name, Priority, StateGroupID, DA, FcdAction) SELECT StateId, Name, Priority, StateGroupID, DA, FcdAction FROM data

INSERT INTO BvThresholdITS ( SurveySID, ITS ) VALUES(0, 1012)
INSERT INTO [BvConfirmitStatus] VALUES( '1012', 'Canceled Transfer', 1012 )

GO
PRINT N'Update complete.';


GO
