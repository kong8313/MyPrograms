CREATE FUNCTION dbo.udf_AlertStatus_TAB_INT
(
    @Value INT,
    @Amber INT,
    @Red INT,
    @Type TINYINT
)
returns table
as return(
    SELECT ( 
        CASE
            WHEN @Type IS NULL /*Old*/ THEN
                CASE 
                    WHEN ((@Amber IS NULL) OR (@Red IS NULL)) THEN 0
                    WHEN ((@Red = @Amber) AND (@Value = @Red)) THEN 2
                    WHEN (@Red > @Amber) THEN 
                        CASE 
                            WHEN (@Value >= @Red) THEN 2
                            WHEN (@Value >= @Amber) THEN 1
                            ELSE 0
                        END
                    WHEN (@Red < @Amber) THEN  
                        CASE 
                            WHEN (@Value <= @RED) THEN 2
                            WHEN (@Value <= @Amber) THEN 1
                            ELSE 0
                        END
                    ELSE 0
                END
            WHEN @Red IS NULL AND @Amber IS NULL THEN NULL            
            WHEN @Type = 1 /*Ascending*/ THEN
                CASE 
                    WHEN @Red IS NOT NULL AND @Red < @Value THEN 2
                    WHEN @Amber IS NOT NULL AND @Amber < @Value THEN 1
                    ELSE 0
                END
            WHEN @Type = 2 /*Descending*/ THEN
                CASE 
                    WHEN @Red IS NOT NULL AND @Red > @Value THEN 2
                    WHEN @Amber IS NOT NULL AND @Amber > @Value THEN 1
                    ELSE 0
                END
        END) AS val
)
