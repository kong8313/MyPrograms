-- Copy of BvCallHistory table with BIGINT ID
CREATE TABLE [dbo].[BvCallHistoryEx]
(
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [FiredTime]		DATETIME   NOT NULL,
    [ApptID]		INT             NULL,
    [ShiftTypeID]	INT             NULL,
    [InterviewID]	INT             NOT NULL,
    [SurveyId]		INT             NOT NULL,
    [ITS]		    SMALLINT		NULL,
    [DialingMode]   TINYINT		    NULL,
    [CallState]		SMALLINT        NULL,
    [Priority]		INT		        NULL,
    [TimeInShift]	DATETIME        NULL,
    [ExpireTime]	DATETIME	    NULL,
    [ExplicitSID]	INT             NULL,
    [ExplicitType]	TINYINT         NULL,
    [CellId]		INT		NULL,
    [OperationId]	INT		NOT NULL,
    [OperationType] 	TINYINT		NOT NULL,
    [CallCenterId]  	INT             NOT NULL,
	[BlockedByFcd]  AS (case when [OperationType]=(9) then CONVERT([bit],(1)) when [OperationType]=(11) then CONVERT([bit],(1)) when [OperationType]=(28) then CONVERT([bit],(1)) when [OperationType]=(29) then CONVERT([bit],(1)) else CONVERT([bit],(0)) end) PERSISTED NOT NULL,

    [DialTypeId] TINYINT NULL, 
    CONSTRAINT [PK_BVCallHistoryEx_ID] PRIMARY KEY CLUSTERED ([ID] ASC)
)
