using System;
using System.Data;
using System.Linq;
using BvDotNetScript.SurveyDataApiWS.Util;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.FilterServiceImplementation;
using Confirmit.CATI.Core.SurveyDataService;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Exceptions;
using Confirmit.CATI.Supervisor.Core.Properties;
using Confirmit.CATI.Supervisor.Core.Resources;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Supervisor.Core.Surveys
{
    /// <summary>
    /// Class responsible for common operations with calls (samples)
    /// </summary>
    public static class CallManager
    {
        private const string ConfirmitInterviewIdColumnName = "respid";
        private const string ResponseIdColumnName = "responseid";

        /// <summary>
        /// Returns range of calls data according given filter. If filter is null,
        /// default filter will be taken.
        /// </summary>
        /// <param name="surveyId">Survey identifier.</param>
        /// <param name="filterId">Filter identifier.</param>
        /// <param name="callState">Call state.</param>
        /// <param name="rangingArgs">Requested range parameters.</param>
        /// <param name="totalCount">Returns total count of calls.</param>
        /// <param name="confirmitVariables">List of confirmit variables names which values we
        /// want to add to resulting data.</param>
        /// <returns>Data table with calls.</returns>
        /// <exception cref="ArgumentException">Survey or filter identifier is invalid.</exception>
        public static DataTable GetCallsRange(
            int surveyId,
            int? filterId,
            CallStates callState,
            RangingArgs rangingArgs, 
            out int totalCount,
            params string[] confirmitVariables)
        {
            if (surveyId == 0)
            {
                throw new ArgumentException(
                    String.Format(Strings.InvalidIdentifierExceptionMessage, "Survey", surveyId),
                    "surveyId"
                );
            }

            string query = GenerateSelectInterviewQuery(
                surveyId,
                filterId,
                callState,
                confirmitVariables,
                rangingArgs,
                out var countQuery);

            var databaseEngine = new DatabaseEngine();
            var result = databaseEngine.ExecuteDataTable<DataTable>(query, CommandType.Text);
            totalCount = databaseEngine.ExecuteScalar<int>(countQuery, CommandType.Text);

            return result;
        }

        /// <summary>
        /// Gets the confirmit variable alias. 
        /// Alias is used instead of name to avoid problems when variable name is the same as other column's key.
        /// </summary>
        /// <param name="variableName">Name of the variable.</param>
        public static string GetConfirmitVariableAlias(string variableName)
        {
            return ConfirmitVariablesHelper.GetConfirmitVariableAlias(variableName);
        }

        /// <summary>
        /// Generates select SQL query for retrieving interview data from Fusion database.
        /// </summary>
        /// <param name="surveyId">Survey identifier.</param>
        /// <param name="filterId">Filter identifier. It could be null. If it is null default 
        /// filter is used.</param>
        /// <param name="callState">Call state.</param>
        /// <param name="selectConfirmitVariables">List of QSL variables names which values we
        /// want to add to resulting data.</param>
        /// <param name="rangingArgs"></param>
        /// <param name="counterQuery"></param>
        /// <returns>SQL query.</returns>        
        private static string GenerateSelectInterviewQuery(
            int surveyId,
            int? filterId,
            CallStates callState,
            string[] selectConfirmitVariables,
            RangingArgs rangingArgs,
            out string counterQuery)
        {
            if (filterId.HasValue && filterId.Value == 0)
            {
                throw new ArgumentException(
                    String.Format(Strings.InvalidIdentifierExceptionMessage, "Filter", filterId.Value),
                    "filterId"
                );
            }

            var replicatedColumns = selectConfirmitVariables.Select( x => new ReplicatedColumn(x, GetConfirmitVariableAlias(x))).ToArray();

            var filterService = ServiceLocator.Resolve<IFilterService>();
            var sqlFilterProvider = ServiceLocator.Resolve<ISqlFilterProvider>();

            var query = filterService.GenerateSqlWithSelect(
                        sqlFilterProvider.TryToGetFilter(filterId, surveyId),
                        surveyId,
                        GetModeForSelect(callState),
                        rangingArgs,
                        replicatedColumns,
                        out counterQuery);

            return query;
        }

        /// <summary>
        /// Gets Fusion mode for call list function.
        /// </summary>
        /// <param name="callState">Call state.</param>
        /// <returns>Fusion mode for call selection.</returns>
        private static FilterGenerateMode GetModeForSelect(CallStates callState)
        {
            switch (callState)
            {
                case CallStates.Scheduled:
                    return FilterGenerateMode.ScheduledInterviews; // scheduled calls
                case CallStates.Suspended:
                    return FilterGenerateMode.SuspendedInterviews; // suspended calls
                case CallStates.All:
                    return FilterGenerateMode.AllInterviews; // all interviews
                case CallStates.HighPriority:
                    return FilterGenerateMode.HighPriorityInterviews;
                case CallStates.SentToDialer:
                    return FilterGenerateMode.SentToDialerInterviews;
                case CallStates.CallsAvailableNow:
                    return FilterGenerateMode.CallsAvailableNow;
				default:
                    throw new ArgumentException(
                        String.Format(Strings.InvalidCallStateExceptionMessage, callState),
                        "callState"
                    );
            }
        }

        private static string GetTitleForAsyncOperation(string text, int surveySid)
        {
            ISurveyRepository surveyRepository = ServiceLocator.Resolve<ISurveyRepository>();
            var survey = surveyRepository.GetById(surveySid);

            return $"{text}\"{survey.Name}\" ({survey.Description})";
        }

        public static BvAsyncOperationQueueEntity ActivateCalls(
            int surveyId,
            int priority,
            CallStates callState,
            int[] resourceIds,
            int shiftTypeId,
            int? its,
            DateTime? timeToCall,
            bool enableDisabledCalls,
            BatchParameters batchParameters)
        {
            var parameters = new CATI.Core.AsyncOperations.Operations.CallsManagementOperations.ActivateCalls.Parameters
            {
                SurveyId = surveyId,
                BatchParameters = batchParameters,
                Priority = priority,
                CallState = callState,
                ResourceIds = resourceIds,
                ShiftTypeId = shiftTypeId,
                TimeToCall = timeToCall ?? DateTime.UtcNow,
                EnableDisabledCalls = enableDisabledCalls,
                ITS = its
            };

            string title;
            switch( batchParameters.Type )
            {
                case BatchType.Selected:
                    title = GetTitleForAsyncOperation("Activate selected calls", surveyId);
                    break;
                case BatchType.Filtered:
                    title = GetTitleForAsyncOperation("Activate filtered calls", surveyId);
                    break;
                case BatchType.FilteredByCells:
                    title = GetTitleForAsyncOperation("Activate filtered by quota cells calls", surveyId);
                    break;
                case BatchType.FilteredByMultipleCells:
                    title = GetTitleForAsyncOperation("Activate filtered by multiple quota cells calls", surveyId);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return StartAsyncOperation(parameters, title);
        }

        public static BvAsyncOperationQueueEntity EditCalls(
            int surveyId,
            DateTime? timeToCall,
            DateTime? timeToExpire,
            int? callState,
            int? callPriority,
            int? shiftType,
            int? extendedStatus,
            byte? dialingMode,
            BatchParameters batchParameters)
        {
            var parameters = new CATI.Core.AsyncOperations.Operations.CallsManagementOperations.EditCalls.Parameters
            {
                SurveyId = surveyId,
                BatchParameters = batchParameters,
                TimeToCall = timeToCall,
                TimeToExpire = timeToExpire,
                CallState = callState,
                CallPriority = callPriority,
                ShiftType = shiftType,
                ExtendedStatus = extendedStatus,
                DialingMode = dialingMode
            };

            string title;
            switch (batchParameters.Type)
            {
                case BatchType.Selected:
                    title = GetTitleForAsyncOperation("Edit selected calls", surveyId);
                    break;
                case BatchType.Filtered:
                    title = GetTitleForAsyncOperation("Edit filtered calls", surveyId);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return StartAsyncOperation(parameters, title);
        }

        public static BvAsyncOperationQueueEntity MoveCalls(
            int surveyId, 
            int itsId, 
            BatchParameters batchParameters)
        {
            if (surveyId == 0)
            {
                throw new ArgumentException(
                    String.Format(Strings.InvalidIdentifierExceptionMessage, "Survey", surveyId),
                    "surveyId"
                );
            }

            if (itsId == 0)
            {
                throw new ArgumentException(
                    String.Format(Strings.InvalidIdentifierExceptionMessage, "ITS", surveyId),
                    "itsId"
                );
            }

            var parameters = new CATI.Core.AsyncOperations.Operations.CallsManagementOperations.MoveCalls.Parameters
            {
                SurveyId = surveyId,
                BatchParameters = batchParameters,
                StateId = itsId,
            };

            string title;

            switch (batchParameters.Type)
            {
                case BatchType.Selected:
                    title = GetTitleForAsyncOperation("Move selected calls", surveyId);
                    break;
                case BatchType.Filtered:
                    title = GetTitleForAsyncOperation("Move filtered calls", surveyId);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return StartAsyncOperation(parameters, title);
        }

        /// <summary>
        /// Moves filtered calls to given ITS and reschedules them.
        /// </summary>
        /// <param name="surveyId">Survey identifier.</param>
        /// <param name="itsId">ITS identifier.</param>
        /// <param name="batchParameters">Id of calls to move.</param>
        /// <param name="appointment">Appointment data.</param>
        /// <exception cref="ArgumentException">Survey or ITS has wrong identifier.</exception>
        public static BvAsyncOperationQueueEntity MoveAndRescheduleCalls(int surveyId, int itsId, BatchParameters batchParameters, Appointment appointment = null)
        {
            if (surveyId == 0)
            {
                throw new ArgumentException(
                    String.Format(Strings.InvalidIdentifierExceptionMessage, "Survey", surveyId),
                    "surveyId"
                );
            }

            if (itsId == 0)
            {
                throw new ArgumentException(
                    String.Format(Strings.InvalidIdentifierExceptionMessage, "ITS", surveyId),
                    "itsId"
                );
            }

            var parameters = new CATI.Core.AsyncOperations.Operations.CallsManagementOperations.MoveAndRescheduleCalls.Parameters
            {
                SurveyId = surveyId,
                StateId = itsId,
                BatchParameters = batchParameters,
                AppointmentPrm = appointment
            };

            string title;

            switch (batchParameters.Type)
            {
                case BatchType.Selected:
                    title = GetTitleForAsyncOperation("Move and reschedule selected calls", surveyId);
                    break;
                case BatchType.Filtered:
                    title = GetTitleForAsyncOperation("Move and reschedule filtered calls", surveyId);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return StartAsyncOperation(parameters, title);
        }

        /// <summary>
        /// Changes shift type of given calls to given shift type.
        /// </summary>
        /// <param name="surveyId">Survey identifier.</param>
        /// <param name="shiftTypeId">Shift type identifier.
        /// This value should be positive number or -1.
        /// If "Any valid" is selected value should be -1. 
        /// If "None" is selected value should be 0.</param>
        /// <param name="batchParameters">Batch parameters of interview ids.</param>
        /// <param name="fromSchedulingScriptChange">True if changed caused by scheduling script change in the survey</param>
        /// <exception cref="ArgumentNullException">Call identifiers list
        /// is null or empty.</exception>
        /// <exception cref="ArgumentException">Survey or shift type has wrong identifier or call 
        /// state is invalid.</exception>
        public static BvAsyncOperationQueueEntity ChangeShiftTypeOfCalls(
            int surveyId,
            int shiftTypeId,
            BatchParameters batchParameters,
            bool fromSchedulingScriptChange = false
        )
        {
            if (surveyId == 0)
            {
                throw new ArgumentException(
                    String.Format(Strings.InvalidIdentifierExceptionMessage, "Survey", surveyId),
                    "surveyId"
                );
            }

            if (shiftTypeId <=0 &&
                shiftTypeId != (int)CallShiftType.AnyValid &&
                shiftTypeId != (int)CallShiftType.None )
            {
                throw new ArgumentException(
                    String.Format(Strings.InvalidIdentifierExceptionMessage, "shiftTypeId", shiftTypeId),
                    "shiftTypeId"
                );
            }

            string title;

            switch (batchParameters.Type)
            {
                case BatchType.Selected:
                    title = GetTitleForAsyncOperation("Change shift type of selected calls", surveyId);
                    break;
                case BatchType.Filtered:
                    title = GetTitleForAsyncOperation(!fromSchedulingScriptChange ? "Change shift type of filtered calls" : "Change shift type of appropriate calls caused by scheduling script change", surveyId);
                    break;
                default:
                    throw new NotImplementedException();
            }


            var parameters = new CATI.Core.AsyncOperations.Operations.CallsManagementOperations.ChangeShiftTypeOfCalls.Parameters
            {
                SurveyId = surveyId,
                ShiftTypeID = shiftTypeId,
                BatchParameters = batchParameters
            };

            return StartAsyncOperation(parameters, title);
        }

        
        /// <summary>
        /// Assigns selected calls to given survey.
        /// </summary>
        /// <param name="surveyId">Survey identifier.</param>
        /// <param name="resourceIds">Identifiers of person or person group. 
        /// This value couldn't be 0.</param>
        /// <param name="batchParameters">Batch parameters of interview ids.</param>
        /// <exception cref="ArgumentException">Survey, role, person or group identifier is invalid.</exception>
        /// <exception cref="ArgumentNullException">Call identifiers list is null or empty.</exception>
        public static BvAsyncOperationQueueEntity AssignCalls( int surveyId, int[] resourceIds, BatchParameters batchParameters )
        {
            if (surveyId == 0)
            {
                throw new ArgumentException(
                    String.Format(Strings.InvalidIdentifierExceptionMessage, "Survey", surveyId),
                    "surveyId"
                );
            }

            if (resourceIds.Length == 0 || resourceIds.Any(x => x == 0))
            {
                throw new ArgumentException(
                    String.Format(Strings.InvalidIdentifierExceptionMessage, "Person or group", surveyId),
                    "personOrGroupId"
                );
            }

            if (batchParameters == null)
            {
                throw new ArgumentNullException("batchParameters");
            }

            string title;

            switch (batchParameters.Type)
            {
                case BatchType.Selected:
                    title = GetTitleForAsyncOperation("Assign selected calls", surveyId);
                    break;
                case BatchType.Filtered:
                    title = GetTitleForAsyncOperation("Assign filtered calls", surveyId);
                    break;
                default:
                    throw new NotImplementedException();
            }


            var parameters = new CATI.Core.AsyncOperations.Operations.CallsManagementOperations.AssignCalls.Parameters
            {
                SurveyId = surveyId,
                ResourceIds = resourceIds,
                BatchParameters = batchParameters
            };

            return StartAsyncOperation(parameters, title);
        }

        public static BvAsyncOperationQueueEntity ChangeCallsPriority(
            int surveyId,
            int priority,
            BatchParameters batchParameters
        )
        {
            if (surveyId == 0)
            {
                throw new ArgumentException(
                    String.Format(Strings.InvalidIdentifierExceptionMessage, "Survey", surveyId),
                    "surveyId"
                );
            }

            if( batchParameters == null)
            {
                throw new ArgumentNullException("batchParameters");
            }

            string title;

            switch( batchParameters.Type)
            {
                case BatchType.Selected:
                    title = GetTitleForAsyncOperation("Change priority of selected calls", surveyId);
                    break;
                case BatchType.Filtered:
                    title = GetTitleForAsyncOperation("Change priority of filtered calls", surveyId);
                    break;
                case BatchType.FilteredByCells:
                    title = GetTitleForAsyncOperation("Change priority of filtered by cells calls", surveyId);
                    break;
                case BatchType.FilteredByMultipleCells:
                    title = GetTitleForAsyncOperation("Change priority of filtered by multiple cells calls", surveyId);
                    break;
                default:
                    throw new NotImplementedException();
            }
            

            var parameters = new CATI.Core.AsyncOperations.Operations.CallsManagementOperations.ChangePriorityOfCalls.Parameters
            {
                SurveyId = surveyId,
                Priority = priority,
                BatchParameters = batchParameters
            };

            return StartAsyncOperation(parameters, title);
        }

        /// <summary>
        /// Adds specified call to call list.
        /// Following Call properties should be initialized for given call:
        /// SurveyId
        /// InterviewId
        /// CallState
        /// RoleId
        /// Priority
        /// </summary>
        /// <param name="call">Call to add.</param>
        /// <exception cref="ArgumentNullException">Call object is null.</exception>
        /// <exception cref="ArgumentException">Call's properties SurveyId,
        /// InterviewId, CallState, RoleId, Priority are not initialized.</exception>
        public static void AddCall(BvCallEntity call)
        {
            var survey = SurveyRepository.GetById(call.SurveySID);

            var evt = new CreateCallEvent(call, survey.ProjectId);

            CheckCall(call);

            CallQueueService.AddCall(call, 0, 0);

            evt.Finish();
        }

        /// <summary>
        /// Updates specified call.
        /// Following Call properties should be initialized for given call:
        /// SurveyId
        /// InterviewId
        /// CallId
        /// CallState
        /// RoleId
        /// Priority
        /// </summary>
        /// <param name="call"> Call to update.</param>
        /// <exception cref="ArgumentNullException">Call object is null.</exception>
        /// <exception cref="ArgumentException">Call's properties SurveyId, CallId,
        /// InterviewId, CallState, RoleId, Priority are not initialized.</exception>
        public static void UpdateCall(BvCallEntity call)
        {
            var survey = SurveyRepository.GetById(call.SurveySID);
            var evt = new UpdateCallEvent(call, survey.ProjectId);

            CheckCall(call);

            if (call.CallID <= 0)
            {
                throw new ArgumentException(
                    String.Format(Strings.InvalidCallPropertyValueExceptionMessage, "CallId", call.CallID), "call.CallId");
            }

            CallQueueService.UpdateCall(call, 0);

            evt.Finish();
        }

        /// <summary>
        /// Checks call if it is valid. Call shouldn't be null and
        /// following Call properties should be initialized:
        /// SurveyId
        /// InterviewId
        /// CallState
        /// RoleId
        /// Priority.
        /// If call is invalid exceptions are thrown.
        /// </summary>
        /// <param name="call">Call to check.</param>
        /// <exception cref="ArgumentNullException">Call object is null.</exception>
        /// <exception cref="ArgumentException">Call's properties SurveyId,
        /// InterviewId, CallState, RoleId, Priority are not initialized.</exception>
        private static void CheckCall(BvCallEntity call)
        {
            if (call == null)
            {
                throw new ArgumentNullException("call", Strings.ItemNotInitializedExceptionMessage);
            }

            if (call.SurveySID <= 0)
            {
                throw new ArgumentException(
                    String.Format(Strings.InvalidCallPropertyValueExceptionMessage, "SurveyId", call.SurveySID), "call.SurveyId");
            }

            if (call.InterviewID < 0)
            {
                throw new ArgumentException(
                    String.Format(Strings.InvalidCallPropertyValueExceptionMessage, "InterviewId", call.InterviewID), "call.InterviewId");
            }

            if (call.CallState <= 0)
            {
                throw new ArgumentException(
                    String.Format(Strings.InvalidCallPropertyValueExceptionMessage, "CallState", call.CallState), "call.CallState");
            }

            if (call.Priority <= 0)
            {
                throw new ArgumentException(
                    String.Format(Strings.InvalidCallPropertyValueExceptionMessage, "Priority", call.Priority), "call.Priority");
            }

            if (!IsInterviewExists(call.SurveySID, call.InterviewID))
            {
                throw new InteviewNotExistsException(call.InterviewID);
            }
        }

        /// <summary>
        /// Deletes specified calls.
        /// </summary>
        /// <param name="surveyId">Id of surveyId.</param>
        /// <param name="batchParameters">Id of calls to delete.</param>
        public static BvAsyncOperationQueueEntity DeleteCalls(int surveyId, BatchParameters batchParameters)
        {
            if( surveyId == 0)
            {
                throw new ArgumentException("surveyId");
            }

            if( batchParameters == null)
            {
                throw new ArgumentNullException("batchParameters");    
            }

            var parameters = new CATI.Core.AsyncOperations.Operations.CallsManagementOperations.DeleteCalls.Parameters
            {
                SurveyId = surveyId,
                BatchParameters = batchParameters
            };

            string title;

            switch (batchParameters.Type)
            {
                case BatchType.Selected:
                    title = GetTitleForAsyncOperation("Deactivate selected calls", surveyId);
                    break;
                case BatchType.Filtered:
                    title = GetTitleForAsyncOperation("Deactivate filtered calls", surveyId);
                    break;
                default:
                    throw new NotImplementedException();
            }
            
            return StartAsyncOperation(parameters, title);
        }

        public static BvAsyncOperationQueueEntity EnableCalls(
            int surveyId, bool enableState, BatchParameters batchParameters)
        {
            var parameters = new CATI.Core.AsyncOperations.Operations.CallsManagementOperations.EnableCalls.Parameters
            {
                SurveyId = surveyId,
                EnablingState = enableState,
                BatchParameters = batchParameters
            };

            string title;
            switch( batchParameters.Type)
            {
                case BatchType.Selected:
                    title = GetTitleForAsyncOperation(enableState ? "Enable Selected Calls" : "Disable Selected Calls", surveyId);
                    break;
                case BatchType.FilteredByCells:
                    title = GetTitleForAsyncOperation(enableState ? "Enable filtered by quota cells calls" : "Disable filtered by quota cells calls", surveyId);
                    break;
                case BatchType.Filtered:
                    title = GetTitleForAsyncOperation(enableState ? "Enable filtered calls" : "Disable filtered calls", surveyId);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return StartAsyncOperation(parameters, title);
        }

        /// <summary>
        /// Gets the interview by survey ID and interview ID.
        /// </summary>
        /// <param name="surveyID">The survey ID.</param>
        /// <param name="interviewID">The interview ID.</param>
        /// <returns>The interview.</returns>
        public static BvInterviewEntity GetInterview(int surveyID, int interviewID)
        {
            if (surveyID <= 0)
                throw new ArgumentOutOfRangeException("surveyID");

            if (interviewID <= 0)
                throw new ArgumentOutOfRangeException("interviewID");

            return InterviewRepository.GetById(surveyID, interviewID);
        }

        /// <summary>
        /// Determines whether interview with the specified survey ID and interview ID exists.
        /// </summary>
        /// <param name="surveyID">The survey ID.</param>
        /// <param name="interviewID">The interview ID.</param>
        /// <returns>
        /// 	<c>true</c> if interview with the specified survey ID and interview ID exists; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInterviewExists(int surveyID, int interviewID)
        {
            try
            {
                var interview = GetInterview(surveyID, interviewID);
                if (interview == null)
                    return false;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Attach CF survey DB if needed. It currently should be done to make Fusion work properly 
        /// because it uses direct access to CF databases.
        /// </summary>
        public static void AttachSurveyDbBySurveyId(int surveyId)
        {
            try
            {
                string projectId = SurveyManager.GetProjectID(surveyId);
                AttachSurveyDb(projectId);
            }
            catch (SurveyNotFoundException ex)
            {
                TraceHelper.TraceException(ex);
            }
        }

        /// <summary>
        /// Attach CF survey DB if needed. It currently should be done to make Fusion work properly 
        /// because it uses direct access to CF databases.
        /// </summary>
        public static void AttachSurveyDb(string projectId)
        {
            try
            {
                var authoringService = ServiceLocator.Resolve<IAuthoringService>();
                authoringService.GetDBVersion(projectId);
            }
            catch
            {
                // Not critical, logged at a lower level.
            }            
        }

        public static DataTable GetInterviewCallHistoryInfo(string projectId, int interviewId)
        {
            TransferDef transferDef = SurveyDataUtil.NewTransferDef(projectId, false, DatabaseType.Production);
            TransferLevel transferLevel = SurveyDataUtil.NewTransferLevel(Settings.Default.CallHistoryLoopId, true);

            transferDef.Levels = (TransferLevel[])SurveyDataUtil.Add(
                transferDef.Levels, transferLevel, typeof(TransferLevel));

            transferLevel.Where = SurveyDataUtil.NewWhereClause(
                SurveyDataUtil.NewBinaryComparison(
                    ComparisonType.Equal,
                    SurveyDataUtil.NewQueryField(ConfirmitInterviewIdColumnName),
                    SurveyDataUtil.NewQueryConstant(ConfirmitDbType.Integer, interviewId)));

            var surveyDataService = ServiceLocator.Resolve<ISurveyDataService>();
            var result = surveyDataService.GetData(transferDef, null).Result.Tables[0];

            if (result.Columns.Count > 0)
            {
                if (result.Columns.Contains(ResponseIdColumnName))
                {
                    result.Columns.Remove(ResponseIdColumnName);
                }

                if (result.Columns.Contains(Settings.Default.CallHistoryLoopId))
                {
                    result.Columns[Settings.Default.CallHistoryLoopId].SetOrdinal(0);
                    
                    if (result.Rows.Count > 0)
                    {
                        /* loop id column type is string, but it contains integer call attempts and they
                         * are sorted in alphabetical order. So we should re-order resulting table by
                         * loop id column using integer numbers ordering */

                        result =
                            result.AsEnumerable().OrderBy(
                                row => Int32.Parse(row[Settings.Default.CallHistoryLoopId].ToString()))
                                .CopyToDataTable();
                    }
                }
            }

            return result;
        }
        
        public static BvAsyncOperationQueueEntity ChangeDialModeOfInterviews(
            int surveyId,
            DialingMode? dialingMode,
            BatchParameters batchParameters)
        {
            var surveyDialMode = SurveyRepository.GetById(surveyId).DialMode;
            if (surveyDialMode != (int)DialingMode.Predictive && surveyDialMode != (int)DialingMode.Automatic)
            {
                throw new ArgumentException(
                    string.Format(Strings.InvalidSurveyDialModeExceptionMessage, surveyId, surveyDialMode), "surveyId");
            }

            var parameters = new CATI.Core.AsyncOperations.Operations.CallsManagementOperations.ChangeDialModeOfInterviews.Parameters
            {
                SurveyId = surveyId,
                BatchParameters =batchParameters,
                DialingMode = dialingMode,                                
            };

            string title;

            switch (batchParameters.Type)
            {
                case BatchType.Selected:
                    title = GetTitleForAsyncOperation("Change dial mode of selected interviews", surveyId);
                    break;
                case BatchType.Filtered:
                    title = GetTitleForAsyncOperation("Change dial mode of filtered interviews", surveyId);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return StartAsyncOperation(parameters, title);
        }             

        public static BvAsyncOperationQueueEntity StartAsyncOperation(IAsyncOperationParameters parameters, string title)
        {
            var supervisorName = ServiceLocator.Resolve<ISupervisorNameProvider>().Name;

            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            var operationEntity = ServiceLocator.Resolve<IAsyncOperationQueue>().Enqueue(
                callCenterId,
                title,
                false,
                parameters,
                AsyncOperationConstants.HighPriority,
                supervisorName);

            return operationEntity;
        }

        public static bool HasQuotas(string surveyName, int sid)
        {
            bool hasQuotas;
            var quotaInfoService = ServiceLocator.Resolve<IQuotaInfoService>();

            try
            {
                hasQuotas = quotaInfoService.HasQuotas(sid);
            }
            catch (Exception)
            {
                AttachSurveyDb(surveyName);
                hasQuotas = quotaInfoService.HasQuotas(sid);
            }

            return hasQuotas;
        }
    }
}
