PRINT N'Delete Toggle.CatiAgent.AggregateInterviewerPerformanceThread setting';
GO

DELETE FROM BvSystemSettings
WHERE SystemName = 'Toggle.CatiAgent.AggregateInterviewerPerformanceThread'
GO

PRINT N'Delete Toggle.CatiAgent.AlertThread setting';
GO

DELETE FROM BvSystemSettings
WHERE SystemName = 'Toggle.CatiAgent.AlertThread'
GO

PRINT N'Delete Toggle.CatiAgent.AppointmentAlertThread setting';
GO

DELETE FROM BvSystemSettings
WHERE SystemName = 'Toggle.CatiAgent.AppointmentAlertThread'
GO

PRINT N'Delete Toggle.CatiAgent.QuotaBalancingThread setting';
GO

DELETE FROM BvSystemSettings
WHERE SystemName = 'Toggle.CatiAgent.QuotaBalancingThread'
GO

PRINT N'Delete Toggle.DoNotUseSMO setting';
GO

DELETE FROM BvSystemSettings
WHERE SystemName = 'Toggle.DoNotUseSMO'
GO

PRINT N'Delete Toggle.EnableAutomaticSetCampaign setting';
GO

DELETE FROM BvSystemSettings
WHERE SystemName = 'Toggle.EnableAutomaticSetCampaign'
GO

PRINT N'Delete Toggle.EnableDeferredMonitoringMode setting';
GO

DELETE FROM BvSystemSettings
WHERE SystemName = 'Toggle.EnableDeferredMonitoringMode'
GO

PRINT N'Delete Toggle.EnableReviewer setting';
GO

DELETE FROM BvSystemSettings
WHERE SystemName = 'Toggle.EnableReviewer'
GO

PRINT N'Delete Toggle.EnableSampleUpdate setting';
GO

DELETE FROM BvSystemSettings
WHERE SystemName = 'Toggle.EnableSampleUpdate'
GO

PRINT N'Delete Toggle.EnableSaveHistoryOptimization setting';
GO

DELETE FROM BvSystemSettings
WHERE SystemName = 'Toggle.EnableSaveHistoryOptimization'
GO
        
PRINT N'Update complete.';
GO

