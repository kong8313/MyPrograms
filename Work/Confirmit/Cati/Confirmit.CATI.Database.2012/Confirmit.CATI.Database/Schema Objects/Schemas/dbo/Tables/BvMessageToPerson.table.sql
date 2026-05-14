CREATE TABLE [dbo].[BvMessageToPerson](
	[MessageId]			INT 	NOT NULL,
	[InterviewerId]		INT 	NOT NULL,
	[IsSeen] BIT NULL, 
    CONSTRAINT [PK_BvMessageToPerson] PRIMARY KEY CLUSTERED 
	(
		[InterviewerId] ASC,
		[MessageId] ASC	
	)
	WITH 
	(
		PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON
	) 
	ON [PRIMARY]
) 