using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.IntegrationTests.Tests.AsyncOperations
{
    public class TestCallManagementOperationFactory
    {
        private readonly int _portionSize;

        public TestCallManagementOperationFactory()
        {
            _portionSize = 1000;
        }

        public TestCallManagementOperationFactory(int portionSize)
        {
            _portionSize = portionSize;
        }

        public AsyncOperationResult CreateActivateCallsSelected(
            int surveyId,
            int[] selectedCalls,
            int priority,
            int personOrGroupId,
            int shiftTypeId,
            CallStates callState,
            bool enableDisabledCalls)
        {
            return CreateActivateCallsSelected(
                surveyId,
                selectedCalls,
                priority,
                personOrGroupId,
                shiftTypeId,
                DateTime.FromOADate(0),
                callState,
                enableDisabledCalls,
                string.Empty);
        }

        public AsyncOperationResult CreateActivateCallsSelected(
            int surveyId,
            int[] selectedCalls,
            int priority,
            int personOrGroupId,
            int shiftTypeId,
            DateTime timeToCall,
            CallStates callState,
            bool enableDisabledCalls)
        {
            return CreateActivateCallsSelected(
                surveyId,
                selectedCalls,
                priority,
                personOrGroupId,
                shiftTypeId,
                timeToCall,
                callState,
                enableDisabledCalls,
                string.Empty);
        }

        public AsyncOperationResult CreateActivateCallsSelected(
            int surveyId,
            int[] selectedCalls,
            int priority,
            int personOrGroupId,
            int shiftTypeId,
            DateTime timeToCall,
            CallStates callState,
            bool enableDisabledCalls,
            int portionSize)
        {
            return CreateActivateCallsSelected(
                surveyId,
                selectedCalls,
                priority,
                personOrGroupId,
                shiftTypeId,
                timeToCall,
                callState,
                enableDisabledCalls,
                string.Empty,
                portionSize);
        }

        public AsyncOperationResult CreateActivateCallsSelected(
            int surveyId,
            int[] selectedCalls,
            int priority,
            int personOrGroupId,
            int shiftTypeId,
            DateTime timeToCall,
            CallStates callState,
            bool enableDisabledCalls,
            string supervisorName)
        {
            return CreateActivateCallsSelected(
                surveyId,
                selectedCalls,
                priority,
                personOrGroupId,
                shiftTypeId,
                timeToCall,
                callState,
                enableDisabledCalls,
                supervisorName,
                _portionSize);
        }

        public AsyncOperationResult CreateActivateCallsSelected(
            int surveyId,
            int[] selectedCalls,
            int priority,
            int personOrGroupId,
            int shiftTypeId,
            DateTime timeToCall,
            CallStates callState,
            bool enableDisabledCalls,
            string supervisorName, 
            int portionSize)
        {
            ServiceLocator.Resolve<ISystemSettings>().AsyncOperation.ActivatePortionSize = portionSize;

            var parameters = new Core.AsyncOperations.Operations.CallsManagementOperations.ActivateCalls.
                Parameters
                {
                    SurveyId = surveyId,
                    BatchParameters = new SelectedBatchParameters(selectedCalls),
                    Priority = priority,
                    CallState = callState,
                    ResourceIds = new []{personOrGroupId},
                    ShiftTypeId = shiftTypeId,
                    TimeToCall = timeToCall,
                    EnableDisabledCalls = enableDisabledCalls
                };

            var queue = ServiceLocator.Resolve<IAsyncOperationQueue>();

            var entity = queue.Enqueue(
                0,
                "", 
                false, 
                parameters, 
                AsyncOperationConstants.NormalPriority, 
                supervisorName);


            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(entity);
        }


        public AsyncOperationResult CreateActivateCallsFiltered(
            int surveyId,
            int filterId,
            int priority,
            int personOrGroupId,
            int shiftTypeId,
            int timezoneId,
            CallStates callState,
            bool enableDisabledCalls)
        {
            return CreateActivateCallsFiltered(
                surveyId,
                filterId,
                priority,
                personOrGroupId,
                shiftTypeId,
                timezoneId,
                DateTime.FromOADate(0),
                callState,
                enableDisabledCalls,
                string.Empty,
                _portionSize);
        }

        public AsyncOperationResult CreateActivateCallsFiltered(
            int surveyId,
            int filterId,
            int priority,
            int personOrGroupId,
            int shiftTypeId,
            int timezoneId,
            DateTime timeToCall,
            CallStates callState,
            bool enableDisabledCalls)
        {
            return CreateActivateCallsFiltered(
                surveyId,
                filterId,
                priority,
                personOrGroupId,
                shiftTypeId,
                timezoneId,
                timeToCall,
                callState,
                enableDisabledCalls,
                string.Empty,
                _portionSize);
        }

        public AsyncOperationResult CreateActivateCallsFiltered(
            int surveyId,
            int filterId,
            int priority,
            int personOrGroupId,
            int shiftTypeId,
            int timezoneId,
            DateTime timeToCall,
            CallStates callState,
            bool enableDisabledCalls,
            string supervisorName,
            int portionSize)
        {
            ServiceLocator.Resolve<ISystemSettings>().AsyncOperation.ActivatePortionSize = portionSize;

            var parameters = new Core.AsyncOperations.Operations.CallsManagementOperations.ActivateCalls.
                Parameters
            {
                SurveyId = surveyId,
                BatchParameters = new FilteredBatchParameters(surveyId, filterId, timezoneId, callState, new SearchParameterCollection()),
                Priority = priority,
                ResourceIds = new[] { personOrGroupId },
                ShiftTypeId = shiftTypeId,
                TimeToCall = timeToCall,
                CallState = callState,
                EnableDisabledCalls = enableDisabledCalls
            };

            var queue = ServiceLocator.Resolve<IAsyncOperationQueue>();

            var entity = queue.Enqueue(
                0,
                "",
                false,
                parameters,
                AsyncOperationConstants.NormalPriority,
                supervisorName);


            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(entity);
        }

        public AsyncOperationResult CreateActivateCallsFiltered(
            int surveyId,
            int filterId,
            int timezoneId,
            SearchParameterCollection searchParams,
            int priority,
            CallStates callState,
            int personOrGroupId,
            int shiftTypeId,
            DateTime? timeToCall,
            bool enableDisabledCalls)
        {
            return CreateActivateCallsFiltered(
                surveyId, 
                string.Empty, 
                filterId, 
                timezoneId,
                searchParams, 
                priority, 
                callState,
                personOrGroupId, 
                shiftTypeId, 
                timeToCall, 
                enableDisabledCalls,
                string.Empty, 
                _portionSize);
        }

        public AsyncOperationResult CreateActivateCallsFiltered(
            int surveyId,
            string surveyName,
            int filterId,
            int timezoneId,
            SearchParameterCollection searchParams,
            int priority,
            CallStates callState,
            int personOrGroupId,
            int shiftTypeId,
            DateTime? timeToCall,
            bool enableDisabledCalls,
            string supervisorName,
            int portionSize)
        {
            ServiceLocator.Resolve<ISystemSettings>().AsyncOperation.ActivatePortionSize = portionSize;

            var parameters = new Core.AsyncOperations.Operations.CallsManagementOperations.ActivateCalls.
                Parameters
            {
                SurveyId = surveyId,
                BatchParameters = new FilteredBatchParameters(surveyId, filterId, timezoneId, callState, searchParams),
                Priority = priority,
                CallState = callState,
                ResourceIds = new[] { personOrGroupId },
                ShiftTypeId = shiftTypeId,
                TimeToCall = timeToCall,
                EnableDisabledCalls = enableDisabledCalls
            };

            var queue = ServiceLocator.Resolve<IAsyncOperationQueue>();

            var entity = queue.Enqueue(
                0,
                "",
                false,
                parameters,
                AsyncOperationConstants.NormalPriority,
                supervisorName);


            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(entity);
        }

        public AsyncOperationResult CreateActivateCallsFilteredCells(
            int surveyId,
            string[] quotaFields,
            string[][] cellsFields,
            int priority,
            CallStates callState,
            int personOrGroupId,
            int shiftTypeId,
            bool enableDisabledCalls)
        {
            return CreateActivateCallsFilteredCells(
                surveyId,
                string.Empty,
                quotaFields,
                cellsFields,
                priority,
                callState,
                personOrGroupId,
                shiftTypeId,
                DateTime.FromOADate(0),
                enableDisabledCalls,
                string.Empty,
                _portionSize);
        }

        public AsyncOperationResult CreateActivateCallsFilteredCells(
            int surveyId,
            string surveyName,
            string[] quotaFields,
            string[][] cellsFields,
            int priority,
            CallStates callState,
            int personOrGroupId,
            int shiftTypeId,
            DateTime? timeToCall,
            bool enableDisabledCalls,
            string supervisorName, 
            int portionSize)
        {
            ServiceLocator.Resolve<ISystemSettings>().AsyncOperation.ActivatePortionSize = portionSize;

            var parameters = new Core.AsyncOperations.Operations.CallsManagementOperations.ActivateCalls.
                Parameters
            {
                SurveyId = surveyId,
                BatchParameters = new FilteredByCellsBatchParameters(surveyId, quotaFields, cellsFields),
                Priority = priority,
                CallState = callState,
                ResourceIds = new[] { personOrGroupId },
                ShiftTypeId = shiftTypeId,
                TimeToCall = timeToCall,
                EnableDisabledCalls = enableDisabledCalls
            };

            var queue = ServiceLocator.Resolve<IAsyncOperationQueue>();

            var entity = queue.Enqueue(
                0,
                "",
                false,
                parameters,
                AsyncOperationConstants.NormalPriority,
                supervisorName);

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(entity);
        }

        public AsyncOperationResult CreateEnableCallsSelected(
            int surveyId, 
            int[] selectedCalls, 
            bool enablingState,
            bool isFcdOperation = false)
        {
            return CreateEnableCallsSelected(
                surveyId, 
                string.Empty, 
                selectedCalls, 
                enablingState, 
                string.Empty,
                _portionSize,
                isFcdOperation);
        }

        public AsyncOperationResult CreateEnableCallsSelected(
            int surveyId, 
            string surveyName, 
            int[] selectedCalls, 
            bool enablingState, 
            string supervisorName, 
            int portionSize,
            bool isFcdOperation)
        {
            ServiceLocator.Resolve<ISystemSettings>().AsyncOperation.MovePortionSize = portionSize;

            var parameters = new Core.AsyncOperations.Operations.CallsManagementOperations.EnableCalls.Parameters
            {
                SurveyId = surveyId,
                BatchParameters = new SelectedBatchParameters(selectedCalls),
                EnablingState = enablingState,
                IsFcdOperation = isFcdOperation
            };

            var queue = ServiceLocator.Resolve<IAsyncOperationQueue>();

            var entity = queue.Enqueue(
                0,
                "",
                false,
                parameters,
                AsyncOperationConstants.NormalPriority,
                supervisorName);

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(entity);
        }

        public AsyncOperationResult CreateEnableCallsFiltered(
            int surveyId, 
            int filterId, 
            bool enablingState)
        {
            ServiceLocator.Resolve<ISystemSettings>().AsyncOperation.MovePortionSize = _portionSize;

            var parameters = new Core.AsyncOperations.Operations.CallsManagementOperations.EnableCalls.Parameters
            {
                SurveyId = surveyId,
                BatchParameters = new FilteredBatchParameters(surveyId, filterId, 1, CallStates.Scheduled, new SearchParameterCollection()),
                EnablingState = enablingState
            };

            var queue = ServiceLocator.Resolve<IAsyncOperationQueue>();

            var entity = queue.Enqueue(
                0,
                "",
                false,
                parameters,
                AsyncOperationConstants.NormalPriority,
                "");

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(entity);
        }

        public AsyncOperationResult ChangeDialModeOfInterviews(int surveyId, BatchParameters batchParameters, ConfirmitDialerInterface.DialingMode? dialingMode)
        {            
            ServiceLocator.Resolve<ISystemSettings>().AsyncOperation.MovePortionSize = _portionSize;

            var parameters = new Core.AsyncOperations.Operations.CallsManagementOperations.ChangeDialModeOfInterviews.Parameters
            {
                SurveyId = surveyId,
                BatchParameters = batchParameters,
                DialingMode = dialingMode
            };            

            var queue = ServiceLocator.Resolve<IAsyncOperationQueue>();

            var entity = queue.Enqueue(
                0,
                "",
                false,
                parameters,
                AsyncOperationConstants.NormalPriority, String.Empty);

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(entity);
        }

    }
}
