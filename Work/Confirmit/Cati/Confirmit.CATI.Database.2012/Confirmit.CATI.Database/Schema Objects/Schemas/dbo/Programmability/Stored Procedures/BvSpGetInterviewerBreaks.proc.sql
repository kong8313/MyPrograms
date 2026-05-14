CREATE PROCEDURE [dbo].[BvSpGetInterviewerBreaks]
	@StartDate DATETIME, @EndDate DATETIME, @SurveySIDs nvarchar(max),  @MaxRows int
AS

IF(@StartDate IS NULL) SET @StartDate = '01-01-1753 00:00:00'
IF(@EndDate IS NULL) SET @EndDate = '12-31-9999 23:59:59.997'

		;WITH SelectedSurveySIDs_CTE AS
		(
		 SELECT [Item] FROM dbo.utilSplitNumbers(ISNULL(@SurveySIDs, ''), ',')
		)
SELECT TOP (@MaxRows)
	[h].[ID] AS [ID],
	[h].[Duration] AS [Duration],
	[h].[InterviewerId] AS [InterviewerId],
	[h].[StartTime] AS [StartTime],
	[bt].Name as [BreakTypeName],
	[bt].IsPaid as [IsPaid],
	[s].[Name] as [ProjectId],
	[s].Description as [SurveyName],
	[p].[Name] AS [InterviewerName],
	[vcc].[Name] AS [CallCenterName]
FROM 
	BvTimeBreaksHistory [h]
	LEFT JOIN BvBreakType [bt] on [bt].Id = [h].BreakTypeId
	LEFT JOIN SelectedSurveySIDs_CTE ss ON h.SurveyId = ss.Item
LEFT JOIN BvPerson [p] ON [p].SID = [h].[InterviewerId]
LEFT JOIN [BvCallCenter] [vcc] on [vcc].[ID] = [h].[CallCenterId]
LEFT JOIN [BvSurvey] [s] on [h].SurveyId = s.SID
WHERE 
	[h].[StartTime] >= @StartDate AND
	[h].[StartTime] < @EndDate AND
	( ss.Item IS NOT NULL OR (@SurveySIDs IS NULL AND s.State <> 2) OR h.SurveyId = 0)
          
RETURN 0