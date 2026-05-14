using Confirmit.Configuration.Bootstrap;
using Prometheus;

namespace Confirmit.CATI.Supervisor
{
    public static class LegacySupervisorMetrics
    {
        public static readonly CollectorRegistry Registry = Metrics.NewCustomRegistry();
        private static readonly MetricFactory Factory = Metrics.WithCustomRegistry(Registry);

        private static readonly Counter PageView = Factory.CreateCounter(
            "cati_legacysupervisor_page_view_count",
            "Total number of page views in legacy supervisor",
            new CounterConfiguration()
            {
                LabelNames = new[] { "page" },
            });

        private static readonly Counter CallManagementAction = Factory.CreateCounter(
            "cati_legacysupervisor_callmanagement_action_count",
            "Total number of call management menu actions invoked by supervisors",
            new CounterConfiguration()
            {
                LabelNames = new[] { "action" },
            });

        public static void OnPageView(string page)
        {
            if (!BootstrapConfig.IsContainerEnvironment)
                return;

            PageView.WithLabels(page).Inc();
        }

        public static void OnCallManagementAction(string action)
        {
            if (!BootstrapConfig.IsContainerEnvironment)
                return;

            CallManagementAction.WithLabels(action).Inc();
        }
    }
}
