GO
PRINT N'Creating [dbo].[BvDialerToCallCenter]...';


GO
CREATE TABLE [dbo].[BvDialerToCallCenter] (
    [CallCenterId] INT NOT NULL,
    [DialerId]     INT NOT NULL,
    CONSTRAINT [PK_BvDialerToCallCenter] PRIMARY KEY CLUSTERED ([CallCenterId] ASC, [DialerId] ASC) ON [PRIMARY]
);


GO
PRINT N'Creating [dbo].[FK_BvDialerToCallCenter_BvDialers]...';


GO
ALTER TABLE [dbo].[BvDialerToCallCenter] WITH NOCHECK
    ADD CONSTRAINT [FK_BvDialerToCallCenter_BvDialers] FOREIGN KEY ([DialerId]) REFERENCES [dbo].[BvDialers] ([Id]) ON DELETE CASCADE;


GO
PRINT N'Creating [dbo].[FK_BvDialerToCallCenter_BvCallCenter]...';


GO
ALTER TABLE [dbo].[BvDialerToCallCenter] WITH NOCHECK
    ADD CONSTRAINT [FK_BvDialerToCallCenter_BvCallCenter] FOREIGN KEY ([CallCenterId]) REFERENCES [dbo].[BvCallCenter] ([ID]) ON DELETE CASCADE;


GO
PRINT N'Altering [dbo].[BvSpGetNextAvailableDialer]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetNextAvailableDialer]
	@SurveyId int, 
	@DialTypeId int,
    @DialerIds NVARCHAR(MAX) = NULL
AS
	SET NOCOUNT ON
BEGIN
       DECLARE @DialerId INT = NULL
       DECLARE @DialerIdsNullOrEmpty bit = 0
       DECLARE @TempDialerIds TABLE (Id nvarchar(100))
       
       IF(NULLIF(@DialerIds, '') IS NULL)
       BEGIN
            SET @DialerIdsNullOrEmpty = 1;
       END

       INSERT INTO @TempDialerIds (Id) SELECT value FROM STRING_SPLIT(@DialerIds, ',')

	   MERGE BvSurveyDialer WITH (UPDLOCK) as t
			USING ( SELECT @SurveyId, @DialTypeId ) as s( SurveyId, DialTypeId )
			ON (t.SurveyId = s.SurveyId AND t.DialTypeId = s.DialTypeId)
			WHEN MATCHED THEN 
				UPDATE SET @DialerId = t.DialerId
			WHEN NOT MATCHED BY TARGET THEN
				INSERT(SurveyId, DialTypeId, DialerId) VALUES( SurveyId, DialTypeId, 0 );


       SET @DialerId = ( SELECT Id FROM BvDialers d 
           WHERE d.DialTypeId = @DialTypeId 
           AND d.IsActive = 1 
           AND DialerOperationalStateNotification = 1 
           AND d.Id = @dialerid 
           AND (@DialerIdsNullOrEmpty = 1 OR @dialerid IN (SELECT Id FROM @TempDialerIds)))

       IF @DialerId is null
       BEGIN
              ;WITH newdialer AS 
              (
                      SELECT TOP 1 id,IsActive FROM BvDialers
                      WHERE DialTypeId = @DialTypeId AND IsActive = 1 AND DialerOperationalStateNotification = 1
                      AND (@DialerIdsNullOrEmpty = 1 OR id IN (SELECT Id FROM @TempDialerIds))
                      ORDER BY LastSelected 
              )
              UPDATE newdialer SET IsActive = 1, @DialerId = id   --just fake update to increase timestamp

              UPDATE BvSurveyDialer SET DialerId = @dialerid WHERE SurveyId = @SurveyId AND DialTypeId = @DialTypeId
       END

       SELECT ISNULL(@DialerId, -1)
END
GO
PRINT N'Checking existing data against newly created constraints';


GO



GO
ALTER TABLE [dbo].[BvDialerToCallCenter] WITH CHECK CHECK CONSTRAINT [FK_BvDialerToCallCenter_BvDialers];

ALTER TABLE [dbo].[BvDialerToCallCenter] WITH CHECK CHECK CONSTRAINT [FK_BvDialerToCallCenter_BvCallCenter];


GO
PRINT N'Update complete.';


GO
