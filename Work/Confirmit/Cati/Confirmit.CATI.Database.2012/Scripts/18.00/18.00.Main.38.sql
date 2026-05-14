GO
ALTER PROCEDURE [dbo].[BvSpSurvey_ListPage]
@CallCenterId INT,
@PageNumber INT, 
@PageSize INT, 
@OrderField NVARCHAR (64), 
@IsOrderASC INT, 
@userName NVARCHAR (255), 
@userID INT=0, 
@accessMask INT=2147483647, 
@SearchCondition NVARCHAR (4000)=NULL
AS
SET NOCOUNT ON

 IF @PageNumber IS NULL AND @PageSize IS NULL
 BEGIN
 /* Looks like we're generating code using FMTONLY. So lets return metadata*/

 SELECT  
        0 AS SID,
        '' AS Name, 
        0 AS SampleSize,
        0 AS State,
        '' AS Description,
		cast(0 as tinyint) AS DialMode
     
     RETURN 0;
 END

DECLARE @Query as nvarchar(4000)
DECLARE @IDField as nvarchar(64)
SET @IDField = 'SID'
SET @Query =
    'SELECT  
        BvSurvey.SID        SID,
        BvSurvey.Name       Name, 
        ISNULL(sample.Count, 0) SampleSize,
        BvSurvey.State,
        BvSurvey.Description,
		BvSurvey.DialMode
        FROM    BvFnSurvey_GetByCallCenterId(' + cast(@CallCenterId AS NVARCHAR) + ') BvSurvey
        LEFT JOIN BvUserSurveyPermission ON ( BvUserSurveyPermission.UserName = '''+@userName+''' AND
                                              BvUserSurveyPermission.SurveySID = BvSurvey.SID)
        LEFT JOIN (SELECT COUNT(*) as Count, SurveySID FROM BvInterview group by SurveySid ) as sample on BvSurvey.SID = sample.SurveySID 
        WHERE
                  ((BvUserSurveyPermission.UserName IS NOT NULL) OR ('''+@userName+''' = '''')) AND BvSurvey.State <> 2'

DECLARE @TotalCount INT
exec @TotalCount = BvSpGetListPage @PageNumber, @PageSize, @OrderField, @IsOrderASC, @Query, @IDField, @SearchCondition
RETURN @TotalCount
