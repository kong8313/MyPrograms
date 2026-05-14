DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF ( @DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH [Data]( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
SELECT 'RoutineMaintenance.Actions.CallHistoryTableCleanup.ShiftType', 'Call history table cleanup maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '2'
UNION ALL
SELECT 'RoutineMaintenance.Actions.CallHistoryTableCleanup.ExpirationPeriod', 'Call history table cleanup expiration period', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '365.00:00:00'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
	SELECT * FROM Data

END
GO


PRINT N'Update complete.';


GO
