CREATE PROCEDURE [dbo].[BvSpUserSurveyPermission_Get]
   @UserName nvarchar( 255 )
AS
   SELECT SurveySID
   FROM BvUserSurveyPermission
   WHERE UserName = @UserName