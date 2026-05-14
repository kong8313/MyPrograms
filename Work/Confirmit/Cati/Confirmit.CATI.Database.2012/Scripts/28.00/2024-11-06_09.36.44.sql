PRINT N'Add Supervisor.ActivityViewLoadTest system setting';
GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
    BEGIN
        ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
                  (
                      SELECT 'Supervisor.ActivityViewLoadTest', 'Should be used for testing only, DO NOT enable it for clients', 'Supervisor', 'When enabled - 1 sec refresh interval should become available in the refresh interval dropdown in Activity View - Interviewer List', 3, 0, 'False'
                  )
         INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
         SELECT * FROM Data
    END
