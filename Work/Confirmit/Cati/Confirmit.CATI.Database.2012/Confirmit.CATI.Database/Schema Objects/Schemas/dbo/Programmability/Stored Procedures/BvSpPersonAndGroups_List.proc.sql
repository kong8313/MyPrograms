CREATE PROCEDURE [dbo].[BvSpPersonAndGroups_List]
        @ParentSID int,
        @SurveySid int,
        @Filter nvarchar(max) = NULL, -- Part of person's or group's name to filter by.
		@CallCenterID INT
AS

    SELECT  p.SID  as SID,
            p.Name as UserName,
            0      as isGroup,    
			0      as MembersCount,
			(SELECT COUNT(*) FROM BvPersonRel r with ( nolock )
					WHERE r.PersonSID = p.SID AND r.ObjectSID = @SurveySid )
				as IsAssignedOnCurrentSurvey,
            (SELECT COUNT(*) FROM BvSvySchedule sv where p.Sid = sv.ExplicitSid
                  and sv.SurveySid = @SurveySid)
				as CurSurvAssign,
            (SELECT COUNT(*) FROM BvSvySchedule sv where p.Sid = sv.ExplicitSid) 
				as AllSurvAssign,          
			(select count( distinct s.SID) from  BvSurvey s, BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID) a
				where  s.SID = a.SurveyId and a.PersonOrGroupId = p.SID  and s.State <> 2)
				as TotalAssignedSurveys 
            FROM   BvFnPerson_Get(@CallCenterID) p
            WHERE  p.SID IN (   SELECT  ObjectSID
                        FROM    BvMembership
                        WHERE   ContainerSID = @ParentSID )
                   AND (@Filter is NULL OR p.Name LIKE (@Filter) )

      UNION 

      select pg.sid     as SID,
             pg.name    as UserName,
             1          as isGroup,
       (SELECT COUNT(*) FROM BvMembership
              LEFT JOIN BvFnPerson_Get(@CallCenterID) p1 ON p1.SID = BvMembership.ObjectSID
     WHERE ContainerSID = pg.sid
           AND (@Filter is NULL OR p1.Name LIKE (@Filter) ) ) as MembersCount,
    1 as IsAssignedOnCurrentSurvey,
             0          as CurSurvAssign,
             0          as AllSurvAssign,
   (select count( distinct s.SID) from  BvSurvey s, BvFnPersonOrGroupAssignmentOnSurvey_Get(@CallCenterID) a
    where  s.SID = a.SurveyId and a.PersonOrGroupId = pg.SID and s.State <> 2)
                       as TotalAssignedSurveys

      from BvPersonGroup pg
      where pg.Sid in ( SELECT  ObjectSID
                        FROM    BvMembership
                        WHERE   ContainerSID = @ParentSID ) 
							AND pg.SID <> 4 /* Exclude '[All]' group. */
							AND (@Filter is NULL OR pg.Name LIKE (@Filter) )