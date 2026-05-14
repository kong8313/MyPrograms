using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Validators.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class PersonGroupRepository : IPersonGroupRepository
    {
        private readonly IPersonGroupValidator _validator;
        private readonly ISqlTableUpdatedPublisher _sqlTableUpdatedPublisher;

        public PersonGroupRepository(IPersonGroupValidator validator,
            ISqlTableUpdatedPublisher sqlTableUpdatedPublisher)
        {
            _validator = validator;
            _sqlTableUpdatedPublisher = sqlTableUpdatedPublisher;
        }

        [NotNull]
        BvPersonGroupEntity IPersonGroupRepository.GetById(int sid)
        {
            return GetById(sid);
        }

        [CanBeNull]
        BvPersonGroupEntity IPersonGroupRepository.TryGetById(int sid)
        {
            return TryGetById(sid);
        }

        [NotNull]
        BvPersonGroupEntity IPersonGroupRepository.GetByName(string name)
        {
            return GetByName(name);
        }

        [CanBeNull]
        BvPersonGroupEntity IPersonGroupRepository.TryGetByName(string name)
        {
            return TryGetByName(name);
        }

        List<BvPersonGroupEntity> IPersonGroupRepository.GetAll()
        {
            return BvPersonGroupCache.Instance.GetAll();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        [NotNull]
        public static List<BvPersonGroupEntity> GetAll()
        {
            return BvPersonGroupCache.Instance.GetAll();
        }
        
        [NotNull]
        public static BvPersonGroupEntity GetById(int sid)
        {
            var interviewerGroup = TryGetById(sid);

            if (interviewerGroup == null)
            {
                throw new InterviewerGroupNotFoundException(sid);
            }

            return interviewerGroup;
        }

        [CanBeNull]
        public static BvPersonGroupEntity TryGetById(int sid)
        {
            return BvPersonGroupCache.Instance.GetBySID(sid);
        }

        [NotNull]
        public static BvPersonGroupEntity GetByName(string name)
        {
            var interviewerGroup = TryGetByName(name);

            if (interviewerGroup == null)
            {
                throw new InterviewerGroupNotFoundException(name);
            }

            return interviewerGroup;
        }

        [CanBeNull]
        public static BvPersonGroupEntity TryGetByName(string name)
        {
            return BvPersonGroupCache.Instance.GetByName(name);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        public int Insert(BvPersonGroupEntity personGroup)
        {
            if (personGroup.SID != 0)
            {
                throw ExceptionManager.NewArgumentException("SID");
            }

            if (!_validator.IsValid(personGroup))
            {
                throw ExceptionManager.NewArgumentException("personGroup");
            }

            personGroup.SID = SiteService.GetNewSid();

            BvSpPersonGroup_InsertAdapter.ExecuteNonQuery(
                personGroup.SID,
                personGroup.Name,
                personGroup.Description,
                personGroup.InboundCallBehavior,
                personGroup.CallTransferBehavior,
                personGroup.IsAdministrative);

            BvPersonGroupCache.Instance.OnTableChanged();
            _sqlTableUpdatedPublisher.PublishPersonGroupUpdated();
            
            return personGroup.SID;
        }

        public void Update(BvPersonGroupEntity personGroup)
        {
            if (personGroup.SID == 0)
            {
                throw ExceptionManager.NewArgumentException("SID");
            }

            if (!_validator.IsValid(personGroup))
            {
                throw ExceptionManager.NewArgumentException("personGroup");
            }

            BvSpPersonGroup_UpdateAdapter.ExecuteNonQuery(
                personGroup.SID,
                personGroup.Name,
                personGroup.Description,
                personGroup.InboundCallBehavior,
                personGroup.CallTransferBehavior,
                personGroup.IsAdministrative);

            BvPersonGroupCache.Instance.OnTableChanged();
            _sqlTableUpdatedPublisher.PublishPersonGroupUpdated();
        }

        public static void Delete(int sid)
        {
            if (sid == 0)
            {
                throw ExceptionManager.NewArgumentException("sid");
            }

            var evt = new DeleteInterviewerGroupEvent(sid, GetById(sid).Name);

            BvSpPersonGroup_DeleteAdapter.ExecuteNonQuery(sid);

            BvPersonGroupCache.Instance.OnTableChanged();
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishPersonGroupUpdated();

            evt.Finish();
        }
    }
}
