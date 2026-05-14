PRINT N'Set true to Toggle.CatiAgent.AggregateInterviewerPerformanceThread, Toggle.CatiAgent.AlertThread, Toggle.CatiAgent.AppointmentAlertThread and Toggle.CatiAgent.QuotaBalancingThread settings';

GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF ( @DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
    BEGIN
        UPDATE [BvSystemSettings]
        SET [Value] = 'True'
        WHERE [SystemName] = 'Toggle.CatiAgent.AggregateInterviewerPerformanceThread' OR
              [SystemName] = 'Toggle.CatiAgent.AlertThread' OR
              [SystemName] = 'Toggle.CatiAgent.AppointmentAlertThread' OR
              [SystemName] = 'Toggle.CatiAgent.QuotaBalancingThread'
    END


GO
PRINT N'Update complete.';
