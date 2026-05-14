CREATE PROCEDURE [dbo].[BvSpLookUpByPerson_ForCallGroup]
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
