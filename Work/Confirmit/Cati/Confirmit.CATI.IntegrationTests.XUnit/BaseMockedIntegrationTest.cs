using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;

namespace Confirmit.CATI.IntegrationTests.XUnit
{
    /// <summary>
    /// Base class that suits most of the integration tests.
    /// It initializes and deinitializes IntegrationTestingFramework.
    /// </summary>
    public class BaseMockedIntegrationTest : IDisposable
    {
        /// <summary>
        /// Shortcut for IntegrationTestingFramework.Instance.
        /// </summary>
        protected IntegrationTestingFramework TestingFramework => IntegrationTestingFramework.Instance;

        protected BackendTools BackendToolsObject { get; }
        protected FilterAndPagingTools FilterAndPagingToolsObject { get; private set; }
        protected FusionLibTestTools FusionLibTools { get; private set; }
        protected PredictiveTools PredictiveToolsObject { get; private set; }
            
        private readonly ISchedulingScriptSettings _schedulingScriptSettings;
            
        protected BaseMockedIntegrationTest()
        {
            TestingFramework.TestInitialize();
            TestingFramework.BackendInitialize();
            BackendToolsObject = new BackendTools(TestingFramework);
            FilterAndPagingToolsObject = new FilterAndPagingTools(TestingFramework, BackendToolsObject);
            FusionLibTools = new FusionLibTestTools(BackendToolsObject);
            PredictiveToolsObject = new PredictiveTools(BackendToolsObject); 
            _schedulingScriptSettings = ServiceLocator.Resolve<ISchedulingScriptSettings>();
        }

        protected void SetSecurityMode(SecurityMode mode)
        {
            _schedulingScriptSettings.EnableRestrictedMode = mode == SecurityMode.Restricted;
        }
        
        public virtual void Dispose()
        {
            TestingFramework.TestCleanup();
        }
    }
}
