CREATE PROCEDURE [dbo].[BvSpInterview_Insert]
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
		@IsSentToReview             bit,
		@DialTypeId                 tinyint
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
		IsSentToReview,
		DialTypeId )
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
			@IsSentToReview,
			@DialTypeId )
            
RETURN @ID