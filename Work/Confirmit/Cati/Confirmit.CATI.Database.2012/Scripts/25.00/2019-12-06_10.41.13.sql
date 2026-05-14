UPDATE dbo.[BvSystemSettings]
SET Value = '0'
WHERE [SystemName] = 'Reports.ScheduledInterviewerProductivityReportTemplateId' AND [Value] IS NULL

GO
PRINT N'Update complete.';


GO
