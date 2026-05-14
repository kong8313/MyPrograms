PRINT N'Update incorrect timestamps for telephony black list';
GO

UPDATE [BvTelephoneBlacklist] SET [Timestamp] = GETUTCDATE()
WHERE [Timestamp] = '0001-01-01 00:00:00'

GO
PRINT N'Update complete.';