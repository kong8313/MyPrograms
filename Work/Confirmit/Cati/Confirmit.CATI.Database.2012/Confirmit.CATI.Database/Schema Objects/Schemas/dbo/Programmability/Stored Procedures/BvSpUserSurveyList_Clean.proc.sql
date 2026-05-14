CREATE PROCEDURE [dbo].[BvSpUserSurveyList_Clean]

 @MaxAddedTime DATETIME
AS
	DELETE FROM BvUserSurveyList WHERE AddedTime < @MaxAddedTime
