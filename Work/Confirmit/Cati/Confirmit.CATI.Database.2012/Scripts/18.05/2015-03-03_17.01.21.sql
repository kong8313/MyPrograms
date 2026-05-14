PRINT 'Add new system settings:'
GO

WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
(
	SELECT 'Reports.CallHistoryReportLoginLogoutEventsRowsLimit', 'CallHistoryReportLoginLogoutEventsRowsLimit', 'Supervisor', 'Limit for login/logout events data rows exported.', 1, 0, '100000'
	UNION ALL
	SELECT 'Reports.CallHistoryReportReplicatedVariables', 'CallHistoryReportReplicatedVariables', 'Supervisor', 'Replicated variables that should be included into report.', 2, 0, NULL
	UNION ALL
	SELECT 'Reports.CallHistoryReportReplicatedVariablesEnabled', 'CallHistoryReportReplicatedVariablesEnabled', 'Supervisor', 'Are replicated variables in call history report enabled?', 3, 0, 'False'
)
INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
	SELECT * FROM Data

GO
PRINT N'Update complete.';


GO
