PRINT N'Add CallManagement.ExportCallsLimit system setting';

GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
	IF (NOT EXISTS(SELECT 1 FROM BvSystemSettings WHERE SystemName = 'CallManagement.ExportCallsLimit'))
	BEGIN
		WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
		(
			SELECT 'CallManagement.ExportCallsLimit', 'Export calls limit', 'Call Management', 'Upper limit of amount of calls that user can export on Call Management page', 1, 0, 10000
		)
		INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
		SELECT * FROM Data
	END
END


GO
PRINT N'Update complete.';


GO