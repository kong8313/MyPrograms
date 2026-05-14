using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;

namespace Confirmit.CATI.Core.UnitTests.Services
{
    [TestClass]
    public class ScheduleServiceTest : BaseTest
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            var backendInstance = new BackendInstance();
            BackendInstance.Current = backendInstance;

            var stubICallCenterRepository = new StubICallCenterRepository
            {
                DefaultGet = () =>
                    new BvCallCenterEntity
                    {
                        ID = 1,
                        Name = "Default",
                        LocalTimezoneId = 1,
                        CanBeDeleted = false,
                        IsDefault = true
                    }
            };

            var registrator = UnitTestsServiceLocatorInitializer.InitializeServiceLocator();
            registrator.RegisterInstance<ICallCenterRepository>(stubICallCenterRepository);
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
            BackendInstance.Current = null;
        }

        void Value_CheckValue_CheckIsCorrect(int shiftTypeID, int shiftID, int stateGroupID, int personSID, int personGroupSID, SchedulingParameterType type, int value, bool result)
        {
            var schedule = new Schedule();

            schedule.ShiftTypes.Add(
                new ShiftType
                {
                    Id = shiftTypeID,
                    ColorInt = 10,
                    Name = "shift type name"
                });

            schedule.Shifts.Add(new Shift
            {
                Id = shiftID,
                ShiftTypeId = shiftTypeID,
                Timezones = new[]
                {
                    new BaseTimezoneData<ShiftData>
                    {
                        Id = null,
                        Data = new ShiftData
                        {
                            StartDayOfWeek = DayOfWeek.Monday,
                            StartTime = TimeSpan.FromMinutes(0),
                            EndDayOfWeek = DayOfWeek.Saturday,
                            EndTime = TimeSpan.FromMinutes(0),
                        }
                    }
                }
            });

            RegistryStub<ITimezoneService, StubITimezoneService>().GetTimezoneInfoInt32 = (id) => TimeZoneInfo.GetSystemTimeZones()[0];

            RegistryStub<IPersonRepository, StubIPersonRepository>().TryGetByIdInt32 = (id) => id == personSID ? new BvPersonEntity() : null;

            RegistryStub<IPersonGroupRepository, StubIPersonGroupRepository>().TryGetByIdInt32 = (id) => id == personGroupSID ? new BvPersonGroupEntity() : null;

            RegistryStub<ISurveyRepository, StubISurveyRepository>().GetByIdInt32 = (id) => new BvSurveyEntity { StateGroupID = stateGroupID };

            RegistryStub<IStateGroupRepository, StubIStateGroupRepository>().GetDefault = () => new BvStateGroupEntity();

            RegistryStub<IStateRepository, StubIStateRepository>().GetByItsAndStateGroupIdInt32Int32 = (its, group) =>
            {
                BvStateEntity res = null;

                if (its >= 1 && its <= 120)
                    res = new BvStateEntity();

                return res;
            };

            Assert.AreEqual( ServiceLocator.Resolve<IScheduleService>().CheckParamValue(schedule, 0, type, value), result);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void Int_Check_True()
        {
            Value_CheckValue_CheckIsCorrect(1, 1, 0, 0, 0, SchedulingParameterType.Integer, Int32.MinValue, true);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ExtendedStatusLessThanMinValue_Check_False()
        {
            Value_CheckValue_CheckIsCorrect(1, 1, 0, 0, 0, SchedulingParameterType.ExtendedStatus, 0, false);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ExtendedStatusIsMinValue_Check_True()
        {
            Value_CheckValue_CheckIsCorrect(1, 1, 0, 0, 0, SchedulingParameterType.ExtendedStatus, 1, true);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ExtendedStatusIs10_Check_True()
        {
            Value_CheckValue_CheckIsCorrect(1, 1, 0, 0, 0, SchedulingParameterType.ExtendedStatus, 10, true);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ExtendedStatusIsMaxValue_Check_True()
        {
            Value_CheckValue_CheckIsCorrect(1, 1, 0, 0, 0, SchedulingParameterType.ExtendedStatus, 120, true);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ExtendedStatusMoreThanMaxValue_Check_False()
        {
            Value_CheckValue_CheckIsCorrect(1, 1, 0, 0, 0, SchedulingParameterType.ExtendedStatus, 121, false);
        }

        public void InvalidResource_Check_False()
        {
            Value_CheckValue_CheckIsCorrect(1, 1, 0, 0, 0, SchedulingParameterType.Resource, -6, false);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ResourceUnchanged_Check_True()
        {
            Value_CheckValue_CheckIsCorrect(1, 1, 0, 0, 0, SchedulingParameterType.Resource, -1, true);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ResourceLastPerson_Check_True()
        {
            Value_CheckValue_CheckIsCorrect(1, 1, 0, 0, 0, SchedulingParameterType.Resource, -2, true);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ResourceSurveyInterviewers_Check_True()
        {
            Value_CheckValue_CheckIsCorrect(1, 1, 0, 0, 0, SchedulingParameterType.Resource, -3, true);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ResourcePerson_Check_True()
        {
            Value_CheckValue_CheckIsCorrect(1, 1, 0, 10, 0, SchedulingParameterType.Resource, 10, true);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ResourcePersonGroup_Check_True()
        {
            Value_CheckValue_CheckIsCorrect(1, 1, 0, 0, 20, SchedulingParameterType.Resource, 20, true);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ResourceWithNotExistsPersonOrGroup_Check_True()
        {
            Value_CheckValue_CheckIsCorrect(1, 1, 0, 0, 0, SchedulingParameterType.Resource, 20, false);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ShiftExists_Check_True()
        {
            Value_CheckValue_CheckIsCorrect(1, 20, 0, 0, 0, SchedulingParameterType.Shift, 20, true);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ShiftNotExists_Check_False()
        {
            Value_CheckValue_CheckIsCorrect(1, ShiftService.ScriptShiftIDToInternalShiftID(20), 0, 0, 0, SchedulingParameterType.Shift, 21, false);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ShiftTypeExists_Check_True()
        {
            Value_CheckValue_CheckIsCorrect(10, 20, 0, 0, 0, SchedulingParameterType.ShiftType, 10, true);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ShiftTypeNotExists_Check_False()
        {
            Value_CheckValue_CheckIsCorrect(10, 20, 0, 0, 0, SchedulingParameterType.ShiftType, 11, false);
        }
    }
}
