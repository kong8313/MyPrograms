GO
PRINT N'Creating [dbo].[BvSpGetCatiPersonSessionHistory]...';


GO
CREATE PROCEDURE [dbo].[BvSpGetCatiPersonSessionHistory]
    @personIds NVARCHAR(MAX),
	@SearchCondition NVARCHAR(MAX),
	@PageIndex INT,
	@PageSize INT,
	@OrderField NVARCHAR(64),
	@IsOrderASC BIT,
	@DatabaseName nvarchar(max),
	@CallCenterId int,
	@CompanyId int
AS
	IF @personIds IS NULL AND @PageIndex IS NULL AND @PageSize IS NULL
	BEGIN
		SELECT  '' AS PersonName,
				CAST(NULL AS DATETIME) AS StartTime,
				CAST(NULL AS DATETIME) AS FinishTime,
				0 AS Duration,
				0 as Event
		WHERE 1 = 0
		RETURN 0;
	END

	DECLARE @count INT = 0
	DECLARE @OrderClause AS NVARCHAR(500)
	DECLARE @OrderDirection AS NVARCHAR(6)
	DECLARE @PersonIdsCondition nvarchar(max) = ''

	IF @personIds IS NOT NULL AND LEN(@personIds) > 0
	BEGIN
		SET @PersonIdsCondition = ' WHERE SID NOT IN('+@personIds+')'
	END

	IF (@IsOrderASC = 1)
	BEGIN
	   SET @OrderDirection = ' ASC '
	END
	ELSE
	BEGIN
	   SET @OrderDirection = ' DESC '
	END

	SET @OrderClause = ' ORDER BY ' + @OrderField + @OrderDirection

	CREATE TABLE #CatiPerson
	(
		[SID] [int] NOT NULL,
		[Name] [nvarchar](255) NOT NULL,
		[FullName] [nvarchar](255) NOT NULL,
		[Description] [nvarchar](255) NOT NULL ,
		[ManualSelection] [int] NOT NULL,
		[PwdHashTxt] [nvarchar](256) NOT NULL,
		[PwdSaltTxt] [nvarchar](256) NOT NULL,
		[HasNewMessage] [bit] NULL,
		[AutomaticSurveyID] [int] NULL,
		[AllowedChoices] [int] NULL,
		[IsLocked] [bit] NOT NULL,
		[LockedDate] [datetime] NULL,
		[AssignmentsListMode] [int] NOT NULL,
		[CallGroupID] [int] NULL,
		[CallCenterID] [int] NOT NULL,
		[Location] [nvarchar](256) NULL,
		[PwdSetDate] [datetime] NOT NULL
	)

	DECLARE @requestToBvPerson nvarchar(max) = N'INSERT INTO #CatiPerson 
	SELECT * FROM FUSIONLINKEDSERVER.ConfirmitCATIV15_' + CONVERT(varchar(9), @CompanyId)+'.dbo.BvPerson '

	IF LEN(@PersonIdsCondition) > 0
	BEGIN
		SET @requestToBvPerson = @requestToBvPerson +' DELETE FROM #CatiPerson ' + @PersonIdsCondition
	END

	print @requestToBvPerson

	declare @top nvarchar(64) = CAST( @PageIndex * @PageSize AS NVARCHAR(64))

	declare @remoteQuery NVARCHAR(MAX) = N'SELECT 
	Name AS PersonName,
	LoginTime AS StartTime, 
	LogoutTime AS FinishTime, 
	DATEDIFF(SECOND,LoginTime,LogoutTime) AS Duration,
	1 as Event
	FROM CatiInterviewerSessionHistory
	INNER JOIN #CatiPerson person ON person.SID = CatiInterviewerSessionHistory.InterviewerId ' +
	'WHERE CatiInterviewerSessionHistory.CallCenterId = ' + CONVERT(varchar(9), @CallCenterId) +
	' AND CatiInterviewerSessionHistory.CompanyId = '+ CONVERT(varchar(9), @CompanyId)
	declare @countQuery NVARCHAR(MAX) = @remoteQuery

	IF LEN(ISNULL(@SearchCondition, '')) > 0
	BEGIN
		SET @countQuery = 'SELECT @count = COUNT(*) FROM ( SELECT * FROM (' + @remoteQuery + ') t WHERE ' + @SearchCondition +' ) c '
		SET @remoteQuery = 'SELECT TOP(' + @top + ') * FROM (' + @remoteQuery + ') t WHERE ' + @SearchCondition + @OrderClause
	END
	ELSE
	BEGIN
		SET @countQuery = 'SELECT @count = COUNT(*) FROM (' + @remoteQuery + ') c'
		SET @remoteQuery = 'SELECT TOP(' + @top + ') * FROM (' + @remoteQuery + ') t ' + @OrderClause
	END

	SET @remoteQuery = @requestToBvPerson + @remoteQuery

	DECLARE @queryLog NVARCHAR(MAX) = @DatabaseName + '.sys.sp_executesql @remoteQuery'
	DECLARE @queryLogCount NVARCHAR(MAX) = @DatabaseName + '.sys.sp_executesql @countQuery, N''@count int out'',@count = @count output'

	EXEC sp_executesql @queryLog, N'@remoteQuery NVARCHAR(MAX)', @remoteQuery = @remoteQuery

	EXEC sp_executesql @queryLogCount, N'@countQuery NVARCHAR(MAX), @count int output', @countQuery, @count = @count output
	
	RETURN @count
RETURN 0
GO
PRINT N'Altering [dbo].[BvSpInterviewerBreaksReport]...';


GO
ALTER PROCEDURE [dbo].[BvSpInterviewerBreaksReport]
    @personIds NVARCHAR(MAX),
	@SearchCondition NVARCHAR(MAX),
	@PageIndex INT,
	@PageSize INT,
	@OrderField NVARCHAR(64),
	@IsOrderASC BIT,
	@DatabaseName nvarchar(max),
	@CallCenterId int,
	@CompanyId int,
	@EventType int
AS
	IF @personIds IS NULL AND @PageIndex IS NULL AND @PageSize IS NULL
	BEGIN
		SELECT  '' AS PersonName,
				CAST(NULL AS DATETIME) AS StartTime,
				CAST(NULL AS DATETIME) AS FinishTime,
				0 AS Duration,
				0 as Event
		WHERE 1 = 0
		RETURN 0;
	END
	
	DECLARE @loginTotalCount int = 0

	IF @EventType != 0
		BEGIN
		CREATE TABLE #CatiPersonSessionHistory
		(
			PersonName nvarchar(max),
			StartTime datetime,
			FinishTime datetime,
			Duration int,
			Event int
		)

		INSERT INTO #CatiPersonSessionHistory
		EXEC @loginTotalCount = BvSpGetCatiPersonSessionHistory @personIds, @SearchCondition, @PageIndex, @PageSize, @OrderField, @IsOrderASC, @DatabaseName, @CallCenterId, @CompanyId
	END
	
	DECLARE @Query NVARCHAR(MAX) = ''
	DECLARE @loginQuery NVARCHAR(MAX) = 'SELECT * FROM #CatiPersonSessionHistory'
	DECLARE @breakQuery NVARCHAR(MAX)= '
	SELECT Name PersonName,
	       StartTime,
		   DATEADD(second, Duration, StartTime) as FinishTime,
		   Duration,
		   0 as Event
	FROM BvTimeBreaksHistory
	INNER JOIN dbo.utilSplitNumbers( ''' + ISNULL(@PersonIds, '') + ''', '','') s1 ON s1.Item = InterviewerId
	INNER JOIN BvPerson ON SID = InterviewerId'
	
	SET @Query = 
        CASE
            WHEN @EventType = -1
                THEN @breakQuery + ' UNION ALL ' + @loginQuery
            WHEN @EventType = 0
                THEN @breakQuery
            WHEN @EventType = 1
                THEN @loginQuery
        END;
	
	DECLARE @TotalCount INT

    EXEC @TotalCount = BvSpGetObjectsPage @PageIndex, @PageSize, @OrderField, @IsOrderASC, @Query, @OrderField, @SearchCondition
	IF @loginTotalCount != 0 AND @PageSize < @loginTotalCount
	BEGIN
		SET @TotalCount = @TotalCount + (@loginTotalCount - (SELECT COUNT(*) FROM #CatiPersonSessionHistory))
	END
    RETURN @TotalCount
RETURN 0
GO
PRINT N'Update complete.';


GO
