CREATE PROCEDURE [dbo].[BvSpScheduleParam_Set]
	@SurveySID INT,
	@ParamID INT,
	@Value INT
AS
	UPDATE BvScheduleParam
		SET Value = @Value
		WHERE	SurveySID = @SurveySID AND 
				ParamID = @ParamID 
				
	IF @@ROWCOUNT = 0 
		RAISERROR( 'Custom parameter with id = %d for survey with SID = %d not found', 12, 1, @ParamID, @SurveySID )
RETURN (0)