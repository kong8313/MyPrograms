GO
PRINT N'Dropping [dbo].[BvSvySchedule].[IX_BvSvyScheduleMain]...';


GO
DROP INDEX [IX_BvSvyScheduleMain]
    ON [dbo].[BvSvySchedule];


GO
PRINT N'Dropping [dbo].[BvSvySchedule].[IX_GetCallByCondition]...';


GO
DROP INDEX [IX_GetCallByCondition]
    ON [dbo].[BvSvySchedule];


GO
PRINT N'Dropping [dbo].[BvSvySchedule].[IX_GetCallBySurvey]...';


GO
DROP INDEX [IX_GetCallBySurvey]
    ON [dbo].[BvSvySchedule];


GO
PRINT N'Dropping [dbo].[GetHighPriorityCalls]...';


GO
DROP FUNCTION [dbo].[GetHighPriorityCalls];


GO
PRINT N'Dropping [dbo].[GetCallsPerGroup]...';


GO
DROP FUNCTION [dbo].[GetCallsPerGroup];


GO
PRINT N'Dropping [dbo].[GetTopCallsForShiftTypeGroupCell]...';


GO
DROP FUNCTION [dbo].[GetTopCallsForShiftTypeGroupCell];


GO
PRINT N'Dropping [dbo].[vLogins].[pk_vLogins]...';


GO
DROP INDEX [pk_vLogins]
    ON [dbo].[vLogins];


GO
PRINT N'Removing schema binding from [dbo].[vLogins]...';


GO
ALTER VIEW [dbo].[vLogins]
AS
SELECT   ObjectSID AS sid,
         SurveySID,
         count_big(*) AS cnt
FROM     dbo.BvLoginGroup
GROUP BY ObjectSID, SurveySID;


GO
PRINT N'Dropping [dbo].[BvSpPerson_Update]...';


GO
DROP PROCEDURE [dbo].[BvSpPerson_Update];


GO
PRINT N'Altering [dbo].[BvLoginGroup]...';


GO
ALTER TABLE [dbo].[BvLoginGroup]
    ADD [DialTypeId] TINYINT CONSTRAINT [DF_BvLoginGroup_DialTypeId] DEFAULT(0) NOT NULL;


GO
PRINT N'Creating [dbo].[BvSvySchedule].[IX_BvSvyScheduleMain]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_BvSvyScheduleMain]
    ON [dbo].[BvSvySchedule]([SurveySID] ASC, [ShiftTypeID] ASC, [ExplicitSID] ASC, [SampleTypeId] ASC, [CellId] ASC, [Priority] DESC, [TimeInShift] ASC, [ExplicitType] DESC, [CallOrder] ASC, [InterviewID] ASC)
    INCLUDE([ID], [CallState], [ApptID], [ConditionValue], [ExpireTime]);


GO
PRINT N'Creating [dbo].[BvSvySchedule].[IX_GetCallByCondition]...';


GO
CREATE NONCLUSTERED INDEX [IX_GetCallByCondition]
    ON [dbo].[BvSvySchedule]([SurveySID] ASC, [ShiftTypeID] ASC, [ExplicitSID] ASC, [SampleTypeId] ASC, [ConditionValue] ASC, [Priority] DESC, [TimeInShift] ASC, [ExplicitType] DESC, [CallOrder] ASC)
    INCLUDE([ID], [ExpireTime], [CallState]) WHERE ConditionValue <> 0;


GO
PRINT N'Creating [dbo].[BvSvySchedule].[IX_GetCallBySurvey]...';


GO
CREATE NONCLUSTERED INDEX [IX_GetCallBySurvey]
    ON [dbo].[BvSvySchedule]([SurveySID] ASC, [ShiftTypeID] ASC, [ExplicitSID] ASC, [SampleTypeId] ASC, [Priority] DESC, [TimeInShift] ASC, [ExplicitType] DESC, [CallOrder] ASC)
    INCLUDE([ID], [ExpireTime], [CallState]) WHERE ConditionValue <> 0;


GO
PRINT N'Altering [dbo].[BvTrBvPersonRel_Insert]...';


GO
ALTER TRIGGER [BvTrBvPersonRel_Insert] ON [dbo].[BvPersonRel] 
AFTER INSERT
AS
BEGIN
	SET NOCOUNT ON
	
	INSERT INTO BvLoginGroup(PersonSID, ObjectSID, SurveySID, DialTypeId ) 
	SELECT i.PersonSID, i.ObjectSID, CASE WHEN p.ManualSelection = 2 /*is survey selection*/ THEN t.SurveySID ELSE 0 END, t.SampleTypeId  FROM inserted i
	INNER JOIN BvTasks t ON i.PersonSID = t.PersonSID
	INNER JOIN BvPerson p ON i.PersonSID = p.SID
	
END
GO
PRINT N'Altering [dbo].[GetCallByCondition]...';


GO
ALTER FUNCTION [dbo].[GetCallByCondition]
(   @DialTypeId TINYINT,
	@ShiftTypeId INT,
    @SurveySid INT,
	@ExplicitSID INT,
	@ConditionValue INT,
	@Now DATETIME) 
RETURNS TABLE WITH SCHEMABINDING
AS RETURN(
		    SELECT TOP(1) [ID],
		                ExplicitSID,
						ExplicitType,
		                SurveySID,
		                InterviewID,
		                CallState,
						ApptId,
						TimeInShift,
						CallOrder,
						Priority,
						ConditionValue
		    FROM [dbo].BvSvySchedule
		    WHERE 
				SampleTypeId = @DialTypeId AND
			    ShiftTypeId = @ShiftTypeId AND
				CallState = 2 AND
				SurveySid = @SurveySid AND
				BvSvySchedule.ExplicitSID = @ExplicitSID AND
				BvSvySchedule.ConditionValue  = @ConditionValue AND
				BvSvySchedule.ConditionValue <> 0 AND 
				BvSvySchedule.TimeInShift < @Now
		ORDER BY Priority DESC, TimeInShift, ExplicitType DESC, CallOrder )
GO
PRINT N'Altering [dbo].[GetCallBySurvey]...';


GO
ALTER FUNCTION [dbo].[GetCallBySurvey]
(   
	@DialTypeId TINYINT,
    @ShifttypeId INT,
    @SurveySid INT,
    @ExplicitSID INT,
	@Now DATETIME) 
RETURNS TABLE WITH SCHEMABINDING
AS RETURN(
          SELECT TOP(1) [ID],
                        ExplicitSID,
                        ExplicitType,
                        SurveySID,
                        InterviewID,
                        CallState,
                        ApptId,
                        TimeInShift,
                        CallOrder,
                        Priority,
                        ConditionValue
          FROM [dbo].BvSvySchedule
          WHERE 
                    SampleTypeId = @DialTypeId AND
                    ShifttypeId = @ShifttypeId AND
                    CallState = 2 AND
                    SurveySid = @SurveySid AND
                    BvSvySchedule.ExplicitSID = @ExplicitSID AND
                    ConditionValue <> 0 AND
					TimeInShift < @Now
            ORDER BY Priority DESC, TimeInShift, ExplicitType DESC, CallOrder )
GO
PRINT N'Altering [dbo].[GetCallsForPredictiveMode]...';


GO
ALTER FUNCTION [dbo].[GetCallsForPredictiveMode]
(   @DialTypeId TINYINT,
	@rowCount AS INT,
    @ShiftTypeId INT,
    @ExplicitSID AS INT,
    @SurveySid AS INT,
    @TimeToRun AS DATETIME) 
RETURNS TABLE
AS RETURN(
          SELECT TOP(@rowCount) [ID],
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
	      FROM BvSvySchedule
          WHERE SampleTypeId = @DialTypeId AND
		        SurveySid = @SurveySid AND
                ExplicitSID = @ExplicitSID AND
				CellId = 0 AND
                CallState = 2 AND
                TimeInShift <= @TimeToRun AND
                ShiftTypeId = @ShiftTypeId
          ORDER BY Priority DESC,
                   TimeInShift,
                   ExplicitType DESC,
                   CallOrder )
GO
PRINT N'Altering [dbo].[GetPriorityCallByExplicitSidAndShiftTypeId]...';


GO
ALTER FUNCTION [dbo].[GetPriorityCallByExplicitSidAndShiftTypeId]
(   @DialTypeId TINYINT,
    @ExplicitSID INT,
    @ShiftTypeID INT,
	@SurveyID INT,
    @SuitableTimeForCalls DATETIME,
	@TopCount INT)
RETURNS TABLE 
AS RETURN
(
	    SELECT TOP(@TopCount) c.*
        FROM BvSvySchedule c with(readpast)
		WHERE SampleTypeId = @DialTypeId AND
		      CallState = 2 AND
			  c.ExplicitSID = @ExplicitSID and
			  c.ShiftTypeID = @ShiftTypeID and
			  c.CellId = 0 and
			  TimeInShift <= @SuitableTimeForCalls AND
			  c.SurveySid = @SurveyID
		ORDER BY Priority DESC,
				 TimeInShift,
				 ExplicitType DESC,
				 CallOrder 
)
GO
PRINT N'Altering [dbo].[GetPriorityCallByExplicitSidAndShiftTypeIdClustered]...';


GO
ALTER FUNCTION [dbo].[GetPriorityCallByExplicitSidAndShiftTypeIdClustered]
(   @DialTypeId TINYINT,
    @ExplicitSID INT,
    @ShiftTypeID INT,
	@SurveyID INT,
	@CellId INT,
    @SuitableTimeForCalls DATETIME,
	@TopCount INT)
RETURNS TABLE 
AS RETURN
(
	SELECT TOP (@TopCount) c.*
        FROM BvSvySchedule c with(readpast)
        WHERE SampleTypeId = @DialTypeId AND
			  CallState = 2 AND
			  c.CellID = @CellID AND
			  c.ExplicitSID = @ExplicitSID and
			  c.ShiftTypeID = @ShiftTypeID and
			  TimeInShift <= @SuitableTimeForCalls AND
			  c.SurveySid = @SurveyID 
		ORDER BY Priority DESC,
				 TimeInShift,
				 ExplicitType DESC,
				 CallOrder 
)
GO
PRINT N'Creating [dbo].[GetTopCallsForShiftTypeGroupCell]...';


GO
CREATE FUNCTION [dbo].[GetTopCallsForShiftTypeGroupCell]
(   @DialTypeId as TINYINT,
    @rowCount AS INT,
    @ShiftTypeId INT,
    @ExplicitSID AS INT,
    @SurveySid AS INT,
	@CellId AS INT,
    @TimeToRun AS DATETIME) 
RETURNS TABLE
AS RETURN(
          SELECT TOP(@rowCount) [ID],
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
	  FROM BvSvySchedule

          WHERE SurveySid = @SurveySid AND
                ExplicitSID = @ExplicitSID AND
				CellId = @CellId AND
				SampleTypeId = @DialTypeId AND
                CallState = 2 AND
                TimeInShift <= @TimeToRun AND
                ShiftTypeId = @ShiftTypeId
          ORDER BY Priority DESC,
                   TimeInShift,
                   ExplicitType DESC,
                   CallOrder )
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
          SELECT TOP (@rowCount) c.*
          FROM BvActiveShiftTypeZone a
		  CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](@DialTypeId, @ObjectSid, a.Id, @SurveySID, @SuitableTimeForCalls, @rowCount) c
		  WHERE a.surveyid = @SurveySid
          ORDER BY priority DESC, TimeInShift, ExplicitType DESC, CallOrder )
GO
PRINT N'Creating [dbo].[GetCallsPerGroup]...';


GO
CREATE FUNCTION [dbo].[GetCallsPerGroup]
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
				ExpireTime					
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
                   TimeInShift,
                   ExplicitType DESC,
                   CallOrder )
GO
PRINT N'Adding schema binding to [dbo].[vLogins]...';


GO
ALTER view dbo.vLogins
with schemabinding
as
    select ObjectSID as sid, DialTypeId, SurveySID, count_big(*) as cnt
        from dbo.BvLoginGroup
    group by ObjectSID, DialTypeId, SurveySID
GO
PRINT N'Creating [dbo].[vLogins].[pk_vLogins]...';


GO
CREATE UNIQUE CLUSTERED INDEX [pk_vLogins]
    ON [dbo].[vLogins]([sid] ASC, [DialTypeId] ASC, [SurveySID] ASC);


GO
PRINT N'Creating [dbo].[GetHighPriorityCalls]...';


GO
CREATE FUNCTION [dbo].[GetHighPriorityCalls]
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
	SELECT  cpg.*
	FROM LoggedInGroups c
	CROSS APPLY dbo.GetCallsPerGroup(c.DialTypeId, c.cnt*@maxCallsPerGroup, c.sid, @SurveySID, @SuitableTimeForCalls) cpg
)
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
	    [ExpireTime] [datetime] )
        
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
		   inserted.[ExpireTime]
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
	  [ExpireTime] [datetime] )
     
	;WITH orderedUpdateTable AS
	(
		SELECT c.*
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
		   inserted.[ExpireTime]
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
    ORDER BY Priority DESC, TimeInShift, CallOrder
 
RETURN (@@ROWCOUNT)
GO
PRINT N'Altering [dbo].[BvSpLogin_SpinUp]...';


GO
ALTER  PROCEDURE [dbo].[BvSpLogin_SpinUp]
@PersonSID INTEGER
AS
declare @SurveySID int
declare @PersonMode int
declare @DialType TINYINT    
	select @SurveySID = SurveySID,
           @DialType = SampleTypeId
	from BvTasks where PersonSID = @PersonSID
    
    if @SurveySID is not null 
    begin
	    select @PersonMode = ManualSelection from BvPerson where sid = @PersonSID

        if(@PersonMode != 2) --is not survey selection
           SET @SurveySID = 0
    
        delete from BvLoginGroup where PersonSID = @PersonSID
        insert into BvLoginGroup WITH(TABLOCKX) select PersonSID, ObjectSID, @SurveySID, @DialType
            from BvPersonRel where PersonSID = @PersonSID
    end
 
return (0)
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
		   where c.SurveySID = s.SID and c.ExplicitSID = l.ObjectSID and c.SampleTypeId = l.DialTypeId)
    
    ;WITH calls AS
	(
	    SELECT TOP(1) c.*
		FROM #surveySids s
		INNER JOIN BvActiveShiftTypeZone a on a.Surveyid = s.Id
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](s.dialType, s.ObjectSID, a.Id, s.Id, @SuitableTimeForCalls, 1) c
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
	DECLARE @callId INT
    DECLARE @rowCount INT
    
    ;WITH calls AS
	(
	    SELECT TOP(1) c.*
        FROM BvLoginGroup t
		INNER JOIN BvActiveShiftTypeZone a on a.Surveyid = t.SurveySid and t.SurveySid = @surveyId and t.PersonSID = @personId
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](t.DialTypeId, t.ObjectSID, a.Id, @surveyId, @SuitableTimeForCalls, 1) c
		ORDER BY Priority DESC,
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
		SELECT p.ObjectSID as ExplicitSID, a.Id as ShiftTypeId, ConditionValue, ConditionPriority, RotatePriority, p.DialTypeId FROM BvLoginGroup p
		INNER JOIN BvActiveShiftTypeZone a on a.SurveyId = @SurveyId
		INNER JOIN BvSvyScheduleRuntimeStatistics s ON s.SurveyId = @SurveyId AND s.ShiftTypeID = a.Id AND s.ExplicitSID = p.ObjectSID
		INNER JOIN BvCallGroupConditionPerSurvey cgc ON cgc.SurveyId = @SurveyID AND cgc.CallGroupId = @CallGroupID 
		WHERE p.PersonSID = @personId 
	),
	calls as
	(
		SELECT TOP(1) cc.* FROM conditions c
		CROSS APPLY dbo.GetCallByCondition( c.DialTypeId, c.ShiftTypeID, @surveyId, c.ExplicitSID, c.ConditionValue, @Now ) cc
		ORDER BY Priority DESC, ConditionPriority DESC, RotatePriority ASC, TimeInShift, ExplicitType DESC, CallOrder
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
		FROM BvSvySchedule c WITH(READPAST)
		INNER JOIN BvLoginGroup p ON p.PersonSID = @personId AND p.DialTypeId = c.SampleTypeId
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
            ORDER BY Priority DESC, TimeInShift, ExplicitType DESC, CallOrder
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
PRINT N'Altering [dbo].[BvSpGetActiveCallsForSurvey]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetActiveCallsForSurvey]
	@SurveySID INT,	
	@SuitableTimeForCalls DATETIME
AS
	
	IF @SurveySID  IS NULL
	BEGIN
		SELECT  '' AS Name,
				0 AS ResultCount,
				0 AS RequestCount		
		RETURN 0;
	END
				    			    
	SELECT ISNULL (g.[Name], '*Survey Assignment*' ) as [Name], 
	count(*)  AS ResultCount, 
	CAST(c.cnt*10  AS INT) AS RequestCount
    FROM vLogins c with ( noexpand, INDEX([pk_vLogins]) )
	INNER JOIN BvActiveShiftTypeZone a on a.SurveyId = @SurveySid
	CROSS APPLY dbo.GetCallsForPredictiveMode(c.DialTypeId, c.cnt*10, a.Id, c.sid, @SurveySID, @SuitableTimeForCalls)
	LEFT JOIN (SELECT [SID], [Name] FROM [BvPerson]
			   UNION 
			   SELECT [SID], [Name] from [BvPersonGroup] ) as g on [ExplicitSid] = g.[SID]
	where c.SurveySID = @SurveySID
	group by g.Name, c.cnt   	
	
RETURN (@@ROWCOUNT)
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
	  [ExpireTime] [datetime] )
        
	;WITH orderedUpdateTable AS
	(
		SELECT TOP(@Count) c.*
        FROM BvActiveShiftTypeZone a
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](0, @SurveySID, a.Id, @SurveySID, @SuitableTimeForCalls, @Count) c
		WHERE a.Surveyid = @SurveySID
		ORDER BY Priority DESC,
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
		   inserted.[ExpireTime]
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
	  [ExpireTime] [datetime] )
        
	;WITH orderedUpdateTable AS
	(
		SELECT TOP(@Count) c.*
        FROM BvActiveShiftTypeZone a
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](0, @groupID, a.Id, @SurveySID, @SuitableTimeForCalls, @Count) c
		WHERE a.Surveyid = @SurveySID
		ORDER BY Priority DESC,
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
		   inserted.[ExpireTime]
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
             TimeInShift,
 			 CallOrder
 
RETURN (@@ROWCOUNT)
GO
PRINT N'Altering [dbo].[BvSpInterview_UpdateRespondentFields]...';


GO
ALTER PROCEDURE [dbo].[BvSpInterview_UpdateRespondentFields]
    @projectId NVARCHAR(64),
    @respId INT,
    @TelephoneNumber NVARCHAR(255),
    @RespondentName NVARCHAR(255),
    @ExtensionNumber NVARCHAR(255),
    @TimeZoneId INT,
	@SampleType TINYINT
AS

    DECLARE @SurveySID INT
    SELECT @SurveySID = SID FROM BvSurvey WHERE Name = @projectId
    IF @SurveySID IS NULL 
    BEGIN
        --RAISERROR( 'survey with projectID = ''%d'' not found', 16, 1, @projectId )
        RETURN (0)
    END

	if @SampleType IS NOT NULL
	BEGIN
        UPDATE BvInterview
            SET TelephoneNumber = @TelephoneNumber,
                RespondentName = @RespondentName,
                ExtensionNumber = @ExtensionNumber,
                TimezoneId = ISNULL( @TimeZoneId, TimezoneId ),
				SampleTypeId = @SampleType
        WHERE ID = @respId AND
              SurveySID = @SurveySID

        UPDATE BvSvySchedule
            SET SampleTypeId = @SampleType
        WHERE InterviewID = @respId AND
              SurveySID = @SurveySID
	END
	ELSE
	BEGIN
    UPDATE BvInterview
        SET TelephoneNumber = @TelephoneNumber,
            RespondentName = @RespondentName,
            ExtensionNumber = @ExtensionNumber,
            TimezoneId = ISNULL( @TimeZoneId, TimezoneId )
    WHERE ID = @respId AND
          SurveySID = @SurveySID
	END
        
	IF @TimeZoneId IS NOT NULL AND @TimeZoneId <> 0
	BEGIN
		UPDATE BvAppointment
		SET TZID = @TimeZoneId
		WHERE SurveySID = @SurveySID AND
			InterviewSID = @respId
	END
GO
PRINT N'Altering [dbo].[BvSpPerson_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpPerson_Insert]
        @SID INT, 
        @Name NVARCHAR( 255 ),  
        @FullName NVARCHAR( 255 ),
        @Description NVARCHAR( 255 ),
        @ManualSelection INT,
        @AssignmentsListMode INT,
        @BvID INT,
        @PwdSaltTxt NVARCHAR(256),
		@CallGroupId INT,
		@CallCenterID INT,
		@Location NVARCHAR(256),
		@SampleTypeId TINYINT
AS

DECLARE @Rows int

IF ( @BvID > 0 )
BEGIN
 EXEC @BvID = BvSpSetObjectNumber @SID, 10, @BvID
 IF @BvID = -1
     RETURN ( 50006 )
END

IF (EXISTS(SELECT 1 FROM BvPerson WHERE [Name]=@Name))
BEGIN
    RAISERROR( 'Person with name %s already exists', 12, 1, @Name )
    RETURN -1
END

INSERT  BvPerson( 
        SID,
        [Name], 
        FullName,
        [Description],
        ManualSelection, 
        AssignmentsListMode,
        PwdSaltTxt,
		CallGroupID,
		CallCenterID,
        Location,
		SampleTypeId)
    VALUES ( 
        @SID,
        @Name, 
        @FullName,
        @Description,
        @ManualSelection,
        @AssignmentsListMode, 
        @PwdSaltTxt,
		@CallGroupId,
		@CallCenterID,
        @Location,
		@SampleTypeId)

INSERT BvPersonFailedLoginAttempts( PersonId, Count ) VALUES( @SID, 0 )

RETURN 0
GO
PRINT N'Altering [dbo].[BvSpSvySch_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpSvySch_Insert]
        @ID                 int,
        @ApptID             int,
        @SurveySID          int,
        @InterviewID        int,
        @CallState          int,
        /* 
         * @ShiftTypeID > 0 means specific shift type id( BvShiftType.ID ) and should be resolved to ShiftZoneId in bvSvySchedule.ShiftTypeID
         * @ShiftTypeID = Int32.MinValue(-2147483648) meens [None] and should ne resolved to Int32.MinValue in BvSvySchedule.ShiftTypeID
         * @ShiftTypeID =-1 @ShiftTypeID means [Any valid] and should be resolved to -Timezone in BvSvySchedule.ShiftTypeID
         */
        @ShiftTypeID        int,
        @Priority           int,
        @TimeInShift        datetime,
        @ExpireTime         datetime,
        @Resource           int,
        @RuleNumber         uniqueidentifier,
        @DefaultTimeZoneID  INT,
	    @FcdBehaviorAlgorithm INT, --0 - delete calls/ 1 - disable calls
	    @transientState     INT
AS
SET NOCOUNT ON
DECLARE @Rows INTEGER
DECLARE @ExplicitSID INTEGER
DECLARE @ExplicitType INTEGER
DECLARE @CallTZ INT

DECLARE @sqlQueryParams NVARCHAR(MAX)
DECLARE @sqlQuery NVARCHAR(MAX)
DECLARE @whereCondition NVARCHAR(MAX)
DECLARE @ClusteredCellIdQuery NVARCHAR(MAX) 
DECLARE @ROWCOUNT INT = 0
DECLARE @alias NVARCHAR(25) = 'repl'
DECLARE @ShiftTypeNone INT = -2147483648; --None constant for bvSvySchedule.ShiftTypeID
DECLARE @currentTransientState INT
DECLARE @ConditionValue INT = 0
DECLARE @DialTypeId TINYINT

    SET @InterviewID = ABS( @InterviewID )

    -- Get call TZ
    SELECT @CallTZ = TimezoneID,
           @currentTransientState = TransientState,
           @DialTypeId = SampleTypeId
    FROM BvInterview
    WHERE SurveySID = @SurveySID AND 
         [ID] = @InterviewID
         
    SET @CallTZ = ISNULL( @CallTZ, 0 )

    IF  @ShiftTypeID <> @ShiftTypeNone --Not [None]
    BEGIN
        DECLARE @ret INT       
        DECLARE @actualShiftTypeID INT
 
		/*
			@ShiftTypeID can contain negative timezone value
			but BvSpCheckCallOnShifts does not understand such values -
			in this case it should think that @ShiftTypeID = -1 [Any Valid] 
		*/
        IF @ShiftTypeID > 0
			SET @actualShiftTypeID = @ShiftTypeID
		ELSE
			SET @actualShiftTypeID = -1
        
        /*
			Note: we remove "Checking call time to be Out of Shifts", because if time in shift isn't hit to 
			shift of specific shift type, call will be delivered bit late.
			But we should call BvSpCheckCallOnShifts without TimeInShift, because we should check that 
			specific shifttype have somoething available shifts for specific timezone.
		*/
        EXEC @ret = BvSpCheckCallOnShifts @CallTZ, @actualShiftTypeID, NULL/*@TimeInShift*/, @SurveySID, @DefaultTimeZoneID
        IF @ret <> 0
            RETURN @ret
    END

    IF @Resource = 0
    BEGIN
        SET @ExplicitSID = @SurveySID

        SET @ExplicitType = 1
        IF @ExplicitSID IS NULL
        BEGIN
            RAISERROR( 'Could not find assignment group, %i', 16, 1, @ExplicitSID )
            RETURN -50002
        END
    END
    ELSE
    BEGIN
        SET @ExplicitSID = @Resource
        SET @ExplicitType = 2
    END

    IF @ShiftTypeID > 0--meens specific shift type id
    BEGIN
        SELECT @ShiftTypeID = [ID]
            FROM BvShiftZones WHERE ShiftTypeID = @ShiftTypeID
                AND TimeZoneID = @CallTZ
    END
    ELSE IF @ShiftTypeID <> @ShiftTypeNone -- means [Any valid]
    BEGIN
		SET @ShiftTypeID = -@CallTZ
    END
    --ELSE/*@ShiftTypeID = @ShiftTypeNone*/ -- means [None]
    --BEGIN
	--	SET @ShiftTypeID = @ShiftTypeNone
    --END

    DECLARE @ExpirationTime DATETIME = @ExpireTime
    DECLARE @TimeInShift1 DATETIME = @TimeInShift
    
    IF @ExpireTime IS NULL
        SET @ExpirationTime = '9999-01-01 00:00:00.000'
    
    IF @TimeInShift IS NULL
        SET @TimeInShift1 = '1899-12-30 00:00:00.000'

    DECLARE @oldApptID INT = NULL
    
    DECLARE @IsRandomCallDeliveryEnabled BIT
    DECLARE @SurveySchedulingMode INT
    DECLARE @ClusteredQuotaName NVARCHAR(256)
    DECLARE @StateGroupId INT 
    
	SELECT @IsRandomCallDeliveryEnabled = IsRandomCallDeliveryEnabled,
                   @SurveySchedulingMode = SurveySchedulingMode,
		   @ClusteredQuotaName = ClusteredQuotaName,
		   @StateGroupId = StateGroupID
	FROM BvSurvey
	WHERE SID = @SurveySID

    IF @transientState = 0 
	BEGIN
		SET @transientState = @currentTransientState
	END
    IF @SurveySchedulingMode = 1 /*CallGroup*/
    BEGIN
    	SET @ConditionValue = @transientState
    END

    EXEC BvClr_QuotaService_GetWhereForAllClosedSurveyCells @SurveySID, @alias, @whereCondition OUTPUT
	
	EXEC BvClr_ClusterQuotaService_GetCellIdQuery @SurveySID, @ClusteredQuotaName, @alias, @ClusteredCellIdQuery OUTPUT
	
	SET @sqlQuery = 
      N'SET @refID = 0
        MERGE BvSvySchedule as target
        USING( SELECT ci.*, CASE WHEN ' + @whereCondition + ' THEN 1 ELSE 0 END, s.FcdAction, ( ' + @ClusteredCellIdQuery + ') as CellId
					 FROM ( SELECT @SurveySID as SurveySID, @InterviewId as InterviewId, @ApptID as ApptId ) ci
					 LEFT JOIN BvState s ON @transientState = s.StateId AND s.StateGroupId = @StateGroupId
					 LEFT JOIN BvReplicatedData_' + CAST(@SurveySID AS NVARCHAR(255)) + ' as repl
                     ON respid = @InterviewId
					  ) AS source (SurveySid, InterviewId, Appt, IsClosed, FcdAction, CellId)
        ON target.SurveySID = source.SurveySID AND
           target.InterviewID = source.InterviewID
        WHEN MATCHED
        THEN
			  UPDATE
			  SET @refoldApptID     = ApptID,
			      @refID            = CASE WHEN FcdAction = 1 OR IsClosed = 0 OR @FcdBehaviorAlgorithm = 1 THEN ID ELSE 0 END,
				  ApptID            = @ApptID,
				  CallState         = CASE WHEN FcdAction = 1 OR IsClosed = 0 THEN @CallState ELSE @FcdBehaviorAlgorithm END,
				  Priority          = @Priority,
				  TimeInShift       = @TimeInShift1,
				  ExpireTime        = @ExpirationTime,
				  ShiftTypeID       = @ShiftTypeID,
				  ExplicitSID       = @ExplicitSID,
				  ExplicitType      = @ExplicitType,
				  RuleNumber        = @RuleNumber,
                  ConditionValue    = @ConditionValue,
				  OldPriority       = 0
        WHEN NOT MATCHED AND ( FcdAction = 1 OR IsClosed = 0 OR @FcdBehaviorAlgorithm = 1)
        THEN
              INSERT(ShiftTypeID,
                     ApptID,
                     InterviewID,
                     SurveySID,
                     CallState,
                     Priority,
                     TimeInShift,
                     ExpireTime,
                     ExplicitSID,
                     ExplicitType,
                     RuleNumber,
                     CallOrder,
					 ConditionValue,
					 CellId,
					 SampleTypeId )
              VALUES(@ShiftTypeID,
                     @ApptID,
                     @InterviewID,
                     @SurveySID,
                     CASE WHEN FcdAction = 1 OR IsClosed = 0 THEN @CallState ELSE 1/*disabled*/ END,
                     @Priority,
                     @TimeInShift1,
                     @ExpirationTime,
                     @ExplicitSID,
                     @ExplicitType,
                     @RuleNumber,
                     CASE WHEN @IsRandomCallDeliveryEnabled = 0 THEN InterviewId
						  ELSE dbo.GetRandomValue(@InterviewID)
					 END,
					 @ConditionValue,
					 CellId,
					 @DialTypeId);
         
        IF( (@@ROWCOUNT > 0) AND (@refoldApptID IS NULL))
           SET @refID = @@IDENTITY'
           
SET @sqlQueryParams = N'@SurveySID INT, @IsRandomCallDeliveryEnabled BIT, @CallState INT, @ApptID INT, @Priority INT, ' +
       '@TimeInShift1 DATETIME, @ExpirationTime DATETIME, @ShiftTypeID INT, @ExplicitSID INT, ' +
       '@ExplicitType INT, @RuleNumber uniqueidentifier, @InterviewID INT, @ConditionValue INT, ' +
	   '@FcdBehaviorAlgorithm INT, @StateGroupId INT, @transientState INT, @DialTypeId TINYINT, ' +
       '@refID INT OUTPUT, @refoldApptID INT OUTPUT'
       
    EXEC sp_executesql @sqlQuery, @sqlQueryParams, @SurveySID, @IsRandomCallDeliveryEnabled, @CallState, @ApptID, @Priority,
       @TimeInShift1, @ExpirationTime, @ShiftTypeID, @ExplicitSID, @ExplicitType, @RuleNumber,
       @InterviewID, @ConditionValue, @FcdBehaviorAlgorithm, @StateGroupId, @transientState, @DialTypeId, @refID = @ID OUTPUT, @refoldApptID = @oldApptID OUTPUT

    IF @ID > 0 --call was updated or inserted
    BEGIN         
		IF @oldApptID > 0
		  UPDATE BvAppointment
		  SET State = 2
		  WHERE ID = @oldApptID
		
	    IF @ApptID > 0
		  UPDATE BvAppointment SET State = 1 WHERE ID = @ApptID 
	END
  
RETURN (@ID)
GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Clean]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Clean]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_DeassignFromCallCenter]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_DeassignFromCallCenter]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Delete]';


GO
PRINT N'Update complete.';


GO
