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
	 CAST(0 as TINYINT) AS DialTypeId
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
	 [BvPerson].[DialTypeId]
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
PRINT N'Creating [dbo].[BvSpPersonDialType_Update]...';


GO
CREATE PROCEDURE [dbo].[BvSpPersonDialType_Update]
    @Qualifier VARCHAR(900),
    @DialTypeId tinyint
AS

SET NOCOUNT ON

DECLARE @Persons TABLE( ID INT) 
INSERT INTO @Persons SELECT Item FROM dbo.utilSplitNumbers(@Qualifier, ',')

UPDATE BvPerson
SET DialTypeId = @DialTypeId
WHERE SID IN
(
	SELECT pg.SID FROM @Persons persons inner JOIN BvViewPersonAndGroup pg ON persons.ID = pg.SID
	WHERE IsGroup = 0
	UNION ALL
	SELECT p.SID FROM @Persons persons 
	INNER JOIN BvViewPersonAndGroup pg ON persons.ID = pg.SID
	INNER JOIN BvMembership on pg.SID = BvMembership.ContainerSID
	INNER join BvPerson p on p.SID = BvMembership.ObjectSID
	WHERE IsGroup = 1
)
GO
PRINT N'Update complete.';


GO
