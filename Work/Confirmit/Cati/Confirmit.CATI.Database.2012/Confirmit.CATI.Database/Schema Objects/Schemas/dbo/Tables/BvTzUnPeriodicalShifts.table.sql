CREATE TABLE [dbo].[BvTzUnPeriodicalShifts] (
    [shift_id]  INT           NOT NULL,
    [type_id]   INT           NOT NULL,
    [owner_id]  INT           NOT NULL,
    [tz_id]     INT           NOT NULL,
    [start_dt]  SMALLDATETIME NOT NULL,
    [finish_dt] SMALLDATETIME NOT NULL
);

