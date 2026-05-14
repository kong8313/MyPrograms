CREATE TYPE BvSurveyQuotaCellType AS TABLE
(
    [SurveyID] int not null,
    [QuotaID] int not null,
    [CellID] int not null,
	[Counter] int not null,
	[Limit] int not null,
	[LiveCounter] int not null,
    [LiveLimit] int not null,
    [IsDisabled] bit not null,
    [IsOpen] bit not null,
    [XmlData] xml not null
)