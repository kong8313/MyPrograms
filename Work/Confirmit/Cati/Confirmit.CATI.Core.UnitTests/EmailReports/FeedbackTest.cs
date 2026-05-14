using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Mail.Feedback;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.SystemSettings.Fakes;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.EmailReports
{
    [TestClass]
    public class FeedbackTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();

            var settings = new StubIEmailSettings
            {
                AdministratorEmailAddressGet = () => "test@firmsw.no" ,
                NotificationEmailBCCGet = () => "bccsample@firmsw.no",
                FeedbackSupportEmailAddressGet = () => "support@firmsw.no"
            };

            ServiceLocator.RegisterInstance<IEmailSettings>(settings);
            ServiceLocator.Register<IFeedbackMessageCreator, FeedbackMessageCreator>();

            _feedbackMessageCreator = ServiceLocator.Resolve<IFeedbackMessageCreator>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }

        private IFeedbackMessageCreator _feedbackMessageCreator;

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void GetMailMessage_CategorySuggestion_Addressee_ShouldBeOne()
        {
            var form = new FeedbackForm
            {
                Category = FeedbackCategory.Suggestion
            };

            var actual = _feedbackMessageCreator.GetMailMessage(form);

            Assert.AreEqual(1, actual.To.Count);
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void GetMailMessage_CategoryBug_Addressee_ShouldBeTwo()
        {
            var form = new FeedbackForm
            {
                Category = FeedbackCategory.Bug
            };

            var actual = _feedbackMessageCreator.GetMailMessage(form);

            Assert.AreEqual(2, actual.To.Count);
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void GetMailMessage_CategoryBug_MailBody_MandatoryFieldsOnly()
        {
            var form = new FeedbackForm
            {
                Category = FeedbackCategory.Bug,
                Description = "test123"
            };

            var actual = _feedbackMessageCreator.GetMailMessage(form).Body.ToLower();

            Assert.IsTrue(actual.Contains("category"));
            Assert.IsTrue(actual.Contains("bug"));

            Assert.IsTrue(actual.Contains("description"));
            Assert.IsTrue(actual.Contains(form.Description));
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void GetMailMessage_CategorySuggestion_MailBody_MandatoryFieldsOnly()
        {
            var form = new FeedbackForm
            {
                Category = FeedbackCategory.Suggestion,
                Description = "test123"
            };

            var actual = _feedbackMessageCreator.GetMailMessage(form).Body.ToLower();

            Assert.IsTrue(actual.Contains("category"));
            Assert.IsTrue(actual.Contains("suggestion"));

            Assert.IsTrue(actual.Contains("description"));
            Assert.IsTrue(actual.Contains(form.Description));
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void GetMailMessage_CategorySuggestion_MailBody_MandatoryFieldsAndName()
        {
            var form = new FeedbackForm
            {
                Category = FeedbackCategory.Suggestion,
                Description = "test123",
                ContactName = "user456"
            };

            var actual = _feedbackMessageCreator.GetMailMessage(form).Body.ToLower();

            Assert.IsTrue(actual.Contains("contact name"));
            Assert.IsTrue(actual.Contains(form.ContactName));
        }
        
        [TestMethod, Owner(@"FIRM\KirillV")]
        public void GetMailMessage_CategorySuggestion_MailBody_MandatoryFieldsAndEmail()
        {
            var form = new FeedbackForm
            {
                Category = FeedbackCategory.Suggestion,
                Description = "test123",
                ContactEmail = "user456@firmsw.no"
            };

            var actual = _feedbackMessageCreator.GetMailMessage(form).Body.ToLower();

            Assert.IsTrue(actual.Contains("contact email"));
            Assert.IsTrue(actual.Contains(form.ContactEmail));
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void GetMailMessage_CategorySuggestion_MailBody_MandatoryFieldsAndNameAndEmail()
        {
            var form = new FeedbackForm
            {
                Category = FeedbackCategory.Suggestion,
                Description = "test123",
                ContactName = "user789",
                ContactEmail = "user456@firmsw.no"
            };

            var actual = _feedbackMessageCreator.GetMailMessage(form).Body.ToLower();

            Assert.IsTrue(actual.Contains("contact email"));
            Assert.IsTrue(actual.Contains(form.ContactEmail));
            Assert.IsTrue(actual.Contains("contact name"));
            Assert.IsTrue(actual.Contains(form.ContactName));
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void GetMailMessage_CategorySuggestion_MailBody_Company()
        {
            var form = new FeedbackForm
            {
                CompanyName = "confirmit",
                CompanyId = 100500
            };

            var actual = _feedbackMessageCreator.GetMailMessage(form).Body.ToLower();

            Assert.IsTrue(actual.Contains("company"));
            Assert.IsTrue(actual.Contains(form.CompanyName));
            Assert.IsTrue(actual.Contains("100500"));
        }
    }
}