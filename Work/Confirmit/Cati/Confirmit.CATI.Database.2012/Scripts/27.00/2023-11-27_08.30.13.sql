GO

PRINT N'Add Toggle.DirectlyInsertResponses system setting';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%')
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
   SELECT 'Toggle.DirectlyInsertResponses', 'Use direct insert into survey database instead of FusionSurveyData SOAP API', 'Toggle', 'When enabled cati backend uses direct insert into survey database instead of FusionSurveyData SOAP API', 3, 0, 'False'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO
