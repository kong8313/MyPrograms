using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework.BulkCopy;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.Logging;
using Confirmit.CATI.Core.Logger;
using System.Collections.Generic;
using Confirmit.CATI.Core.Logger.Kibana;
using YamlDotNet.Serialization;

namespace Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging
{
    //CREATE TABLE [dbo].[CatiInterviewerActivity](
    //    [ID] [int] IDENTITY(1,1) NOT NULL,
    //    [EventTypeId] [int] NOT NULL,
    //    [EventTypeName] [varchar](64) NOT NULL,
    //    [ServerName] [varchar](50) NOT NULL,
    //    [CompanyId] [int] NOT NULL,
    //    [SurveyId] [int] NULL,
    //    [SurveyName] [varchar](255) NULL,
    //    [InterviewerSid] [int] NOT NULL,
    //    [StartTime] [datetime] NOT NULL,
    //    [FinishTime] [datetime] NOT NULL,
    //    [Duration] [int] NOT NULL,
    //    [PhoneNumber] [varchar](255) NULL,
    //    [Details] [xml] NULL,
    //    [InterviewId] [int] NULL,
    // CONSTRAINT [PK_CatiInterviewerActivity_ID] PRIMARY KEY NONCLUSTERED 
    //(
    //    [ID] ASC
    //)
    public class InterviewerActivityEventBase<TDetails> : IInterviewerActivityEventBase
        where TDetails : InterviewerActivityEventDetailsBase, new()
    {
        /// <summary>
        /// Provides the ability to measure the duration of the event activity.
        /// </summary>
        private readonly Stopwatch _stopWatch = new Stopwatch();

        /// <summary>
        /// Gets or sets the type of the management event.
        /// </summary>
        public InterviewerActivityEventType EventTypeId { get; private set; }

        /// <summary>
        /// Gets or sets the event type name.
        /// </summary>
        public string EventTypeName
        {
            get { return GetType().Name; }
        }

        /// <summary>
        /// Gets or sets the machine name of the server the operation is executed on. Useful in multi server configuration.
        /// </summary>
        public string ServerName { get; private set; }

        /// <summary>
        /// Gets or sets the company ID.
        /// </summary>
        public int CompanyId { get; protected set; }

        /// <summary>
        /// Gets or sets survey ID.
        /// </summary>
        public int? SurveySid { get; set; }

        /// <summary>
        /// Gets or sets survey name.
        /// </summary>
        public string SurveyName { get; set; }

        /// <summary>
        /// Gets or sets the interviewer ID.
        /// </summary>
        public int InterviewerSid { get; set; }

        /// <summary>
        /// Gets or sets the start time of the event.
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// Gets or sets the finish time of the event.
        /// </summary>
        public DateTime FinishTime
        {
            get
            {
                return StartTime.AddMilliseconds(_stopWatch.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// Gets or sets the duration of activity event.
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return _stopWatch.Elapsed;
            }
        }

        /// <summary>
        /// Gets or sets telephone number.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets interview id.
        /// </summary>
        public int? InterviewId { get; set; }

        /// <summary>
        /// Gets or sets the additional event parameters.
        /// </summary>
        public TDetails Details { get; set; }

        /// <summary>
        /// Constructs the interviewer event object.
        /// For such event type we always have event type
        /// and interviewer id. So we can set it in the base class.
        /// </summary>
        /// <param name="eventTypeId"></param>
        public InterviewerActivityEventBase(InterviewerActivityEventType eventTypeId)
        {
            StartTime = DateTime.UtcNow;

            _stopWatch.Start();

            CompanyId = ServiceLocator.Resolve<ICompanyInfo>().CompanyId;

            ServerName = ServiceLocator.Resolve<IProcessAndEnvironmentInfo>().MachineName;

            EventTypeId = eventTypeId;

            Details = new TDetails();
        }

        /// <summary>
        /// Saves the event to the events accumulator, so, later events will go to the database using bulk copy.
        /// </summary>
        public void Save()
        {
            //
            // We don't need timings for the short events.
            var settings = ServiceLocator.Resolve<IActivityLoggingSettings>();
            if (_stopWatch.ElapsedMilliseconds < settings.InterviewerActivityEventTimingsThreshold.TotalMilliseconds)
            {
                Details.Timings = null;
            }

            _stopWatch.Stop();

            var eventsAccumulator = ServiceLocator.Resolve<IBulkCopyEntityAccumulator<IInterviewerActivityEventBase>>();
            eventsAccumulator.AddEntity(this);

            SaveToKibana();
            
            var eventName = EventTypeName;
            if (eventName.EndsWith("Event", StringComparison.OrdinalIgnoreCase))
            {
                // remove Event suffix
                eventName = eventName.Substring(0, eventName.Length - 5);
            }
            
            CustomMetrics.OnActivityEvent("BackendInterviewer", eventName, _stopWatch.Elapsed);
        }

        protected void SaveIfEventTookLongerThan(int durationInMilliseconds)
        {
            if (_stopWatch.ElapsedMilliseconds < durationInMilliseconds)
                return;

            Save();
        }

        private void SaveToKibana()
        {
            try
            {
                var fields = LogData.ToCustomFields();

                var logFields = fields.Concat(GetEventFields()).ToArray();

                var customFields = GetEventCustomFields();
                if(customFields != null)
                    logFields = logFields.Concat(customFields).ToArray();

                var interviewerId = " InterviewerId=" + InterviewerSid;
                var projectId = string.IsNullOrEmpty(SurveyName) ? "" : " ProjectId=" + SurveyName;
                var interviewId = InterviewId == null ? "" : " InterviewId=" + InterviewId.Value;
                
                var logWriter = ServiceLocator.Resolve<ILogWriter>();
                logWriter.Write(LogLevel.Info, $"Interviewer activity: {EventTypeName}{interviewerId}{projectId}{interviewId}", logFields, "InterviewerActivityEvent");
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
        }

        private IEnumerable<CustomField> GetEventFields()
        {
            var eventFields = new List<CustomField>
            {
                new CustomField("EventTypeName", EventTypeName),
                new CustomField("ActivityName", EventTypeName),
                new CustomField("ProjectId", SurveyName),
                new CustomField("InterviewerSid", InterviewerSid),
                new CustomField("StartTime", StartTime),
                new CustomField("FinishTime", FinishTime),
                new CustomField("Duration", (int) Duration.TotalMilliseconds),
                new CustomField("PhoneNumber", PhoneNumber),
                new CustomField("ActivityType", "DesktopInterviewer"),
                new CustomField("Details", DetailsToYaml() ?? "-")
            };

            if (SurveySid.HasValue)
            {
                eventFields.Add(new CustomField("SurveySid", SurveySid.Value));
            }

            if (InterviewId.HasValue)
            {
                eventFields.Add(new CustomField("InterviewId", InterviewId.Value));
            }

            if (BackendInstance.IsInitialized)
            {
                eventFields.Add(new CustomField("CompanyName", BackendInstance.Current.CompanyName));
            }

            return eventFields;
        }

        protected virtual IEnumerable<CustomField> GetEventCustomFields() => null;

        public void UpdateEventPropertiesFromTask(BvTasksEntity task)
        {
            if (task == null)
                return;

            InterviewerSid = task.PersonSID;
            InterviewId = task.InterviewID;
            SurveySid = task.SurveySID;

            if (task.SurveySID == 0)
                return;

            var survey = SurveyRepository.GetById(task.SurveySID);
            SurveyName = survey.Name;
        }

        public void UpdateEventPropertiesFromInterview(BvInterviewEntity interview)
        {
            if (interview == null)
                return;

            InterviewId = interview.ID;
            SurveySid = interview.SurveySID;
            PhoneNumber = interview.TelephoneNumber;

            var survey = SurveyRepository.GetById(interview.SurveySID);
            SurveyName = survey.Name;

            if (interview.LastCallPersonSID.HasValue)
            {
                InterviewerSid = interview.LastCallPersonSID.Value;
            }
        }

        public void UpdateEventPropertiesFromCall(BvCallEntity call)
        {
            if (call == null)
                return;

            InterviewId = call.InterviewID;
            SurveySid = call.SurveySID;

            var survey = SurveyRepository.GetById(call.SurveySID);
            SurveyName = survey.Name;
        }

        /// <summary>
        /// Serializes the additional event parameters to XML.
        /// </summary>
        /// <returns>String with additional event parameters as XML.</returns>
        public string DetailsToXml()
        {
            if ((typeof(TDetails) == typeof(NoParameters)) &&
                ((Details.Timings == null) || !Details.Timings.Any()))
                return null;

            var serializer = new XmlSerializer(typeof(TDetails));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, Details, namespaces);
                return stringWriter.ToString();
            }
        }

        public string DetailsToYaml()
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
            var noDetails = typeof(TDetails) == typeof(NoParameters);

            return noDetails && noTimings && noMessages;
        }

        public void AddTiming(string timingName)
        {
            Details.AddTiming(timingName);
        }
    }
}
