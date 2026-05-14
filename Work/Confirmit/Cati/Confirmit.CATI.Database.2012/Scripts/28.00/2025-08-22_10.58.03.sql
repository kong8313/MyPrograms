GO
PRINT N'Add FCD.InterviewQuotaCellsTransactionThreshold system setting';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'FCD.InterviewQuotaCellsTransactionThreshold', 'Transaction threshold for importing FCD quotas', 'FCD', 'Defines the maximum number of interview-to-quota mappings to import quotas in a single transaction. If (interviews) * (FCD quotas) in a survey exceeds threshold, import runs without transaction', 1, 0, '5000000'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data
END

GO
PRINT N'Update complete.';

