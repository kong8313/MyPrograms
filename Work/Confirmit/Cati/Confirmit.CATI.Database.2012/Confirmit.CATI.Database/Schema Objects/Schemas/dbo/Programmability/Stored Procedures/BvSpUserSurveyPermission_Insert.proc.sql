CREATE PROCEDURE [dbo].[BvSpUserSurveyPermission_Insert]
   @UserName nvarchar( 255 ),
   @SurveyName nvarchar( 255 )
AS
   INSERT INTO BvUserSurveyPermission(UserName, SurveySID)
   SELECT @UserName, SID
   FROM BvSurvey
   WHERE Name = @SurveyName AND
         NOT EXISTS( SELECT * 
                     FROM BvUserSurveyPermission
                     WHERE UserName = @UserName AND
                           SurveySID = SID)