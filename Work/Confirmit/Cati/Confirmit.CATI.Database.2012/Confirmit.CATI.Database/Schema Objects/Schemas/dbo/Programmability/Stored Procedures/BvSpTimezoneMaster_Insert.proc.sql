CREATE PROCEDURE [dbo].[BvSpTimezoneMaster_Insert]
        @ID                 int,
        @Name               nvarchar( 255 ),
        @Bias               int,
        @DaylightType       int,
        @StandardName       nvarchar( 255 ),
        @StandardStart      datetime,
        @StandardDayOfWeek  int,
        @StandardBias       int,
        @DaylightName       nvarchar( 255 ),
        @DaylightStart      datetime,
        @DaylightDayOfWeek  int,
        @DaylightBias       int
AS

    BEGIN TRANSACTION
    IF @ID IS NULL BEGIN
        SELECT @ID = MAX( ID ) FROM BvTimezoneMaster
        IF @ID IS NULL BEGIN
            SELECT @ID = 0
        END
        SELECT @ID = @ID + 1
    END
    INSERT BvTimezoneMaster(
            ID,
            Name,
            Bias,
            DaylightType,
            StandardName,
            StandardStart,
            StandardDayOfWeek,
            StandardBias,
            DaylightName,
            DaylightStart,
            DaylightDayOfWeek,
            DaylightBias ) VALUES(
            @ID,
            @Name,
            @Bias,
            @DaylightType,
            @StandardName,
            @StandardStart,
            @StandardDayOfWeek,
            @StandardBias,
            @DaylightName,
            @DaylightStart,
            @DaylightDayOfWeek,
            @DaylightBias )
    COMMIT TRANSACTION

    RETURN @ID