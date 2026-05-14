PRINT N'Add Toggle.EnableBBCCLogin setting';

GO
DECLARE @EnableNotificationValue nvarchar(128) = (SELECT [Value] FROM BvSystemSettings WHERE SystemName = 'Toggle.EnableBBCCNotification');

IF (@EnableNotificationValue IS NOT NULL)
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Toggle.EnableBBCCLogin', 'Enable users to login via the BBCC', 'Toggle', 'Enable users to login via the BBCC', 3, 0, @EnableNotificationValue
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END


GO
PRINT N'Update complete.';


GO
