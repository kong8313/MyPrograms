using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIInboundTelephoneNumberRepository : IInboundTelephoneNumberRepository 
    {
        private IInboundTelephoneNumberRepository _inner;

        public StubIInboundTelephoneNumberRepository()
        {
            _inner = null;
        }

        public IInboundTelephoneNumberRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvInboundTelephoneNumberEntity TryGetByTelephoneNumberStringDelegate(string telephoneNumber);
        public TryGetByTelephoneNumberStringDelegate TryGetByTelephoneNumberString;

        BvInboundTelephoneNumberEntity IInboundTelephoneNumberRepository.TryGetByTelephoneNumber(string telephoneNumber)
        {


            if (TryGetByTelephoneNumberString != null)
            {
                return TryGetByTelephoneNumberString(telephoneNumber);
            } else if (_inner != null)
            {
                return ((IInboundTelephoneNumberRepository)_inner).TryGetByTelephoneNumber(telephoneNumber);
            }

            return default(BvInboundTelephoneNumberEntity);
        }

        public delegate List<BvInboundTelephoneNumberEntity> GetByTelephoneNumbersArrayOfStringDelegate(string[] telephoneNumber);
        public GetByTelephoneNumbersArrayOfStringDelegate GetByTelephoneNumbersArrayOfString;

        List<BvInboundTelephoneNumberEntity> IInboundTelephoneNumberRepository.GetByTelephoneNumbers(string[] telephoneNumber)
        {


            if (GetByTelephoneNumbersArrayOfString != null)
            {
                return GetByTelephoneNumbersArrayOfString(telephoneNumber);
            } else if (_inner != null)
            {
                return ((IInboundTelephoneNumberRepository)_inner).GetByTelephoneNumbers(telephoneNumber);
            }

            return default(List<BvInboundTelephoneNumberEntity>);
        }

        public delegate List<BvInboundTelephoneNumberEntity> GetValidByDialerIdInt32Delegate(int dialerId);
        public GetValidByDialerIdInt32Delegate GetValidByDialerIdInt32;

        List<BvInboundTelephoneNumberEntity> IInboundTelephoneNumberRepository.GetValidByDialerId(int dialerId)
        {


            if (GetValidByDialerIdInt32 != null)
            {
                return GetValidByDialerIdInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IInboundTelephoneNumberRepository)_inner).GetValidByDialerId(dialerId);
            }

            return default(List<BvInboundTelephoneNumberEntity>);
        }

        public delegate List<BvInboundTelephoneNumberEntity> GetBySurveyIdInt32Delegate(int surveyId);
        public GetBySurveyIdInt32Delegate GetBySurveyIdInt32;

        List<BvInboundTelephoneNumberEntity> IInboundTelephoneNumberRepository.GetBySurveyId(int surveyId)
        {


            if (GetBySurveyIdInt32 != null)
            {
                return GetBySurveyIdInt32(surveyId);
            } else if (_inner != null)
            {
                return ((IInboundTelephoneNumberRepository)_inner).GetBySurveyId(surveyId);
            }

            return default(List<BvInboundTelephoneNumberEntity>);
        }

        public delegate void InsertBvInboundTelephoneNumberEntityDelegate(BvInboundTelephoneNumberEntity entity);
        public InsertBvInboundTelephoneNumberEntityDelegate InsertBvInboundTelephoneNumberEntity;

        void IInboundTelephoneNumberRepository.Insert(BvInboundTelephoneNumberEntity entity)
        {

            if (InsertBvInboundTelephoneNumberEntity != null)
            {
                InsertBvInboundTelephoneNumberEntity(entity);
            } else if (_inner != null)
            {
                ((IInboundTelephoneNumberRepository)_inner).Insert(entity);
            }
        }

        public delegate void UpdateBvInboundTelephoneNumberEntityDelegate(BvInboundTelephoneNumberEntity entity);
        public UpdateBvInboundTelephoneNumberEntityDelegate UpdateBvInboundTelephoneNumberEntity;

        void IInboundTelephoneNumberRepository.Update(BvInboundTelephoneNumberEntity entity)
        {

            if (UpdateBvInboundTelephoneNumberEntity != null)
            {
                UpdateBvInboundTelephoneNumberEntity(entity);
            } else if (_inner != null)
            {
                ((IInboundTelephoneNumberRepository)_inner).Update(entity);
            }
        }

        public delegate void DeleteArrayOfStringDelegate(string[] numbers);
        public DeleteArrayOfStringDelegate DeleteArrayOfString;

        void IInboundTelephoneNumberRepository.Delete(string[] numbers)
        {

            if (DeleteArrayOfString != null)
            {
                DeleteArrayOfString(numbers);
            } else if (_inner != null)
            {
                ((IInboundTelephoneNumberRepository)_inner).Delete(numbers);
            }
        }

    }
}