UPDATE BvSystemSettings 
    SET SystemName = 'RoutineMaintenance.Actions.DatabaseMaintenance.ShiftType'
    WHERE SystemName = 'RoutineMaintenance.Actions.RebuildIndexes.ShiftType'

UPDATE BvSystemSettings 
    SET SystemName = 'RoutineMaintenance.Actions.DatabaseMaintenance.IgnoredIndexes',
        Description = 'List of ignored system indexes that will not be rebuilded/reorginized.'
    WHERE SystemName = 'RoutineMaintenance.Actions.RebuildIndexes.Ignored'

UPDATE BvSystemSettings 
    SET SystemName = 'RoutineMaintenance.Actions.DatabaseMaintenance.IndexFragmentationDetectMode'
    WHERE SystemName = 'RoutineMaintenance.Actions.RebuildIndexes.FragmentationDetectMode'

UPDATE BvSystemSettings 
    SET SystemName = 'RoutineMaintenance.Actions.DatabaseMaintenance.FragmentationIndexReorganizeThreshold'
    WHERE SystemName = 'RoutineMaintenance.Actions.RebuildIndexes.FragmentationReorganizeThreshold'

UPDATE BvSystemSettings 
    SET SystemName = 'RoutineMaintenance.Actions.DatabaseMaintenance.FragmentationIndexRebuildThreshold'
    WHERE SystemName = 'RoutineMaintenance.Actions.RebuildIndexes.FragmentationRebuildThreshold'

UPDATE BvSystemSettings 
    SET SystemName = 'RoutineMaintenance.Actions.DatabaseMaintenance.UpdateStatisticTables'
    WHERE SystemName = 'RoutineMaintenance.Actions.UpdateStatistics.Updated'

DELETE FROM BvSystemSettings WHERE SystemName = 'RoutineMaintenance.Actions.UpdateStatistics.ShiftType'

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
    ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
    (
    SELECT 'RoutineMaintenance.Actions.DatabaseMaintenance.MinIndexPageCount', 'Minimum index page count', 'Supervisor', 'If count of pages is used in index less than minimum index page count, so index willn''t be rebuilded/reoginized.', 1, 0, '100'
    )
    INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
        SELECT * FROM Data
END

GO


GO
PRINT N'Update complete.';

GO
