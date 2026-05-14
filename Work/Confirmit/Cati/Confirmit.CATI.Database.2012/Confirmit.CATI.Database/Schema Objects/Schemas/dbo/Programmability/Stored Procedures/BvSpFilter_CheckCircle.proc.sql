CREATE PROCEDURE [dbo].[BvSpFilter_CheckCircle]
	@FilterSID    INTEGER,
	@SubFilterSID INTEGER
AS
SET NOCOUNT ON

	DECLARE @Ret INT = 0
    SELECT @Ret = COUNT(*) FROM dbo.udf_GetSubFilters(@SubFilterSID) WHERE SID = @FilterSID
    IF @Ret > 1
       SET @Ret = 1

RETURN @Ret
