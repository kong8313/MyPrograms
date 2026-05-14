CREATE VIEW [dbo].[RestView_BreakHistory]
AS
	SELECT
		[h].[ID] AS [Id],
		CONVERT(DATETIMEOFFSET, [h].[StartTime]) AS [Time],
		[s].[Name] as [SurveyId],
		[h].[Duration] AS [Duration],
		[h].[InterviewerId] AS [InterviewerId],
		[h].[CallCenterID] as [CallCenterId],
		[h].[BreakTypeId] as [BreakTypeId],
		[bt].[IsPaid] as [IsPaid],
		[bt].[Name] as [BreakTypeName]
	FROM BvTimeBreaksHistory [h]
		LEFT JOIN [BvSurvey] [s] on [h].SurveyId = s.SID
		LEFT JOIN [BvBreakType] [bt] on [bt].Id = [h].BreakTypeId
