GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF ( @DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Ivr.TransferTimeout', 'Transfer timeout', 'Ivr', 'The timeout value (specified in seconds) determines how long the system will wait when attempting to transfer from IVR to a live agent. If the transfer to a live agent is not completed within the given timeout period it will be returned to IVR.', 4, 0, '0.00:00:30'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data
END


GO
