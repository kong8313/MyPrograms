CREATE FUNCTION dbo.udf_AlertStatus_INT
(
    @Value INT,
    @Amber INT,
    @Red INT
)
RETURNS INT
BEGIN
    IF( (@Amber IS NULL) OR (@Red IS NULL) )
    BEGIN
       RETURN (0)
    END

    IF @Red = @Amber 
    BEGIN
        IF @Value = @Red
            RETURN (2)
    END
    ELSE IF @Red > @Amber 
    BEGIN
        IF @Value >= @Red
            RETURN (2)
        ELSE IF @Value >= @Amber
            RETURN (1)
    END
    ELSE --IF @Red < @Amber 
    BEGIN
        IF @Value <= @Red
            RETURN (2)
        ELSE IF @Value <= @Amber
            RETURN (1)
    END
    RETURN (0)
END