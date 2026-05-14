CREATE PROCEDURE [dbo].[BvSpInterview_Update]
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