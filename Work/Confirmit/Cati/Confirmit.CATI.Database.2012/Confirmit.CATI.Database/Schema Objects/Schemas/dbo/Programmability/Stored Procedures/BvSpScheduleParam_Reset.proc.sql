CREATE PROCEDURE [dbo].[BvSpScheduleParam_ResetParam] 
 @SurveySID INT
AS
 UPDATE s 
  SET s.Value = d.Value
  FROM BvScheduleParam d
  INNER JOIN BvScheduleParam s
  ON d.ScheduleID = s.ScheduleID AND s.ParamID = d.ParamID AND d.SurveySID = 0 AND s.SurveySID = @SurveySID
 
 RETURN 0