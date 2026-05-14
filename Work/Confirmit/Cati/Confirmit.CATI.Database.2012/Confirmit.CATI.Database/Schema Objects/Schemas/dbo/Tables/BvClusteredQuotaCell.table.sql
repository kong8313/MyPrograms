CREATE TABLE [dbo].[BvClusteredQuotaCell]
(
    [SurveyId] INT NOT NULL,
	[CellId] INT NOT NULL,
	Name NVARCHAR(255) NOT NULL,
	LiveCount INT NOT NULL,
	LiveLimit INT NOT NULL
)
