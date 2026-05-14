using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confirmit.CATI.Backend;
using Confirmit.CATI.Backend.WebApiServices;
using Confirmit.CATI.Backend.WebApiServices.Fakes;
using Confirmit.CATI.Backend.WebApiServices.Logging;
using Confirmit.CATI.Backend.WebApiServices.Logging.Fakes;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.ServiceRegistration;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.REST.SDK.Client;
using Confirmit.CATI.REST.SDK.Exceptions;
using Confirmit.CATI.REST.SDK.Model;
using Confirmit.CATI.REST.SDK.Services;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.RestApi
{
    [TestClass]
    public class TelephoneBlackListTests
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private IDisposable _webApiHost;
        private RestClient _client;
        private BlacklistService _blacklistService;

        

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize(false);

            var serviceLocator = new ServiceLocator();

            var serviceRegistryInitializer = new ServicesRegistryInitializer(serviceLocator);
            serviceRegistryInitializer.RegisterRegistries(new IServiceLocatorRegistry[]
            {
                new BackendServiceRegistry(),
            });

            ServiceLocator.RegisterInstance<IAuthorizationKeyProvider>(new StubIAuthorizationKeyProvider());
            ServiceLocator.RegisterInstance<IRestApiMonitorLogger>(new StubIRestApiMonitorLogger());

            var resolver = (IServiceResolver)serviceLocator;

            _webApiHost = WebApp.Start<Startup>(url: $"http://*/catiapi/companies/{BackendInstance.Current.CompanyId}");

            _client = new RestClient("http://localhost/", null, "", BackendInstance.Current.CompanyId);
            _blacklistService = new BlacklistService(_client);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();

            _webApiHost.Dispose();
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public async Task RestApi_BlackListService_GetNumberById()
        {
           new TestData
            {
                Surveys = new[]
                {
                    new SurveyData { Tag="S1"}
                },
                TelephoneBlacklist = new[] { "111*", "1112*", "11112" },
            }.Create();

           var blacklistItem = await _blacklistService.GetAsync(1);
           AssertTelephoneBlackListItem(
               new TelephoneBlacklistItem {TelephoneNumber = "111", Type = BlacklistPatternType.StartWith},
               blacklistItem);
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public async Task RestApi_BlackListService_GetFiltered()
        {
           new TestData
            {
                Surveys = new[]
                {
                    new SurveyData { Tag="S1"}
                },
                TelephoneBlacklist = new[] { "111*", "1112*", "11112" },
            }.Create();

           var blacklistItems = await _blacklistService.GetAsync("$filter=Type eq 'Equal'");
           Assert.AreEqual(1,blacklistItems.Count);
           AssertTelephoneBlackListItem(
               new TelephoneBlacklistItem {TelephoneNumber = "11112", Type = BlacklistPatternType.Equal},
               blacklistItems[0]);

           blacklistItems = await _blacklistService.GetAsync("$filter=Type eq '1'");
           Assert.AreEqual(2, blacklistItems.Count);
           AssertTelephoneBlackListItem(
               new TelephoneBlacklistItem {TelephoneNumber = "111", Type = BlacklistPatternType.StartWith},
               blacklistItems[0]);
           AssertTelephoneBlackListItem(
               new TelephoneBlacklistItem {TelephoneNumber = "1112", Type = BlacklistPatternType.StartWith},
               blacklistItems[1]);
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public async Task RestApi_BlackListService_ImportNumbers()
        {
           new TestData
            {
                Surveys = new[]
                {
                    new SurveyData { Tag="S1"}
                }
            }.Create();

           //import correct items and check added items
           await _blacklistService.ImportAsync(new TelephoneBlacklistItems()
           {
               Items = new List<TelephoneBlacklistItem>
               {
                   new TelephoneBlacklistItem {TelephoneNumber = "123", Type = BlacklistPatternType.StartWith},
                   new TelephoneBlacklistItem {TelephoneNumber = "99999999", Type = BlacklistPatternType.Equal}
               }
           });

           var blacklistItem1 = await _blacklistService.GetAsync(1);
           AssertTelephoneBlackListItem(
               new TelephoneBlacklistItem {TelephoneNumber = "123", Type = BlacklistPatternType.StartWith},
               blacklistItem1);

           var blacklistItem2 = await _blacklistService.GetAsync(2);
           AssertTelephoneBlackListItem(
               new TelephoneBlacklistItem {TelephoneNumber = "99999999", Type = BlacklistPatternType.Equal},
               blacklistItem2);

            var allItems = await _blacklistService.GetAsync("");
            Assert.AreEqual(2, allItems.Count);

            //try to add 2 items with wrong telephone numbers and check that wasn't added any items
            var isBadRequestExceptionOccurs = false;
            try
            {
                await _blacklistService.ImportAsync(new TelephoneBlacklistItems()
                {
                    Items = new List<TelephoneBlacklistItem>
                    {
                        new TelephoneBlacklistItem {TelephoneNumber = "1234a", Type = BlacklistPatternType.StartWith},
                        new TelephoneBlacklistItem {TelephoneNumber = "9999b", Type = BlacklistPatternType.Equal}
                    }
                });
            }
            catch (BadRequestException ex)
            {
                Assert.IsTrue(ex.Content.Contains("Incorrect list of the telephone blacklist items"));
                isBadRequestExceptionOccurs = true;
            }
            Assert.IsTrue(isBadRequestExceptionOccurs);

            allItems = await _blacklistService.GetAsync("");
            Assert.AreEqual(2, allItems.Count);

            //add 2 items, one of them with wrong number, check that was added only 1 item
            await _blacklistService.ImportAsync(new TelephoneBlacklistItems
            {
                Items = new List<TelephoneBlacklistItem>
                {
                    new TelephoneBlacklistItem {TelephoneNumber = "12345", Type = BlacklistPatternType.StartWith},
                    new TelephoneBlacklistItem {TelephoneNumber = "99997b", Type = BlacklistPatternType.Equal}
                }
            });
            allItems = await _blacklistService.GetAsync("");
            Assert.AreEqual(3, allItems.Count);

            Assert.AreEqual(1, allItems.Count(x => x.TelephoneNumber == "12345" && x.Type == BlacklistPatternType.StartWith));

        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public async Task RestApi_BlackListService_AddNumber()
        {
           new TestData
            {
                Surveys = new[]
                {
                    new SurveyData { Tag="S1"}
                },
                TelephoneBlacklist = new[] { "111*", "1112*", "11112" },
            }.Create();

           var newBlackListNumberId = await _blacklistService.AddAsync(new TelephoneBlacklistItem()
               {TelephoneNumber = "123123123", Type = BlacklistPatternType.Equal});

           var blacklistItem = await _blacklistService.GetAsync(newBlackListNumberId);

           AssertTelephoneBlackListItem(
               new TelephoneBlacklistItem {TelephoneNumber = "123123123", Type = BlacklistPatternType.Equal},
               blacklistItem);

           //try to add item with wrong number
           var isBadRequestExceptionOccurs = false;
           try
           {
               await _blacklistService.AddAsync(new TelephoneBlacklistItem
                   { TelephoneNumber = "12312312a", Type = BlacklistPatternType.Equal });
           }
           catch (BadRequestException ex)
           {
               Assert.IsTrue(ex.Content.Contains("Wrong telephone number or blacklist pattern type"));
               isBadRequestExceptionOccurs = true;
           }

           Assert.IsTrue(isBadRequestExceptionOccurs);
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public async Task RestApi_BlackListService_UpdateNumber()
        {
           new TestData
            {
                Surveys = new[]
                {
                    new SurveyData { Tag="S1"}
                },
                TelephoneBlacklist = new[] { "111*", "1112*", "11112" },
            }.Create();

           await _blacklistService.PutAsync(1, new TelephoneBlacklistItem()
           {
               Id = 1, TelephoneNumber = "123", Type = BlacklistPatternType.StartWith
           });
           var blacklistItem = await _blacklistService.GetAsync(1);

           AssertTelephoneBlackListItem(
               new TelephoneBlacklistItem {TelephoneNumber = "123", Type = BlacklistPatternType.StartWith},
               blacklistItem);

           //try to update item with wrong number
           var isBadRequestExceptionOccurs = false;
           try
           {
               await _blacklistService.PutAsync(1, new TelephoneBlacklistItem
                   { Id = 1, TelephoneNumber = "12312312a", Type = BlacklistPatternType.Equal });
           }
           catch (BadRequestException ex)
           {
               Assert.IsTrue(ex.Content.Contains("Wrong telephone number or blacklist pattern type"));
               isBadRequestExceptionOccurs = true;
           }

           Assert.IsTrue(isBadRequestExceptionOccurs);
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public async Task RestApi_BlackListService_DeleteNumber()
        {
           new TestData
            {
                Surveys = new[]
                {
                    new SurveyData { Tag="S1"}
                },
                TelephoneBlacklist = new[] { "111*", "1112*", "11112" },
            }.Create();

           await _blacklistService.DeleteAsync(1);

           var isNotFoundExceptionOccurs = false;
           try
           {
               await _blacklistService.GetAsync(1);
           }
           catch (NotFoundException ex)
           {
               Assert.IsTrue(ex.Content.Contains("Telephone blacklist item is not found"));
               isNotFoundExceptionOccurs = true;
           }

           Assert.IsTrue(isNotFoundExceptionOccurs);
        }

        private void AssertTelephoneBlackListItem(TelephoneBlacklistItem expectedItem,
            TelephoneBlacklistItem actualBlacklistItem)
        {
            Assert.AreEqual(expectedItem.TelephoneNumber, actualBlacklistItem.TelephoneNumber);
            Assert.AreEqual(expectedItem.Type, actualBlacklistItem.Type);
        }
    }
}
