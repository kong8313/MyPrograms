GO
PRINT N'Add Supervisor.TablesPreserveSelectionState system settings';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
            (
			SELECT 'Supervisor.TablesPreserveSelectionState', 'Preserve selection state on filtering and switching pages in Interviewers List', 'Supervisor', 'When enabled, selection state on filtering and switching pages preserves. Works in the Interviewers List only', 3, 0, 'False'
			)
   INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
SELECT * FROM Data

END


GO
PRINT N'Update complete.';