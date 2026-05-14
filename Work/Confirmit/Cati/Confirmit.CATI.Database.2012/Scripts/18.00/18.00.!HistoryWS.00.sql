GO
DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN

	;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
	(
	SELECT 'WebApi.Enabled', 'WebApiEnabled', 'WebApi', 'Is WebApi enabled for the company', 3, 0, 'false'
	UNION ALL
	SELECT 'WebApi.PageSize', 'WebApiPageSize', 'WebApi', 'WebApi page size', 1, 0, '10000'
	)
	INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
		SELECT * FROM Data
END
GO

CREATE VIEW [dbo].[BvCallHistory]
AS
    SELECT
	    [h].[Id] AS [Id],
        [h].[FiredTime] AS [Time],
        [s].[Name] AS [SurveyId],
		[s].[Description] AS [SurveyName],
        [h].[InterviewId] AS [InterviewId],
        [h].[PersonSID] AS [RespondentId],
		(CASE WHEN [p].[SID] IS NOT NULL THEN [p].[Name]
			  WHEN [h].[PersonSID] = 0 THEN 'Dialer'
			  ELSE NULL END) [RespondentName],
        [h].[TelephoneNumber] AS [TelephoneNumber], 
        [h].[ITS] AS [ExtendedStatus],
        [h].[Duration] AS [Duration], 
        [h].[WaitingTime] AS [WaitingTime],
		[vcc].Name AS [CallCenterName]
    FROM [BvHistory] [h] 
        LEFT JOIN [BvSurvey]  [s] ON [h].[SurveyId] = [s].[SID]
	    LEFT JOIN [BvCallCenter] [vcc] ON [h].[CallCenterID] = [vcc].ID
	    LEFT JOIN BvPerson [p] ON [p].SID = [h].[PersonSID]
    WHERE 
        [h].[RoleID] = 2 /*CATI*/  AND
        [h].[InterviewID] IS NOT NULL
GO


CREATE VIEW [dbo].[BvBreakHistory]
AS
	SELECT
		[h].[ID] AS [Id],
		[h].[StartTime] AS [Time],
		[s].[Name] as [SurveyId],
		[s].[Description] as [SurveyName],
		[h].[Duration] AS [Duration],
		[h].[InterviewerId] AS [RespondentId],
		[p].[Name] AS [RespondentName],
		[vcc].[Name] AS [CallCenterName]
	FROM BvTimeBreaksHistory [h]
		LEFT JOIN BvPerson [p] ON [p].SID = [h].[InterviewerId]
		LEFT JOIN [BvCallCenter] [vcc] on [vcc].[ID] = [h].[CallCenterId]
		LEFT JOIN [BvSurvey] [s] on [h].SurveyId = s.SID
GO

PRINT N'Update complete.';
GO
