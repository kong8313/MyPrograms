DELETE FROM [BvSystemSettings]
WHERE [SystemName] = 'Console.UseHttpsForConsoleStateService'
GO

PRINT N'Update complete.';
GO