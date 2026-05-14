using System;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Logger
{
    class DatabaseTraceListener : CatiTraceListener
    {
        private string _confirmlogConnectionString;
        
        public override void TraceEvent(TraceEventCache eventCache, string source,
                                        TraceEventType severity, int id, string message)
        {
            message = PrepareMessageText(message);
            SaveEventToDatabase(severity, message, source, id);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source,
                                        TraceEventType severity, int id,
                                        string format, params object[] args)
        {
            var sb = BuildMessage(format, args);
            string message = PrepareMessageText(sb);
            SaveEventToDatabase(severity, message, source, id);
        }

        public override void TraceData(TraceEventCache eventCache, String source,
                                       TraceEventType severity, int id, object data)
        {
            string message = PrepareMessageText(data);
            SaveEventToDatabase(severity, message, source, id);
        }

        public override void TraceData(TraceEventCache eventCache, String source,
                                       TraceEventType severity, int id, params object[] data)
        {
            var sb = BuildMessage(data);
            string message = PrepareMessageText(sb);
            SaveEventToDatabase(severity, message, source, id);
        }

        /// <summary>
        /// Writes a message to this instance's event log.
        /// </summary>
        /// <param name="message">Message that should be logged</param>
        public override void Write(string message)
        {
            message = PrepareMessageText(message);
            SaveEventToDatabase(TraceEventType.Information, message, String.Empty, 0);
        }

        /// <summary>
        /// Writes a message to this instance's event log followed by a line terminator.
        /// The default line terminator is a carriage return followed by a line feed (\r\n).
        /// </summary>
        /// <param name="message">Message that should be logged</param>
        public override void WriteLine(string message)
        {
            Write(message);
        }

        private string PrepareMessageText(object text)
        {
            return text + Environment.NewLine + LogData.ToMessageFooter();
        }
        
        /// <summary>
        /// Saves the event to database.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="text">The event text.</param>
        /// <param name="source">The event source. For client errors we suppose that it is defined in <see cref="ClientErrorSource"/>.</param>
        /// <param name="id">The event ID. For client errors we suppose that it is company ID.</param>
        private void SaveEventToDatabase(TraceEventType eventType, string text, string source, int id)
        {
            try
            {
                if (!ShouldTrace(text))
                {
                    return;
                }

                if (eventType == TraceEventType.Error && ShouldTraceErrorAsWarning(text))
                {
                    eventType = TraceEventType.Warning;
                }

                string sqlQuery =
                    "INSERT INTO [dbo].[CatiEventLog]\r\n" +
                    "   ( [EventTypeId], [EventTypeName], [ServerName], [CompanyId], [EventTime], [Text] )\r\n" +
                    "VALUES\r\n" +
                    "   ( @EventTypeId, @EventTypeName, @ServerName, @CompanyId, GETUTCDATE(), @Text )";

                string eventTypeName = GetEventTypeName(source, eventType.ToString());
                int companyId = ServiceLocator.Resolve<ICompanyInfo>().GetCompanyId(id, source);

                var parameters = new[]
                {
                    new SqlParameter("EventTypeId", (int)eventType),
                    new SqlParameter("EventTypeName", eventTypeName),
                    new SqlParameter("ServerName", ServerName),
                    new SqlParameter("CompanyId", companyId),
                    new SqlParameter("Text", text)
                };

                if (_confirmlogConnectionString == null)
                {
                    _confirmlogConnectionString =
                        ServiceLocator.Resolve<IConnectionStrings>().ConfirmlogConnectionString;
                }

                using (var connection = new SqlConnection(_confirmlogConnectionString))
                {
                    using (var command = new SqlCommand(sqlQuery, connection))
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
                FallbackLog(
                    $"Error writing to database log:\r\n{ex}\r\n\r\n" +
                    $"Original log message:\r\n[{eventType},{id}]: {text}\r\n");
            }
        }
    }
}
