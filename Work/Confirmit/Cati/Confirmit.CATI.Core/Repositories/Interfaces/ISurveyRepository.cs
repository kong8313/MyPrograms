using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface ISurveyRepository
    {
        [NotNull]
        BvSurveyEntity GetById(int sid);

        [NotNull]
        BvSurveyEntity GetWithNoCache(int sid);

        [CanBeNull]
        BvSurveyEntity TryGetById(int sid);

        [NotNull]
        BvSurveyEntity GetByName(string name);

        [CanBeNull]
        BvSurveyEntity TryGetByName(string name);

        [NotNull]
        BvSurveyEntity GetByProjectId(string projectId);

        [CanBeNull]
        BvSurveyEntity TryGetByProjectId(string projectId);

        [NotNull]
        BvSurveyEntity GetByCampaignId(long campaignId);

        [CanBeNull]
        BvSurveyEntity TryGetByCampaignId(long campaignId);

        string GetSurveyNameOrErrorString(int surveyId);

        string CampaignIdToProjectId(long compaingId);

        IEnumerable<BvSurveyEntity> GetAll();

        int Insert([NotNull] BvSurveyEntity survey);
        void Update([NotNull] BvSurveyEntity survey);
        void Delete(int sid);
    }
}