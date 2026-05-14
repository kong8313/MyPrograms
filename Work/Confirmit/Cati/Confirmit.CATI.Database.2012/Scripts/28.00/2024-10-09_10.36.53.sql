PRINT N'Add Toggle.EnableHttpKeepAliveForDialer system setting';
GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
    BEGIN
        ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
                  (
                      SELECT 'Toggle.EnableHttpKeepAliveForDialer', 'Enable HTTP keep-alive for requests to the dialer', 'Toggle', 'HTTP persistent connection, also called HTTP keep-alive, or HTTP connection reuse, is the idea of using a single TCP connection to send and receive multiple HTTP requests/responses, as opposed to opening a new connection for every single request/response pair', 3, 0, 'False'
                  )
         INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
         SELECT * FROM Data
    END