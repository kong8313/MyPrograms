CREATE VIEW [dbo].[RestView_Project]
	AS 
	SELECT
	    s.Name as [ProjectId],
		s.Description as [ProjectName],
		ISNULL(sample.Count, 0) SampleSize,
		s.State as [State]
	FROM
	    [BvSurvey] s
		LEFT JOIN (SELECT COUNT(*) as Count, SurveySID FROM BvInterview group by SurveySid ) as sample on s.SID = sample.SurveySID 