CREATE TYPE [dbo].[BvInterviewTypeOrdered] AS TABLE(
	[OrderId]	int not null,
	[SurveySid] int not null,
	[IID] int not null
)