using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Core.Surveys
{
    public class CallOperationsProvider : ICallOperationsProvider
    {
        private static readonly List<CallOperation> Operations = GetOperationsNamesBasedOnLocale(); 

        public List<CallOperation> GetAll()
        {
            return Operations;
        }

        private static List<CallOperation> GetOperationsNamesBasedOnLocale()
        {
            //currently we just get English version

            var operations = new List<CallOperation>
            {
                new CallOperation {Title = Strings.OperationInterview, Id = OperationType.Interview},
                new CallOperation {Title = Strings.OperationWebInterview, Id = OperationType.WebInterview},
                new CallOperation {Title = Strings.OperationAddedInWebInterview, Id = OperationType.AddRecordInWebInterview},
                new CallOperation {Title = Strings.OperationActivateCalls, Id = OperationType.ActivateCalls},
                new CallOperation {Title = Strings.OperationAddCall, Id = OperationType.AddCall},
                new CallOperation {Title = Strings.OperationAssignCalls, Id = OperationType.AssignCalls},
                new CallOperation {Title = Strings.OperationChangeDialMode, Id = OperationType.ChangeDiallingMode},
                new CallOperation {Title = Strings.OperationChangePriorityOfCalls, Id = OperationType.ChangePriorityOfCalls},
                new CallOperation {Title = Strings.OperationChangeShiftTypesOfCalls, Id = OperationType.ChangeShiftTypesOfCall},
                new CallOperation {Title = Strings.OperationDeleteCalls, Id = OperationType.DeleteCalls},
                new CallOperation {Title = Strings.OperationDeleteCallsByFcd, Id = OperationType.DeleteCallsByFcd},
                new CallOperation {Title = Strings.OperationDisableByFcd, Id = OperationType.DisableByFcd},
                new CallOperation {Title = Strings.OperationDisableCalls, Id = OperationType.DisableCalls},
                new CallOperation {Title = Strings.OperationEnableCalls, Id = OperationType.EnableCalls},
                new CallOperation {Title = Strings.OperationExpiredByDialler, Id = OperationType.ExpireByDialler},
                new CallOperation {Title = Strings.OperationExpiredCall, Id = OperationType.ExpiredCall},
                new CallOperation {Title = Strings.OperationFullSchedulingSample, Id = OperationType.SampleAddFullScheduling},
                new CallOperation {Title = Strings.OperationMoveCallsToIts, Id = OperationType.MoveCallsToIts},
                new CallOperation {Title = Strings.OperationMovedAndRescheduled, Id = OperationType.MovedAndReschedule},
                new CallOperation {Title = Strings.OperationNotConnectedCall, Id = OperationType.NotConnectedCall},
                new CallOperation {Title = Strings.OperationPromoteCall, Id = OperationType.PromoteCall},
                new CallOperation {Title = Strings.OperationReturnedNotDialled, Id = OperationType.ReturnNotDialled},
                new CallOperation {Title = Strings.OperationSimpleSchedulingSample, Id = OperationType.SimpleAddSchedulingSample},
                new CallOperation {Title = Strings.OperationTelephonyError, Id = OperationType.TelephonyError},
                new CallOperation {Title = Strings.OperationTerminateTask, Id = OperationType.TerminateTask},
                new CallOperation {Title = Strings.OperationUpdateCall, Id = OperationType.UpdateCall},
                new CallOperation {Title = Strings.OperationUpdatedInWebInterview, Id = OperationType.UpdateRecordInWebInterview},
                new CallOperation {Title = Strings.OperationDeleteByFcdDuringSample, Id = OperationType.DeleteByFcdDuringSample},
                new CallOperation {Title = Strings.OperationDisableByFcdDuringSample, Id = OperationType.DisableByFcdDuringSample},
                new CallOperation {Title = Strings.SynchronizeEnableDisableCallState, Id = OperationType.SynchronizeEnableDisableCallState},
                new CallOperation {Title = Strings.SchedulingScriptExecutionError, Id = OperationType.SchedulingScriptExecutionError},
                new CallOperation {Title = Strings.OperationDeleteCallByBlacklistDuringSample, Id = OperationType.DeleteCallByBlacklistInAddSample},
                new CallOperation {Title = Strings.OperationSampleUpdate, Id = OperationType.UpdateBySampleUpdate},
                new CallOperation {Title = Strings.OperationAddRecordFromConsole, Id = OperationType.AddRecordFromConsole},
                new CallOperation {Title = Strings.OperationAddRecordByInboundCall, Id = OperationType.AddRecordByInboundCall},
                new CallOperation {Title = Strings.InboundCall, Id = OperationType.InboundCall},
                new CallOperation {Title = Strings.InternalTransfer, Id = OperationType.InternalTransfer},
                new CallOperation {Title = Strings.DroppedByRespondent, Id = OperationType.DroppedByRespondent},
                new CallOperation {Title = Strings.OperationEditCalls, Id = OperationType.EditCalls},
                new CallOperation {Title = Strings.OperationEditCallHistory, Id = OperationType.EditCallHistory},
                new CallOperation {Title = Strings.OperationDeleteCallHistory, Id = OperationType.DeleteCallHistory},
                new CallOperation {Title = Strings.SynchronizedSample, Id = OperationType.SynchronizeRespondents},
               
            };

            return operations;
        }
    }
}