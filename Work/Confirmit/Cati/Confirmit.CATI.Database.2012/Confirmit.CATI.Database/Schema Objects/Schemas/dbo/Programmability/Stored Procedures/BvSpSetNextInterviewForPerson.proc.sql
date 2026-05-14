CREATE PROCEDURE [BvSpSetNextInterviewForPerson]
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