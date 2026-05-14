using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.IntegrationTests.Framework.Fakes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.Test.Common.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Exclusion = Confirmit.CATI.IntegrationTests.Framework.Tools.Exclusion;
using Shift = Confirmit.CATI.IntegrationTests.Framework.Tools.Shift;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ScheduleCheck : BaseMockedIntegrationTest
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleCheck()
        {
            _scheduleService = ServiceLocator.Resolve<IScheduleService>();
        }
        
        private static Schedule DeserializeScheduleFromXml(string xml)
        {
            if ( string.IsNullOrEmpty( xml ) )
            {
                return new Schedule();
            }

            var xmlSerializer = new XmlSerializer( typeof(Schedule ) );
            var settings = new XmlReaderSettings();
            var stringReader = new StringReader( xml );

            var xmlReader = XmlReader.Create( stringReader, settings );

            return (Schedule) xmlSerializer.Deserialize( xmlReader );
        }

        private static void ScheduleUpdateAndLaunch( string scheduleName, TestScript script )
        {
            BvScheduleEntity schedule = ScheduleRepository.GetByName( scheduleName );

            schedule.XmlUnderDev = script.GenerateXML();
            ScheduleRepository.Update( schedule );

            ScheduleService.Launch( schedule.ScheduleID );
        }

        [Theory, Owner(@"FIRM\AlexeyN")]
        [ClassData(typeof(TestDataGenerator))]
        public void CreateValidSchedulingScript_CheckSuccess(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript( new Action( Action.Operation.SetNewITS, "1" ),
                new Shift( 1, 1, "2.12:00:00", "2.22:00:00" ),
                new Shift( 2, 1, "3.12:00:00", "3.22:00:00" ),
                new Shift( 3, 1, "4.12:00:00", "4.22:00:00" ),
                new Exclusion( 1, "2009-02-18T10:00:00Z", "2009-02-18T14:00:00Z" ) );

            Schedule schedule = DeserializeScheduleFromXml( script.GenerateXML() );

            _scheduleService.Check(schedule);
        }

        [Theory, Owner(@"FIRM\AlexeyN")]
        [ClassData(typeof(TestDataGenerator))]
        public void CreateSchedulingScript_ShiftsCrossed_CheckFailed(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript( new Action( Action.Operation.SetNewITS, "1" ),
                new Shift( 1, 1, "2.12:00:00", "2.16:00:00" ), // 
                new Shift( 2, 1, "2.10:00:00", "2.23:00:00" ), // crossed shifts
                new Shift( 3, 1, "3.12:00:00", "3.22:00:00" ),
                new Exclusion( 1, "2009-02-18T10:00:00Z", "2009-02-18T14:00:00Z" ) );

            string scriptXML = script.GenerateXML();

            // we need to mock validation, otherwise deserialize of incorrect
            // scheduling script will fail before tested ScheduleService.Check call.
            ServiceLocator.RegisterInstance<ISchedulingObjectValidator>(new FakeSchedulingObjectValidator());

            Schedule schedule = DeserializeScheduleFromXml( scriptXML );

            Xunit.Assert.Throws<UserMessageException>(() => _scheduleService.Check(schedule));
        }

        [Theory, Owner(@"FIRM\AlexeyN")]
        [ClassData(typeof(TestDataGenerator))]
        public void CreateSchedulingScript_ExclusionsCrossed_CheckFailed(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript( new Action( Action.Operation.SetNewITS, "1" ),
                new Shift( 1, 1, "2.12:00:00", "2.16:00:00" ),
                new Shift( 2, 1, "3.10:00:00", "3.23:00:00" ),
                new Exclusion( 1, "2009-02-18T10:00:00Z", "2009-02-18T14:00:00Z" ), //
                new Exclusion( 3, "2009-02-18T13:00:00Z", "2009-02-18T19:00:00Z" )  /* crossed exclusions */);

            string scriptXML = script.GenerateXML();

            // we need to mock validation, otherwise deserialize of incorrect
            // scheduling script will fail before tested ScheduleService.Check call.
            ServiceLocator.RegisterInstance<ISchedulingObjectValidator>(new FakeSchedulingObjectValidator());

            Schedule schedule = DeserializeScheduleFromXml( scriptXML );

            Xunit.Assert.Throws<UserMessageException>(() => _scheduleService.Check(schedule));
        }

        [Theory, Owner(@"FIRM\AlexeyN")]
        [ClassData(typeof(TestDataGenerator))]
        public void CreateSchedulingScript_ShiftHasInvalidShiftTypeId_CheckFailed(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript( new Action( Action.Operation.SetNewITS, "1" ),
                new Shift( 1, 1, "2.12:00:00", "2.16:00:00" ),
                new Shift( 2, 3, "3.10:00:00", "3.23:00:00" ),
                new Exclusion( 1, "2009-02-18T10:00:00Z", "2009-02-18T14:00:00Z" ),
                new Exclusion( 2, "2009-02-18T15:00:00Z", "2009-02-18T19:00:00Z" ) );

            string scriptXML = script.GenerateXML();

            Schedule schedule = DeserializeScheduleFromXml( scriptXML );

            //
            // Because shift types in this test were creates automatically we should
            // remove specific shift type to reproduce test situation
            schedule.ShiftTypes = GetCopyWithFirst2Elements(schedule.ShiftTypes);

            Xunit.Assert.Throws<UserMessageException>(() => _scheduleService.Check(schedule));
        }

        [Theory, Owner(@"FIRM\GrigoryK")]
        [ClassData(typeof(TestDataGenerator))]
        public void CreateSchedulingScript_DeserializeWrongScheduleFromXml_ExceptionHasThrown(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(new Action(Action.Operation.SetNewITS, "1"),
                new Shift(1, 1, "2.12:00:00", "2.16:00:00"),
                new Shift(2, 1, "3.10:00:00", "3.23:00:00"),
                new Exclusion(1, "2009-02-18T10:00:00Z", "2009-02-18T14:00:00Z"), //
                new Exclusion(3, "2009-02-18T13:00:00Z", "2009-02-18T19:00:00Z")  /* crossed exclusions */);

            string scriptXML = script.GenerateXML();

            Xunit.Assert.Throws<InvalidOperationException>(() => DeserializeScheduleFromXml(scriptXML));
        }

        private ShiftTypeCollection GetCopyWithFirst2Elements(ShiftTypeCollection shiftTypes)
        {
            var newShiftTypes = new ShiftTypeCollection {shiftTypes[0], shiftTypes[1]};
            return newShiftTypes;
        }

        /// <summary>
        /// 1. create scheduling script with 2 valid shifts and launch it
        /// 2. override one shift with specific timezone shift and launch
        /// 3. delete overriden shift with specific timezone
        /// 4. launch should be successful
        /// </summary>
        /// <param name="mode"></param>
        [Theory, Owner( @"FIRM\AlexeyN" ), Bug( 42289 )]
        [ClassData(typeof(TestDataGenerator))]
        public void OverrideShift_DeleteOverridedShift_LaunchSuccess(SecurityMode mode)
        {
            SetSecurityMode(mode);

            TimezoneManager.AddTimezone( 6 );
            const string scriptName = "OverrideShiftTest_ShedulingScript";

            //
            // create scheduling script
            var schedule = new BvScheduleEntity
            {
                Name = scriptName
            };

            ScheduleRepository.Insert( schedule );

            //
            // add 2 valid shifts and launch it
            var script = new TestScript( new Action( Action.Operation.SetNewITS, "30" ),
                new Shift( 1, 1, "2.12:00:00", "2.18:00:00" ),
                new Shift( 2, 1, "3.12:00:00", "3.18:00:00" ) );

            ScheduleUpdateAndLaunch( scriptName, script );

            //
            // override one shift with specific timezone shift and launch
            script = new TestScript( new Action( Action.Operation.SetNewITS, "30" ),
                new Shift( 1, 1, "2.12:00:00", "2.18:00:00" ),
                new Shift( 2, 1, new ShiftTimezone(null, "3.12:00:00", "3.18:00:00"),
                                 new ShiftTimezone(6, "3.12:00:00", "3.16:00:00")) );

            ScheduleUpdateAndLaunch( scriptName, script );

            //
            // delete overriden shift
            script = new TestScript( new Action( Action.Operation.SetNewITS, "30" ),
                new Shift( 1, 1, "2.12:00:00", "2.18:00:00" ) );

            ScheduleUpdateAndLaunch( scriptName, script );
        }
    }
}
