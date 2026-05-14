GO
PRINT N'Turn on Toggle.EnableInterviewerMetricsConfiguration and Console.Metrics.EnableInterviewerMetrics system settings';

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());
IF ( @DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
    UPDATE BvSystemSettings SET [Value] = 'True' WHERE [SystemName] = 'Toggle.EnableInterviewerMetricsConfiguration' OR [SystemName] = 'Console.Metrics.EnableInterviewerMetrics'
END

GO
PRINT N'Update complete.';
GO