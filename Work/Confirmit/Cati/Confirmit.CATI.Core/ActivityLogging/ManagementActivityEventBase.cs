using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Logger.Kibana;
using Confirmit.Logging;
using YamlDotNet.Serialization;

namespace Confirmit.CATI.Core.ActivityLogging
{
    public abstract class ManagementActivityEventBase<TDetails> where TDetails : ManagementActivityEventDetails, new()
    {
        public ManagementEvent EventType { get; }
        public ManagementEventCategory Category { get; }
        public DateTime StartTime { get; set; }
        public int CompanyId { get; set; }
        public string ServerName { get; set; }
        public string Supervisor { get; set; }
        public int ObjectId { get; set; }
        public string ObjectName { get; set; }

        /// <summary>
        /// Gets or sets the additional event parameters
        /// </summary>
        public TDetails Details { get; protected set; }

        private string ProjectId
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ObjectName) && ManagementActivityEventHelper.ProjectIdRegex.IsMatch(ObjectName))
                {
                    return ObjectName;
                }

                return null;
            }
        }

        protected ManagementActivityEventBase(ManagementEventCategory category, ManagementEvent eventType)
        {
            Category = category;
            EventType = eventType;
            Details = new TDetails();
        }

        protected void Save(string confirmlogConnectionString, int duration)
        {
            const string sql = @"INSERT INTO [dbo].[CatiManagementActivity]
           ([EventTypeId],[EventTypeName],[ServerName],[CompanyId],[UserId],[StartTime],[FinishTime],[Duration],[ObjectId],[ObjectName],[Details])
       VALUES
           (@EventTypeId,@EventTypeName,@ServerName,@CompanyId,@UserId,@StartTime,@FinishTime,@Duration,@ObjectId,@ObjectName,@Details)";

            var parameters = new[]
            {
                new SqlParameter("EventTypeId", (int) EventType),
                new SqlParameter("EventTypeName", GetType().Name),
                new SqlParameter("ServerName", ServerName),
                new SqlParameter("CompanyId", CompanyId),
                new SqlParameter("UserId", (object)Supervisor ?? DBNull.Value),
                new SqlParameter("StartTime", StartTime),
                new SqlParameter("FinishTime", StartTime.AddMilliseconds(duration)),
                new SqlParameter("Duration", duration),
                new SqlParameter("ObjectId", ObjectId),
                new SqlParameter("ObjectName", (object)ObjectName ?? DBNull.Value),
                new SqlParameter("Details", DetailsToXml() ?? DBNull.Value)
            };

            try
            {
                using (var connection = new SqlConnection(confirmlogConnectionString))
                {
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddRange(parameters);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error during activity log event commit:" + Environment.NewLine + ex);
            }
        }

        protected void SaveToKibana(int duration)
        {
            var fields = LogData.ToCustomFields().ToList();

            string projectId = ProjectId;
            if (projectId != null)
            {
                fields.Add(new CustomField("ProjectId", projectId));

                if (ObjectId > 0)
                {
                    fields.Add(new CustomField("SurveySid", ObjectId));
                }
            }

            var eventFields = new[]
            {
                new CustomField("ActivityType", "Management"),
                new CustomField("ActivityCategory", Category.ToString()),
                new CustomField("ActivityName", GetType().Name),
                new CustomField("StartTime", StartTime),
                new CustomField("FinishTime", StartTime.AddMilliseconds(duration)),
                new CustomField("Duration", duration),
                new CustomField("ObjectId", ObjectId),
                new CustomField("ObjectName", ObjectName),
                new CustomField("Details", DetailsToYaml() ?? "-"),
            };

            fields.AddRange(eventFields);
            
            var id = ObjectId != 0 ? " ID=" + ObjectId : "";
            var name = !string.IsNullOrEmpty(ObjectName) ? " Name=" + ObjectName : "";

            var logWriter = ServiceLocator.Resolve<ILogWriter>();
            logWriter.Write(LogLevel.Info, $"Management activity: {GetType().Name}{id}{name}", fields.ToArray(), "ManagementActivityEvent");
        }

        /// <summary>
        /// Serializes the additional event parameters to XML.
        /// </summary>
        /// <returns>String with additional event parameters as XML.</returns>
        internal object DetailsToXml()
        {
            if (IsEmptyDetails()) return null;

            var serializer = new XmlSerializer(typeof(TDetails));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, Details, namespaces);
                return stringWriter.ToString();
            }
        }

        /// <summary>
        /// Serializes the additional event parameters to YAML.
        /// </summary>
        private string DetailsToYaml()
        {
            if (IsEmptyDetails()) return null;

            var serializer = new SerializerBuilder().EmitDefaults().Build();
            var yaml = serializer.Serialize(Details);

            return yaml;
        }

        private bool IsEmptyDetails()
        {
            var noTimings = (Details.Timings == null) || !Details.Timings.Any();
            var noMessages = (Details.Messages == null) || !Details.Messages.Any();
            var noDetails = typeof(TDetails) == typeof(NoManagementParameters);

            return noDetails && noTimings && noMessages;
        }
    }
}