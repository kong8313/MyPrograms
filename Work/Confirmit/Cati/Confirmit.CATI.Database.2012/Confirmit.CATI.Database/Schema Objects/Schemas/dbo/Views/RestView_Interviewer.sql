CREATE VIEW [dbo].[RestView_Interviewer]
	AS 
	SELECT 
		[p].[SID] as InterviewerId,
		[p].[Name],
		[p].[Description],
		[p].[FullName],
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
		CONVERT(DATETIMEOFFSET, [p].[PwdSetDate]) as [PwdSetDate],
		[p].[DialTypeId],
		[p].[Attribute1],
		[p].[Attribute2],
		[p].[Attribute3],
		[p].[Attribute4],
		[p].[Attribute5]
	FROM 
	    [BvPerson] p
        LEFT JOIN [BvSurvey]  [s] ON [p].[AutomaticSurveyID] = [s].[SID]
