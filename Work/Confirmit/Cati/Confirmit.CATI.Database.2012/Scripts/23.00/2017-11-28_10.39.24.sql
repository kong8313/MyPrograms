DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF ( @DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;DELETE FROM BvSystemSettings WHERE [SystemName] = 'Dialer.InboundAudioMessages'

  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Dialer.InboundAudioMessagesJson', 'InboundAudioMessagesJson', 'Telephony', 'Audio messages that are being used during inbound call handling', 2, 0, ''
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data
END
GO

PRINT N'Update complete.';
GO