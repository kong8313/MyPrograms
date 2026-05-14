DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
 ;DELETE FROM BvSystemSettings WHERE [SystemName] = 'Setup.IsAliveHtmlLocation'
 ;DELETE FROM BvSystemSettings WHERE [SystemName] = 'Setup.MinFreeSpaceOnDiskInMb'
 ;DELETE FROM BvSystemSettings WHERE [SystemName] = 'Setup.DatabasesSnapshotFilePath'
 ;DELETE FROM BvSystemSettings WHERE [SystemName] = 'Setup.IsSslAcceleratorUse'
END


GO
PRINT N'Update complete.';


GO
