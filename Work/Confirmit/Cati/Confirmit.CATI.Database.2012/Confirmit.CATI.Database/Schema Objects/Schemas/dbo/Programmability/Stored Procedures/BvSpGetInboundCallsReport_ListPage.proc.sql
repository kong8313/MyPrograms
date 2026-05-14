CREATE PROCEDURE [BvSpGetInboundCallsReport_ListPage]
	@SupervisorName NVARCHAR(255),
	@PageNumber INT, 
	@PageSize INT, 
	@OrderField NVARCHAR (64), 
	@IsOrderASC INT,
	@SearchCondition NVARCHAR (4000) = NULL
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
		''	AS [InboundNumber],
		'' AS [RespondentNumber],
		0 AS [InterviewId],
		0 AS [OperationType]

     
		RETURN 0;
	END

	DECLARE @Query NVARCHAR(MAX) = 'SELECT
		hist.[ID]							AS [ID],
		hist.[FiredTime]					AS [EventDate],
		ISNULL(survey.[SID], '''')			AS [SurveySID],
		ISNULL(survey.[Name], '''')			AS [ProjectID],
		ISNULL(survey.[Description], '''')	AS [ProjectName],
		ISNULL(hist.InboundTelNumber, '''')	AS [InboundNumber],
		ISNULL(hist.RespondentTelNumber,'''') AS [RespondentNumber],
		ISNULL(hist.InterviewId, '''')		AS [InterviewId],
		hist.OperationType					AS [OperationType]
		FROM
		[BvInboundCallsHistory] hist 
		LEFT JOIN [BvSurvey] survey 
			ON hist.SurveyId = survey.[SID]
		LEFT JOIN [BvUserSurveyPermission] perm 
			ON (perm.SurveySID = survey.[SID] AND perm.UserName = ''' + @SupervisorName + ''')
		WHERE survey.[SID] IS NULL OR (survey.[SID] IS NOT NULL AND perm.UserName IS NOT NULL)'

	DECLARE @TotalCount INT
	exec @TotalCount = BvSpGetListPage @PageNumber, @PageSize, @OrderField, @IsOrderASC, @Query, 'ID', @SearchCondition
	RETURN @TotalCount
END
