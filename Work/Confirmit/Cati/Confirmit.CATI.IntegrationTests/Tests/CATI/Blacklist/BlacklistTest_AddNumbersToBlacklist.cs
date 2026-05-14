using System;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Core.BlackList;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI.Blacklist
{
    [TestClass]
    public class BlacklistTest_AddNumbersToBlacklist : BaseMockedIntegrationTest
    {
        private IBlackListService _blackListService;
        private ITelephoneBlacklistRepository _telephoneBlacklistRepository;
        
        [TestInitialize]
        public void Initialize()
        {     
            base.TestInitialize();
            var registrar = ServiceLocator.Resolve<IServiceRegistrator>();
            registrar.Register<IBlackListService, BlackListService>();

            _blackListService = ServiceLocator.Resolve<IBlackListService>();
            _telephoneBlacklistRepository = ServiceLocator.Resolve<ITelephoneBlacklistRepository>();
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void AddUpdateRemoveBlacklistEntries()
        {
            var repository = ServiceLocator.Resolve<ITelephoneBlacklistRepository>();

            _blackListService.AddNumber(new BvTelephoneBlacklistEntity { TelephoneNumber = "11111" });
            _blackListService.AddNumber(new BvTelephoneBlacklistEntity { TelephoneNumber = "12121" });
            _blackListService.AddNumber(new BvTelephoneBlacklistEntity { TelephoneNumber = "13131" });
            _blackListService.AddNumber(new BvTelephoneBlacklistEntity { TelephoneNumber = "22222" });
            _blackListService.AddNumber(new BvTelephoneBlacklistEntity { TelephoneNumber = "33333" });
            BackendTools.ExecuteAllAsyncOperations();

            CollectionAssert.AreEquivalent(new[] { "11111", "12121", "13131", "22222", "33333" },
                repository.GetAll().Select(x => x.TelephoneNumber).ToArray());

            _blackListService.UpdateNumber("33333", new BvTelephoneBlacklistEntity { TelephoneNumber = "44444" });

            BackendTools.ExecuteAllAsyncOperations();

            CollectionAssert.AreEquivalent(new[] { "11111", "12121", "13131", "22222", "44444" },
                repository.GetAll().Select(x => x.TelephoneNumber).ToArray());

            repository.Delete(repository.GetAll().Where(x => x.TelephoneNumber == "12121" || x.TelephoneNumber == "13131").Select(x => x.Id));

            CollectionAssert.AreEquivalent(new[] { "11111", "22222", "44444" },
                repository.GetAll().Select(x => x.TelephoneNumber).ToArray());

            repository.DeleteAll();

            Assert.AreEqual(0, repository.GetAll().Count);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void AddUpdateRemoveBlacklistEntriesWithCommentAndCreationDate()
        {
            var repository = ServiceLocator.Resolve<ITelephoneBlacklistRepository>();

            string number = "12345";
            DateTime date = DateTime.UtcNow;
            string comment = "CommentWith74SymbolsCommentWith74SymbolsCommentWith74SymbolsCommentWith74";
            _blackListService.AddNumber(new BvTelephoneBlacklistEntity { TelephoneNumber = number, Comment = comment });
            BackendTools.ExecuteAllAsyncOperations();

            var allList = repository.GetAll();
            Assert.AreEqual(1, allList.Count);
            Assert.AreEqual(number, allList[0].TelephoneNumber);
            Assert.IsTrue((allList[0].Timestamp - date).Milliseconds < 3000);
            Assert.AreEqual(comment, allList[0].Comment);

            // Change Timestamp to check that _blackListService.UpdateNumber function sets the current utc time
            allList[0].Timestamp = date.AddDays(-1);
            _telephoneBlacklistRepository.Update(allList[0]);
            
            string newNumber = "54321";
            DateTime newDate = DateTime.UtcNow;
            string newComment = "DifferentComment";
            _blackListService.UpdateNumber(number, new BvTelephoneBlacklistEntity { TelephoneNumber = newNumber, Comment = newComment  });
            BackendTools.ExecuteAllAsyncOperations();

            allList = repository.GetAll();
            Assert.AreEqual(1, allList.Count);
            Assert.AreEqual(newNumber, allList[0].TelephoneNumber);
            Assert.IsTrue((allList[0].Timestamp - newDate).Milliseconds < 3000);
            Assert.AreEqual(newComment, allList[0].Comment);

            repository.Delete(new[] { allList[0].Id });
            Assert.AreEqual(0, repository.GetAll().Count);
        }
        
        [TestMethod, Owner(@"Firm\VictorR")]
        public void ImportWithStartNumber_InvalidNumbers_UserExceptionThrown()
        {
            try
            {
                _blackListService.ImportNumbers(new[] { "+2224", "111*" });
            }
            catch (UserMessageException ex)
            {
                Assert.AreEqual("The list cannot be uploaded because some numbers contain special symbols. The telephone number should only contain digits 0-9 or the * symbol (which may be placed at the end of the number). The total number length cannot be more than 255 characters.",
                    ex.Message);
                return;
            }
            Assert.Fail("User message exception expected");
        }

        private TestDataContext PrepareContext()
        {
            var context = new TestData()
            {
                Surveys = new[] {new SurveyData
                {
                    Tag = "S1", IsUseDb = true, IsSupportBlackList = true
                }},
                TelephoneBlacklist = new[] { "88001001000" }
            }.Create();

            var survey = context.GetSurvey("S1");

            var interviews = new[]
            {
                new InterviewData() {Tag = "S1.I1", TelephoneNumber = "8 (800) 100-10-10"},
                new InterviewData() {Tag = "S1.I2", TelephoneNumber = "8_800_100,10,10 (str to ignore)"},
                new InterviewData() {Tag = "S1.I3", TelephoneNumber = "88001001011"}
            };

            survey.AddSample(SchedulingMode.Simple, interviews);

            return context;
        }
    }
}
 