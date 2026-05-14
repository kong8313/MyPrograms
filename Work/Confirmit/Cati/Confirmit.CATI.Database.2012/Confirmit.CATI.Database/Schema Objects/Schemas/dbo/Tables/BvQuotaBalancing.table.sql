CREATE TABLE [dbo].[BvQuotaBalancing]
(
	surveyId INT NOT NULL,
	[quotaId] INT NOT NULL,
	[quotaName] NVARCHAR(256) NOT NULL,
	priority INT  NOT NULL CONSTRAINT DF_BvQuotaBalancing_priority DEFAULT(500),
	promotionThreshold INT NOT NULL,
	promotionCoefficient REAL NOT NULL CONSTRAINT DF_BvQuotaBalancing_promotionCoefficient DEFAULT(0.8),
	CONSTRAINT PK_BvQuotaBalancing PRIMARY KEY CLUSTERED (surveyId, quotaId),
	CONSTRAINT FK_BvQuotaBalancing_surveyId FOREIGN KEY ([surveyId]) REFERENCES [dbo].[BvSurvey] ([SID]) ON DELETE CASCADE 
)
