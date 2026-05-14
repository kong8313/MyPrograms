PRINT N'Add RoutineMaintenance.Actions.ServiceBrokerObjectsCleanup.ShiftType and RoutineMaintenance.Actions.ServiceBrokerObjectsCleanup.ExpirationPeriod system settings';


GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
  SELECT 'RoutineMaintenance.Actions.ServiceBrokerObjectsCleanup.ShiftType', 'Unused service broker objects cleanup shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '1'
  UNION ALL
  SELECT 'RoutineMaintenance.Actions.ServiceBrokerObjectsCleanup.ExpirationPeriod', 'Unused service broker objects cleanup expiration period', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '1.00:00:00'

  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO