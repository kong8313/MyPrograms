GO
PRINT N'Altering [dbo].[BvSpGetCatiPersonSessionHistory]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetCatiPersonSessionHistory]
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
	DECLARE @PersonIdsToKeep nvarchar(max) = ''

	IF @personIds IS NOT NULL AND LEN(@personIds) > 0
	BEGIN
		SET @PersonIdsToKeep = ' WHERE SID NOT IN('+@personIds+')'
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

	DECLARE @requestToBvPerson nvarchar(max) = N'SELECT SID, Name INTO #CatiPerson 
	FROM FUSIONLINKEDSERVER.ConfirmitCATIV15_' + CONVERT(varchar(9), @CompanyId)+'.dbo.BvPerson '

	IF LEN(@PersonIdsToKeep) > 0
	BEGIN
		SET @requestToBvPerson = @requestToBvPerson +' DELETE FROM #CatiPerson ' + @PersonIdsToKeep
	END

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

	SET @remoteQuery = @requestToBvPerson + @remoteQuery + ' ' + @countQuery

	DECLARE @queryLog NVARCHAR(MAX) = @DatabaseName + '.sys.sp_executesql @remoteQuery, N''@count int out'',@count = @count output'

	EXEC sp_executesql @queryLog, N'@remoteQuery NVARCHAR(MAX), @count int output', @remoteQuery, @count = @count output
	
	RETURN @count
RETURN 0
GO

PRINT N'Update complete.';


GO
