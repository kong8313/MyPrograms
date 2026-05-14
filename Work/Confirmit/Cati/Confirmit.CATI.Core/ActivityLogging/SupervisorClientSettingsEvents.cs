using System;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [Serializable]
    public class CallManagementCustomViewParameters : ManagementActivityEventDetails
    {
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public string Columns { get; set; }
    }

    [ManagementEvent(ManagementEvent.AddCallManagementCustomView)]
    public class AddCallManagementCustomViewEvent : ManagementActivityEvent<CallManagementCustomViewParameters>
    {
        public AddCallManagementCustomViewEvent(int surveyId, string projectId, string name, bool isDefault, string columns):
            base(ManagementEventCategory.CallManagementUI, ManagementEvent.AddCallManagementCustomView)
        {
            ObjectId = surveyId;
            ObjectName = projectId;
            Details = new CallManagementCustomViewParameters { Name = name, IsDefault = isDefault, Columns = columns };
        }
    }

    [ManagementEvent(ManagementEvent.EditCallManagementCustomView)]
    public class EditCallManagementCustomViewEvent : ManagementActivityEvent<CallManagementCustomViewParameters>
    {
        public EditCallManagementCustomViewEvent(int surveyId, string projectId, string name, bool isDefault, string columns):
            base(ManagementEventCategory.CallManagementUI, ManagementEvent.EditCallManagementCustomView)
        {
            ObjectId = surveyId;
            ObjectName = projectId;
            Details = new CallManagementCustomViewParameters { Name = name, IsDefault = isDefault, Columns = columns };
        }
    }

    [ManagementEvent(ManagementEvent.DeleteCallManagementCustomView)]
    public class DeleteCallManagementCustomViewEvent : ManagementActivityEvent<CallManagementCustomViewParameters>
    {
        public DeleteCallManagementCustomViewEvent(int surveyId, string projectId, string name, bool isDefault):
            base(ManagementEventCategory.CallManagementUI, ManagementEvent.DeleteCallManagementCustomView)
        {
            ObjectId = surveyId;
            ObjectName = projectId;
            Details = new CallManagementCustomViewParameters { Name = name, IsDefault = isDefault };
        }
    }

    [Serializable]
    public class RedirectToLoginPageParameters : ManagementActivityEventDetails
    {
        public string RequestUrl { get; set; }
        public string RedirectUrl { get; set; }
        public string Referrer { get; set; }
        public string UserHostAddress { get; set; }
        public bool IsSecureConnection { get; set; }
    }

    [ManagementEvent(ManagementEvent.RedirectToLoginPage)]
    public class RedirectToLoginPageEvent : ManagementActivityEvent<RedirectToLoginPageParameters>
    {
        public RedirectToLoginPageEvent() :
            base(ManagementEventCategory.View, ManagementEvent.RedirectToLoginPage, "Unknown")
        {
        }
    }
}