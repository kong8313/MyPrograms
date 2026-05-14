PRINT N'AccountLocking.MaxFailedLoginAttemptsForced system setting';


GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'AccountLocking.MaxFailedLoginAttemptsForced', 'Account locking max failed login attempts applied by default', 'Interviewing', 'Number of consecutive unsuccessful login attempts after which the account will be locked automatically. This setting is applied only if AccountLocking.Enabled setting is disabled. Otherwise it will be overridden by the value of the AccountLocking.MaxFailedLoginAttempts setting.', 1, 0, '100'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END


GO
PRINT N'Update complete.';


GO