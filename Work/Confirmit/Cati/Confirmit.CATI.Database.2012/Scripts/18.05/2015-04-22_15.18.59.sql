PRINT N'Add new RoutineMaintenance.FrequencyExecution system setting'
GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
    ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
    (
        SELECT 'RoutineMaintenance.FrequencyExecution', 'Frequency of execution', 'Supervisor', 'Frequency of execution routing maintance operation for each company from default backend instance.', 4, 0, '0.01:00:00'
    )
    INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
        SELECT * FROM Data
END

GO

PRINT N'Update complete.';
GO
