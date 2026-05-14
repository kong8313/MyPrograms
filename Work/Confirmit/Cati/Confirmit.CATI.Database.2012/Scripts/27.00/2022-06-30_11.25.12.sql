
DECLARE @tables NVARCHAR(MAX);
SELECT @tables = BvSystemSettings.Value FROM BvSystemSettings WHERE SystemName='RoutineMaintenance.Actions.DatabaseMaintenance.UpdateStatisticTables';

UPDATE BvSystemSettings SET Value = @tables + ',BvAsyncOperationQueue' WHERE SystemName='RoutineMaintenance.Actions.DatabaseMaintenance.UpdateStatisticTables';



GO
PRINT N'Update complete.';


GO
