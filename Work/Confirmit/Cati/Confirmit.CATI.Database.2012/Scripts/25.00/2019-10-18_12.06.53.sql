GO
PRINT 'Add new system settigs: Surveys.DefaultCallDeliveryMode'
GO
;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
(
SELECT 'Surveys.DefaultCallDeliveryMode', 'Default call delivery mode', 'Surveys', 'Default call delivery mode for new surveys. Following types are allowed: 0-order by ID (lowest first), 1-random order', 1, 0, '0'
)
INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
	SELECT * FROM Data
GO

GO
PRINT N'Update complete.';


GO
