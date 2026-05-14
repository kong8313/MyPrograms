CREATE FUNCTION dbo.udf_AlertStatus_TAB_DATETIME
(
    @Value DATETIME,
    @Now DATETIME,
    @Amber INT,
    @Red INT
)
returns table
as return(
    SELECT ( CASE WHEN ((@Amber IS NULL) OR (@Red IS NULL)) THEN 0
                  WHEN ((@Red = @Amber) AND (@Value = DATEADD(second, - @Red, @Now))) THEN 2
                  WHEN (@Red > @Amber) THEN (CASE WHEN (@Value <= DATEADD(second, - @Red, @Now)) THEN 2
                                                  WHEN (@Value <= DATEADD(second, - @Amber, @Now)) THEN 1
                                                  ELSE 0
                                             END)
                  WHEN (@Red < @Amber) THEN (CASE WHEN (@Value >= DATEADD(second, - @Red, @Now)) THEN 2
                                                  WHEN (@Value >= DATEADD(second, - @Amber, @Now)) THEN 1
                                                  ELSE 0
                                             END)
                  ELSE 0
             END ) AS val
)
