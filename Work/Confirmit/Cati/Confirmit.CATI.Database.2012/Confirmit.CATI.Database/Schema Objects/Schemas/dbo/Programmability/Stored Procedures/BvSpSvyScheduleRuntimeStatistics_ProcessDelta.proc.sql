CREATE PROCEDURE [dbo].[BvSpSvyScheduleRuntimeStatistics_ProcessDelta]
AS
    CREATE TABLE #SvyScheduleRuntimeStatisticsDelta
    (
        SurveyId INT NOT NULL,
        ShiftTypeID INT NOT NULL,
        ExplicitSID INT NOT NULL,
        CallState INT NOT NULL,
        CountDelta INT NOT NULL
    )
    delete from BvSvyScheduleRuntimeStatisticsDelta OUTPUT deleted.* INTO #SvyScheduleRuntimeStatisticsDelta
    ;WITH stat as
    (
        SELECT SurveyId, ShiftTypeID, ExplicitSID, SUM( CountDelta ) as TotalCountDelta,
            SUM( CASE WHEN CallState IN ( 2 ) THEN CountDelta ELSE 0 END ) as FreeCountDelta
            FROM #SvyScheduleRuntimeStatisticsDelta
            GROUP BY SurveyId, ShiftTypeID, ExplicitSID
    )
    MERGE INTO BvSvyScheduleRuntimeStatistics as t
    USING (
        SELECT * FROM stat WHERE TotalCountDelta <> 0 OR FreeCountDelta <> 0
        ) as s (SurveyId, ShiftTypeID, ExplicitSID, TotalCountDelta, FreeCountDelta )
        ON t.SurveyId = s.SurveyId AND t.ShiftTypeID = s.ShiftTypeID AND t.ExplicitSID = s.ExplicitSID
        --delete records if counters is zero
        WHEN MATCHED AND t.TotalCount = -s.TotalCountDelta AND t.FreeCount = -s.FreeCountDelta
            THEN DELETE
        WHEN MATCHED
            THEN UPDATE
                SET t.TotalCount = t.TotalCount + s.TotalCountDelta,
                    t.FreeCount = t.FreeCount + s.FreeCountDelta
        WHEN NOT MATCHED BY TARGET
            THEN INSERT (SurveyId, ShiftTypeID, ExplicitSID, TotalCount, FreeCount )
                VALUES(SurveyId, ShiftTypeID, ExplicitSID, TotalCountDelta, FreeCountDelta );
    ;with AggregateSurveyDelta as
    (
        select SurveyId, SUM( CountDelta ) as Delta from #SvyScheduleRuntimeStatisticsDelta
        GROUP BY SurveyId
    )
    UPDATE BvAggregateSurvey
        SET ScheduledCallsCount = ScheduledCallsCount + Delta
        FROM BvAggregateSurvey t
        INNER JOIN AggregateSurveyDelta s ON t.SID = s.SurveyId