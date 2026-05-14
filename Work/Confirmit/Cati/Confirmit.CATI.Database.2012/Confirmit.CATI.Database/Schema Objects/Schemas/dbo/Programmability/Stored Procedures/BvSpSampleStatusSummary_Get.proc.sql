CREATE PROCEDURE [dbo].[BvSpSampleStatusSummary_Get]
@SurveySID INT,
@onlyCatiInterviews BIT = 0
AS
    DECLARE @StateGroupID INT
    SELECT @StateGroupID = StateGroupID FROM BvSurvey WHERE SID = @SurveySID

     SELECT  BvSampleStatusSummary.SurveySID as SurveySID,
            BvSampleStatusSummary.ITS  as StateID,
            BvState.Name as StateName,
            SUM(BvSampleStatusSummary.Cnt) as Cnt,
            Max(BvSampleStatusSummary.AlertStatus) as AlertStatus
        FROM BvSampleStatusSummary 
        INNER JOIN BvState
            ON  BvState.StateID = BvSampleStatusSummary.ITS AND 
                BvSampleStatusSummary.SurveySID = @SurveySID AND 
                BvState.StateGroupID = @StateGroupID
        WHERE @onlyCatiInterviews = 0 OR IsCati = 1
		GROUP BY 
            SurveySID, 
            BvSampleStatusSummary.ITS, 
            BvState.Name

    RETURN(0)