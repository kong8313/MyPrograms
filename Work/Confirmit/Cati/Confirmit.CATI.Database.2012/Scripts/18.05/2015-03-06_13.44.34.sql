
PRINT N'Delete CallManagament.SpecificPreviewEnabled system settings...';
GO

DELETE FROM BvSystemSettings
	WHERE SystemName = 'CallManagament.SpecificPreviewEnabled'

GO
PRINT N'Update complete.';


GO
