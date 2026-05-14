PRINT N'Change value of [SchedulingScript.UseDirectDbAccess], [MultipleAssignments.Enabled] and [QuotaClustering.Enabled] settings in [BvSystemSettings]';
GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF( @DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%')
BEGIN
	UPDATE BvSystemSettings
	SET Value = 'True'
	WHERE SystemName = 'SchedulingScript.UseDirectDbAccess' OR SystemName = 'MultipleAssignments.Enabled' OR SystemName = 'QuotaClustering.Enabled'
END

IF( @DbName LIKE 'ConfirmitCATIV15[_]%')
BEGIN
	DELETE FROM BvSystemSettings
	WHERE SystemName = 'SchedulingScript.UseDirectDbAccess' OR SystemName = 'MultipleAssignments.Enabled' OR SystemName = 'QuotaClustering.Enabled'
END
GO

PRINT N'Update complete.';
GO