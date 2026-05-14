PRINT N'Add new SchedulingScript.UseDirectDbAccess system setting'
GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
    ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
    (
    SELECT 'SchedulingScript.UseDirectDbAccess', 'Use direct database access inside F function', 'Scheduling script', 'Enable usage of direct database access for scheduling script F function instead of usage WebServ.', 3, 0, 'False'
    )
    INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
        SELECT * FROM Data
END

GO

PRINT N'Update complete.';
GO
