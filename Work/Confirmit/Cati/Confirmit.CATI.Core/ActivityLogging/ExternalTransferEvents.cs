using System;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [Serializable]
    public class ExternalTransferNumberEventParameters : ManagementActivityEventDetails
    {
        public string Description { get; set; }
        public int[] AssignedSurveysIds { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.CreateExternalTransferNumber)]
    public class CreateExternalTransferNumberEvent : ManagementActivityEvent<ExternalTransferNumberEventParameters>
    {
        public CreateExternalTransferNumberEvent(string telephoneNumber, string description, int[] assignedSurveysIds):
            base(ManagementEventCategory.ExternalTransferNumber, ManagementEvent.CreateExternalTransferNumber)
        {
            ObjectName = telephoneNumber;
            Details.Description = description;
            Details.AssignedSurveysIds = assignedSurveysIds;
        }
    }

    [ManagementEventAttribute(ManagementEvent.UpdateExternalTransferNumber)]
    public class UpdateExternalTransferNumberEvent : ManagementActivityEvent<ExternalTransferNumberEventParameters>
    {
        public UpdateExternalTransferNumberEvent(int id, string telephoneNumber, string description, int[] assignedSurveysIds):
            base(ManagementEventCategory.ExternalTransferNumber, ManagementEvent.UpdateExternalTransferNumber)
        {
            ObjectId = id;
            ObjectName = telephoneNumber;
            Details.Description = description;
            Details.AssignedSurveysIds = assignedSurveysIds;
        }
    }

    [Serializable]
    public class DeleteExternalTransferNumbersParameters : ManagementActivityEventDetails
    {
        public int[] DeletedIds { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.DeleteExternalTransferNumbers)]
    public class DeleteExternalTransferNumbersEvent : ManagementActivityEvent<DeleteExternalTransferNumbersParameters>
    {
        public DeleteExternalTransferNumbersEvent(int[] deletedIds):
            base(ManagementEventCategory.ExternalTransferNumber, ManagementEvent.DeleteExternalTransferNumbers)
        {
            Details.DeletedIds = deletedIds;
        }
    }
}
