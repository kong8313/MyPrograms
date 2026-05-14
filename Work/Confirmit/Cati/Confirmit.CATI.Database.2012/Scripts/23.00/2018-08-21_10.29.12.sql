DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
 ;DELETE FROM BvSystemSettings WHERE [SystemName] = 'Setup.ConfirmitAuthoringServer'
 ;DELETE FROM BvSystemSettings WHERE [SystemName] = 'Setup.ConfirmitDeploymentServer'
 ;DELETE FROM BvSystemSettings WHERE [SystemName] = 'Setup.ConfirmitWebServiceServer'
 ;DELETE FROM BvSystemSettings WHERE [SystemName] = 'SetupCD.ClientDeploymentsVersion'
 ;WITH [Data]( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
SELECT 'Setup.InterviewerConsoleVersion', 'Interviewer Console version', 'Setup', 'Version of Interviewer Console', 2, 0, ''
UNION ALL
SELECT 'Setup.MonitoringConsoleVersion', 'Monitoring Console version', 'Setup', 'Version of Monitoring Console', 2, 0, ''
  )
  UPDATE BvSystemSettings
  SET
  [BvSystemSettings].[DisplayName] = [Data].[DisplayName],
  [BvSystemSettings].[Group] = [Data].[Group],
  [BvSystemSettings].[Description] = [Data].[Description],
  [BvSystemSettings].[Type] = [Data].[Type],
  [BvSystemSettings].[Hidden] = [Data].[Hidden]
  FROM [Data]
  WHERE [BvSystemSettings].[SystemName] = [Data].[SystemName]
END
GO

GO
PRINT N'Update complete.';


GO
