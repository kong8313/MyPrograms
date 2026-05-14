using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.FilterServiceImplementation;
using Confirmit.CATI.Core.Services.Survey;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    public enum SelectMode
    {
        All,
        CustomFilter,
        Selected
    }

    public enum ShiftTypeIDs
    {
        Default = 1,
        Sunday = 2
    }
        
    public enum SchedulingScriptType
    {
        Default,
        ScriptForShiftType
    }

    public class FusionLibTestTools
    {
        private readonly BackendTools _backendTools;

        public FusionLibTestTools(BackendTools backendTools)
        {
            _backendTools = backendTools;
        }

        public string CreateSurveyWithPersonForTest(SchedulingScriptType scriptMode,
            out int surveySid,
            out int personSid,
            int manualSelection = 0)
        {
            return CreateSurveyWithPersonForTest(scriptMode, null, out surveySid, out personSid, manualSelection);
        }

        public string CreateSurveyWithPersonForTest(SchedulingScriptType scriptMode,
            string cfSqlServerConnectionString,
            out int surveySid,
            out int personSid,
            int manualSelection = 0)
        {
            string projectName = BackendTools.GenerateSurveyName();

            if (scriptMode == SchedulingScriptType.ScriptForShiftType)
            {
                var script = new TestScript(
                new Action(Action.Operation.SetNewITS, "17"),
                new object[]{new Shift(1, (int)ShiftTypeIDs.Sunday, "0.00:00:00", "1.00:00:00"),
                      new Shift(2, (int)ShiftTypeIDs.Default, "1.00:00:00", "0.00:00:00")});

                surveySid = _backendTools.CreateSurvey(script, projectName, cfSqlServerConnectionString);
            }
            else
            {
                surveySid = _backendTools.CreateSurvey(projectName, cfSqlServerConnectionString);

                _backendTools.LaunchAllHoursScript();
            }

            var _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            _surveyStateService.Open(surveySid);

            var person = new BvPersonEntity
            {
                Name = "interviewer" + surveySid.ToString(CultureInfo.InvariantCulture),
                Description = "interviewer1 description",
                ManualSelection = manualSelection,
                CallCenterID = CallCenterTools.DefaultId
            };

            var personRepository = ServiceLocator.Resolve<IPersonRepository>();
            personSid = personRepository.Insert(person);
            BackendTools.AssignCatiPersonToSurvey(surveySid, personSid);

            return projectName;
        }

        /// <summary>
        /// For amenity interviewID = ITS
        /// </summary>
        /// <param name="surveySid"></param>
        /// <param name="interviewIds"></param>
        public static IEnumerable<BvInterviewEntity> CreateInterviewsForTest(
            int surveySid,
            IEnumerable<int> interviewIds,
            DialType dialType = DialType.Landline)
        {
            return CreateInterviewsForTestWithTelephoneNumbers(surveySid, interviewIds, false, dialType);
        }

        public static IEnumerable<BvInterviewEntity> CreateInterviewsForTestWithTelephoneNumbers(
            int surveySid,
            IEnumerable<int> interviewIds,
            bool fillTelephoneNumbers,
            DialType dialType = DialType.Landline)
        {
            var interviewList = new List<BvInterviewEntity>();

            foreach (var interviewId in interviewIds)
            {
                var interview = new BvInterviewEntity
                {
                    ID = interviewId,
                    SurveySID = surveySid,
                    TransientState = interviewId % 120 + 1,
                    TelephoneNumber = fillTelephoneNumbers ? interviewId.ToString(CultureInfo.InvariantCulture) : null,
                    DialTypeId = (byte)dialType
                };

                interviewList.Add(interview);

                BackendTools.CreateInterview(interview);
            }

            return interviewList;
        }

        public static IEnumerable<BvCallEntity> CreateCallsForTest(IEnumerable<BvInterviewEntity> interviews, short? priority)
        {
            var calls = new List<BvCallEntity>(interviews.Select(x => new BvCallEntity
            {
                InterviewID = x.ID,
                SurveySID = x.SurveySID,
                CallState = 2,
                ShiftID = (int)CallShiftType.None,
                Priority = priority ?? (short)x.ID,
                ResourceType = 1,
                Resource = x.SurveySID,
                DialTypeId = x.DialTypeId
            }));

            calls.ForEach(x =>
            {
                CallQueueService.AddCall(x, 0, 0);
                x.CallID = CallQueueService.GetCallAndNoLock(x.SurveySID, x.InterviewID).CallID;
            });

            return calls;
        }

        public static IEnumerable<BvCallEntity> CreateCallsForTest(IEnumerable<BvInterviewEntity> interviews)
        {
            return CreateCallsForTest(interviews, null);
        }

        /// <summary>
        /// Filter by any interview column with integer type
        /// </summary>
        /// <returns></returns>
        public static int CreateFilterForTest(string column,
            FilterOperator filterOperator,
            string value)
        {
            var filter = new BvFiltersEntity { Name = ("test filter") };

            int filterID = FilterRepository.Insert(filter);
            var filterFields = new List<BvFilterFieldsEntity>
            {
                new BvFilterFieldsEntity
                {
                    Table = (int)TableTypes.Interview,
                    Column = column,
                    Type = (int)VariableTypes.Integer,
                    Sign = (int)filterOperator,
                    Value = value
                }
            };

            FilterService.SetFields(filterID, filterFields);

            return filterID;
        }

        public static void UpdateStatePriorityOfNewIts(int surveySid, int its, int itsPriority)
        {
            var stateEntity = StateRepository.GetByItsAndStateGroupId(
                its,
                SurveyRepository.GetById(surveySid).StateGroupID);
            stateEntity.Priority = itsPriority;

            StateRepository.Update(stateEntity);
        }
    }
}
