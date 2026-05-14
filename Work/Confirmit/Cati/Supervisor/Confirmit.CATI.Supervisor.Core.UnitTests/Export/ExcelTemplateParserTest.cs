using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Supervisor.Core.Export.Parse;

namespace Confirmit.CATI.Supervisor.Core.UnitTests.Export
{
    /// <summary>
    /// Summary description for Export
    /// </summary>
    [TestClass]
    public class ExcelTemplateParserTest
    {
        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseCellMarkup_EmptyLexeme_ArgumentException()
        {
            string markup = "";
            LexemeInfo lexemeInfo = ExcelTemplateParser.ParseCellMarkup(markup).Single();            
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void ParseCellMarkup_InvalidValidOneLexeme_SucessZeroObject()
        {
            string markup = "xxx"; 
            LexemeInfo[] lexemeInfos = ExcelTemplateParser.ParseCellMarkup(markup);
            Assert.AreEqual(0, lexemeInfos.Count());

            markup = "<%xxx.InterviewID%>";
            lexemeInfos = ExcelTemplateParser.ParseCellMarkup(markup);
            Assert.AreEqual(0, lexemeInfos.Count());

            markup = "<%Recourses.InterviewID, HorizontalDetails%>";
            lexemeInfos = ExcelTemplateParser.ParseCellMarkup(markup);
            Assert.AreEqual(0, lexemeInfos.Count());           
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void ParseCellMarkup_ValidOneLexeme_SuccessOneObject()
        {
            string markup = "<% Data.InterviewID %>";
            LexemeInfo[] lexemeInfos = ExcelTemplateParser.ParseCellMarkup(markup);

            Assert.AreEqual(1, lexemeInfos.Count());
            Assert.AreEqual(LexemeType.Data, lexemeInfos[0].Type);
            Assert.AreEqual("InterviewID", lexemeInfos[0].Value);

            markup = "<%Resources.InterviewID%>";
            lexemeInfos = ExcelTemplateParser.ParseCellMarkup(markup);

            Assert.AreEqual(1, lexemeInfos.Count());
            Assert.AreEqual(LexemeType.Resources, lexemeInfos[0].Type);
            Assert.AreEqual("InterviewID", lexemeInfos[0].Value);
        }       

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void ParseCellMarkup_ValidTwoLexeme_Success()
        {            
            string markup = "<%Data.InterviewID, HorizontalDetails%>";

            LexemeInfo[] lexemeInfos = ExcelTemplateParser.ParseCellMarkup(markup);

            Assert.AreEqual(2, lexemeInfos.Count());
            Assert.AreEqual(LexemeType.Data, lexemeInfos[0].Type);
            Assert.AreEqual("InterviewID", lexemeInfos[0].Value);

            Assert.AreEqual(LexemeType.HorizontalDetails, lexemeInfos[1].Type);
            Assert.AreEqual("", lexemeInfos[1].Value);
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void ParseCellMarkup_ValidParamsLexeme_Success()
        {
            string markup = "<%Params.MyParamName%>";

            LexemeInfo[] lexemeInfos = ExcelTemplateParser.ParseCellMarkup(markup);

            Assert.AreEqual(1, lexemeInfos.Count());
            Assert.AreEqual(LexemeType.Params, lexemeInfos[0].Type);
            Assert.AreEqual("MyParamName", lexemeInfos[0].Value);            
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void ParseCellMarkup_ValidTwoLexemeWithStyle_Success()
        {
            string markup = "<%Data.InterviewID, HorizontalDetails( style = A1 )%>";

            LexemeInfo[] lexemeInfos = ExcelTemplateParser.ParseCellMarkup(markup);

            Assert.AreEqual(2, lexemeInfos.Count());
            Assert.AreEqual(LexemeType.Data, lexemeInfos[0].Type);
            Assert.AreEqual("InterviewID", lexemeInfos[0].Value);

            Assert.AreEqual(LexemeType.HorizontalDetails, lexemeInfos[1].Type);
            Assert.AreEqual("A1", lexemeInfos[1].Value);
            
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void ParseCellMarkup_ValidDateLexem_Success()
        {
            string markup = "<%Date%>";

            LexemeInfo[] lexemeInfos = ExcelTemplateParser.ParseCellMarkup(markup);

            Assert.AreEqual(1, lexemeInfos.Count());
            Assert.AreEqual(LexemeType.Date, lexemeInfos[0].Type);
            Assert.AreEqual("", lexemeInfos[0].Value);            

        }
    }
}
