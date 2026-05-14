GO
PRINT N'Altering [dbo].[BvSpGetPersonsListPage]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetPersonsListPage]
 @ParentGroupsIDs NVARCHAR (MAX), 
 @PageIndex INT,
 @PageSize INT, 
 @OrderField NVARCHAR (64), 
 @IsOrderASC BIT, 
 @SearchCondition NVARCHAR (4000)=NULL,
 @CallCenterId INT
AS
BEGIN
 IF @ParentGroupsIDs IS NULL AND @PageIndex IS NULL
 BEGIN
 /* Looks like we're generating code using FMTONLY. So lets return metadata*/
 SELECT
     0  AS PersonSID,
     '' AS PersonName,
     '' AS PersonDescription,
     CAST(0 as BIT)  AS LoggedIn,
     0 as ManualSelection,
     0 as AllowedChoices,
     '' as SurveyID,
     CAST(0 as BIT)  AS IsLocked,
     CAST(NULL AS DATETIME) AS LockedDate,
	 0 AS CallGroupId,
	 '' AS CallGroupName,
     '' AS PersonLocation,
	 CAST(0 as TINYINT) AS DialTypeId,
	 CAST(0 as TINYINT) AS [Type],
	 '' AS GroupNamesJson
     RETURN 0;
 END
 
 DECLARE @Query nvarchar(max)
 DECLARE @IDField nvarchar(64)
 SET @IDField = 'PersonSID'
   
 SET @Query = 
   N'SELECT DISTINCT [BvPerson].[SID] PersonSID,
    [BvPerson].[Name] PersonName,
    [BvPerson].[Description] PersonDescription,
    cast((case when t.[PersonSID] is null 
       then 0
       else 1 
    end) as bit) as [LoggedIn],
    [BvPerson].[ManualSelection] as ManualSelection,
    [BvPerson].[AllowedChoices] as AllowedChoices,
    ISNULL ( s.Name, '''' ) as [SurveyID],
    [BvPerson].[IsLocked] as IsLocked,
    [BvPerson].[LockedDate] as LockedDate,
	[BvCallGroup].[ID] as CallGroupId,
	ISNULL( [BvCallGroup].[Name], '''' ) as CallGroupName,
	[BvPerson].[Location] as PersonLocation,
	[BvPerson].[DialTypeId],
	[BvPerson].[Type],
	''['' + (
		SELECT ''"'' + BvPersonGroup.Name + ''",'' AS [text()]
		FROM [BvPersonGroup]
		INNER JOIN [BvMembership] ON BvPersonGroup.SID = BvMembership.ContainerSID
		WHERE BvMembership.ObjectSID = BvPerson.SID
		ORDER BY BvMembership.ObjectSID
		FOR XML PATH ('''')
	) + '']'' as GroupNamesJson
    FROM BvFnPerson_Get(' + CAST( @CallCenterId AS NVARCHAR(64)) + ') as BvPerson
	LEFT JOIN [BvCallGroup]
	  ON [BvPerson].[CallGroupID] = [BvCallGroup].ID
    LEFT JOIN [dbo].[BvMembership]
      ON [BvMembership].[ObjectSID] = [BvPerson].[SID]
    LEFT JOIN dbo.BvTasks t
      on [BvPerson].SID = t.PersonSID
    LEFT JOIN  dbo.bvsurvey s
       on s.SID = t.SurveySID and s.State <> 2
    WHERE [BvPerson].[SID] = [BvMembership].[ObjectSID] 
    AND [BvMembership].[ContainerSID] in (' + @ParentGroupsIDs + ')'
   
   IF @OrderField = '' OR @OrderField = null
   SET @OrderField = 'PersonSID' 
   
   DECLARE @TotalCount INT

   EXEC @TotalCount = BvSpGetObjectsPage @PageIndex, @PageSize, @OrderField, @IsOrderASC, @Query, @IDField, @SearchCondition
   RETURN @TotalCount
END
GO
PRINT N'Update complete.';


GO
