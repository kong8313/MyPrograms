using System;

using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging
{
    public interface IInterviewerActivityEventBase
    {
        /// <summary>
        /// Gets or sets the type of the management event.
        /// </summary>
        InterviewerActivityEventType EventTypeId { get; }

        /// <summary>
        /// Gets or sets the event type name.
        /// </summary>
        string EventTypeName { get; }

        /// <summary>
        /// Gets or sets the machine name of the server the operation is executed on. Useful in multi server configuration.
        /// </summary>
        string ServerName { get; }

        /// <summary>
        /// Gets or sets the company ID.
        /// </summary>
        int CompanyId { get; }

        /// <summary>
        /// Gets or sets survey ID.
        /// </summary>
        int? SurveySid { get; set; }

        /// <summary>
        /// Gets or sets survey name.
        /// </summary>
        string SurveyName { get; set; }

        /// <summary>
        /// Gets or sets the interviewer ID.
        /// </summary>
        int InterviewerSid { get; set; }

        /// <summary>
        /// Gets or sets the start time of the event.
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// Gets or sets the finish time of the event.
        /// </summary>
        DateTime FinishTime { get; }

        /// <summary>
        /// Gets or sets the duration of activity event.
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// Gets or sets telephone number.
        /// </summary>
        string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets interview id.
        /// </summary>
        int? InterviewId { get; set; }

        /// <summary>
        /// Updates InterviewId, InterviewerId, SurveySid, SurveyName from task.
        /// </summary>
        /// <param name="task"></param>
        void UpdateEventPropertiesFromTask(BvTasksEntity task);

        /// <summary>
        /// Serializes the additional event parameters to XML.
        /// </summary>
        /// <returns>String with additional event parameters as XML.</returns>
        string DetailsToXml();

        /// <summary>
        /// Adds time value passed from the previous AddTiming call
        /// </summary>
        /// <param name="timingName">A description string for the time value</param>
        void AddTiming(string timingName);
    }
}