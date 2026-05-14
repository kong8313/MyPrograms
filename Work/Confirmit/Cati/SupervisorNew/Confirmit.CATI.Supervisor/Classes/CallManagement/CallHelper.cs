using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Filters;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Classes.CallManagement
{
    public class CallHelper
    {
        public const string HasAudioColumnName = "HasAudio";
        public const string CallStateColumnName = "CallStateText";
        public const string DialModeColumnName = "DialingMode";
        public const string ReviewStatusColumnName = "ReviewStatusText";

        public static readonly DateTime FusionDateNow = CallQueueService.DefaultTimeInShift;
        public static readonly DateTime FusionDateNever = CallQueueService.ExpirationDateNever;

        /// <summary>
        /// Dictionary for DateTime columns.
        /// key: database column name
        /// value: grid column key
        /// </summary>
        public static Dictionary<string, string> DateTimeColumnNames = new Dictionary<string, string>
                                                                       {
                                                                           {"Time", "TimeText"},
                                                                           {"ExpireTime", "ExpireTimeText"},
                                                                           {"LastCallTime", "LastCallTimeText"},
                                                                           {"ApptTime", "ApptTimeText"},
                                                                           {"ExpTime", "ExpTimeText"},
                                                                       };

        /// <summary>
        /// Returns a timezone by ID. For ID=0 and for Interviewer timezone mode local Fusion timezone returns.
        /// </summary>
        /// <param name="timezoneId">Timezone identifier.</param>
        /// <param name="mode">Interviewer or Respondent timezone mode.</param>
        /// <param name="localTimezoneId"></param>
        /// <exception cref="ArgumentException">Timezone ID has invalid value.</exception>
        public static int GetTimezoneIdOrDefault(int timezoneId, ShowTimeMode mode, int localTimezoneId)
        {
            return mode == ShowTimeMode.Interviewer || timezoneId == 0 ? localTimezoneId : timezoneId;
        }

        /// <summary>
        /// Checks if DateTime is equal to FusionDateNow or FusionDateNever
        /// </summary>
        private static bool IsFusionPredefinedDate(DateTime dt)
        {
            return dt.Date <= FusionDateNow.Date || dt.Date == FusionDateNever.Date;
        }

        /// <summary>
        /// Returns text replacing fusion predefind date for specific column name.
        /// </summary>
        private static string GetTextForPredefinedDate(DateTime dt, string colName)
        {
            string result;

            switch (colName)
            {
                case "Time":
                    result = Strings.Now;
                    break;
                case "ExpireTime":
                    result = Strings.Never;
                    break;
                case "LastCallTime":
                    result = String.Empty;
                    break;
                case "ApptTime":
                    result = String.Empty;
                    break;
                case "ExpTime":
                    result = Strings.Never;
                    break;
                default:
                    result = dt.ToString();
                    break;
            }

            return result;
        }

        /// <summary>
        /// Returns page of calls data according given filter. If filter is null,
        /// default filter will be taken.
        /// It is added special column 'InterviewCallID'. Needs for selection depending of CallState.
        /// Times of each call convertes to local timezone or call's timezone, depending on timeMode value.
        /// </summary>
        /// <param name="surveyId">Survey identifier.</param>
        /// <param name="filterId">Filter identifier.</param>
        /// <param name="timezoneId"></param>
        /// <param name="callState">Call state.</param>
        /// <param name="pagingArgs">Paging arguments.</param>
        /// <param name="totalCount">Returns total count of calls.</param>
        /// <param name="timeMode">Selected time zone mode (Interviewer or Respondent).</param>
        /// <param name="retrieveAudio"> </param>
        /// <param name="confirmitVariables">List of confirmit variables names which values we
        /// want to add to resulting data.</param>
        /// <returns>Data table with calls.</returns>
        /// <exception cref="ArgumentException">Survey or filter identifier is invalid.</exception>
        public static DataTable GetCallsPage(
            int surveyId, 
            int? filterId, 
            int timezoneId, 
            CallStates callState, 
            PagingArgs pagingArgs, 
            out int totalCount, 
            ShowTimeMode timeMode, 
            bool retrieveAudio, 
            params string[] confirmitVariables)
        {
            return GetCallsRange(
                surveyId,
                filterId,
                timezoneId,
                callState,
                pagingArgs.StartElementIndex,
                pagingArgs.ElementsCount,
                pagingArgs.SortField,
                pagingArgs.SortOrderAsc,
                pagingArgs.SearchParameters,
                out totalCount,
                timeMode,
                retrieveAudio,
                confirmitVariables);
        }

        /// <summary>
        /// Performs processing of call list.
        /// </summary>
        private static void CallListPostProduction(
            DataTable list, CallStates callState, ShowTimeMode timeMode, int surveySid, int localTimezoneId, bool retrieveAudio)
        {
            // Makes multiple operations on DataTable faster
            list.BeginLoadData();
            foreach (DataRow row in list.Rows)
            {
                row.BeginEdit();
            }

            AddServiceColumns(list);
            ToggleAndReplaceTime(list, timeMode, localTimezoneId);
            ReplaceShiftType(list);

            if (retrieveAudio)
            {
                ServiceLocator.Resolve<IAddHasAudioColumnToCallList>().Add(list, surveySid);
            }

            AddCallStateColumn(callState, list);
            AddReviewStatusColumn(callState, list);

            EnsureAllColumns(list);

            HidePiiData(list);

            list.AcceptChanges();
            list.EndLoadData();
        }

        /// <summary>
        /// Hide TelephoneNumber and RespondentName values if needed
        /// </summary>
        private static void HidePiiData(DataTable list)
        {
            if (!ServiceLocator.Resolve<ICallCenterService>().IsNeedToHidePii())
            {
                return;
            }

            HideDataInColumn(list, "TelephoneNumber");
            HideDataInColumn(list, "RespondentName");
        }

        private static void HideDataInColumn(DataTable list, string columnName)
        {
            if (!list.Columns.Contains(columnName))
            {
                return;
            }

            list.Columns[columnName].ReadOnly = false;
            if (list.Columns[columnName].MaxLength < 3)
            {
                list.Columns[columnName].MaxLength = 3;
            }

            foreach (DataRow row in list.Rows)
            {
                row[columnName] = "***";
            }
        }

        private static void EnsureAllColumns(DataTable list)
        {
            if (!list.Columns.Contains("Priority"))
            {
                var dc = new DataColumn("Priority");
                dc.DataType = typeof (int);
                list.Columns.Add(dc);
            }
        }

        private static void AddCallStateColumn(CallStates callState, DataTable list)
        {
            list.Columns.Add(new DataColumn(CallStateColumnName, typeof(string)));

            if (callState == CallStates.Scheduled)
            {
                for (int i = 0; i < list.Rows.Count; i++)
                {
                    var phase = list.Rows[i]["CallState"];
                    if (!(phase is DBNull))
                    {
                        var state = "";

                        switch ((int)phase)
                        {
                            case (int)CallState.DisabledByFCD:
                                state = Strings.DisabledByFCDStateString;
                                break;
                            case (int)CallState.DisabledByUser:
                                state = Strings.DisabledByUserStateString;
                                break;
                        }

                        list.Rows[i][CallStateColumnName] = state ;
                    }
                }
            }
        }

        private static void AddReviewStatusColumn(CallStates callState, DataTable list)
        {
            list.Columns.Add(new DataColumn(ReviewStatusColumnName, typeof(string)));

            if (callState == CallStates.All)
            {
                for (var i = 0; i < list.Rows.Count; i++)
                {
                    var reviewStatus = list.Rows[i]["ReviewStatus"];
                    if (!(reviewStatus is DBNull))
                    {
                        var status = "";
                        switch ((byte)reviewStatus)
                        {
                            case (byte)ReviewStatus.SentToReview:
                                status = Strings.SentToReview;
                                break;
                            case (byte)ReviewStatus.ReviewStarted:
                                status = Strings.SessionReviewStarted;
                                break;
                            case (byte)ReviewStatus.ReviewCompleted:
                                status = Strings.SessionReviewCompleted;
                                break;
                            case (byte)ReviewStatus.NotSentToReview:
                                status = Strings.NotSentToReview;
                                break;
                        }

                        list.Rows[i][ReviewStatusColumnName] = status;
                    }
                }
            }
        }

        /// <summary>
        /// Returns range of calls data according given filter. If filter is null,
        /// default filter will be taken.
        /// It is added special column 'InterviewCallID'. Needs for selection depending of CallState.
        /// Times of each call convertes to local timezone or call's timezone, depending on timeMode value.
        /// </summary>
        /// <param name="surveyId">Survey identifier.</param>
        /// <param name="filterId">Filter identifier.</param>
        /// <param name="timezoneId"></param>
        /// <param name="callState">Call state.</param>
        /// <param name="startIndex">Start index of range (1 based index).</param>
        /// <param name="objectsCount">Number of records in range.</param>
        /// <param name="sortExpression">Sorting expression.</param>
        /// <param name="sortOrderAsc">Sorting order.</param>
        /// <param name="searchParams">Search parameters.</param>
        /// <param name="totalCount">Returns total count of calls.</param>
        /// <param name="timeMode">Selected time zone mode (Interviewer or Respondent).</param>
        /// <param name="retrieveAudio"> </param>
        /// <param name="confirmitVariables">List of confirmit variables names which values we
        /// want to add to resulting data.</param>
        /// <returns>Data table with calls.</returns>
        /// <exception cref="ArgumentException">Survey or filter identifier is invalid.</exception>
        public static DataTable GetCallsRange(
            int surveyId,
            int? filterId,
            int timezoneId, 
            CallStates callState,
            int startIndex,
            int objectsCount,
            string sortExpression,
            bool sortOrderAsc,
            SearchParameterCollection searchParams,
            out int totalCount,
            ShowTimeMode timeMode,
            bool retrieveAudio,
            params string[] confirmitVariables)
        {
            string sortExp = GetColumnKey(sortExpression);
            DataTable result;

            using (var transaction = new DatabaseTransactionScope("Supervisor.GetCallsPage", DeadlockPriority.Supervisor))
            {
                using (FilterHelper helper = new FilterHelper(filterId ?? 0, timezoneId, searchParams))
                {
                    result = CallManager.GetCallsRange(
                        surveyId,
                        helper.FilterID == 0 ? (int?) null : helper.FilterID,
                        callState,
                        new RangingArgs( startIndex, objectsCount, sortExp, sortOrderAsc),
                        out totalCount,
                        confirmitVariables);
                }

                transaction.Commit();
            }

            CallListPostProduction(result, callState, timeMode, surveyId, timezoneId, retrieveAudio);

            return result;
        }

        /// <summary>
        /// Gets database column name match grid column key.
        /// </summary>
        /// <param name="value">Value to find in DateTimeColumnNames</param>
        /// <returns>If there is value in DateTimeColumnNames, returns the first key in dictionary that match specified value; otherwise specified value</returns>
        private static string GetColumnKey(string value)
        {
            string key = value;
            if (DateTimeColumnNames.ContainsValue(value))
            {
                foreach (string dictKey in DateTimeColumnNames.Keys)
                    if (DateTimeColumnNames[dictKey] == value)
                        return dictKey;
            }
            return key;
        }

        /// <summary>
        /// Changes Time, ExpireTime, LastCallTime and ExpTime fields of each call in DataTable
        /// according to its TimezoneID value and replaces predifined dates with string values.
        /// </summary>
        /// <param name="dt">DataTable to made changes in</param>
        /// <param name="mode">ShowTimeMode to use</param>
        /// <param name="localTimezoneId"></param>
        /// <exception cref="ArgumentException">Timezone ID has invalid value.</exception>
        public static void ToggleAndReplaceTime(DataTable dt, ShowTimeMode mode, int localTimezoneId)
        {
            foreach (string key in DateTimeColumnNames.Keys)
            {
                var dc = new DataColumn(DateTimeColumnNames[key], typeof(String));
                dt.Columns.Add(dc);
                var dc2 = new DataColumn(key + "ExportColumn", typeof(object));
                dt.Columns.Add(dc2);
            }

            foreach (DataRow row in dt.Rows)
            {
                int tzID = ParseTimezoneId(row["TimezoneID"]);

                int timezoneId = GetTimezoneIdOrDefault(tzID, mode, localTimezoneId);

                foreach (string key in DateTimeColumnNames.Keys)
                {
                    row[DateTimeColumnNames[key]] = ParseDateTime(row, key, timezoneId, out object val);
                    row[key + "ExportColumn"] = val;
                }
            }
        }

        /// <summary>
        /// Replacing empty cells for "None" and "Any" shift types with its string values.
        /// </summary>
        private static void ReplaceShiftType(DataTable dt)
        {
            // when we fill dt from reader, column "ShiftType" is set read only.
            // in such case we should make it read-write manualy
            var column = dt.Columns["ShiftType"];
            column.ReadOnly = false;
            // aslo we should extend MaxLength
            column.MaxLength = column.MaxLength < 100 ? 100 : column.MaxLength;

            foreach (DataRow row in dt.Rows)
            {
                int shiftId = (int)row["Shift_ID"];
                if (shiftId == (int)CallShiftType.None)
                    row["ShiftType"] = Strings.ShiftTypeNoneString;
                else if (shiftId <= 0)
                    row["ShiftType"] = Strings.ShiftTypeAnyString;
            }
        }

        /// <summary>
        /// Add service columns to call list.
        /// </summary>
        /// <param name="list">Call list.</param>
        private static void AddServiceColumns(DataTable list)
        {
            DataColumn dc = new DataColumn("InterviewCallID");
            dc.DataType = typeof(String);
            list.Columns.Add(dc);

            foreach (DataRow row in list.Rows)
            {
                object callID = row["CallID"] != DBNull.Value ? row["CallID"] : 0;

                row["InterviewCallID"] = string.Format("{0}_{1}", row["InterviewID"], callID);
            }
        }

        /// <summary>
        /// Converting DateTime values accroding to selected timezone and replace it with text if needed.
        /// </summary>
        private static string ParseDateTime(DataRow row, string columnName, int timezoneId, out object val)
        {
            DateTime dateTimeValue;

            if (row[columnName] == DBNull.Value)
            {
                switch (columnName)
                {
                    case "ExpireTime":
                        val = Strings.Never;
                        break;
                    case "Time":
                        val = row["CallID"] != DBNull.Value ? Strings.Now : string.Empty;
                        break;
                    default:
                        val = string.Empty;
                        break;
                }
            }
            else
            {
                if (DateTime.TryParse(row[columnName].ToString(), out dateTimeValue))
                {
                    if (IsFusionPredefinedDate(dateTimeValue))
                    {
                        val = GetTextForPredefinedDate(dateTimeValue, columnName);
                    }
                    else
                    {
                        val = TimezoneManager.ConvertToTzLocalTime(timezoneId, dateTimeValue);
                    }
                }
                else
                {
                    val = row[columnName].ToString();
                }
            }

            return val.ToString();
        }

        /// <summary>
        /// Converts timezone identifier to int.
        /// </summary>
        /// <param name="timezoneId">Timezone identifier to convert.</param>
        /// <exception cref="ArgumentException">Timezone ID has invalid value.</exception>
        /// <returns>Parsed timezone identifier</returns>
        private static int ParseTimezoneId(object timezoneId)
        {
            int tzId = 0;
            if (timezoneId != null &&
                timezoneId != DBNull.Value &&
                (!Int32.TryParse(timezoneId.ToString(), out tzId) || tzId < 0))
            {
                throw new ArgumentException(
                    String.Format(
                        Strings.InvalidCallPropertyValueExceptionMessage,
                        "TimezoneID",
                        timezoneId
                    ),
                    "timezoneId"
                );
            }
            return tzId;
        }
    }
}
