using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Confirmit.CATI.Core.Services.Survey.Quota
{
    public class QuotaMatcherBuilder
    {
        private readonly IQuotaDatabaseReader _quotaDatabaseReader;
        private readonly IQuotaCellRepository _quotaCellRepository;
        private readonly IQuotaRepository _quotaRepository;

        public QuotaMatcherBuilder(
            IQuotaDatabaseReader quotaDatabaseReader,
            IQuotaCellRepository quotaCellRepository,
            IQuotaRepository quotaRepository)
        {
            _quotaDatabaseReader = quotaDatabaseReader;
            _quotaCellRepository = quotaCellRepository;
            _quotaRepository = quotaRepository;
        }

        public QuotaMatcher Build(int surveyId)
        {
            var quotaMatcher = new QuotaMatcher();

            var quotas = _quotaDatabaseReader.GetQuotas(surveyId);
            foreach (var quota in quotas)
            {
                if (quota.IsFcd)
                {
                    AddQuota(quotaMatcher, surveyId, quota.Id);
                }
            }

            return quotaMatcher;
        }

        public QuotaMatcher Build(int surveyId, int quotaId)
        {
            var quotaMatcher = new QuotaMatcher();

            var quota = _quotaRepository.TryGetById(surveyId, quotaId);
            if (quota?.IsFCD == 1)
            {
                AddQuota(quotaMatcher, surveyId, quotaId);
            }

            return quotaMatcher;
        }

        private void AddQuota(QuotaMatcher quotaMatcher, int surveyId, int quotaId)
        {
            var fields = _quotaDatabaseReader.GetQuotaFields(surveyId, quotaId);
            var quotaCells = _quotaCellRepository.GetCells(surveyId, quotaId);
            var availableValues = _quotaDatabaseReader.GetFieldPrecodes(surveyId, quotaId);

            quotaMatcher.AddQuota(fields.ToList(), quotaCells, availableValues);
        }
    }
}
