GO
PRINT N'Altering Table [dbo].[BvTimeBreaksHistory]...';


GO
ALTER TABLE [dbo].[BvTimeBreaksHistory]
    ADD [DialTypeId] TINYINT NULL;


GO
PRINT N'Refreshing View [dbo].[RestView_BreakHistory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_BreakHistory]';


GO
PRINT N'Refreshing Function [dbo].[GetLastTimeBreak]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[GetLastTimeBreak]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpFinishInterviewerBreak]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpFinishInterviewerBreak]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetInterviewerActiveBreak]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetInterviewerActiveBreak]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetInterviewerBreaks]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetInterviewerBreaks]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpInterviewerProductivityReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterviewerProductivityReport]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpStartInterviewerBreak]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpStartInterviewerBreak]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSurveyOverviewReportForAllPersons]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyOverviewReportForAllPersons]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetListSurveyTasks]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetListSurveyTasks]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSurveyOverviewReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyOverviewReport]';


GO
PRINT N'Update complete.';


GO
