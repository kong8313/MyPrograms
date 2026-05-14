CREATE PROCEDURE BvSpGetAppointmentCount
      @batchID int
AS
   SELECT SurveySID,
          SurveyName,
          ProjectID,
          CountForShortInterval,
          CountForLongInterval
   FROM BvAppointmentCounters ac
   INNER JOIN BvTransferArrays ta ON (ta.BatchID = @batchID AND
                                      ta.ItemID = ac.SurveySID)