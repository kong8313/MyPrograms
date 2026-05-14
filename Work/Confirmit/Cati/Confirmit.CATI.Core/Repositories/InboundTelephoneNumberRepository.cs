using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class InboundTelephoneNumberRepository : IInboundTelephoneNumberRepository
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly ISqlTableUpdatedPublisher _sqlTableUpdatedPublisher;

        public InboundTelephoneNumberRepository(ISurveyRepository surveyRepository, ISqlTableUpdatedPublisher sqlTableUpdatedPublisher)
        {
            _surveyRepository = surveyRepository;
            _sqlTableUpdatedPublisher = sqlTableUpdatedPublisher;
        }

        public BvInboundTelephoneNumberEntity TryGetByTelephoneNumber(string telephoneNumber)
        {
            return BvInboundTelephoneNumberCache.Instance.GetByTelephoneNumber(telephoneNumber);
        }

        public List<BvInboundTelephoneNumberEntity> GetByTelephoneNumbers(string[] numbers)
        {
            if (BackendInstance.Current.IsCacheEnabled)
            {
                return BvInboundTelephoneNumberCache.Instance.GetAll().Where(x => numbers.Contains(x.TelephoneNumber)).ToList();
            }
            else
            {
                return BvInboundTelephoneNumberAdapter.GetByCondition(string.Format("[TelephoneNumber] IN ({0})", string.Join(",", numbers.Select(x => $"'{x}'"))));
            }
        }

        public List<BvInboundTelephoneNumberEntity> GetValidByDialerId(int dialerId)
        {
            List<BvInboundTelephoneNumberEntity> ddiNumbers = BackendInstance.Current.IsCacheEnabled 
                ? BvInboundTelephoneNumberCache.Instance.GetAll().Where(x => x.DialerId == dialerId).ToList()
                : BvInboundTelephoneNumberAdapter.GetByCondition("[DialerId] = @DialerId", new SqlParameter("@DialerId", dialerId)); 

            return ddiNumbers.Where(x => x.SurveyId.HasValue && _surveyRepository.GetById(x.SurveyId.Value).State != (int)SurveyState.SoftDeleted).ToList();
        }

        public List<BvInboundTelephoneNumberEntity> GetBySurveyId(int surveyId)
        {
            if (BackendInstance.Current.IsCacheEnabled)
            {
                return BvInboundTelephoneNumberCache.Instance.GetAll().Where(x => x.SurveyId == surveyId).ToList();
            }
            else
            {
                return BvInboundTelephoneNumberAdapter.GetByCondition("[SurveyId] = @SurveyId",
                    new SqlParameter("@SurveyId", surveyId));
            }
        }

        public void Insert(BvInboundTelephoneNumberEntity entity)
        {
            BvInboundTelephoneNumberAdapter.Insert(entity);
            BvInboundTelephoneNumberCache.Instance.OnTableChanged();
            _sqlTableUpdatedPublisher.PublishInboundTelephoneNumberUpdated();
        }

        public void Update(BvInboundTelephoneNumberEntity entity)
        {
            BvInboundTelephoneNumberAdapter.Update(entity);
            BvInboundTelephoneNumberCache.Instance.OnTableChanged();
            _sqlTableUpdatedPublisher.PublishInboundTelephoneNumberUpdated();
        }

        public void Delete(string[] numbers)
        {
            if (numbers.Any())
            {
                BvInboundTelephoneNumberAdapter.DeleteByCondition(string.Format("[TelephoneNumber] IN ({0})", string.Join(",", numbers.Select(x => $"'{x}'"))));
                BvInboundTelephoneNumberCache.Instance.OnTableChanged();
                _sqlTableUpdatedPublisher.PublishInboundTelephoneNumberUpdated();
            }
        }
    }
}