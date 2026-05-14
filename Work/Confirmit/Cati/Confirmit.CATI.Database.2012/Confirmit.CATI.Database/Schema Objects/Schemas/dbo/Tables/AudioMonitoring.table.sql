CREATE TABLE [dbo].[AudioMonitoring]
(
    [SupervisorName]  [nvarchar](255) NOT NULL,	-- who monitors?
    [InterviewerSID]  [int]           NOT NULL,				-- who is being monitored?
    [TelephoneNumber] [nvarchar](255) NOT NULL,				-- supervisor telephone number
    [SessionID]       [nvarchar](255) NOT NULL,				-- the audio monitoring sesion Id
    [MonitorMode]     [int]           NOT NULL CONSTRAINT DF_AudioMonitoring_MonitorMode DEFAULT 0,	-- the audio monitoring mode (Listening, Coaching, Barging)
	CONSTRAINT PK_AudioMonitoring_SupervisorName PRIMARY KEY (SupervisorName)
)
