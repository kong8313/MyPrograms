PRINT N'Dropping [dbo].[BvSpCallCenter_Update]...';


GO
DROP PROCEDURE [dbo].[BvSpCallCenter_Update];


GO
PRINT N'Altering [dbo].[BvCallCenter]...';


GO
ALTER TABLE [dbo].[BvCallCenter]
    ADD [HidePii] BIT CONSTRAINT [DF_HidePii] DEFAULT (0) NOT NULL;


GO
ALTER PROCEDURE [dbo].[BvSpCallCenter_Insert]
	@Name NVARCHAR(MAX),
	@Description NVARCHAR(MAX),
	@LocalTimezoneId INT,
	@DialerId INT,
	@HidePii BIT = 0
AS

	DECLARE @Count INT = (SELECT COUNT(*) FROM BvCallCenter )
	IF @Count >= 255
	BEGIN
		RAISERROR( 'Count of call centers can''t be greater 255', 12, 1 )
		RETURN 0
	END

	INSERT INTO BvCallCenter( Name, Description, LocalTimezoneId, DialerId, HidePii ) VALUES( @Name, @Description, @LocalTimezoneId, @DialerId, @HidePii )

	RETURN SCOPE_IDENTITY()
	
	
GO
PRINT N'Refreshing [dbo].[BvSpCallCenter_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallCenter_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpCallCenter_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallCenter_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpCallCenter_ListOfAssignedToSurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallCenter_ListOfAssignedToSurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpCallHistory_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallHistory_List]';


GO
PRINT N'Refreshing [dbo].[BvSpCallHistoryData]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallHistoryData]';


GO
PRINT N'Refreshing [dbo].[BvSpGetExtendedCallHistory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetExtendedCallHistory]';


GO
PRINT N'Refreshing [dbo].[BvSpGetInterviewerBreaks]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetInterviewerBreaks]';


GO
PRINT N'Refreshing [dbo].[BvSpTimezone_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTimezone_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpTimezone_DeleteUnused]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTimezone_DeleteUnused]';


GO
PRINT N'Refreshing [dbo].[BvSpCall_Activate]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_Activate]';


GO
PRINT N'Refreshing [dbo].[BvSpSvySch_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSvySch_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpCall_Enable]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_Enable]';


GO
PRINT N'Refreshing [dbo].[BvSpPromoteCalls]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPromoteCalls]';


GO
PRINT N'Refreshing [dbo].[BvSpSetCallDeliveryMode]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSetCallDeliveryMode]';


GO
PRINT N'Update complete.';


GO
