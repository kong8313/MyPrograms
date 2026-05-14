using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIActiveDialRepository : IActiveDialRepository 
    {
        private IActiveDialRepository _inner;

        public StubIActiveDialRepository()
        {
            _inner = null;
        }

        public IActiveDialRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvActiveDialEntity TryGetByIdNullableOfInt64Delegate(long? dialId);
        public TryGetByIdNullableOfInt64Delegate TryGetByIdNullableOfInt64;

        BvActiveDialEntity IActiveDialRepository.TryGetById(long? dialId)
        {


            if (TryGetByIdNullableOfInt64 != null)
            {
                return TryGetByIdNullableOfInt64(dialId);
            } else if (_inner != null)
            {
                return ((IActiveDialRepository)_inner).TryGetById(dialId);
            }

            return default(BvActiveDialEntity);
        }

        public delegate BvActiveDialEntity TryGetByCallIdNullableOfInt64Delegate(long? callId);
        public TryGetByCallIdNullableOfInt64Delegate TryGetByCallIdNullableOfInt64;

        BvActiveDialEntity IActiveDialRepository.TryGetByCallId(long? callId)
        {


            if (TryGetByCallIdNullableOfInt64 != null)
            {
                return TryGetByCallIdNullableOfInt64(callId);
            } else if (_inner != null)
            {
                return ((IActiveDialRepository)_inner).TryGetByCallId(callId);
            }

            return default(BvActiveDialEntity);
        }

        public delegate BvActiveDialEntity TryGetByTransferIdStringDelegate(string transferId);
        public TryGetByTransferIdStringDelegate TryGetByTransferIdString;

        BvActiveDialEntity IActiveDialRepository.TryGetByTransferId(string transferId)
        {


            if (TryGetByTransferIdString != null)
            {
                return TryGetByTransferIdString(transferId);
            } else if (_inner != null)
            {
                return ((IActiveDialRepository)_inner).TryGetByTransferId(transferId);
            }

            return default(BvActiveDialEntity);
        }

        public delegate BvActiveDialEntity TryGetByInboundCallIdStringDelegate(string inboundCallId);
        public TryGetByInboundCallIdStringDelegate TryGetByInboundCallIdString;

        BvActiveDialEntity IActiveDialRepository.TryGetByInboundCallId(string inboundCallId)
        {


            if (TryGetByInboundCallIdString != null)
            {
                return TryGetByInboundCallIdString(inboundCallId);
            } else if (_inner != null)
            {
                return ((IActiveDialRepository)_inner).TryGetByInboundCallId(inboundCallId);
            }

            return default(BvActiveDialEntity);
        }

        public delegate BvActiveDialEntity TryGetBySurveyAndInterviewIdInt32Int32Delegate(int surveyId, int interviewId);
        public TryGetBySurveyAndInterviewIdInt32Int32Delegate TryGetBySurveyAndInterviewIdInt32Int32;

        BvActiveDialEntity IActiveDialRepository.TryGetBySurveyAndInterviewId(int surveyId, int interviewId)
        {


            if (TryGetBySurveyAndInterviewIdInt32Int32 != null)
            {
                return TryGetBySurveyAndInterviewIdInt32Int32(surveyId, interviewId);
            } else if (_inner != null)
            {
                return ((IActiveDialRepository)_inner).TryGetBySurveyAndInterviewId(surveyId, interviewId);
            }

            return default(BvActiveDialEntity);
        }

        public delegate BvActiveDialEntity GetByCallIdWithCheckInt64Delegate(long callId);
        public GetByCallIdWithCheckInt64Delegate GetByCallIdWithCheckInt64;

        BvActiveDialEntity IActiveDialRepository.GetByCallIdWithCheck(long callId)
        {


            if (GetByCallIdWithCheckInt64 != null)
            {
                return GetByCallIdWithCheckInt64(callId);
            } else if (_inner != null)
            {
                return ((IActiveDialRepository)_inner).GetByCallIdWithCheck(callId);
            }

            return default(BvActiveDialEntity);
        }

        public delegate BvActiveDialEntity GetByTransferIdWithCheckStringDelegate(string transferId);
        public GetByTransferIdWithCheckStringDelegate GetByTransferIdWithCheckString;

        BvActiveDialEntity IActiveDialRepository.GetByTransferIdWithCheck(string transferId)
        {


            if (GetByTransferIdWithCheckString != null)
            {
                return GetByTransferIdWithCheckString(transferId);
            } else if (_inner != null)
            {
                return ((IActiveDialRepository)_inner).GetByTransferIdWithCheck(transferId);
            }

            return default(BvActiveDialEntity);
        }

        public delegate BvActiveDialEntity InsertBvActiveDialEntityDelegate(BvActiveDialEntity dial);
        public InsertBvActiveDialEntityDelegate InsertBvActiveDialEntity;

        BvActiveDialEntity IActiveDialRepository.Insert(BvActiveDialEntity dial)
        {


            if (InsertBvActiveDialEntity != null)
            {
                return InsertBvActiveDialEntity(dial);
            } else if (_inner != null)
            {
                return ((IActiveDialRepository)_inner).Insert(dial);
            }

            return default(BvActiveDialEntity);
        }

        public delegate void UpdateBvActiveDialEntityDelegate(BvActiveDialEntity entity);
        public UpdateBvActiveDialEntityDelegate UpdateBvActiveDialEntity;

        void IActiveDialRepository.Update(BvActiveDialEntity entity)
        {

            if (UpdateBvActiveDialEntity != null)
            {
                UpdateBvActiveDialEntity(entity);
            } else if (_inner != null)
            {
                ((IActiveDialRepository)_inner).Update(entity);
            }
        }

        public delegate void DeleteInt64CallCompleteStatusDelegate(long id, CallCompleteStatus callCompleteStatus);
        public DeleteInt64CallCompleteStatusDelegate DeleteInt64CallCompleteStatus;

        void IActiveDialRepository.Delete(long id, CallCompleteStatus callCompleteStatus)
        {

            if (DeleteInt64CallCompleteStatus != null)
            {
                DeleteInt64CallCompleteStatus(id, callCompleteStatus);
            } else if (_inner != null)
            {
                ((IActiveDialRepository)_inner).Delete(id, callCompleteStatus);
            }
        }

    }
}