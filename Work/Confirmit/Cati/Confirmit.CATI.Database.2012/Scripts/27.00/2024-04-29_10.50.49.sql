GO
PRINT N'Add Console.ManualCallsInsideShiftOnly system setting';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Console.ManualCallsInsideShiftOnly', 'Display only interviews on the manual selection screen that are valid for the current shift and have a time to call value either now or in the past', 'Interviewing', 'Display only interviews on the manual selection screen that are valid for the current shift and have a time to call value either now or in the past', 3, 0, 'False'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO