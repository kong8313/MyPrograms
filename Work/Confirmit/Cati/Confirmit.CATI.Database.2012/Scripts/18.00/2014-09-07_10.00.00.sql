ALTER VIEW [dbo].[RestView_BreakHistory]
AS
	SELECT
		[h].[ID] AS [Id],
		CONVERT(DATETIMEOFFSET, [h].[StartTime]) AS [Time],
		[s].[Name] as [SurveyId],
		[h].[Duration] AS [Duration],
		[h].[InterviewerId] AS [InterviewerId],
		[h].[CallCenterID] as [CallCenterId]
	FROM BvTimeBreaksHistory [h]
		LEFT JOIN [BvSurvey] [s] on [h].SurveyId = s.SID
GO

ALTER VIEW [dbo].[RestView_CallHistory]
AS
    SELECT
	    [h].[Id] AS [Id],
        CONVERT(DATETIMEOFFSET, [h].[FiredTime]) AS [Time],
        [s].[Name] AS [SurveyId],
        [h].[InterviewId] AS [InterviewId],
        [h].[PersonSID] AS [InterviewerId],
        [h].[TelephoneNumber] AS [TelephoneNumber], 
        [h].[ITS] AS [ExtendedStatus],
        [h].[Duration] AS [Duration], 
        [h].[WaitingTime] AS [WaitingTime],
		[h].[CallCenterID] as [CallCenterId]
    FROM [BvHistory] [h] 
        LEFT JOIN [BvSurvey]  [s] ON [h].[SurveyId] = [s].[SID]
	    LEFT JOIN BvPerson [p] ON [p].SID = [h].[PersonSID]
    WHERE 
        [h].[RoleID] = 2 /*CATI*/  AND
        [h].[InterviewID] IS NOT NULL
GO

ALTER VIEW [dbo].[RestView_Group]
	AS SELECT 
		[SID] as GroupId,
		[Name],
		[Description]
	FROM 
		[BvPersonGroup]
GO

ALTER VIEW [dbo].[RestView_Interviewer]
	AS 
	SELECT 
		[p].[SID] as InterviewerId,
		[p].[Name],
		[p].[Description],
		[p].[ManualSelection],
		[p].[HasNewMessage],
		[s].[Name] AS [AutomaticSurveyId],
		[p].[AllowedChoices],
		[p].[IsLocked],
		CONVERT(DATETIMEOFFSET, [p].[LockedDate]) as [LockedDate],
		[p].[AssignmentsListMode],
		[p].[CallGroupID] as [CallGroupId],
		[p].[CallCenterID] as [CallCenterId],
		[p].[Location],
		CONVERT(DATETIMEOFFSET, [p].[PwdSetDate]) as [PwdSetDate]
	FROM 
	    [BvPerson] p
        LEFT JOIN [BvSurvey]  [s] ON [p].[AutomaticSurveyID] = [s].[SID]
GO

DROP VIEW [dbo].[RestView_Project]
GO

CREATE VIEW [dbo].[RestView_Survey]
	AS 
	SELECT
	    s.Name as [SurveyId],
		s.Description as [SurveyName],
		ISNULL(sample.Count, 0) SampleSize,
		s.State as [State]
	FROM
	    [BvSurvey] s
		LEFT JOIN (SELECT COUNT(*) as Count, SurveySID FROM BvInterview group by SurveySid ) as sample on s.SID = sample.SurveySID 
