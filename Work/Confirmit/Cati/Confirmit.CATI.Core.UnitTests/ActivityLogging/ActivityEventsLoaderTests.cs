using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.ActivityLogging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.ActivityLogging
{
    [TestClass]
    public class ActivityEventsLoaderTests
    {
        private static ActivityEventsLoader _activityEventsLoader;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _activityEventsLoader = new ActivityEventsLoader();
        }

        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void GetInterviewerActivityEvents_EachEventHasAttribute_ReturnsEventsCollection()
        {
            var eventsCollection = _activityEventsLoader.GetInterviewerActivityEvents();
            var currentsEventCollection = GetInterviewerActivityEvents();

            var eventsWithoutAttributes = currentsEventCollection.Except(eventsCollection).ToList();

            Assert.IsTrue(eventsWithoutAttributes.Count == 0, "Events without InterviewerActivityEventAttribute atrribute: " + string.Join(", ", eventsWithoutAttributes));
        }

        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void GetInterviewerActivityEvents_OnlyEventsHaveAppropriateAttributes_ReturnsEventsCollection()
        {
            var eventsCollection = _activityEventsLoader.GetInterviewerActivityEvents();
            var currentsEventCollection = GetInterviewerActivityEvents();

            var extraAttributes = eventsCollection.Except(currentsEventCollection).ToList();

            Assert.IsTrue(extraAttributes.Count == 0, "These classes don't need InterviewerActivityEventAttribute atrribute: " + string.Join(", ", extraAttributes));
        }

        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void GetManagementActivityEvents_EachEventHasAttribute_ReturnsEventsCollection()
        {
            var eventsCollection = _activityEventsLoader.GetManagementActivityEvents();
            var currentsEventCollection = GetManagementActivityEvents();

            var eventsWithoutAttributes = currentsEventCollection.Except(eventsCollection).ToList();

            Assert.IsTrue(eventsWithoutAttributes.Count == 0, "Events without ManagementEventAttribute atrribute: " + string.Join(", ", eventsWithoutAttributes));
        }

        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void GetManagementActivityEvents_OnlyEventsHaveAppropriateAttributes_ReturnsEventsCollection()
        {
            var eventsCollection = _activityEventsLoader.GetManagementActivityEvents();
            var currentsEventCollection = GetManagementActivityEvents();

            var extraAttributes = eventsCollection.Except(currentsEventCollection).ToList();

            Assert.IsTrue(extraAttributes.Count == 0, "These classes don't need ManagementEventAttribute atrribute: " + string.Join(", ", extraAttributes));
        }

        private static List<Type> GetInterviewerActivityEvents()
        {
            return Assembly.Load("Confirmit.CATI.Core").GetTypes().Where(type =>
                                                                         type.BaseType != null &&
                                                                         InheritsFrom(type, "InterviewerActivityEventBase") &&
                                                                         !type.BaseType.ToString().Contains("BulkCopyEntitySerializerBase") &&
                                                                         type.IsAbstract == false
                                                                         ).ToList();
        }

        private static List<Type> GetManagementActivityEvents()
        {
            var types = Assembly.Load("Confirmit.CATI.Core").GetTypes();
            return types.Where(type =>
                                                                         !type.IsGenericType &&
                                                                         type.BaseType != null &&
                                                                         !type.IsAbstract &&
                                                                         (type.BaseType.Name.Contains("ManagementActivityEvent") &&
                                                                         !type.BaseType.Name.Contains("ManagementActivityEventDetails") ||
                                                                         (type.BaseType.BaseType != null &&
                                                                         type.BaseType.BaseType.Name.Contains("ManagementActivityEvent") &&
                                                                         !type.BaseType.BaseType.Name.Contains("ManagementActivityEventDetails")))
                                                                        ).ToList();
        }

        private static bool InheritsFrom(Type type, string baseTypeName)
        {
            var currentType = type;

            while (currentType != null)
            {
                if (currentType.BaseType!=null && currentType.BaseType.ToString().Contains(baseTypeName))
                {
                    return true;
                }

                currentType = currentType.BaseType;
            }

            return false;
        }
    }
}
