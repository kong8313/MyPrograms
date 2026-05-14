using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;

namespace Confirmit.CATI.Core.ActivityLogging
{
    public class ActivityEventsLoader : IActivityEventsLoader
    {
        /// <summary>
        /// Method returns a collection of interviewer activity event's tipes,
        /// </summary>
        public List<Type> GetInterviewerActivityEvents()
        {
            return
                Assembly.Load("Confirmit.CATI.Core").GetTypes().Where(
                    type => Attribute.IsDefined(type, typeof (InterviewerActivityEventAttribute)) && type.IsAbstract == false).ToList();
        }

        /// <summary>
        /// Method returns a collection of management activity event's tipes,
        /// </summary>
        public List<Type> GetManagementActivityEvents()
        {
            return
                Assembly.Load("Confirmit.CATI.Core").GetTypes().Where(
                    type => Attribute.IsDefined(type, typeof (ManagementEventAttribute)) && !type.IsAbstract).ToList();
        }

        /// <summary>
        /// Method returns a GeneralizedEvent for given type,
        /// using InterviewerActivityEventAttribute 
        /// for to get event's id from InterviewerActivityEventType enum
        /// </summary>
        /// <param name="eventType">Event class type</param>
        public EventDetails GetInterviewerActivityEventDetails(Type eventType)
        {
            var attribute = eventType.GetCustomAttributes(typeof (InterviewerActivityEventAttribute), false);

            return new EventDetails
                       {
                           EventGroup = "Default group",
                           EventName = eventType.Name,
                           EventId = (int) attribute.Cast<InterviewerActivityEventAttribute>().ElementAt(0).CurrentEvent
                       };
        }

        /// <summary>
        /// Method returns a GeneralizedEvent for given type,
        /// using ManagementEventAttribute 
        /// for to get event's id from ManagementEvent enum
        /// </summary>
        /// <param name="eventType">Event class type</param>
        public EventDetails GetManagementActivityEventDetails(Type eventType)
        {
            var attributes = eventType.GetCustomAttributes(typeof (ManagementEventAttribute), false);

            return new EventDetails
                       {
                           EventGroup = "Default group",
                           EventName = eventType.Name,
                           EventId = (int) attributes.Cast<ManagementEventAttribute>().ElementAt(0).CurrentEvent
                       };
        }
    }
}