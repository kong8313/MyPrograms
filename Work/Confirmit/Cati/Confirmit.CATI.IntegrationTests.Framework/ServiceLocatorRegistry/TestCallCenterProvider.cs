using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Supervisor.Core.CallCenters;

namespace Confirmit.CATI.IntegrationTests.Framework.ServiceLocatorRegistry
{
    public class TestCallCenterProvider : ICallCenterProvider
    {
        public int CurrentId
        {
            get; set;
        }

        public int GetCurrentId()
        {
            return CurrentId;
        }

        public BvCallCenterEntity GetCurrent()
        {
            return new BvCallCenterEntity
                       {
                           ID = 1,
                           Name = "Default",
                           IsDefault = true,
                           CanBeDeleted = false,
                           LocalTimezoneId = 1
                       };
        }

        public TestCallCenterProvider()
        {
            CurrentId = 1;
        }
    }
}
