PRINT N'Create bunch of Toggle.CatiAgent system settings';


GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
    SELECT 'Toggle.CatiAgent.AggregateInterviewerPerformanceThread', 'Run AggregateInterviewerPerformanceThread in CatiAgent', 'Toggle', 'Run AggregateInterviewerPerformanceThread in CatiAgent', 3, 0, 'False'
    UNION ALL
    SELECT 'Toggle.CatiAgent.AlertThread', 'Run AlertThread in CatiAgent', 'Toggle', 'Run AlertThread in CatiAgent', 3, 0, 'False'
    UNION ALL
    SELECT 'Toggle.CatiAgent.AppointmentAlertThread', 'Run AppointmentAlertThread in CatiAgent', 'Toggle', 'Run AppointmentAlertThread in CatiAgent', 3, 0, 'False'
    UNION ALL
    SELECT 'Toggle.CatiAgent.AutoLogoutThread', 'Run AutoLogoutThread in CatiAgent', 'Toggle', 'Run AutoLogoutThread in CatiAgent', 3, 0, 'False'
    UNION ALL
    SELECT 'Toggle.CatiAgent.AutoLogoutWebConsoleThread', 'Run AutoLogoutWebConsoleThread in CatiAgent', 'Toggle', 'Run AutoLogoutWebConsoleThread in CatiAgent', 3, 0, 'False'
    UNION ALL
    SELECT 'Toggle.CatiAgent.DialerHealthControlThread', 'Run DialerHealthControlThread in CatiAgent', 'Toggle', 'Run DialerHealthControlThread in CatiAgent', 3, 0, 'False'
    UNION ALL
    SELECT 'Toggle.CatiAgent.EmailReportsThread', 'Run EmailReportsThread in CatiAgent', 'Toggle', 'Run EmailReportsThread in CatiAgent', 3, 0, 'False'
    UNION ALL
    SELECT 'Toggle.CatiAgent.ExpiredCallsThread', 'Run ExpiredCallsThread in CatiAgent', 'Toggle', 'Run ExpiredCallsThread in CatiAgent', 3, 0, 'False'
    UNION ALL
    SELECT 'Toggle.CatiAgent.IvrThread', 'Run IvrThread in CatiAgent', 'Toggle', 'Run IvrThread in CatiAgent', 3, 0, 'False'
    UNION ALL
    SELECT 'Toggle.CatiAgent.QuotaBalancingThread', 'Run QuotaBalancingThread in CatiAgent', 'Toggle', 'Run QuotaBalancingThread in CatiAgent', 3, 0, 'False'
    UNION ALL
    SELECT 'Toggle.CatiAgent.ReplicationThread', 'Run ReplicationThread in CatiAgent', 'Toggle', 'Run ReplicationThread in CatiAgent', 3, 0, 'False'
    UNION ALL
    SELECT 'Toggle.CatiAgent.ReviewerUpdateReviewStatusThread', 'Run ReviewerUpdateReviewStatusThread in CatiAgent', 'Toggle', 'Run ReviewerUpdateReviewStatusThread in CatiAgent', 3, 0, 'False'
    UNION ALL
    SELECT 'Toggle.CatiAgent.RoutineMaintenanceThread', 'Run RoutineMaintenanceThread in CatiAgent', 'Toggle', 'Run RoutineMaintenanceThread in CatiAgent', 3, 0, 'False'
    UNION ALL
    SELECT 'Toggle.CatiAgent.ScheduleThread', 'Run ScheduleThread in CatiAgent', 'Toggle', 'Run ScheduleThread in CatiAgent', 3, 0, 'False'
    UNION ALL
    SELECT 'Toggle.CatiAgent.ScheduleErrorsNotificationThread', 'Run ScheduleErrorsNotificationThread in CatiAgent', 'Toggle', 'Run ScheduleErrorsNotificationThread in CatiAgent', 3, 0, 'False'
    UNION ALL    
    SELECT 'Toggle.CatiAgent.AsyncOperationSchedulerThread', 'Run AsyncOperationSchedulerThread in CatiAgent', 'Toggle', 'Run AsyncOperationSchedulerThread in CatiAgent', 3, 0, 'False'
    UNION ALL
    SELECT 'Toggle.CatiAgent.AsyncOperationsHeartBeatUpdaterThread', 'Run AsyncOperationsHeartBeatUpdaterThread in CatiAgent', 'Toggle', 'Run AsyncOperationsHeartBeatUpdaterThread in CatiAgent', 3, 0, 'False'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
      SELECT * FROM Data

END


GO
PRINT N'Update complete.';
