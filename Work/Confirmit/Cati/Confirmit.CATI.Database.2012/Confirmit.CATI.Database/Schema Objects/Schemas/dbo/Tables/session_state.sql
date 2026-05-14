CREATE TABLE [dbo].[session_state](
	[session_id] [varchar](255) NOT NULL,
	[resource_id] [varchar](255) NOT NULL,
	[modified_date] [datetime] NOT NULL,
	[resource_data] [image] NULL,
 CONSTRAINT [pk_session_state] PRIMARY KEY CLUSTERED 
(
	[resource_id] ASC,
	[session_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]