CREATE PROCEDURE [dbo].[BvSpGetSurveys]
 @Filter NVARCHAR(MAX) = NULL,
 @UserName NVARCHAR(MAX) = NULL,
 @CallCenterId INT
AS
SELECT DISTINCT
 [s].[SID] AS [SID],
 [s].[Name] AS [ConfirmitID],
 [s].[Description] AS [Name]
FROM    [BvFnSurvey_GetByCallCenterId](@CallCenterId) [s] 
left join [bvUserSurveyPermission] [p] on [s].[SID] = [p].[SurveySID]
WHERE
     ( p.UserName = @UserName or @UserName is null)
 AND (@Filter IS NULL OR [s].[Description] LIKE @Filter)
 AND ( s.State <> 2)