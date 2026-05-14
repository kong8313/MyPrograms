PRINT N'Add new RoutineMaintenance.Actions.DatabaseMaintenance.RebuildIndexShiftType system setting'
GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
    ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
    (
	SELECT 'RoutineMaintenance.Actions.DatabaseMaintenance.RebuildIndexShiftType', 'Shift type of rebuild index activity', 'Supervisor', 'Shift type of rebuild index activity.', 1, 0, '2'
    )
    INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
        SELECT * FROM Data
END

UPDATE BvSystemSettings SET value = '1' WHERE SystemName = 'RoutineMaintenance.Actions.DatabaseMaintenance.ShiftType'

GO

PRINT N'Update complete.';
GO
