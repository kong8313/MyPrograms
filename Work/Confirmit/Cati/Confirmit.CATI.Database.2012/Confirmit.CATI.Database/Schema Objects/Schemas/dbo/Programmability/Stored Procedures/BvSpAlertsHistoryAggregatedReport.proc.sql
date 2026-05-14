CREATE PROCEDURE BvSpAlertsHistoryAggregatedReport
    @PersonIds NVARCHAR(MAX),
    @SurveyIds NVARCHAR(MAX),
    @StartDate DATETIME,
    @EndDate   DATETIME,
    @InterviewState TINYINT
 AS
	;WITH Persons AS
	(
		SELECT p.SID AS PersonId,
			   p.Name AS PersonName
		FROM dbo.utilSplitNumbers( ISNULL(@PersonIds, ''), ',') s
		INNER JOIN BvPerson p ON p.SID = s.Item
		
		UNION 

		SELECT p.SID AS PersonId,
		       p.Name AS PersonName
		FROM BvPerson p
		WHERE @PersonIds IS NULL
	),
	Surveys AS
	(
		SELECT s.Item AS SurveyId
		FROM dbo.utilSplitNumbers( ISNULL(@SurveyIds, ''), ',') s
	)
	SELECT p.PersonId,
		   p.PersonName,
           ISNULL(SUM(h.AnswerSubmissionAlert^1), 0) AnswerSubmissionAmberCounts,
           ISNULL(SUM(h.AnswerSubmissionAlert^0), 0) AnswerSubmissionRedCounts,
           ISNULL(SUM(h.QuickAnswerSubmissionAlert^1), 0) QuickAnswerSubmissionAmberCounts,
           ISNULL(SUM(h.QuickAnswerSubmissionAlert^0), 0) QuickAnswerSubmissionRedCounts
    FROM BvAnswerSubmissionAlertHistory h
    INNER JOIN Persons p ON p.PersonId = h.PersonId
    INNER JOIN Surveys s ON s.SurveyID = h.SurveyId
    WHERE SubmissionTime >= @startDate AND
          SubmissionTime <= @endDate AND
          (InterviewState = @InterviewState OR @InterviewState IS NULL)
    GROUP BY p.PersonId, p.PersonName
