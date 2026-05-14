using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using System.Collections.Generic;
using Confirmit.CATI.Core.CallCenters;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubICallCenterRepository : ICallCenterRepository 
    {
        private ICallCenterRepository _inner;

        public StubICallCenterRepository()
        {
            _inner = null;
        }

        public ICallCenterRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvCallCenterEntity GetInt32Delegate(int id);
        public GetInt32Delegate GetInt32;

        BvCallCenterEntity ICallCenterRepository.Get(int id)
        {


            if (GetInt32 != null)
            {
                return GetInt32(id);
            } else if (_inner != null)
            {
                return ((ICallCenterRepository)_inner).Get(id);
            }

            return default(BvCallCenterEntity);
        }

        public delegate BvCallCenterEntityWithDialerIds GetCallCenterWithDialersInt32Delegate(int id);
        public GetCallCenterWithDialersInt32Delegate GetCallCenterWithDialersInt32;

        BvCallCenterEntityWithDialerIds ICallCenterRepository.GetCallCenterWithDialers(int id)
        {


            if (GetCallCenterWithDialersInt32 != null)
            {
                return GetCallCenterWithDialersInt32(id);
            } else if (_inner != null)
            {
                return ((ICallCenterRepository)_inner).GetCallCenterWithDialers(id);
            }

            return default(BvCallCenterEntityWithDialerIds);
        }

        public delegate List<BvCallCenterEntity> GetAssignedToSurveyInt32Delegate(int surveyId);
        public GetAssignedToSurveyInt32Delegate GetAssignedToSurveyInt32;

        List<BvCallCenterEntity> ICallCenterRepository.GetAssignedToSurvey(int surveyId)
        {


            if (GetAssignedToSurveyInt32 != null)
            {
                return GetAssignedToSurveyInt32(surveyId);
            } else if (_inner != null)
            {
                return ((ICallCenterRepository)_inner).GetAssignedToSurvey(surveyId);
            }

            return default(List<BvCallCenterEntity>);
        }

        public delegate List<BvCallCenterEntity> GetAllDelegate();
        public GetAllDelegate GetAll;

        List<BvCallCenterEntity> ICallCenterRepository.GetAll()
        {


            if (GetAll != null)
            {
                return GetAll();
            } else if (_inner != null)
            {
                return ((ICallCenterRepository)_inner).GetAll();
            }

            return default(List<BvCallCenterEntity>);
        }

        public delegate List<BvCallCenterEntityWithDialerIds> GetAllWithDialerIdsDelegate();
        public GetAllWithDialerIdsDelegate GetAllWithDialerIds;

        List<BvCallCenterEntityWithDialerIds> ICallCenterRepository.GetAllWithDialerIds()
        {


            if (GetAllWithDialerIds != null)
            {
                return GetAllWithDialerIds();
            } else if (_inner != null)
            {
                return ((ICallCenterRepository)_inner).GetAllWithDialerIds();
            }

            return default(List<BvCallCenterEntityWithDialerIds>);
        }

        public delegate void InsertBvCallCenterEntityDelegate(BvCallCenterEntity entity);
        public InsertBvCallCenterEntityDelegate InsertBvCallCenterEntity;

        void ICallCenterRepository.Insert(BvCallCenterEntity entity)
        {

            if (InsertBvCallCenterEntity != null)
            {
                InsertBvCallCenterEntity(entity);
            } else if (_inner != null)
            {
                ((ICallCenterRepository)_inner).Insert(entity);
            }
        }

        public delegate void InsertBvCallCenterEntityWithDialerIdsDelegate(BvCallCenterEntityWithDialerIds entity);
        public InsertBvCallCenterEntityWithDialerIdsDelegate InsertBvCallCenterEntityWithDialerIds;

        void ICallCenterRepository.Insert(BvCallCenterEntityWithDialerIds entity)
        {

            if (InsertBvCallCenterEntityWithDialerIds != null)
            {
                InsertBvCallCenterEntityWithDialerIds(entity);
            } else if (_inner != null)
            {
                ((ICallCenterRepository)_inner).Insert(entity);
            }
        }

        public delegate void UpdateBvCallCenterEntityDelegate(BvCallCenterEntity entity);
        public UpdateBvCallCenterEntityDelegate UpdateBvCallCenterEntity;

        void ICallCenterRepository.Update(BvCallCenterEntity entity)
        {

            if (UpdateBvCallCenterEntity != null)
            {
                UpdateBvCallCenterEntity(entity);
            } else if (_inner != null)
            {
                ((ICallCenterRepository)_inner).Update(entity);
            }
        }

        public delegate void UpdateBvCallCenterEntityWithDialerIdsArrayOfInt32ArrayOfInt32Delegate(BvCallCenterEntityWithDialerIds entity, int[] newDialerIds, int[] oldDialerIds);
        public UpdateBvCallCenterEntityWithDialerIdsArrayOfInt32ArrayOfInt32Delegate UpdateBvCallCenterEntityWithDialerIdsArrayOfInt32ArrayOfInt32;

        void ICallCenterRepository.Update(BvCallCenterEntityWithDialerIds entity, int[] newDialerIds, int[] oldDialerIds)
        {

            if (UpdateBvCallCenterEntityWithDialerIdsArrayOfInt32ArrayOfInt32 != null)
            {
                UpdateBvCallCenterEntityWithDialerIdsArrayOfInt32ArrayOfInt32(entity, newDialerIds, oldDialerIds);
            } else if (_inner != null)
            {
                ((ICallCenterRepository)_inner).Update(entity, newDialerIds, oldDialerIds);
            }
        }

        public delegate void DeleteInt32Int32InterviewerActionOnCallCenterDeleteDelegate(int id, int moveToCallCenterId, InterviewerActionOnCallCenterDelete interviewerAction);
        public DeleteInt32Int32InterviewerActionOnCallCenterDeleteDelegate DeleteInt32Int32InterviewerActionOnCallCenterDelete;

        void ICallCenterRepository.Delete(int id, int moveToCallCenterId, InterviewerActionOnCallCenterDelete interviewerAction)
        {

            if (DeleteInt32Int32InterviewerActionOnCallCenterDelete != null)
            {
                DeleteInt32Int32InterviewerActionOnCallCenterDelete(id, moveToCallCenterId, interviewerAction);
            } else if (_inner != null)
            {
                ((ICallCenterRepository)_inner).Delete(id, moveToCallCenterId, interviewerAction);
            }
        }

        private BvCallCenterEntity _Default;
        public Func<BvCallCenterEntity> DefaultGet;
        public Action<BvCallCenterEntity> DefaultSetBvCallCenterEntity;

        BvCallCenterEntity ICallCenterRepository.Default
        {
            get
            {
                if (DefaultGet != null)
                {
                    return DefaultGet();
                } else if (_inner != null)
                {
                    return ((ICallCenterRepository)_inner).Default;
                }

                if (DefaultSetBvCallCenterEntity == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Default;
                }

                return default(BvCallCenterEntity);
            }

        }

    }
}