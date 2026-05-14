GO
PRINT N'Creating [dbo].[BvSvyScheduleRuntimeStatistics]...';


GO
CREATE TABLE [dbo].[BvSvyScheduleRuntimeStatistics] (
    [SurveyId]    INT NOT NULL,
    [ShiftTypeID] INT NOT NULL,
    [ExplicitSID] INT NOT NULL,
    [TotalCount]  INT NOT NULL,
    [FreeCount]   INT NOT NULL,
    CONSTRAINT [PK_BvSvyScheduleRuntimeStatistics] PRIMARY KEY CLUSTERED ([SurveyId] ASC, [ShiftTypeID] ASC, [ExplicitSID] ASC)
);

GO
PRINT N'Initialize [BvSvyScheduleRuntimeStatistics]...';

INSERT INTO BvSvyScheduleRuntimeStatistics(SurveyId, ShiftTypeID, ExplicitSID, TotalCount, FreeCount )
	SELECT SurveySID, ShiftTypeID, ExplicitSID, COUNT(*), SUM( CASE WHEN CallState = 2 THEN 1 ELSE 0 END )
		FROM BvSvySchedule 
		WHERE CallState IN ( -2, 2 )
		GROUP BY SurveySID, ShiftTypeID, ExplicitSID

GO
PRINT N'Creating [dbo].[BvSvyScheduleRuntimeStatisticsDelta]...';


GO
CREATE TABLE [dbo].[BvSvyScheduleRuntimeStatisticsDelta] (
    [SurveyId]    INT NOT NULL,
    [ShiftTypeID] INT NOT NULL,
    [ExplicitSID] INT NOT NULL,
    [CallState]   INT NOT NULL,
    [CountDelta]  INT NOT NULL
);


GO
PRINT N'Altering [dbo].[BvTrBvSvySchedule_CallsDelete]...';


GO
ALTER TRIGGER [BvTrBvSvySchedule_CallsDelete] ON [dbo].[BvSvySchedule]
FOR DELETE
AS 
BEGIN
	SET NOCOUNT ON
                                      
    INSERT INTO BvSvyScheduleRuntimeStatisticsDelta(SurveyId, ShiftTypeID, ExplicitSID, CallState, CountDelta )
        SELECT  SurveySid, ShiftTypeId, ExplicitSID, CallState, -COUNT(*) as CountDelta
                FROM deleted
                WHERE CallState IN ( -2, 2 )
                GROUP BY SurveySid, ShiftTypeId, ExplicitSID, CallState

END
GO
PRINT N'Altering [dbo].[BvTrBvSvySchedule_CallsInsert]...';


GO
ALTER TRIGGER [BvTrBvSvySchedule_CallsInsert] ON [dbo].[BvSvySchedule]
AFTER INSERT
AS 
BEGIN
	SET NOCOUNT ON
	
	INSERT INTO BvSvyScheduleRuntimeStatisticsDelta(SurveyId, ShiftTypeID, ExplicitSID, CallState, CountDelta )
        SELECT  SurveySid, ShiftTypeId, ExplicitSID, CallState, COUNT(*) as CountDelta
                FROM inserted
                WHERE CallState IN ( -2, 2 )
                GROUP BY SurveySid, ShiftTypeId, ExplicitSID, CallState

END
GO
PRINT N'Altering [dbo].[BvTrBvSvySchedule_CallsUpdate]...';


GO
ALTER TRIGGER [dbo].[BvTrBvSvySchedule_CallsUpdate] ON [dbo].[BvSvySchedule]
FOR UPDATE
AS
BEGIN
    SET NOCOUNT ON
     
    IF UPDATE( SurveySid ) OR UPDATE( ShiftTypeId ) OR UPDATE( ExplicitSID ) OR UPDATE( CallState )
    BEGIN
        ;WITH stat AS
        (
            SELECT  SurveySid, ShiftTypeId, ExplicitSID, CallState, COUNT(*) as CountDelta
                    FROM inserted
                    WHERE CallState IN ( -2, 2 )
                    GROUP BY SurveySid, ShiftTypeId, ExplicitSID, CallState
            UNION ALL
            SELECT  SurveySid, ShiftTypeId, ExplicitSID, CallState, -COUNT(*)
                    FROM deleted
                    WHERE CallState IN ( -2, 2 )
                    GROUP BY SurveySid, ShiftTypeId, ExplicitSID, CallState
        )
        INSERT INTO BvSvyScheduleRuntimeStatisticsDelta(SurveyId, ShiftTypeID, ExplicitSID, CallState, CountDelta )
            SELECT SurveySid, ShiftTypeId, ExplicitSID, CallState, SUM(CountDelta) as Delta
                FROM stat
                GROUP BY SurveySid, ShiftTypeId, ExplicitSID, CallState
                HAVING SUM(CountDelta) <> 0
                 
    END
END
GO
PRINT N'Altering [dbo].[BvSpGetLiveShifts]...';


GO
ALTER procedure [dbo].[BvSpGetLiveShifts]
@utc smalldatetime,    -- in utc time
@tz_local INT
as
set nocount on
declare @date1 int
DECLARE @ShiftTypeNone INT = -2147483648; --None constant for bvSvySchedule.ShiftTypeID

 
set @date1 = @@DATEFIRST
set DATEFIRST 7
 
    create table #temp_tz ( 
        tz_id    int,
        ltStart  smalldatetime,
        minStart int
    )
 
    create table #active
    (
        [ID] int not null,
        ScheduleID int not null,
                tz_id int not null
    )
 
 
    -- check in future
    set @utc = dateadd( minute, 1, @utc )
 
    -- insert into temp normalize date by all timezone
    -- normalize date - time in minute from start of week
    -- = day_of_week * 24 * 60 + hour * 60 + minute
    insert into #temp_tz select [ID] AS TzID, ltStart,
                        ( DATEPART( dw, ltStart ) - 1 ) * 1440 + 
                        DATEPART( hour, ltStart ) * 60 + 
                        DATEPART( minute, ltStart ) as minStart
                      from ( select  [ID], dbo.UTC2LT( @utc, Bias, DaylightType,
                                StandardDayOfWeek, StandardStart, StandardBias,
                                DaylightDayOfWeek, DaylightStart, DaylightBias ) as ltStart
                             from BvTimezone ) s1
 
        --select * from #temp_tz
 
 
 
    -- insert periodical active shifts info
    insert into #active
        select distinct z.[ID], tzs.owner_id, tzs.tz_id
        from #temp_tz
        inner join BvTzPeriodicalShifts tzs on
            #temp_tz.tz_id = tzs.tz_id
              and ( #temp_tz.minStart >= tzs.start_dt 
              and #temp_tz.minStart < tzs.finish_dt OR 
              #temp_tz.minStart + 10080/*week*/ >= tzs.start_dt 
              and #temp_tz.minStart + 10080/*week*/ < tzs.finish_dt)
        inner join BvShiftZones z on
              ( z.TimeZoneID = tzs.tz_id or
              ( z.TimeZoneID = 0 and tzs.tz_id = @tz_local ) )
              and z.ShiftTypeID = tzs.type_id
 
    -- delete shifts which fits exclusions
        delete from #active 
        from  #active a 
                        join BvTzUnPeriodicalShifts utzs on
                                a.tz_id = utzs.tz_id
                                 and a.ScheduleID = utzs.owner_id
                        join #temp_tz on #temp_tz.tz_id = utzs.tz_id
                        
                        where 
                                #temp_tz.ltStart >= utzs.start_dt and #temp_tz.ltStart < utzs.finish_dt
                        
                                                        
 
    set DATEFIRST @date1
    drop table #temp_tz

    -- insert timezones for [AnyValid] calls
    insert into #active
        select distinct -z.TimeZoneID, a.ScheduleID, a.tz_id
        from #active a, BvShiftZones z
        where a.[ID] = z.[ID]
    -- insert fictive shift for [None] calls
    insert into #active
        select @ShiftTypeNone, ScheduleID, 0 FROM BvSchedule
 
    select a.[ID], b.SID
        from  BvSurvey b
		inner join #active a
		on a.ScheduleID = b.ScheduleID
        WHERE b.State = 1 /* survey opened */
			  AND EXISTS( SELECT 1 FROM BvSvyScheduleRuntimeStatistics srs WHERE b.SID = srs.SurveyId AND srs.ShiftTypeID = a.ID AND srs.FreeCount > 0)

 
return (0)
GO
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpSvyScheduleRuntimeStatistics_ProcessDelta]...';


GO
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
GO
PRINT N'Refreshing [dbo].[BvSpQueueUpSheduleTask3]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpQueueUpSheduleTask3]';


GO
PRINT N'Update complete.';


GO
