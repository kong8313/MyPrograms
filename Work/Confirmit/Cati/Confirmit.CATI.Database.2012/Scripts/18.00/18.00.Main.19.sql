
GO

PRINT N'Add new system settings...';

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
 ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
 (
  SELECT 'Dialer.WaitDialerNotificationAtEnableDialerCommandTimeoutInMs', 'WaitDialerNotificationAtEnableDialerCommandTimeoutInMs', 'Telephony', 'Period to wait for successful back notification from dialer in response to supervisor EnableDialer command (in ms).', 1, 0, '10000'
 )
 INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  SELECT d.* FROM Data d LEFT JOIN BvSystemSettings ss ON d.[SystemName] = ss.[SystemName] WHERE ss.[SystemName] IS NULL
END

GO

PRINT N'Update complete.';

GO
