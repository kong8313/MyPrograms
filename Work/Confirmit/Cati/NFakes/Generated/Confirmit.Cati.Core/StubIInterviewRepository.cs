using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIInterviewRepository : IInterviewRepository 
    {
        private IInterviewRepository _inner;

        public StubIInterviewRepository()
        {
            _inner = null;
        }

        public IInterviewRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvInterviewWithOriginEntity GetByIdInt32Int32Delegate(int surveySid, int interviewId);
        public GetByIdInt32Int32Delegate GetByIdInt32Int32;

        BvInterviewWithOriginEntity IInterviewRepository.GetById(int surveySid, int interviewId)
        {


            if (GetByIdInt32Int32 != null)
            {
                return GetByIdInt32Int32(surveySid, interviewId);
            } else if (_inner != null)
            {
                return ((IInterviewRepository)_inner).GetById(surveySid, interviewId);
            }

            return default(BvInterviewWithOriginEntity);
        }

        public delegate BvInterviewWithOriginEntity GetByIdWithCheckInt32Int32Delegate(int surveySid, int interviewId);
        public GetByIdWithCheckInt32Int32Delegate GetByIdWithCheckInt32Int32;

        BvInterviewWithOriginEntity IInterviewRepository.GetByIdWithCheck(int surveySid, int interviewId)
        {


            if (GetByIdWithCheckInt32Int32 != null)
            {
                return GetByIdWithCheckInt32Int32(surveySid, interviewId);
            } else if (_inner != null)
            {
                return ((IInterviewRepository)_inner).GetByIdWithCheck(surveySid, interviewId);
            }

            return default(BvInterviewWithOriginEntity);
        }

        public delegate BvInterviewWithOriginEntity GetByTelephoneNumberInt32StringDelegate(int surveyId, string telephoneNumber);
        public GetByTelephoneNumberInt32StringDelegate GetByTelephoneNumberInt32String;

        BvInterviewWithOriginEntity IInterviewRepository.GetByTelephoneNumber(int surveyId, string telephoneNumber)
        {


            if (GetByTelephoneNumberInt32String != null)
            {
                return GetByTelephoneNumberInt32String(surveyId, telephoneNumber);
            } else if (_inner != null)
            {
                return ((IInterviewRepository)_inner).GetByTelephoneNumber(surveyId, telephoneNumber);
            }

            return default(BvInterviewWithOriginEntity);
        }

        public delegate void UpdateBvInterviewWithOriginEntitySchedulingScriptExecutionOptionsDelegate(BvInterviewWithOriginEntity interview, SchedulingScriptExecutionOptions schedulingOptions);
        public UpdateBvInterviewWithOriginEntitySchedulingScriptExecutionOptionsDelegate UpdateBvInterviewWithOriginEntitySchedulingScriptExecutionOptions;

        void IInterviewRepository.Update(BvInterviewWithOriginEntity interview, SchedulingScriptExecutionOptions schedulingOptions)
        {

            if (UpdateBvInterviewWithOriginEntitySchedulingScriptExecutionOptions != null)
            {
                UpdateBvInterviewWithOriginEntitySchedulingScriptExecutionOptions(interview, schedulingOptions);
            } else if (_inner != null)
            {
                ((IInterviewRepository)_inner).Update(interview, schedulingOptions);
            }
        }

        public delegate void InsertOnlyBvInterviewEntityDelegate(BvInterviewEntity interview);
        public InsertOnlyBvInterviewEntityDelegate InsertOnlyBvInterviewEntity;

        void IInterviewRepository.InsertOnly(BvInterviewEntity interview)
        {

            if (InsertOnlyBvInterviewEntity != null)
            {
                InsertOnlyBvInterviewEntity(interview);
            } else if (_inner != null)
            {
                ((IInterviewRepository)_inner).InsertOnly(interview);
            }
        }

        public delegate void InsertBvInterviewWithOriginEntitySchedulingScriptExecutionOptionsISampleDataStorageDelegate(BvInterviewWithOriginEntity interview, SchedulingScriptExecutionOptions schedulingOptions, ISampleDataStorage sampleStorage);
        public InsertBvInterviewWithOriginEntitySchedulingScriptExecutionOptionsISampleDataStorageDelegate InsertBvInterviewWithOriginEntitySchedulingScriptExecutionOptionsISampleDataStorage;

        void IInterviewRepository.Insert(BvInterviewWithOriginEntity interview, SchedulingScriptExecutionOptions schedulingOptions, ISampleDataStorage sampleStorage)
        {

            if (InsertBvInterviewWithOriginEntitySchedulingScriptExecutionOptionsISampleDataStorage != null)
            {
                InsertBvInterviewWithOriginEntitySchedulingScriptExecutionOptionsISampleDataStorage(interview, schedulingOptions, sampleStorage);
            } else if (_inner != null)
            {
                ((IInterviewRepository)_inner).Insert(interview, schedulingOptions, sampleStorage);
            }
        }

    }
}