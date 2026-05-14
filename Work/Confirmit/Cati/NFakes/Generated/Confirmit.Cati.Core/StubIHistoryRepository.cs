using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Data;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIHistoryRepository : IHistoryRepository 
    {
        private IHistoryRepository _inner;

        public StubIHistoryRepository()
        {
            _inner = null;
        }

        public IHistoryRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate int InsertBvHistoryEntityDelegate(BvHistoryEntity history);
        public InsertBvHistoryEntityDelegate InsertBvHistoryEntity;

        int IHistoryRepository.Insert(BvHistoryEntity history)
        {


            if (InsertBvHistoryEntity != null)
            {
                return InsertBvHistoryEntity(history);
            } else if (_inner != null)
            {
                return ((IHistoryRepository)_inner).Insert(history);
            }

            return default(int);
        }

        public delegate void DeleteInt32Delegate(int id);
        public DeleteInt32Delegate DeleteInt32;

        void IHistoryRepository.Delete(int id)
        {

            if (DeleteInt32 != null)
            {
                DeleteInt32(id);
            } else if (_inner != null)
            {
                ((IHistoryRepository)_inner).Delete(id);
            }
        }

        public delegate BvHistoryEntity GetByIdInt32Delegate(int id);
        public GetByIdInt32Delegate GetByIdInt32;

        BvHistoryEntity IHistoryRepository.GetById(int id)
        {


            if (GetByIdInt32 != null)
            {
                return GetByIdInt32(id);
            } else if (_inner != null)
            {
                return ((IHistoryRepository)_inner).GetById(id);
            }

            return default(BvHistoryEntity);
        }

        public delegate void UpdateBvHistoryEntityDelegate(BvHistoryEntity entity);
        public UpdateBvHistoryEntityDelegate UpdateBvHistoryEntity;

        void IHistoryRepository.Update(BvHistoryEntity entity)
        {

            if (UpdateBvHistoryEntity != null)
            {
                UpdateBvHistoryEntity(entity);
            } else if (_inner != null)
            {
                ((IHistoryRepository)_inner).Update(entity);
            }
        }

        public delegate DataTable GetCallAttemptsForInterviewInt32Int32Delegate(int surveyId, int interviewId);
        public GetCallAttemptsForInterviewInt32Int32Delegate GetCallAttemptsForInterviewInt32Int32;

        DataTable IHistoryRepository.GetCallAttemptsForInterview(int surveyId, int interviewId)
        {


            if (GetCallAttemptsForInterviewInt32Int32 != null)
            {
                return GetCallAttemptsForInterviewInt32Int32(surveyId, interviewId);
            } else if (_inner != null)
            {
                return ((IHistoryRepository)_inner).GetCallAttemptsForInterview(surveyId, interviewId);
            }

            return default(DataTable);
        }

    }
}