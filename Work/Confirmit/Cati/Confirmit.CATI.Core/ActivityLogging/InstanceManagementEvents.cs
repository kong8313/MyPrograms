using System;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [ManagementEventAttribute(ManagementEvent.CreateMultimodeInstance)]
    public class CreateMultimodeInstanceEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public CreateMultimodeInstanceEvent(int companyId):
            base(ManagementEventCategory.System, ManagementEvent.CreateMultimodeInstance)
        {
            CompanyId = companyId;
            ObjectId = companyId;
            ObjectName = companyId.ToString();
        }
    }

    [ManagementEventAttribute(ManagementEvent.DeleteMultimodeInstance)]
    public class DeleteMultimodeInstanceEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public DeleteMultimodeInstanceEvent(int companyId):
            base(ManagementEventCategory.System, ManagementEvent.DeleteMultimodeInstance)
        {
            CompanyId = companyId;
            ObjectId = companyId;
            ObjectName = companyId.ToString();
        }
    }

    [Serializable]
    public class StartMultimodeInstanceEventParameters : ManagementActivityEventDetails
    {
    }

    [ManagementEventAttribute(ManagementEvent.StartMultimodeInstance)]
    public class StartMultimodeInstanceEvent : ManagementActivityEvent<StartMultimodeInstanceEventParameters>
    {
        public StartMultimodeInstanceEvent():
            base(ManagementEventCategory.System, ManagementEvent.StartMultimodeInstance)
        {
        }
    }

    [ManagementEventAttribute(ManagementEvent.StopMultimodeInstance)]
    public class StopMultimodeInstanceEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public StopMultimodeInstanceEvent():
            base(ManagementEventCategory.System, ManagementEvent.StopMultimodeInstance)
        {
        }
    }

    [ManagementEventAttribute(ManagementEvent.ResynchronizeServices)]
    public class ResynchronizeServicesEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public ResynchronizeServicesEvent():
            base(ManagementEventCategory.System, ManagementEvent.ResynchronizeServices)
        {
        }
    }
}