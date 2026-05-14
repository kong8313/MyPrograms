CREATE TABLE [dbo].[BvQuotaFilter]
(
	surveyId INT NOT NULL,
	FieldName NVARCHAR(MAX) NOT NULL,
	CONSTRAINT FK_BvQuotaFilter_surveyId FOREIGN KEY ([surveyId]) REFERENCES [dbo].[BvSurvey] ([SID]) ON DELETE CASCADE
)
