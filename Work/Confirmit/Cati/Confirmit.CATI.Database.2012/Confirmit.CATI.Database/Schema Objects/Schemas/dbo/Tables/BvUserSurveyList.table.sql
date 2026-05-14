CREATE TABLE BvUserSurveyList
(
	UserName NVARCHAR(255) NOT NULL,
	ListType TINYINT NOT NULL,
	SurveyId INT NOT NULL,
	AddedTime DATETIME NOT NULL,
	CONSTRAINT [PK_BvUserSurveyList_UserName_SurveyId] PRIMARY KEY (UserName, ListType, SurveyId )
)
 
GO

CREATE INDEX IX_BvUserSurveyList_UserName_AddedTime  ON BvUserSurveyList(UserName, ListType, AddedTime )