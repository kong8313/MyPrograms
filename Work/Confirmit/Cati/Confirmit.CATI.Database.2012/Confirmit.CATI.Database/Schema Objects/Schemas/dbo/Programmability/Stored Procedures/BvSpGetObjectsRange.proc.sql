CREATE PROCEDURE [dbo].[BvSpGetObjectsRange]
@StartIndex INT, 
@ObjectCount INT, 
@OrderField NVARCHAR (64), 
@IsOrderASC BIT, 
@Query NVARCHAR (MAX), 
@IDField NVARCHAR (64), 
@SearchCondition NVARCHAR (4000)=NULL,
@CounterQuery NVARCHAR (MAX) = NULL
AS
if @OrderField = ''
  set @OrderField = 'ID'

  DECLARE @TotalCount INT
  exec @TotalCount = BvSpGetListRange @StartIndex, @ObjectCount, @OrderField, @IsOrderASC, @Query, @IDField, @SearchCondition, @CounterQuery
  return @TotalCount