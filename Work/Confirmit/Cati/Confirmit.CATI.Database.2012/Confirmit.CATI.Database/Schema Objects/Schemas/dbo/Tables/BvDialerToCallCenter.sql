CREATE TABLE [dbo].[BvDialerToCallCenter]
(
	[CallCenterId] INT NOT NULL,
    [DialerId] INT NOT NULL,
	CONSTRAINT [PK_BvDialerToCallCenter] PRIMARY KEY CLUSTERED 
	(
		[CallCenterId] ASC,
		[DialerId] ASC	
	) ON [PRIMARY],
	CONSTRAINT [FK_BvDialerToCallCenter_BvCallCenter] FOREIGN KEY ([CallCenterId]) REFERENCES [BvCallCenter]([ID]) ON DELETE CASCADE, 
    CONSTRAINT [FK_BvDialerToCallCenter_BvDialers] FOREIGN KEY ([DialerId]) REFERENCES [BvDialers]([Id]) ON DELETE CASCADE
)
