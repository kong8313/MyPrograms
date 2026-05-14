using System;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using System.Collections.Generic;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Paging;

namespace Confirmit.CATI.Core.ActivityLogging
{
    abstract public class CallEventParameters : ManagementActivityEventDetails
    {
        #region Constructors

        protected CallEventParameters()
        {
        }

        protected CallEventParameters(BvCallEntity callEntity)
        {
            InterviewId = callEntity.InterviewID;
            CallId = callEntity.CallID;
            Priority = callEntity.Priority;
            ShiftId = callEntity.ShiftID;
            TimeInShift = callEntity.TimeInShift;
            TimeToExpire = callEntity.TimeToExpire;
        }

        #endregion

        #region Properties

        public virtual int InterviewId { get; set; }

        public virtual int CallId { get; set; }

        public virtual int Priority { get; set; }

        public virtual int? ShiftId { get; set; }

        public virtual DateTime? TimeInShift { get; set; }

        public virtual DateTime? TimeToExpire { get; set; }

        #endregion
    }

    [Serializable]
    public class CreateCallEventParameters : CallEventParameters
    {
        #region Constructors

        public CreateCallEventParameters()
            : base()
        {
        }

        public CreateCallEventParameters(BvCallEntity callEntity)
            : base(callEntity)
        {
        }

        #endregion
    }

    [Serializable]
    public class UpdateCallEventParameters : CallEventParameters
    {
        #region Constructors

        public UpdateCallEventParameters()
            : base()
        {
        }

        public UpdateCallEventParameters(BvCallEntity callEntity)
            : base(callEntity)
        {
        }

        #endregion
    }

    public abstract class SelectedCallsEventParameters : ManagementActivityEventDetails
    {
        #region Constructors

        protected SelectedCallsEventParameters()
        {
        }

        protected SelectedCallsEventParameters(IEnumerable<BvCallEntity> calls)
        {
            Ids = calls.Select(x => x.CallID).ToArray();
        }

        protected SelectedCallsEventParameters(IEnumerable<int> callIds)
        {
            Ids = callIds.ToArray();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets selected calls identifiers.
        /// </summary>
        public virtual int[] Ids { get; set; }

        #endregion
    }

    public abstract class FilteredCallsEventParameters : ManagementActivityEventDetails
    {
        #region Properties

        public virtual int FilterId { get; set; }
        public virtual string FilterSql { get; set; }

        public virtual int CallStates { get; set; }

        public virtual SearchParameterCollection SearchParameters { get; set; }

        #endregion
    }

    public abstract class FilteredCellCallsEventParameters : ManagementActivityEventDetails
    {
        public virtual string[] QuotaFields { get; set; }
        public virtual string[][] QuotaFieldsCells { get; set; }
    }

    [Serializable]
    public class DeleteCallEventParameters : SelectedCallsEventParameters
    {
        #region Constructors

        public DeleteCallEventParameters()
            : base()
        {
        }

        public DeleteCallEventParameters(IEnumerable<BvCallEntity> calls)
            : base(calls)
        {
            InterviewIds = calls.Select(x => x.InterviewID).ToArray();
        }

        #endregion

        #region Properties

        public int[] InterviewIds { get; set; }

        #endregion
    }

    [Serializable]
    public class DeleteFilteredCallsEventParameters : FilteredCallsEventParameters
    {
    }

    [ManagementEventAttribute(ManagementEvent.CreateCall)]
    public class CreateCallEvent : ManagementActivityEvent<CreateCallEventParameters>
    {
        #region Constructors

        public CreateCallEvent(BvCallEntity callEntity, string surveyName):
            base(ManagementEventCategory.Call, ManagementEvent.CreateCall)
        {
            ObjectId = callEntity.SurveySID;
            ObjectName = surveyName;
            Details = new CreateCallEventParameters(callEntity);
        }

        #endregion
    }

    [ManagementEventAttribute(ManagementEvent.UpdateCall)]
    public class UpdateCallEvent : ManagementActivityEvent<UpdateCallEventParameters>
    {
        #region Constructors

        public UpdateCallEvent(BvCallEntity callEntity, string surveyName):
            base(ManagementEventCategory.Call, ManagementEvent.UpdateCall)
        {
            ObjectId = callEntity.SurveySID;
            ObjectName = surveyName;
            Details = new UpdateCallEventParameters(callEntity);
        }

        #endregion
    }

    [Serializable]
    public class AsyncOperationEventParameters<T> : ManagementActivityEventDetails
    {
        public T Parameters { get; set; }
        public BvAsyncOperationQueueEntity Entity { get; set; }
        public string Result { get; set; }
    }

    public class BaseAsyncOperationManagementActivityEvent<T> : ManagementActivityEvent<AsyncOperationEventParameters<T>>
    {
        public BaseAsyncOperationManagementActivityEvent(ManagementEventCategory category, ManagementEvent eventType, T parameters, BvAsyncOperationQueueEntity entity):
            base(category, eventType)
        {
            Details = new AsyncOperationEventParameters<T> { Parameters = parameters, Entity = entity};

            this.Supervisor = entity.CreatedBySupervisorName;
        }
    }

    [Serializable]
    public class ExportCallListEventParameters : ManagementActivityEventDetails
    {
        public int? FilterId { get; set; }
        public string ShowTimeMode { get; set; }
        public PagingArgs PageArguments { get; set; }
        public string CallState { get; set; }
        public string PagesForExport { get; set; }
        public string[] Variables { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.ExportCallList)]
    public class ExportCallListEvent : ManagementActivityEvent<ExportCallListEventParameters>
    {
        public ExportCallListEvent(
            int surveyId,
            string projectId,
            int? filterId,
            PagingArgs pageArguments,
            string showTimeMode,
            string callState,
            string pages,
            string[] variables):
            base(ManagementEventCategory.Call, ManagementEvent.ExportCallList)
        {
            ObjectId = surveyId;
            ObjectName = projectId;
            Details = new ExportCallListEventParameters
            {
                FilterId = filterId,
                    ShowTimeMode = showTimeMode,
                    CallState = callState,
                    PageArguments = pageArguments,
                    PagesForExport = pages,
                    Variables = variables
            };
        }
    }

    [Serializable]
    public class SaveCallManagementSearchableFieldsEventParameters : ManagementActivityEventDetails
        {
        public string[] Variables { get; set; }
        }

    [ManagementEventAttribute(ManagementEvent.SaveCallMangementSearchableFields)]
    public class SaveCallMangementSearchableFieldsEvent :
        ManagementActivityEvent<SaveCallManagementSearchableFieldsEventParameters>
    {
        public SaveCallMangementSearchableFieldsEvent(int surveyId, string projectId, string[] variables):
            base(ManagementEventCategory.Call, ManagementEvent.SaveCallMangementSearchableFields)
        {
            ObjectId = surveyId;
            ObjectName = projectId;
            Details = new SaveCallManagementSearchableFieldsEventParameters()
                {
                    Variables = variables
                };
        }
    }

    [ManagementEventAttribute(ManagementEvent.ActivateFilteredCalls)]
    public class ActivateFilteredCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.ActivateCalls.Parameters>
    {
        public ActivateFilteredCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.ActivateCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.ActivateFilteredCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.ActivateSelectedCalls)]
    public class ActivateSelectedCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.ActivateCalls.Parameters>
    {
        public ActivateSelectedCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.ActivateCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.ActivateSelectedCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.ActivateFilteredByCellsCalls)]
    public class ActivateFilteredByCellsCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.ActivateCalls.Parameters>
    {
        public ActivateFilteredByCellsCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.ActivateCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.ActivateFilteredByCellsCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.EditFilteredCalls)]
    public class EditFilteredCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.EditCalls.Parameters>
    {
        public EditFilteredCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.EditCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.EditFilteredCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.EditSelectedCalls)]
    public class EditSelectedCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.EditCalls.Parameters>
    {
        public EditSelectedCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.EditCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.EditSelectedCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.DeleteSelectedCalls)]
    public class DeleteSelectedCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.DeleteCalls.Parameters>
    {
        public DeleteSelectedCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.DeleteCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.DeleteSelectedCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.DeleteFilteredCalls)]
    public class DeleteFilteredCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.DeleteCalls.Parameters>
    {
        public DeleteFilteredCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.DeleteCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.DeleteFilteredCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.DeleteFilteredByClosedQuotaCellCalls)]
    public class DeleteFilteredByClosedQuotaCellEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.DeleteCalls.Parameters>
    {
        public DeleteFilteredByClosedQuotaCellEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.DeleteCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.DeleteFilteredByClosedQuotaCellCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.MoveSelectedCalls)]
    public class MoveSelectedCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.MoveCalls.Parameters>
    {
        public MoveSelectedCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.MoveCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.MoveSelectedCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.MoveFilteredCalls)]
    public class MoveFilteredCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.MoveCalls.Parameters>
    {
        public MoveFilteredCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.MoveCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.MoveFilteredCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.EnableSelectedCalls)]
    public class EnableSelectedCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.EnableCalls.Parameters>
    {
        public EnableSelectedCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.EnableCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.EnableSelectedCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
       }
    }

    
    [ManagementEventAttribute(ManagementEvent.UpdateFcdStatusOfCalls)]
    public class UpdateFcdStatusOfCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.UpdateFcdStatusOfCalls.Parameters>
    {
        public UpdateFcdStatusOfCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.UpdateFcdStatusOfCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.UpdateFcdStatusOfCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
       }
    }

    [ManagementEventAttribute(ManagementEvent.EnableFilteredCalls)]
    public class EnableFilteredCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.EnableCalls.Parameters>
    {
        public EnableFilteredCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.EnableCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.EnableFilteredCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.EnableFilteredByCellsCalls)]
    public class EnableFilteredByCellsCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.EnableCalls.Parameters>
    {
        public EnableFilteredByCellsCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.EnableCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.EnableFilteredByCellsCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.DisableSelectedCalls)]
    public class DisableSelectedCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.EnableCalls.Parameters>
    {
        public DisableSelectedCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.EnableCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.DisableSelectedCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.DisableFilteredCalls)]
    public class DisableFilteredCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.EnableCalls.Parameters>
    {
        public DisableFilteredCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.EnableCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.DisableFilteredCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.DisableFilteredByCellsCalls)]
    public class DisableFilteredByCellsCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.EnableCalls.Parameters>
    {
        public DisableFilteredByCellsCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.EnableCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.DisableFilteredByCellsCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [Serializable]
    public class SetCallPreviewDialingModeEventParameters : SelectedCallsEventParameters
    {
        public SetCallPreviewDialingModeEventParameters() : base() { }

        public SetCallPreviewDialingModeEventParameters(IEnumerable<int> interviewIds)
            : base(interviewIds)
        {
            InterviewIds = interviewIds.ToArray();
        }

        public int[] InterviewIds { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.ChangeDialModeOfSelectedInterviews)]
    public class ChangeDialModeOfSelectedInterviewsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.ChangeDialModeOfInterviews.Parameters>
    {
        public ChangeDialModeOfSelectedInterviewsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.ChangeDialModeOfInterviews.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.ChangeDialModeOfSelectedInterviews, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.ChangeDialModeOfFilteredInterviews)]
    public class ChangeDialModeOfFilteredInterviewsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.ChangeDialModeOfInterviews.Parameters>
    {
        public ChangeDialModeOfFilteredInterviewsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.ChangeDialModeOfInterviews.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.ChangeDialModeOfFilteredInterviews, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.ChangePriorityOfSelectedCalls)]
    public class ChangePriorityOfSelectedCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.ChangePriorityOfCalls.Parameters>
    {
        public ChangePriorityOfSelectedCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.ChangePriorityOfCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.ChangePriorityOfSelectedCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
        }

    [ManagementEventAttribute(ManagementEvent.ChangePriorityOfFilteredCalls)]
    public class ChangePriorityOfFilteredCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.ChangePriorityOfCalls.Parameters>
    {
        public ChangePriorityOfFilteredCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.ChangePriorityOfCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.ChangePriorityOfFilteredCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.ChangePriorityOfFilteredByCellsCalls)]
    public class ChangePriorityOfFilteredByCellsCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.ChangePriorityOfCalls.Parameters>
    {
        public ChangePriorityOfFilteredByCellsCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.ChangePriorityOfCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.ChangePriorityOfFilteredByCellsCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.ChangeShiftTypeOfSelectedCalls)]
    public class ChangeShiftTypeOfSelectedCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.ChangeShiftTypeOfCalls.Parameters>
    {
        public ChangeShiftTypeOfSelectedCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.ChangeShiftTypeOfCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.ChangeShiftTypeOfSelectedCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.ChangeShiftTypeOfFilteredCalls)]
    public class ChangeShiftTypeOfFilteredCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.ChangeShiftTypeOfCalls.Parameters>
    {
        public ChangeShiftTypeOfFilteredCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.ChangeShiftTypeOfCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.ChangeShiftTypeOfFilteredCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.AssignSelectedCalls)]
    public class AssignSelectedCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.AssignCalls.Parameters>
    {
        public AssignSelectedCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.AssignCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.AssignSelectedCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.AssignFilteredCalls)]
    public class AssignFilteredCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.AssignCalls.Parameters>
    {
        public AssignFilteredCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.AssignCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.AssignFilteredCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.MoveAndRescheduleSelectedCalls)]
    public class MoveAndRescheduleSelectedCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.MoveAndRescheduleCalls.Parameters>
    {
        public MoveAndRescheduleSelectedCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.MoveAndRescheduleCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.MoveAndRescheduleSelectedCalls, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.MoveAndRescheduleFilteredCalls)]
    public class MoveAndRescheduleFilteredCallsEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.CallsManagementOperations.MoveAndRescheduleCalls.Parameters>
    {
        public MoveAndRescheduleFilteredCallsEvent(int surveyId, string surveyName, AsyncOperations.Operations.CallsManagementOperations.MoveAndRescheduleCalls.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.MoveAndRescheduleFilteredCalls, parameters, entity)
    {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

}
