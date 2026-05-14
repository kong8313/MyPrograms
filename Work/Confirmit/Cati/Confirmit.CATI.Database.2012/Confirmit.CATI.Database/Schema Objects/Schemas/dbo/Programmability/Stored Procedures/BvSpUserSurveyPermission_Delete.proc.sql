CREATE PROCEDURE [dbo].[BvSpUserSurveyPermission_Delete]
   @UserName nvarchar( 255 ),
   @SurveyName nvarchar( 255 ) = NULL
AS
   DELETE BvUserSurveyPermission
   WHERE  UserName = @UserName AND
      ((@SurveyName IS NULL) OR (SurveySID = (SELECT SID
                                              FROM BvSurvey
                                              WHERE Name = @SurveyName)))