GO
PRINT N'Add Console.AllowTransferToAssignedSurveysOnly system setting';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Console.AllowTransferToAssignedSurveysOnly', 'Restrict internal call transfers only to groups which have surveys assigned explicitly', 'Interviewing', 'Restrict internal call transfers only to groups which have surveys assigned explicitly', 3, 0, 'False'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO
