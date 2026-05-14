PRINT N'Delete Toggle.EnableBBCCNotification setting';
GO

DELETE FROM BvSystemSettings
WHERE SystemName = 'Toggle.EnableBBCCNotification'
GO

PRINT N'Update complete.';
GO