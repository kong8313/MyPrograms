CREATE PROCEDURE [dbo].[BvSpGetDeferredMonitoringListPage] 
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
		'' AS CallCenterName,
		0 AS InterviewDuration
     
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

	DECLARE @Query NVARCHAR(4000) = 'select def.ID, def.PersonSID, def.SurveySID, def.HasAudio, 
		def.InterviewID, def.ExtendedStatus, st.Name as ExtendedStatusName, def.TimeStamp, survey.Name as SurveyName, survey.Description as SurveyConfirmitName, 
		person.Name as PersonLogin,	person.FullName as PersonName, def.RespondentName, def.TelephoneNumber,
		def.CallCenterID, vcc.Name as CallCenterName, def.InterviewDuration ' + @QueryBody

	DECLARE @TotalCount INT

	EXEC @TotalCount = BvSpGetObjectsPage @PageIndex, @PageSize, @OrderField, @IsOrderASC, @Query, 'ID', @SearchCondition
	RETURN @TotalCount
END