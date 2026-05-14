PRINT N'Adding TimeZoneBalancing.EndOfShiftThreshold...';
GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
     SELECT 'TimeZoneBalancing.EndOfShiftThreshold', 'Time zones balancing end of shift threshold', 'Time zones balancing', 'Time at the end of a shift which controls call delivery algorithm to favour calls for a timezone in which a shift is about to be finshed', 1, 0, '0'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END
GO

PRINT N'Altering [dbo].[BvActiveShiftTypeZone]...';


GO
ALTER TABLE [dbo].[BvActiveShiftTypeZone]
    ADD [ShiftPriority] TINYINT CONSTRAINT [DF_BvActiveShiftTypeZone_ShiftPriority] DEFAULT (0) NOT NULL;


GO
PRINT N'Altering [dbo].[GetCallsForGroupForPredictiveSurvey]...';


GO
ALTER FUNCTION dbo.GetCallsForGroupForPredictiveSurvey
(
    @DialTypeId TINYINT,
    @rowCount AS INT,
    @SurveySid AS INT,
    @ObjectSid AS INT,
	@SuitableTimeForCalls DATETIME
)
RETURNS TABLE
AS RETURN(
          SELECT TOP (@rowCount) c.*, a.ShiftPriority
          FROM BvActiveShiftTypeZone a
		  CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](@DialTypeId, @ObjectSid, a.Id, @SurveySID, @SuitableTimeForCalls, @rowCount) c
		  WHERE a.surveyid = @SurveySid
          ORDER BY priority DESC, a.ShiftPriority DESC, TimeInShift, ExplicitType DESC, CallOrder )
GO
PRINT N'Altering [dbo].[GetCallsPerGroup]...';


GO
ALTER FUNCTION [dbo].[GetCallsPerGroup]
(
	@DialType as TINYINT,
    @rowCount AS INT,
    @ExplicitSID AS INT,
    @SurveySid AS INT,
    @TimeToRun AS DATETIME)
RETURNS TABLE
AS RETURN(
          SELECT TOP(@rowCount) 
				c.[ID],
                ExplicitSID,
				ExplicitType,
                SurveySID,
                InterviewID,
                CallState,
				ApptId,
				TimeInShift,
				CallOrder,
				Priority,
				ShiftTypeID, 
				ExpireTime,
				a.ShiftPriority					
	FROM BvActiveShiftTypeZone a 
	CROSS JOIN
		(SELECT cc.CellId AS CellId from  BvClusteredQuotaCell cc
			WHERE  cc.SurveyId = @SurveySid
		 UNION 
		 SELECT 0 AS CellId
		 ) cells
	CROSS APPLY dbo.[GetTopCallsForShiftTypeGroupCell](@DialType, @rowCount, a.Id, @ExplicitSID, @SurveySid, cells.CellId, @TimeToRun) c
	WHERE a.SurveyId = @SurveySid
    ORDER BY Priority DESC,
				   a.ShiftPriority DESC,
                   TimeInShift,
                   ExplicitType DESC,
                   CallOrder )

GO
PRINT N'Refreshing [dbo].[GetHighPriorityCalls]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[GetHighPriorityCalls]';

PRINT N'Altering [dbo].[GetHighPriorityCalls]...';


GO


ALTER FUNCTION [dbo].[GetHighPriorityCalls]
(
	  @SurveySid AS INT,
	  @SuitableTimeForCalls DATETIME,
	  @maxCallsPerGroup AS INT
)
RETURNS TABLE
AS RETURN(

	WITH LoggedInGroups AS 
	(
		SELECT SUM(cnt) as cnt, SID, DialTypeId
		FROM vLogins WITH ( noexpand, INDEX([pk_vLogins]) )
		WHERE  SurveySID = @SurveySid OR SurveySID = 0
		GROUP BY SID, DialTypeId
	)
	SELECT TOP 500			--we need this to have ORDER BY plus we do not need more calls because we do paging
				[ID],
                ExplicitSID,
				ExplicitType,
                SurveySID,
                InterviewID,
                CallState,
				ApptId,
				TimeInShift,
				CallOrder,
				Priority,
				ShiftTypeID, 
				ExpireTime				
	FROM LoggedInGroups c
	CROSS APPLY dbo.GetCallsPerGroup(c.DialTypeId, c.cnt*@maxCallsPerGroup, c.sid, @SurveySID, @SuitableTimeForCalls) cpg
	ORDER BY Priority DESC,
				   ShiftPriority DESC,
                   TimeInShift,
                   ExplicitType DESC,
                   CallOrder
)


GO
PRINT N'Altering [dbo].[BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnly]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnly]
 @SurveySID INT,
 @Count  INT, --number of requested calls
 @SuitableTimeForCalls DATETIME
AS
--best if it should be established at the connection level
--it may influence on count of recompilations
SET NOCOUNT ON

	DECLARE @FixeNumberCallsPerPerson INT = 10

      --it should be at end of the SP. it uses for decrease recompilation
	DECLARE @CachedCalls TABLE (
	  [ExplicitSID] [int] NOT NULL,
	  [ID] [int] NOT NULL,
	  [InterviewID] [int] NOT NULL,
	  [TimeInShift] [datetime] NULL,
	  [Priority] [INT] NOT NULL,
	  [CallOrder] [INT] NOT NULL,
	  [ApptID] [int] not null,
	  [ExpireTime] [datetime],
	  [ShiftPriority] TINYINT NOT NULL )
        
	;WITH orderedUpdateTable AS
	(
		SELECT TOP(@Count) c.*, a.ShiftPriority
        FROM BvActiveShiftTypeZone a
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](0, @SurveySID, a.Id, @SurveySID, @SuitableTimeForCalls, @Count) c
		WHERE a.Surveyid = @SurveySID
		ORDER BY Priority DESC,
			     a.ShiftPriority DESC,
                 TimeInShift,
				 ExplicitType DESC,
				 CallOrder
	)
    UPDATE orderedUpdateTable
    SET CallState = -2 
	OUTPUT 0,
		   inserted.[ID],
		   inserted.[interviewID],
		   inserted.[TimeInShift],
		   inserted.[Priority],
		   inserted.[CallOrder],
		   inserted.[ApptID],
		   inserted.[ExpireTime],
		   deleted.[ShiftPriority]
	INTO @CachedCalls
		
	INSERT INTO bvCallsSentToDialer
	SELECT @SuitableTimeForCalls AS [Time], 
		   @SurveySID AS SurveySID, 
		   ExplicitSID AS ExplicitSid, 
		   Count(*) AS CallsCount
	 FROM @CachedCalls GROUP BY ExplicitSID

    SELECT c.[ID], 
           c.[ExplicitSid], 
           @SurveySID SurveySid, 
           i.DialingMode DiallingMode,
		   c.[InterviewID], 
		   i.[TelephoneNumber],
		   i.[ExtensionNumber],
		   (CASE WHEN c.ApptId > 0 THEN c.[TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
		   0 as [GroupID],
		   c.ExpireTime
    FROM @CachedCalls c 
    INNER JOIN BvInterview i ON c.[Interviewid] = i.[ID] AND
                                i.[SurveySID] = @SurveySID
    ORDER BY Priority DESC,
		     c.ShiftPriority DESC,
             TimeInShift,
 			 CallOrder
 
RETURN (@@ROWCOUNT)
GO
PRINT N'Altering [dbo].[BvSpGetCachedCallsForPredictiveSurveyByPersonGroup]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetCachedCallsForPredictiveSurveyByPersonGroup]
 @SurveySID INT,
 @GroupID INT,	
 @Count  INT, --number of requested calls
 @SuitableTimeForCalls DATETIME
AS
--best if it should be established at the connection level
--it may influence on count of recompilations
SET NOCOUNT ON

	DECLARE @FixeNumberCallsPerPerson INT = 10

      --it should be at end of the SP. it uses for decrease recompilation
	DECLARE @CachedCalls TABLE (
	  [ExplicitSID] [int] NOT NULL,
	  [ID] [int] NOT NULL,
	  [InterviewID] [int] NOT NULL,
	  [TimeInShift] [datetime] NULL,
	  [Priority] [INT] NOT NULL,
	  [CallOrder] [INT] NOT NULL,
	  [ApptID] [int] not null,
	  [ExpireTime] [datetime],
	  [ShiftPriority] TINYINT NOT NULL )
        

	;WITH orderedUpdateTable AS
	(
		SELECT TOP(@Count) c.*, a.ShiftPriority
        FROM BvActiveShiftTypeZone a
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](0, @groupID, a.Id, @SurveySID, @SuitableTimeForCalls, @Count) c
		WHERE a.Surveyid = @SurveySID
		ORDER BY Priority DESC,
				 a.ShiftPriority DESC,
                 TimeInShift,
				 ExplicitType DESC,
				 CallOrder
	)
    UPDATE orderedUpdateTable 
    SET CallState = -2 
	OUTPUT 0,
		   inserted.[ID],
		   inserted.[interviewID],
		   inserted.[TimeInShift],
		   inserted.[Priority],
		   inserted.[CallOrder],
		   inserted.[ApptID],
		   inserted.[ExpireTime],
		   deleted.[ShiftPriority]
	INTO @CachedCalls

	INSERT INTO bvCallsSentToDialer
	SELECT @SuitableTimeForCalls AS [Time], 
		   @SurveySID AS SurveySID, 
		   ExplicitSID AS ExplicitSid, 
		   Count(*) AS CallsCount
	 FROM @CachedCalls GROUP BY ExplicitSID

    SELECT c.[ID], 
           c.[ExplicitSid], 
           @SurveySID SurveySid, 
           i.DialingMode DiallingMode,
		   c.[InterviewID], 
		   i.[TelephoneNumber],
		   i.[ExtensionNumber],
		   (CASE WHEN c.ApptId > 0 THEN c.[TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
		   @GroupID as [GroupID],
		   c.ExpireTime
    FROM @CachedCalls c 
    INNER JOIN BvInterview i ON c.[Interviewid] = i.[ID] AND
                                i.[SurveySID] = @SurveySID
    ORDER BY Priority DESC,
			 c.ShiftPriority DESC,
             TimeInShift,
			 CallOrder
 
RETURN (@@ROWCOUNT)
GO
PRINT N'Altering [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]
    @SurveySID INT,
    @DialerId INT = 1,
	@Count  INT,  --number of requested calls
	@SuitableTimeForCalls DATETIME
AS

	DECLARE @Groups TABLE(
		[ObjectSid] [int] NOT NULL,
		[GroupSize] [int] NOT NULL)
		
    DECLARE @MinDistributedCalls INT = 5


	
	IF ( (SELECT COUNT(*) FROM BvDialers ) > 1 )
	BEGIN
		;WITH Logins AS (
			select lg.ObjectSID as sid, lg.SurveySID, count(*) as cnt
				from dbo.BvLoginGroup lg
				INNER JOIN BvTasks t 
					ON lg.PersonSID = t.PersonSID
				WHERE t.DialerId = @DialerId AND lg.DialTypeId = 0
				group by lg.ObjectSID, lg.SurveySID
		)
		INSERT INTO @Groups
		SELECT c.sid, count(*)
		FROM Logins c 
		INNER JOIN BvActiveShiftTypeZone a on a.SurveyId = @SurveySid
		CROSS APPLY dbo.GetCallsForPredictiveMode(0, c.cnt*20, a.Id, c.sid, @SurveySID, @SuitableTimeForCalls)
		where c.SurveySID = @SurveySID
		group by c.sid
	END
	ELSE
	BEGIN
		INSERT INTO @Groups
		SELECT c.sid, count(*)
		FROM vLogins c with ( noexpand, INDEX([pk_vLogins]) )
		INNER JOIN BvActiveShiftTypeZone a on a.SurveyId = @SurveySid
		CROSS APPLY dbo.GetCallsForPredictiveMode(0, c.cnt*20, a.Id, c.sid, @SurveySID, @SuitableTimeForCalls)
		where c.SurveySID = @SurveySID AND c.DialTypeId = 0
		group by c.sid
	END
    
    DECLARE @totalCount INT 
    SELECT @totalCount = SUM(GroupSize) FROM @Groups
    DECLARE @part FLOAT = CAST(@Count AS FLOAT)/CAST(@totalCount AS FLOAT)
    DECLARE @current INT
    DECLARE @currentMinValue INT
    
    UPDATE @Groups
    SET @current = GroupSize*@part+0.5,
        @currentMinValue = CASE WHEN GroupSize < @MinDistributedCalls THEN GroupSize ELSE @MinDistributedCalls END,
        @current = CASE WHEN @current < @MinDistributedCalls THEN @currentMinValue ELSE @current END,
        GroupSize = @current
        
    DECLARE @usedCalls TABLE(
        [ObjectSid] [int] NOT NULL,
        [ID] [int] NOT NULL, 
        [Interview] [int] NOT NULL,
        [TimeInShift] [datetime] NOT NULL,
		[Priority] [INT] NOT NULL,
	    [CallOrder] [INT] NOT NULL,
		[ApptID] [INT],
	    [ExpireTime] [datetime],
		[ShiftPriority] TINYINT NOT NULL )
        
	;WITH orderedUpdateTable as
	(    
		SELECT calls.*
		FROM @Groups g
		CROSS APPLY dbo.GetCallsForGroupForPredictiveSurvey(0, g.GroupSize, @SurveySID, g.ObjectSid, @SuitableTimeForCalls) calls
	)
	UPDATE orderedUpdateTable WITH(READPAST)
    SET CallState = -2 
	OUTPUT inserted.ExplicitSid,
		   inserted.[ID],
		   inserted.[interviewID],
		   inserted.[TimeInShift],
		   inserted.[Priority],
		   inserted.[CallOrder],
		   inserted.[ApptID],
		   inserted.[ExpireTime],
		   deleted.[ShiftPriority]
	INTO @usedCalls

	INSERT INTO bvCallsSentToDialer
	SELECT @SuitableTimeForCalls AS [Time], 
		   @SurveySID AS SurveySID, 
		   ObjectSid AS ExplicitSid, 
		   Count(*) AS CallsCount
	 FROM @usedCalls GROUP BY ObjectSid
    
    SELECT c.ID, 
           ISNULL( p.Sid, 0 ) AS ExplicitSid, --person id (if person is assigned) or 0 (if survey or person group)
           @SurveySID AS SurveySid,
           i.DialingMode DiallingMode,
		   Interview AS InterviewID, 
		   TelephoneNumber,
		   ExtensionNumber,
		   (CASE WHEN c.ApptId > 0 THEN c.[TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
		   (CASE WHEN p.Sid IS NULL AND @SurveySID <> ObjectSid THEN ObjectSid
                 ELSE 0
            END) AS GroupID, --Explicit Group ID or 0 in all other cases (when call is assigned to  implicit survey group or a user)
		   c.ExpireTime
    FROM @usedCalls c
    INNER JOIN BvInterview i ON Interview = i.ID AND   --we should avoid this join. this field can be stored in bvsvyschedule or somewhere else
                                SurveySID = @SurveySID
    LEFT JOIN BvPerson p on p.SID = ObjectSid
	ORDER BY Priority DESC,
			 c.ShiftPriority DESC,
             TimeInShift,
			 CallOrder
	
RETURN (@@ROWCOUNT)
GO
PRINT N'Altering [dbo].[BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned]
 @SurveySID INT,
 @DialerId INT = 0,
 @Count  INT,  --number of requested calls
 @SuitableTimeForCalls DATETIME
AS

SET NOCOUNT ON
	DECLARE @FixeNumberCallsPerPerson INT = 10

      --it should be at end of the SP. it uses for decrease recompilation
	DECLARE @CachedCalls TABLE (
	  [ExplicitSID] [int] NOT NULL,
	  [ID] [int] NOT NULL,
	  [InterviewID] [int] NOT NULL,
	  [TimeInShift] [datetime] NULL,
	  [Priority] [int] NOT NULL,
	  [CallOrder] [int] NOT NULL,
	  [ApptId] [int],
	  [ExpireTime] [datetime],
	  [ShiftPriority] TINYINT NOT NULL )
     
	;WITH orderedUpdateTable AS
	(
		SELECT c.*, a.ShiftPriority
		FROM BvLoginGroup g
		INNER JOIN BvTasks t ON g.PersonSID = t.PersonSID
		INNER JOIN BvActiveShiftTypeZone a on a.SurveyId=@SurveySID and a.SurveyId=g.SurveySID
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](0, g.PersonSID, a.Id, @SurveySID, @SuitableTimeForCalls, @FixeNumberCallsPerPerson) c
		WHERE g.PersonSid = g.ObjectSID AND t.DialerId = @DialerId and g.DialTypeId = 0
	)
    UPDATE orderedUpdateTable 
    SET CallState = -2 
	OUTPUT inserted.ExplicitSid,
		   inserted.[ID],
		   inserted.[interviewID],
		   inserted.[TimeInShift],
		   inserted.[Priority],
		   inserted.[CallOrder],
		   inserted.[ApptID],
		   inserted.[ExpireTime],
		   deleted.[ShiftPriority]
	INTO @CachedCalls

	INSERT INTO bvCallsSentToDialer
	SELECT @SuitableTimeForCalls AS [Time], 
		   @SurveySID AS SurveySID, 
		   ExplicitSID AS ExplicitSid, 
		   Count(*) AS CallsCount
	 FROM @CachedCalls GROUP BY ExplicitSID

    SELECT c.[ID], 
           c.[ExplicitSid], 
           @SurveySID SurveySid, 
           i.DialingMode DiallingMode,
		   c.[InterviewID], 
		   i.[TelephoneNumber],
		   i.[ExtensionNumber],
		   (CASE WHEN c.ApptId > 0 THEN c.[TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
		   0 as [GroupID],
		   c.ExpireTime
    FROM @CachedCalls c 
    INNER JOIN BvInterview i ON c.[Interviewid] = i.[ID] AND
                                i.[SurveySID] = @SurveySID
    ORDER BY Priority DESC, c.ShiftPriority DESC, TimeInShift, CallOrder
 
RETURN (@@ROWCOUNT)

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
	DECLARE @callId INT
    DECLARE @rowCount INT
    
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
		@surveyId = SurveySid,
		@callId = Id

	SET @rowCount = @@ROWCOUNT

	select @callID CallID, @surveyId SurveySID, @interviewId iid
	where @callID is not null

	IF(@rowCount = 0) RETURN 0
	
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
    IF @personId is null and @SuitableTimeForCalls is null
	begin
		select 0 CallID,
		       0 SurveySID,
			   0 iid
		where 1 = 0
		return 0
	end

    DECLARE @interviewId INT
	DECLARE @callId INT
    DECLARE @rowCount INT
    
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
		@surveyId = SurveySid,
		@callId = Id

	SET @rowCount = @@ROWCOUNT

	select @callID CallID, @surveyId SurveySID, @interviewId iid
	where @callID is not null

	IF(@rowCount = 0) RETURN 0
	
	UPDATE BvAppointment 
	SET State = 2 
	WHERE State = 1 AND 
	      SurveysId = @surveyId AND 
	      InterviewSid = @interviewId
RETURN 0
GO
PRINT N'Altering [dbo].[BvSpLookUpByPerson_ForCallGroup]...';


GO
ALTER PROCEDURE [dbo].[BvSpLookUpByPerson_ForCallGroup]
	@SurveyID INT,
	@CallGroupID INT,
	@PersonID INT,
	@Now DATETIME
AS
	DECLARE @interviewId INT
	DECLARE @rowCount INT
	DECLARE @CallID INT
	DECLARE @ConditionValue INT
		    
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
		@CallID = ID,
		@interviewId = InterviewID,
		@surveyId = SurveySid,
		@ConditionValue = ConditionValue
	
	SET @rowCount = @@ROWCOUNT
			
	SELECT @CallID as CallID, @surveyId as SurveySID, @interviewId as iid WHERE @rowCount <> 0
		
	IF(@rowCount = 0) RETURN 0
			
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
PRINT N'Altering [dbo].[BvSpLookUpByPerson_ForSurvey]...';


GO
ALTER PROCEDURE [dbo].[BvSpLookUpByPerson_ForSurvey]
      @surveyId int,
      @personId int,
      @Now DATETIME
AS
    DECLARE @CallID INT
    DECLARE @interviewId INT
    DECLARE @rowCount INT
    
    ;WITH ExplicitSIDs AS
    (
            SELECT p.ObjectSID, p.DialTypeId FROM BvLoginGroup p WHERE p.PersonSID = @personId
    )
    ,calls AS
      (
            SELECT TOP(1) cc.*
            FROM ExplicitSIDs e
			inner join BvActiveShiftTypeZone a on a.SurveyId = @surveyId
            CROSS APPLY [dbo].[GetCallBySurvey](e.DialTypeId, a.Id, @surveyId, e.ObjectSID, @Now) cc
            ORDER BY Priority DESC, a.ShiftPriority DESC, TimeInShift, ExplicitType DESC, CallOrder
      )
      UPDATE calls WITH(READPAST)
      SET CallState = -1,
            @CallID = ID,
            @interviewId = InterviewID

      SET @rowCount = @@ROWCOUNT
      
      SELECT @CallID as CallID, @surveyId as SurveySID, @interviewId as iid WHERE @rowCount <> 0
      
      IF(@rowCount = 0) RETURN 0
      
      UPDATE BvAppointment 
      SET State = 2 
      WHERE State = 1 AND 
            SurveysId = @surveyId AND 
            InterviewSid = @interviewId
RETURN 0
GO
PRINT N'Altering [dbo].[BvSpGetLiveShifts]...';


GO
alter procedure [dbo].[BvSpGetLiveShifts]
@utc smalldatetime,    -- in utc time
@tz_local INT,
@TzBalancingThreshold INT=0
as
set nocount on
declare @date1 int
DECLARE @ShiftTypeNone INT = -2147483648; --None constant for bvSvySchedule.ShiftTypeID
 
set @date1 = @@DATEFIRST
set DATEFIRST 7
 
    create table #temp_tz ( 
        tz_id    int,
        ltStart  smalldatetime,				--local time in a specific TZ
        minStart int						--offset in minutes withing a week (for ltStart)
    )
 
    create table #active
    (
        [ID] int not null,
        ScheduleID int not null,
        tz_id int not null,
		[ShiftPriority] int not null
    )
 
    -- check in future
    set @utc = dateadd( minute, 1, @utc )
 
    -- insert into temp normalize date by all timezone
    -- normalize date - time in minute from start of week
    -- = day_of_week * 24 * 60 + hour * 60 + minute
    insert into #temp_tz 
	select	[ID]										AS TzID, 
														ltStart,
            (DATEPART( dw, ltStart ) - 1 ) * 1440 + 
             DATEPART( hour, ltStart ) * 60 + 
             DATEPART( minute, ltStart )				AS minStart

    from ( select  [ID], dbo.UTC2LT( @utc, Bias, DaylightType,
            StandardDayOfWeek, StandardStart, StandardBias,
            DaylightDayOfWeek, DaylightStart, DaylightBias ) as ltStart
            from BvTimezone ) s1
 
        --select * from #temp_tz
  
    -- insert periodical active shifts to 
    insert into #active
        select distinct 
			z.[ID], 
			tzs.owner_id, 
			tzs.tz_id,
			case 
				when (tzs.finish_dt - #temp_tz.minStart) < @TzBalancingThreshold
					then 1
					else 0
			end	as [ShiftPriority]
        from #temp_tz
        inner join BvTzPeriodicalShifts tzs on
            #temp_tz.tz_id = tzs.tz_id
              and ( #temp_tz.minStart >= tzs.start_dt 
              and #temp_tz.minStart < tzs.finish_dt OR 
              #temp_tz.minStart + 10080/*week*/ >= tzs.start_dt 
              and #temp_tz.minStart + 10080/*week*/ < tzs.finish_dt)
        inner join BvShiftZones z on
              ( z.TimeZoneID = tzs.tz_id or
              ( z.TimeZoneID = 0 and tzs.tz_id = @tz_local ) )
              and z.ShiftTypeID = tzs.type_id
 
    -- delete shifts which fits exclusions
        delete from #active 
        from  #active a 
                        join BvTzUnPeriodicalShifts utzs on
                                a.tz_id = utzs.tz_id
                                 and a.ScheduleID = utzs.owner_id
                        join #temp_tz on #temp_tz.tz_id = utzs.tz_id
                        
                        where 
                                #temp_tz.ltStart >= utzs.start_dt and #temp_tz.ltStart < utzs.finish_dt
                        
    set DATEFIRST @date1
    drop table #temp_tz

    -- insert timezones for [AnyValid] calls
    insert into #active
        select distinct -z.TimeZoneID, a.ScheduleID, a.tz_id, a.[ShiftPriority]
        from #active a, BvShiftZones z
        where a.[ID] = z.[ID]
    -- insert fictive shift for [None] calls
    insert into #active
        select 
			@ShiftTypeNone, 
			ScheduleID, 
			0,
			0				-- for simplicity we won't prioritise such call in tz balancing mechanism
		FROM BvSchedule
 
    select a.[ID], b.SID, a.[ShiftPriority]
        from  BvSurvey b
		inner join #active a
		on a.ScheduleID = b.ScheduleID
        WHERE b.State = 1 /* survey opened */
			  AND EXISTS( SELECT 1 FROM BvSvyScheduleRuntimeStatistics srs WHERE b.SID = srs.SurveyId AND srs.ShiftTypeID = a.ID AND srs.FreeCount > 0)

return (0)
GO
PRINT N'Altering [dbo].[BvSpQueueUpSheduleTask3]...';


GO
ALTER PROCEDURE [dbo].[BvSpQueueUpSheduleTask3]
    @NowUTC           datetime,
    @DefaultTZ        INT,
	@TzBalancingThreshold INT=0
as
set nocount on

declare @rows int
 
    -- temp table for determine active shifts/survey
    create table #temp
    (
        [ID] int not null,
        SurveySID int not null,
		ShiftPriority int not null
    )
 
    -- calculate live shifts 
    insert into #temp exec BvSpGetLiveShifts @NowUTC, @DefaultTZ, @TzBalancingThreshold
 
        -- copy new shifts information
     delete BvActiveShiftTypeZone
     insert into BvActiveShiftTypeZone
     select [ID], SurveySID, ShiftPriority from #temp
 
     drop table #temp
return (0)
GO
PRINT N'Refreshing [dbo].[BvSpGetActiveCallsForSurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetActiveCallsForSurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpGetOpenedSurveys]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetOpenedSurveys]';


GO
PRINT N'Update complete.';


GO
