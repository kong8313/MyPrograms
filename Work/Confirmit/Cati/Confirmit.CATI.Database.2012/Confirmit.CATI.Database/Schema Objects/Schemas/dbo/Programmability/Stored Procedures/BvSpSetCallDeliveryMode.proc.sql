CREATE PROCEDURE [dbo].[BvSpSetCallDeliveryMode]
    @SurveyId INT,
	@Mode BIT -- 0 - order by interview id, 1 - random order
AS
    DECLARE @PreviosMode BIT
    
	UPDATE BvSurvey
	SET IsRandomCallDeliveryEnabled = @Mode,
	    @PreviosMode = IsRandomCallDeliveryEnabled
	WHERE SID = @SurveyId
	
	IF @PreviosMode != @Mode
	BEGIN
	    UPDATE BvSvySchedule
	    SET CallOrder = CASE WHEN @Mode = 0 THEN InterviewId
	                         ELSE CHECKSUM(NEWID()) % 2147483647
	                    END
	    WHERE SurveySid = @SurveyId
	END
RETURN 0