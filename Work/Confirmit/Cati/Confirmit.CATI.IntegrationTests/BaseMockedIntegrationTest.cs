using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Confirmit.CATI.IntegrationTests.Framework;

namespace Confirmit.CATI.IntegrationTests
{
    /// <summary>
    /// Base class that suits most of integration tests. It initialize and deinitialize IntegrationTestingFramework.
    /// </summary>    
    [TestClass]
    public class BaseMockedIntegrationTest
    {
        private readonly IntegrationTestingFramework _testingFramework = IntegrationTestingFramework.Instance;
        
        

        /// <summary>
        /// Shortcat for IntegrationTestingFramework.Instance.
        /// </summary>
        public IntegrationTestingFramework TestingFramework { get { return _testingFramework; } }

        public BackendTools BackendToolsObject { get; private set; }

        public FilterAndPagingTools FilterAndPagingToolsObject { get; private set; }

        public FusionLibTestTools FusionLibTools { get; private set; }

        [TestInitialize]
        public virtual void TestInitialize()
        {
            TestingFramework.TestInitialize();
            TestingFramework.BackendInitialize();
            BackendToolsObject = new BackendTools(TestingFramework);
            FilterAndPagingToolsObject = new FilterAndPagingTools(TestingFramework, BackendToolsObject);
            FusionLibTools = new FusionLibTestTools(BackendToolsObject);
            
            OnPostTestInitialize();
        }

        public virtual void OnPostTestInitialize()
        {
            
        }

        [TestCleanup]
        public virtual void TestCleanup()
        {
            TestingFramework.TestCleanup();
        }
    }
}
