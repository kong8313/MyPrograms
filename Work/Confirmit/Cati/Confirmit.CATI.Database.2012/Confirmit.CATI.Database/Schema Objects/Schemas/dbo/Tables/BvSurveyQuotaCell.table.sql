CREATE TABLE [dbo].[BvSurveyQuotaCell] (
    [SurveyID]			INT           NOT NULL,
    [QuotaID]			INT           NOT NULL,
    [CellID]			INT			  NOT NULL,
    [Counter]			INT			  NOT NULL,
    [Limit]				INT			  NOT NULL,
    [LiveCounter]		INT			  NOT NULL,
    [LiveLimit]			INT			  NOT NULL,
    [IsDisabled]		BIT			  NOT NULL,
    [IsOpen]	    	BIT			  NOT NULL CONSTRAINT DF_BvSurveyQuotaCell_IsOpen DEFAULT 1,
    [XmlData]			Xml			  NULL,
    CONSTRAINT [PK_BvSurveyQuotaCell] PRIMARY KEY CLUSTERED ([SurveyID] ASC, [QuotaID] ASC, [CellID] ASC),
    CONSTRAINT [FK_BvSurveyQuotaCell_SurveyQuota] FOREIGN KEY ([SurveyID], [QuotaID]) REFERENCES [BvSurveyQuota]([SurveyID], [QuotaID]) ON DELETE CASCADE
);

