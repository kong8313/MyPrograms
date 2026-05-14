using System;

namespace Confirmit.CATI.Core.ActivityLogging
{
    public interface IManagementActivityEvent : IActivityEvent
    {
        /// <summary>
        /// Gets or sets the type of the management event.
        /// </summary>
        ManagementEvent EventType { get; }

        /// <summary>
        /// Gets or sets the start time of the event.
        /// </summary>
        DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the company ID.
        /// </summary>
        int CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the machine name of the server the operation is executed on. Useful in multi server configuration.
        /// </summary>
        string ServerName { get; set; }

        /// <summary>
        /// Gets or sets the duration of activity event.
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// Gets or sets the login name of a supervisor that is performing the operation.
        /// </summary>
        string Supervisor { get; set; }

        /// <summary>
        /// Gets or sets the object ID. Object could be different for different activities.
        /// </summary>
        int ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the name of the object. Object could be different for different activities.
        /// </summary>
        string ObjectName { get; set; }
    }
}