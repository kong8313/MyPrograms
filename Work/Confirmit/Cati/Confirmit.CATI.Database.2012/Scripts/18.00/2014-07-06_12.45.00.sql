CREATE VIEW [dbo].[RestView_CallHistory]
AS
    SELECT
	    [h].[Id] AS [Id],
        CONVERT(DATETIMEOFFSET, [h].[FiredTime]) AS [Time],
        [s].[Name] AS [ProjectId],
        [h].[InterviewId] AS [InterviewId],
        [h].[PersonSID] AS [InterviewerId],
        [h].[TelephoneNumber] AS [TelephoneNumber], 
        [h].[ITS] AS [ExtendedStatus],
        [h].[Duration] AS [Duration], 
        [h].[WaitingTime] AS [WaitingTime],
		[h].[CallCenterID] as [CallCenterId],
		[vcc].Name AS [CallCenterName]
    FROM [BvHistory] [h] 
        LEFT JOIN [BvSurvey]  [s] ON [h].[SurveyId] = [s].[SID]
	    LEFT JOIN [BvCallCenter] [vcc] ON [h].[CallCenterID] = [vcc].ID
	    LEFT JOIN BvPerson [p] ON [p].SID = [h].[PersonSID]
    WHERE 
        [h].[RoleID] = 2 /*CATI*/  AND
        [h].[InterviewID] IS NOT NULL
GO

CREATE VIEW [dbo].[RestView_BreakHistory]
AS
	SELECT
		[h].[ID] AS [Id],
		CONVERT(DATETIMEOFFSET, [h].[StartTime]) AS [Time],
		[s].[Name] as [ProjectId],
		[h].[Duration] AS [Duration],
		[h].[InterviewerId] AS [InterviewerId],
		[h].[CallCenterID] as [CallCenterId],
		[vcc].[Name] AS [CallCenterName]
	FROM BvTimeBreaksHistory [h]
		LEFT JOIN [BvCallCenter] [vcc] on [vcc].[ID] = [h].[CallCenterId]
		LEFT JOIN [BvSurvey] [s] on [h].SurveyId = s.SID
GO

CREATE VIEW [dbo].[RestView_Interviewer]
AS 
SELECT 
	[p].[SID] as InterviewerId,
	[p].[Name],
	[p].[Description],
	[p].[ManualSelection],
	[p].[HasNewMessage],
	[s].[Name] AS [AutomaticProjectId],
	[p].[AllowedChoices],
	[p].[IsLocked],
	CONVERT(DATETIMEOFFSET, [p].[LockedDate]) as [LockedDate],
	[p].[AssignmentsListMode],
	[p].[CallGroupID] as [CallGroupId],
	[p].[CallCenterID] as [CallCenterId],
	[vcc].Name AS [CallCenterName],
	[p].[Location],
	CONVERT(DATETIMEOFFSET, [p].[PwdSetDate]) as [PwdSetDate]
FROM 
	[BvPerson] p
    LEFT JOIN [BvSurvey]  [s] ON [p].[AutomaticSurveyID] = [s].[SID]
	LEFT JOIN [BvCallCenter] [vcc] ON [p].[CallCenterID] = [vcc].ID
GO

CREATE VIEW [dbo].[RestView_Group]
	AS SELECT 
		[SID] as GroupId,
		[Name],
		[Description]
	FROM 
		[BvPersonGroup]
GO

DROP VIEW [dbo].[BvBreakHistory]
GO

DROP VIEW [dbo].[BvCallHistory]
GO
