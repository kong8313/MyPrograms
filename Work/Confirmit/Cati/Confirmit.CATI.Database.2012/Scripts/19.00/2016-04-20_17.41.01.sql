GO
PRINT N'Altering [dbo].[BvInterview]...';


GO
ALTER TABLE [dbo].[BvInterview]
    ADD [IsSentToReview] BIT CONSTRAINT [DF_BvInterview_IsSentToReview] DEFAULT (0) NOT NULL;


GO
PRINT N'Refreshing [dbo].[RestView_Survey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_Survey]';


GO
PRINT N'Altering [dbo].[BvSpInterview_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpInterview_Insert]
	    @ID                         int,
        @SurveySID                  int,        
        @TimeZoneID                 int,
        @TransientState             int,
        @LastCallPersonSID          int,
        @Duration                   int,
        @TelephoneNumber            varchar( 255 ),
        @RespondentName             nvarchar( 255 ),
        @LastCallTime               datetime,
        @ExtensionNumber            varchar( 255 ),
        @LastChannelID              tinyint,
        @ConfirmitSid               varchar(64) = '',
        @DialingMode                tinyint,
		@IsSentToReview             bit
AS

 IF (@TimeZoneID > 0)
    IF NOT EXISTS (SELECT TOP (1) 1 FROM BvTimezone WHERE ID = @TimeZoneID)
       BEGIN
         RAISERROR( 'Unrecognized time zone assigned to respondent, ensure the time zone is available from the active time zone list', 16, 1 )
         RETURN (-1)  
       END 


IF @TimeZoneID = 0 
        SET @TimeZoneID = NULL

INSERT BvInterview( 
		ID,
        SurveySID,        
        TimezoneID,
        TransientState,
        LastCallPersonSID,
        Duration,
        TelephoneNumber,
        RespondentName,
        LastCallTime,
        ExtensionNumber,
        BatchID,
        LastChannelID,
        ConfirmitSid,
        DialingMode,
		IsSentToReview )
        VALUES(
			@ID,
            @SurveySID,            
            @TimeZoneID,
            @TransientState,
            @LastCallPersonSID,
            @Duration,
            @TelephoneNumber,
            @RespondentName,
            @LastCallTime,
            @ExtensionNumber,
            0,
            @LastChannelID,
            @ConfirmitSid,
            @DialingMode,
			@IsSentToReview )
            
RETURN @ID
GO
PRINT N'Altering [dbo].[BvSpInterview_Update]...';


GO
ALTER PROCEDURE [dbo].[BvSpInterview_Update]
        @ID                         int,
        @SurveySID                  int,        
        @TimeZoneID                 int,
        @TransientState             int, 
        @LastCallPersonSID          int,
        @Duration                   int,
        @TelephoneNumber            varchar( 255 ),
        @RespondentName             nvarchar( 255 ),
        @LastCallTime               datetime,
        @ExtensionNumber            varchar( 255 ),
        @LastChannelID              tinyint,
        @DialingMode                tinyint,
		@DialerId					int,
		@IsSentToReview				bit
AS

 IF (@TimeZoneID > 0)
    IF NOT EXISTS (SELECT 1 FROM BvTimezone WHERE ID = @TimeZoneID)
       BEGIN
         RAISERROR( 'Unrecognized time zone assigned to respondent, ensure the time zone is available from the active time zone list', 16, 1 )
         RETURN (-1)  
       END 
       
UPDATE  BvInterview SET
        TimezoneID                  = CASE WHEN @TimeZoneID = 0 THEN NULL ELSE @TimeZoneID END,
        TransientState              = CASE WHEN @TransientState = 0 THEN TransientState ELSE @TransientState END,
        LastCallPersonSID           = @LastCallPersonSID,
        Duration                    = @Duration,
        TelephoneNumber             = @TelephoneNumber,
        RespondentName              = @RespondentName,
        LastCallTime                = @LastCallTime,
        ExtensionNumber             = @ExtensionNumber,
        LastChannelID               = @LastChannelID,
        DialingMode                 = @DialingMode,
		DialerId					= @DialerId,
		IsSentToReview				= @IsSentToReview
        WHERE SurveySID = @SurveySID AND ID = @ID
       
RETURN 0
GO

PRINT N'Creating [dbo].[BvSpInterviews_UpdateIsSentToReview_Batch]...';


GO
CREATE PROCEDURE [dbo].[BvSpInterviews_UpdateIsSentToReview_Batch]
@SurveySID INT, @BatchID INT, @IsSentToReview bit
AS
UPDATE BvInterview
   SET IsSentToReview = @IsSentToReview 
   FROM BvInterview i
   INNER JOIN BvTransferArrays ta ON 
   i.ID = ta.ItemID AND
   i.SurveySID = @SurveySID AND
   ta.BatchID = @BatchID
GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
 ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
 (
    SELECT 'Reviewer.WebServiceUrl', 'Confirmit Reviewer WS Url', 'System', 'Confirmit Reviewer Web Service Url.', 2, 0, NULL
    UNION ALL
    SELECT 'Reviewer.LimitOfAmountOfInterviewsPerSession', 'Maximum number of interviews for one session', 'System', 'Maximum number of interviews for one session.', 2, 0, '100'
    UNION ALL
    SELECT 'Reviewer.SessionUrlTemplate', 'Url template for session to review', 'System', 'Url template for session to review.', 2, 0, NULL
    UNION ALL
    SELECT 'Toggle.EnableReviewer', 'EnableReviewer', 'Toggle', 'Enable Reviewer', 3, 0, 'False'
 )
 INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  SELECT d.* FROM Data d LEFT JOIN BvSystemSettings ss ON d.[SystemName] = ss.[SystemName] WHERE ss.[SystemName] IS NULL
END


PRINT N'Update complete.';


GO
