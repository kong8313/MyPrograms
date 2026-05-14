using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml.Serialization;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;

namespace Confirmit.CATI.Core.UnitTests.Scheduling
{
    [TestClass]
    public class ScheduleTest : BaseTest
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();

            ServiceLocator.RegisterInstance<ISchedulingObjectValidator>(new SchedulingObjectValidator(null));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Serialize_XmlSerialization_Success()
        {
            Schedule schedule = ScheduleCreator.GetSchedule();

            using(MemoryStream stream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer( typeof( Schedule ) );
                serializer.Serialize( stream, schedule );
            }
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Deserialize_XmlDeserialization_Success()
        {
            string xmlSchedule = ScheduleCreator.GetXmlSchedule();

            using(StringReader reader = new StringReader( xmlSchedule ))
            {
                XmlSerializer serializer = new XmlSerializer( typeof( Schedule ) );
                object result = serializer.Deserialize( reader );
            }
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Deserialize_XmlDeserializationWithCheck_Success()
        {
            XmlSerializer serializer = new XmlSerializer( typeof( Schedule ) );
            Schedule schedule = ScheduleCreator.GetSchedule();
			
            string serializationResult = TestUtility.SerializeSchedule(schedule);
            Schedule deserializedSchedule = TestUtility.DeserializeSchedule(serializationResult);
            // we are going to serialize new deserialized object and check the result
            // with the previous saved one
            string serializationResult2 = TestUtility.SerializeSchedule(deserializedSchedule);

            Assert.AreEqual(serializationResult, serializationResult2);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetUsedTimezoneIds_EmptyShiftsExclusions_ReturnsEmptyArray()
        {
            Schedule schedule = new Schedule();

            int[] result = schedule.GetUsedTimezoneIds();
            Assert.AreEqual<int>( result.Length, 0 );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetUsedTimezoneIds_FilledSchedule_Success()
        {
            Schedule schedule = ScheduleCreator.GetSchedule();
            int[] result = schedule.GetUsedTimezoneIds();

            Assert.AreEqual<int>( result.Length, 3 );
            Assert.IsTrue( Array.IndexOf( result, Shift.RespondentTimezoneId ) >= 0 );
            Assert.IsTrue( Array.IndexOf( result, 1 ) >= 0 );
            Assert.IsTrue( Array.IndexOf( result, 2 ) >= 0 );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetNumberByGuid_UnexistingObject_Fails()
        {
            Guid newGuid = Guid.NewGuid();

            Assert.IsTrue( String.IsNullOrEmpty( ScheduleCreator.GetSchedule().GetNumberByGuid( newGuid ) ) );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetNumberByGuid_ExistingRule_Success()
        {
            Schedule schedule = ScheduleCreator.GetSchedule();
            Guid guid = schedule.Rules[1].Id.Value;

            Assert.AreEqual( "2", schedule.GetNumberByGuid( guid ) );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetNumberByGuid_ExistingSubRule_Success()
        {
            Schedule schedule = ScheduleCreator.GetSchedule();
            Guid guid = schedule.Rules[0].SubRules[1].Id.Value;

            Assert.AreEqual<string>( schedule.GetNumberByGuid( guid ), "1.2" );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(FormatException))]
        public void GetGuidByNumber_WrongNumberFormat_ExceptionThrows()
        {
            ScheduleCreator.GetSchedule().GetGuidByNumber( "234.23d" );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetGuidByNumber_FirstRule_ReturnsRuleGuid()
        {
            Schedule schedule = ScheduleCreator.GetSchedule();
            Guid firstRuleGuid = schedule.Rules[0].Id.Value;

            Assert.AreEqual<Guid>( schedule.GetGuidByNumber( "1" ), firstRuleGuid );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetGuidByNumber_LastSubRuleOfFirstRule_ReturnsSubRuleGuid()
        {
            Schedule schedule = ScheduleCreator.GetSchedule();
            Guid subRuleGuid = schedule.Rules[1].SubRules[0].Id.Value;

            Assert.AreEqual<Guid>( schedule.GetGuidByNumber( "2.1" ), subRuleGuid );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void OnRemovingRule_UnusedRule_Success()
        {
            Schedule schedule = ScheduleCreator.GetSchedule();
            ErrorCollection errors;
            Assert.IsTrue( schedule.Rules.RemoveAt( 1, out errors ) );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void OnRemovingRule_UsedRule_Fails()
        {
            Schedule schedule = ScheduleCreator.GetSchedule();
            ErrorCollection errors;
            Assert.IsFalse( schedule.Rules.RemoveAt( 0, out errors ) );
            Assert.AreEqual<int>( errors.Count, 1 );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void OnRemovingRule_SubRuleActionWithWrongParameter_WritesMessageToLog()
        {
            var stub = new SchedulingObjectValidatorStub();
            ServiceLocator.RegisterInstance<ISchedulingObjectValidator>(stub);

            ErrorCollection errors;
            Schedule schedule = ScheduleCreator.GetSchedule();
            SubRuleAction action = new SubRuleAction();
            SubRuleActionCollection coll = schedule.Rules[0].SubRules[0].SubRuleActions;
            action.Id = coll.GetNewId();
            // set next rule action identifier 
            action.ActionId = 25;
            action.Enabled = true;
            action.Parameter.Constant = "sdf312";

            coll.Add( action );

            schedule.Rules.RemoveAt( 1, out errors );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Clone_CorrectObject_Success()
        {
            Schedule schedule = ScheduleCreator.GetSchedule();

            Assert.IsTrue(
                TestUtility.CompareScheduleWithSerialization( schedule, (Schedule)schedule.Clone() )
                );
        }

        class SchedulingObjectValidatorStub : ISchedulingObjectValidator
        {
            public bool Validate<T>(T item, out ErrorCollection errors)
            {
                errors = new ErrorCollection();
                return true;
            }

            public bool ValidateWithCollection<T, TType>(BaseCollection<T, TType> baseCollection, T item, out ErrorCollection errors)
                where T : BaseObject<TType>
                where TType : struct
            {
                errors = new ErrorCollection();
                return true;
            }
        }
    }
}