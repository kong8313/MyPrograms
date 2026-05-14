PRINT N'Altering [dbo].[BvPersonDeferredMonitoring]...';


GO
ALTER TABLE [dbo].[BvPersonDeferredMonitoring]
    ADD [RespondentName]  NVARCHAR (255) NULL,
        [TelephoneNumber] VARCHAR (255)  NULL;
GO
update BvPersonDeferredMonitoring
set RespondentName = bvinterview.RespondentName,
    TelephoneNumber = bvinterview.TelephoneNumber
from bvinterview
where BvPersonDeferredMonitoring.InterviewId = bvinterview.id AND
      BvPersonDeferredMonitoring.SurveySid = bvinterview.SurveySid

GO
PRINT N'Creating [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring_InterviewId]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvPersonDeferredMonitoring_InterviewId]
    ON [dbo].[BvPersonDeferredMonitoring]([InterviewID] ASC, [ID] ASC, [IsComplete] ASC);


GO
PRINT N'Creating [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring_TelephoneNumber]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvPersonDeferredMonitoring_TelephoneNumber]
    ON [dbo].[BvPersonDeferredMonitoring]([TelephoneNumber] ASC, [ID] ASC, [IsComplete] ASC);


GO
PRINT N'Altering [dbo].[BvSpGetDeferredMonitoringListPage]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetDeferredMonitoringListPage] 
	@PageIndex INT, 
	@PageSize INT, 
	@OrderField NVARCHAR (64),
	@IsOrderASC BIT, 
	@userName NVARCHAR (255),
	@SearchCondition NVARCHAR(4000) = NULL
AS
BEGIN
	IF @userName IS NULL AND @PageIndex IS NULL
	BEGIN
		/* Looks like we're generating code using FMTONLY. So lets return metadata*/
		SELECT
		0  AS ID,
		0 AS PersonSID,
		0 AS SurveySID,
		CAST(0 as bit) AS HasAudio,
		0 AS InterviewID,
		0 as ExtendedStatus,
		'' as ExtendedStatusName,
		GETDATE() AS TimeStamp,
		'' AS SurveyName,
		'' AS SurveyConfirmitName,
		'' AS PersonLogin,
		'' AS PersonName,
		'' AS RespondentName,
		'' AS TelephoneNumber,
		0 AS CallCenterId,
		'' AS CallCenterName
     
		RETURN 0;
	END
	
	DECLARE @StateGroupID INT
	SELECT @StateGroupID = MIN(ID) FROM BvStateGroup
	
	DECLARE @QueryBody as nvarchar(4000) = 
	   'from BvPersonDeferredMonitoring as def 
	    inner join BvSurvey as survey on def.SurveySID = survey.SID
		inner join BvUserSurveyPermission as perm on perm.SurveySID = def.SurveySID
		inner join BvPerson as person on person.SID = def.PersonSID
		left join BvCallCenter as vcc on def.[CallCenterId] = vcc.[ID]
		left join BvState as st on def.ExtendedStatus = st.StateID AND st.StateGroupID = '+ CAST(@StateGroupID AS NVARCHAR) +'
	where 
		def.IsComplete = 1 and perm.UserName = ''' + @userName + ''' and survey.State <> 2'

	DECLARE @Counter as nvarchar(4000) = 'select count(*) cnt '

	DECLARE @Query NVARCHAR(4000) = 'select def.ID, def.PersonSID, def.SurveySID, def.HasAudio, 
		def.InterviewID, def.ExtendedStatus, st.Name as ExtendedStatusName, def.TimeStamp, survey.Name as SurveyName, survey.Description as SurveyConfirmitName, 
		person.Name as PersonLogin,	person.FullName as PersonName, def.RespondentName, def.TelephoneNumber,
		def.CallCenterID, vcc.Name as CallCenterName ' + @QueryBody

	SET @Counter = @Counter + 
	   'from BvPersonDeferredMonitoring as def 
	    inner join BvSurvey as survey on def.SurveySID = survey.SID
		inner join BvUserSurveyPermission as perm on perm.SurveySID = def.SurveySID
		inner join BvPerson as person on person.SID = def.PersonSID
	where 
		def.IsComplete = 1 and perm.UserName = ''' + @userName + ''' and survey.State <> 2'	

	DECLARE @TotalCount INT

	EXEC @TotalCount = BvSpGetObjectsPage @PageIndex, @PageSize, @OrderField, @IsOrderASC, @Query, 'ID', @SearchCondition, @Counter
	RETURN @TotalCount
END
GO
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Refreshing [dbo].[BvSpCleanDeferredMonitoring]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpCleanDeferredMonitoring';


GO
PRINT N'Refreshing [dbo].[BvSpGetDeferredMonitoringStartFile]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpGetDeferredMonitoringStartFile';


GO
PRINT N'Update complete.';


GO
