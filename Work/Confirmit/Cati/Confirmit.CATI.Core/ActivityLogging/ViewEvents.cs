using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.Paging;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [Serializable]
    public class ViewCallListEventParameters : ManagementActivityEventDetails
    {
        public int? FilterId { get; set; }
        public string ShowTimeMode { get; set; }
        public PagingArgs PageArguments { get; set; }
        public string CallState { get; set; }
        public string ViewName { get; set; }
        public string[] Variables { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.ViewCallList)]
    public class ViewCallListEvent : ManagementActivityEvent<ViewCallListEventParameters>
    {
        public ViewCallListEvent(
            int surveyId, 
            string projectId, 
            int? filterId, 
            string callState, 
            string viewName, 
            PagingArgs pageArgs, 
            string shoeTimeMode, 
            IEnumerable<string> variables):
            base(ManagementEventCategory.View, ManagementEvent.ViewCallList)
        {
            ObjectId = surveyId;
            ObjectName = projectId;
            Details = new ViewCallListEventParameters
            {
                FilterId = filterId,
                CallState = callState,
                ViewName = viewName,
                PageArguments = pageArgs,
                ShowTimeMode = shoeTimeMode,
                Variables = variables.ToArray()
            };
        }
    }

    [Serializable]
    public class ViewDeferredMonitoringListEventParameters : ManagementActivityEventDetails
    {
        public string UserName { get; set; }
        public PagingArgs PageArguments { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.ViewDeferredMonitoringList)]
    public class ViewDeferredMonitoringListEvent : ManagementActivityEvent<ViewDeferredMonitoringListEventParameters>
    {
        public ViewDeferredMonitoringListEvent(string userName, PagingArgs pageArgs):
            base(ManagementEventCategory.View, ManagementEvent.ViewDeferredMonitoringList)
        {
            Details = new ViewDeferredMonitoringListEventParameters
                {
                    UserName = userName,
                    PageArguments = pageArgs
                };
        }
    }

    [Serializable]
    public class ViewQuotaEventParameters : ManagementActivityEventDetails
    {
        public string QuotaName { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.ViewQuota)]
    public class ViewQuotaEvent : ManagementActivityEvent<ViewQuotaEventParameters>
    {
        public ViewQuotaEvent(int surveyId, string projectId, string quotaName):
            base(ManagementEventCategory.View, ManagementEvent.ViewQuota)
        {
            ObjectId = surveyId;
            ObjectName = projectId;
            Details = new ViewQuotaEventParameters { QuotaName = quotaName };
        }
    }
}