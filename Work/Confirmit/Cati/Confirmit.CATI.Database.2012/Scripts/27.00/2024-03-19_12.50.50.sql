GO
PRINT N'Add Console.OrderInterviewsByPriority and Console.EnableInterviewsRandomization system setting';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Console.OrderInterviewsByPriority', 'Enable ordering interviews by priority in the manual selection screen', 'Console', 'When enabled - interviews are ordered by priority in the manual selection interface', 3, 0, 'True'
	UNION ALL
	SELECT 'Console.EnableInterviewsRandomization', 'Enable interviews randomization in the manual selection screen', 'Console', 'When enabled - interviews are randomized in the manual selection interface', 3, 0, 'False'
	UNION ALL
	SELECT 'Console.RandomizationInterviewCount', 'The pool size of interviews that will be used for randomization on the manual selection screen', 'Console', 'The pool size of interviews that will be used for randomization on the manual selection screen', 1, 0, '25000'

  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO
