GO
PRINT N'Altering Function [dbo].[BvFnPerson_Get]...';


GO
ALTER FUNCTION BvFnPerson_Get( @CallCenterId INT )
RETURNS TABLE
AS
RETURN 
(
	SELECT  * FROM BvPerson  a with( nolock ) WHERE @CallCenterId = 0 OR CallCenterID = @CallCenterId
)
GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetAllPersonsAndGroups]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetAllPersonsAndGroups]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetInterviewerPerformanceList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetInterviewerPerformanceList]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetListSurveyTasks]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetListSurveyTasks]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetPersonGroups]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetPersonGroups]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetPersonGroupsLevel]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetPersonGroupsLevel]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetPersonsLevel]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetPersonsLevel]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpGetSystemWideInfo]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetSystemWideInfo]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpPersonAndGroups_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonAndGroups_List]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSendMessageToGroups]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSendMessageToGroups]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpSurvey_GetAssignedPersonList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_GetAssignedPersonList]';


GO
PRINT N'Update complete.';


GO
