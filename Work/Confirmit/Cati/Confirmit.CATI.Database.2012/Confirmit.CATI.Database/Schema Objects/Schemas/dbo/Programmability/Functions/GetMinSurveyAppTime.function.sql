CREATE FUNCTION [dbo].[GetMinSurveyAppTime]
(
	@SurveySID INT
)
RETURNS TABLE AS RETURN
( 
	SELECT MIN(Time) as minTime FROM BvAppointment WHERE @SurveySID = SurveySID AND State=1
)