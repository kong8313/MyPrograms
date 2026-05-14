GO
PRINT N'Altering [dbo].[RestView_Interviewer]...';


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
		CONVERT(DATETIMEOFFSET, [p].[PwdSetDate]) as [PwdSetDate],
		[p].[DialTypeId]
	FROM 
	    [BvPerson] p
        LEFT JOIN [BvSurvey]  [s] ON [p].[AutomaticSurveyID] = [s].[SID]
GO
PRINT N'Update complete.';


GO
