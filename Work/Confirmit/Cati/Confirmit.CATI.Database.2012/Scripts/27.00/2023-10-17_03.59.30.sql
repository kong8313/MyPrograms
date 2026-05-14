GO
PRINT N'RoutineMaintenance.Actions.SchedulingScriptLogTableCleanup.ShiftType system setting change value to 60 days';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  UPDATE BvSystemSettings
  SET Value = '60.00:00:00'
  WHERE SystemName = 'RoutineMaintenance.Actions.SchedulingScriptLogTableCleanup.ExpirationPeriod'
END

GO
PRINT N'Update complete.';


GO
