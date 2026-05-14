using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;

namespace Confirmit.CATI.Core.Repositories
{
    public class SurveyRepository : ISurveyRepository
    {
        [NotNull]
        BvSurveyEntity ISurveyRepository.GetById(int sid)
        {
            return GetById(sid);
        }
        
        [NotNull]
        BvSurveyEntity ISurveyRepository.GetWithNoCache(int sid)
        {
            return GetWithNoCache(sid);
        }

        [CanBeNull]
        BvSurveyEntity ISurveyRepository.TryGetById(int sid)
        {
            return TryGetById(sid);
        }

        [NotNull]
        BvSurveyEntity ISurveyRepository.GetByName(string name)
        {
            return GetByName(name);
        }

        [CanBeNull]
        BvSurveyEntity ISurveyRepository.TryGetByName(string name)
        {
            return TryGetByName(name);
        }

        [NotNull]
        BvSurveyEntity ISurveyRepository.GetByProjectId(string projectId)
        {
            return GetByName(projectId);
        }

        [CanBeNull]
        BvSurveyEntity ISurveyRepository.TryGetByProjectId(string projectId)
        {
            return TryGetByName(projectId);
        }

        string ISurveyRepository.CampaignIdToProjectId(long compaingId)
        {
            return CampaignIdToProjectId(compaingId);
        }

        public static string CampaignIdToProjectId(long compaingId)
        {
            return "p" + compaingId;
        }

        [NotNull]
        BvSurveyEntity ISurveyRepository.GetByCampaignId(long campaignId)
        {
            return ((ISurveyRepository)this).GetByProjectId(CampaignIdToProjectId(campaignId));
        }

        [CanBeNull]
        BvSurveyEntity ISurveyRepository.TryGetByCampaignId(long campaignId)
        {
            return ((ISurveyRepository)this).TryGetByProjectId(CampaignIdToProjectId(campaignId));
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        [NotNull]
        public static BvSurveyEntity GetById(int sid)
        {
            var survey = TryGetById(sid);

            if (survey == null)
            {
                throw new SurveyNotFoundException(sid);
            }

            return survey;
        }
        
        [NotNull]
        public static BvSurveyEntity GetWithNoCache(int sid)
        {
            var survey = BvSurveyAdapter.GetByCondition("[SID] = @Sid", new SqlParameter("@Sid", sid)).Single();

            if (survey == null)
            {
                throw new SurveyNotFoundException(sid);
            }

            return survey;
        }

        [CanBeNull]
        public static BvSurveyEntity TryGetById(int sid)
        {
            var survey = BvSurveyCache.Instance.GetBySID(sid);

            return survey;
        }

        [NotNull]
        public static BvSurveyEntity GetByName(string name)
        {
            var survey = TryGetByName(name);

            if (survey == null)
            {
                throw new SurveyNotFoundException(name);
            }

            return survey;
        }

        [CanBeNull]
        public static BvSurveyEntity TryGetByName(string name)
        {
            var survey = BvSurveyCache.Instance.GetByName(name);

            return survey;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        [NotNull]
        public static string GetSurveyNameForLogging(int sid)
        {
            var survey = GetById(sid);

            return string.Format("{0}/{1}/{2}", survey.Name, survey.Description, survey.SID);
        }

        public string GetSurveyNameOrErrorString(int surveyId)
        {
            if (surveyId == 0)
            {
                return "unknown (0)";
            }

            var survey = TryGetById(surveyId);

            if (survey == null)
            {
                return "unknown (" + surveyId + ")";
            }
            
            return survey.Name;
        }

        public IEnumerable<BvSurveyEntity> GetAll()
        {
            return BvSurveyCache.Instance.GetAll();
        }

        /// <summary>
        /// Gets the sorted page of surveys list.
        /// </summary>
        /// <param name="callCenterId"></param>
        /// <param name="pagingArgs">The paging arguments.</param>
        /// <param name="userName">Supervisor login name to filter surveys by user permissions (BvUserSurveyPermission table).</param>
        /// <param name="totalCount">The total count.</param>
        /// <returns></returns>
        public static List<BvSpSurvey_ListPageEntity> GetPage(int callCenterId, PagingArgs pagingArgs, string userName, out int totalCount)
        {
            return BvSpSurvey_ListPageAdapter.ExecuteEntityList(
                callCenterId,
                pagingArgs.PageIndex,
                pagingArgs.PageSize,
                pagingArgs.SortField,
                pagingArgs.SortOrderAsc ? 1 : 0,
                userName,
                0,
                0,
                SearchManager.GetSqlCondition(pagingArgs.SearchParameters),
                out totalCount);
        }

        public static int Insert([NotNull] BvSurveyEntity survey)
        {
            if (survey.SID != 0)
            {
                throw ExceptionManager.NewArgumentException("SID");
            }

            // generate new sid
            survey.SID = SiteService.GetNewSid();

            // insert survey
            BvSpSurvey_InsertAdapter.ExecuteNonQuery(
                survey.SID,
                survey.Name,
                survey.Description,
                survey.QuotaType,
                survey.DialMode,
                survey.State,
                survey.ForceOpnRev,
                survey.StateGroupID,
                survey.RecWholeInt,
                survey.InterviewScreenRecording,
                null,
                survey.CfDbSchemaPath,
                survey.DestinationTableName,
                survey.ReplicationStatus,
                survey.ScheduleID,
                survey.DialerParameters,
                survey.IsTelephoneBlacklistSupported,
                survey.NotificationEmail,
                survey.EnforceHttps,
                survey.SurveySchedulingMode,
                survey.SurveySqlServerName);

            BvSurveyCache.Instance.OnTableChanged();
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishScheduleParamsUpdated();
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishSurveyUpdated();
            
            return survey.SID;
        }

        int ISurveyRepository.Insert([NotNull] BvSurveyEntity survey)
        {
            return Insert(survey);
        }

        public static void Update([NotNull] BvSurveyEntity survey)
        {
            if (survey.SID == 0)
            {
                throw ExceptionManager.NewArgumentException("SID");
            }

            BvSpSurvey_UpdateAdapter.ExecuteNonQuery(
                survey.SID,
                survey.Name,
                survey.Description,
                survey.QuotaType,
                survey.DialMode,
                survey.ForceOpnRev,
                survey.StateGroupID,
                survey.RecWholeInt,
                survey.InterviewScreenRecording,
                survey.DestinationTableName,
                survey.ReplicationStatus,
                survey.ScheduleID,
                survey.DialerParameters,
                survey.IsTelephoneBlacklistSupported,
                survey.IsRespondentsDynamicCreationAllowed,
                survey.NotificationEmail,
                survey.EnforceHttps,
                survey.LastTouchTime,
                survey.SurveySchedulingMode,
                survey.ClusteredQuotaName,
                survey.ClusteredQuotaThreshold,
                survey.DialerId,
                survey.Target,
                survey.InternalTransferType,
                survey.ExternalTransferType,
                survey.IsLiveMonitoringEnabled,
                survey.IsQuotaInCatiDb,
                survey.InboundCallBehavior,
                survey.Comment,
                survey.IsStateLocked);

            BvSurveyCache.Instance.OnTableChanged();
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishScheduleParamsUpdated();
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishSurveyUpdated();
        }

        void ISurveyRepository.Update([NotNull] BvSurveyEntity survey)
        {
            Update(survey);
        }

        public static void Delete(int sid)
        {
            if (sid == 0)
            {
                throw ExceptionManager.NewArgumentException("SID");
            }

            //
            // BvSpSurvey_DeleteAdapter deletes BvPhase and othe survey specific objects,
            // so we do not need to do something else
            BvSpSurvey_DeleteAdapter.ExecuteNonQuery(
                sid);

            BvSurveyCache.Instance.OnTableChanged();
            BvInboundTelephoneNumberCache.Instance.OnTableChanged();
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishScheduleParamsUpdated();
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishInboundTelephoneNumberUpdated();
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishSurveyUpdated();
        }

        void ISurveyRepository.Delete(int sid)
        {
            Delete(sid);
        }
        
        public static int GetInterviewsCount(int surveyId)
        {
            var query = "SELECT SUM(Cnt) FROM BvSampleStatusSummary WHERE SurveySID = @SurveySID";
            return new DatabaseEngine().ExecuteScalar<int>(query, CommandType.Text, new SqlParameter("@SurveySID", surveyId));
        }
    }
}
