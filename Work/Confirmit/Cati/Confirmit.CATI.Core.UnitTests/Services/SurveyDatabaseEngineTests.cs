using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Services
{
    [TestClass]
    public class SurveyDatabaseEngineTests : BaseTest
    {
        private ISurveyConnectionStringProvider _surveyConnectionStringProvider;
        private IRetryingServiceSettings _retryingServiceSettings;
        private IAsyncManager _asyncManager;

        private SurveyDatabaseEngine _surveyDatabaseEngine;

        [TestInitialize]
        public override void TestInitialize()
        {
            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();

            _surveyConnectionStringProvider = ServiceLocator.Resolve<ISurveyConnectionStringProvider>();
            _retryingServiceSettings = ServiceLocator.Resolve<IRetryingServiceSettings>();
            _asyncManager = ServiceLocator.Resolve<IAsyncManager>();

            _retryingServiceSettings.DelayBetweenRetriesInMilliseconds = 100;
            _retryingServiceSettings.NumberOfRetryAttempts = 3;

            _surveyDatabaseEngine = new SurveyDatabaseEngine(
                _surveyConnectionStringProvider,
                _retryingServiceSettings,
                _asyncManager);
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }
        [TestMethod]
        public void RetryOnDeadlock_ShouldRetryOnDeadlock()
        {
            int executionCount = 0;

            void ActionUnderTest()
            {
                executionCount++;
                if (executionCount <= 5)
                {
                    throw CreateSqlException(1205); // Deadlock error code
                }
            }

            AssertExtensions.Throws<SqlException>(() =>
            {
                _surveyDatabaseEngine.RetryOnDeadlock("Test Deadlock", ActionUnderTest);
            });

            Assert.AreEqual(3, executionCount, "The action should be retried exactly 3 times.");
        }

        [TestMethod]
        public void RetryOnDeadlock_ShouldRetryOnTimeout()
        {
            int executionCount = 0;

            void ActionUnderTest()
            {
                executionCount++;
                if (executionCount <= 3)
                {
                    throw CreateSqlException(-2); // Timeout error code
                }
            }

            AssertExtensions.Throws<SqlException>(() =>
            {
                _surveyDatabaseEngine.RetryOnDeadlock("Test Timeout", ActionUnderTest);
            });

            Assert.AreEqual(3, executionCount, "The action should be retried exactly 3 times.");
        }

        [TestMethod]
        public void RetryOnDeadlock_ShouldNotRetryOnNonRetryableError()
        {
            int executionCount = 0;

            void ActionUnderTest()
            {
                executionCount++;
                throw CreateSqlException(12345); // Non-retryable error code
            }

            AssertExtensions.Throws<Exception>(() =>
            {
                _surveyDatabaseEngine.RetryOnDeadlock("Test Non-Retryable Error", ActionUnderTest);
            });

            Assert.AreEqual(1, executionCount, "The action should not be retried for non-retryable errors.");
        }

        public static SqlException CreateSqlException(int errorNumber = 1234, string errorMessage = "Test SQL exception")
        {
            var errorCollectionType = typeof(SqlException).Assembly.GetType("System.Data.SqlClient.SqlErrorCollection");

            var errorCollection = Activator.CreateInstance(errorCollectionType, nonPublic: true);

            var sqlErrorType = typeof(SqlException).Assembly.GetType("System.Data.SqlClient.SqlError");
            var sqlErrorCtor = sqlErrorType.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new Type[]
                {
                typeof(int),
                typeof(byte),
                typeof(byte),
                typeof(string),
                typeof(string),
                typeof(string),
                typeof(int)
                },
                null);

            var sqlError = sqlErrorCtor.Invoke(new object[]
            {
            errorNumber,
            (byte)0,
            (byte)0,
            "ServerName",
            errorMessage,
            "ProcedureName",
            1
            });

            var addMethod = errorCollectionType.GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance);
            addMethod.Invoke(errorCollection, new[] { sqlError });

            var sqlExceptionCtor = typeof(SqlException).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new Type[]
                {
                typeof(string),
                errorCollectionType,
                typeof(Exception),
                typeof(Guid)
                },
                null);

            return (SqlException)sqlExceptionCtor.Invoke(new object[]
            {
            errorMessage,
            errorCollection,
            null,
            Guid.NewGuid()
            });
        }
    }

    public static class AssertExtensions
    {
        /// <summary>
        /// Verifies that a specific exception is thrown by the provided action.
        /// </summary>
        /// <typeparam name="T">The type of the expected exception.</typeparam>
        /// <param name="func">The action that is expected to throw the exception.</param>
        /// <param name="assertions">Optional assertions on the thrown exception.</param>
        public static void Throws<T>(Action func, Action<T> assertions = null) where T : Exception
        {
            try
            {
                func.Invoke();
            }
            catch (T ex)
            {
                // Perform additional assertions on the exception, if provided
                assertions?.Invoke(ex);
                return;
            }
            catch (Exception ex)
            {
                throw new AssertFailedException(
                    $"An exception of type {typeof(T)} was expected, but {ex.GetType()} was thrown instead.\nException message: {ex.Message}");
            }

            throw new AssertFailedException(
                $"An exception of type {typeof(T)} was expected, but no exception was thrown.");
        }
    }
}
