PRINT N'Altering [dbo].[BvSpGetNextAvailableDialer]...';
GO

ALTER PROCEDURE [dbo].[BvSpGetNextAvailableDialer]
	@SurveyId int
AS
	SET NOCOUNT ON
BEGIN
       DECLARE @DialerId INT

       SELECT @dialerid = DialerId 
       FROM BvSurvey s  WITH (UPDLOCK)
       JOIN BvDialers d
              ON s.DialerId=d.Id
       WHERE s.SID = @SurveyId AND d.IsActive = 1 AND DialerOperationalStateNotification = 1

       IF @DialerId is null
       BEGIN
              ;WITH newdialer AS 
              (
                      SELECT TOP 1 id,IsActive FROM BvDialers
                      WHERE IsActive = 1 AND DialerOperationalStateNotification = 1
                      ORDER BY LastSelected 
              )
              UPDATE newdialer SET IsActive = 1, @DialerId = id   --just fake update to increase timestamp

              UPDATE BvSurvey SET DialerId = @dialerid WHERE SID = @SurveyId
       END

       SELECT ISNULL(@DialerId, -1)
END
GO

PRINT N'Update complete.';
GO
