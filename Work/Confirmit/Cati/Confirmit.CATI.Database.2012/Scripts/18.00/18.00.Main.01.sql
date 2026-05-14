PRINT N'Creating [dbo].[BvTrBvPersonRel_Delete]...';


GO
CREATE TRIGGER [BvTrBvPersonRel_Delete] ON [dbo].[BvPersonRel] 
AFTER DELETE
AS
BEGIN
	SET NOCOUNT ON
	
	DELETE FROM BvLoginGroup
	FROM deleted 
	WHERE BvLoginGroup.PersonSID = deleted.PersonSID AND BvLoginGroup.ObjectSID = deleted.ObjectSID 
	
END
GO
PRINT N'Creating [dbo].[BvTrBvPersonRel_Insert]...';


GO
CREATE TRIGGER [BvTrBvPersonRel_Insert] ON [dbo].[BvPersonRel] 
AFTER INSERT
AS
BEGIN
	SET NOCOUNT ON
	
	INSERT INTO BvLoginGroup(PersonSID, ObjectSID, SurveySID ) 
	SELECT i.PersonSID, i.ObjectSID, CASE WHEN p.ManualSelection = 2 /*is survey selection*/ THEN t.SurveySID ELSE 0 END  FROM inserted i
	INNER JOIN BvTasks t ON i.PersonSID = t.PersonSID
	INNER JOIN BvPerson p ON i.PersonSID = p.SID
	
END
GO
PRINT N'Altering [dbo].[BvSpAssignment_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpAssignment_Insert]
@SID INT, 
@SurveySID INT, 
@InterviewSID INT, 
@PersonSID INT, 
@RoleID INT, 
@FromCall INT=0,
@CallCenterID INT
AS
SET NOCOUNT ON
DECLARE @InsertedAssignmentsCount INTEGER = 0

IF @InterviewSID > 0 OR @FromCall > 0 
BEGIN

            UPDATE BvSvySchedule SET
                ExplicitSID = @PersonSID, 
                ExplicitType = 2, --Person type
                Priority = CASE WHEN OldPriority > 0 THEN OldPriority ELSE Priority END,
                OldPriority = 0
            WHERE SurveySID = @SurveySID AND 
                  InterviewID = @InterviewSID AND
                  CallState > 0

            exec BvSpAddUniqueAssignment @PersonSID
END
ELSE
BEGIN
        
    IF NOT EXISTS ( SELECT * FROM BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID)
        WHERE PersonOrGroupId = @PersonSID AND SurveyId = @SurveySID)
          INSERT INTO BvPersonOrGroupAssignmentOnSurvey( PersonOrGroupId, SurveyId, CallCenterID )
              VALUES( @PersonSID, @SurveySID, @CallCenterID )
              
    SET @InsertedAssignmentsCount = @@ROWCOUNT          
   
   IF EXISTS ( SELECT SID FROM BvPerson WHERE SID = @PersonSID )
   BEGIN
	   INSERT INTO BvPersonRel( PersonSID, ObjectSID, RoleID, Type )
	   VALUES(@PersonSID, @SurveySID, 2, 2)
   END
   ELSE
   BEGIN
       INSERT INTO BvPersonRel( PersonSID, ObjectSID, RoleID, Type )
       SELECT r.PersonSid, @SurveySID, 2, 2
       FROM BVPersonRel r
	   LEFT JOIN BvPerson p 
		ON r.PersonSID = p.SID
       WHERE @PersonSID = r.ObjectSID AND
             ObjectSID != r.PersonSid AND
			 ( p.CallCenterID = @CallCenterID OR p.SID IS NULL )
   END
END

RETURN @InsertedAssignmentsCount
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

	create table #output(CallID int,
						 SurveySID int,
						 iid int)
    
    ;WITH calls AS
	(
		SELECT TOP(1) c.*
		FROM BvSvySchedule c
		INNER JOIN BvLoginGroup p ON p.PersonSID = @personId
		INNER JOIN BvSurvey on SID = c.SurveySid AND DialMode !=  4 AND State =1
		WHERE CallState = 2 AND
		      p.ObjectSID = c.ExplicitSID AND
			  TimeInShift <= @SuitableTimeForCalls AND
			  IsInActiveShiftType = 1
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
PRINT N'Altering [dbo].[BvSpLookUpByPerson_ForAssignmentMode]...';


GO
ALTER PROCEDURE [dbo].[BvSpLookUpByPerson_ForAssignmentMode]
	@surveyId INT,
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

	create table #output(CallID int,
						 SurveySID int,
						 iid int)
    
    ;WITH calls AS
	(
		SELECT TOP(1) c.*
		FROM BvSvySchedule c
		INNER JOIN BvLoginGroup p ON p.PersonSID = @personId
		WHERE CallState = 2 AND
			  TimeInShift <= @SuitableTimeForCalls AND
			  IsInActiveShiftType = 1 AND
		      c.SurveySid = @surveyId AND
			  p.ObjectSID = c.ExplicitSID
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
	   deleted.InterviewID
	INTO #output

	SET @rowCount = @@ROWCOUNT

	select * from #output
	
	IF(@rowCount = 0) RETURN 0
	
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
    DECLARE @Call TABLE
	(
		CallID INT,
		ApptID INT,
		SurveySID INT,
		iid INT,
		CallState INT,
		ShiftID INT,
		Priority INT,
		TimeInShift DATETIME,
		TimeToExpire DATETIME,
		Resource INT,
		Resource_Type INT,
		RuleNumber UNIQUEIDENTIFIER,
		roleid INT	
	);

	DECLARE @PersonAssignmentsListMode INT;
	SELECT @PersonAssignmentsListMode = AssignmentsListMode FROM BvPerson WHERE SID = @personId

	;WITH call AS
	(
		SELECT c.*
		FROM BvSvySchedule c
		INNER JOIN BvLoginGroup p ON p.PersonSID = @personId
		WHERE CallState = 2 AND
		      c.SurveySid = @surveyId AND
		      InterviewId = @interviewId AND
			  (@PersonAssignmentsListMode = 1 OR p.ObjectSID = c.ExplicitSID)
	)
	UPDATE call
	SET CallState = -1
	OUTPUT
		   deleted.[ID] CallID,
		   deleted.ApptID,
		   deleted.SurveySID,
		   deleted.InterviewID iid,
		   deleted.CallState,
		   deleted.ShiftTypeID ShiftID,
		   deleted.Priority,
		   deleted.TimeInShift,
		   deleted.ExpireTime TimeToExpire,
		   deleted.ExplicitSID Resource,
		   deleted.ExplicitType Resource_Type,
		   deleted.RuleNumber,
		   2 roleid	
	INTO @Call
	
	UPDATE BvAppointment 
	SET State = 2 
	WHERE State = 1 AND 
	      SurveysId = @surveyId AND 
	      InterviewSid = @interviewId
	      
	SELECT * FROM @Call
RETURN 0
GO
PRINT N'Altering [dbo].[BvSpPerson_SpinUp]...';


GO
ALTER  PROCEDURE [dbo].[BvSpPerson_SpinUp]
    @PersonSID INT
AS
	--if person is not found then we use 0 call center id, because person group is global.
	DECLARE @CallCenterID TINYINT = ISNULL( (SELECT CallCenterID FROM BvPerson WHERE SID = @PersonSID ), 0 )
    
	declare @temp table
    (
        sid int not null,
        role_id int not null,
        type int not null
    )

    insert into @temp
        select distinct m.ContainerSID, g.RoleID, 1
        from BvMemberShip m
        inner join BvPersonGroup g on g.SID = m.ContainerSID
        where m.ObjectSID = @PersonSID

    insert into @temp values ( @PersonSID, 0, 1 )

    insert into @temp
        select distinct a.SurveyId, 2, 2 from BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID) a
		inner join @temp temp
		ON a.PersonOrGroupId = temp.sid
        where a.CallCenterID = @CallCenterID
    
    delete from BvPersonRel where PersonSID = @PersonSID
    insert into BvPersonRel( PersonSID, ObjectSID, RoleID, Type )
        select @PersonSID, sid, role_id, type from @temp

RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpSurvey_Clean]...';


GO
ALTER PROCEDURE [dbo].[BvSpSurvey_Clean]
    @SurveyId INT
AS
    DECLARE @CountOfDeletedAssignment INT
    DECLARE @CountOfDeletedCalls INT

    DELETE BvPersonOrGroupAssignmentOnSurvey 
    WHERE SurveyId = @SurveyId

    SET @CountOfDeletedAssignment = @@ROWCOUNT
    
    DELETE FROM bvpersonrel
    WHERE type = 2 AND objectsid = @SurveyId
    
    DELETE FROM bvlogingroup WHERE surveysid = @surveyID
    

    DELETE FROM BvSvySchedule WHERE SurveySid = @SurveyId
    SET @CountOfDeletedCalls = @@ROWCOUNT

    SELECT @CountOfDeletedAssignment as CountOfDeletedAssignment, @CountOfDeletedCalls as CountOfDeletedCalls
GO
PRINT N'Altering [dbo].[BvSpSurvey_DeassignFromCallCenter]...';


GO
ALTER  PROCEDURE [dbo].[BvSpSurvey_DeassignFromCallCenter]
        @SurveyId INT,
        @CallCenterId INT
AS
SET NOCOUNT ON

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

RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpSurvey_Delete]...';


GO
ALTER PROCEDURE [dbo].[BvSpSurvey_Delete]
        @surveyID int
AS
    DECLARE @State INTEGER

	IF EXISTS( SELECT 1 FROM BvTasks WHERE SurveySID = @surveyID )
	BEGIN
		DECLARE @Name NVARCHAR(MAX) 
		SELECT @Name = name FROM BvSurvey WHERE SID = @surveyID
		RAISERROR( 'Survey with name = ''%s'' can''t be deleted, because active users exist for it survey', 16, 1, @name )
		RETURN -1
	END

    DELETE FROM BvThresholdITS WHERE SurveySID = @surveyID

    DELETE FROM BvMembership WITH(ROWLOCK)
    WHERE ObjectSID = @surveyID
    
    DELETE BvAppointment 
    WHERE SurveySID = @surveyID
    
    DELETE FROM BvSvySchedule 
    WHERE SurveySID = @surveyID

    DELETE BvPersonOrGroupAssignmentOnSurvey WHERE SurveyId = @surveyID 

    DELETE BvInterview WHERE SurveySID = @surveyID
    
    EXEC BvSpMembership_Delete 0, @surveyID
    
    --delete specific survey schedule params
    DELETE FROM BvScheduleParam WHERE SurveySID = @surveyID

    EXEC BvSpBvID_Delete 2, @surveyID

    DELETE  BvSurvey WHERE SID = @surveyID
    DELETE FROM BvSampleStatusSummary WHERE SurveySID = @surveyID
    
    DECLARE @FilterSID INTEGER
    SELECT @FilterSID = SID FROM BvFilters WHERE [Name] = CAST( @surveyID AS NVARCHAR(255) )
    IF @FilterSID IS NOT NULL
    BEGIN
        DELETE FROM BvFilterFields WHERE FilterSID = @FilterSID
        DELETE FROM BvFilters WHERE SID = @FilterSID
    END
    
    DELETE FROM BvFilterFields
    FROM BvFilterFields
    INNER JOIN BvFilters ON ( SID = FilterSid )
    WHERE SurveySID = @surveyID

    DELETE FROM BvFilters WHERE SurveySID = @surveyID
    
    delete from bvpersonrel where type = 2 and objectsid = @surveyID
    
    delete from bvlogingroup where surveysid = @surveyID

RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpAssignment_Delete]...';


GO
ALTER PROCEDURE [dbo].[BvSpAssignment_Delete]
@SurveySID INT, 
@Count INT, 
@PersonSID INT, 
@RoleID INT,
@CallCenterID INT
AS
SET NOCOUNT ON

DECLARE @InsertedAssignmentsCount INTEGER = 0

 IF @Count > 0 
 BEGIN

    UPDATE BvSvySchedule SET ExplicitSID = @SurveySID, ExplicitType = 1
    WHERE ExplicitSID = @PersonSID AND
          SurveySID = @SurveySID AND
          CallState = 2 AND
          @RoleID = 2
    
    RETURN @InsertedAssignmentsCount
 END
 ELSE
 BEGIN
    DELETE FROM BvPersonOrGroupAssignmentOnSurvey
      WHERE PersonOrGroupId = @PersonSID AND SurveyId = @SurveySID AND CallCenterID = @CallCenterID
    SET @InsertedAssignmentsCount = @@ROWCOUNT
 END

-- recalculate login cache
IF EXISTS ( SELECT SID FROM BvPerson WHERE SID = @PersonSID )
   EXEC BvSpPerson_SpinUp @PersonSID
ELSE
BEGIN
   DELETE BvPersonRel
   FROM BvPersonRel base
   WHERE ObjectSid = @SurveySID AND    --look at assignments to survey only
         Type = 2 AND                          
         PersonSid IN (SELECT PersonSid        --look at all persons inside current group
                       FROM BvPersonRel pr
                       WHERE Type = 1 AND
                             ObjectSid = @PersonSID) AND
         NOT EXISTS (SELECT *                  --if person doesn't assign directly to survey
                     FROM BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID)
                     WHERE PersonOrGroupId = base.PersonSid AND
                           SurveyId = @SurveySID) AND
         NOT EXISTS (SELECT *                  --if person doesn't belong to others groups assigned to survey
                     FROM BvMemberShip
                     INNER JOIN BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID) ON PersonOrGroupId = ContainerSid AND
                                                                     SurveyId = @SurveySID
                     WHERE ObjectSid = base.PersonSid)
END

RETURN @InsertedAssignmentsCount
GO
PRINT N'Refreshing [dbo].[BvSpCallCenter_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpCallCenter_Delete';


GO
PRINT N'Refreshing [dbo].[BvSpPersonGroup_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpPersonGroup_Insert';


GO
PRINT N'Update complete.';


GO
