GO
PRINT 'Add new system settigs: Reports.ScheduledInterviewerProductivityReportTemplateId'
GO
;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
(
SELECT 'Reports.ScheduledInterviewerProductivityReportTemplateId', 'Interviewer productivity report template id to use for scheduled email report', 'Supervisor', 'Interviewer productivity report template id to use for scheduled email report', 1, 0, NULL
)
INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
	SELECT * FROM Data
GO

GO
PRINT N'Update complete.';


GO
