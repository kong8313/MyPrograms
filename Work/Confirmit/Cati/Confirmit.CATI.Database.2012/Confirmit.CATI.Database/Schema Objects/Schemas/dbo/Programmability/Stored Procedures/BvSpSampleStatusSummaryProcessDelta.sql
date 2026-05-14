CREATE PROCEDURE [dbo].[BvSpSampleStatusSummaryProcessDelta]
AS

	DECLARE @BvSampleStatusSummaryDelta TABLE
	(
		[ID]			BIGINT,
		[SurveySID]		INT NOT NULL,
		[ITS]			INT NOT NULL,
		[Cnt]			INT NOT NULL,
		[IsCati]		BIT NOT NULL
	);

	DELETE FROM [BvSampleStatusSummaryDelta] WITH (READPAST)
	OUTPUT DELETED.* INTO @BvSampleStatusSummaryDelta

	UPDATE aggrTbl
		SET aggrTbl.Cnt = aggrTbl.Cnt + data.Dif,
			alertStatus = dbo.udf_AlertStatus_INT( aggrTbl.Cnt + data.Dif, ThresholdDef.Amber, ThresholdDef.Red )
	FROM BvSampleStatusSummary aggrTbl
		INNER JOIN ( 
			SELECT SurveySID, ITS, SUM(Cnt) as Dif, IsCati FROM @BvSampleStatusSummaryDelta GROUP BY SurveySID, ITS, IsCati
				) as data
		ON aggrTbl.SurveySID = data.SurveySID AND aggrTbl.ITS = data.ITS AND aggrTbl.IsCati = data.IsCati
	LEFT JOIN BvThresholdITS as ThresholdDef
		ON ThresholdDef.SurveySID = 0 /*Use default thresholds, survey specific thresholds are not supported now*/ AND ThresholdDef.ITS = data.ITS 

RETURN 0
