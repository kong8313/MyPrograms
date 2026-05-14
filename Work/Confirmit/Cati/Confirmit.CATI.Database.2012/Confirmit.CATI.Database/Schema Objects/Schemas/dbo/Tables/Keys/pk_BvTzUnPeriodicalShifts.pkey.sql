ALTER TABLE [dbo].[BvTzUnPeriodicalShifts]
    ADD CONSTRAINT [pk_BvTzUnPeriodicalShifts] PRIMARY KEY CLUSTERED ([tz_id] ASC, [start_dt] ASC, [finish_dt] ASC, [owner_id] ASC, [shift_id] ASC, [type_id] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

