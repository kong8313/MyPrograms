GO
PRINT N'Add Toggle.SendGoNotReadyImmediately system setting';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Toggle.SendGoNotReadyImmediately', 'Immediately notify the dialer when an interviewer enters or exits the Pending logout or Pending break state instead of waiting until the end of the interview', 'Toggle', 'Enables GoReady and GoNotReady commands to be sent to the dialer immediately when an interviewer presses either the Pending logout or Pending break button when he is doing an interview with a dialer in a survey with a predictive dialing mode', 3, 0, 'False'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO