PRINT N'Add Server.BackendMinThreadPoolSize system setting';
GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
    BEGIN
        ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
                  (
                      SELECT 'Server.BackendMinThreadPoolSize', 'Minimum thread pool size in backend', 'System', 'Set minimum thread pool size during start of backend service if value is more then 0', 1, 0, '0'
                  )
         INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
         SELECT * FROM Data
    END