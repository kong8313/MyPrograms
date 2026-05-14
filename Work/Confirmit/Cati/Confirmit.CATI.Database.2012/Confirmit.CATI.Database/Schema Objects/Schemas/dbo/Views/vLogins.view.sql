create view dbo.vLogins
with schemabinding
as
    select ObjectSID as sid, DialTypeId, SurveySID, count_big(*) as cnt
        from dbo.BvLoginGroup
    group by ObjectSID, DialTypeId, SurveySID