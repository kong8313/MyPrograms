GO
PRINT N'Altering [dbo].[BvSpGetOpenedSurveys]...';


GO
ALTER  PROCEDURE [dbo].[BvSpGetOpenedSurveys]
   @PersonSID INT
AS
SET NOCOUNT ON
    DECLARE @CallCenterId INT = (SELECT CallCenterID FROM BvPerson WHERE SID = @PersonSID )
    declare @utcnow datetime = getutcdate()
    SELECT com.SID, com.Name
    FROM (
         SELECT s.SID, s.[Name]
         FROM BvSurvey s
		 INNER JOIN BvPersonRel l on l.PersonSid = @PersonSID AND
		                              l.ObjectSID = s.SID
         WHERE s.State = 1
 
         UNION

         SELECT s.SID, s.[Name]
         FROM BvSurvey s
		 INNER JOIN BvSurveyAssignmentOnCallCenter saocc
         ON s.SID = saocc.SurveyId AND saocc.CallCenterId = @CallCenterId
		 WHERE s.State = 1 AND
		 EXISTS ( SELECT 1
			      FROM BvPersonRel l
			      INNER JOIN BvActiveShiftTypeZone a on a.SurveyId = s.SID
			      CROSS APPLY dbo.GetPriorityCallByExplicitSidAndShiftTypeId(l.ObjectSID, a.Id, a.SurveyId, @utcnow, 1)
			      WHERE l.PersonSID = @PersonSID
		 )) com
      ORDER BY com.Name

RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpLookUpByPerson]...';


GO
ALTER PROCEDURE [dbo].[BvSpLookUpByPerson]
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
    DECLARE @rowCount INT
    DECLARE @surveyId INT
	DECLARE @CallCenterId INT = (SELECT CallCenterID FROM BvPerson WHERE SID = @personId )

	create table #output(CallID int,
						 SurveySID int,
						 iid int)

	create table #surveySids(id int, objectSid int)

	insert into #surveySids
	select distinct s.SID, l.ObjectSid
	FROM [BvFnSurvey_GetByCallCenterId](@CallCenterId) s
	CROSS JOIN BvLoginGroup l
	WHERE s.DialMode !=  4 AND State =1 AND l.PersonSid = @personId AND EXISTS
	      (select * from bvsvyschedule c
		   where c.SurveySID = s.SID and c.ExplicitSID = l.ObjectSID)
    
    ;WITH calls AS
	(
	    SELECT TOP(1) c.*
		FROM #surveySids s
		INNER JOIN BvActiveShiftTypeZone a on a.Surveyid = s.Id
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](s.ObjectSID, a.Id, s.Id, @SuitableTimeForCalls, 1) c
		ORDER BY Priority DESC,
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
	   deleted.[ID] CallID,
	   deleted.SurveySID,
	   deleted.InterviewID iid
	INTO #output
	
	SET @rowCount = @@ROWCOUNT

	SELECT * FROM #output
	
	IF(@rowCount = 0) RETURN 0
	
	UPDATE BvAppointment 
	SET State = 2 
	WHERE State = 1 AND 
	      SurveysId = @surveyId AND 
	      InterviewSid = @interviewId
RETURN 0
GO
PRINT N'Altering [dbo].[BvSpSurvey_DeassignFromCallCenter]...';


GO
ALTER  PROCEDURE [dbo].[BvSpSurvey_DeassignFromCallCenter]
        @SurveyId INT,
        @CallCenterId INT
AS
SET NOCOUNT ON

	SELECT SID INTO #DeasiggnedExplicitSIDs FROM BvPerson WHERE CallCenterID = @CallCenterId

	DELETE FROM BvSurveyAssignmentOnCallCenter 
		WHERE SurveyId = @SurveyId AND CallCenterId = @CallCenterId

	IF @@ROWCOUNT = 0 
	BEGIN
		RETURN (0)
	END

	DELETE FROM BvPersonOrGroupAssignmentOnSurvey
		WHERE SurveyId = @SurveyId AND CallCenterID = @CallCenterId

	DELETE BvPersonRel 
		WHERE ObjectSID = @SurveyId AND Type = 2 AND PersonSID IN ( SELECT SID FROM BvPerson WHERE CallCenterID = @CallCenterId )

	DELETE BvLoginGroup 
		WHERE SurveySID = @SurveyId AND PersonSID IN ( SELECT SID FROM BvPerson WHERE CallCenterID = @CallCenterId )

	UPDATE BvSvySchedule 
		SET ExplicitSID = @SurveyId
		FROM #DeasiggnedExplicitSIDs d
		WHERE SurveySID = @SurveyId AND ExplicitSID = d.SID

RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpSurvey_GetCountOfLoggedPerson]...';


GO
CREATE  PROCEDURE [dbo].[BvSpSurvey_GetCountOfLoggedPerson]
        @SurveyId INT,
        @CallCenterId INT,
		@TaskChoiceMode INT
AS
SET NOCOUNT ON

	SELECT COUNT(*) FROM BvTasks t
		INNER JOIN BvPerson p
		ON t.PersonSID = p.SID
		WHERE t.SurveySID = @SurveyId AND t.CallCenterID = @CallCenterId AND p.ManualSelection = @TaskChoiceMode
RETURN (0)
GO
PRINT N'Update complete.';


GO
