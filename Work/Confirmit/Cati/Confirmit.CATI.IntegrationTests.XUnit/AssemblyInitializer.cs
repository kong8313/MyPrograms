using System;
using Confirmit.CATI.IntegrationTests.Framework;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true, MaxParallelThreads = 1)]

namespace Confirmit.CATI.IntegrationTests.XUnit
{
    public static class TestConstants
    {
        public const string CollectionName = "xUnitTestsCollection";
        public const string TraitName = "shard";
        public const string Trait1 = "trait1";
        public const string Trait2 = "trait2";
    }
    
    /// <summary>
    /// Class needs only to initialize framework.
    /// Ctor of AssemblyInitialize is called before all tests executed.
    /// Method Dispose is called after all tests finished.
    /// </summary>
    public class AssemblyInitializer : IDisposable
    {
        public AssemblyInitializer()
        {
            IntegrationTestingFramework.Instance.FrameworkInitialize();            
        }

        public void Dispose()
        {
            IntegrationTestingFramework.Instance.FrameworkCleanup();
        }
    }

    [CollectionDefinition(TestConstants.CollectionName)]
    public class MyCollectionDefinition : ICollectionFixture<AssemblyInitializer>
    {
        // Nothing needed here
    }
}