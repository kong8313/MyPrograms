using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Common.Exceptions;
using System.Threading;
using System.Globalization;
using Confirmit.CATI.Core.Services.Fakes;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

namespace Confirmit.CATI.Core.UnitTests.Services
{
    [TestClass]
    public class ShiftServiceTests : BaseTest
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU", false);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();
        }

        [TestCleanup]
        public override void  TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }


        ShiftService.Shift CreateShift(int Id, int TzID, string StartTime, string FinishTime )
        {
            return new ShiftService.Shift()
            {
                ID = Id,
                ShiftTypeID = 1,
                TzID = TzID,
                StartTime = TimeSpan.Parse(StartTime),
                FinishTime = TimeSpan.Parse(FinishTime)
            };
        }

        ShiftService.Exclusion CreateExclusion(int Id, int TzID, string StartDate, string FinishDate)
        {
            return new ShiftService.Exclusion()
            {
                ID = Id,
                ShiftTypeID = -1,
                TzID = TzID,
                StartDate = DateTime.Parse(StartDate),
                FinishDate = DateTime.Parse(FinishDate)
            };
        }

        void CheckConfiguration(IEnumerable<ShiftService.Shift> shifts, IEnumerable<ShiftService.Exclusion> exclusions, string userErrorMessage)
        {
            bool isExeptionThrow = false;
            try
            {
                ShiftService.Create(shifts, exclusions).CheckConfiguration();
            }
            catch (UserMessageException ex)
            {
                if( userErrorMessage == null )
                    throw;

                Assert.AreEqual(userErrorMessage, ex.Message);

                isExeptionThrow = true;
            }

            if( userErrorMessage != null)
                Assert.IsTrue( isExeptionThrow, "UserMessageException wasn't thrown");
        }

        // Два дефолтных шифта в дефолтой Tz пересекаются между собой. 
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void TwoDefShiftWithCrossing_CheckConfiguration_ExceptionThrow()
        {
            var shifts = new ShiftService.Shift[]
                    {
                        CreateShift(1, 0, "1.10:00:00", "2.20:00:00"),
                        CreateShift(2, 0, "2.10:00:00", "3.20:00:00")
                    };

            var exclusions = new ShiftService.Exclusion[]
                    {
                    };

            CheckConfiguration(shifts, exclusions, 
                "Shift( ID = 1 TypeID = 1, TimezoneID = 0, StartTime = 1.10:00:00, FinishTime = 2.20:00:00) is crossing with shift( ID = 2 TypeID = 1, TimezoneID = 0, StartTime = 2.10:00:00, FinishTime = 3.20:00:00)");
        }


        // Два дефолтных шифта в дефолтой Tz. 
        // В Tz1 один из шифтов переопределен так, что он расширен и пересекается с вторым дефолтным шифтом
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void TwoDefShiftAndOneShiftInTz1WithCrossing_CheckConfiguration_ExceptionThrow()
        {
            var shifts = new ShiftService.Shift[]
                    {
                        CreateShift(1, 0, "1.10:00:00", "1.20:00:00"),
                        CreateShift(2, 0, "2.10:00:00", "2.20:00:00"),
                        CreateShift(2, 1, "1.10:00:00", "2.20:00:00")
                    };

            var exclusions = new ShiftService.Exclusion[]
                    {
                    };

            CheckConfiguration(shifts, exclusions, 
                "Shift( ID = 1 TypeID = 1, TimezoneID = 0, StartTime = 1.10:00:00, FinishTime = 1.20:00:00) is crossing with shift( ID = 2 TypeID = 1, TimezoneID = 1, StartTime = 1.10:00:00, FinishTime = 2.20:00:00)");
        }

        // Два дефолтных шифта в дефолтой Tz поменяны местами в Tz1
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void TwoDefShiftAndTwoShiftInTz1WithoutCrossing_CheckConfiguration_ExceptionNotThrow()
        {
            var shifts = new ShiftService.Shift[]
                    {
                        CreateShift(1, 0, "1.10:00:00", "1.20:00:00"),
                        CreateShift(2, 0, "2.10:00:00", "2.20:00:00"),
                        CreateShift(1, 1, "2.10:00:00", "2.20:00:00"),
                        CreateShift(2, 1, "1.10:00:00", "1.20:00:00")
                    };

            var exclusions = new ShiftService.Exclusion[]
                    {
                    };

            CheckConfiguration(shifts, exclusions, null);
        }

        // Два примыкающих друг к другу дефолтных шифта в дефолтой Tz
        // Два переопределенных шифта в Tz1 не пересекаются между собой
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void TwoContinuousDefShiftAndTwoShiftInTz1WithoutCrossing_CheckConfiguration_ExceptionNotThrow()
        {
            var shifts = new ShiftService.Shift[]
                    {
                        CreateShift(1, 0, "1.00:00:00", "2.00:00:00"),
                        CreateShift(2, 0, "2.00:00:00", "3.00:00:00"),
                        CreateShift(1, 1, "2.00:00:00", "3.00:00:00"),
                        CreateShift(2, 1, "1.00:00:00", "2.00:00:00")
                    };

            var exclusions = new ShiftService.Exclusion[]
                    {
                    };

            CheckConfiguration(shifts, exclusions, null);
        }

        // Два примыкающих друг к другу дефолтных шифта в дефолтой Tz
        // Два переопределенных шифта в Tz1 пересекаются между собой
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void TwoContinuousDefShiftAndTwoShiftInTz1WithCrossing_CheckConfiguration_ExceptionThrow()
        {
            var shifts = new ShiftService.Shift[]
                    {
                        CreateShift(1, 0, "1.00:00:00", "2.00:00:00"),
                        CreateShift(2, 0, "2.00:00:00", "3.00:00:00"),
                        CreateShift(1, 1, "2.00:00:00", "3.00:00:00"),
                        CreateShift(2, 1, "1.00:00:00", "2.00:01:00")
                    };

            var exclusions = new ShiftService.Exclusion[]
                    {
                    };

            CheckConfiguration(shifts, exclusions, 
                "Shift( ID = 2 TypeID = 1, TimezoneID = 1, StartTime = 1.00:00:00, FinishTime = 2.00:01:00) is crossing with shift( ID = 1 TypeID = 1, TimezoneID = 1, StartTime = 2.00:00:00, FinishTime = 3.00:00:00)");
        }

        //Два перемекающихся дефолных exlusion-а
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void TwoDefExclusionsWithCrossing_CheckConfiguration_ExceptionThrow()
        {
            var shifts = new ShiftService.Shift[]
                    {
                        CreateShift(1, 0, "1.00:00:00", "2.00:00:00"),
                        CreateShift(2, 0, "2.00:00:00", "3.00:00:00"),
                    };

            var exclusions = new ShiftService.Exclusion[]
                    {
                        CreateExclusion(1, 0, "2008-11-22T00:00:00", "2008-11-24T00:00:00" ),
                        CreateExclusion(2, 0, "2008-11-23T00:00:00", "2008-11-25T00:00:00" ),
                    };

            CheckConfiguration(shifts, exclusions, "Exclusion( ID = 1, TimezoneID = 0, StartDate = 22.11.2008 0:00:00, FinishDate = 24.11.2008 0:00:00) is crossing with Exclusion( ID = 2, TimezoneID = 0, StartDate = 23.11.2008 0:00:00, FinishDate = 25.11.2008 0:00:00)");
        }

        //Два подряд идущих дефолных exlusion-а не пересекаются
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void TwoContinuousDefExclusionsWithoutCrossing_CheckConfiguration_ExceptionNotThrow()
        {
            var shifts = new ShiftService.Shift[]
                    {
                        CreateShift(1, 0, "1.00:00:00", "2.00:00:00"),
                        CreateShift(2, 0, "2.00:00:00", "3.00:00:00"),
                    };

            var exclusions = new ShiftService.Exclusion[]
                    {
                        CreateExclusion(1, 0, "2008-11-22T00:00:00", "2008-11-24T00:00:00" ),
                        CreateExclusion(2, 0, "2008-11-24T00:00:00", "2008-11-26T00:00:00" ),
                    };

            CheckConfiguration(shifts, exclusions, null);
        }

        //Два дефолных exlusion-а не пересекаются
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void TwoDefExclusionsWithoutCrossing_CheckConfiguration_ExceptionNotThrow()
        {
            var shifts = new ShiftService.Shift[]
                    {
                        CreateShift(1, 0, "1.00:00:00", "2.00:00:00"),
                        CreateShift(2, 0, "2.00:00:00", "3.00:00:00"),
                    };

            var exclusions = new ShiftService.Exclusion[]
                    {
                        CreateExclusion(1, 0, "2008-11-22T00:00:00", "2008-11-24T00:00:00" ),
                        CreateExclusion(2, 0, "2008-11-25T00:00:00", "2008-11-27T00:00:00" ),
                    };

            CheckConfiguration(shifts, exclusions, null);
        }

        //Дефолтный exlusion пересекается в конце с exlusion в Tz1 
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DefExclusionIsCrossingWithTz1Exlusion_CheckConfiguration_ExceptionThrow()
        {
            var shifts = new ShiftService.Shift[]
                    {
                        CreateShift(1, 0, "1.00:00:00", "2.00:00:00"),
                        CreateShift(2, 0, "2.00:00:00", "3.00:00:00"),
                    };

            var exclusions = new ShiftService.Exclusion[]
                    {
                        CreateExclusion(1, 0, "2008-11-22T00:00:00", "2008-11-24T00:00:00" ),
                        CreateExclusion(2, 1, "2008-11-23T00:00:00", "2008-11-27T00:00:00" ),
                    };

            CheckConfiguration(shifts, exclusions, "Exclusion( ID = 1, TimezoneID = 0, StartDate = 22.11.2008 0:00:00, FinishDate = 24.11.2008 0:00:00) is crossing with Exclusion( ID = 2, TimezoneID = 1, StartDate = 23.11.2008 0:00:00, FinishDate = 27.11.2008 0:00:00)");
        }

        //Дефолтный exlusion пересекается в начале с exlusion в Tz1 
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DefExclusionIsCrossingWithTz2Exlusion_CheckConfiguration_ExceptionThrow()
        {
            var shifts = new ShiftService.Shift[]
                    {
                        CreateShift(1, 0, "1.00:00:00", "2.00:00:00"),
                        CreateShift(2, 0, "2.00:00:00", "3.00:00:00"),
                    };

            var exclusions = new ShiftService.Exclusion[]
                    {
                        CreateExclusion(1, 0, "2008-11-22T00:00:00", "2008-11-24T00:00:00" ),
                        CreateExclusion(2, 2, "2008-11-21T00:00:00", "2008-11-23T00:00:00" ),
                    };

            CheckConfiguration(shifts, exclusions, "Exclusion( ID = 2, TimezoneID = 2, StartDate = 21.11.2008 0:00:00, FinishDate = 23.11.2008 0:00:00) is crossing with Exclusion( ID = 1, TimezoneID = 0, StartDate = 22.11.2008 0:00:00, FinishDate = 24.11.2008 0:00:00)");
        }

        //Дефолтный exlusion примыкает к началу exlusion в Tz1 
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DefExclusionIsNotCrossingWithTz1Exlusion_CheckConfiguration_ExceptionNotThrow()
        {
            var shifts = new ShiftService.Shift[]
                    {
                        CreateShift(1, 0, "1.00:00:00", "2.00:00:00"),
                        CreateShift(2, 0, "2.00:00:00", "3.00:00:00"),
                    };

            var exclusions = new ShiftService.Exclusion[]
                    {
                        CreateExclusion(1, 0, "2008-11-22T00:00:00", "2008-11-24T00:00:00" ),
                        CreateExclusion(2, 1, "2008-11-24T00:00:00", "2008-11-26T00:00:00" ),
                    };

            CheckConfiguration(shifts, exclusions, null);
        }
        //Дефолтный exlusion примыкает к концу exlusion в Tz1 
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DefExclusionIsNotCrossingWithTz2Exlusion_CheckConfiguration_ExceptionNotThrow()
        {
            var shifts = new ShiftService.Shift[]
                    {
                        CreateShift(1, 0, "1.00:00:00", "2.00:00:00"),
                        CreateShift(2, 0, "2.00:00:00", "3.00:00:00"),
                    };

            var exclusions = new ShiftService.Exclusion[]
                    {
                        CreateExclusion(1, 0, "2008-11-22T00:00:00", "2008-11-24T00:00:00" ),
                        CreateExclusion(2, 1, "2008-11-20T00:00:00", "2008-11-22T00:00:00" ),
                    };

            CheckConfiguration(shifts, exclusions, null);
        }

        //Дефолтный exlusion расположен строго после exlusion в Tz1 
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DefExclusionIsNotCrossingWithTz3Exlusion_CheckConfiguration_ExceptionNotThrow()
        {
            var shifts = new ShiftService.Shift[]
                    {
                        CreateShift(1, 0, "1.00:00:00", "2.00:00:00"),
                        CreateShift(2, 0, "2.00:00:00", "3.00:00:00"),
                    };

            var exclusions = new ShiftService.Exclusion[]
                    {
                        CreateExclusion(1, 0, "2008-11-22T00:00:00", "2008-11-24T00:00:00" ),
                        CreateExclusion(2, 1, "2008-11-19T00:00:00", "2008-11-21T00:00:00" ),
                    };

            CheckConfiguration(shifts, exclusions, null);
        }

        //Дефолтный exlusion расположен строго перед exlusion в Tz1 
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DefExclusionIsNotCrossingWithTz4Exlusion_CheckConfiguration_ExceptionNotThrow()
        {
            var shifts = new ShiftService.Shift[]
                    {
                        CreateShift(1, 0, "1.00:00:00", "2.00:00:00"),
                        CreateShift(2, 0, "2.00:00:00", "3.00:00:00"),
                    };

            var exclusions = new ShiftService.Exclusion[]
                    {
                        CreateExclusion(1, 0, "2008-11-22T00:00:00", "2008-11-24T00:00:00" ),
                        CreateExclusion(2, 1, "2008-11-25T00:00:00", "2008-11-27T00:00:00" ),
                    };

            CheckConfiguration(shifts, exclusions, null);
        }

        //Два exclusion-а в дефолтной Tz переопределены местами в Tz1
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ToDefExclusionIsNotCrossingWithTowTz1Exlusions_CheckConfiguration_ExceptionNotThrow()
        {
            var shifts = new ShiftService.Shift[]
                    {
                        CreateShift(1, 0, "1.00:00:00", "2.00:00:00"),
                        CreateShift(2, 0, "2.00:00:00", "3.00:00:00"),
                    };

            var exclusions = new ShiftService.Exclusion[]
                    {
                        CreateExclusion(1, 0, "2008-11-22T00:00:00", "2008-11-24T00:00:00" ),
                        CreateExclusion(2, 0, "2008-11-24T00:00:00", "2008-11-26T00:00:00" ),
                        CreateExclusion(1, 1, "2008-11-24T00:00:00", "2008-11-26T00:00:00" ),
                        CreateExclusion(2, 1, "2008-11-22T00:00:00", "2008-11-24T00:00:00" ),
                    };

            CheckConfiguration(shifts, exclusions, null);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetNextValidTimeFromInvalid_CheckEvery30MinutesForTheNextYearForAllTimezones_InvalidDateTimeConvertsToValid()
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones();
            int cnt = 0;
            
            foreach (var timeZoneInfo in timeZones)
            {
                var dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                var finishDateTime = dateTime.AddYears(1);
               
                while (dateTime < finishDateTime)
                {
                    if (timeZoneInfo.IsInvalidTime(dateTime))
                    {
                        cnt++;
                        var testDateTime = ShiftService.MatchingShift.GetNextValidTimeFromInvalid(dateTime, timeZoneInfo);

                        Assert.IsFalse(timeZoneInfo.IsInvalidTime(testDateTime), $"Wrong time '{dateTime:yyyy-MM-dd HH:mm:ss}' in timezone '{timeZoneInfo.DisplayName}'");
                    }

                    dateTime = dateTime.AddMinutes(30);
                }
            }

            Assert.IsTrue(cnt > 0);
        }
    }
}
