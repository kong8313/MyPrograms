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
