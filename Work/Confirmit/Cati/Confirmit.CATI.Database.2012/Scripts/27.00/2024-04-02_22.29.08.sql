GO
PRINT N'Add Toggle.EnforceCatiHostNameForSurveys system setting';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Toggle.EnforceCatiHostNameForSurveys', 'Change the hostname in the survey links to the hostname from MultimodeBaseURL', 'Toggle', 'Change the hostname in the survey links to the hostname from MultimodeBaseURL', 3, 0, 'False'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO