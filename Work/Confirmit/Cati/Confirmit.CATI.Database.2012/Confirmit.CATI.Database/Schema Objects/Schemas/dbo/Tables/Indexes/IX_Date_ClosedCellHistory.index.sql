CREATE NONCLUSTERED INDEX [IX_Date_ClosedCellHistory]
    ON [dbo].[BvClosedCellHistory]
	(SurveySid, ClosingTime)
	INCLUDE
	(GeneratedWhereForCell)
