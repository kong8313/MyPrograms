PRINT N'update extended status names';
GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());
    IF ( @DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
    BEGIN
        UPDATE BvState SET Name='Too Many Call Attempts' WHERE StateID=31;
		UPDATE BvState SET Name='Soft Appointment' WHERE StateID=33;
    END
GO
PRINT N'Update complete.';


GO
