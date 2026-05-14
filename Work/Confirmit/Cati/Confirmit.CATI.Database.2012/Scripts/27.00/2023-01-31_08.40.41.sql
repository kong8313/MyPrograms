GO
PRINT N'Adding Supervisor.SurveysListOldStyleEnabled system setting';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Supervisor.SurveysListOldStyleEnabled', 'Show surveys list in old style', 'Supervisor', 'Indicates whether CATI Supervisor should show surveys list in old style.', 3, 0, 'True'

  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO
