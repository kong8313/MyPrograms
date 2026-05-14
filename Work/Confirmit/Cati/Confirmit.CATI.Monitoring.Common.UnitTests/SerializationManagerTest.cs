using Confirmit.CATI.Monitoring.Common.Serialization;
using Confirmit.CATI.Monitoring.Common.StateData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Monitoring.Common.UnitTests
{
    [TestClass]
    public class SerializationManagerTest
    {
        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Serialize_DataIsNull_ReturnsEmptyArray()
        {
            byte[] result =
                SerializationManager.Serialize(
                    MonitoringMessageTypes.AppointmentFormInitialMessage,
                    null
                );

            Assert.AreEqual<int>(0, result.Length);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Serialize_UncompessedSerialization_Success()
        {
            TextControlStateData data = new TextControlStateData
            {
                ControlName = "Name",
                Text = "Text"
            };

            SerializationManager.Serialize(
                MonitoringMessageTypes.AppointmentFormNameChangedMessage, 
                data
            );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Deserialize_UncompessedDeserializationWithCheck_Success()
        {
            TextControlStateData data = new TextControlStateData
            {
                ControlName = "Name",
                Text = "Text"
            };

            byte[] bytes = SerializationManager.Serialize(
                MonitoringMessageTypes.AppointmentFormNameChangedMessage,
                data
            );

            TextControlStateData data2 = (TextControlStateData)SerializationManager.Deserialize(
                MonitoringMessageTypes.AppointmentFormNameChangedMessage,
                bytes
            );

            Assert.AreEqual<string>(data.ControlName, data2.ControlName);
            Assert.AreEqual<string>(data.Text, data2.Text);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Serialize_CompessedSerialization_Success()
        {
            DocumentCompletedStateData data = new DocumentCompletedStateData
            {
                ControlName = "Name",
                PageContent = "<html><body></body></html>"
            };

            SerializationManager.Serialize(
                MonitoringMessageTypes.InterviewInitialMessage,
                data
            );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Deserialize_CompessedDeserializationWithCheck_Success()
        {
            DocumentCompletedStateData data = new DocumentCompletedStateData
            {
                ControlName = "Name",
                PageContent = "<html><body></body></html>"
            };

            byte[] bytes = SerializationManager.Serialize(
                MonitoringMessageTypes.InterviewInitialMessage,
                data
            );

            DocumentCompletedStateData data2 = (DocumentCompletedStateData)SerializationManager.Deserialize(
                MonitoringMessageTypes.InterviewInitialMessage,
                bytes
            );

            Assert.AreEqual<string>(data.ControlName, data2.ControlName);
            Assert.AreEqual<string>(data.PageContent, data2.PageContent);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Deserialize_EmptyStream_ReturnsNull()
        {
            object result = 
                SerializationManager.Deserialize(
                    MonitoringMessageTypes.InterviewFinishMessage,
                    new byte[0]
                );

            Assert.IsNull(result);
        }
    }
}
