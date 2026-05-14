PRINT N'Altering [dbo].[BvSpUserSurveyList_Get]...';


GO
ALTER PROCEDURE [dbo].[BvSpUserSurveyList_Get]

 @UserName NVARCHAR(255),
 @ListType TINYINT,
 @CallCenterId INT
AS

SELECT TOP(20) sc.SID, sc.Name as ProjectId, sc.Description as Name FROM [BvUserSurveyList] usl
	INNER JOIN BvUserSurveyPermission usp
		ON usl.SurveyId = usp.SurveySID AND usp.UserName = @UserName
	INNER JOIN BvFnSurvey_GetByCallCenterId(@CallCenterId) sc
	ON usl.SurveyId = sc.SID
	WHERE usl.UserName = @UserName AND usl.ListType = @ListType AND sc.State != 2
	ORDER BY usl.AddedTime DESC
GO
PRINT N'Update complete.';


GO
