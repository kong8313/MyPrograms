using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubITaskRepository : ITaskRepository 
    {
        private ITaskRepository _inner;

        public StubITaskRepository()
        {
            _inner = null;
        }

        public ITaskRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvTasksEntity GetByIdInt32Int32Delegate(int surveySid, int interviewId);
        public GetByIdInt32Int32Delegate GetByIdInt32Int32;

        BvTasksEntity ITaskRepository.GetById(int surveySid, int interviewId)
        {


            if (GetByIdInt32Int32 != null)
            {
                return GetByIdInt32Int32(surveySid, interviewId);
            } else if (_inner != null)
            {
                return ((ITaskRepository)_inner).GetById(surveySid, interviewId);
            }

            return default(BvTasksEntity);
        }

        public delegate BvTasksEntity GetByIdWithCheckInt32Int32Delegate(int surveySid, int interviewId);
        public GetByIdWithCheckInt32Int32Delegate GetByIdWithCheckInt32Int32;

        BvTasksEntity ITaskRepository.GetByIdWithCheck(int surveySid, int interviewId)
        {


            if (GetByIdWithCheckInt32Int32 != null)
            {
                return GetByIdWithCheckInt32Int32(surveySid, interviewId);
            } else if (_inner != null)
            {
                return ((ITaskRepository)_inner).GetByIdWithCheck(surveySid, interviewId);
            }

            return default(BvTasksEntity);
        }

        public delegate BvTasksEntity GetByPersonInt32Delegate(int personSid);
        public GetByPersonInt32Delegate GetByPersonInt32;

        BvTasksEntity ITaskRepository.GetByPerson(int personSid)
        {


            if (GetByPersonInt32 != null)
            {
                return GetByPersonInt32(personSid);
            } else if (_inner != null)
            {
                return ((ITaskRepository)_inner).GetByPerson(personSid);
            }

            return default(BvTasksEntity);
        }

        public delegate BvTasksEntity GetByPersonWithCheckInt32Delegate(int personSid);
        public GetByPersonWithCheckInt32Delegate GetByPersonWithCheckInt32;

        BvTasksEntity ITaskRepository.GetByPersonWithCheck(int personSid)
        {


            if (GetByPersonWithCheckInt32 != null)
            {
                return GetByPersonWithCheckInt32(personSid);
            } else if (_inner != null)
            {
                return ((ITaskRepository)_inner).GetByPersonWithCheck(personSid);
            }

            return default(BvTasksEntity);
        }

        public delegate void InsertBvTasksEntityDelegate(BvTasksEntity task);
        public InsertBvTasksEntityDelegate InsertBvTasksEntity;

        void ITaskRepository.Insert(BvTasksEntity task)
        {

            if (InsertBvTasksEntity != null)
            {
                InsertBvTasksEntity(task);
            } else if (_inner != null)
            {
                ((ITaskRepository)_inner).Insert(task);
            }
        }

        public delegate void UpdateBvTasksEntityDelegate(BvTasksEntity task);
        public UpdateBvTasksEntityDelegate UpdateBvTasksEntity;

        void ITaskRepository.Update(BvTasksEntity task)
        {

            if (UpdateBvTasksEntity != null)
            {
                UpdateBvTasksEntity(task);
            } else if (_inner != null)
            {
                ((ITaskRepository)_inner).Update(task);
            }
        }

        public delegate BvTasksEntity DeleteByPersonInt32Delegate(int personSid);
        public DeleteByPersonInt32Delegate DeleteByPersonInt32;

        BvTasksEntity ITaskRepository.DeleteByPerson(int personSid)
        {


            if (DeleteByPersonInt32 != null)
            {
                return DeleteByPersonInt32(personSid);
            } else if (_inner != null)
            {
                return ((ITaskRepository)_inner).DeleteByPerson(personSid);
            }

            return default(BvTasksEntity);
        }

        public delegate void MergeBvTasksEntityDelegate(BvTasksEntity task);
        public MergeBvTasksEntityDelegate MergeBvTasksEntity;

        void ITaskRepository.Merge(BvTasksEntity task)
        {

            if (MergeBvTasksEntity != null)
            {
                MergeBvTasksEntity(task);
            } else if (_inner != null)
            {
                ((ITaskRepository)_inner).Merge(task);
            }
        }

        public delegate Task UpdateActiveQuestionStringInt32StringDateTimeDelegate(string projectId, int catiInterviewerId, string questionId, DateTime showTime);
        public UpdateActiveQuestionStringInt32StringDateTimeDelegate UpdateActiveQuestionStringInt32StringDateTime;

        Task ITaskRepository.UpdateActiveQuestion(string projectId, int catiInterviewerId, string questionId, DateTime showTime)
        {


            if (UpdateActiveQuestionStringInt32StringDateTime != null)
            {
                return UpdateActiveQuestionStringInt32StringDateTime(projectId, catiInterviewerId, questionId, showTime);
            } else if (_inner != null)
            {
                return ((ITaskRepository)_inner).UpdateActiveQuestion(projectId, catiInterviewerId, questionId, showTime);
            }

            return default(Task);
        }

        public delegate IEnumerable<int> GetPersonIdsFromBBCCDelegate();
        public GetPersonIdsFromBBCCDelegate GetPersonIdsFromBBCC;

        IEnumerable<int> ITaskRepository.GetPersonIdsFromBBCC()
        {


            if (GetPersonIdsFromBBCC != null)
            {
                return GetPersonIdsFromBBCC();
            } else if (_inner != null)
            {
                return ((ITaskRepository)_inner).GetPersonIdsFromBBCC();
            }

            return default(IEnumerable<int>);
        }

        public delegate BvTasksEntity GetByPersonNotLockedInt32Delegate(int personSid);
        public GetByPersonNotLockedInt32Delegate GetByPersonNotLockedInt32;

        BvTasksEntity ITaskRepository.GetByPersonNotLocked(int personSid)
        {


            if (GetByPersonNotLockedInt32 != null)
            {
                return GetByPersonNotLockedInt32(personSid);
            } else if (_inner != null)
            {
                return ((ITaskRepository)_inner).GetByPersonNotLocked(personSid);
            }

            return default(BvTasksEntity);
        }

        public delegate IEnumerable<BvTasksEntity> GetBySurveyNotLockedInt32Delegate(int surveySid);
        public GetBySurveyNotLockedInt32Delegate GetBySurveyNotLockedInt32;

        IEnumerable<BvTasksEntity> ITaskRepository.GetBySurveyNotLocked(int surveySid)
        {


            if (GetBySurveyNotLockedInt32 != null)
            {
                return GetBySurveyNotLockedInt32(surveySid);
            } else if (_inner != null)
            {
                return ((ITaskRepository)_inner).GetBySurveyNotLocked(surveySid);
            }

            return default(IEnumerable<BvTasksEntity>);
        }

    }
}