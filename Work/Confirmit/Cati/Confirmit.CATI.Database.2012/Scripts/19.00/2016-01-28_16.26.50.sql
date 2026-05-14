
GO
ALTER PROCEDURE [dbo].[BvSpGetInterviewerPerformanceList] 
 @CallCenterId INT,
 @onlyLoggedIn bit,
 @bySurveys bit,
 @activeSurveysOnly bit 
AS 

IF(@onlyLoggedIn = 0)	
	BEGIN
		IF(@bySurveys = 0)
			SELECT InterviewerId, 
				   InterviewerName,
				   0 AS SurveyID,
				   '' AS ProjectID,
				   '' AS ProjectName,
				   SUM(InterviewingTime) AS InterviewingTime,
				   SUM(TotalInterviewCount) AS TotalInterviewCount, 
				   SUM(CompletedInterviewCount) AS CompletedInterviewCount,
				   SUM(CompletedInLastHourCount) AS  CompletedInLastHourCount
			FROM BvInterviewerPerformance ip INNER JOIN BvFnPerson_Get(@CallCenterId) p ON ip.[InterviewerId] = p.[SID]
			GROUP BY InterviewerId, InterviewerName
		ELSE
			IF(@activeSurveysOnly = 0)
				SELECT InterviewerId, 
					   InterviewerName,
					   ip.SurveyId AS SurveyID,
					   s.Name AS ProjectID,
					   s.[Description] AS ProjectName,
					   InterviewingTime,
					   TotalInterviewCount, 
					   CompletedInterviewCount,
					   CompletedInLastHourCount 
				FROM BvInterviewerPerformance ip INNER JOIN BvFnPerson_Get(@CallCenterId) p ON ip.[InterviewerId] = p.[SID]
												 INNER JOIN BvSurvey s ON ip.SurveyId = s.[SID]
			ELSE
				SELECT InterviewerId, 
					   InterviewerName,
					   ip.SurveyId AS SurveyID,
					   s.Name AS ProjectID,
					   s.[Description] AS ProjectName,
					   InterviewingTime,
					   TotalInterviewCount, 
					   CompletedInterviewCount,
					   CompletedInLastHourCount 
				FROM BvTasks bt INNER JOIN BvInterviewerPerformance ip ON bt.PersonSID = ip.[InterviewerId]
							 INNER JOIN BvFnPerson_Get(@CallCenterId) p ON ip.[InterviewerId] = p.[SID]
							 INNER JOIN BvSurvey s ON ip.SurveyId = s.[SID]
				WHERE ip.SurveyId = bt.SurveySID
	END
ELSE
	BEGIN
		IF(@bySurveys = 0)
			SELECT InterviewerId, 
				   InterviewerName,
				   0 AS SurveyID,
				   '' AS ProjectID,
				   '' AS ProjectName,
				   SUM(InterviewingTime) AS InterviewingTime,
				   SUM(TotalInterviewCount) AS TotalInterviewCount, 
				   SUM(CompletedInterviewCount) AS CompletedInterviewCount,
				   SUM(CompletedInLastHourCount) AS  CompletedInLastHourCount
			FROM BvTasks INNER JOIN BvInterviewerPerformance ip ON BvTasks.PersonSID = ip.[InterviewerId]
						 INNER JOIN BvFnPerson_Get(@CallCenterId) p ON ip.[InterviewerId] = p.[SID]
			GROUP BY InterviewerId, InterviewerName
		ELSE
			IF(@activeSurveysOnly = 0)
				SELECT InterviewerId, 
					   InterviewerName,
					   ip.SurveyId AS SurveyID,
					   s.Name AS ProjectID,
					   s.[Description] AS ProjectName,
					   InterviewingTime,
					   TotalInterviewCount, 
					   CompletedInterviewCount,
					   CompletedInLastHourCount 
					FROM BvTasks INNER JOIN BvInterviewerPerformance ip ON BvTasks.PersonSID = ip.[InterviewerId]
								 INNER JOIN BvFnPerson_Get(@CallCenterId) p ON ip.[InterviewerId] = p.[SID]
								 INNER JOIN BvSurvey s ON ip.SurveyId = s.[SID]
			ELSE
				SELECT InterviewerId, 
					   InterviewerName,
					   ip.SurveyId AS SurveyID,
					   s.Name AS ProjectID,
					   s.[Description] AS ProjectName,
					   InterviewingTime,
					   TotalInterviewCount, 
					   CompletedInterviewCount,
					   CompletedInLastHourCount 
				FROM BvTasks bt INNER JOIN BvInterviewerPerformance ip ON bt.PersonSID = ip.[InterviewerId]
							 INNER JOIN BvFnPerson_Get(@CallCenterId) p ON ip.[InterviewerId] = p.[SID]
							 INNER JOIN BvSurvey s ON ip.SurveyId = s.[SID]
				WHERE ip.SurveyId = bt.SurveySID
	END
GO
