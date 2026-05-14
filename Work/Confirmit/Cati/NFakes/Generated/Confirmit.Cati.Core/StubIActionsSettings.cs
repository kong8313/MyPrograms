using System;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;

namespace Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Fakes
{
    public class StubIActionsSettings : IActionsSettings 
    {
        private IActionsSettings _inner;

        public StubIActionsSettings()
        {
            _inner = null;
        }

        public IActionsSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private IAnswerSubmissionAlertHistoryTableCleanupSettings _AnswerSubmissionAlertHistoryTableCleanup;
        public Func<IAnswerSubmissionAlertHistoryTableCleanupSettings> AnswerSubmissionAlertHistoryTableCleanupGet;
        public Action<IAnswerSubmissionAlertHistoryTableCleanupSettings> AnswerSubmissionAlertHistoryTableCleanupSetIAnswerSubmissionAlertHistoryTableCleanupSettings;

        IAnswerSubmissionAlertHistoryTableCleanupSettings IActionsSettings.AnswerSubmissionAlertHistoryTableCleanup
        {
            get
            {
                if (AnswerSubmissionAlertHistoryTableCleanupGet != null)
                {
                    return AnswerSubmissionAlertHistoryTableCleanupGet();
                } else if (_inner != null)
                {
                    return ((IActionsSettings)_inner).AnswerSubmissionAlertHistoryTableCleanup;
                }

                if (AnswerSubmissionAlertHistoryTableCleanupSetIAnswerSubmissionAlertHistoryTableCleanupSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AnswerSubmissionAlertHistoryTableCleanup;
                }

                return default(IAnswerSubmissionAlertHistoryTableCleanupSettings);
            }

        }

        private IAssignmentResourceTableCleanupSettings _AssignmentResourceTableCleanup;
        public Func<IAssignmentResourceTableCleanupSettings> AssignmentResourceTableCleanupGet;
        public Action<IAssignmentResourceTableCleanupSettings> AssignmentResourceTableCleanupSetIAssignmentResourceTableCleanupSettings;

        IAssignmentResourceTableCleanupSettings IActionsSettings.AssignmentResourceTableCleanup
        {
            get
            {
                if (AssignmentResourceTableCleanupGet != null)
                {
                    return AssignmentResourceTableCleanupGet();
                } else if (_inner != null)
                {
                    return ((IActionsSettings)_inner).AssignmentResourceTableCleanup;
                }

                if (AssignmentResourceTableCleanupSetIAssignmentResourceTableCleanupSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AssignmentResourceTableCleanup;
                }

                return default(IAssignmentResourceTableCleanupSettings);
            }

        }

        private IAsyncOperationQueueTableCleanupSettings _AsyncOperationQueueTableCleanup;
        public Func<IAsyncOperationQueueTableCleanupSettings> AsyncOperationQueueTableCleanupGet;
        public Action<IAsyncOperationQueueTableCleanupSettings> AsyncOperationQueueTableCleanupSetIAsyncOperationQueueTableCleanupSettings;

        IAsyncOperationQueueTableCleanupSettings IActionsSettings.AsyncOperationQueueTableCleanup
        {
            get
            {
                if (AsyncOperationQueueTableCleanupGet != null)
                {
                    return AsyncOperationQueueTableCleanupGet();
                } else if (_inner != null)
                {
                    return ((IActionsSettings)_inner).AsyncOperationQueueTableCleanup;
                }

                if (AsyncOperationQueueTableCleanupSetIAsyncOperationQueueTableCleanupSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AsyncOperationQueueTableCleanup;
                }

                return default(IAsyncOperationQueueTableCleanupSettings);
            }

        }

        private ICallHistoryTableCleanupSettings _CallHistoryTableCleanup;
        public Func<ICallHistoryTableCleanupSettings> CallHistoryTableCleanupGet;
        public Action<ICallHistoryTableCleanupSettings> CallHistoryTableCleanupSetICallHistoryTableCleanupSettings;

        ICallHistoryTableCleanupSettings IActionsSettings.CallHistoryTableCleanup
        {
            get
            {
                if (CallHistoryTableCleanupGet != null)
                {
                    return CallHistoryTableCleanupGet();
                } else if (_inner != null)
                {
                    return ((IActionsSettings)_inner).CallHistoryTableCleanup;
                }

                if (CallHistoryTableCleanupSetICallHistoryTableCleanupSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CallHistoryTableCleanup;
                }

                return default(ICallHistoryTableCleanupSettings);
            }

        }

        private ICallsSentToDialerTableCleanupSettings _CallsSentToDialerTableCleanup;
        public Func<ICallsSentToDialerTableCleanupSettings> CallsSentToDialerTableCleanupGet;
        public Action<ICallsSentToDialerTableCleanupSettings> CallsSentToDialerTableCleanupSetICallsSentToDialerTableCleanupSettings;

        ICallsSentToDialerTableCleanupSettings IActionsSettings.CallsSentToDialerTableCleanup
        {
            get
            {
                if (CallsSentToDialerTableCleanupGet != null)
                {
                    return CallsSentToDialerTableCleanupGet();
                } else if (_inner != null)
                {
                    return ((IActionsSettings)_inner).CallsSentToDialerTableCleanup;
                }

                if (CallsSentToDialerTableCleanupSetICallsSentToDialerTableCleanupSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CallsSentToDialerTableCleanup;
                }

                return default(ICallsSentToDialerTableCleanupSettings);
            }

        }

        private IDatabaseMaintenanceSettings _DatabaseMaintenance;
        public Func<IDatabaseMaintenanceSettings> DatabaseMaintenanceGet;
        public Action<IDatabaseMaintenanceSettings> DatabaseMaintenanceSetIDatabaseMaintenanceSettings;

        IDatabaseMaintenanceSettings IActionsSettings.DatabaseMaintenance
        {
            get
            {
                if (DatabaseMaintenanceGet != null)
                {
                    return DatabaseMaintenanceGet();
                } else if (_inner != null)
                {
                    return ((IActionsSettings)_inner).DatabaseMaintenance;
                }

                if (DatabaseMaintenanceSetIDatabaseMaintenanceSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DatabaseMaintenance;
                }

                return default(IDatabaseMaintenanceSettings);
            }

        }

        private IFullSynchronizationOfCatiDataInHubSettings _FullSynchronizationOfCatiDataInHub;
        public Func<IFullSynchronizationOfCatiDataInHubSettings> FullSynchronizationOfCatiDataInHubGet;
        public Action<IFullSynchronizationOfCatiDataInHubSettings> FullSynchronizationOfCatiDataInHubSetIFullSynchronizationOfCatiDataInHubSettings;

        IFullSynchronizationOfCatiDataInHubSettings IActionsSettings.FullSynchronizationOfCatiDataInHub
        {
            get
            {
                if (FullSynchronizationOfCatiDataInHubGet != null)
                {
                    return FullSynchronizationOfCatiDataInHubGet();
                } else if (_inner != null)
                {
                    return ((IActionsSettings)_inner).FullSynchronizationOfCatiDataInHub;
                }

                if (FullSynchronizationOfCatiDataInHubSetIFullSynchronizationOfCatiDataInHubSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _FullSynchronizationOfCatiDataInHub;
                }

                return default(IFullSynchronizationOfCatiDataInHubSettings);
            }

        }

        private ILargeObjectHeapFragmentationSettings _LargeObjectHeapFragmentation;
        public Func<ILargeObjectHeapFragmentationSettings> LargeObjectHeapFragmentationGet;
        public Action<ILargeObjectHeapFragmentationSettings> LargeObjectHeapFragmentationSetILargeObjectHeapFragmentationSettings;

        ILargeObjectHeapFragmentationSettings IActionsSettings.LargeObjectHeapFragmentation
        {
            get
            {
                if (LargeObjectHeapFragmentationGet != null)
                {
                    return LargeObjectHeapFragmentationGet();
                } else if (_inner != null)
                {
                    return ((IActionsSettings)_inner).LargeObjectHeapFragmentation;
                }

                if (LargeObjectHeapFragmentationSetILargeObjectHeapFragmentationSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _LargeObjectHeapFragmentation;
                }

                return default(ILargeObjectHeapFragmentationSettings);
            }

        }

        private IMessageTableCleanupSettings _MessageTableCleanup;
        public Func<IMessageTableCleanupSettings> MessageTableCleanupGet;
        public Action<IMessageTableCleanupSettings> MessageTableCleanupSetIMessageTableCleanupSettings;

        IMessageTableCleanupSettings IActionsSettings.MessageTableCleanup
        {
            get
            {
                if (MessageTableCleanupGet != null)
                {
                    return MessageTableCleanupGet();
                } else if (_inner != null)
                {
                    return ((IActionsSettings)_inner).MessageTableCleanup;
                }

                if (MessageTableCleanupSetIMessageTableCleanupSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MessageTableCleanup;
                }

                return default(IMessageTableCleanupSettings);
            }

        }

        private IPersonDeferredMonitoringTableCleanupSettings _PersonDeferredMonitoringTableCleanup;
        public Func<IPersonDeferredMonitoringTableCleanupSettings> PersonDeferredMonitoringTableCleanupGet;
        public Action<IPersonDeferredMonitoringTableCleanupSettings> PersonDeferredMonitoringTableCleanupSetIPersonDeferredMonitoringTableCleanupSettings;

        IPersonDeferredMonitoringTableCleanupSettings IActionsSettings.PersonDeferredMonitoringTableCleanup
        {
            get
            {
                if (PersonDeferredMonitoringTableCleanupGet != null)
                {
                    return PersonDeferredMonitoringTableCleanupGet();
                } else if (_inner != null)
                {
                    return ((IActionsSettings)_inner).PersonDeferredMonitoringTableCleanup;
                }

                if (PersonDeferredMonitoringTableCleanupSetIPersonDeferredMonitoringTableCleanupSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _PersonDeferredMonitoringTableCleanup;
                }

                return default(IPersonDeferredMonitoringTableCleanupSettings);
            }

        }

        private IPromotionHistoryTableCleanupSettings _PromotionHistoryTableCleanup;
        public Func<IPromotionHistoryTableCleanupSettings> PromotionHistoryTableCleanupGet;
        public Action<IPromotionHistoryTableCleanupSettings> PromotionHistoryTableCleanupSetIPromotionHistoryTableCleanupSettings;

        IPromotionHistoryTableCleanupSettings IActionsSettings.PromotionHistoryTableCleanup
        {
            get
            {
                if (PromotionHistoryTableCleanupGet != null)
                {
                    return PromotionHistoryTableCleanupGet();
                } else if (_inner != null)
                {
                    return ((IActionsSettings)_inner).PromotionHistoryTableCleanup;
                }

                if (PromotionHistoryTableCleanupSetIPromotionHistoryTableCleanupSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _PromotionHistoryTableCleanup;
                }

                return default(IPromotionHistoryTableCleanupSettings);
            }

        }

        private ISchedulingScriptLogTableCleanupSettings _SchedulingScriptLogTableCleanup;
        public Func<ISchedulingScriptLogTableCleanupSettings> SchedulingScriptLogTableCleanupGet;
        public Action<ISchedulingScriptLogTableCleanupSettings> SchedulingScriptLogTableCleanupSetISchedulingScriptLogTableCleanupSettings;

        ISchedulingScriptLogTableCleanupSettings IActionsSettings.SchedulingScriptLogTableCleanup
        {
            get
            {
                if (SchedulingScriptLogTableCleanupGet != null)
                {
                    return SchedulingScriptLogTableCleanupGet();
                } else if (_inner != null)
                {
                    return ((IActionsSettings)_inner).SchedulingScriptLogTableCleanup;
                }

                if (SchedulingScriptLogTableCleanupSetISchedulingScriptLogTableCleanupSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SchedulingScriptLogTableCleanup;
                }

                return default(ISchedulingScriptLogTableCleanupSettings);
            }

        }

        private IServiceBrokerObjectsCleanupSettings _ServiceBrokerObjectsCleanup;
        public Func<IServiceBrokerObjectsCleanupSettings> ServiceBrokerObjectsCleanupGet;
        public Action<IServiceBrokerObjectsCleanupSettings> ServiceBrokerObjectsCleanupSetIServiceBrokerObjectsCleanupSettings;

        IServiceBrokerObjectsCleanupSettings IActionsSettings.ServiceBrokerObjectsCleanup
        {
            get
            {
                if (ServiceBrokerObjectsCleanupGet != null)
                {
                    return ServiceBrokerObjectsCleanupGet();
                } else if (_inner != null)
                {
                    return ((IActionsSettings)_inner).ServiceBrokerObjectsCleanup;
                }

                if (ServiceBrokerObjectsCleanupSetIServiceBrokerObjectsCleanupSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ServiceBrokerObjectsCleanup;
                }

                return default(IServiceBrokerObjectsCleanupSettings);
            }

        }

        private ISurveyCleanupSettings _SurveyCleanup;
        public Func<ISurveyCleanupSettings> SurveyCleanupGet;
        public Action<ISurveyCleanupSettings> SurveyCleanupSetISurveyCleanupSettings;

        ISurveyCleanupSettings IActionsSettings.SurveyCleanup
        {
            get
            {
                if (SurveyCleanupGet != null)
                {
                    return SurveyCleanupGet();
                } else if (_inner != null)
                {
                    return ((IActionsSettings)_inner).SurveyCleanup;
                }

                if (SurveyCleanupSetISurveyCleanupSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SurveyCleanup;
                }

                return default(ISurveyCleanupSettings);
            }

        }

        private IUserSurveyListTableCleanupSettings _UserSurveyListTableCleanup;
        public Func<IUserSurveyListTableCleanupSettings> UserSurveyListTableCleanupGet;
        public Action<IUserSurveyListTableCleanupSettings> UserSurveyListTableCleanupSetIUserSurveyListTableCleanupSettings;

        IUserSurveyListTableCleanupSettings IActionsSettings.UserSurveyListTableCleanup
        {
            get
            {
                if (UserSurveyListTableCleanupGet != null)
                {
                    return UserSurveyListTableCleanupGet();
                } else if (_inner != null)
                {
                    return ((IActionsSettings)_inner).UserSurveyListTableCleanup;
                }

                if (UserSurveyListTableCleanupSetIUserSurveyListTableCleanupSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _UserSurveyListTableCleanup;
                }

                return default(IUserSurveyListTableCleanupSettings);
            }

        }

    }
}