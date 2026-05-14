GO
PRINT N'Removing system setting Setup.IsEventlogLoggingEnabled and Setup.IsDatabaseLoggingEnabled';

delete from BvSystemSettings where SystemName = 'Setup.IsEventlogLoggingEnabled' OR SystemName = 'Setup.IsDatabaseLoggingEnabled'

GO
PRINT N'Update complete.';


GO
