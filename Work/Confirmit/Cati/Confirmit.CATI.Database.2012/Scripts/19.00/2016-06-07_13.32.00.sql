DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
	IF (NOT EXISTS(SELECT 1 FROM BvSystemSettings WHERE SystemName = 'Toggle.EnableTCPA'))
	BEGIN
		WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
		(
			SELECT 'Toggle.EnableTCPA', 'EnableTCPA', 'Toggle', 'Enable TCPA', 3, 0, 'False'
		)
		INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
		SELECT * FROM Data
	END
END

GO