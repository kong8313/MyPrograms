using System.Collections.Generic;
using BvCallHandlerLibrary;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.Telephony
{
    [TestClass]
    public class DialerHealthControllerTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private IDialerHealthController _dialerHealthController;
        private IDialerAvailabilityManager _dialerAvailabilityManager;
        private IDialersRepository _dialersRepository;
        private ITelephony _telephony;

        

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();

            Stubs.SetNewIAuthoringServiceStub(true);

            _dialerHealthController = ServiceLocator.Resolve<IDialerHealthController>();
            _dialerAvailabilityManager = ServiceLocator.Resolve<IDialerAvailabilityManager>();
            _dialersRepository = ServiceLocator.Resolve<IDialersRepository>();
            _telephony = ServiceLocator.Resolve<ITelephony>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void CheckDialersHealth_GetStateAndNotificationOk_DialerConnected()
        {
            var context = new TestData
            {
                Dialers = new[] {new DialerData {Tag = "D1", ReplyType = ReplyType.Sync, Id = 1}},
                SystemSettings = new Dictionary<string, object> {{SystemSettingConstants.Dialer.WaitDialerNotificationAtEnableDialerCommandTimeoutInMs, "1000" }}, 
            }.Create();
            
            context.GetDialer("D1").Behavior.Methods.GetState.Init(DialerMethodBehaviors.SendDialerStateAvailable);

            _dialerHealthController.CheckDialersHealth();

            Assert.IsTrue(context.GetDialer("D1").Behavior.Methods.GetState.History.Count > 0);
            Assert.IsTrue(_dialerAvailabilityManager.IsDialerInitializedAndAvaialble(1));
        }
        
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void CheckDialersHealth_GetStateUnavailable_DialerDisconnected()
        {
            var context = new TestData
            {
                Dialers = new[] {new DialerData {Tag = "D1", ReplyType = ReplyType.Sync, Id = 1}},
                SystemSettings = new Dictionary<string, object>{{SystemSettingConstants.Dialer.WaitDialerNotificationAtEnableDialerCommandTimeoutInMs, "1000" }}, 
            }.Create();
            
            context.GetDialer("D1").Behavior.Methods.GetState.Init(DialerState.Unavailable);
            
            _dialerHealthController.CheckDialersHealth();

            Assert.IsTrue(context.GetDialer("D1").Behavior.Methods.GetState.History.Count > 0);
            Assert.IsFalse(_dialerAvailabilityManager.IsDialerInitializedAndAvaialble(1));
        }
        
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void CheckDialersHealth_NoDialerStateNotification_DialerDisconnected()
        {
            var context = new TestData
            {
                Dialers = new[] {new DialerData {Tag = "D1", ReplyType = ReplyType.Sync, Id = 1}},
                SystemSettings = new Dictionary<string, object> {{SystemSettingConstants.Dialer.WaitDialerNotificationAtEnableDialerCommandTimeoutInMs, "1000" }}, 
            }.Create();
            
            context.GetDialer("D1").Behavior.Methods.GetState.Init(DialerState.Available);
            
            _dialerHealthController.CheckDialersHealth();

            Assert.IsTrue(context.GetDialer("D1").Behavior.Methods.GetState.History.Count > 0);
            Assert.IsFalse(_dialerAvailabilityManager.IsDialerInitializedAndAvaialble(1));
        }
        
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void CheckDialersHealth_DialerStateNotificationUnavailable_DialerDisconnected()
        {
            var context = new TestData
            {
                Dialers = new[] {new DialerData {Tag = "D1", ReplyType = ReplyType.Sync, Id = 1}},
                SystemSettings = new Dictionary<string, object> {{SystemSettingConstants.Dialer.WaitDialerNotificationAtEnableDialerCommandTimeoutInMs, "1000" }}, 
            }.Create();
            
            context.GetDialer("D1").Behavior.Methods.GetState.Init(DialerMethodBehaviors.SendDialerStateNotificationUnavailable);

            _dialerHealthController.CheckDialersHealth();

            Assert.IsTrue(context.GetDialer("D1").Behavior.Methods.GetState.History.Count > 0);
            Assert.IsFalse(_dialerAvailabilityManager.IsDialerInitializedAndAvaialble(1));
        }

        [TestMethod, Owner(@"FIRM\EvgeniiL")]
        public void ReconnectingDialerEnabled_DialerConnectedAndActivate_DialerDisconnected_DialerConnectedAndActivate()
        {
            var context = new TestData
            {
                Dialers = new[] { new DialerData { Tag = "D1", ReplyType = ReplyType.Sync, Id = 1, IsConnected = true, IsActive = true, ReconnectionDuration = 1000000, ExpectedState = (int)DialerStatus.ConnectedAndActivated } },
                SystemSettings = new Dictionary<string, object> {
                    { SystemSettingConstants.Dialer.WaitDialerNotificationAtEnableDialerCommandTimeoutInMs, "500" },
                    { SystemSettingConstants.Dialer.HealthControlUnavailableTimeoutInMs, "500"} }
            }.Create();

            context.GetDialer("D1").Behavior.Methods.GetState.Init(DialerMethodBehaviors.SendDialerStateAvailable);
            _dialerHealthController.CheckDialersHealth();

            Assert.IsTrue(context.GetDialer("D1").Behavior.Methods.GetState.History.Count > 0);
            Assert.IsTrue(_dialerAvailabilityManager.IsDialerInitializedAndAvaialble(1));

            context.GetDialer("D1").Behavior.Methods.GetState.Init(DialerState.Unavailable);

            WaitingForDialerDisconnected();

            context.GetDialer("D1").Behavior.Methods.GetState.Init(DialerState.Available);
            context.GetDialer("D1").Behavior.Methods.GetState.Init(DialerMethodBehaviors.SendDialerStateAvailable);

            _dialerHealthController.CheckDialersHealth();
            _telephony.UpdateDialersCollection();

            Assert.IsTrue(_dialerAvailabilityManager.IsDialerInitializedAndAvaialble(1));
        }

        [TestMethod, Owner(@"FIRM\EvgeniiL")]
        public void ReconnectingDialerEnabled_ReconnectionDurationExpired_DialerConnectedAndActivate_DialerDisconnected_DialerDisconnected()
        {
            var context = new TestData
            {
                Dialers = new[] { new DialerData { Tag = "D1", ReplyType = ReplyType.Sync, Id = 1, IsConnected = true, IsActive = true, ReconnectionDuration = 500, ExpectedState = (int)DialerStatus.ConnectedAndActivated } },
                SystemSettings = new Dictionary<string, object> {
                    { SystemSettingConstants.Dialer.WaitDialerNotificationAtEnableDialerCommandTimeoutInMs, "500" },
                    { SystemSettingConstants.Dialer.HealthControlUnavailableTimeoutInMs, "500"} }
            }.Create();

            context.GetDialer("D1").Behavior.Methods.GetState.Init(DialerMethodBehaviors.SendDialerStateAvailable);
            _dialerHealthController.CheckDialersHealth();
            Assert.IsTrue(context.GetDialer("D1").Behavior.Methods.GetState.History.Count > 0);
            Assert.IsTrue(_dialerAvailabilityManager.IsDialerInitializedAndAvaialble(1));

            context.GetDialer("D1").Behavior.Methods.GetState.Init(DialerState.Unavailable);

            WaitingForDialerDisconnected();

            context.GetDialer("D1").Behavior.Methods.GetState.Init(DialerState.Available);
            context.GetDialer("D1").Behavior.Methods.GetState.Init(DialerMethodBehaviors.SendDialerStateAvailable);
            _dialerHealthController.CheckDialersHealth();

            Assert.IsFalse(_dialerAvailabilityManager.IsDialerInitializedAndAvaialble(1));
        }

        [TestMethod, Owner(@"FIRM\EvgeniiL")]
        public void ReconnectingDialerEnabled_ReconnectionDurationExpired_ConnectedAndDeactivated_DialerDisconnected__DialerConnectedAndDeactivated()
        {
            var context = new TestData
            {
                Dialers = new[] { new DialerData { Tag = "D1", ReplyType = ReplyType.Sync, Id = 1, IsConnected = true, IsActive = false, ReconnectionDuration = 10000, ExpectedState = (int)DialerStatus.ConnectedAndDeactivated } },
                SystemSettings = new Dictionary<string, object> {
                    { SystemSettingConstants.Dialer.WaitDialerNotificationAtEnableDialerCommandTimeoutInMs, "500" },
                    { SystemSettingConstants.Dialer.HealthControlUnavailableTimeoutInMs, "500"} }
            }.Create();

            context.GetDialer("D1").Behavior.Methods.GetState.Init(DialerMethodBehaviors.SendDialerStateAvailable);

            _dialerHealthController.CheckDialersHealth();
            var dialer = _dialersRepository.GetById(1);
            Assert.IsTrue(context.GetDialer("D1").Behavior.Methods.GetState.History.Count > 0);
            Assert.IsTrue(_dialerAvailabilityManager.IsDialerNotificationStateOperational(1));
            Assert.IsFalse(dialer.IsActive);

            context.GetDialer("D1").Behavior.Methods.GetState.Init(DialerState.Unavailable);

            WaitingForDialerDisconnected();

            context.GetDialer("D1").Behavior.Methods.GetState.Init(DialerState.Available);
            context.GetDialer("D1").Behavior.Methods.GetState.Init(DialerMethodBehaviors.SendDialerStateAvailable);
            _dialerHealthController.CheckDialersHealth();

            dialer = _dialersRepository.GetById(1);
            Assert.IsTrue(_dialerAvailabilityManager.IsDialerNotificationStateOperational(1));
            Assert.IsFalse(dialer.IsActive);
        }

        [TestMethod, Owner(@"FIRM\EvgeniiL")]
        public void ReconnectingDialerDisabled_DialerConnectedAndActivate_DialerDisconnected_DialerDisconnected()
        {
            var context = new TestData
            {
                Dialers = new[] { new DialerData { Tag = "D1", ReplyType = ReplyType.Sync, Id = 1, IsConnected = true, IsActive = true, ReconnectionDuration = null, ExpectedState = (int)DialerStatus.ConnectedAndActivated } },
                SystemSettings = new Dictionary<string, object> {
                    { SystemSettingConstants.Dialer.WaitDialerNotificationAtEnableDialerCommandTimeoutInMs, "500" },
                    { SystemSettingConstants.Dialer.HealthControlUnavailableTimeoutInMs, "500"} }
            }.Create();

            context.GetDialer("D1").Behavior.Methods.GetState.Init(DialerMethodBehaviors.SendDialerStateAvailable);
            _dialerHealthController.CheckDialersHealth();

            Assert.IsTrue(context.GetDialer("D1").Behavior.Methods.GetState.History.Count > 0);
            Assert.IsTrue(_dialerAvailabilityManager.IsDialerInitializedAndAvaialble(1));

            context.GetDialer("D1").Behavior.Methods.GetState.Init(DialerState.Unavailable);

            WaitingForDialerDisconnected();

            context.GetDialer("D1").Behavior.Methods.GetState.Init(DialerState.Available);
            context.GetDialer("D1").Behavior.Methods.GetState.Init(DialerMethodBehaviors.SendDialerStateAvailable);
            _dialerHealthController.CheckDialersHealth();

            Assert.IsFalse(_dialerAvailabilityManager.IsDialerInitializedAndAvaialble(1));
        }
        
        private void WaitingForDialerDisconnected()
        {
            var waitingTimeOut = 2000;
            var waitingPeriod = 200;
            var waitingCounter = 0;

            while (_dialerAvailabilityManager.IsDialerInitializedAndAvaialble(1) && waitingCounter < waitingTimeOut)
            {
                System.Threading.Thread.Sleep(waitingPeriod);
                waitingCounter += waitingPeriod;
                _dialerHealthController.CheckDialersHealth();
            }

            Assert.IsFalse(_dialerAvailabilityManager.IsDialerInitializedAndAvaialble(1));
        }
    }
}