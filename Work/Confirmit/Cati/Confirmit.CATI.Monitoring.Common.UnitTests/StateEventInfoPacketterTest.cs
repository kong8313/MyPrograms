using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Confirmit.CATI.Monitoring.Common.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Monitoring.Common.UnitTests
{
    /// <summary>
    /// Summary description for StateEventInfoPacketterTest
    /// </summary>
    [TestClass]
    public class StateEventInfoPacketterTest
    {
        public TestContext TestContext { get; set; }

        #region Constructor
        
        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void Constructor_New_CreatesDefaultStream()
        {
            var res = new StateEventInfoPacketter();
            Assert.IsNotNull(res.PacketStream);
        }
        
        #endregion
        
        #region ClearStream
        
        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void ClearStream_Execute_CreatesNewStream()
        {
            var res = new StateEventInfoPacketter();
            Stream str = res.PacketStream;
            res.ClearStream();
            Assert.AreNotEqual(str, res.PacketStream);
        }
        
        #endregion

        #region GetPacketSize

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetPacketSize_Execute_ReturnsStreamSize()
        {
            var res = new StateEventInfoPacketter();
            var buf = new byte[123];            
            res.PacketStream.Write(buf, 0, buf.Length);
            Assert.AreEqual(buf.Length, res.GetPacketSize());
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetPacketSize_ClearStream_ReturnsZero()
        {
            var res = new StateEventInfoPacketter();
            var buf = new byte[123];
            res.PacketStream.Write(buf, 0, buf.Length);
            res.ClearStream();
            Assert.AreEqual(0, res.GetPacketSize());
        }

        #endregion

        #region GetPacketBytes

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetPacketBytes_Execute_ReturnsStreamContent()
        {
            var res = new StateEventInfoPacketter();
            var buf = new byte[123];
            buf[1] = 89;
            buf[77] = 77;
            buf[99] = 4;
            res.PacketStream.Write(buf, 0, buf.Length);
            Assert.IsTrue(buf.SequenceEqual(res.GetPacketBytes()));
        }
        
        #endregion

        #region AddToPacket

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void AddToPacket_2objects_ResultingStreamsIsConcatenationOf2Streams()
        {
            var res1 = new StateEventInfoPacketter();
            var obj1 = new StateEventInfo {TimeStamp = DateTime.UtcNow};
            res1.AddToPacket(obj1);
            var buf1 = res1.GetPacketBytes();
            var res2 = new StateEventInfoPacketter();
            var obj2 = new StateEventInfo {TimeStamp = DateTime.UtcNow.AddMinutes(10)};
            res2.AddToPacket(obj2);
            var buf2 = res2.GetPacketBytes();
            var buf = buf1.Concat(buf2);
            var res3 = new StateEventInfoPacketter();
            res3.AddToPacket(obj1);
            res3.AddToPacket(obj2);
            Assert.IsTrue(buf.SequenceEqual(res3.GetPacketBytes()));
        }

        private class Tester : IEqualityComparer<StateEventInfo>
        {
            #region IEqualityComparer<StateEventInfo> Members

            public bool Equals(StateEventInfo x, StateEventInfo y)
            {
                return x.TimeStamp == y.TimeStamp;
            }

            public int GetHashCode(StateEventInfo obj)
            {
                return obj.GetHashCode();
            }

            #endregion
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void AddToPacket_2ObjectsInCollection_DepackerReturnsTheSameEvents()
        {
            var res1 = new StateEventInfoPacketter();
            var obj1 = new StateEventInfo { TimeStamp = DateTime.UtcNow };
            res1.AddToPacket(obj1);
            var obj2 = new StateEventInfo { TimeStamp = DateTime.UtcNow.AddMinutes(10) };
            res1.AddToPacket(obj2);
            var buf1 = res1.GetPacketBytes();

            var res3 = new StateEventInfoPacketter();
            res3.AddToPacket(new List<StateEventInfo> {obj1, obj2});
            var buf3 = res3.GetPacketBytes();

            Assert.IsTrue(StateEventInfoDepacker.GetAllEvents(buf1).SequenceEqual(
                StateEventInfoDepacker.GetAllEvents(buf3), new Tester()));            
        }


        #endregion

        #region CreatePacketFromEvent

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void CreatePacketFromEvent_SameParameters_ReturnTheSameBytes()
        {
            var res1 = new StateEventInfoPacketter();
            var obj1 = new StateEventInfo { TimeStamp = DateTime.UtcNow };
            res1.AddToPacket(obj1);
            var buf1 = res1.GetPacketBytes();

            var buf2 = StateEventInfoPacketter.CreatePacketFromEvent(obj1);
            
            Assert.IsTrue(buf1.SequenceEqual(buf2));
        }

        #endregion
    }
}
