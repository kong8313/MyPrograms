using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Core.Activity;

namespace Confirmit.CATI.IntegrationTests.Framework.Data.Builders
{
    public class AlertDataBuilder : BaseObjectBuilder<BaseAlertData>
    {
        public AlertDataBuilder(TestDataContext context, BaseAlertData data, DataGenerator dataGenerator)
            : base(context, data, dataGenerator)
        {
        }

        public override void Create()
        {
        }

        public override void Setup()
        {
            var esData = Data as ExtendedStatusAlertData;
            if (esData != null)
            {
                ActivityManager.SetStatusAlert(new StatusAlertInfo(0, Data.Amber, Data.Red, (int)esData.ITS, string.Format("Alert status for {0}", esData.ITS)));
            }
            var aData = Data as AlertData;
            if (aData != null)
            {
                ActivityManager.SetAlert(new SurveyAlertInfo(0, Data.Amber, Data.Red, (int)aData.Type));
            }
        }
    }
}