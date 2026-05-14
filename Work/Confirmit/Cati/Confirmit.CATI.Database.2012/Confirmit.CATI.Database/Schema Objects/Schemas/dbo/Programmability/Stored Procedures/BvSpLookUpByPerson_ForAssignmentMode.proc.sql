CREATE PROCEDURE [dbo].[BvSpLookUpByPerson_ForAssignmentMode]
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