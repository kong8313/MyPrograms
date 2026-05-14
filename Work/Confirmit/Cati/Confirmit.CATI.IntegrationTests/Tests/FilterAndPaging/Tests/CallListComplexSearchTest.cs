using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.FilterServiceImplementation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Repositories;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using System.Data;
using System.Linq;
using Confirmit.CATI.IntegrationTests.Framework.Data;

namespace Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tests
{
    [TestClass]
    public class CallListComplexSearchTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private int _timezoneId;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();

            _timezoneId = ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId();

            _framework.SetTestHttpContextCurrentWithSupervisorPrincipal();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.ClearTestHttpContextCurrent();

            new SqlObjectCreator(_framework).CleanTablesInSurveyDatabase(_framework.TestSurveyDatabaseName);

            _framework.TestCleanup();
        }

        private List<string> FilterPhones(string filterValue, int surveyId, SearchOperator searchOperator = SearchOperator.Like)
        {
            var search = new SearchParameterCollection
            {
                new SearchParameter
                {
                    ColumnName = "TelephoneNumber",
                    ColumnType = SearchColumnType.Text,
                    Operator = searchOperator,
                    Value = filterValue
                }
            };
            
            var args = new PagingArgs(1, 100, "InterviewID", true, search);

            var result = CallHelper.GetCallsPage(surveyId, null, _timezoneId, CallStates.All,
                args, out var totalCount, ShowTimeMode.Interviewer, false);
            
            return (from DataRow row in result.Rows select row.Field<string>("TelephoneNumber")).ToList();
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetCallsPage_FilterBySeveralTelephoneNumbers_CorrectCallsAreReturned()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsUseDb = true,
                        Interviews = new[]
                        {
                            new InterviewData { Tag = "S1.I1", Call = new CallData(), TelephoneNumber = "1" },     
                            new InterviewData { Tag = "S1.I2", Call = new CallData(), TelephoneNumber = "2" },     
                            new InterviewData { Tag = "S1.I3", Call = new CallData(), TelephoneNumber = "3" },
                            new InterviewData { Tag = "S1.I4", Call = new CallData(), TelephoneNumber = "11" },
                            new InterviewData { Tag = "S1.I5", Call = new CallData(), TelephoneNumber = "22" },
                            new InterviewData { Tag = "S1.I6", Call = new CallData(), TelephoneNumber = "33" },
                            new InterviewData { Tag = "S1.I7", Call = new CallData(), TelephoneNumber = "123" },
                            new InterviewData { Tag = "S1.I8", Call = new CallData(), TelephoneNumber = "1234" },
                            new InterviewData { Tag = "S1.I9", Call = new CallData(), TelephoneNumber = "234" },
                            new InterviewData { Tag = "S1.I10", Call = new CallData(), TelephoneNumber = "2345" },
                            new InterviewData { Tag = "S1.I11", Call = new CallData(),  TelephoneNumber = null },
                            new InterviewData { Tag = "S1.I12", Call = new CallData(),  TelephoneNumber = string.Empty }
                        }
                    }
                }
            }.Create();

            var surveyId = context.GetSurvey("S1").Id;
            
            // Several strict values
            var phones = FilterPhones("\"1\",\"2\"  ,  \"22\"", surveyId);
            CollectionAssert.AreEquivalent(new List<string> {"1", "2", "22"}, phones);
            
            // Several "start from" values
            phones = FilterPhones("11, 234", surveyId);
            CollectionAssert.AreEquivalent(new List<string> {"11", "234", "2345"}, phones);
            
            // Several "start from" and strict values
            phones = FilterPhones("123, \"234\"", surveyId);
            CollectionAssert.AreEquivalent(new List<string> {"123", "1234", "234"}, phones);
            
            // Some "start from" value without some strict values
            phones = FilterPhones("1, !\"11\", !\"123\"", surveyId);
            CollectionAssert.AreEquivalent(new List<string> {"1", "1234"}, phones);
            
            // Some "start from" value without some "start from" values
            phones = FilterPhones("1, !123", surveyId);
            CollectionAssert.AreEquivalent(new List<string> {"1", "11"}, phones);
                
            // Some "contains" value without some "contains" values
            phones = FilterPhones("*1, !*2", surveyId);
            CollectionAssert.AreEquivalent(new List<string> {"1", "11"}, phones);
            
            // Get empty values only 
            phones = FilterPhones("\"\"", surveyId, SearchOperator.IsNullOrEmpty);
            CollectionAssert.AreEquivalent(new List<string> { null, "" }, phones);
            
            // Get not empty values 
            phones = FilterPhones("!\"\"", surveyId);
            CollectionAssert.AreEquivalent(new List<string> { "1", "2", "3", "11", "22", "33", "123", "1234", "234", "2345" }, phones);
        }
        
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetCallsPage_FilterBySeveralTelephoneNumbers_UseExistedFilter_CorrectCallsAreReturned()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsUseDb = true,
                        Interviews = new[]
                        {
                            new InterviewData { Tag = "S1.I1", Call = new CallData(), LastCallPerson = "P1", TelephoneNumber = "12" },     
                            new InterviewData { Tag = "S1.I2", Call = new CallData(), LastCallPerson = "P2", TelephoneNumber = "123" },     
                            new InterviewData { Tag = "S1.I3", Call = new CallData(), LastCallPerson = "P3", TelephoneNumber = "1234" },
                            new InterviewData { Tag = "S1.I4", Call = new CallData(), LastCallPerson = "P4", TelephoneNumber = "12345" }
                        }
                    }
                },
                Persons = new[]
                {
                    new PersonData { Name = "user1", Tag = "P1" }, 
                    new PersonData { Name = "user11", Tag = "P2" },
                    new PersonData { Name = "user112", Tag = "P3" }, 
                    new PersonData { Name = "user1113", Tag = "P4" } 
                }
            }.Create();

            var filter = new BvFiltersEntity
            {
                Name = Guid.NewGuid().ToString(),
                AndOrOperator = (byte)AndOrOperator.And
            };
            var filterId = FilterRepository.Insert(filter);

            var fields = new List<BvFilterFieldsEntity>
            {
                new BvFilterFieldsEntity
                    {
                        Column = "Name",
                        Table = (int) TableTypes.Person,
                        Type = (int) VariableTypes.String,
                        Sign = (int) FilterOperator.Like,
                        Value = "user11"
                    }
            };
            FilterService.SetFields(filterId, fields);

            var search = new SearchParameterCollection
            {
                new SearchParameter
                {
                    ColumnName = "TelephoneNumber",
                    ColumnType = SearchColumnType.Text,
                    Operator = SearchOperator.Like,
                    Value = "!\"1234\""
                }
            };
            var args = new PagingArgs(1, 100, "InterviewID", true, search);

            var result = CallHelper.GetCallsPage(context.GetSurvey("S1").Id, filterId, _timezoneId, CallStates.All,
                args, out var totalCount, ShowTimeMode.Interviewer, false);

            var persons = (from DataRow row in result.Rows select (string)row["LastInterviewerName"]).ToList();
            CollectionAssert.AreEquivalent(new List<string> {"user11", "user1113"}, persons);
        }
    }
}
