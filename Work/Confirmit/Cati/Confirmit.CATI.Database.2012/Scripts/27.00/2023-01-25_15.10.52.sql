DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF ( @DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
	UPDATE [BvSystemSettings] SET [Value] = '1' WHERE [SystemName] = 'FCD.BehaviorType'
END

GO
PRINT N'Update complete.';


GO
