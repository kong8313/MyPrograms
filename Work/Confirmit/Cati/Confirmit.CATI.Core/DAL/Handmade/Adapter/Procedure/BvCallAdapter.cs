using System;
using System.Collections.Generic;
using System.Data;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;

namespace Confirmit.CATI.Core.DAL.Handmade.Adapter.Procedure
{
    public class BvCallAdapter
    {
        static public BvCallEntity Read(IDataReader dr)
        {
            if (!dr.Read())
                return null;

            BvCallEntity call = new BvCallEntity();

            call.CallID = dr.GetInt32( dr.GetOrdinal( "CallID" ) );
            call.SurveySID = dr.GetInt32( dr.GetOrdinal( "SurveySID" ) );
            call.InterviewID = dr.GetInt32( dr.GetOrdinal( "iid" ) );
            call.CallState = dr.GetInt32( dr.GetOrdinal( "CallState" ) );
            call.ShiftID = dr.GetInt32( dr.GetOrdinal( "ShiftID" ) );
            call.TimeInShift = dr.GetDateTime( dr.GetOrdinal( "TimeInShift" ) );
            call.TimeToExpire = dr.GetDateTime( dr.GetOrdinal( "TimeToExpire" ) );
            call.Priority = dr.GetInt32( dr.GetOrdinal( "Priority" ) );
            call.Resource = dr.GetInt32( dr.GetOrdinal( "Resource" ) );
            call.ApptID = dr.GetInt32( dr.GetOrdinal( "ApptID" ) );
            call.ResourceType = dr.GetInt32( dr.GetOrdinal( "Resource_Type" ) );
            call.OldPriority = dr.GetInt32(dr.GetOrdinal("OldPriority"));
            call.RuleNumber = dr.GetGuid(dr.GetOrdinal("RuleNumber"));
            call.ConditionValue = dr.GetInt32(dr.GetOrdinal("ConditionValue"));
            call.CellId = dr.GetInt32(dr.GetOrdinal("CellId"));
            call.DialTypeId = dr.GetByte(dr.GetOrdinal("DialTypeId"));
            call.Type = dr.GetByte(dr.GetOrdinal("Type"));
            call.DialerId = dr.GetInt32(dr.GetOrdinal("DialerId"));
            call.ActiveDialId = dr.GetInt64(dr.GetOrdinal("ActiveDialId"));

            return call;
        }

        public static List<BvCallEntity> ReadList(IDataReader dr)
        {
            var callsList = new List< BvCallEntity >();

            while ( true )
            {
                var entity = Read(dr);

                if ( entity == null )
                {
                    break;
                }

                callsList.Add( entity );
            }

            return callsList;
        }
    }
}
