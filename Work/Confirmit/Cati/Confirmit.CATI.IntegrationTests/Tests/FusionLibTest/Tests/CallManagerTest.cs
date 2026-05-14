using System;
using System.Linq;
using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.FilterServiceImplementation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.Services;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.Tests.FusionLibTest.Tests
{
    [TestClass]
    public class CallManagerTest : BaseMockedIntegrationTest
    {
        private int _surveySID;
        private const int Count = 4;//count of created objects in tests (it should not be greater than possible amount of its.
        //because we set its the same as interview id 
        private const int Priority = 5;//current priority of calls in tests
        private int _filterID; //filter ID
        private int _personID; // person ID
        private readonly BvInterviewEntity[] _interviews = new BvInterviewEntity[Count]; // interviews for test
        private readonly BvCallEntity[] _calls = new BvCallEntity[Count]; // calls for test

        private ISurveyStateService _surveyStateService;
        private IPersonRepository _personRepository;
        private IInterviewRepository _interviewRepository;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            _personRepository = ServiceLocator.Resolve<IPersonRepository>();
            _interviewRepository = ServiceLocator.Resolve<IInterviewRepository>();
        }

        #region Support methods

        /// <summary>
        /// Creates simple filter for testing Call Management.
        /// There are _count interviews. all of them has ITS
        /// the same as id. we should make filter for getting 
        /// 2 calls. it means its should not be bigger than 2.
        /// </summary>
        /// <returns>Filter ID</returns>
        private static int CreateFilterForTest()
        {
            var filter = new BvFiltersEntity { Name = ("test filter") };

            int filterID = FilterRepository.Insert(filter);
            var filterFields = new List<BvFilterFieldsEntity>();
            var filterField = new BvFilterFieldsEntity
            {
                Table = (int) TableTypes.Interview,
                Column = "TransientState",
                Type = (int) VariableTypes.Integer,
                Sign = (int) FilterOperator.LessEqual,
                Value = "2"
            };

            filterFields.Add(filterField);
            FilterService.SetFields(filterID, filterFields);

            return filterID;
        }

        /// <summary>
        /// We should not create calls in Suspended mode.
        /// We create calls for all interviews in Scheduled mode.
        /// We don't create calls for every second interview for All interviews mode.
        /// </summary>
        /// <param name="callState"></param>
        private void CreateCalls(CallStates callState)
        {
            if (callState == CallStates.Suspended)
                return;

            var defaultResource = _surveySID;

            for (int i = 0; i < Count; ++i)
            {
                if (callState != CallStates.All ||
                    (i & 1) != 1)
                {
                    _calls[i] = new BvCallEntity
                    {
                        InterviewID = _interviews[i].ID,
                        SurveySID = _surveySID,
                        CallState = 2,
                        ShiftID = (int)CallShiftType.None,
                        Priority = Priority,
                        ResourceType = 1,
                        Resource = defaultResource
                    };

                    CallQueueService.AddCall(_calls[i], 0, 0);
                    _calls[i].CallID = CallQueueService.GetCallAndNoLock(_surveySID, _interviews[i].ID).CallID;
                }
            }
        }

        /// <summary>
        /// Creates survey.
        /// Opens survey.
        /// Adds scheduling script.
        /// Creates 4 interviews.
        /// Creates 4 calls if withCall = true.
        /// Creates person.
        /// Assignes person to survey if assignPersonToSurvey = true.
        /// </summary>
        /// <param name="selectMode"></param>
        /// <param name="callState"></param>
        /// <param name="assignPersonToSurvey">If true person is assigned to survey.</param>
        /// <param name="setShiftType"></param>
        private void PrepareDataForTest(SelectMode selectMode, CallStates callState, bool assignPersonToSurvey, bool setShiftType)
        {
            if (setShiftType)
            {
                var script = new TestScript(
                new Action(Action.Operation.SetNewITS, "17"),
                new object[]{new Shift(1, (int)ShiftTypeIDs.Sunday, "0.00:00:00", "1.00:00:00"),
                      new Shift(2, (int)ShiftTypeIDs.Default, "1.00:00:00", "0.00:00:00")});

                _surveySID = BackendToolsObject.CreateSurvey(script);
            }
            else
            {
                _surveySID = BackendToolsObject.CreateSurvey("p0000111");

                BackendToolsObject.LaunchAllHoursScript();
                _surveyStateService.Open(_surveySID);
            }

            //create interviews all interviews have its = id
            for (int i = 0; i < Count; i++)
            {
                int id = i + 1;
                _interviews[i] = new BvInterviewEntity
                {
                    ID = id,
                    SurveySID = _surveySID,
                    TransientState = id
                };

                _interviewRepository.InsertOnly(_interviews[i]);
            }

            CreateCalls(callState);

            if (selectMode == SelectMode.CustomFilter)
            {
                _filterID = CreateFilterForTest();
            }

            var person = new BvPersonEntity
            {
                Name = "interviewer1",
                Description = "interviewer1 description",
                CallCenterID = CallCenterTools.DefaultId
            };

            _personID = _personRepository.Insert(person);
            if (assignPersonToSurvey)
                BackendTools.AssignCatiPersonToSurvey(_surveySID, _personID);
        }        
        #endregion

        #region DeleteCalls()

        /// <summary>
        /// Base test method for DeleteCalls.
        /// Creates 4 calls, 
        /// calls method DeleteCalls for 1st and 2nd of them, 
        /// checks results for all calls.
        /// </summary>
        /// <param name="selectMode"></param>
        private void Test_DeleteCalls(SelectMode selectMode)
        {
            PrepareDataForTest(selectMode, CallStates.Scheduled, true, false);

            if (selectMode != SelectMode.Selected)
            {
                CallTools.DeleteCalls(_surveySID, _filterID, 1, CallStates.Scheduled, null);
            }
            else
                CallTools.DeleteCalls(_surveySID, new[] { _calls[0].InterviewID, _calls[1].InterviewID });

            BackendTools.LoginPerson(_personID, "");
            
            int countOfProcessedInterview = (selectMode == SelectMode.All ? Count : 2);

            for (int i = 0; i < Count; i++)
            {
                if (i < countOfProcessedInterview) // only 1st and 2nd calls should be deleted
                {
                    Assert.IsFalse(BackendTools.IsCallExists(_surveySID, _interviews[i].ID));
                }
                else
                {
                    Assert.IsTrue(BackendTools.IsCallExists(_surveySID, _interviews[i].ID));

                    // check if calls are given by LookupByPersonSID
                    BvTasksEntity task = TaskService.LookupByPersonSid(_personID, _surveySID);
                    Assert.AreEqual(task.CallID, _calls[i].CallID);
                }
            }
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void DeleteCalls_CallsExist_CallsDeleted()
        {
            Test_DeleteCalls(SelectMode.Selected);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void DeleteCalls_CallsExistAndCustomFiltered_CallsDeleted()
        {
            Test_DeleteCalls(SelectMode.CustomFilter);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void DeleteCalls_CallsExistAndDefaultFiltered_CallsDeleted()
        {
            Test_DeleteCalls(SelectMode.All);
        }

        #endregion

        #region ChangeCallsShiftType()

        /// <summary>
        /// Base test method for ChangeCallsShiftType.
        /// Creates 4 calls, 
        /// calls method ChangeCallsShiftType for 1st and 2nd of them, 
        /// checks results for all calls.
        /// </summary>
        /// <param name="selectMode">If true filter is created for test</param>
        private void Test_ChangeCallsShiftType(SelectMode selectMode)
        {
            PrepareDataForTest(selectMode, CallStates.Scheduled, true, true);

            const int newShiftTypeID = (int)ShiftTypeIDs.Sunday;
            int dbShiftTypeID =
                SurveyManager.GetShiftTypes(_surveySID).Find(x => x.Id == newShiftTypeID).ObjectId;


            if (selectMode != SelectMode.Selected)
            {
                CallTools.ChangeCallsShiftType(_surveySID, _filterID, CallStates.Scheduled,
                                                 dbShiftTypeID);
            }
            else
                CallTools.ChangeCallsShiftType(_surveySID, new[] { _calls[0].CallID, _calls[1].CallID },
                                                 CallStates.Scheduled, dbShiftTypeID);

            _surveyStateService.Open(_surveySID);

            int countOfProcessedInterview = (selectMode == SelectMode.All ? Count : 2);

            for (int i = 0; i < Count; i++)
            {
                if (i < countOfProcessedInterview)
                {
                    _calls[i].ShiftID = BackendTools.GetShiftTypeWorkID(newShiftTypeID);
                    BackendTools.CheckCall(_calls[i]);
                }
                else
                {
                    BvCallEntity actualCall = CallQueueService.GetCallAndNoLock(_calls[i].SurveySID, _calls[i].InterviewID);
                    TestAssert.AreEqual(_calls[i], actualCall);
                }
            }
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void ChangeCallsShiftType_CallsExist_ShiftTypeChanged()
        {
            Test_ChangeCallsShiftType(SelectMode.Selected);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void ChangeCallsShiftType_CallsExistAndFiltered_ShiftTypeChanged()
        {
            Test_ChangeCallsShiftType(SelectMode.CustomFilter);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void ChangeCallsShiftType_CallsExistAndCustomFiltered_ShiftTypeChanged()
        {
            Test_ChangeCallsShiftType(SelectMode.CustomFilter);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void ChangeCallsShiftType_CallsExistAndDefaultFiltered_ShiftTypeChanged()
        {
            Test_ChangeCallsShiftType(SelectMode.All);
        }

        class Callinfo
        {
            public int TzID;
            public int ShiftId;
            public int NewShiftId;
        }

        void Test_ChangeCallsShiftType(int newShiftId, Callinfo[] testData)
        {
            // create survey with scheduling script
            var script = new TestScript(
                new Action(Action.Operation.SetNewITS, "17"),
                new object[]{new Shift(1, (int)ShiftTypeIDs.Sunday, "0.00:00:00", "1.00:00:00"),
                      new Shift(2, (int)ShiftTypeIDs.Default, "1.00:00:00", "0.00:00:00")});

            int surveySID = BackendToolsObject.CreateSurvey(script);


            // create interviewas with calls
            var interviews = new BvInterviewEntity[testData.Length];
            var calls = new BvCallEntity[testData.Length];

            for( int i = 0 ; i < testData.Length ; i++ )
            {
                var data = testData[i];

                var interview = BackendTools.NewInterview(surveySID);
                interview.TimezoneID = data.TzID;
                BackendTools.CreateInterview(interview);

                var call = BackendTools.NewCall(interview);
                
                call.ShiftID = data.ShiftId > 0 ? script.GetShiftTypeWorkID( data.ShiftId ) : data.ShiftId;

                CallQueueService.AddCall(call, 0, interview.TransientState);

                call.CallID =  CallQueueService.GetCallAndNoLock(surveySID, interview.ID, 0, false).CallID;
                
                call.ShiftID = data.NewShiftId > 0 ? script.GetShiftTypeWorkID( data.NewShiftId ) : data.NewShiftId;

                interviews[i] = interview;
                calls[i] = call;
            }

            //change shift type
            if (newShiftId > 0)//specific shift
                CallTools.ChangeCallsShiftType(surveySID, calls.Select(x => x.InterviewID).ToArray(), CallStates.Scheduled, script.GetShiftTypeWorkID(newShiftId));
            else
                CallTools.ChangeCallsShiftType(surveySID, calls.Select(x => x.InterviewID).ToArray(), CallStates.Scheduled, newShiftId);
            
            //check result

            foreach( var call in calls )
            {
                BackendTools.CheckCall( call );
            }
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void CallsWithDifferentShiftTypes_ChangeShiftTypeToNone_AllCallsCorrected()
        {
            TimezoneManager.AddTimezone(1);
            TimezoneManager.AddTimezone(10);
            
            Test_ChangeCallsShiftType(
                (int)CallShiftType.None,//new shift
                new[]{
                    new Callinfo{ 
                        TzID = 0, 
                        ShiftId = (int)CallShiftType.None, 
                        NewShiftId = (int)CallShiftType.None
                    },
                    new Callinfo{ 
                        TzID = 1, 
                        ShiftId = (int)CallShiftType.None, 
                        NewShiftId = (int)CallShiftType.None
                    },
                    new Callinfo{ 
                        TzID = 10, 
                        ShiftId = (int)CallShiftType.None, 
                        NewShiftId = (int)CallShiftType.None
                    },
                    new Callinfo{ 
                        TzID = 0, 
                        ShiftId = (int)CallShiftType.AnyValid, 
                        NewShiftId = (int)CallShiftType.None
                    },
                    new Callinfo{ 
                        TzID = 1, 
                        ShiftId = (int)CallShiftType.AnyValid, 
                        NewShiftId = (int)CallShiftType.None
                    },
                    new Callinfo{ 
                        TzID = 10, 
                        ShiftId = (int)CallShiftType.AnyValid, 
                        NewShiftId = (int)CallShiftType.None
                    },
                    new Callinfo{ 
                        TzID = 0, 
                        ShiftId = (int)ShiftTypeIDs.Sunday, 
                        NewShiftId = (int)CallShiftType.None
                    },
                    new Callinfo{ 
                        TzID = 1, 
                        ShiftId = (int)ShiftTypeIDs.Sunday, 
                        NewShiftId = (int)CallShiftType.None
                    },
                    new Callinfo{ 
                        TzID = 10, 
                        ShiftId = (int)ShiftTypeIDs.Sunday, 
                        NewShiftId = (int)CallShiftType.None
                    }
                });
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void CallsWithDifferentShiftTypes_ChangeShiftTypeToAnyValid_AllCallsCorrected()
        {
            TimezoneManager.AddTimezone(1);
            TimezoneManager.AddTimezone(10);

            Test_ChangeCallsShiftType(
                (int)CallShiftType.AnyValid,//new shift
                new[]{
                    new Callinfo{ 
                        TzID = 0, 
                        ShiftId = (int)CallShiftType.None, 
                        NewShiftId = 0
                    },
                    new Callinfo{ 
                        TzID = 1, 
                        ShiftId = (int)CallShiftType.None, 
                        NewShiftId = -1
                    },
                    new Callinfo{ 
                        TzID = 10, 
                        ShiftId = (int)CallShiftType.None, 
                        NewShiftId = -10
                    },
                    new Callinfo{ 
                        TzID = 0, 
                        ShiftId = (int)CallShiftType.AnyValid, 
                        NewShiftId = 0
                    },
                    new Callinfo{ 
                        TzID = 1, 
                        ShiftId = (int)CallShiftType.AnyValid, 
                        NewShiftId = -1
                    },
                    new Callinfo{ 
                        TzID = 10, 
                        ShiftId = (int)CallShiftType.AnyValid, 
                        NewShiftId = -10
                    },
                    new Callinfo{ 
                        TzID = 0, 
                        ShiftId = (int)ShiftTypeIDs.Sunday, 
                        NewShiftId = 0
                    },
                    new Callinfo{ 
                        TzID = 1, 
                        ShiftId = (int)ShiftTypeIDs.Sunday, 
                        NewShiftId = -1
                    },
                    new Callinfo{ 
                        TzID = 10, 
                        ShiftId = (int)ShiftTypeIDs.Sunday, 
                        NewShiftId = -10
                    }
                });
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void CallsWithDifferentShiftTypes_ChangeShiftTypeToSpecificShift_AllCallsCorrected()
        {
            TimezoneManager.AddTimezone(1);
            TimezoneManager.AddTimezone(10);

            Test_ChangeCallsShiftType(
                (int)ShiftTypeIDs.Default,//new shift
                new[]{
                    new Callinfo{ 
                        TzID = 0, 
                        ShiftId = (int)CallShiftType.None, 
                        NewShiftId = (int)ShiftTypeIDs.Default
                    },
                    new Callinfo{ 
                        TzID = 1, 
                        ShiftId = (int)CallShiftType.None, 
                        NewShiftId = (int)ShiftTypeIDs.Default
                    },
                    new Callinfo{ 
                        TzID = 10, 
                        ShiftId = (int)CallShiftType.None, 
                        NewShiftId = (int)ShiftTypeIDs.Default
                    },
                    new Callinfo{ 
                        TzID = 0, 
                        ShiftId = (int)CallShiftType.AnyValid, 
                        NewShiftId = (int)ShiftTypeIDs.Default
                    },
                    new Callinfo{ 
                        TzID = 1, 
                        ShiftId = (int)CallShiftType.AnyValid, 
                        NewShiftId = (int)ShiftTypeIDs.Default
                    },
                    new Callinfo{ 
                        TzID = 10, 
                        ShiftId = (int)CallShiftType.AnyValid, 
                        NewShiftId = (int)ShiftTypeIDs.Default
                    },
                    new Callinfo{ 
                        TzID = 0, 
                        ShiftId = (int)ShiftTypeIDs.Sunday, 
                        NewShiftId = (int)ShiftTypeIDs.Default
                    },
                    new Callinfo{ 
                        TzID = 1, 
                        ShiftId = (int)ShiftTypeIDs.Sunday, 
                        NewShiftId = (int)ShiftTypeIDs.Default
                    },
                    new Callinfo{ 
                        TzID = 10, 
                        ShiftId = (int)ShiftTypeIDs.Sunday, 
                        NewShiftId = (int)ShiftTypeIDs.Default
                    }
                });
        }
        #endregion

        #region AssignCalls()

        /// <summary>
        /// Base test method for AssignCalls.
        /// Creates 4 calls, 
        /// calls method AssignCalls for 1st and 2nd of them, 
        /// checks results for all calls.
        /// </summary>
        /// <param name="selectMode"></param>
        /// <param name="toPerson">If true calls are assigned to person; otherwise to group of persons</param>  
        private void Test_AssignCalls(SelectMode selectMode, bool toPerson)
        {
            PrepareDataForTest(selectMode, CallStates.Scheduled, false, false);

            // create person or group
            int personOrGrouID;
            if (toPerson)
                personOrGrouID = _personID;
            else
            {
                personOrGrouID = PersonManager.CreatePersonGroup(new BvPersonGroupEntity() { Name = "Test group", Description = "Test group description" }, new[] { PersonManager.GetCatiRootID() });
                _personID = PersonTools.CreatePerson("Test person in group", "password", AgentTaskChoiceMode.Automatic, new[] { personOrGrouID });
            }

            if (selectMode != SelectMode.Selected)
            {
                CallTools.AssignCalls(_surveySID, _filterID, personOrGrouID);
            }
            else
                CallTools.AssignCalls(_surveySID, new[] { _interviews[0].ID, _interviews[1].ID }, personOrGrouID);

            BackendTools.LoginPerson(_personID, "");

            int countOfProcessedInterview = (selectMode == SelectMode.All ? Count : 2);

            for (int i = 0; i < Count; i++)
            {
                BvCallEntity call = CallQueueService.GetCallAndNoLock(_surveySID, _interviews[i].ID);
                if (i < countOfProcessedInterview)
                {
                    call.Resource = personOrGrouID;

                    TestAssert.AreEqual(call, CallQueueService.GetCallAndNoLock(call.SurveySID, call.InterviewID));

                    BvTasksEntity task = TaskService.LookupByPersonSid(_personID, _surveySID);
                    Assert.AreEqual(task.CallID, call.CallID);
                }
                else
                {
                    BackendTools.CheckCall(call);
                }
            }
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void AssignCalls_AssignToPerson_CallsAssigned()
        {
            Test_AssignCalls(SelectMode.Selected, true);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void AssignCalls_AssignToPersonAndCustomFiltered_CallsAssigned()
        {
            Test_AssignCalls(SelectMode.CustomFilter, true);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void AssignCalls_AssignToPersonAndDefaultFiltered_CallsAssigned()
        {
            Test_AssignCalls(SelectMode.All, true);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void AssignCalls_AssignToGroup_CallsAssigned()
        {
            Test_AssignCalls(SelectMode.Selected, false);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void AssignCalls_AssignToGroupAndCustomFiltered_CallsAssigned()
        {
            Test_AssignCalls(SelectMode.CustomFilter, false);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void AssignCalls_AssignToGroupAndDefaultFiltered_CallsAssigned()
        {
            Test_AssignCalls(SelectMode.All, false);
        }
        #endregion

        #region UpdateCall()

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void UpdateCall_CallExists_CallUpdated()
        {
            _surveySID = BackendToolsObject.CreateSurvey("p0000123");

            _surveyStateService.Open(_surveySID);
            BackendToolsObject.LaunchAllHoursScript();

            var interview = new BvInterviewEntity
            {
                ID = 1,
                SurveySID = _surveySID,
                TransientState = 1
            };
            _interviewRepository.InsertOnly(interview);

            DateTime now = DateTime.Now;
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second).ToUniversalTime();

            // create call with priority=1, roleID=1
            var call = new BvCallEntity
                                    {
                                        InterviewID = interview.ID,
                                        SurveySID = _surveySID,
                                        Priority = 1,
                                        ShiftID = (int)CallShiftType.None,
                                        TimeInShift = now.AddMonths(-10),
                                        TimeToExpire = now.AddMonths(10)
                                    };

            CallQueueService.AddCall(call, 0, interview.TransientState);
            BvCallEntity existingCall = CallQueueService.GetCallAndNoLock(_surveySID, interview.ID);
            int callID = existingCall.CallID;

            // update call 
            var newCall = new BvCallEntity
                                       {
                                           CallID = callID,
                                           SurveySID = _surveySID,
                                           InterviewID = interview.ID,
                                           CallState = 2,
                                           ShiftID = (int)CallShiftType.None,//None
                                           Priority = 2,
                                           TimeInShift = now,
                                           TimeToExpire = now.AddMonths(5)
                                       };

            CallManager.UpdateCall(newCall);

            var person = new BvPersonEntity
                                     {
                                         Name = "interviewer1",
                                         Description = "interviewer1 description",
                                         CallCenterID = CallCenterTools.DefaultId
                                     };
            int personID = _personRepository.Insert(person);
            BackendTools.AssignCatiPersonToSurvey(_surveySID, personID);

            BackendTools.RunSchedulingProcedure();

            TestAssert.AreEqual(newCall, CallQueueService.GetCallAndNoLock(_surveySID, interview.ID));
            BackendTools.LoginPerson(personID, "");
            
            BvTasksEntity task = TaskService.LookupByPersonSid(personID, _surveySID);
            Assert.AreEqual(task.CallID, callID);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void UpdateCall_CallHasTimeZoneWithID3AndTimeToCallLessThenRightBounfOfShiftLess1Hour_CallisUpdated()
        {
            const int shiftTypeID = 1;
            const int respondentTimeZoneID = 3;

            var script = new TestScript(
                new Action(Action.Operation.SetNewITS, "17"),
                new Shift(1, shiftTypeID, "0.08:00:00", "0.20:00:00"));

            int surveySID = BackendToolsObject.CreateSurvey(script);

            //activate all timezones
            TimezoneManager.AddTimezone(respondentTimeZoneID);

            //create interview
            var interview = new BvInterviewEntity
                                              {
                                                  ID = 1,
                                                  SurveySID = surveySID,
                                                  TimezoneID = respondentTimeZoneID
                                              };
            BackendTools.CreateInterview(interview);

            //create call
            var call = new BvCallEntity
                                    {
                                        Priority = 1,
                                        SurveySID = surveySID,
                                        InterviewID = interview.ID,
                                        TimeInShift = DateTime.Parse("2009-02-15T13:00:00"),
                                        TimeToExpire = DateTime.Parse("4999-02-15T13:00:00")
                                    };

            BackendTools.CreateCall(call);

            // update call

            BvCallEntity newCall = CallQueueService.GetCallAndNoLock(surveySID, interview.ID);
            newCall.CallState = 2;
            newCall.Priority = 2;
            DateTime dt = DateTime.Parse("2009-02-15T19:45:00");
            //convert time from respondent time zone to utc
            dt = TimezoneManager.ConvertToUTC(respondentTimeZoneID, dt);
            newCall.TimeInShift = dt;

            int dbShiftTypeID = SurveyManager.GetShiftTypes(surveySID).Find(x => x.Id == shiftTypeID).ObjectId;

            newCall.ShiftID = dbShiftTypeID;

            CallManager.UpdateCall(newCall);

            call = CallQueueService.GetCallAndNoLock(surveySID, interview.ID);
            int callShiftTypeID = BackendTools.GetShiftTypeWorkID(shiftTypeID);

            Assert.AreEqual(call.ShiftID, callShiftTypeID);
            Assert.AreEqual(call.TimeInShift, dt);
        }
        
        #endregion
    }
}
