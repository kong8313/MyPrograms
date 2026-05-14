CREATE PROCEDURE [dbo].[BvSpLookUpByPerson_ForManualMode]
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