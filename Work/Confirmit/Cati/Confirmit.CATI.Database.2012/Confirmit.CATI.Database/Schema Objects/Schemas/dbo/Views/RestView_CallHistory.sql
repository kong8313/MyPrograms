CREATE VIEW [dbo].[RestView_CallHistory]
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
