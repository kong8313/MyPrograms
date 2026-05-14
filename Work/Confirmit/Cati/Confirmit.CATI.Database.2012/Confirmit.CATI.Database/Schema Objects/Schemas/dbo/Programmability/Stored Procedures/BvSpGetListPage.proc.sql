CREATE  PROCEDURE [dbo].[BvSpGetListPage]
@PageNumber INT, 
@PageSize INT, 
@OrderField NVARCHAR (64), 
@IsOrderASC INT,
@Query NVARCHAR (MAX), 
@IDField NVARCHAR (64), 
@SearchCondition NVARCHAR (4000)=NULL
AS
 DECLARE @StartIndex INT
 IF @PageSize != 2147483647
 BEGIN
  SET @StartIndex = (@PageNumber - 1) * @PageSize + 1
 END
 
 DECLARE @TotalCount INT
 exec @TotalCount = BvSpGetListRange @StartIndex, @PageSize, @OrderField, @IsOrderASC, @Query, @IDField, @SearchCondition
 RETURN @TotalCount