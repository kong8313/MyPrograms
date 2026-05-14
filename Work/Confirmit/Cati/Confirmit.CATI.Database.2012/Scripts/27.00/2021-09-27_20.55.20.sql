PRINT N'Adding Console.BBCC.OptimisticConcurrency system setting';


GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
    SELECT 'Console.BBCC.OptimisticConcurrency', 'Optimistic concurrency for interviewer state', 'Interviewing', 'Controls the way how transactions updating interviewer state are synchronized. When enabled - database locks are not used and transaction is automatically retried if there is a conflict. When disabled - exclusive database lock is placed on interviewer state.', 3, 0, 'False'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO