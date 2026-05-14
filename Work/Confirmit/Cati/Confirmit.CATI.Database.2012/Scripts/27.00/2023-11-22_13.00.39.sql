GO
PRINT N'Add CallManagement.PageSize to set number of calls in a single Call Management page';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'CallManagement.PageSize', 'Set number of calls in a single Call Management page', 'Call Management', 'Number of calls in a single Call Management page', 1, 0, '20'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO

