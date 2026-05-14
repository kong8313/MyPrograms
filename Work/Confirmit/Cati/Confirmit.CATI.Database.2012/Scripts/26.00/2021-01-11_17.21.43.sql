GO
PRINT N'Altering [dbo].[BvSpGetCallAttemptsReport_ListPage]...';


GO
ALTER PROCEDURE BvSpGetCallAttemptsReport_ListPage 
	@SupervisorName NVARCHAR(255),
	@PageNumber INT, 
	@PageSize INT, 
	@OrderField NVARCHAR (64), 
	@IsOrderASC INT,
	@SearchCondition NVARCHAR (4000) = NULL,
	@IncludeDisposedByDialerAttempts BIT
AS
BEGIN
	IF @SupervisorName IS NULL AND @PageNumber IS NULL AND @PageSize IS NULL
	BEGIN
	/* Looks like we're generating code using FMTONLY. So lets return metadata*/
		SELECT
		0 as [ID],
		GETDATE() as [EventDate],
		0 as [SurveySID],
		'' as [ProjectID],
		'' as [ProjectName],
		'' as [InterviewerName],
		0 as [InterviewID],
		0 as [CallDuration],
		CAST( 0 as SMALLINT) as [ExtendedStatus],
		'' as [ExtendedStatusName],
		'' as [TelephoneNumber],
		0 as [WaitingTime],
		0 as [DisplayTime]
     
		RETURN 0;
	END
 
	DECLARE @StateGroupID INT
	SELECT @StateGroupID = ID FROM [BvStateGroup] WHERE [Order] = (SELECT MIN([Order]) FROM [BvStateGroup])
	
	DECLARE @Query NVARCHAR(MAX) = 'SELECT
		hist.[ID] as [ID],
		hist.[FiredTime] as [EventDate],
		survey.[SID] as [SurveySID],
		survey.[Name] as [ProjectID],
		survey.[Description] as [ProjectName],
		ISNULL(person.[Name], ''Not Applicable'') as [InterviewerName],
		hist.[InterviewId] as [InterviewID],
		hist.[Duration] as [CallDuration],
		hist.[ITS] as [ExtendedStatus],
		states.[Name] as [ExtendedStatusName],
		hist.[TelephoneNumber] as [TelephoneNumber],
		hist.[WaitingTime] as [WaitingTime],
		hist.[DisplayTime] as [DisplayTime]
		FROM
		[BvHistory] hist INNER JOIN [BvSurvey] survey ON hist.SurveyId = survey.[SID]
		INNER JOIN [BvUserSurveyPermission] perm ON (perm.SurveySID = survey.[SID] AND perm.UserName = ''' + @SupervisorName + ''')
		LEFT JOIN [BvPerson] person ON person.[SID] = hist.[PersonSID] 
		INNER JOIN [BvState] states ON states.StateID = hist.[ITS] AND states.StateGroupID = ' + CAST(@StateGroupID AS NVARCHAR(20)) +
		' WHERE hist.[RoleID] = 2 AND hist.InterviewId IS NOT NULL AND survey.State <> 2 AND person.[Name] IS NOT NULL OR
		(person.[Name] IS NULL AND '+ CAST(@IncludeDisposedByDialerAttempts AS NVARCHAR(50)) + ' = 1)'

	DECLARE @TotalCount INT
	exec @TotalCount = BvSpGetListPage @PageNumber, @PageSize, @OrderField, @IsOrderASC, @Query, 'ID', @SearchCondition
	RETURN @TotalCount
END
GO
PRINT N'Update complete.';


GO
