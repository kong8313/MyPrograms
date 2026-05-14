CREATE PROCEDURE [dbo].[BvSpLookUpByPerson_ForAssignmentModeClustered]
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