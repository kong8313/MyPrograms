CREATE OR ALTER PROCEDURE [dbo].[usp_SetCompLevelForConfirmitDatabases] 
@SystemCompLevel int = 0, @SurveyCompLevel int = 0, @HubCompLevel int = 0
AS
	-- changed by CATI deploy

	DECLARE @SQL varchar(max) = ''
	
	IF(@SystemCompLevel = 0)
	BEGIN
		SET @SystemCompLevel = 140		
	END	

	IF(@SurveyCompLevel = 0)
	BEGIN
		SET @SurveyCompLevel = 140		
	END

	IF(@HubCompLevel = 0)
	BEGIN
		SET @HubCompLevel = 140
	END
	
	--SYSTEM DATABASES
	SELECT @SQL += 'ALTER DATABASE ' + quotename(NAME) + ' SET COMPATIBILITY_LEVEL = ' + cast(@SystemCompLevel as char (3)) + ';' + CHAR(10) + CHAR(13)
	FROM sys.databases
	WHERE 
	[name] not like 'survey!_%' escape '!' AND [name] not like 'surveys!_%'  escape '!' AND 
	[name] not like 'hub!_%' escape '!' AND [name] not like 'hubs!_%' escape '!' AND [name] not like 'vault!_%' escape '!' AND 
	COMPATIBILITY_LEVEL <> @SystemCompLevel

	PRINT @SQL
	EXEC (@SQL)

	--SURVEY DATABASES
	SELECT @SQL += 'ALTER DATABASE ' + quotename(NAME) + ' SET COMPATIBILITY_LEVEL = ' + cast(@SurveyCompLevel as char (3)) + ';' + CHAR(10) + CHAR(13)
	FROM sys.databases
	WHERE 
	([name] like 'survey!_%' escape '!' OR [name] like 'surveys!_%'  escape '!') AND
	COMPATIBILITY_LEVEL <> @SurveyCompLevel

	PRINT @SQL
	EXEC (@SQL)

	--HUB DATABASES
	SELECT @SQL += 'ALTER DATABASE ' + quotename(NAME) + ' SET COMPATIBILITY_LEVEL = ' + cast(@HubCompLevel as char (3)) + ';' + CHAR(10) + CHAR(13)
	FROM sys.databases
	WHERE 
	[name] <> 'hub_admin' AND ([name] like 'hub!_%' escape '!' OR [name] like 'hubs!_%' escape '!' OR [name] like 'vault!_%' escape '!') AND
	COMPATIBILITY_LEVEL <> @HubCompLevel

	PRINT @SQL
	EXEC (@SQL)
