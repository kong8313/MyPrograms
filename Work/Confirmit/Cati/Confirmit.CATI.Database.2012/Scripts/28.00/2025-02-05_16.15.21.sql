PRINT N'Add WebApi.RateLimiting system setting';
GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
    BEGIN
        ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
                  (
                      SELECT 'WebApi.RateLimiting', 'Enable IP rate limiting for CATI REST API', 'WebApi', 'A single IP address may make up to 20 requests per second, 1,000 requests per 15 minutes, and 10,000 requests per 12 hours', 3, 0, 'True'
                  )
         INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
         SELECT * FROM Data
    END
