CREATE PROCEDURE BvSpCall_Enable
	@SurveySID INT,
	@BatchID INT,
	@IsFcdMode BIT,
	@Enable BIT
AS
IF  @Enable = 1 
BEGIN
	
	DECLARE @Query NVARCHAR(MAX)	
	
    SET @Query = 'UPDATE BvSvySchedule SET CallState = 
    CASE WHEN (
            (
            SELECT DISTINCT 1 FROM BvInterviewQuotaCell AS icell 
            INNER JOIN BvSurveyQuotaCell AS qcell 
            ON icell.SurveyID = qcell.SurveyID AND icell.QuotaID = qcell.QuotaID AND icell.CellID = qcell.CellID AND qcell.IsOpen = 0
            WHERE icell.SurveyID = @SurveySID AND icell.InterviewId = ta.ItemID 
            ) IS NOT NULL
        )
        THEN 1
        ELSE 2 
    END
    FROM BvTransferArrays ta
    WHERE	BvSvySchedule.SurveySID = @SurveySID AND
    BvSvySchedule.InterviewID = ta.ItemID AND
    ta.BatchID = @BatchID AND
    BvSvySchedule.CallState IN (1,3)';

	EXEC sp_executesql @Query, N'@SurveySID INT, @BatchID INT', @SurveySID, @BatchID
END
ELSE
BEGIN
	IF @IsFcdMode = 1 
	BEGIN
		UPDATE BvSvySchedule SET CallState = 1/*Disabled by FCD*/
		FROM BvTransferArrays ta
		WHERE	BvSvySchedule.SurveySID = @SurveySID AND
				BvSvySchedule.InterviewID = ta.ItemID AND
				ta.BatchID = @BatchID AND
				BvSvySchedule.CallState IN ( -2/*In dialer*/, 2/*Normal*/)
	END
	ELSE
	BEGIN
		UPDATE BvSvySchedule SET CallState = 3/*Disabled by User*/
		FROM BvTransferArrays ta
		WHERE	BvSvySchedule.SurveySID = @SurveySID AND
				BvSvySchedule.InterviewID = ta.ItemID AND
				ta.BatchID = @BatchID AND
				BvSvySchedule.CallState IN ( -2/*In dialer*/, 1/*Disabled by FCD*/, 2/*Normal*/)
	END
END  