using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IInboundTelephoneNumberRepository
    {
        BvInboundTelephoneNumberEntity TryGetByTelephoneNumber(string telephoneNumber);

        List<BvInboundTelephoneNumberEntity> GetByTelephoneNumbers(string[] telephoneNumber);

        List<BvInboundTelephoneNumberEntity> GetValidByDialerId(int dialerId);

        List<BvInboundTelephoneNumberEntity> GetBySurveyId(int surveyId);
        void Insert(BvInboundTelephoneNumberEntity entity);

        void Update(BvInboundTelephoneNumberEntity entity);

        void Delete(string[] numbers);
    }
}