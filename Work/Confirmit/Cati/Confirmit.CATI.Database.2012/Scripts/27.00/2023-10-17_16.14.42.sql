GO
PRINT N'RoutineMaintenance.Actions.LargeObjectHeapFragmentation.ShiftType system setting';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'RoutineMaintenance.Actions.LargeObjectHeapFragmentation.ShiftType', 'Large object heap fragmentation shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '1'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO
