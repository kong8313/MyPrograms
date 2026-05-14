using Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.Batch.Initializers;
using Confirmit.CATI.Core.Batch.Interfaces;
using Confirmit.CATI.Core.Batch.Tools;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public class AsyncOperationRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator
                .RegisterSingleton<IAsyncOperationExecutor, AsyncOperationExecutor>()
                .Register<IAsyncOperationFactory, AsyncOperationFactory>()
                .Register<IAsyncOperationProgressLogger, AsyncOperationProgressLogger>()
                .Register<IAsyncOperationQueue, AsyncOperationQueue>()
                .Register<IAsyncOperationRepository, AsyncOperationRepository>()
                .Register<IAsyncOperationRetry, AsyncOperationRetry>()
                .Register<IAsyncOperationAwaiter, AsyncOperationAwaiter>()
                .Register<ISystemSettingRepository, SystemSettingRepository>()
                .RegisterSingleton<IAsyncOperationSchedulerThread, AsyncOperationSchedulerThread>()
                .RegisterSingleton<IAsyncOperationsHeartBeatUpdaterThread, AsyncOperationsHeartBeatUpdaterThread>()
                .RegisterSingleton<AsyncOperationCancellationService>(new AsyncOperationCancellationService())

                .Register<IOperationDescriptor, Operations.CallsManagementOperations.ActivateCalls.Descriptor>("Activate.Descriptor")
                .Register<IOperationDescriptor, Operations.CallsManagementOperations.EditCalls.Descriptor>("Edit.Descriptor")
                .Register<IOperationDescriptor, Operations.CallsManagementOperations.EnableCalls.Descriptor>("Enable.Descriptor")
                .Register<IOperationDescriptor, Operations.CallsManagementOperations.DeleteCalls.Descriptor>("Delete.Descriptor")
                .Register<IOperationDescriptor, Operations.CallsManagementOperations.MoveCalls.Descriptor>("Move.Descriptor")
                .Register<IOperationDescriptor, Operations.CallsManagementOperations.ChangeDialModeOfInterviews.Descriptor>("ChangeDialMode.Descriptor")
                .Register<IOperationDescriptor, Operations.CallsManagementOperations.MoveAndRescheduleCalls.Descriptor>("MoveAndReschedule.Descriptor")
                .Register<IOperationDescriptor, Operations.CallsManagementOperations.AssignCalls.Descriptor>("Assign.Descriptor")
                .Register<IOperationDescriptor, Operations.CallsManagementOperations.ChangePriorityOfCalls.Descriptor>("ChangePriority.Descriptor")
                .Register<IOperationDescriptor, Operations.CallsManagementOperations.ChangeShiftTypeOfCalls.Descriptor>("ChangeShiftType.Descriptor")
                .Register<IOperationDescriptor, Operations.CallsManagementOperations.UpdateFcdStatusOfCalls.Descriptor>("UpdateFcdStatusOfCalls.Descriptor")
                .Register<IOperationDescriptor, Operations.RestoreSurvey.Descriptor>("RestoreSurvey.Descriptor")
                .Register<IOperationDescriptor, Operations.LaunchSurvey.Descriptor>("LaunchSurvey.Descriptor")
                .Register<IOperationDescriptor, Operations.DeleteSurvey.Descriptor>("DeleteSurvey.Descriptor")
                .Register<IOperationDescriptor, Operations.RereadSurveyReplicatedData.Descriptor>("RereadSurveyReplicatedData.Descriptor")
                .Register<IOperationDescriptor, Operations.DeleteRespondents.Descriptor>("DeleteRespondents.Descriptor")
                .Register<IOperationDescriptor, Operations.ConfigureClusteredQuota.Descriptor>("ConfigureClusteredQuota.Descriptor")
                .Register<IOperationDescriptor, Operations.ExecuteRoutineMaintenance.Descriptor>("ExecuteRoutineMaintenance.Descriptor")
                .Register<IOperationDescriptor, Operations.SynchronizeRespondents.Descriptor>("SynchronizeRespondents.Descriptor")
                .Register<IOperationDescriptor, Operations.SampleUpload.Descriptor>("SampleUpload.Descriptor")
                
                .Register<Operations.CallsManagementOperations.ActivateCalls.Operation, Operations.CallsManagementOperations.ActivateCalls.Operation>()
                .Register<Operations.CallsManagementOperations.EditCalls.Operation, Operations.CallsManagementOperations.EditCalls.Operation>()
                .Register<Operations.CallsManagementOperations.EnableCalls.Operation, Operations.CallsManagementOperations.EnableCalls.Operation>()
                .Register<Operations.CallsManagementOperations.DeleteCalls.Operation, Operations.CallsManagementOperations.DeleteCalls.Operation>()
                .Register<Operations.CallsManagementOperations.MoveCalls.Operation, Operations.CallsManagementOperations.MoveCalls.Operation>()
                .Register<Operations.CallsManagementOperations.ChangeDialModeOfInterviews.Operation, Operations.CallsManagementOperations.ChangeDialModeOfInterviews.Operation>()
                .Register<Operations.CallsManagementOperations.MoveAndRescheduleCalls.Operation, Operations.CallsManagementOperations.MoveAndRescheduleCalls.Operation>()
                .Register<Operations.CallsManagementOperations.AssignCalls.Operation, Operations.CallsManagementOperations.AssignCalls.Operation>()
                .Register<Operations.CallsManagementOperations.ChangePriorityOfCalls.Operation, Operations.CallsManagementOperations.ChangePriorityOfCalls.Operation>()
                .Register<Operations.CallsManagementOperations.ChangeShiftTypeOfCalls.Operation, Operations.CallsManagementOperations.ChangeShiftTypeOfCalls.Operation>()
                .Register<Operations.CallsManagementOperations.UpdateFcdStatusOfCalls.Operation, Operations.CallsManagementOperations.UpdateFcdStatusOfCalls.Operation>()
                .Register<Operations.RestoreSurvey.Operation, Operations.RestoreSurvey.Operation>()
                .Register<Operations.LaunchSurvey.Operation, Operations.LaunchSurvey.Operation>()
                .Register<Operations.DeleteSurvey.Operation, Operations.DeleteSurvey.Operation>()
                .Register<Operations.RereadSurveyReplicatedData.Operation, Operations.RereadSurveyReplicatedData.Operation>()
                .Register<Operations.DeleteRespondents.Operation, Operations.DeleteRespondents.Operation>()
                .Register<Operations.ConfigureClusteredQuota.Operation, Operations.ConfigureClusteredQuota.Operation>()
                .Register<Operations.ExecuteRoutineMaintenance.Operation, Operations.ExecuteRoutineMaintenance.Operation>()
                .Register<Operations.SynchronizeRespondents.Operation, Operations.SynchronizeRespondents.Operation>()

                .Register<ICallsManagementBatchedOperationBase, CallsManagementBatchedOperationBase>()
                .Register<ICallsManagementOperationBase, CallsManagementOperationBase>()
                
                //batch
                .Register<IBatchFactory, BatchFactory>()
                .Register<IDatabaseBatchItemTransfer, DatabaseBatchItemTransfer>()
                .Register<IBatchInitializer, FilteredBatchInitializer>("FilteredBatchInitializer")
                .Register<IBatchInitializer, FilteredByCellsBatchInitializer>("FilteredByCellsBatchInitializer")
                .Register<IBatchInitializer, FilteredByMultipleCellsBatchInitializer>("FilteredByMultipleCellsBatchInitializer")
                .Register<IBatchInitializer, FilteredByClosedQuotaCellBatchInitializer>("FilteredByClosedQuotaCellBatchInitializer")
                .Register<IBatchInitializer, FilteredByOpenedQuotaCellBatchInitializer>("FilteredByOpenedQuotaCellBatchInitializer")
                .Register<IBatchInitializer, QueriedBatchInitializer>("QueriedBatchInitializer")
                .Register<IBatchInitializer, SelectedBatchInitializer>("SelectedBatchInitializer");
        }
    }
}
