PRINT N'Altering [dbo].[BvSpGetSurveyActivityWithAlerts]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetSurveyActivityWithAlerts]
   @BatchID int, @onlyActiveSurveys bit, @CustomITS1 INT, @CustomITS2 INT, 
   @CustomITS3 INT, @CustomITS4 INT, @CustomITS5 INT
AS  
	;WITH CustomITSes as (
		SELECT 
			SurveySID,
			SUM( CASE WHEN ITS = @CustomITS1 THEN Cnt ELSE NULL END ) as CustomITS1_Cnt,
			MAX( CASE WHEN ITS = @CustomITS1 THEN AlertStatus ELSE 0 END ) as CustomITS1_Alert,
			SUM( CASE WHEN ITS = @CustomITS2 THEN Cnt ELSE NULL END ) as CustomITS2_Cnt,
			MAX( CASE WHEN ITS = @CustomITS2 THEN AlertStatus ELSE 0 END ) as CustomITS2_Alert,
			SUM( CASE WHEN ITS = @CustomITS3 THEN Cnt ELSE NULL END ) as CustomITS3_Cnt,
			MAX( CASE WHEN ITS = @CustomITS3 THEN AlertStatus ELSE 0 END ) as CustomITS3_Alert,
			SUM( CASE WHEN ITS = @CustomITS4 THEN Cnt ELSE NULL END ) as CustomITS4_Cnt,
			MAX( CASE WHEN ITS = @CustomITS4 THEN AlertStatus ELSE 0 END ) as CustomITS4_Alert,
			SUM( CASE WHEN ITS = @CustomITS5 THEN Cnt ELSE NULL END ) as CustomITS5_Cnt,
			MAX( CASE WHEN ITS = @CustomITS5 THEN AlertStatus ELSE 0 END ) as CustomITS5_Alert
		FROM BvSampleStatusSummary
		GROUP BY SurveySID
	)
    SELECT asas.[SID] as SurveySID,
           asas.[Name] as ProjectID,
           asas.[Description]  as SurveyName,
           asas.[InterviewersLoggedCount],
           asas.[InterviewersLoggedCountPrev],
           asas.[NextAppointmentTime],
           asas.[TotalSampleSize], -- count of interview with 'fresh sample' its
           asas.[ScheduledCallsCount],
           asas.[ScheduledCallsCountPrev],
           asas.[SuspendedCallsCount],
           asas.[SuspendedCallsCountPrev],
           asas.[MinutesSpentWorkingOnSurvey],
           asas.[AssignedInterviewersCount],
           asas.[StrikeRate],
           asas.[CountCalls],
           asas.[AvgDuration],
           asas.[AlertStatusOfInterviewersLoggedCount],
           asas.[AlertStatusOfNextAppointmentTime],
           asas.[AlertStatusOfTotalSampleSize],
           asas.[AlertStatusOfScheduledCallsCount],
           asas.[AlertStatusOfSuspendedCallsCount],
           asas.[AlertStatusOfMinutesSpentWorkingOnSurvey],
           asas.[AlertStatusOfAssignedInterviewersCount],
           asas.[AlertStatusOfStrikeRate],
           asas.[AlertStatusOfCountCalls],
           asas.[MaxStatusOfITSAlerts],
           BvSurvey.[Target],
           asas.[StrikeRate1h],
           asas.[CountCalls1h],
           asas.[AvgDuration1h],
           asas.[MinutesSpentWorkingOnSurveyInDay],
           asas.[AlertStatusOfStrikeRate1h],
           asas.[AlertStatusOfCountCalls1h],
           CustomITSes.[CustomITS1_Cnt],
           CustomITSes.[CustomITS1_Alert],
           CustomITSes.[CustomITS2_Cnt],
           CustomITSes.[CustomITS2_Alert],
           CustomITSes.[CustomITS3_Cnt],
           CustomITSes.[CustomITS3_Alert],
           CustomITSes.[CustomITS4_Cnt],
           CustomITSes.[CustomITS4_Alert],
           CustomITSes.[CustomITS5_Cnt],
           CustomITSes.[CustomITS5_Alert]
    FROM BvTransferArrays ta
    INNER JOIN BvAggregateSurveyAlertStatus asas
        ON ta.ItemID = asas.SID
    INNER JOIN BvSurvey 
        ON (BvSurvey.SID = asas.SID)
	LEFT JOIN CustomITSes 
		ON asas.SID = CustomITSes.SurveySID
    WHERE ta.BatchID = @BatchID
	AND BvSurvey.State <> 2
	AND	InterviewersLoggedCount >= @onlyActiveSurveys
GO
PRINT N'Update complete.';


GO
