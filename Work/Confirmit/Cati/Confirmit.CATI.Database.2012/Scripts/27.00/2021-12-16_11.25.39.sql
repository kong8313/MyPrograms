PRINT N'Delete Toggle.EnableBBCCLogin setting';
GO

DELETE FROM BvSystemSettings
WHERE SystemName = 'Toggle.EnableBBCCLogin'
GO

PRINT N'Update complete.';
GO
