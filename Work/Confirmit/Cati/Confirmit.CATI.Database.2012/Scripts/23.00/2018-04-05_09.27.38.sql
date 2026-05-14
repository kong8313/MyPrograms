PRINT N'Adding Security.AlwaysEncryptFiles and Security.UserForEncryption...';
GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	 SELECT 'Security.UserForEncryption', 'User for encryption', 'Security', 'User for encryption', 2, 0, ''
	 UNION ALL
     SELECT 'Security.AlwaysEncryptFiles', 'Always Encrypt Files', 'Security', 'Always use encrypted file transfer', 3, 0, 'False'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END
GO

PRINT N'Update complete.';
GO