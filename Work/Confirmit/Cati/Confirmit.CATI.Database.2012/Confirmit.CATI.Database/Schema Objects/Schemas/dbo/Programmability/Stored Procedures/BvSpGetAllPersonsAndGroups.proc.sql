CREATE  PROCEDURE [dbo].[BvSpGetAllPersonsAndGroups]
 @CallCenterId INT,
 @SurveyIdForExcludeAssignment INT,
 @IncludeAdministrativeGroups BIT = 1
 AS

IF @CallCenterId IS NULL
 BEGIN
 /* Return metadata*/
 SELECT
     0  AS Id,
     '' AS Name,     
     '' as [Description],
     CAST(0 as BIT)  AS IsGroup     
     RETURN 0;
 END
 
 SELECT d.* FROM ( 
  SELECT SID as Id, Name, Description, CAST( 0 AS BIT ) as IsGroup FROM BvFnPerson_Get(@CallCenterId)
  UNION
  SELECT SID as Id, Name, Description, CAST(1 AS BIT ) as IsGroup FROM BvPersonGroup pg WHERE @IncludeAdministrativeGroups = 1 OR pg.IsAdministrative = 0) d
 LEFT JOIN BvPersonOrGroupAssignmentOnSurvey pga 
 ON d.Id = pga.PersonOrGroupId AND 
    pga.SurveyId = @SurveyIdForExcludeAssignment AND 
    pga.CallCenterId = @CallCenterId
 WHERE pga.Id IS NULL OR @SurveyIdForExcludeAssignment IS NULL


