PRINT N'Toggle.BBCC.Messaging and Toggle.BBCC.TwoWayMessaging system settings';


GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Toggle.BBCC.Messaging', 'Enable messaging in BBCC', 'Toggle', 'Enable all messaging functionality in BBCC', 3, 0, 'False'
	UNION
	SELECT 'Toggle.BBCC.TwoWayMessaging', 'Enable 2-way messaging in BBCC', 'Toggle', 'Enable 2-way messaging functionality in BBCC', 3, 0, 'False'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO
