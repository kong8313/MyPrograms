PRINT N'Remove WebServiceUrl.DictionaryApi CATI system setting';
GO

DELETE FROM BvSystemSettings
WHERE SystemName = 'WebServiceUrl.DictionaryApi'
GO

PRINT N'Update complete.';
GO
