CREATE procedure [dbo].[BvSpGetObjectsPage]
 @PageIndex int,
 @PageSize int,
 @OrderField nvarchar(64),
 @IsOrderASC bit,
 @Query nvarchar(MAX),
 @IDField nvarchar(64),
 @SearchCondition NVARCHAR(4000) = NULL,
 @CounterQuery NVARCHAR (MAX) = NULL
as
	DECLARE @StartIndex INT
	DECLARE @TotalCount INT
	IF @PageSize != 2147483647
	BEGIN
		SET @StartIndex = (@PageIndex - 1) * @PageSize + 1
	END
	exec @TotalCount = BvSpGetObjectsRange @StartIndex, @PageSize, @OrderField, @IsOrderASC, @Query, @IDField, @SearchCondition, @CounterQuery
	return @TotalCount