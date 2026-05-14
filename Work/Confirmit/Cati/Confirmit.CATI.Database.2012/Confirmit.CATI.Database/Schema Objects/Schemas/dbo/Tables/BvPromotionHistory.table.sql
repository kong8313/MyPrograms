CREATE TABLE [dbo].[BvPromotionHistory]
(
	ID INT IDENTITY(1, 1) NOT NULL,
	SurveyId INT NOT NULL,
	FiredTime DATETIME NOT NULL,
	CallsToPromoteCount INT NOT NULL,
	PromotedCallsCount INT NOT NULL,
	CellId INT NOT NULL,
	CellInfo NVARCHAR(MAX) NOT NULL,
	QuotaName NVARCHAR(256) NOT NULL
	
)
