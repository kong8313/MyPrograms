DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
 ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
 (
  SELECT 'AsyncOperation.RestoreSurveySqlTimeout', 'Restore survey sql command timeout', 'Supervisor', 'Sql command timeout which will used inside of ''RestoreSurvey'' operation.', 4, 0, '00.00:15:00'
 )
 INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  SELECT d.* FROM Data d LEFT JOIN BvSystemSettings ss ON d.[SystemName] = ss.[SystemName] WHERE ss.[SystemName] IS NULL
END
GO

PRINT N'Update complete.';
                          
GO
