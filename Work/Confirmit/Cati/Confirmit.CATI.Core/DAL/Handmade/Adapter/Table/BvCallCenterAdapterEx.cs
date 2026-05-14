using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;

namespace Confirmit.CATI.Core.DAL.Handmade.Adapter.Table
{
    public class BvCallCenterAdapterEx
    {
        public static readonly string SelectSql = @"
            SELECT [ID]
                ,[Name]
                ,[Description]
                ,[IsDefault]
                ,[CanBeDeleted]
                ,[LocalTimezoneId]
                ,[HidePii]
                ,COALESCE(String_agg (BvDialerToCallCenter.DialerId, ','), '0') AS DialerIds
            FROM [dbo].[BvCallCenter]
            LEFT JOIN [dbo].[BvDialerToCallCenter] ON ID = CallCenterId
            GROUP BY [ID], [Name], [Description], [IsDefault], [LocalTimezoneId], [HidePii], [CanBeDeleted]
        ";

        public static List<BvCallCenterEntityWithDialerIds> GetByCondition(
            string condition,
            params SqlParameter[] parameters)
        {
            var query = string.IsNullOrEmpty(condition) ? SelectSql : SelectSql + "\r\nwhere\r\n" + condition;

            return ExecuteQuery(query, parameters);
        }

        private static List<BvCallCenterEntityWithDialerIds> ExecuteQuery(
            string query,
            params SqlParameter[] parameters)
        {
            var disposableResources = new Stack<IDisposable>();

            try
            {
                var command = new SqlCommand();

                var connectionScope = new ConnectionScope();
                disposableResources.Push(connectionScope);

                command.Connection = connectionScope.Connection;

                if (DatabaseTransactionScope.Current != null)
                {
                    command.Transaction = DatabaseTransactionScope.Current.Transaction;
                }

                disposableResources.Push(command);

                command.CommandTimeout = Framework.Constants.DefaultDatabaseCommandTimeout;

                command.CommandType = CommandType.Text;
                command.CommandText = query;

                if (parameters.Any())
                {
                    command.Parameters.AddRange(parameters);
                }

                var reader = command.ExecuteReader();

                disposableResources.Push(reader);

                return ReadList(reader);
            }
            finally
            {
                while (disposableResources.Count != 0)
                {
                    var resource2Dispose = disposableResources.Pop();

                    resource2Dispose.Dispose();
                }
            }
        }

        private static List<BvCallCenterEntityWithDialerIds> ReadList(IDataReader rd)
        {
            var bvCallCenterList = new List<BvCallCenterEntityWithDialerIds>();

            int idOrdinal = rd.GetOrdinal("ID");
            int nameOriginal = rd.GetOrdinal("Name");
            int descriptionOrdinal = rd.GetOrdinal("Description");
            int isDefaultOrdinal = rd.GetOrdinal("IsDefault");
            int canBeDeletedOrdinal = rd.GetOrdinal("CanBeDeleted");
            int localTimezoneIdOrdinal = rd.GetOrdinal("LocalTimezoneId");
            int hidePiiOrdinal = rd.GetOrdinal("HidePii");
            int dialerIdsOrdinal = rd.GetOrdinal("DialerIds");

            while (true)
            {
                bool isRead = rd.Read();

                if (isRead == false)
                    break;

                var entity = new BvCallCenterEntityWithDialerIds();

                if (!rd.IsDBNull(idOrdinal))
                    entity.ID =
                        rd.GetInt32(idOrdinal);

                if (!rd.IsDBNull(nameOriginal))
                    entity.Name =
                        rd.GetString(nameOriginal);

                if (!rd.IsDBNull(descriptionOrdinal))
                    entity.Description =
                        rd.GetString(descriptionOrdinal);

                if (!rd.IsDBNull(isDefaultOrdinal))
                    entity.IsDefault =
                        rd.GetBoolean(isDefaultOrdinal);

                if (!rd.IsDBNull(canBeDeletedOrdinal))
                    entity.CanBeDeleted =
                        rd.GetBoolean(canBeDeletedOrdinal);

                if (!rd.IsDBNull(localTimezoneIdOrdinal))
                    entity.LocalTimezoneId =
                        rd.GetInt32(localTimezoneIdOrdinal);

                if (!rd.IsDBNull(hidePiiOrdinal))
                    entity.HidePii =
                        rd.GetBoolean(hidePiiOrdinal);

                if (!rd.IsDBNull(dialerIdsOrdinal))
                    entity.DialerIds =
                        rd.GetString(dialerIdsOrdinal).Split(',').Select(x => int.Parse(x)).ToArray();

                bvCallCenterList.Add(entity);
            }

            return bvCallCenterList;
        }
    }
}
