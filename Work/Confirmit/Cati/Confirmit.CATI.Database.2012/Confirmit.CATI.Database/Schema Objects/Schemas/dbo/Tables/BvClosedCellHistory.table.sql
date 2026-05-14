CREATE TABLE [dbo].[BvClosedCellHistory]
(
	Id INT IDENTITY(1, 1) NOT NULL,
	ClosingTime DATETIME NOT NULL,
	SurveySid INT NOT NULL,
	QuotaId INT NOT NULL,
	CellId INT NOT NULL,
	GeneratedWhereForCell NVARCHAR(MAX) NOT NULL
)
