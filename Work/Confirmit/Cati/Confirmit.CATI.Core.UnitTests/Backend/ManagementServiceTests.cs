using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InternalService = Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Core.Misc.Fakes;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;

namespace Confirmit.CATI.Core.UnitTests.Backend
{
    [TestClass]
    public class ManagementServiceTests
    {
        private IEnumerable<EventDetails> _collectionOfInterviewerActivityEvents;
        private IEnumerable<EventDetails> _collectionOfManagmentActivityEvents;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// Is filled automatically.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();

            ServiceLocator.RegisterSingleton<IDbLibProvider>(new StubIDbLibProvider()
            {
                CatiDefaultConnectionStringGet = () => "Data Source=UnitTestSQL;Initial Catalog=UnitTestDb;"
            });
            ServiceLocator.RegisterInstance<IServiceDiscoveryClientProxy>(new StubIServiceDiscoveryClientProxy());

            var backendInstance = new BackendInstance();
            BackendInstance.Current = backendInstance;

            _collectionOfInterviewerActivityEvents = new InternalService.ManagementService().GetInterviewerActivityEventsList();
            _collectionOfManagmentActivityEvents = new InternalService.ManagementService().GetManagmentActivityEventsList();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }

        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void GetInterviewerActivityEventsList_EventsMatchEnumValues_ReturnsCollectionOfEvents()
        {
            CheckEventsMatchEnumValues(typeof(InterviewerActivityEventType), _collectionOfInterviewerActivityEvents);
        }

        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void GetInterviewerActivityEventsList_EachEventHasOnlyOneAttribute_ReturnsCollectionOfEvents()
        {
            CheckEachEnumValueMatchsOnlyOneEvent(typeof(InterviewerActivityEventType), _collectionOfInterviewerActivityEvents);
        }

        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void GetManagementActivityEventsList_EventsMatchEnumValues_ReturnsCollectionOfEvents()
        {
            var excludeEvents = new[] { ManagementEvent.SyncQueueAdd, ManagementEvent.SyncQueueResync, ManagementEvent.SyncQueueDelete };// these events are specified in Confrirmit code
            var excludeEventsDictionary = excludeEvents.ToDictionary(managementEvent => (int) managementEvent, managementEvent => managementEvent.ToString());

            CheckEventsMatchEnumValues(typeof(ManagementEvent), _collectionOfManagmentActivityEvents, excludeEventsDictionary);
        }

        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void GetManagmentActivityEventsList_EachEventHasOnlyOneAttribute_ReturnsCollectionOfEvents()
        {
            CheckEachEnumValueMatchsOnlyOneEvent(typeof(ManagementEvent), _collectionOfManagmentActivityEvents);
        }

        public void CheckEachEnumValueMatchsOnlyOneEvent(Type enumType, IEnumerable<EventDetails> collectionOfEvents)
        {
            var eventIds = collectionOfEvents.GroupBy(x => x.EventId).Where(x => x.Count() > 1).ToArray();
            var eventsWithTheSameId = (from eventDetailse in eventIds from detailse in eventDetailse select detailse.EventName).ToList();

            Assert.IsTrue(eventIds.Count() == 0, "Thare are the events with the same event id. Repeated event ids: " + string.Join(", ", eventsWithTheSameId));
        }

        private static void CheckEventsMatchEnumValues(Type enumType, IEnumerable<EventDetails> collectionOfEvents, Dictionary<int, string> excludeEvents = null)
        {
            var enumValues = GetEnumValues(enumType);

            if (excludeEvents != null)
                enumValues = enumValues.Except(excludeEvents).ToDictionary(x => x.Key, y => y.Value);

            var eventIds = collectionOfEvents.Select(x => x.EventId).ToArray();
            var difference = GetExtraEvents(enumValues, eventIds);

            Assert.IsTrue(difference.Count == 0, "There are no events for the next event types: " + string.Join(", ", difference));
        }

        private static Dictionary<int, string> GetExtraEvents(Dictionary<int, string> enumValues, int[] eventIds)
        {
            return enumValues.Keys.Except(eventIds).ToDictionary(x => x, x => enumValues[x]);
        }

        private static Dictionary<int, string> GetEnumValues(Type enumType)
        {
            var duplicates = Enum.GetNames(enumType).GroupBy(x => (int) Enum.Parse(enumType, x))
                .Where(x => x.Count() > 1).SelectMany( x =>  x.Select( v => $"{v}={x.Key}")).ToArray();

            Assert.IsTrue(duplicates.Length == 0, $"'{enumType}' enum contains following elements with same value:{duplicates.JoinInString("\n\r")}");

            return Enum.GetNames(enumType).ToDictionary(x => (int)Enum.Parse(enumType, x));
        }
    }
}