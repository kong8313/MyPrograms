using System;
using Confirmit.CATI.Common.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;

namespace Confirmit.CATI.Core.UnitTests.Scheduling
{
    [TestClass]
    public class ExclusionDataTest
    {
        private ISchedulingObjectValidator _validator;

        [TestInitialize]
        public void TestInitialize()
        {
            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();
            _validator = new SchedulingObjectValidator(null);
            ServiceLocator.RegisterInstance(_validator);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_UninitializedObject_ValidationFails()
        {
            ExclusionData data = new ExclusionData();
            ErrorCollection errors;

            Assert.IsFalse(_validator.Validate(data, out errors));
            Assert.AreEqual(2, errors.Count);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_CorrectPeriod_ValidationSuccess()
        {
            DateTime startDate = new DateTime(2007, 12, 4, 12, 0, 0);
            DateTime endDate = new DateTime(2007, 12, 4, 13, 0, 0);

            ErrorCollection errors;
            ExclusionData data = new ExclusionData(startDate, endDate);

            Assert.IsTrue(_validator.Validate(data, out errors));
            Assert.IsTrue(errors.Count == 0);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void Validate_IncorrectPeriod_ValidationFails()
        {
            DateTime startDate = new DateTime(2007, 12, 4, 13, 0, 0);
            DateTime endDate = new DateTime(2007, 12, 4, 12, 0, 0);

            ErrorCollection errors;
            ExclusionData data = new ExclusionData(startDate, endDate);

            Assert.IsFalse(_validator.Validate(data, out errors));
            Assert.IsTrue(errors.Count == 1);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ApplicationException))]
        public void HasIntersection_InvalidCurrentObject_ExceptionThrows()
        {
            ExclusionData data1 = new ExclusionData(
                new DateTime( 2008, 12, 10, 10, 0, 0 ),
                new DateTime( 2007, 11, 13, 10, 0, 0 )
                );

            ExclusionData data2 = new ExclusionData(
                new DateTime( 2007, 11, 11, 10, 0, 0 ),
                new DateTime( 2007, 11, 11, 10, 0, 0 )
                );

            data1.HasIntersection( data2 );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentException))]
        public void HasIntersection_InvalidGivenObject_ExceptionThrows()
        {
            ExclusionData data1 = new ExclusionData(
                new DateTime( 2007, 11, 11, 10, 0, 0 ),
                new DateTime( 2007, 11, 11, 10, 0, 0 )
                );

            ExclusionData data2 = new ExclusionData(
                new DateTime( 2007, 12, 10, 10, 0, 0 ),
                new DateTime( 2007, 11, 13, 10, 0, 0 )
                );

            data1.HasIntersection( data2 );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void HasIntersection_IntersectedObjects_ReturnsTrue()
        {
            ExclusionData data1 = new ExclusionData(
                new DateTime( 2007, 12, 11, 10, 0, 0 ),
                new DateTime( 2007, 12, 11, 11, 0, 0 )
                );

            ExclusionData data2 = new ExclusionData(
                new DateTime( 2005, 1, 12, 10, 0, 0 ),
                new DateTime( 2008, 4, 23, 10, 0, 0 )
                );

            Assert.IsTrue( data1.HasIntersection( data2 ) );
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void HasIntersection_NonintersectedObjects_ReturnsFalse()
        {
            ExclusionData data1 = new ExclusionData(
                new DateTime( 2007, 11, 13, 10, 0, 0 ),
                new DateTime( 2007, 11, 13, 11, 0, 0 )
                );

            ExclusionData data2 = new ExclusionData(
                new DateTime( 2007, 11, 13, 11, 0, 0 ),
                new DateTime( 2007, 11, 13, 23, 0, 0 )
                );

            Assert.IsFalse( data1.HasIntersection( data2 ) );
        }
    }
}