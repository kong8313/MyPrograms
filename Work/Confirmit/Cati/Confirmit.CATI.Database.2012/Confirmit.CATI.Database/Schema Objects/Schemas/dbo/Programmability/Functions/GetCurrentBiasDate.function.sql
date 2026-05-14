CREATE FUNCTION dbo.GetCurrentBiasDate (
                @Date datetime,
                @ReferenceDate datetime,
                @ReferenceDOW int
)
RETURNS datetime
AS
BEGIN

                DECLARE @CurrentDay datetime
                DECLARE @CurrentDayDOW int
                DECLARE @Delta int
                
                SET @CurrentDay = 
                                CONVERT(CHAR(4), YEAR(@Date)) +
                                '-' +
                                RIGHT('0'+ CONVERT(VARCHAR(2), MONTH(@ReferenceDate)), 2) +
                                '-01 ' +
                                CONVERT(VARCHAR(8), @ReferenceDate, 108) 
                
                SET @CurrentDayDOW =
                                DATEPART(dw, @CurrentDay) 
                
                SET @Delta = DATEPART(day, @ReferenceDate)
                
                IF @CurrentDayDOW < (@ReferenceDOW + 1)
                                SET @CurrentDay = 
                                                DATEADD(day, 
                                                                @ReferenceDOW - @CurrentDayDOW + 1, 
                                                                @CurrentDay)
                ELSE IF @CurrentDayDOW > (@ReferenceDOW + 1)
                                SET @CurrentDay = 
                                                DATEADD(day, 
                                                                8 + @ReferenceDOW - @CurrentDayDOW, 
                                                                @CurrentDay)
                
                 
                SET @CurrentDay = 
                                DATEADD(week, @Delta - 1, @CurrentDay)
                
                WHILE DATEPART(month, @CurrentDay) > 
                                DATEPART(month, @ReferenceDate) 
                
                                                SET @CurrentDay = 
                                                                DATEADD(week, - 1, @CurrentDay)

                RETURN (@CurrentDay)   
END