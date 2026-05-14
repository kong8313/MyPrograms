CREATE PROCEDURE [dbo].[BvSpGetListRange]
@StartIndex INT, 
@ObjectCount INT, 
@OrderField NVARCHAR (64), 
@IsOrderASC INT, 
@Query NVARCHAR (MAX), 
@IDField NVARCHAR (64), 
@SearchCondition NVARCHAR (4000)=NULL,
@CounterQuery NVARCHAR (MAX) = NULL
AS

print @StartIndex
print @ObjectCount
print @OrderField
print @IsOrderASC
print @Query
print @IDField
print @SearchCondition

IF (@SearchCondition IS NOT NULL AND @SearchCondition <> '')
BEGIN
 SET @Query = 'SELECT * FROM (' + @Query + ') t WHERE ' + @SearchCondition
END

DECLARE @TotalCount INT
DECLARE @CountQuery NVARCHAR(MAX)

IF(@CounterQuery IS NOT NULL)
	SET @CountQuery = N'with T as (' + @CounterQuery + ') select @TotalCountOut = cnt from T'
ELSE
	SET @CountQuery = N'with T as (' + @Query + ') select @TotalCountOut = count(1) from T'

EXEC sp_executesql @CountQuery, N'@TotalCountOut int output', @TotalCountOut = @TotalCount output
  
DECLARE @OrderClause AS NVARCHAR(500)
DECLARE @OrderDirection AS NVARCHAR(6)

IF (@IsOrderASC = 1)
BEGIN
   SET @OrderDirection = ' ASC '
END
ELSE
BEGIN
   SET @OrderDirection = ' DESC '
END

IF (UPPER(@OrderField) != UPPER(@IDField))
BEGIN
    SET @OrderClause = ' ORDER BY ' + @OrderField + @OrderDirection + ',' + @IDField + @OrderDirection
END
ELSE
BEGIN
    SET @OrderClause = ' ORDER BY ' + @OrderField + @OrderDirection
END

DECLARE @SQL AS NVARCHAR(MAX)
IF @ObjectCount = 2147483647
BEGIN
    -- request all records
    SET @SQL = 'SELECT * FROM (' + @Query + ') S ' + @OrderClause
END
ELSE
BEGIN
    SET @SQL = 'SELECT * FROM (SELECT S.*, ROW_NUMBER() OVER(' + @OrderClause + ') AS SpecialTempRowNumberForPaging
      FROM (' + @Query + ') S ) S
      WHERE SpecialTempRowNumberForPaging BETWEEN ' + STR(@StartIndex) + ' AND ' + STR(@StartIndex + @ObjectCount - 1) +
      @OrderClause
END

EXEC sp_executesql @SQL
RETURN ISNULL(@TotalCount, 0)