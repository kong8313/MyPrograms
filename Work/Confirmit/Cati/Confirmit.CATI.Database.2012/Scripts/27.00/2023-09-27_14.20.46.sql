GO
PRINT N'Adding Dialer.OpenSurveysOnDialersIndividually';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Dialer.OpenSurveysOnDialersIndividually', 'Enable opening and closing surveys on each dialer individually.', 'Telephony', 'When enabled - surveys are opened and closed on each dialer individually. When disabled - surveys are opened and closed on dialers via batch requests for dialers with the same dial type. Should be enabled to support multiple dialers of the same type pointing to different dialer webservices.', 3, 0, 'False'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO