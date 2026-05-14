PRINT N'Altering [dbo].[BvSpHistory_GetLinkedInterviews]...';


GO
ALTER PROCEDURE [BvSpHistory_GetLinkedInterviews]
	@LinkedInterviewSessionId INT
AS

	SELECT 
	    ROW_NUMBER()  OVER(ORDER BY h.ID) AS InterviewsOrder,
		h.SurveyId	AS SurveyId,
		s.Name		AS ProjectId, 
		s.[Description] AS SurveyName,
		h.InterviewId	AS InterviewId,
		@LinkedInterviewSessionId AS LinkedInterviewSessionId
	FROM BvHistory h
	JOIN BvSurvey s
		ON h.SurveyId = s.SID
	WHERE LinkedInterviewSessionId = @LinkedInterviewSessionId
	ORDER BY h.Id
	
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
	begin
		select 0 CallID,
		       0 SurveySID,
			   0 iid
		where 1 = 0
		return 0
	end

    DECLARE @rowCount INT
	DECLARE @CallCenterId INT = (SELECT CallCenterID FROM BvPerson WHERE SID = @personId )

	create table #output(CallID int,
						 SurveySID int,
						 iid int)

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
		   deleted.[ID] CallID,
		   deleted.SurveySID,
		   deleted.InterviewID iid
		INTO #output
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
		   deleted.[ID] CallID,
		   deleted.SurveySID,
		   deleted.InterviewID iid
		INTO #output
	END
	
	SET @rowCount = @@ROWCOUNT

	SELECT * FROM #output
	
	IF(@rowCount = 0) RETURN 0
	
	UPDATE BvAppointment 
	SET State = 2 
	WHERE State = 1 AND 
	      SurveysId = @SurveySId AND 
	      InterviewSid = @InterviewId
RETURN 0
GO
PRINT N'Update complete.';


GO
