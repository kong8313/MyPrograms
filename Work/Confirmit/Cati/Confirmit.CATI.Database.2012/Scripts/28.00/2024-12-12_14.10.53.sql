GO
PRINT N'Creating Index [dbo].[BvTasks].[IX_BvTasks_SurveySID_InterviewId]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvTasks_SurveySID_InterviewId]
    ON [dbo].[BvTasks]([SurveySID] ASC, [InterviewID] ASC);


GO
PRINT N'Altering Procedure [dbo].[BvSpInterviewsAndAppointments_Delete_Batch]...';


GO
ALTER PROCEDURE [dbo].[BvSpInterviewsAndAppointments_Delete_Batch]
@surveySid INT, @batchId INT
AS
-- Delete appointments

 DELETE BvAppointment
 FROM BvTransferArrays
 WHERE SurveySID = @SurveySID AND
       BvTransferArrays.BatchId = @batchId AND
       ItemId = BvAppointment.InterviewSID

-- Delete interviews
 DELETE BvInterview 
 FROM BvTransferArrays
 WHERE SurveySID = @surveySid AND
       BvTransferArrays.BatchId = @batchId AND
       ID = ItemID
GO
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

	create table #surveySids(id int, objectSid int, dialType tinyint, shiftTypeId int, shiftPriority int)

	insert into #surveySids
	select distinct s.SID, l.ObjectSid, l.DialTypeId, a.Id, a.ShiftPriority
	FROM [BvFnSurvey_GetByCallCenterId](@CallCenterId) s
	INNER JOIN BvActiveShiftTypeZone a on a.Surveyid = s.SID
	CROSS JOIN BvLoginGroup l
	WHERE s.DialMode !=  4 AND State =1 AND l.PersonSid = @personId AND EXISTS
	      (select * from bvsvyschedule c
		   where c.SurveySID = s.SID and c.ShiftTypeID = a.Id and c.ExplicitSID = l.ObjectSID and c.DialTypeId = l.DialTypeId )
    
    ;WITH calls AS
	(
	    SELECT TOP(1) c.*
		FROM #surveySids s
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](s.dialType, s.ObjectSID, s.shiftTypeId, s.Id, @SuitableTimeForCalls, 1) c
		ORDER BY Priority DESC,
				 s.shiftPriority DESC,
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
PRINT N'Update complete.';


GO
