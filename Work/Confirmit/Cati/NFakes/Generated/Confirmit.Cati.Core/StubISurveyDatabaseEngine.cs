using System;
using Confirmit.CATI.Core.Services.Interfaces;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubISurveyDatabaseEngine : ISurveyDatabaseEngine 
    {
        private ISurveyDatabaseEngine _inner;

        public StubISurveyDatabaseEngine()
        {
            _inner = null;
        }

        public ISurveyDatabaseEngine Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetSurveySchemeInt32Delegate(int surveyId);
        public GetSurveySchemeInt32Delegate GetSurveySchemeInt32;

        string ISurveyDatabaseEngine.GetSurveyScheme(int surveyId)
        {


            if (GetSurveySchemeInt32 != null)
            {
                return GetSurveySchemeInt32(surveyId);
            } else if (_inner != null)
            {
                return ((ISurveyDatabaseEngine)_inner).GetSurveyScheme(surveyId);
            }

            return default(string);
        }

        public delegate DataTable ExecuteQueryInt32StringArrayOfSqlParameterDelegate(int surveyId, string query, SqlParameter[] parameters);
        public ExecuteQueryInt32StringArrayOfSqlParameterDelegate ExecuteQueryInt32StringArrayOfSqlParameter;

        DataTable ISurveyDatabaseEngine.ExecuteQuery(int surveyId, string query, SqlParameter[] parameters)
        {


            if (ExecuteQueryInt32StringArrayOfSqlParameter != null)
            {
                return ExecuteQueryInt32StringArrayOfSqlParameter(surveyId, query, parameters);
            } else if (_inner != null)
            {
                return ((ISurveyDatabaseEngine)_inner).ExecuteQuery(surveyId, query, parameters);
            }

            return default(DataTable);
        }

        public delegate void ExecuteNonQueryInt32StringArrayOfSqlParameterDelegate(int surveyId, string query, SqlParameter[] sqlParams);
        public ExecuteNonQueryInt32StringArrayOfSqlParameterDelegate ExecuteNonQueryInt32StringArrayOfSqlParameter;

        void ISurveyDatabaseEngine.ExecuteNonQuery(int surveyId, string query, SqlParameter[] sqlParams)
        {

            if (ExecuteNonQueryInt32StringArrayOfSqlParameter != null)
            {
                ExecuteNonQueryInt32StringArrayOfSqlParameter(surveyId, query, sqlParams);
            } else if (_inner != null)
            {
                ((ISurveyDatabaseEngine)_inner).ExecuteNonQuery(surveyId, query, sqlParams);
            }
        }

        public delegate void ExecuteNonQuerySqlConnectionInt32StringArrayOfSqlParameterDelegate(SqlConnection surveyConnection, int surveyId, string query, SqlParameter[] sqlParams);
        public ExecuteNonQuerySqlConnectionInt32StringArrayOfSqlParameterDelegate ExecuteNonQuerySqlConnectionInt32StringArrayOfSqlParameter;

        void ISurveyDatabaseEngine.ExecuteNonQuery(SqlConnection surveyConnection, int surveyId, string query, SqlParameter[] sqlParams)
        {

            if (ExecuteNonQuerySqlConnectionInt32StringArrayOfSqlParameter != null)
            {
                ExecuteNonQuerySqlConnectionInt32StringArrayOfSqlParameter(surveyConnection, surveyId, query, sqlParams);
            } else if (_inner != null)
            {
                ((ISurveyDatabaseEngine)_inner).ExecuteNonQuery(surveyConnection, surveyId, query, sqlParams);
            }
        }

        IEnumerable<T> ISurveyDatabaseEngine.ExecuteScalarList<T>(int surveyId, string query, SqlParameter[] parameters)
        {


            return default(IEnumerable<T>);
        }

        IEnumerable<T> ISurveyDatabaseEngine.ExecuteScalarList<T>(int surveyId, string query, Func<IDataReader, T> converter, SqlParameter[] parameters)
        {


            return default(IEnumerable<T>);
        }

        T ISurveyDatabaseEngine.ExecuteScalar<T>(int surveyId, string query, SqlParameter[] parameters)
        {


            return default(T);
        }

        public delegate string GetSchemedQueryInt32StringDelegate(int surveyId, string query);
        public GetSchemedQueryInt32StringDelegate GetSchemedQueryInt32String;

        string ISurveyDatabaseEngine.GetSchemedQuery(int surveyId, string query)
        {


            if (GetSchemedQueryInt32String != null)
            {
                return GetSchemedQueryInt32String(surveyId, query);
            } else if (_inner != null)
            {
                return ((ISurveyDatabaseEngine)_inner).GetSchemedQuery(surveyId, query);
            }

            return default(string);
        }

    }
}