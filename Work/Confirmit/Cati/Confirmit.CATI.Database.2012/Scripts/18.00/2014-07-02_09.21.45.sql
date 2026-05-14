DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
	;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
	(
	SELECT 'Setup.RedisHostName', 'RedisHostName', 'Setup', 'Redis host name. Make sense only if SessionStateMode is Redis', 2, 0, NULL
	)
	INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
		SELECT d.* FROM Data d LEFT JOIN BvSystemSettings ss ON d.[SystemName] = ss.[SystemName] WHERE ss.[SystemName] IS NULL

	;
	UPDATE BvSystemSettings 
	SET  [Description] = 'Session state mode. Possible values: InProc, SQLMode, Redis'
	WHERE [SystemName] = 'Setup.SessionStateMode'
END
