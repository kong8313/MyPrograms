using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Handmade.Entity;
using Confirmit.CATI.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using DialerCommon.DialerParameters;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    public class TestAssert
    {
        public static void AreEqual(BvInterviewEntity expected, BvInterviewEntity actual)
        {
            Assert.AreEqual(expected.ID, actual.ID, "Interview ID is not as expected");
            Assert.AreEqual(expected.SurveySID, actual.SurveySID, "SurveySID is not as expected");
            Assert.AreEqual(expected.TransientState, actual.TransientState, "ITS is not as expected");
            Assert.AreEqual(expected.TimezoneID ?? 0, actual.TimezoneID ?? 0, "TZ is not as expected");
            Assert.AreEqual(expected.RespondentName, actual.RespondentName, "RespondentName is not as expected");
            Assert.AreEqual(expected.DialingMode, actual.DialingMode, "DialingMode is not as expected");
        }

        private static void AreEqualBase(BvCallEntity expected, BvCallEntity actual)
        {
            if (expected.CallID != 0)
            {
                Assert.AreEqual(expected.CallID, actual.CallID, "ApptId is incorrect");
            }

            Assert.AreEqual(expected.ApptID, actual.ApptID, "ApptId is incorrect");
            Assert.AreEqual(expected.ShiftID, actual.ShiftID, "ShiftID is incorrect");
            Assert.AreEqual(expected.InterviewID, actual.InterviewID, "InterviewID is incorrect");
            Assert.AreEqual(expected.SurveySID, actual.SurveySID, "SurveySID is incorrect");
            Assert.AreEqual(expected.CallState, actual.CallState, "CallState is incorrect");
            Assert.AreEqual(expected.Priority, actual.Priority, "Priority is incorrect");
            Assert.AreEqual(expected.RuleNumber, actual.RuleNumber, "RuleNumber is incorrect");
            Assert.AreEqual(expected.Resource, actual.Resource, "Resource is incorrect");

            if ((actual.TimeToExpire == DateTime.MinValue || actual.TimeToExpire.Value.Year >= 9999) &&
                (expected.TimeToExpire == DateTime.MinValue || expected.TimeToExpire.Value.Year >= 9999))
            {

            }
            else
            {
                Assert.AreEqual(expected.TimeToExpire, actual.TimeToExpire);
            }
        }

        public static void AreEqual(BvCallEntity expected, BvCallEntity actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual, "Here isn't expected call, but it exists");
                return;
            }
            
            Assert.IsNotNull(actual, "Call should exist");

            AreEqualBase(expected, actual);

            Assert.AreEqual(expected.TimeInShift ?? DateTime.Parse("1899-12-30T00:00:00"), actual.TimeInShift);
        }

        public static void AreEqual(BvCallEntity expected, BvCallEntity actual, DateTime timeInShiftStart, DateTime timeInShiftFinish)
        {
            AreEqualBase(expected, actual);

            Assert.IsTrue(timeInShiftStart < actual.TimeInShift);
            Assert.IsTrue(timeInShiftFinish > actual.TimeInShift);
        }

        public static void AreEqual(QuotaCellCounter expected, QuotaCellCounter actual)
        {
            Assert.AreEqual(expected.Descriptor,actual.Descriptor, "OperationDescriptor of cell is wrong");
            Assert.AreEqual(expected.Value, actual.Value, "Extra quota counter is wrong");
        }

        public static void AreEqual(DialerParameter expected, DialerParameter actual)
        {
            Assert.AreEqual(expected.Id, actual.Id, String.Format("The Id '{0}' for parameter does not not equal '{1}'", expected.Id, actual.Id));
            Assert.AreEqual(expected.Name, actual.Name, String.Format("The Name '{0}' for parameter does not not equal '{1}'", expected.Name, actual.Name));
            Assert.AreEqual(expected.Type, actual.Type, String.Format("The Type '{0}' for parameter does not not equal '{1}'", expected.Type, actual.Type));
            Assert.AreEqual(expected.Value, actual.Value, String.Format("The Value '{0}' for parameter does not not equal '{1}'", expected.Value, actual.Value));
        }

        public static void InvokeMethodAndVerifyExceptionThrown<T>(System.Action action) where T : Exception
        {
            try
            {
                action.Invoke();
            }
            catch (T)
            {
                return;
            }
            
            Assert.Fail("Exception wasn't thrown. Expected exception with type {0}", typeof(T));
        }

        /// <summary>
        /// Invokes "action" method and checks that method throws exception of type T.
        /// You also should specify custom checker for expected exception.
        /// </summary>
        /// <typeparam name="T">Exception type.</typeparam>
        /// <param name="action">Action to check.</param>
        /// <param name="checker">Custom checked which checks thrown exception.</param>
        public static void InvokeMethodAndVerifyExceptionThrown<T>(System.Action action, Action<T> checker ) where T : Exception
        {
            try
            {
                action.Invoke();
            }
            catch (T exception)
            {
                checker(exception);
                return;
            }

            Assert.Fail("Exception wasn't thrown. Expected exception with type {0}", typeof(T));
        }

        public static void AreEqual<T>(T expected, T actual)
        {
            if (expected is BvInterviewEntity)
            {
                AreEqual(expected as BvInterviewEntity, actual as BvInterviewEntity);
            }
            else if (expected is BvCallEntity)
            {
                AreEqual(expected as BvCallEntity, actual as BvCallEntity);
            }
            else if (expected is DialerParameter)
            {
                AreEqual(expected as DialerParameter, actual as DialerParameter);
            }
            else if( expected is QuotaCellCounter )
            {
                AreEqual(expected as QuotaCellCounter, actual as QuotaCellCounter);
            }
            else
            {
                Assert.AreEqual(expected, actual);
            }
        }

        public static void AreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            var expectedList = expected.ToArray();
            var actualList = actual.ToArray();

            Assert.AreEqual(expectedList.Length, actualList.Length, "Arrays have different lenght");

            for (int i = 0; i < expectedList.Length; ++i)
            {
                TestAssert.AreEqual(expectedList[i], actualList[i]);
            }
        }

        public class TestComparrer<T>
        {
            private FieldInfo[] _fields;
            private readonly PropertyInfo[] _properties;

            public TestComparrer(FieldInfo[] fields, PropertyInfo[] properties)
            {
                _fields = fields;
                _properties = properties;
            }


            public bool Compare(T x, T y)
            {
                if (_fields.Any(field => !Compare(field.GetValue(x), field.GetValue(y))))
                {
                    return false;
                }
                
                if (_properties.Any(property => !property.GetValue(x).Equals( property.GetValue(y))))
                {
                    return false;
                }

                return true;
            }

            private bool Compare(object x, object y)
            {
                return x == null ? y == null : x.Equals(y);
            }
        }

        public class TestFormatter<T>
        {
            private FieldInfo[] _fields;
            private readonly PropertyInfo[] _properties;

            public TestFormatter(FieldInfo[] fields, PropertyInfo[] properties)
            {
                _fields = fields;
                _properties = properties;
            }

            public string Format(T[] array, bool withHeader = false)
            {
                var data = new List<List<string>>();

                if (withHeader)
                {
                    data.Add(_fields.Select(x => x.Name).Union(_properties.Select(x => x.Name)).ToList());
                }

                data.AddRange(
                    _fields.Select(
                        field => new[] {field.Name}.Concat(array.Select(item => FromatValue(field.GetValue(item)))).ToList()));

                data.AddRange(
                    _properties.Select(
                        property => new[] { property.Name }.Concat(array.Select(item => FromatValue(property.GetValue(item)))).ToList()));

                var sizes = data.Select(column => column.Max(x => x.Length)).Select(l => l + 1).ToArray();

                var result = new StringBuilder();
                
                for (int r = 0; r < array.Length + 1; r++)
                {
                    result.AppendLine();
                    for (int c = 0; c < sizes.Length; c++)
                    {
                        var value = data[c][r];
                        result.Append(new string(' ', sizes[c] - value.Length));
                        result.Append(value);
                    }
                }

                return result.ToString();
            }

            private string FromatValue(object value)
            {
                if (value == null)
                    return "<NULL>";
                if (value is DateTime)
                {
                    var result = ((DateTime)value).ToString("M/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                    return result;
                }

                return value.ToString();
            }
        }

        public static TestComparrer<T> CreateComparer<T>()
        {
            var type = typeof (T);
            
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.GetField | BindingFlags.Instance );
            var proeprties = type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);

            return new TestComparrer<T>(fields, proeprties);
        }

        public static TestFormatter<T> CreateFormatter<T>()
        {
            var type = typeof(T);

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.GetField | BindingFlags.Instance);
            var proeprties = type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);

            return new TestFormatter<T>(fields, proeprties);
        }

        public static void AreEqual<TA, TC>(IEnumerable<TA> expected, IEnumerable<TA> actual, Func<TA, TC> converter)
        {
            var expectedList = expected.ToArray();
            var actualList = actual.ToArray();

            var expectedData = expectedList.Select(converter).ToArray();
            var actualData = actualList.Select(converter).ToArray();

            string reason = null;

            if (expectedData.Length != actualData.Length)
            {
                reason = String.Format( "Arrays have different length Expected:{0}, Actual:{1}", expectedData.Length, actualData.Length);
            }
            else
            {
                var comparer = CreateComparer<TC>();
                
                for (int i = 0; i < expectedData.Length; ++i)
                {
                    if (!comparer.Compare(expectedData[i], actualData[i]))
                    {
                        reason = String.Format("{0}th elements are different", i + 1);
                        break;
                    }
                }
            }
            

            if (reason != null)
            {
                var formatter = CreateFormatter<TA>();
                var message = String.Format(@"
Following collections are not equal:
Reason:{0}
Expected:
{1}
Actual:
{2}",
                reason,
                formatter.Format(expectedList),
                formatter.Format(actualList));

                Assert.Fail(message);
            }
        }

        public static void AreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, Func<T, T, bool> assertion)
        {
            var expectedList = expected.ToArray();
            var actualList = actual.ToArray();

            Assert.AreEqual(expectedList.Length, actualList.Length, "Arrays have different lenght");

            for (int i = 0; i < expectedList.Length; ++i)
            {
                if (!assertion(expectedList[i], actualList[i]))
                    Assert.Fail(String.Format("{0}th elements are different", i));
            }
        }

        public static void ManagementActivityEventExists(ManagementEvent eventType, string eventName, int objectId)
        {
            Assert.IsTrue(
                DoesEventExist(eventType, eventName, objectId), 
                "Management event {0} of type {1} was not found for object {2}", eventName, eventType, objectId);
        }

        public static void ManagementActivityEventDoesntExist(ManagementEvent eventType, string eventName,
                                                                int objectId)
        {
            Assert.IsFalse(
                DoesEventExist(eventType, eventName, objectId),
                "Management event {0} of type {1} was unexpectedly found for object {2}", eventName, eventType, objectId);
        }

        private static bool DoesEventExist(ManagementEvent eventType, string eventName, int objectId)
        {
            var sql =
                "SELECT * FROM [dbo].[CatiManagementActivity] WHERE EventTypeId = @EventTypeId AND EventTypeName = @EventTypeName AND ObjectID = @ObjectID";

            using (var connection = new SqlConnection(BackendInstance.Current.ConfirmlogConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddRange(new[]
                                                {
                                                    new SqlParameter("@EventTypeId", eventType),
                                                    new SqlParameter("@EventTypeName", eventName),
                                                    new SqlParameter("@ObjectID", objectId),
                                                });
                connection.Open();
                var reader = command.ExecuteReader();
                return reader.Read();
            }
        }

        public static void WaitCondition(Func<bool> condition, string failMessage, int timeout = 30)
        {
            DateTime deadTime = DateTime.Now + TimeSpan.FromSeconds(timeout);
            while (!condition())
            {
                if (deadTime < DateTime.Now)
                {
                    Assert.Fail(failMessage);
                }
                Thread.Sleep(10);
            }
        }
    }
}
