using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface ISurveyDatabaseEngine
    {
        string GetSurveyScheme(int surveyId);

        DataTable ExecuteQuery(int surveyId, string query, params SqlParameter[] parameters);

        void ExecuteNonQuery(int surveyId, string query, params SqlParameter[] sqlParams);

        void ExecuteNonQuery(SqlConnection surveyConnection, int surveyId, string query,
            params SqlParameter[] sqlParams);

        IEnumerable<T> ExecuteScalarList<T>(int surveyId, string query, params SqlParameter[] parameters);

        IEnumerable<T> ExecuteScalarList<T>(int surveyId, string query, Func<IDataReader, T> converter, params SqlParameter[] parameters);

        T ExecuteScalar<T>(int surveyId, string query, params SqlParameter[] parameters);

        string GetSchemedQuery(int surveyId, string query);
    }
}