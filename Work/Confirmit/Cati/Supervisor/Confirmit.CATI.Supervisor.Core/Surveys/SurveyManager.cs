using System;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using System.Collections.Generic;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using ConfirmitDialerInterface;

using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;

namespace Confirmit.CATI.Supervisor.Core.Surveys
{
    public class SurveyManager
    {

        //---------------------------------------------------------------------------
        private SurveyManager()
        {
        }

        /// <summary>
        /// Returns surveys with specified parent group allowed for specified user
        /// </summary>
        /// <param name="userName">Current user name</param>
        /// <param name="filter">Filter string</param>
        /// <returns></returns>
        public static List<SurveyInfo> GetSurveyList(string userName, string filter)
        {
            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            filter = SearchManager.FormatLikeValueForSql(filter);

            return (from c in BvSpSurvey_GetListByFolderAdapter.ExecuteEntityList(
                        userName,
                        filter,
                        callCenterId)
                    select new SurveyInfo(
                        c.SID.Value,
                        c.Name,
                        c.ConfirmitID,
                        c.TotalAssignedPersons.Value
                    )
                ).ToList();
        }

        /// <summary>
        /// Returns all surveys list.
        /// </summary>
        /// <param name="userName">Supervisor name</param>
        /// <param name="filter">Survey name filter.</param>
        public static List<SurveyInfoItem> GetSurveys(string userName, string filter)
        {
            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            filter = SearchManager.FormatLikeValueForSql(filter);

            return (from c in BvSpGetSurveysAdapter.ExecuteEntityList(
                        String.IsNullOrEmpty(filter) ? null : filter,
                        String.IsNullOrEmpty(userName) ? null : userName,
                        callCenterId)
                    select new SurveyInfoItem(
                        c.SID.Value,
                        c.Name,
                        c.ConfirmitID
                    )
                ).ToList();
        }

        public static List<SurveyInfoItem> GetRecentSurveys(string userName, string filter)
        {
            var userSurveyListRepository = ServiceLocator.Resolve<IUserSurveyListRepository>();

            return userSurveyListRepository.GetList(UserSurveyListType.Recent).Select((c,i) =>
                   new SurveyInfoItem(
                    c.SID.Value,
                    c.Name,
                    c.ProjectId,
                    0,
                    i
                )).ToList();
        }

        public static List<SurveyInfoItem> GetRecentSurveysDescending(string userName, string filter)
        {
            var userSurveyListRepository = ServiceLocator.Resolve<IUserSurveyListRepository>();

            return userSurveyListRepository.GetList(UserSurveyListType.Recent).Select((c, i) =>
                new SurveyInfoItem(
                    c.SID.Value,
                    c.Name,
                    c.ProjectId,
                    0,
                    int.MaxValue - i
                )).ToList();
        }

        public static List<SurveyInfoItem> GetOpenSurveys(string userName, string filter)
        {
            var surveys = GetSurveys(userName, String.Empty);

            var openedSurveyIDs = SurveyService.OpenedSurveys.Select(x => x.SID);

            return surveys.Where(x => openedSurveyIDs.Contains(x.Id)).ToList();
        }

        /// <summary>
        /// Gets shift types for current survey without exclusions and empty shift types.
        /// <param name="surveySID">Fusion survey SID.</param>
        /// </summary>
        public static List<ShiftType> GetShiftTypes(int surveySID)
        {
            if (surveySID <= 0)
                throw new ArgumentOutOfRangeException("surveySID");

            List<ShiftType> shiftTypes = new List<ShiftType>();

            int scheduleID = SurveyRepository.GetById(surveySID).ScheduleID;

            List<int> nonEmptyShiftTypeObjectIds = GetNonEmptyShiftTypeObjectIds(scheduleID);
            
            var scheduleService = ServiceLocator.Resolve<IScheduleService>();
            List<BvSpShiftType_ListEntity> shiftTypesList = scheduleService.GetShiftTypeList(scheduleID);

            foreach (BvSpShiftType_ListEntity shiftType in shiftTypesList)
            {
                int shiftId = shiftType.ID.Value;
                string shiftName = shiftType.Name;
                int objectId = shiftType.ObjectID.Value;

                // Don't include exclusions and empty shift types.
                if (shiftId != Int32.MaxValue && nonEmptyShiftTypeObjectIds.Contains(objectId))
                {
                    shiftTypes.Add(new ShiftType(shiftId, shiftName, objectId));
                }
            }

            return shiftTypes;
        }

        /// <summary>
        /// Gets the list of object IDs of non empty shift types (that has shifts) in current schedule.
        /// </summary>
        public static List<int> GetNonEmptyShiftTypeObjectIds(int scheduleID)
        {
            List<int> nonEmptyShiftTypeIds = new List<int>();
            
            var scheduleService = ServiceLocator.Resolve<IScheduleService>();
            List<BvSpShift_ListEntity> list = scheduleService.GetShiftList(scheduleID, 0, 0);

            foreach (BvSpShift_ListEntity shift in list)
            {
                int shiftTypeId = shift.ShiftTypeID.Value;
                nonEmptyShiftTypeIds.Add(shiftTypeId);
            }

            return nonEmptyShiftTypeIds.Distinct().ToList();
        }

        /// <summary>
        /// Looks up for survey with specified name.
        /// </summary>
        /// <param name="name">Name to look up.</param>
        /// <returns>Survey ID if survey found; 0 otherwise.</returns>
        public static int LookupSurveyName(string name)
        {
            BvSurveyEntity survey = SurveyRepository.GetByName(name);
            if (survey != null)
                return survey.SID;
            return 0;
        }

        /// <summary>
        /// Gets the Confirmit project ID by Fusion survey SID.
        /// </summary>
        /// <param name="surveySid">The survey sid.</param>
        /// <returns>Confirmit project ID</returns>
        public static string GetProjectID(int surveySid)
        {
            return SurveyRepository.GetById(surveySid).Name;
        }

        /// <summary>
        /// Gets the survey dialing mode by survey SID.
        /// </summary>
        /// <param name="surveySid">The survey SID.</param>
        /// <returns>Survey dialing mode.</returns>
        public static DialingMode GetDialingMode(int surveySid)
        {
            return SurveyService.GetDialingMode(surveySid);
        }

        /// <summary>
        /// Formats survey name according following template:
        /// 'survey name (p0000000)'.
        /// </summary>
        /// <param name="survey">Survey entity.</param>
        /// <returns>Expanded survey name.</returns>
        public static string FormatSurveyName(BvSurveyEntity survey)
        {
            if (survey == null)
            {
                throw new ArgumentNullException("survey");
            }

            return (String.IsNullOrEmpty(survey.Description) ?
                survey.Name:
                String.Format("{0} ({1})", survey.Description, survey.Name)
            );
        }
    }
}