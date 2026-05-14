GO
PRINT N'Altering Table [dbo].[BvHistory]...';


GO
ALTER TABLE [dbo].[BvHistory]
    ADD [Custom1] NVARCHAR (50) NULL,
        [Custom2] NVARCHAR (50) NULL,
        [Custom3] NVARCHAR (50) NULL,
        [Custom4] NVARCHAR (50) NULL,
        [Custom5] NVARCHAR (50) NULL;


GO
PRINT N'Refreshing View [dbo].[RestView_CallHistory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_CallHistory]';


GO
PRINT N'Refreshing Function [dbo].[GetCountsForSample]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[GetCountsForSample]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpAggregateInterviewerPerformance]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAggregateInterviewerPerformance]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpAlert_RecalculateAll]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAlert_RecalculateAll]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpAttemptsByDispositionReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpAttemptsByDispositionReport]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpCallHistory_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallHistory_List]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpCallHistoryData]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallHistoryData]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpHistory_GetLinkedInterviews]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpHistory_GetLinkedInterviews]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpInterviewerProductivityReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterviewerProductivityReport]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpNumberOfAttemptsReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpNumberOfAttemptsReport]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpReportInboundCalls]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpReportInboundCalls]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpReportSampleStatusSummaryForDatesRange]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpReportSampleStatusSummaryForDatesRange]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSurveyOverviewReportForAllPersons]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyOverviewReportForAllPersons]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSurveyProductivityReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyProductivityReport]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSampleUtilisationReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSampleUtilisationReport]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSurveyOverviewReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyOverviewReport]';


GO
PRINT N'Update complete.';


GO
