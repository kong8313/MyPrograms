CREATE PROCEDURE BvSpThresholds_insert
   @ObjectSID INT,
   @ThresholdsTypeID INT,
   @Amber INT,
   @Red INT
AS
   UPDATE BvThresholds
      SET Amber = @Amber,
          Red = @Red
      WHERE ObjectSID = @ObjectSID AND
            ThresholdsTypeID = @ThresholdsTypeID
   IF @@ROWCOUNT = 0
   INSERT INTO BvThresholds
   VALUES(@ObjectSID, @ThresholdsTypeID, @Amber, @Red)