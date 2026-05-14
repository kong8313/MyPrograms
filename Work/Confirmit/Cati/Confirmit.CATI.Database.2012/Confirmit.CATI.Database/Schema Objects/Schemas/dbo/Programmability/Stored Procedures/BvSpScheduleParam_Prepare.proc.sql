CREATE PROCEDURE [dbo].[BvSpScheduleParam_Prepare]
	@ParamBatchID INT,
	@ParamID INT,
    @Name NVARCHAR(256),
	@Description NVARCHAR(MAX),
    @Type INT,
    @Value INT 
AS
    IF @ParamBatchID <= 0 
	BEGIN
		RAISERROR( '@ParamBatchID should be > 0 ', 16, 1 )
	END

	INSERT INTO BvScheduleParam( 
		ScheduleID, 
		SurveySID, 
		ParamID, 
		[Name], 
		Description, 
		Type, 
		Value ) 
    VALUES( 
		-@ParamBatchID, 
		0, 
		@ParamID, 
		@Name, 
		@Description, 
		@Type, 
		@Value )

RETURN (0)