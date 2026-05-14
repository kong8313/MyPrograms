CREATE PROCEDURE [dbo].[BvSpUserSurveyList_Insert]

 @UserName NVARCHAR(255),
 @ListType TINYINT,
 @SurveyId INT
AS

;MERGE BvUserSurveyList AS t
	USING( SELECT @UserName as UserName, @ListType as ListType, @SurveyId as SurveyId) as s
		ON t.UserName = s.UserName AND t.ListType = s.ListType AND t.SurveyId = s.SurveyId
	WHEN MATCHED THEN 
		UPDATE SET t.AddedTime = GETUTCDATE()
	WHEN NOT MATCHED BY TARGET THEN 
		INSERT(UserName, ListType, SurveyId, AddedTime) VALUES( @UserName, @ListType, @SurveyId, GETUTCDATE() );
