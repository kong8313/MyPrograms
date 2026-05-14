using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Telephony.DialerCommon.EventNotifications.Fakes;
using Confirmit.CATI.Telephony.DialerCommon.Fakes;
using Confirmit.CATI.Telephony.DialerService;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DialerServiceNameSpace = Confirmit.CATI.Telephony.DialerService;

namespace DialerService.UnitTests
{
    [TestClass]
    public class AgentStatesFilterTest
    {
        private const string DialerDriverAssemblyNamespace = "Confirmit.CATI.Telephony.SimulatorDialerDriver";
        private const string DialerDriverAssemblyMainClassName = "SimulatorDialerDriver";
        private const string DialerDriverAssemblyName = "SimulatorDialerDriverDemo";

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void AllowedAgentStateListIsEmptyInConfig_MandatoryAgentStatesAreIncludedToTheListAutomatically()
        {
            var allowedAgentStates = new StringCollection();
            var agentStates = DialerServiceNameSpace.DialerService.MandatoryAgentStates;

            TryStates_NotificationsArePassed(allowedAgentStates, agentStates);
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void AllowedAgentStateListContainsOneIncorrectValue_MandatoryAgentStatesAreIncludedToTheListAutomatically()
        {
            var allowedAgentStates = new StringCollection
                {
                    "15" // Out of AgentState enumeration range value
                };
            var agentStates = DialerServiceNameSpace.DialerService.MandatoryAgentStates;

            TryStates_NotificationsArePassed(allowedAgentStates, agentStates);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void NotAllowedAgentStatesDoNotGoThrough()
        {
            var agentStates = new[]
            {
                AgentState.Ready,
                AgentState.OnHook,
                AgentState.OffHook,
                (AgentState)15 // Out of AgentState enumeration range value
            };

            TryStates_NotificationsAreNotPassed(agentStates);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void AllowedAgentStatesGoThrough()
        {
            var allowedAgentStates = new StringCollection
                {
                    "LoggedIn", 
                    "LoggedOut", 
                    "Ready", 
                    "NotReady", 
                    "OnHook", 
                    "OffHook", 
                    "15" // Out of AgentState enumeration range value
                };
            var agentStates = allowedAgentStates.Cast<string>().Select(x => (AgentState)Enum.Parse(typeof(AgentState), x));

            TryStates_NotificationsArePassed(allowedAgentStates, agentStates);
        }

        private void TryStates_NotificationsAreNotPassed(
            [NotNull] IEnumerable<AgentState> agentStates)
        {
            var isSendEventNotificationCalled = false;

            var stubIDialerEventNotificationsSender = new StubIDialerEventNotificationsSender
            {
                SendEventNotificationIDialerEvent = dialerEvent =>
                {
                    isSendEventNotificationCalled = true;
                }
            };

            var stubINotificationsSenderInitializer = new StubINotificationsSenderInitializer
            {
                InitializeIdentityInt32Int32 = (id, companyId) => stubIDialerEventNotificationsSender
            };

            Settings.Default["AllowedAgentStates"] = new StringCollection();

            var dialerService = new DialerServiceNameSpace.DialerService(
                DialerDriverAssemblyNamespace,
                DialerDriverAssemblyMainClassName,
                DialerDriverAssemblyName,
                stubINotificationsSenderInitializer);

            foreach (var agentState in agentStates)
            {
                dialerService.NotifyAgentState(1, 1, 1, 1, agentState);
            }

            Assert.IsFalse(isSendEventNotificationCalled, "SendEventNotification should not be called");
        }

        private void TryStates_NotificationsArePassed(
            IEnumerable statesCollection,
            [NotNull] IEnumerable<AgentState> agentStates)
        {
            Assert.IsNotNull(agentStates);

            var callsCount = 0;

            var stubIDialerEventNotificationsSender = new StubIDialerEventNotificationsSender
            {
                // Increment the counter each time a notification is sent
                SendEventNotificationIDialerEvent = x => callsCount++
            };

            var stubINotificationsSenderInitializer = new StubINotificationsSenderInitializer
            {
                InitializeIdentityInt32Int32 = (id, companyId) => stubIDialerEventNotificationsSender
            };

            Settings.Default["AllowedAgentStates"] = statesCollection;

            var dialerService = new DialerServiceNameSpace.DialerService(
                DialerDriverAssemblyNamespace,
                DialerDriverAssemblyMainClassName,
                DialerDriverAssemblyName,
                stubINotificationsSenderInitializer);

            // For each of the states call notification method and count number of calls
            var actualCount = agentStates.Count(x =>
                {
                    dialerService.NotifyAgentState(1, 1, 1, 1, x);
                    return true; // We want to count all elements
                });

            Assert.AreEqual(actualCount, callsCount);
        }
    }
}