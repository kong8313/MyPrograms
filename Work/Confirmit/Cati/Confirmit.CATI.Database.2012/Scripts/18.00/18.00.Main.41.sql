PRINT N'Altering [dbo].[BvSpGetSurveyCallCenterAssignmentPage]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetSurveyCallCenterAssignmentPage]
    @CallCenterId INT,
    @UserName NVARCHAR (255),
    @PageIndex INT,
    @PageSize INT, 
    @OrderField NVARCHAR (64), 
    @IsOrderASC BIT, 
    @SearchCondition NVARCHAR (4000)=NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @PageIndex IS NULL AND @PageSize IS NULL
        BEGIN
        /* Looks like we're generating code using FMTONLY. So lets return metadata*/
         SELECT  
             0 AS SurveyId,
			 '' AS ProjectId,
             '' AS SurveyName, 
             '' AS CallCenterNames
     
        RETURN 0;
    END

	DECLARE @Query as nvarchar(4000)
	DECLARE @IDField as nvarchar(64)
	SET @IDField = 'SurveyId'
	SET @Query =
	'SELECT 
        s.SID as SurveyId,
        s.Name as ProjectId,
        s.Description as SurveyName,
        Stuff( (SELECT '', '' +  Name FROM BvCallCenter cs INNER JOIN BvSurveyAssignmentOnCallCenter a ON cs.ID = a.CallCenterID
        WHERE a.SurveyId = s.SID
        FOR XML PATH('''') ), 1, 2, '''' ) as CallCenterNames
        FROM BvSurvey s 
	    INNER JOIN BvUserSurveyPermission perm ON (perm.[UserName] = '''+@UserName+''' AND perm.[SurveySID] = s.[SID])
        WHERE s.State <> 2'

	IF @CallCenterId IS NOT NULL 
	BEGIN
        IF @SearchCondition IS NOT NULL AND LEN(@SearchCondition) > 0
		    SET @SearchCondition = @SearchCondition + ' AND '
	    
		SET @SearchCondition = @SearchCondition + ' SurveyId IN (SELECT SurveyID FROM BvSurveyAssignmentOnCallCenter WHERE CallCenterID = ' + CAST(@CallCenterId AS nvarchar) + ')'
	END

    DECLARE @TotalCount INT
    exec @TotalCount = BvSpGetListPage @PageIndex, @PageSize, @OrderField, @IsOrderASC, @Query, @IDField, @SearchCondition
    RETURN @TotalCount
END
GO

PRINT N'Altering [dbo].[BvSpSurvey_ListPage]...';

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
        INNER JOIN BvUserSurveyPermission ON ( BvUserSurveyPermission.UserName = '''+@userName+''' AND
                                              BvUserSurveyPermission.SurveySID = BvSurvey.SID)
        LEFT JOIN (SELECT COUNT(*) as Count, SurveySID FROM BvInterview group by SurveySid ) as sample on BvSurvey.SID = sample.SurveySID 
        WHERE BvSurvey.State <> 2'

DECLARE @TotalCount INT
exec @TotalCount = BvSpGetListPage @PageNumber, @PageSize, @OrderField, @IsOrderASC, @Query, @IDField, @SearchCondition
RETURN @TotalCount
GO
PRINT N'Update complete.';


GO
