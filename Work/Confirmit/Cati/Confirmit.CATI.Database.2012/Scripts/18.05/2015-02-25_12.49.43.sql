PRINT 'Add new system setigs:'
GO

WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
(
	SELECT 'CallManagament.SpecificPreviewEnabled', 'SpecificPreviewEnabled', 'CallManagament', 'Is specific preview context menu item enabled for call management table', 3, 0, 'false'
)
INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
	SELECT * FROM Data

GO
PRINT N'Update complete.';


GO
