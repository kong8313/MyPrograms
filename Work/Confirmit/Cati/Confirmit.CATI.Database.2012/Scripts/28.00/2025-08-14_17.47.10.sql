
GO
PRINT N'RoutineMaintenance.Actions.FullSynchronizationOfCatiDataInHub.ShiftType system setting';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'RoutineMaintenance.Actions.FullSynchronizationOfCatiDataInHub.ShiftType', 'Flags CATI data for full synchronization in the HUB shift type to facilitate removal of old records.', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '2'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO