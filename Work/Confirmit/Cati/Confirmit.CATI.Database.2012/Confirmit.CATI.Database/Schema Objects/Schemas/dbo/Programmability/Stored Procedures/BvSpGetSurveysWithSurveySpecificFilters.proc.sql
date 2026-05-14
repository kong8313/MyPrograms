CREATE PROCEDURE [dbo].[BvSpGetSurveysWithSurveySpecificFilters]
  @userName NVARCHAR (255)
AS
SELECT 
  [s].[SID] as [SurveySid],
  [s].[Name] as [ProjectId],
  [s].[Description] as [ProjectName],
  COUNT(*) as [FiltersCount]
FROM [BvSurvey] [s]
INNER JOIN [BvUserSurveyPermission] [p] on [s].[SID] = [p].SurveySID
INNER JOIN [BvFilters] [f] ON [s].[SID] = [f].[SurveySID]
WHERE [s].[State] != 2 -- Exclude soft-deleted surveys
	AND [p].[UserName] = @userName
GROUP BY [s].[SID], [s].[Name], [s].[Description]