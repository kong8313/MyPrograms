GO
PRINT N'Add Supervisor.ActivityViewPageSize system setting';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Supervisor.ActivityViewPageSize', 'Set the maximum number of rows to be shown on a single page for the Interviewers List and Performance List Activity Views', 'Supervisor', 'Number of rows on a single Interviewers List and Performance List page', 1, 0, '500'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO