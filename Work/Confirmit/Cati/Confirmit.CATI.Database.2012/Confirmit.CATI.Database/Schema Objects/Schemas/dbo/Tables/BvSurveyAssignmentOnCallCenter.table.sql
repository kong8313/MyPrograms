CREATE TABLE  BvSurveyAssignmentOnCallCenter
(
	SurveyId INT,
	CallCenterId INT,
	CONSTRAINT PK_BvSurveyAssignmentOnCallCenter PRIMARY KEY ( SurveyId, CallCenterId ) WITH ( IGNORE_DUP_KEY = ON )
)
