CREATE  procedure [dbo].[BvSpSurvey_GetListByFolder]
 @UserName NVARCHAR(MAX) = NULL,
 @Filter NVARCHAR(MAX) = NULL,
 @CallCenterId INT

as

SELECT  
        BvSurvey.SID    AS    [SID],
        BvSurvey.Name   AS    [ConfirmitID],
        BvSurvey.Description AS [Name], 
  (select count(distinct BvPersonOrGroupAssignmentOnSurvey.PersonOrGroupId) from BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID) BvPersonOrGroupAssignmentOnSurvey
        where BvPersonOrGroupAssignmentOnSurvey.SurveyId = BvSurvey.[SID]) 
         as TotalAssignedPersons 
FROM    [BvFnSurvey_GetByCallCenterId](@CallCenterId) [BvSurvey]
INNER JOIN [bvUserSurveyPermission] [p] ON BvSurvey.SID = [p].SurveySID
WHERE  p.UserName = @UserName AND 
       BvSurvey.[Description] <> '' AND 
       (@Filter IS NULL OR BvSurvey.[Description] LIKE @Filter + '%') AND
	   BvSurvey.State <> 2