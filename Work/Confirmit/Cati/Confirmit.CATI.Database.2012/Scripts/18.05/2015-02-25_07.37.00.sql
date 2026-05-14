DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
 ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
 (
  SELECT 'Setup.IsLoadBalancedEnvironment', 'IsLoadBalancedEnvironment', 'Setup', 'Is load balanced environment use. Possible values: True or False', 2, 0, NULL
  UNION ALL
  SELECT 'Setup.IsNonDisruptiveUpdateModeEnabled', 'IsNonDisruptiveUpdateModeEnabled', 'Setup', 'Type of installation: non-disruptive or disruptive. Required for load balanced environment. Possible values: True or False', 2, 0, NULL
  UNION ALL
  SELECT 'Setup.LoadBalancerIsAlivePageUrl', 'LoadBalancerIsAlivePageUrl', 'Setup', 'Url path to IsAlive.htm file. Required for non-disruptive installation in load balanced environment', 2, 0, NULL
  UNION ALL
  SELECT 'Setup.LoadBalancerIsAlivePageRenameTimeout', 'LoadBalancerIsAlivePageRenameTimeout', 'Setup', 'Timeout after renaming of IsAlive.htm file', 2, 0, NULL
 )
 INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  SELECT d.* FROM Data d LEFT JOIN BvSystemSettings ss ON d.[SystemName] = ss.[SystemName] WHERE ss.[SystemName] IS NULL
END


GO
PRINT N'Update complete.';


GO
