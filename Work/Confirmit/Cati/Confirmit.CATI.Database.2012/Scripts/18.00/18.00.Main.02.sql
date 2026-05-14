PRINT N'Dropping FK_BvPersonMonitoringEvents_BvPersonMonitoring...';


GO
ALTER TABLE [dbo].[BvPersonMonitoringEvents] DROP CONSTRAINT [FK_BvPersonMonitoringEvents_BvPersonMonitoring];


GO
PRINT N'Dropping FK_BvPersonMonitoringLastID_BvPersonMonitoring...';


GO
ALTER TABLE [dbo].[BvPersonMonitoringLastID] DROP CONSTRAINT [FK_BvPersonMonitoringLastID_BvPersonMonitoring];


GO
PRINT N'Dropping FK_BvPersonMonitoring_BvPerson...';


GO
ALTER TABLE [dbo].[BvPersonMonitoring] DROP CONSTRAINT [FK_BvPersonMonitoring_BvPerson];


GO
PRINT N'Dropping PK_BvPersonMonitoring...';


GO
ALTER TABLE [dbo].[BvPersonMonitoring] DROP CONSTRAINT [PK_BvPersonMonitoring];


GO
PRINT N'Starting rebuilding table [dbo].[BvPersonMonitoring]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_BvPersonMonitoring] (
    [PersonSID]           INT            NOT NULL,
    [supervisorName]      NVARCHAR (255) NOT NULL,
    [MonitoringSessionID] BIGINT         NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_BvPersonMonitoring] PRIMARY KEY CLUSTERED ([PersonSID] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[BvPersonMonitoring])
    BEGIN
        
        INSERT INTO [dbo].[tmp_ms_xx_BvPersonMonitoring] ([PersonSID], [supervisorName], [MonitoringSessionID])
        SELECT   [PersonSID],
                 [supervisorName],
                 [MonitoringSessionID]
        FROM     [dbo].[BvPersonMonitoring]
        ORDER BY [PersonSID] ASC;
        
    END

DROP TABLE [dbo].[BvPersonMonitoring];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_BvPersonMonitoring]', N'BvPersonMonitoring';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_BvPersonMonitoring]', N'PK_BvPersonMonitoring', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating [dbo].[BvPersonMonitoring].[NCLIDX_BvPersonMonitoring_MonitoringSessionID]...';


GO
CREATE NONCLUSTERED INDEX [NCLIDX_BvPersonMonitoring_MonitoringSessionID]
    ON [dbo].[BvPersonMonitoring]([MonitoringSessionID] ASC);


GO
PRINT N'Creating FK_BvPersonMonitoringEvents_BvPersonMonitoring...';


GO
ALTER TABLE [dbo].[BvPersonMonitoringEvents] WITH NOCHECK
    ADD CONSTRAINT [FK_BvPersonMonitoringEvents_BvPersonMonitoring] FOREIGN KEY ([PersonSID]) REFERENCES [dbo].[BvPersonMonitoring] ([PersonSID]) ON DELETE CASCADE;


GO
PRINT N'Creating FK_BvPersonMonitoringLastID_BvPersonMonitoring...';


GO
ALTER TABLE [dbo].[BvPersonMonitoringLastID] WITH NOCHECK
    ADD CONSTRAINT [FK_BvPersonMonitoringLastID_BvPersonMonitoring] FOREIGN KEY ([PersonSID]) REFERENCES [dbo].[BvPersonMonitoring] ([PersonSID]) ON DELETE CASCADE;


GO
PRINT N'Creating FK_BvPersonMonitoring_BvPerson...';


GO
ALTER TABLE [dbo].[BvPersonMonitoring] WITH NOCHECK
    ADD CONSTRAINT [FK_BvPersonMonitoring_BvPerson] FOREIGN KEY ([PersonSID]) REFERENCES [dbo].[BvPerson] ([SID]) ON DELETE CASCADE;


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
	  [ApptID] [int] not null)
        
	;WITH orderedUpdateTable AS
	(
		SELECT TOP ( @Count ) *
		FROM BvSvySchedule WITH(READPAST)
		WHERE SurveySID = @SurveySID AND
				ExplicitSid = @SurveySID AND 
				CallState = 2 AND
				TimeInShift <= @SuitableTimeForCalls AND
			    IsInActiveShiftType = 1
		ORDER BY Priority DESC,
                 TimeInShift,
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
		   inserted.[ApptID]
	INTO @CachedCalls

    SELECT c.[ID], 
           c.[ExplicitSid], 
           @SurveySID SurveySid, 
           i.DialingMode DiallingMode,
		   c.[InterviewID], 
		   i.[TelephoneNumber],
		   (CASE WHEN c.ApptId > 0 THEN c.[TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
		   0 as [GroupID]
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
	  [ApptID] [int] not null)
        
	;WITH orderedUpdateTable AS
	(
		SELECT TOP ( @Count ) *
		FROM BvSvySchedule WITH(READPAST)
		WHERE SurveySID = @SurveySID AND
				ExplicitSid = @groupID AND 
				CallState = 2 AND
				TimeInShift <= @SuitableTimeForCalls AND
			    IsInActiveShiftType = 1
		ORDER BY Priority DESC,
                 TimeInShift,
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
		   inserted.[ApptID]
	INTO @CachedCalls

    SELECT c.[ID], 
           c.[ExplicitSid], 
           @SurveySID SurveySid, 
           i.DialingMode DiallingMode,
		   c.[InterviewID], 
		   i.[TelephoneNumber],
		   (CASE WHEN c.ApptId > 0 THEN c.[TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
		   @GroupID as [GroupID]
    FROM @CachedCalls c 
    INNER JOIN BvInterview i ON c.[Interviewid] = i.[ID] AND
                                i.[SurveySID] = @SurveySID
    ORDER BY Priority DESC,
             TimeInShift,
 			 CallOrder
 
RETURN (@@ROWCOUNT)
GO
PRINT N'Altering [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]
	@SurveySID INT,
	@Count  INT,  --number of requested calls
	@SuitableTimeForCalls DATETIME
AS

	DECLARE @Groups TABLE(
		[ObjectSid] [int] NOT NULL,
		[GroupSize] [int] NOT NULL)
		
    DECLARE @MinDistributedCalls INT = 5
	
	INSERT INTO @Groups
    SELECT c.ExplicitSID, 
           COUNT(*) GroupSize --should we limit this value as it was limited during filling bvcachedcalls.
    FROM BvSvySchedule c
	INNER JOIN vLogins v on c.ExplicitSID = v.sid AND
	                        c.SurveySID = @SurveySID AND
                            c.CallState = 2 AND
							TimeInShift <= @SuitableTimeForCalls AND
		                    c.IsInActiveShiftType = 1
    GROUP BY c.ExplicitSID
    
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
		[ApptID] [INT])
        
	;WITH orderedUpdateTable as
	(    
		SELECT calls.*
		FROM @Groups groups
		CROSS APPLY dbo.GetCallsForGroupForPredictiveSurvey( 
			groups.GroupSize, @SurveySID, groups.ObjectSid, @SuitableTimeForCalls) calls
	)
	UPDATE orderedUpdateTable WITH(READPAST)
    SET CallState = -2 
	OUTPUT inserted.ExplicitSid,
		   inserted.[ID],
		   inserted.[interviewID],
		   inserted.[TimeInShift],
		   inserted.[Priority],
		   inserted.[CallOrder],
		   inserted.[ApptID]
	INTO @usedCalls
    
    SELECT c.ID, 
           ISNULL( p.Sid, 0 ) AS ExplicitSid, --person id (if person is assigned) or 0 (if survey or person group)
           @SurveySID AS SurveySid,
           i.DialingMode DiallingMode,
		   Interview AS InterviewID, 
		   TelephoneNumber,
		   (CASE WHEN c.ApptId > 0 THEN c.[TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
		   (CASE WHEN p.Sid IS NULL AND @SurveySID <> ObjectSid THEN ObjectSid
                 ELSE 0
            END) AS GroupID --Explicit Group ID or 0 in all other cases (when call is assigned to  implicit survey group or a user)
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
	  [ApptId] [int])
        
	;WITH orderedUpdateTable AS
	(
		SELECT calls.*, ROW_NUMBER() over (partition by ExplicitSid order by Priority DESC, TimeInShift, CallOrder) rn
		FROM BvSvySchedule calls WITH(READPAST)
		where CallState = 2 AND 
		      SurveySID = @SurveySID AND
			  TimeInShift <= @SuitableTimeForCalls AND
			  IsInActiveShiftType = 1
	)
    UPDATE orderedUpdateTable 
    SET CallState = -2 
	OUTPUT inserted.ExplicitSid,
		   inserted.[ID],
		   inserted.[interviewID],
		   inserted.[TimeInShift],
		   inserted.[Priority],
		   inserted.[CallOrder],
		   inserted.[ApptID]
	INTO @CachedCalls
    where ExplicitSid in(select PersonSID from BvTasks where SurveySID = @SurveySID ) and rn <= @FixeNumberCallsPerPerson

    SELECT c.[ID], 
           c.[ExplicitSid], 
           @SurveySID SurveySid, 
           i.DialingMode DiallingMode,
		   c.[InterviewID], 
		   i.[TelephoneNumber],
		   (CASE WHEN c.ApptId > 0 THEN c.[TimeInShift] ELSE '1899-12-30T00:00:00.000' END) AS TimeInShift,
		   0 as [GroupID]
    FROM @CachedCalls c 
    INNER JOIN BvInterview i ON c.[Interviewid] = i.[ID] AND
                                i.[SurveySID] = @SurveySID
    ORDER BY Priority DESC, TimeInShift, CallOrder
 
RETURN (@@ROWCOUNT)
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
		FROM BvSvySchedule c WITH(READPAST)
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
		FROM BvSvySchedule c WITH(READPAST)
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
		FROM BvSvySchedule c WITH(READPAST)
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
PRINT N'Refreshing [dbo].[BvSpGetListSurveyTasks]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpGetListSurveyTasks';


GO
PRINT N'Refreshing [dbo].[BvSpPersonMonitoring_IsStart]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpPersonMonitoring_IsStart';


GO
PRINT N'Refreshing [dbo].[BvSpPersonMonitoring_Start]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpPersonMonitoring_Start';


GO
PRINT N'Refreshing [dbo].[BvSpPersonMonitoring_Stop]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpPersonMonitoring_Stop';


GO
PRINT N'Checking existing data against newly created constraints';


GO



GO
ALTER TABLE [dbo].[BvPersonMonitoringEvents] WITH CHECK CHECK CONSTRAINT [FK_BvPersonMonitoringEvents_BvPersonMonitoring];

ALTER TABLE [dbo].[BvPersonMonitoringLastID] WITH CHECK CHECK CONSTRAINT [FK_BvPersonMonitoringLastID_BvPersonMonitoring];

ALTER TABLE [dbo].[BvPersonMonitoring] WITH CHECK CHECK CONSTRAINT [FK_BvPersonMonitoring_BvPerson];


GO
PRINT N'Update complete.';


GO
