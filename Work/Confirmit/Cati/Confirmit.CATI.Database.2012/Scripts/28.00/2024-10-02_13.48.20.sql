PRINT N'Altering Table [dbo].[BvTelephoneBlacklist]...';


GO
ALTER TABLE [dbo].[BvTelephoneBlacklist]
    ADD [Timestamp] DATETIME2(0) CONSTRAINT [DF_BvTelephoneBlacklist_TestTemp] DEFAULT (GETUTCDATE()) NOT NULL,
        [Comment]      VARCHAR (74) NULL;


GO
PRINT N'Refreshing Function [dbo].[BvFnBlacklist_GetInterviewIdsForBlacklistRange]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvFnBlacklist_GetInterviewIdsForBlacklistRange]';


GO
PRINT N'Refreshing Function [dbo].[BvFnBlacklist_IsTelephoneNumberFiltered]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvFnBlacklist_IsTelephoneNumberFiltered]';


GO
PRINT N'Altering Procedure [dbo].[BvSpTelephoneBlacklist_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpTelephoneBlacklist_Insert]
@Type TINYINT,
@TelephoneNumber varchar(255),
@Comment varchar(74) = NULL
AS
SET NOCOUNT ON

DECLARE @ID TABLE( ID INT )
  
INSERT INTO [dbo].[BvTelephoneBlacklist]([Type], [TelephoneNumber], [Timestamp], [Comment])
	OUTPUT inserted.ID INTO @ID
    VALUES (@Type, @TelephoneNumber, GETUTCDATE(), @Comment)

RETURN ISNULL(( SELECT ID FROM @ID ), 0)
GO
PRINT N'Refreshing Procedure [dbo].[BvSpTelephoneBlacklist_GetSurveysToDeleteCalls]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTelephoneBlacklist_GetSurveysToDeleteCalls]';


GO
PRINT N'Refreshing Procedure [dbo].[BvSpTelephoneBlacklist_Filter]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTelephoneBlacklist_Filter]';


GO
PRINT N'Update complete.';


GO
