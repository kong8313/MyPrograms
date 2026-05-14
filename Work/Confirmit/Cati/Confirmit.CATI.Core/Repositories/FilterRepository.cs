using System;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Security;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class FilterRepository : IFilterRepository
    {
        BvFiltersEntity IFilterRepository.GetById(int sid)
        {
            return GetById(sid);
        }

        [CanBeNull]
        public static BvFiltersEntity GetById(int sid)
        {
            var entities = BvFiltersAdapter.GetByCondition(
                "[Sid] = @Sid",
                new SqlParameter("@Sid", sid));

            return entities.FirstOrDefault();
        }

        public static List<BvFiltersEntity> GetAll()
        {
            return BvFiltersAdapter.GetByCondition("[Hidden] = 0");
        }

        /// <summary>
        /// Gets the list of survey specific filters for the specified survey SID.
        /// </summary>
        /// <param name="surveyId">The survey SID to get list of filters for.</param>
        /// <returns>The list of survey specific filters.</returns>
        /// <remarks>
        /// The filter is survey specific if it refers to the variable of the survey. 
        /// Filters specific to one survey couldn't be used to filter other surveys.
        /// </remarks>
        private static List<BvFiltersEntity> GetSurveySpecific(int surveyId)
        {
            return BvFiltersAdapter.GetByCondition(
                "[Hidden] = 0 AND [SurveySID] = @SurveySID",
                new SqlParameter("@SurveySID", surveyId));
        }

        /// <summary>
        /// Gets the list of survey specific and site wide filters for the specified survey SID.
        /// </summary>
        /// <param name="surveyId">The survey SID to get list of filters for.</param>
        /// <returns>The list of survey specific and site wide filters.</returns>
        /// <remarks>
        /// The filter is survey specific if it refers to the variable of the survey.
        /// Filters specific to one survey couldn't be used to filter other surveys.
        /// Site wide filters does not refer to survey variables and can be used to filter all surveys.
        /// </remarks>
        private static List<BvFiltersEntity> GetSurveySpecificAndSiteWide(int surveyId)
        {
            return BvFiltersAdapter.GetByCondition(
                "[Hidden] = 0 AND ([SurveySID] = @SurveySID OR [SurveySID] = 0)",
                new SqlParameter("@SurveySID", surveyId));
        }

        /// <summary>
        /// Gets the list of the filters for the specified survey SID.
        /// </summary>
        /// <param name="includeSiteWide">if set to <c>true</c> both survey specific and 
        /// site wide filters are returned, otherwise survey specific only.</param>
        /// <param name="surveyId">The survey SID to get filters for.</param>
        /// <returns>The list of survey specific and site wide filters.</returns>
        /// <remarks>
        /// The filter is survey specific if it refers to the variable of the survey.
        /// Filters specific to one survey couldn't be used to filter other surveys.
        /// Site wide filters does not refer to survey variables and can be used to filter all surveys.
        /// </remarks>
        public static List<BvFiltersEntity> GetFiltersList(bool includeSiteWide, int surveyId)
        {
            return includeSiteWide ? GetSurveySpecificAndSiteWide(surveyId) : GetSurveySpecific(surveyId);
        }

        List<BvFiltersEntity> IFilterRepository.GetFiltersList(bool includeSiteWide, int surveyId)
        {
            return GetFiltersList(includeSiteWide, surveyId);
        }

        public List<int> GetAllParentFilters(int filterSid)
        {
            if (filterSid <= 0)
            {
                throw new ArgumentOutOfRangeException("filterSid");
            }

            return BvSpFilter_GetParentFiltersAdapter.ExecuteEntityList(filterSid).Select(x => x.SID.Value).ToList();
        }

        /// <summary>
        /// Inserts the specified filter into the repository. Filter SID should be equal to 0.
        /// </summary>
        /// <param name="filter">The filter entity to insert.</param>
        /// <returns>SID of the inserted filter.</returns>
        /// <exception cref="ArgumentException">Filter SID is not equal to 0.</exception>
        public static int Insert([NotNull] BvFiltersEntity filter)
        {
            if (filter.SID != 0)
            {
                throw ExceptionManager.NewArgumentException("filter.SID");
            }

            int newSid;

            BvSpFilter_InsertAdapter.ExecuteNonQuery(
                filter.Name,
                filter.Description,
                filter.AndOrOperator,
                filter.SurveySID,
                filter.Hidden,
                out newSid);

            return newSid;
        }

        /// <summary>
        /// Updates the specified filter in the repository. Filter SID should not be equal or less than 0.
        /// </summary>
        /// <param name="filter">The filter entity to update.</param>
        /// <exception cref="ArgumentException">Filter SID is equal or less than 0.</exception>
        public static void Update([NotNull] BvFiltersEntity filter)
        {
            if (filter.SID <= 0)
            {
                throw ExceptionManager.NewArgumentException("filter.SID");
            }

            DataValidationManager.CheckForSqlInjection(filter.Name);
            DataValidationManager.CheckForSqlInjection(filter.Description);

            BvSpFilter_UpdateAdapter.ExecuteNonQuery(
                filter.SID,
                filter.Name,
                filter.Description,
                filter.AndOrOperator,
                filter.SurveySID);
        }

        /// <summary>
        /// Deletes the filter with the specified SID form the repository. Filter SID should not be equal or less than  0.
        /// </summary>
        /// <param name="sid">The SID of the filter to delete.</param>
        /// <exception cref="ArgumentException">Filter SID is equal or less than 0.</exception>
        public static void Delete(int sid)
        {
            if (sid <= 0)
            {
                throw ExceptionManager.NewArgumentException("sid");
            }

            BvSpFilter_DeleteAdapter.ExecuteNonQuery(sid);
        }
    }
}