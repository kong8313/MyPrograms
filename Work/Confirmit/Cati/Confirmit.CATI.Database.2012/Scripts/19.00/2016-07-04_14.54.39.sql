PRINT N'Insert new system settings for RoutineMaintenance.Actions.UserSurveyListTableCleanup action...';
GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
	;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
	(
		SELECT 'RoutineMaintenance.Actions.UserSurveyListTableCleanup.ShiftType', 'Maintenance shift type', 'Supervisor', 'Maintenance shift type which should be used for run this action.', 1, 0, '1'
		UNION ALL 
		SELECT 'RoutineMaintenance.Actions.UserSurveyListTableCleanup.ExpirationPeriod', 'Expiration period', 'Supervisor', 'Time span from the current date after which records in the table will be deleted.', 4, 0, '30.00:00:00'
	)
	INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
	SELECT * FROM Data
END

GO
PRINT N'Creating [dbo].[BvUserSurveyList]...';


GO
CREATE TABLE [dbo].[BvUserSurveyList] (
    [UserName]  NVARCHAR (255) NOT NULL,
    [ListType]  TINYINT        NOT NULL,
    [SurveyId]  INT            NOT NULL,
    [AddedTime] DATETIME       NOT NULL,
    CONSTRAINT [PK_BvUserSurveyList_UserName_SurveyId] PRIMARY KEY CLUSTERED ([UserName] ASC, [ListType] ASC, [SurveyId] ASC)
);


GO
PRINT N'Creating [dbo].[BvUserSurveyList].[IX_BvUserSurveyList_UserName_AddedTime]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvUserSurveyList_UserName_AddedTime]
    ON [dbo].[BvUserSurveyList]([UserName] ASC, [ListType] ASC, [AddedTime] ASC);


GO
PRINT N'Creating [dbo].[BvSpUserSurveyList_Clean]...';


GO
CREATE PROCEDURE [dbo].[BvSpUserSurveyList_Clean]

 @MaxAddedTime DATETIME
AS
	DELETE FROM BvUserSurveyList WHERE AddedTime < @MaxAddedTime
GO
PRINT N'Creating [dbo].[BvSpUserSurveyList_Get]...';


GO
CREATE PROCEDURE [dbo].[BvSpUserSurveyList_Get]

 @UserName NVARCHAR(255),
 @ListType TINYINT,
 @CallCenterId INT
AS

SELECT TOP(20) sc.SID, sc.Name as ProjectId, sc.Description as Name FROM [BvUserSurveyList] usl
	INNER JOIN BvUserSurveyPermission usp
		ON usl.SurveyId = usp.SurveySID AND usp.UserName = @UserName
	INNER JOIN BvFnSurvey_GetByCallCenterId(@CallCenterId) sc
	ON usl.SurveyId = sc.SID
	WHERE usl.UserName = @UserName AND usl.ListType = @ListType
	ORDER BY usl.AddedTime DESC
GO
PRINT N'Creating [dbo].[BvSpUserSurveyList_Insert]...';


GO
CREATE PROCEDURE [dbo].[BvSpUserSurveyList_Insert]

 @UserName NVARCHAR(255),
 @ListType TINYINT,
 @SurveyId INT
AS

;MERGE BvUserSurveyList AS t
	USING( SELECT @UserName as UserName, @ListType as ListType, @SurveyId as SurveyId) as s
		ON t.UserName = s.UserName AND t.ListType = s.ListType AND t.SurveyId = s.SurveyId
	WHEN MATCHED THEN 
		UPDATE SET t.AddedTime = GETUTCDATE()
	WHEN NOT MATCHED BY TARGET THEN 
		INSERT(UserName, ListType, SurveyId, AddedTime) VALUES( @UserName, @ListType, @SurveyId, GETUTCDATE() );
GO
PRINT N'Update complete.';


GO
