CREATE TABLE [dbo].[BvSurveyQuota] (
    [SurveyID]			INT           NOT NULL,
    [QuotaID]			INT           NOT NULL,
    [Name]				VARCHAR (16)  NOT NULL,
    [TableName]			VARCHAR (16)  NOT NULL,
    [Email]				VARCHAR (128) NULL,
    [IsFCD]				INT           NOT NULL, -- move to step file CONSTRAINT DF_BvSurveyQuota_IsFCD DEFAULT (0),
    [IsOptimistic]		BIT           NOT NULL, -- move to step file CONSTRAINT DF_BvSurveyQuota_IsOptimistic DEFAULT (0),
	[XmlData]			Xml			  NULL,
    CONSTRAINT [PK_BvSurveyQuota] PRIMARY KEY CLUSTERED ([SurveyID] ASC, [QuotaID] ASC), 
    CONSTRAINT [FK_BvSurveyQuota_Survey] FOREIGN KEY ([SurveyID]) REFERENCES [BvSurvey]([SID]) ON DELETE CASCADE
);

