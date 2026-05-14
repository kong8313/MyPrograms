PRINT N'Add Console.NoCallsTimeout setting';


GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF ( @DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
      SELECT 'Console.NoCallsTimeout', 'No_Calls timeout in seconds', 'Interviewing', 'During this timeout console will wait interview in No_Calls state', 1, 0, '60'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data
END


GO
PRINT N'Update complete.';