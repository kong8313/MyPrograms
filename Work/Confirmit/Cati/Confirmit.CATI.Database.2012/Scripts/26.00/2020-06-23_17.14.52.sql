PRINT N'Delete Setup.IsNonDisruptiveUpdateModeEnabled setting';
GO

DELETE FROM BvSystemSettings
WHERE SystemName = 'Setup.IsNonDisruptiveUpdateModeEnabled'
GO

PRINT N'Update complete.';
GO
