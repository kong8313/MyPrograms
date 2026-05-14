GO
PRINT N'Add RecordedInterviews.MaxSaved system setting';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'RecordedInterviews.MaxSaved', 'Set the maximum number of recorded interviews to be marked for retention', 'Supervisor', 'Number of recorded interviews that can be marked for retention', 1, 0, '100'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO
