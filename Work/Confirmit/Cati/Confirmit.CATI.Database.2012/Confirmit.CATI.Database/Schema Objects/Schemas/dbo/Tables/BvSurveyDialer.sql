CREATE TABLE [dbo].[BvSurveyDialer]
(
	[SurveyId] INT NOT NULL,
	[DialTypeId] TINYINT NOT NULL,
	[DialerId] INT NULL,
	CONSTRAINT PK_BvSurveyDialer_SurveyId_DialTypeId PRIMARY KEY CLUSTERED (SurveyId, DialTypeId)
)
