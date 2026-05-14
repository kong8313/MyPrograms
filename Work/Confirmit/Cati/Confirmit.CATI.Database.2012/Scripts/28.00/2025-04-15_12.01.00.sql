PRINT N'Add SmartHub related system settings';
GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
    BEGIN
        ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
                  (
                    SELECT 'Toggle.EnableHubIntegration', 'Enables access to Manage call history in SmartHub page and data synchronization process', 'Toggle', 'When enabled, the supervisor can manage the system CATI hub and synchronize data with it.', 3, 0, 'False'
                    UNION ALL
                    SELECT 'CallHistoryHub.SyncSleepPeriod', 'Determines the interval between synchronization operations of CATI call history data to SmartHub.', 'CallHistoryHub', 'Delay between data synchronization operations of CATI call history data with SmartHub.', 4, 0, '0.00:05:00'
                    UNION ALL
                    SELECT 'CallHistoryHub.SyncEnabled', 'When eneabled, CATI call history data will be synchronizing with SmartHub system CATI hub.', 'CallHistoryHub', 'When eneabled, CATI call history data will be synchronizing with SmartHub system CATI hub.', 3, 0, 'False'
                    UNION ALL
                    SELECT 'CallHistoryHub.RetentionPeriod', 'Specifies the retention period, in days, for storing CATI call history data in the SmartHub CATI system hub.', 'CallHistoryHub', 'The retention period, in days, for storing CATI call history data in the SmartHub CATI system hub.', 1, 0, '180'
                  )
         INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
         SELECT * FROM data
    END

GO
PRINT N'Update complete.';


GO