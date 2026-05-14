using System;
using System.Collections.Generic;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;

namespace Confirmit.CATI.Core.ActivityLogging
{
    public interface IActivityEventsLoader
    {
        /// <summary>
        /// Method returns a collection of interviewer activity event's tipes,
        /// </summary>
        List<Type> GetInterviewerActivityEvents();

        /// <summary>
        /// Method returns a collection of management activity event's tipes,
        /// </summary>
        List<Type> GetManagementActivityEvents();

        /// <summary>
        /// Method returns a GeneralizedEvent for given type,
        /// using InterviewerActivityEventAttribute 
        /// for to get event's id from InterviewerActivityEventType enum
        /// </summary>
        /// <param name="eventType">Event class type</param>
        EventDetails GetInterviewerActivityEventDetails(Type eventType);

        /// <summary>
        /// Method returns a GeneralizedEvent for given type,
        /// using ManagementEventAttribute 
        /// for to get event's id from ManagementEvent enum
        /// </summary>
        /// <param name="eventType">Event class type</param>
        EventDetails GetManagementActivityEventDetails(Type eventType);
    }
}