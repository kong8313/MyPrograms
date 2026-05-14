using System;
using System.Data.SqlClient;
using System.Diagnostics;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Backend.WebApiServices.Logging
{  
    public  class RestApiMonitorLogger : IRestApiMonitorLogger
    {
        private readonly IConnectionStrings _connectionStrings;

        private const string InsertCommand = @"insert into MetadataApiMonitor 
                                             (ResourceCollectionName, ResourceIdentifier, Uri, TimeTakenMs, Application, UnitOfWork, Method, StatusCode, ContentType, UserId, CompanyId, AuthenticationType, Exception, WebServer) 
                                             values(@ResourceCollectionName, @ResourceIdentifier, @Uri, @TimeTakenMs, @Application, @UnitOfWork, @Method, @StatusCode, @ContentType, @UserId, @CompanyId, @AuthenticationType, @Exception, @WebServer)";

        public RestApiMonitorLogger(IConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public  void Log(RestApiMonitorInfo info)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionStrings.ConfirmlogConnectionString))
                {
                    using (var command = new SqlCommand(InsertCommand, connection))
                    {
                        command.Parameters.AddWithValue("@ResourceCollectionName", info.ResourceCollectionName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ResourceIdentifier",  info.ResourceIdentifier ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Uri", GetDbValue(info.Uri.PathAndQuery, 2000));
                        command.Parameters.AddWithValue("@TimeTakenMs", info.TimeTakenInMs);
                        command.Parameters.AddWithValue("@Application", GetDbValue(info.Application, 250));
                        command.Parameters.AddWithValue("@UnitOfWork", GetDbValue(info.UnitOfWork, 350));
                        command.Parameters.AddWithValue("@Method", info.Method.Method);
                        command.Parameters.AddWithValue("@StatusCode", (int)info.StatusCode);
                        command.Parameters.AddWithValue("@ContentType", GetDbValue(info.ContentType, 50));
                        command.Parameters.AddWithValue("@UserId", info.UserId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CompanyId", info.CompanyId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@AuthenticationType", info.AuthenticationHeaderType ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Exception", info.Exception != null ? info.Exception.ToString() : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@WebServer", GetDbValue(info.WebServerName));
                        connection.Open();

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error during saving information for Rest Api Monitor:" + Environment.NewLine +  ex);
            }
        }

        private static object GetDbValue(string value, int maxLength = 0)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return DBNull.Value;
            }

            return (maxLength > 0) ? value.Substring(0, Math.Min(maxLength, value.Length)) : 
                                     value;
        }        
    }
}
