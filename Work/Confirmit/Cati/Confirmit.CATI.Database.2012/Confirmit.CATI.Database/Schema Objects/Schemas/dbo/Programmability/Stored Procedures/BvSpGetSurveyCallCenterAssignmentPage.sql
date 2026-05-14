CREATE PROCEDURE [dbo].[BvSpGetSurveyCallCenterAssignmentPage]
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
