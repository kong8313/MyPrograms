GO
PRINT N'Dropping [dbo].[BvSpInterviewerBreaksReport]...';


GO
DROP PROCEDURE [dbo].[BvSpInterviewerBreaksReport];


GO
PRINT N'Creating [dbo].[BvSpInterviewerSessionsReport]...';


GO
CREATE PROCEDURE [dbo].[BvSpInterviewerSessionsReport]
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
	IF @loginTotalCount != 0
	BEGIN
		SET @TotalCount = @TotalCount + (@loginTotalCount - (SELECT COUNT(*) FROM #CatiPersonSessionHistory))
	END
    RETURN @TotalCount
RETURN 0
GO
PRINT N'Update complete.';


GO
