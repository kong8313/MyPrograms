DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
	IF (NOT EXISTS(SELECT 1 FROM BvSystemSettings WHERE SystemName = 'Setup.ReleaseNumber'))
	BEGIN
		WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
		(
			SELECT 'Setup.ReleaseNumber', 'Octopus release number', 'Setup', 'Number of the latest octopus release', 2, 0, ''
		)
		INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
		SELECT * FROM Data
	END
	
	IF (NOT EXISTS(SELECT 1 FROM BvSystemSettings WHERE SystemName = 'Setup.ReleaseDate'))
	BEGIN
		WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
		(
			SELECT 'Setup.ReleaseDate', 'Octopus release date', 'Setup', 'Date of the latest octopus release', 2, 0, ''
		)
		INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
		SELECT * FROM Data
	END
END


GO
PRINT N'Update complete.';


GO

