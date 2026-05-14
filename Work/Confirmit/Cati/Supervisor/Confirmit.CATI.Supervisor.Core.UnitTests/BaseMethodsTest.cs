using System;
using System.Linq;
using Confirmit.Test.Common.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Core.Paging;
using System.Collections.Generic;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Core.UnitTests;

namespace Confirmit.CATI.Supervisor.Core.UnitTests
{
    [TestClass]
    public class BaseMethodsTest : BaseTest
    {
        // RegEx metacharecters (excetp '*')
        private static readonly string[] Metacharacters = { @"\", "+", "?", "|", "{", @"[", @"(", ")", @"^", @"$", @".", @"#" };

        #region Nested types

        internal class Test
        {
            #region Properties

            public int PropertyInt32
            {
                get;
                set;
            }

            public double PropertyDouble
            {
                get;
                set;
            }

            public DateTime PropertyDateTime
            {
                get;
                set;
            }

            public string PropertyString
            {
                get;
                set;
            }

            public bool PropertyBoolean
            {
                get;
                set;
            }

            #endregion

            #region Methods

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                if (obj.GetType() != GetType())
                {
                    return false;
                }

                Test test = (Test)obj;
                return (PropertyInt32 == test.PropertyInt32 &&
                    Math.Abs(PropertyDouble - test.PropertyDouble) < 0.001 &&
                    PropertyDateTime == test.PropertyDateTime &&
                    PropertyString == test.PropertyString);
            }

            public override int GetHashCode()
            {
                return PropertyInt32 ^ PropertyDouble.GetHashCode() ^ PropertyDateTime.GetHashCode() ^ PropertyString.GetHashCode();
            }
            #endregion
        }

        #endregion

        #region Utility methods

        private bool Check(List<Test> first, List<Test> second)
        {
            bool result = true;

            if (first.Count != second.Count)
            {
                result = false;
            }
            else
            {
                for (int i = 0; i < first.Count; i++)
                {
                    if (!Equals(first.ElementAt(i), second.ElementAt(i)))
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        private List<Test> PrepareCollection()
        {
            var testColl = new List<Test>
            {
                /*0*/
                new Test
                {
                    PropertyInt32 = 10,
                    PropertyDouble = 9.8,
                    PropertyDateTime = new DateTime(2009, 3, 10, 14, 59, 23, DateTimeKind.Utc),
                    PropertyString = "Q gvfdllkm   fddkl'k     kkk "
                },
                /*1*/
                new Test
                {
                    PropertyInt32 = 67,
                    PropertyDouble = -647.832,
                    PropertyDateTime = new DateTime(2009, 3, 11, 0, 50, 02, DateTimeKind.Utc),
                    PropertyString = "342"
                },
                /*2*/
                new Test
                {
                    PropertyInt32 = -433,
                    PropertyDouble = 3432,
                    PropertyDateTime = new DateTime(2009, 3, 11, 10, 10, 10, DateTimeKind.Utc),
                    PropertyString = "dfasa1"
                },
                /*3*/
                new Test
                {
                    PropertyInt32 = -1,
                    PropertyDouble = 3421.11,
                    PropertyDateTime = new DateTime(2009, 3, 11, 20, 59, 23, DateTimeKind.Utc),
                    PropertyString = "Date",
                    PropertyBoolean = true
                },
                /*4*/
                new Test
                {
                    PropertyInt32 = -1,
                    PropertyDouble = 3421.11,
                    PropertyDateTime = new DateTime(2009, 3, 11, 23, 0, 0, DateTimeKind.Utc),
                    PropertyString = "text1",
                },
                /*5*/
                new Test
                {
                    PropertyInt32 = -1,
                    PropertyDouble = 3421.11,
                    PropertyDateTime = new DateTime(2009, 3, 12, 14, 0, 0, DateTimeKind.Utc),
                    PropertyString = "text2",
                },
                /*6*/
                new Test
                {
                    PropertyInt32 = -1,
                    PropertyDouble = 3421.11,
                    PropertyDateTime = new DateTime(2009, 3, 13, 14, 59, 23, DateTimeKind.Utc),
                    PropertyString = "text3",
                },
                /*7*/
                new Test
                {
                    PropertyInt32 = -1,
                    PropertyDouble = 3421.11,
                    PropertyDateTime = new DateTime(2019, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                    PropertyString = null,
                }
            };

            return testColl;
        }
        #endregion

        #region BaseMethods.FilterCollection tests

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FilterCollection_CollectionIsNull_ThrowArgumentNullException()
        {
            BaseMethods.FilterCollection<int>(null, new SearchParameterCollection());
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FilterCollection_ConditionsAreNull_ThrowArgumentNullException()
        {
            BaseMethods.FilterCollection(new int[0], null);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void FilterCollection_ConditionAreEmpty_CollectionIsNotChanged()
        {
            List<Test> testColl = PrepareCollection();

            List<Test> test = BaseMethods.FilterCollection(testColl, new SearchParameterCollection());

            Assert.IsTrue(Check(testColl, test), "Collections are not equal");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void FilterCollection_NumberValueIsGreater_Success()
        {
            List<Test> test = PrepareCollection();
            SearchParameterCollection search = new SearchParameterCollection
            {
                new SearchParameter()
                {
                    ColumnName = "PropertyInt32",
                    ColumnType = SearchColumnType.Number,
                    Operator = SearchOperator.Greater,
                    Value = 20
                }
            };

            List<Test> expected = new List<Test> {test[1]};

            List<Test> actual = BaseMethods.FilterCollection(test, search);

            Assert.IsTrue(Check(expected, actual), "Collections are not equal");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void FilterCollection_DecimalIsLessOrEqual_Success()
        {
            List<Test> test = PrepareCollection();
            SearchParameterCollection search = new SearchParameterCollection
            {
                new SearchParameter()
                {
                    ColumnName = "PropertyDouble",
                    ColumnType = SearchColumnType.Decimal,
                    Operator = SearchOperator.LessThanOrEqual,
                    Value = -647.832
                }
            };

            List<Test> expected = new List<Test> {test[1]};

            List<Test> actual = BaseMethods.FilterCollection(test, search);

            Assert.IsTrue(Check(expected, actual), "Collections are not equal");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void FilterCollection_DateIsNotEqual_Success()
        {
            List<Test> test = PrepareCollection();
            SearchParameterCollection search = new SearchParameterCollection
            {
                new SearchParameter()
                {
                    ColumnName = "PropertyDateTime",
                    ColumnType = SearchColumnType.DateTime,
                    Operator = SearchOperator.NotEqual,
                    Value = new DateTime(1990, 12, 11, 0, 0, 0)
                }
            };

            List<Test> expected = test;

            List<Test> actual = BaseMethods.FilterCollection(test, search);

            Assert.IsTrue(Check(expected, actual), "Collections are not equal");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void FilterCollection_DateEqual_Success()
        {
            List<Test> test = PrepareCollection();
            SearchParameterCollection search = new SearchParameterCollection
            {
                new SearchParameter()
                {
                    ColumnName = "PropertyDateTime",
                    ColumnType = SearchColumnType.DateTime,
                    Operator = SearchOperator.Equal,
                    Value = new DateTime(2009, 3, 11, 10, 10, 10)
                }
            };

            List<Test> expected = new List<Test> {test[2]};

            List<Test> actual = BaseMethods.FilterCollection(test, search);

            Assert.IsTrue(Check(expected, actual), "Collections are not equal");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void FilterCollection_StringAndDecimalAreEqual_EmptyCollection()
        {
            List<Test> test = PrepareCollection();
            SearchParameterCollection search = new SearchParameterCollection
            {
                new SearchParameter()
                {
                    ColumnName = "PropertyDouble",
                    ColumnType = SearchColumnType.Decimal,
                    Operator = SearchOperator.NotEqual,
                    Value = (double) 11
                },
                new SearchParameter
                {
                    ColumnName = "PropertyString",
                    ColumnType = SearchColumnType.Text,
                    Operator = SearchOperator.Equal,
                    Value = "Value"
                }
            };

            List<Test> actual = BaseMethods.FilterCollection(test, search);

            Assert.IsTrue(Check(new List<Test>(), actual), "Collections are not equal");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void FilterCondition_StringLikeWithSimplePattern_Success()
        {
            List<Test> test = PrepareCollection();
            SearchParameterCollection search = new SearchParameterCollection
            {
                new SearchParameter()
                {
                    ColumnName = "PropertyString",
                    ColumnType = SearchColumnType.Text,
                    Operator = SearchOperator.Like,
                    Value = "d"
                }
            };

            List<Test> expected = new List<Test> {test[2], test[3]};

            List<Test> actual = BaseMethods.FilterCollection(test, search);

            Assert.IsTrue(Check(expected, actual), "Collections are not equal");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void FilterCondition_StringCaseInsensitive_Success()
        {
            List<Test> test = PrepareCollection();
            SearchParameterCollection search = new SearchParameterCollection
            {
                new SearchParameter()
                {
                    ColumnName = "PropertyString",
                    ColumnType = SearchColumnType.Text,
                    Operator = SearchOperator.Equal,
                    Value = "dATE"
                }
            };

            List<Test> expected = new List<Test> {test[3]};

            List<Test> actual = BaseMethods.FilterCollection(test, search);

            Assert.IsTrue(Check(expected, actual), "Collections are not equal");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void FilterCondition_StringLikeWithAsteriskPattern_Success()
        {
            List<Test> test = PrepareCollection();
            SearchParameterCollection search = new SearchParameterCollection
            {
                new SearchParameter()
                {
                    ColumnName = "PropertyString",
                    ColumnType = SearchColumnType.Text,
                    Operator = SearchOperator.Like,
                    Value = "Q*kk"
                }
            };

            List<Test> expected = new List<Test> {test[0]};

            List<Test> actual = BaseMethods.FilterCollection(test, search);

            Assert.IsTrue(Check(expected, actual), "Collections are not equal");
        }

        [TestMethod, Owner(@"FIRM\AlexanderM"), Bug(38178)]
        public void FilterCondition_MetacharactersInPatternAndData_Success()
        {
            foreach(string metachar in Metacharacters)
            {
                var testList = new List<Test> { new Test { PropertyString = metachar + "aaa"} };

                var search = new SearchParameterCollection
                                 {
                                     new SearchParameter
                                         {
                                             ColumnName = "PropertyString",
                                             ColumnType = SearchColumnType.Text,
                                             Operator = SearchOperator.Like,
                                             Value = metachar
                                         }
                                 };

                List<Test> actual = BaseMethods.FilterCollection(testList, search);

                Assert.IsTrue(Check(testList, actual), "Collections are not equal");
            }
        }

        [TestMethod, Owner(@"FIRM\AlexanderM"), Bug(38178)]
        public void FilterCondition_MetacharactersInPatternOnly_EmptyResult()
        {
            foreach (string metachar in Metacharacters)
            {
                var testList = new List<Test> { new Test { PropertyString = "aaa" } };

                var search = new SearchParameterCollection
                                 {
                                     new SearchParameter
                                         {
                                             ColumnName = "PropertyString",
                                             ColumnType = SearchColumnType.Text,
                                             Operator = SearchOperator.Like,
                                             Value = metachar
                                         }
                                 };

                List<Test> actual = BaseMethods.FilterCollection(testList, search);
                Assert.AreEqual(0, actual.Count);
            }
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void FilterCondition_StringLikeCaseInsensitive_Success()
        {
            List<Test> test = PrepareCollection();
            SearchParameterCollection search = new SearchParameterCollection
            {
                new SearchParameter()
                {
                    ColumnName = "PropertyString",
                    ColumnType = SearchColumnType.Text,
                    Operator = SearchOperator.Like,
                    Value = "TeXT1"
                }
            };

            List<Test> expected = new List<Test> {test[4]};

            List<Test> actual = BaseMethods.FilterCollection(test, search);

            Assert.IsTrue(Check(expected, actual), "Collections are not equal");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void FilterCondition_BooleanValueEqual_Success()
        {
            List<Test> test = PrepareCollection();
            SearchParameterCollection search = new SearchParameterCollection
            {
                new SearchParameter()
                {
                    ColumnName = "PropertyBoolean",
                    ColumnType = SearchColumnType.Number,
                    Operator = SearchOperator.Equal,
                    Value = 1
                }
            };

            List<Test> expected = new List<Test> {test[3]};

            List<Test> actual = BaseMethods.FilterCollection(test, search);

            Assert.IsTrue(Check(expected, actual), "Collections are not equal");
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void FilterCondition_TesxNullValueEqual_Success()
        {
            List<Test> test = PrepareCollection();
            SearchParameterCollection search = new SearchParameterCollection
            {
                new SearchParameter()
                {
                    ColumnName = "PropertyString",
                    ColumnType = SearchColumnType.Text,
                    Operator = SearchOperator.Equal,
                    Value = null
                }
            };

            List<Test> expected = new List<Test> {test[7]};

            List<Test> actual = BaseMethods.FilterCollection(test, search);

            Assert.IsTrue(Check(expected, actual), "Collections are not equal");
        }

        #endregion

        #region BaseMethod.GetPage with timezone

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void GetPageWithTimezone_DateEqual_Success()
        {
            List<Test> test = PrepareCollection(); // in UTC
            var search = new SearchParameterCollection
            {
                new SearchParameter
                {
                    ColumnName = "PropertyDateTime",
                    ColumnType = SearchColumnType.DateTime,
                    Operator = SearchOperator.Equal,
                    Value = DateTime.Parse("2009-03-11T00:00:00Z") // in 16 TZ
                }
            };

            List<Test> expected = new List<Test> {test[1], test[2], test[3]};

            int totalCount;
            var args = new PagingArgs(1, 10, "PropertyDateTime", true, search);

            BvTimezoneEntity timezone = new BvTimezoneEntity
            {
                Bias = -180,
                DaylightBias = -60,
                DaylightDayOfWeek = 0,
                DaylightName = "Russian Daylight Time",
                DaylightStart = DateTime.Parse("2000-03-05T02:00:00.000Z"),
                DaylightType = 2,
                ID = 16,
                Name = "(GMT+03:00) Moscow, St. Petersburg, Volgograd",
                StandardBias = 0,
                StandardDayOfWeek = 0,
                StandardName = "Russian Standard Time",
                StandardStart = DateTime.Parse("2000-10-05T03:00:00.000Z")
            };

            ITimezoneRepository timezoneRepository = new StubITimezoneRepository 
            {
                Inner = ServiceLocator.Resolve<ITimezoneRepository>(),
                GetInt32 = sid => timezone
            };
            ServiceLocator.RegisterInstance(timezoneRepository);

            List<Test> actual = BaseMethods.GetPage(test, args, 16, out totalCount); // TimezoneID=16 - GMT+3

            Assert.IsTrue(Check(expected, actual), "Collections are not equal");
        }
        
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void GetPageWithTimezone_DateLess_Success()
        {
            List<Test> test = PrepareCollection(); // in UTC
            var search = new SearchParameterCollection
            {
                new SearchParameter
                {
                    ColumnName = "PropertyDateTime",
                    ColumnType = SearchColumnType.DateTime,
                    Operator = SearchOperator.Less,
                    Value = DateTime.Parse("2009-03-12T00:00:00Z") // in 16 TZ
                }
            };

            List<Test> expected = new List<Test> {test[0], test[1], test[2], test[3]};

            int totalCount;
            var args = new PagingArgs(1, 10, "PropertyDateTime", true, search);

            BvTimezoneEntity timezone = new BvTimezoneEntity
            {
                Bias = -180,
                DaylightBias = -60,
                DaylightDayOfWeek = 0,
                DaylightName = "Russian Daylight Time",
                DaylightStart = DateTime.Parse("2000-03-05T02:00:00.000Z"),
                DaylightType = 2,
                ID = 16,
                Name = "(GMT+03:00) Moscow, St. Petersburg, Volgograd",
                StandardBias = 0,
                StandardDayOfWeek = 0,
                StandardName = "Russian Standard Time",
                StandardStart = DateTime.Parse("2000-10-05T03:00:00.000Z")
            };

            ITimezoneRepository timezoneRepository = new StubITimezoneRepository 
            {
                GetInt32 = sid => timezone
            };
            ServiceLocator.RegisterInstance(timezoneRepository);

            List<Test> actual = BaseMethods.GetPage(test, args, 16, out totalCount); // TimezoneID=16 - GMT+3

            Assert.IsTrue(Check(expected, actual), "Collections are not equal");
        }
        
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void GetPageWithTimezone_DateLessOrEqual_Success()
        {
            List<Test> test = PrepareCollection(); // in UTC
            var search = new SearchParameterCollection
            {
                new SearchParameter
                {
                    ColumnName = "PropertyDateTime",
                    ColumnType = SearchColumnType.DateTime,
                    Operator = SearchOperator.LessThanOrEqual,
                    Value = DateTime.Parse("2009-03-12T00:00:00Z") // in 16 TZ
                }
            };

            List<Test> expected = new List<Test> {test[0], test[1], test[2], test[3], test[4], test[5]};

            int totalCount;
            var args = new PagingArgs(1, 10, "PropertyDateTime", true, search);

            BvTimezoneEntity timezone = new BvTimezoneEntity
            {
                Bias = -180,
                DaylightBias = -60,
                DaylightDayOfWeek = 0,
                DaylightName = "Russian Daylight Time",
                DaylightStart = DateTime.Parse("2000-03-05T02:00:00.000Z"),
                DaylightType = 2,
                ID = 16,
                Name = "(GMT+03:00) Moscow, St. Petersburg, Volgograd",
                StandardBias = 0,
                StandardDayOfWeek = 0,
                StandardName = "Russian Standard Time",
                StandardStart = DateTime.Parse("2000-10-05T03:00:00.000Z")
            };

            ITimezoneRepository timezoneRepository = new StubITimezoneRepository 
            {
                GetInt32 = sid => timezone
            };
            ServiceLocator.RegisterInstance(timezoneRepository);

            List<Test> actual = BaseMethods.GetPage(test, args, 16, out totalCount); // TimezoneID=16 - GMT+3

            Assert.IsTrue(Check(expected, actual), "Collections are not equal");
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void GetPageWithTimezone_DateGreater_Success()
        {
            List<Test> test = PrepareCollection(); // in UTC
            var search = new SearchParameterCollection
            {
                new SearchParameter
                {
                    ColumnName = "PropertyDateTime",
                    ColumnType = SearchColumnType.DateTime,
                    Operator = SearchOperator.Greater,
                    Value = DateTime.Parse("2009-03-12T00:00:00Z") // in 16 TZ
                }
            };

            List<Test> expected = new List<Test> {test[6], test[7]};

            int totalCount;
            var args = new PagingArgs(1, 10, "PropertyDateTime", true, search);
            BvTimezoneEntity timezone = new BvTimezoneEntity
                {
                    Bias = -180,
                    DaylightBias = -60,
                    DaylightDayOfWeek = 0,
                    DaylightName = "Russian Daylight Time",
                    DaylightStart = DateTime.Parse("2000-03-05T02:00:00.000Z"),
                    DaylightType = 2,
                    ID = 16,
                    Name = "(GMT+03:00) Moscow, St. Petersburg, Volgograd",
                    StandardBias = 0,
                    StandardDayOfWeek = 0,
                    StandardName = "Russian Standard Time",
                    StandardStart = DateTime.Parse("2000-10-05T03:00:00.000Z")
                };

            ITimezoneRepository timezoneRepository = new StubITimezoneRepository 
            {
                Inner = ServiceLocator.Resolve<ITimezoneRepository>(),
                GetInt32 = sid => timezone
            };
            ServiceLocator.RegisterInstance(timezoneRepository);

            List<Test> actual = BaseMethods.GetPage(test, args, timezone.ID, out totalCount); // TimezoneID=16 - GMT+3

            Assert.IsTrue(Check(expected, actual), "Collections are not equal");
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void GetPageWithTimezone_DateGreaterOrEqual_Success()
        {
            List<Test> test = PrepareCollection(); // in UTC
            var search = new SearchParameterCollection
            {
                new SearchParameter
                {
                    ColumnName = "PropertyDateTime",
                    ColumnType = SearchColumnType.DateTime,
                    Operator = SearchOperator.GreaterThanOrEqual,
                    Value = DateTime.Parse("2009-03-12T00:00:00Z") // in 16 TZ
                }
            };

            List<Test> expected = new List<Test> {test[4], test[5], test[6], test[7]};

            int totalCount;
            var args = new PagingArgs(1, 10, "PropertyDateTime", true, search);

            BvTimezoneEntity timezone = new BvTimezoneEntity
            {
                Bias = -180,
                DaylightBias = -60,
                DaylightDayOfWeek = 0,
                DaylightName = "Russian Daylight Time",
                DaylightStart = DateTime.Parse("2000-03-05T02:00:00.000Z"),
                DaylightType = 2,
                ID = 16,
                Name = "(GMT+03:00) Moscow, St. Petersburg, Volgograd",
                StandardBias = 0,
                StandardDayOfWeek = 0,
                StandardName = "Russian Standard Time",
                StandardStart = DateTime.Parse("2000-10-05T03:00:00.000Z")
            };

            ITimezoneRepository timezoneRepository = new StubITimezoneRepository 
            {
                GetInt32 = sid => timezone
            };
            ServiceLocator.RegisterInstance(timezoneRepository);

            List<Test> actual = BaseMethods.GetPage(test, args, timezone.ID, out totalCount); // TimezoneID=16 - GMT+3

            Assert.IsTrue(Check(expected, actual), "Collections are not equal");
        }
        #endregion


        [TestMethod, Owner(@"FIRM/SergeyC")]
        public void GetPage_GivenPageIndexIsGreaterThanNumberOfPagesInCollection_LastPageIsReturned()
        {
            List<Test> list = PrepareCollection();

            var pageSize = 5;
            var pagingArgs = new PagingArgs(100, pageSize, "PropertyInt32", true);

            var numberOfPagesInTheList = (list.Count/pageSize) + (list.Count%pageSize == 0 ? 0 : 1);
            var expected = list.OrderBy(item => item.PropertyInt32).Skip((numberOfPagesInTheList - 1)*pageSize).ToList();

            int totalCount;
            var actual = BaseMethods.GetPage(list, pagingArgs, out totalCount);

            Assert.IsTrue(Check(expected, actual), "Collections are not equal");
        }

        [TestMethod, Owner(@"FIRM/SergeyC")]
        public void GetPage_GivenPageIndexAndEmptyCollection_EmptyCollectionIsReturned()
        {
            List<Test> list = new List<Test>();

            var pageSize = 5;
            var pagingArgs = new PagingArgs(100, pageSize, "PropertyInt32", true);

            int totalCount;
            var actual = BaseMethods.GetPage(list, pagingArgs, out totalCount);

            Assert.IsFalse(actual.Any(), "Collections should empty");
        }

        [TestMethod, Owner(@"FIRM/SergeyC")]
        public void GetPage_GivenPageIndexIsGreaterThanNumberOfPagesInCollectionWithFiltration_FirstPageWithSingleItemIsReturned()
        {
            List<Test> list = PrepareCollection();

            var pageSize = 5;
            var searching = new SearchParameterCollection
                           {
                               new SearchParameter
                               {
                                   ColumnName = "PropertyInt32",
                                   ColumnType = SearchColumnType.Number,
                                   Operator = SearchOperator.Equal,
                                   Value = 67
                               }
                           };
            var pagingArgs = new PagingArgs(100, pageSize, "PropertyInt32", true, searching);

            var expected = list.Where(item => item.PropertyInt32 == 67).ToList();

            int totalCount;
            var actual = BaseMethods.GetPage(list, pagingArgs, out totalCount);

            Assert.IsTrue(Check(expected, actual), "Collections are not equal");
        }

        [TestMethod, Owner(@"FIRM/SergeyC")]
        public void GetPageMultiSorting_GivenPageIndexAndEmptyCollection_EmptyCollectionIsReturned()
        {
            List<Test> list = new List<Test>();

            var pageSize = 5;
            var pagingArgs = new MultiSortPagingArgs(
                100, 
                pageSize,
                new SortingArgsCollection {new SortingArgs("PropertyInt32", true)});

            int totalCount;
            var actual = BaseMethods.GetPage(list, pagingArgs, out totalCount);

            Assert.IsFalse(actual.Any(), "Collections should empty");
        }

        [TestMethod, Owner(@"FIRM/SergeyC")]
        public void GetPageMultiSorting_GivenPageIndexIsGreaterThanNumberOfPagesInCollection_LastPageIsReturned()
        {
            List<Test> list = PrepareCollection();

            var pageSize = 5;
            var pagingArgs = new MultiSortPagingArgs(
                100,
                pageSize,
                new SortingArgsCollection { new SortingArgs("PropertyInt32", true) });

            var numberOfPagesInTheList = (list.Count / pageSize) + (list.Count % pageSize == 0 ? 0 : 1);
            var expected = list.OrderBy(item => item.PropertyInt32).Skip((numberOfPagesInTheList - 1) * pageSize).ToList();

            int totalCount;
            var actual = BaseMethods.GetPage(list, pagingArgs, out totalCount);

            Assert.IsTrue(Check(expected, actual), "Collections are not equal");
        }

        [TestMethod, Owner(@"FIRM/SergeyC")]
        public void GetPageMultiSorting_GivenPageIndexIsGreaterThanNumberOfPagesInCollectionWithFiltration_FirstPageWithSingleItemIsReturned()
        {
            List<Test> list = PrepareCollection();

            var pageSize = 5;
            var searching = new SearchParameterCollection
                           {
                               new SearchParameter
                               {
                                   ColumnName = "PropertyInt32",
                                   ColumnType = SearchColumnType.Number,
                                   Operator = SearchOperator.Equal,
                                   Value = 67
                               }
                           };
            var pagingArgs = new MultiSortPagingArgs(100, pageSize, new SortingArgsCollection{ new SortingArgs("PropertyInt32", true)}, searching);

            var expected = list.Where(item => item.PropertyInt32 == 67).ToList();

            int totalCount;
            var actual = BaseMethods.GetPage(list, pagingArgs, out totalCount);

            Assert.IsTrue(Check(expected, actual), "Collections are not equal");
        }
    }
}
