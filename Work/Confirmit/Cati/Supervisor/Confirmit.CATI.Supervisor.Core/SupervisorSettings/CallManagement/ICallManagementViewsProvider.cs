using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.SupervisorSettings.CallManagement
{
    public interface ICallManagementViewsProvider
    {
        List<CallManagementColumnKey> ScheduledColumnKeys { get; }

        CallManagementViews GetDefaultViews();

        CallManagementViews MergeViews(CallManagementViews defaultViews, CallManagementViews customViews);

        CallManagementViews RemoveDefaultViews(CallManagementViews callManagementViews);

        int GetViewNameIndex(string name, int customViewIndex);

        string GetTranslation(CallManagementColumnKey callManagementColumnKey);
    }
}