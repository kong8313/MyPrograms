CREATE PROCEDURE BvSpThresholds_delete
   @ObjectSID INT,
   @ThresholdsTypeID INT
AS
   DELETE BvThresholds
   WHERE ObjectSID = @ObjectSID AND
         ThresholdsTypeID = @ThresholdsTypeID