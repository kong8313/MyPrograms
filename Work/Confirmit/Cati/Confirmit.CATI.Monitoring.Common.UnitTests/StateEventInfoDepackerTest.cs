using System;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Monitoring.Common.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Monitoring.Common.UnitTests
{
    /// <summary>
    /// Summary description for StateEventInfoDepackerTest
    /// </summary>
    [TestClass]
    public class StateEventInfoDepackerTest
    {
        public TestContext TestContext { get; set; }

        #region Constructor
        
        [TestMethod, Owner(@"FIRM\SergeyL")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullStream_ThrowsException()
        {
            new StateEventInfoDepacker((Stream)null);            
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]        
        public void Constructor_ValidStramParameter_CreatesObject()
        {
            var res = new StateEventInfoDepacker(new MemoryStream());
            Assert.IsNotNull(res);
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullBuffer_ThrowsException()
        {
            new StateEventInfoDepacker((byte[])null);
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void Constructor_ValidBufferParameter_CreatesObject()
        {
            var res = new StateEventInfoDepacker(new byte[10]);
            Assert.IsNotNull(res);
        }
        
        #endregion

        #region GetAllEvents

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetAllEvents_OneEvent_ReturnsOneEvent()
        {
            var res1 = new StateEventInfoPacketter();
            var obj1 = new StateEventInfo { TimeStamp = DateTime.UtcNow };
            res1.AddToPacket(obj1);

            var dep = new StateEventInfoDepacker(res1.GetPacketBytes());
            var res = dep.GetAllEvents();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Count()==1);
            Assert.IsTrue(obj1.TimeStamp == res.First().TimeStamp);
        }
        
        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetAllEvents_2Events_ReturnsEvents()
        {
            var res1 = new StateEventInfoPacketter();
            var obj1 = new StateEventInfo { TimeStamp = DateTime.UtcNow };
            res1.AddToPacket(obj1);
            res1.AddToPacket(obj1);

            var dep = new StateEventInfoDepacker(res1.GetPacketBytes());
            var res = dep.GetAllEvents();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Count() == 2);
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetAllEvents_2EventsInArray1Event_ReturnsEvents()
        {
            var res1 = new StateEventInfoPacketter();
            res1.AddToPacket(new[]
                              {
                                  new StateEventInfo { TimeStamp = DateTime.UtcNow },
                                  new StateEventInfo { TimeStamp = DateTime.UtcNow.AddMinutes(1) }
                              });
            res1.AddToPacket(new StateEventInfo { TimeStamp = DateTime.UtcNow.AddMinutes(10) });
            res1.PacketStream.Position = 0;
            var dep = new StateEventInfoDepacker(res1.PacketStream);
            var res = dep.GetAllEvents();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Count() == 3);
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetAllEvents_1Event2EventsInList_ReturnsEvents()
        {
            var res1 = new StateEventInfoPacketter();
            res1.AddToPacket(new StateEventInfo { TimeStamp = DateTime.UtcNow.AddMinutes(10) });
            res1.AddToPacket(new List<StateEventInfo>
                              {
                                  new StateEventInfo { TimeStamp = DateTime.UtcNow },
                                  new StateEventInfo { TimeStamp = DateTime.UtcNow.AddMinutes(1) }
                              });

            res1.PacketStream.Position = 0;
            var dep = new StateEventInfoDepacker(res1.PacketStream);
            var res = dep.GetAllEvents();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Count() == 3);
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        [ExpectedException(typeof(InvalidDataException))]
        public void GetAllEvents_InvalidObject_ThrowsException()
        {
            //Note: low level implementation details.
            var formatter = new BinaryFormatter { AssemblyFormat = FormatterAssemblyStyle.Simple };
            var packetStream = new MemoryStream(16 * 1024);
            //Any serializable class is good here (except supported classes).
            using (var deflateStream = new Ionic.Zlib.DeflateStream(packetStream, Ionic.Zlib.CompressionMode.Compress, true))
            {
                formatter.Serialize(deflateStream, new AudioIdentityObject());
            }            
            packetStream.Position = 0;
            var dep = new StateEventInfoDepacker(packetStream);
            dep.GetAllEvents();
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetAllEvents_3Events_ReturnsSortedByDateEvents()
        {
            DateTime dt1 = DateTime.UtcNow;
            DateTime dt2 = DateTime.UtcNow.AddMinutes(-1);
            DateTime dt3 = DateTime.UtcNow.AddMinutes(10);

            var res1 = new StateEventInfoPacketter();
            var obj1 = new StateEventInfo { TimeStamp = dt1 };
            res1.AddToPacket(obj1);
            var obj2 = new StateEventInfo { TimeStamp = dt2 };
            res1.AddToPacket(obj2);
            var obj3 = new StateEventInfo { TimeStamp = dt3 };
            res1.AddToPacket(obj3);

            var dep = new StateEventInfoDepacker(res1.GetPacketBytes());
            var res = dep.GetAllEvents();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Count() == 3);
            Assert.IsTrue(res.Select(e=>e.TimeStamp).SequenceEqual(new[]{dt2, dt1, dt3}));
            
        }

        #endregion

        #region  Static GetAllEvents
        
        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetAllEvents_SameBytes_ReturnsSameEvents()
        {
            DateTime dt1 = DateTime.UtcNow;
            DateTime dt2 = DateTime.UtcNow.AddMinutes(-1);
            DateTime dt3 = DateTime.UtcNow.AddMinutes(10);

            var res1 = new StateEventInfoPacketter();
            var obj1 = new StateEventInfo { TimeStamp = dt1 };
            res1.AddToPacket(obj1);
            var obj2 = new StateEventInfo { TimeStamp = dt2 };
            res1.AddToPacket(obj2);
            var obj3 = new StateEventInfo { TimeStamp = dt3 };
            res1.AddToPacket(obj3);

            var dep = new StateEventInfoDepacker(res1.GetPacketBytes());
            var res = dep.GetAllEvents();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Count() == 3);
            Assert.IsTrue(res.Select(e => e.TimeStamp).SequenceEqual(new[] { dt2, dt1, dt3 }));

            var ses = StateEventInfoDepacker.GetAllEvents(res1.GetPacketBytes());
            Assert.IsNotNull(ses);
            Assert.IsTrue(ses.Count() == 3);
            Assert.IsTrue(ses.Select(e => e.TimeStamp).SequenceEqual(new[] { dt2, dt1, dt3 }));

            Assert.IsTrue(ses.Select(e => e.TimeStamp).SequenceEqual(res.Select(e => e.TimeStamp)));
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetAllEvents_SameStreams_ReturnsSameEvents()
        {
            DateTime dt1 = DateTime.UtcNow;
            DateTime dt2 = DateTime.UtcNow.AddMinutes(-1);
            DateTime dt3 = DateTime.UtcNow.AddMinutes(10);

            var res1 = new StateEventInfoPacketter();
            var obj1 = new StateEventInfo { TimeStamp = dt1 };
            res1.AddToPacket(obj1);
            var obj2 = new StateEventInfo { TimeStamp = dt2 };
            res1.AddToPacket(obj2);
            var obj3 = new StateEventInfo { TimeStamp = dt3 };
            res1.AddToPacket(obj3);
            
            res1.PacketStream.Position = 0;
            var dep = new StateEventInfoDepacker(res1.PacketStream);
            var res = dep.GetAllEvents();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Count() == 3);
            Assert.IsTrue(res.Select(e => e.TimeStamp).SequenceEqual(new[] { dt2, dt1, dt3 }));
            
            res1.PacketStream.Position = 0;
            var ses = StateEventInfoDepacker.GetAllEvents(res1.PacketStream);
            Assert.IsNotNull(ses);
            Assert.IsTrue(ses.Count() == 3);
            Assert.IsTrue(ses.Select(e => e.TimeStamp).SequenceEqual(new[] { dt2, dt1, dt3 }));

            Assert.IsTrue(ses.Select(e => e.TimeStamp).SequenceEqual(res.Select(e => e.TimeStamp)));
        }
        
        #endregion

        #region GetAllEvents from the stream with compressed and uncompressed events. Needs only for temporary time.

        private void AddUncompressed(Stream source, object events)
        {
            //Note: low level implementation details.
            var formatter = new BinaryFormatter
            {
                AssemblyFormat = FormatterAssemblyStyle.Simple
            };
            formatter.Serialize(source, events);
        }


        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetAllEvents_1UncompressedEvent2CompressedEventsInList_ReturnsEvents()
        {
            var res1 = new StateEventInfoPacketter();
            AddUncompressed(res1.PacketStream, new StateEventInfo { TimeStamp = DateTime.UtcNow.AddMinutes(10) });
            res1.AddToPacket(new List<StateEventInfo>
                              {
                                  new StateEventInfo { TimeStamp = DateTime.UtcNow },
                                  new StateEventInfo { TimeStamp = DateTime.UtcNow.AddMinutes(1) }
                              });

            res1.PacketStream.Position = 0;
            var dep = new StateEventInfoDepacker(res1.PacketStream);
            var res = dep.GetAllEvents();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Count() == 3);
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetAllEvents_1CompressedEvent2UncompressedEventsInList_ReturnsEvents()
        {
            var res1 = new StateEventInfoPacketter();
            res1.AddToPacket(new StateEventInfo { TimeStamp = DateTime.UtcNow.AddMinutes(10) });
            AddUncompressed(res1.PacketStream, new List<StateEventInfo>
                              {
                                  new StateEventInfo { TimeStamp = DateTime.UtcNow },
                                  new StateEventInfo { TimeStamp = DateTime.UtcNow.AddMinutes(1) }
                              });

            res1.PacketStream.Position = 0;
            var dep = new StateEventInfoDepacker(res1.PacketStream);
            var res = dep.GetAllEvents();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Count() == 3);
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetAllEvents_2UncompressedEvents_ReturnsEvents()
        {
            var res1 = new StateEventInfoPacketter();
            AddUncompressed(res1.PacketStream, new StateEventInfo { TimeStamp = DateTime.UtcNow.AddMinutes(10) });
            AddUncompressed(res1.PacketStream, new StateEventInfo { TimeStamp = DateTime.UtcNow.AddMinutes(11) });

            res1.PacketStream.Position = 0;
            var dep = new StateEventInfoDepacker(res1.PacketStream);
            var res = dep.GetAllEvents();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Count() == 2);
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetAllEvents_1Compressed1Uncompressed1CompressedEvent_ReturnsEvents()
        {
            var res1 = new StateEventInfoPacketter();
            res1.AddToPacket(new StateEventInfo { TimeStamp = DateTime.UtcNow.AddMinutes(10) });
            AddUncompressed(res1.PacketStream, new StateEventInfo { TimeStamp = DateTime.UtcNow.AddMinutes(11) });
            res1.AddToPacket(new StateEventInfo { TimeStamp = DateTime.UtcNow.AddMinutes(12) });

            res1.PacketStream.Position = 0;
            var dep = new StateEventInfoDepacker(res1.PacketStream);
            var res = dep.GetAllEvents();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Count() == 3);
        }

        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void GetAllEvents_1UncompressedEvent2UncompressedEventsInList_ReturnsEvents()
        {
            var res1 = new StateEventInfoPacketter();
            AddUncompressed(res1.PacketStream, new StateEventInfo { TimeStamp = DateTime.UtcNow.AddMinutes(10) });
            AddUncompressed(res1.PacketStream, new List<StateEventInfo>
                              {
                                  new StateEventInfo { TimeStamp = DateTime.UtcNow },
                                  new StateEventInfo { TimeStamp = DateTime.UtcNow.AddMinutes(1) }
                              });

            res1.PacketStream.Position = 0;
            var dep = new StateEventInfoDepacker(res1.PacketStream);
            var res = dep.GetAllEvents();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Count() == 3);
        }


        #endregion
    }
}
