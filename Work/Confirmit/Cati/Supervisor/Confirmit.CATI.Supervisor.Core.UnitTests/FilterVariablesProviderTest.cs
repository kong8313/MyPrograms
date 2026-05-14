using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Random;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Supervisor.Core.Filters;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.CATI.Supervisor.Core.Confirmit.Fakes;
using Confirmit.CATI.Supervisor.Core.Filters.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.Core.UnitTests
{
    [TestClass]
    public class FilterVariablesProviderTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            var serviceLocator = new ServiceLocator();
            serviceLocator.Initialize();
            
            var serviceceRegistrator = ServiceLocator.Resolve<IServiceRegistrator>();
            serviceceRegistrator
                .Register<IConfirmitQuestionsProvider, ConfirmitQuestionsProvider>()
                .Register<IFilterVariablesProvider, FilterVariablesProvider>()
                .Register<IFilterManager, FilterManager>()
                .Register<IFilterRepository, FilterRepository>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ServiceLocator.StaticCleanup();
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetVariables_CallMethod_ReturnCallInterviewAndAppointmentVariables()
        {
            IConfirmitQuestionsProvider confirmitQuestionsProviderStub = new StubIConfirmitQuestionsProvider
            {
                Inner = ServiceLocator.Resolve<IConfirmitQuestionsProvider>(),
                GetReplicatedQuestionsOrderedByNameInt32 = sid => new List<VariableInfo>()
            };
            ServiceLocator.RegisterInstance(confirmitQuestionsProviderStub);

            IFilterManager filterManagerStub = new StubIFilterManager
            {
                Inner = ServiceLocator.Resolve<IFilterManager>(),
                GetFiltersInt32NullableOfInt32 = (sid, currentFilterSid) => new List<VariableInfo>()
            };
            ServiceLocator.RegisterInstance(filterManagerStub);

            var filterVariablesProvider = ServiceLocator.Resolve<IFilterVariablesProvider>();
            List<VariableInfo> result = filterVariablesProvider.GetVariables(0, null);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
            Assert.IsTrue(result.Any(x => x.TableType == TableTypes.Call));
            Assert.IsTrue(result.Any(x => x.TableType == TableTypes.Interview));
            Assert.IsTrue(result.Any(x => x.TableType == TableTypes.Appointment));
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetFilters_CallMethod_ReturnFiltersList()
        {
            List<BvFiltersEntity> filters = new List<BvFiltersEntity>();
            BvFiltersEntity filter = new BvFiltersEntity
            {
                SID = 1000,
                Name = "test"
            };
            filters.Add(filter);

            int count = Randomizer.Next(2, 50);
            for (int i = 1; i < count; i++)
            {
                BvFiltersEntity filter1 = new BvFiltersEntity
                {
                    Name = Randomizer.NextDouble().ToString(CultureInfo.InvariantCulture),
                    SID = i
                };
                filters.Add(filter1);
            }
            List<int> parentFilters = new List<int>();

            IFilterRepository filterRepositoryStub = new StubIFilterRepository
            {
                Inner = ServiceLocator.Resolve<IFilterRepository>(),
                GetFiltersListBooleanInt32 = (includeWide, sid) => filters,
                GetAllParentFiltersInt32 = sid => parentFilters
            };
            ServiceLocator.RegisterInstance(filterRepositoryStub);

            var filterManager = ServiceLocator.Resolve<IFilterManager>();
            List<VariableInfo> result = filterManager.GetFilters(0, 1000).ToList();
           
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
            Assert.IsFalse(result.Contains(new VariableInfo("test", VariableTypes.String, TableTypes.Subfilter)));
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetFilters_FiltersAreParent_ReturnEmptyList()
        {
            List<BvFiltersEntity> filters = new List<BvFiltersEntity>();
            BvFiltersEntity filter = new BvFiltersEntity
            {
                SID = 1000,
                Name = "test"
            };
            filters.Add(filter);

            int count = Randomizer.Next(2, 50);
            for (int i = 1; i < count; i++)
            {
                BvFiltersEntity filter1 = new BvFiltersEntity
                {
                    Name = Randomizer.NextDouble().ToString(CultureInfo.InvariantCulture),
                    SID = i
                };
                filters.Add(filter1);
            }
            List<int> parentFilters = filters.Select(x => x.SID).ToList();

            IFilterRepository filterRepositoryStub = new StubIFilterRepository
            {
                Inner = ServiceLocator.Resolve<IFilterRepository>(),
                GetFiltersListBooleanInt32 = (includeWide, sid) => filters,
                GetAllParentFiltersInt32 = sid => parentFilters
            };
            ServiceLocator.RegisterInstance(filterRepositoryStub);

            var filterManager = ServiceLocator.Resolve<IFilterManager>();
            List<VariableInfo> result = filterManager.GetFilters(0, 1000).ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

    }
}