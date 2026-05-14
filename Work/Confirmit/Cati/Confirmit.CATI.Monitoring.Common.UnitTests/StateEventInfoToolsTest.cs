using System;
using System.Collections.Generic;
using Confirmit.CATI.Monitoring.Common.Contracts;
using Confirmit.CATI.Monitoring.Common.Serialization;
using Confirmit.CATI.Monitoring.Common.StateData;
using Confirmit.CATI.Monitoring.Common.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Monitoring.Common.UnitTests
{
    [TestClass]
    public class StateEventInfoToolsTest
    {
        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAudioEventWithAudioIdentity_NotAudioStartEvent_ExceptionIsThrown()
        {
            var stateEventInfo = new StateEventInfo
                                 {
                                     MessageType = MonitoringMessageTypes.AppointmentFormAppointmentDateChangedMessage,
                                     TimeStamp = DateTime.Now
                                 };

            StateEventInfoTools.UpdateAudioEventsWithAudioIdentities(new List<StateEventInfo> {stateEventInfo}, new List<AudioIdentityObject> { new AudioIdentityObject() });
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void UpdateAudioEventWithAudioIdentity_CorrectParameters_AudioIdentityIsUpdated()
        {
            var stateEventInfo = new StateEventInfo
                                 {
                                     MessageType = MonitoringMessageTypes.AudioStartMessage,
                                     TimeStamp = DateTime.Now,
                                     State = SerializationManager.Serialize(
                                         MonitoringMessageTypes.AudioStartMessage,
                                         new AudioStartStateData
                                         {
                                             AudioRecordID = new AudioIdentityObject
                                                             {
                                                                 ID = @"c:\file1",
                                                                 Name = "file1"
                                                             }
                                         })
                                 };
            var expectedAudioIdentity = new AudioIdentityObject {ID = @"c:\lll", Name = "lll"};

            StateEventInfoTools.UpdateAudioEventsWithAudioIdentities(new List<StateEventInfo> { stateEventInfo }, new List<AudioIdentityObject> { expectedAudioIdentity });

            var newState = SerializationManager.Deserialize(MonitoringMessageTypes.AudioStartMessage,
                stateEventInfo.State) as AudioStartStateData;

            Assert.AreEqual(expectedAudioIdentity.ID, newState.AudioRecordID.ID);
            Assert.AreEqual(expectedAudioIdentity.Name, newState.AudioRecordID.Name);
        }
    }
}
