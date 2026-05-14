CREATE PROCEDURE [BvSpReportSampleStatusSummaryForDatesRange]
@SurveySID INT, 
@StartDate DATETIME,
@EndDate DATETIME
AS

DECLARE @Total INT

DECLARE @ItsCounts TABLE
(
	[ID] INT,
	[Name] VARCHAR(MAX),
	[Count] INT,
	[Percent] VARCHAR(MAX),
	[Total] INT
)

INSERT INTO @ItsCounts
SELECT 
	h.ITS AS [ID],
	st.Name AS [Name],
	COUNT(*) AS [Count],
	CAST(CAST( (COUNT(*)*1.0/(SUM(COUNT(*)) OVER()) * 100.0) as decimal(5,2)) as VARCHAR(MAX)) as [Percent],
	SUM(COUNT(*)) OVER()
FROM
	(
		SELECT ITS, SurveyId, FiredTime FROM Bvhistory
		UNION ALL
		SELECT ITS, SurveyId, FiredTime FROM BvViewBothCallHistories
		WHERE ITS = 15 OR ITS = 25
	) AS h
JOIN BvSurvey s
	ON s.SID = h.SurveyId
JOIN BvState st
	ON s.StateGroupID = st.StateGroupID and h.ITS = st.StateID
WHERE h.SurveyId = @SurveySID AND h.FiredTime BETWEEN @StartDate AND @EndDate
GROUP BY h.ITS, st.Name
ORDER BY h.ITS

SELECT TOP 1 @Total = [Total] FROM @ItsCounts
SELECT [ID], [Name], [Count], [Percent] FROM @ItsCounts

RETURN @Total