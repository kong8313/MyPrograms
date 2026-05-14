using Confirmit.CATI.Common.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.UnitTests
{
    [TestClass]
    public class ValidationHelperTest
    {
        private IInputParameterValidator _ParameterValidator = new InputParameterValidator();

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
        }

        private void ShouldBeEmail(string email)
        {
            if (!_ParameterValidator.IsValidEmail(email))
            {
                Assert.Fail(string.Format("Valid email '{0}' was not recognized.", email));
            }
        }

        private void ShouldNotBeEmail(string email)
        {
            if (_ParameterValidator.IsValidEmail(email))
            {
                Assert.Fail(string.Format("Invalid string '{0}' was recognized as valid email.", email));
            }
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void ValidEmail_IsEmail_True()
        {
            var email = "Svetlana.Tyurina@confirmit.com";
            ShouldBeEmail(email);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void OneLetterDomain_IsEmail_False()
        {
            var email = "Svetlana.Tyurina@confirmit.c";
            ShouldNotBeEmail(email);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void NumberInName_IsEmail_True()
        {
            var email = "Svetlana111.Tyurina111@confirmit.com";
            ShouldBeEmail(email);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void UnderlineIsFirst_IsEmail_False()
        {
            var email = "_Svetlana.Tyurina@confirmit.com";
            ShouldNotBeEmail(email);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void DotIsFirst_IsEmail_False()
        {
            var email = ".Svetlana.Tyurina@confirmit.com";
            ShouldNotBeEmail(email);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void NameContainsInvalidSymbol_IsEmail_False()
        {
            var email = "Svet@ana.Tyurina@confirmit.com";
            ShouldNotBeEmail(email);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void NumberIsFirst_IsEmail_True()
        {
            var email = "1Svetlana.Tyurina@confirmit.com";
            ShouldBeEmail(email);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void OneLettersInLastDomainName_IsEmail_False()
        {
            var email = "Svetlana.Tyurina@confirmit.c";
            ShouldNotBeEmail(email);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void TwoLettersInLastDomainName_IsEmail_True()
        {
            var email = "Svetlana.Tyurina@confirmit.co";
            ShouldBeEmail(email);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void ThreeLettersInLastDomainName_IsEmail_True()
        {
            var email = "Svetlana.Tyurina@confirmit.com";
            ShouldBeEmail(email);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void MoreThan6LettersInLastDomainName_IsEmail_False()
        {
            var email = "Svetlana.Tyurina@confirmit.abcdefg";
            ShouldNotBeEmail(email);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void SeveralPartsDomainName_IsEmail_True()
        {
            var email = "Svetlana.Tyurina@confirmit.confirmit1.confirmit2.com";
            ShouldBeEmail(email);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void IsStringValid_UnicodeControlCharInString_Invalid()
        {
            for (var c = (char) 0x00; c <= (char) 0x1F; c++)
            {
                var str = "assd" + c + "dd";
                Assert.IsFalse(_ParameterValidator.IsValid(str), "Character {0} is not allowed", (int)c);
            }

            for (var c = (char)0x7F; c <= (char)0x9F; c++)
            {
                var str = "assd" + c + "dd";
                Assert.IsFalse(_ParameterValidator.IsValid(str), "Character {0} is not allowed", (int)c);
            }
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void IsStringValid_UnsupportedSymbolsInString_Invalid()
        {
            var unsupportedChars = "<>&';";
            foreach (var c in unsupportedChars)
            {
                var str = "assd" + c + "dd";
                Assert.IsFalse(_ParameterValidator.IsValid(str), "Character {0} should re restricted", c);
            }
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void IsValidQuestionId_CorrectQuestionId_Valid()
        {
            var questionIds = new[] {"q12_23", "_q123", "qwerty"};
            
            foreach (var qid in questionIds)
            {
                Assert.IsTrue(_ParameterValidator.IsValidQuestionId(qid));
            }
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void IsValidQuestionId_QuestionIdStartsWithNumeric_Invalid()
        {
            var questionIds = new[] { "1question" };

            foreach (var qid in questionIds)
            {
                Assert.IsFalse(_ParameterValidator.IsValidQuestionId(qid));
            }
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void IsValidQuestionId_QuestionIdIsRestrictedWord_Invalid()
        {
            var questionIds = new[] { "con", "r", "s", "rid", "interviewer", "c", "page", "projectid", "env", "state" };

            foreach (var qid in questionIds)
            {
                Assert.IsFalse(_ParameterValidator.IsValidQuestionId(qid));
            }
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void IsValidQuestionId_QuestionIdWithIllegalCharacter_Invalid()
        {
            var questionIds = new[] {"q12!", "q$", "*q1"};

            foreach (var qid in questionIds)
            {
                Assert.IsFalse(_ParameterValidator.IsValidQuestionId(qid));
            }
        }
    }
}
