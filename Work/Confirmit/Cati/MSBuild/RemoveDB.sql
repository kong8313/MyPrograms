-- drops all user databases
DECLARE @command nvarchar(max)
SET @command = ''

SELECT  @command = @command
+ 'ALTER DATABASE [' + [name] + ']  SET single_user with rollback immediate;'+CHAR(13)+CHAR(10)
+ 'DROP DATABASE [' + [name] +'];'+CHAR(13)+CHAR(10)
FROM  [master].[sys].[databases] 
 where [name] not in ( 'master', 'model', 'msdb', 'tempdb', 'ProfileDatabase', 'Confirmlog_Test');

EXECUTE sp_executesql @command