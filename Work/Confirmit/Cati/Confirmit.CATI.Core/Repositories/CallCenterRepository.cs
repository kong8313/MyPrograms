using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.CallCenters;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Timezones;

namespace Confirmit.CATI.Core.Repositories
{
    public class CallCenterRepository : ICallCenterRepository
    {
        private readonly Lazy<ISqlTableUpdatedPublisher> _sqlTableUpdatedPublisher;

        public CallCenterRepository()
        {
            _sqlTableUpdatedPublisher = new Lazy<ISqlTableUpdatedPublisher>(()=> ServiceLocator.Resolve<ISqlTableUpdatedPublisher>());
        }

        public BvCallCenterEntity Get(int id)
        {
            return BvCallCenterCache.Instance.GetByID(id);
        }

        public BvCallCenterEntityWithDialerIds GetCallCenterWithDialers(int id)
        {
            var callCenter = BvCallCenterCache.Instance.GetByID(id);
            var dialers = BvDialerToCallCenterAdapter.GetByCondition(
                "[CallCenterId] = @CallCenterId",
                new SqlParameter("@CallCenterId", callCenter.ID));
            return new BvCallCenterEntityWithDialerIds(callCenter, dialers.Select(x => x.DialerId).ToArray());
        }

        public BvCallCenterEntity Default
        {
            get
            {
                return BvCallCenterCache.Instance.GetAll().Single(callCenter => callCenter.IsDefault);
            }
        }

        public List<BvCallCenterEntity> GetAssignedToSurvey(int surveyId)
        {
            return BvCallCenterAdapter.ReadList(BvSpCallCenter_ListOfAssignedToSurveyAdapter.ExecuteReader(surveyId));
        }

        public List<BvCallCenterEntity> GetAll()
        {
            return BvCallCenterCache.Instance.GetAll();
        }

        public List<BvCallCenterEntityWithDialerIds> GetAllWithDialerIds()
        {
            return BvCallCenterAdapterEx.GetByCondition("");
        }

        public void Insert(BvCallCenterEntity entity)
        {
            InsertWithCacheUpdate(entity);
            if (entity.DialerId > 0)
            {
                BvDialerToCallCenterAdapter.Insert(new BvDialerToCallCenterEntity
                {
                    CallCenterId = entity.ID,
                    DialerId = entity.DialerId,
                });
            }
        }


        public void Insert(BvCallCenterEntityWithDialerIds entity)
        {
            InsertWithCacheUpdate(entity);

            foreach (var dialerId in entity.DialerIds)
            {
                BvDialerToCallCenterAdapter.Insert(new BvDialerToCallCenterEntity
                {
                    CallCenterId = entity.ID,
                    DialerId = dialerId,
                });
            }
        }

        public void Update(BvCallCenterEntity entity)
        {
            UpdateWithCacheUpdate(entity);
        }

        public void Update(BvCallCenterEntityWithDialerIds entity, int[] newDialerIds, int[] oldDialerIds)
        {
            UpdateWithCacheUpdate(entity, newDialerIds);

            var dialerIdsToAdd = newDialerIds.Except(oldDialerIds);
            var dialerIdsToDelete = oldDialerIds.Except(newDialerIds);

            foreach (var dialerId in dialerIdsToAdd)
            {
                BvDialerToCallCenterAdapter.Insert(new BvDialerToCallCenterEntity
                {
                    CallCenterId = entity.ID,
                    DialerId = dialerId,
                });
            }

            foreach (var dialerId in dialerIdsToDelete)
            {
                BvDialerToCallCenterAdapter.DeleteByCondition(
                "[CallCenterId] = @Id AND [DialerId] = @DialerId",
                new SqlParameter("@Id", entity.ID),
                new SqlParameter("@DialerId", dialerId)
                );
            }
        }

        public void Delete(int id, int moveToCallCenterId, InterviewerActionOnCallCenterDelete interviewerAction)
        {
            var evt = new DeleteCallCenterEvent(id, Get(id).Name, moveToCallCenterId, interviewerAction);

            BvSpCallCenter_DeleteAdapter.ExecuteNonQuery(id, moveToCallCenterId, (int)interviewerAction);

            evt.Finish();

            BvCallCenterCache.Instance.OnTableChanged();
            _sqlTableUpdatedPublisher.Value.PublishCallCenterUpdated();
        }

        private void InsertWithCacheUpdate(BvCallCenterEntity entity)
        {
            CheckLocalTimezone(entity);

            var evt = new CreateCallCenterEvent(entity.ID, entity.Name);
            BvSpCallCenter_InsertAdapter.ExecuteNonQuery(entity.Name, entity.Description, entity.LocalTimezoneId, entity.DialerId, entity.HidePii, out var res);
            entity.ID = (byte)res;

            evt.ObjectId = entity.ID;
            evt.Finish();

            BvCallCenterCache.Instance.OnTableChanged();
            _sqlTableUpdatedPublisher.Value.PublishCallCenterUpdated();
        }

        private void UpdateWithCacheUpdate(BvCallCenterEntity entity, int[] newDialerIds = null)
        {
            CheckLocalTimezone(entity);
            
            var evt = new UpdateCallCenterEvent(entity.ID, entity.Name, newDialerIds);

            BvCallCenterAdapter.Update(entity);

            evt.Finish();

            BvCallCenterCache.Instance.OnTableChanged();
            _sqlTableUpdatedPublisher.Value.PublishCallCenterUpdated();
        }

        private void CheckLocalTimezone(BvCallCenterEntity entity)
        {
            if (TimezoneManager.ActiveTimezonesList.Any(tz => tz.ID == entity.LocalTimezoneId) == false)
            {
                throw ExceptionManager.NewInternalErrorException(
                    "Provided time zone id ({0}) is not valid. Time zone should exist and be active.",
                    entity.LocalTimezoneId);
            }
        }
    }
}
