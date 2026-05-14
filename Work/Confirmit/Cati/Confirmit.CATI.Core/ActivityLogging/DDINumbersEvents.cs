using System;


namespace Confirmit.CATI.Core.ActivityLogging
{
    [ManagementEventAttribute(ManagementEvent.AddDdiNumber)]
    public class AddDdiNumberEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public AddDdiNumberEvent(string number):
            base(ManagementEventCategory.DdiNumber, ManagementEvent.AddDdiNumber)
        {
            ObjectName = number;
        }
    }

    [ManagementEventAttribute(ManagementEvent.UpdateDdiNumber)]
    public class UpdateDdiNumberEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public UpdateDdiNumberEvent(int surveyId, string number):
            base(ManagementEventCategory.DdiNumber, ManagementEvent.UpdateDdiNumber)
        {
            ObjectId = surveyId;
            ObjectName = number;
        }
    }

    [Serializable]
    public class DeleteDdiNumbersEventParameters : ManagementActivityEventDetails
    {
        public int DeletedNumbersCount { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.DeleteDdiNumbers)]
    public class DeleteDdiNumberEvent : ManagementActivityEvent<DeleteDdiNumbersEventParameters>
    {
        public DeleteDdiNumberEvent(int deletedNumbersCount):
            base(ManagementEventCategory.DdiNumber, ManagementEvent.DeleteDdiNumbers)
        {
            Details = new DeleteDdiNumbersEventParameters { DeletedNumbersCount = deletedNumbersCount };
        }
    }
}