PRINT N'Add monitoring mode feature related system settings';


GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF ( @DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
      SELECT 'Monitoring.AllowCoachingMode', 'Allow coaching mode during live monitoring', 'Supervisor', 'If true a coaching mode buttons will be shown during live monitoring. Possible values: true or false.', 3, 0, 'True'
      UNION ALL
      SELECT 'Monitoring.AllowBargingMode', 'Allow barging mode during live monitoring', 'Supervisor', 'If true a barging mode buttons will be shown during live monitoring. Possible values: true or false.', 3, 0, 'True'
      UNION ALL
      SELECT 'Toggle.EnableMonitoringCoachingMode', 'Enable monitoring coaching mode ', 'Toggle', 'Enable coaching mode for live monitoring', 3, 0, 'False'
      UNION ALL
      SELECT 'Toggle.EnableMonitoringBargingMode', 'Enable monitoring barging mode', 'Toggle', 'Enable barging mode for live monitoring', 3, 0, 'False'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data
END


GO
PRINT N'Update complete.';