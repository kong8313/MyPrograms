
GO
PRINT N'Creating [dbo].[BvCallsSentToDialer]...';


GO
CREATE TABLE [dbo].[BvCallsSentToDialer] (
    [Time]        DATETIME NOT NULL,
    [SurveySID]   INT      NOT NULL,
    [ExplicitSID] INT      NOT NULL,
    [CallsCount]  INT      NOT NULL
);


GO
PRINT N'Creating [dbo].[BvCallsSentToDialer].[IX_BvCallsSentToDialer_Time]...';


GO
CREATE CLUSTERED INDEX [IX_BvCallsSentToDialer_Time]
    ON [dbo].[BvCallsSentToDialer]([Time] ASC);


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
		SELECT TOP(@Count) c.*
        FROM BvActiveShiftTypeZone a
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](@SurveySID, a.Id, @SurveySID, @SuitableTimeForCalls, @Count) c
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
		   inserted.[ApptID]
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
		SELECT TOP(@Count) c.*
        FROM BvActiveShiftTypeZone a
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](@groupID, a.Id, @SurveySID, @SuitableTimeForCalls, @Count) c
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
		   inserted.[ApptID]
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
    SELECT c.sid, count(*)
    FROM vLogins c with ( noexpand, INDEX([pk_vLogins]) )
	INNER JOIN BvActiveShiftTypeZone a on a.SurveyId = @SurveySid
	CROSS APPLY dbo.GetCallsForPredictiveMode(c.cnt*20, a.Id, c.sid, @SurveySID, @SuitableTimeForCalls)
	where c.SurveySID = @SurveySID
	group by c.sid
    
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
		FROM @Groups g
		CROSS APPLY dbo.GetCallsForGroupForPredictiveSurvey(g.GroupSize, @SurveySID, g.ObjectSid, @SuitableTimeForCalls) calls
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
		SELECT c.*
		FROM BvLoginGroup g
		INNER JOIN BvActiveShiftTypeZone a on a.SurveyId=@SurveySID and a.SurveyId=g.SurveySID
		CROSS APPLY [GetPriorityCallByExplicitSidAndShiftTypeId](g.PersonSID, a.Id, @SurveySID, @SuitableTimeForCalls, @FixeNumberCallsPerPerson) c
		WHERE g.PersonSid = g.ObjectSID
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
PRINT N'Creating [dbo].[BvSpGetCallsSentToDialerDistribution]...';


GO
/* Stored procedure dbo.BvSpGetCallsSentToDialerDistribution returns a breakdown of calls sent to dialer per ExplicitSid ( user/group )
   for 20 min starting from @StartTime for a specified suurvey @SurveySid */
CREATE PROCEDURE [dbo].[BvSpGetCallsSentToDialerDistribution]
	@StartTime DATETIME = NULL,                      -- expects UTC time
	@SurveySid INT,
	@timezoneId INT
AS
 
DECLARE @tableStartTime AS DateTime,
		@tableEndTime AS DateTime,
		@cols         AS NVARCHAR(MAX),
		@sql         AS NVARCHAR(MAX),
		@bias         AS INT;
	
IF(@StartTime IS NULL)
BEGIN
	SET @tableEndTime = GETUTCDATE();
	SET @tableStartTime =  (SELECT TOP(1) * 
							FROM(SELECT DISTINCT TOP(20) [Time] 
								 FROM [BvCallsSentToDialer] 
							     WHERE SurveySid = @SurveySid AND [Time] <= @tableEndTime  ORDER BY [Time] desc) AS r 
							ORDER BY r.[Time] ASC)
END
ELSE 
BEGIN
	SET @tablestartTime = @StartTime;
	SET @tableEndTime =  (SELECT TOP(1) * 
						  FROM (SELECT DISTINCT TOP(20) [Time] 
							    FROM [BvCallsSentToDialer] 
							    WHERE SurveySid = @SurveySid AND [Time] >= @tableStartTime ORDER BY [Time]) AS r 
						  ORDER BY r.[Time] DESC);
END
 

SELECT  @bias = DATEDIFF( [mi], @tablestartTime,  dbo.UTC2LT( @tablestartTime, Bias, DaylightType,
							StandardDayOfWeek, StandardStart, StandardBias,
							DaylightDayOfWeek, DaylightStart, DaylightBias ))
FROM  [BvTimezone]
WHERE [ID] = @timezoneId

;WITH timeList AS
(
	SELECT DISTINCT TOP(20) [Time] 
	FROM [BvCallsSentToDialer] 
	WHERE SurveySid= @SurveySid AND [Time] >= @tableStartTime ORDER BY [Time]
)
SELECT  
@cols = STUFF((SELECT N', ' +  QUOTENAME(CONVERT(nvarchar(max),  DATEADD( mi, @bias, [Time]), 21)) FROM timeList FOR XML PATH('')), 1, 2, '');
 
-- Construct the full T-SQL statement and execute dynamically. Query could look like this
/*
SELECT *
FROM (SELECT ISNULL ( g.Name,'*Survey Assignment*') as [Group/User Name], convert(char(5), DATEADD( mi,-300, [time]), 108) AS [minutes], [CallsCount]
          FROM dbo.BvActiveCallsInfo LEFT JOIN ( SELECT SID, Name FROM BvPerson UNION SELECT SID, Name from BvPersonGroup ) as g on ExplicitSid = g.SID 
          where surveysid=1 and [time] >='Dec  7 2009 11:50AM' and [time] <='Dec  7 2009 12:10PM' ) AS D
  PIVOT(MAX(CallsCount) FOR minutes IN([07:00],[07:01],[07:02],[07:03],[07:04],[07:05],[07:07],[07:08],[07:09],[07:10]) )  as  P order by [Group/User Name] ;
*/
SET @sql = N'SELECT *
FROM (SELECT ISNULL ( g.[Name],' + '''' + '*Survey Assignment*' + '''' + ') as [Group/User name], 
			 CONVERT(nvarchar(max), DATEADD( mi,' + CAST( @bias AS VARCHAR(MAX)) + ', [time]), 21) AS [requestTime], 
			 [CallsCount]
          FROM [dbo].[BvCallsSentToDialer]
          LEFT JOIN ( 
			  SELECT [SID], [Name] FROM [BvPerson]
			  UNION 
			  SELECT [SID], [Name] from [BvPersonGroup] ) as g on [ExplicitSid] = g.[SID] 
			  WHERE [SurveySID]=' + CAST( @SurveySid  AS VARCHAR(32))+ ' AND [Time] >=' + '''' + 
			  + Convert(nvarchar(max), @tableStartTime , 21) + '''' + ' AND [Time] <=' + '''' + 
			  Convert(nvarchar(max), @tableEndTime , 21) + '''' + ') AS D
  PIVOT( MAX([CallsCount]) FOR [requestTime] IN(' + @cols + N') ) AS P ORDER BY [Group/User name];';
  
EXEC sp_executesql @sql;
GO
PRINT N'Update complete.';


GO
