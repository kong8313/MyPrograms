GO
PRINT N'Creating [dbo].[BvAlerts]...';


GO
CREATE TABLE [dbo].[BvAlerts] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Type]         NVARCHAR (100) NOT NULL,
    [TriggerTime]  DATETIME       NOT NULL,
    [CallCenterId] INT            NOT NULL,
    CONSTRAINT [PK_BvAlerts] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY]
);

GO

PRINT N'Add Alerting system settings';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%')
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
	SELECT 'Alerting.SchedulingErrors.IsAlertEnabled', 'Enable alerting for scheduling errors', 'Alerting', 'Enable supervisor notification when multiple scheduling errors occurring for the same survey', 3, 0, 'True'
	UNION ALL
	SELECT 'Alerting.SchedulingErrors.NumberOfErrors', 'Set minimum number of scheduling errors to trigger supervisor notification', 'Alerting', 'Set minimum number of scheduling errors to trigger supervisor notification', 1, 0, '5'
	UNION ALL
	SELECT 'Alerting.SchedulingErrors.TimePeriod', 'Set time period for accumulating scheduling errors', 'Alerting', 'Within specified time period supervisor notification will be triggered if number of scheduling errors for the same survey exceeds limit defined in Alerting.SchedulingErrors.NumberOfErrors setting', 4, 0, '0.00:05:00'
	UNION ALL
	SELECT 'Alerting.SchedulingErrors.NotificationFrequency', 'Set how often notification occurs', 'Alerting', 'Supervisor will only be notified once in a specified time period', 4, 0, '0.00:30:00'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data

END

GO
PRINT N'Update complete.';


GO