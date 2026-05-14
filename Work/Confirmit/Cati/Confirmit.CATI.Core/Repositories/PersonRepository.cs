using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ISqlTableUpdatedPublisher _sqlTableUpdatedPublisher;

        public PersonRepository(ISqlTableUpdatedPublisher sqlTableUpdatedPublisher)
        {
            _sqlTableUpdatedPublisher = sqlTableUpdatedPublisher;
        }

        [NotNull]
        BvPersonEntity IPersonRepository.GetById(int sid)
        {
            return GetById(sid);
        }

        [CanBeNull]
        BvPersonEntity IPersonRepository.TryGetById(int sid)
        {
            return TryGetById(sid);
        }

        [NotNull]
        BvPersonEntity IPersonRepository.GetByName(string name)
        {
            return GetByName(name);
        }

        [CanBeNull]
        BvPersonEntity IPersonRepository.TryGetByName(string name)
        {
            return TryGetByName(name);
        }

        [CanBeNull]
        List<BvPersonEntity> IPersonRepository.GetAll()
        {
            return GetAll();
        }

        [CanBeNull]
        List<BvPersonEntity> IPersonRepository.GetByType(AgentType type)
        {
            return GetByType(type);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        [NotNull]
        public static BvPersonEntity GetById(int sid)
        {
            var interviewer = TryGetById(sid);

            if (interviewer == null)
            {
                throw new InterviewerNotFoundException(sid);
            }

            return interviewer;
        }

        [CanBeNull]
        public static BvPersonEntity TryGetById(int sid)
        {
            var interviewer = BvPersonCache.Instance.GetBySID(sid);

            return interviewer;
        }

        [NotNull]
        public static BvPersonEntity GetByName(string name)
        {
            var interviewer = TryGetByName(name);

            if (interviewer == null)
            {
                throw new InterviewerNotFoundException(name);
            }

            return interviewer;
        }

        [CanBeNull]
        public static BvPersonEntity TryGetByName(string name)
        {
            var interviewer = BvPersonCache.Instance.GetByName(name);

            return interviewer;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static List<BvPersonEntity> GetByType(AgentType type)
        {
            return BvPersonAdapter.GetByCondition("[Type] = @Type", new SqlParameter("@Type", (byte)type));
        }

        public static List<BvPersonEntity> GetAll()
        {
            return BvPersonAdapter.GetAll();
        }

        public static List<BvPersonEntity> GetAll(int callCenterId)
        {
            return BvPersonAdapter.GetByCondition("[CallCenterID] = @CallCenterID", new SqlParameter("@CallCenterID", callCenterId));
        }

        public static List<BvPersonEntity> GetAllNotAssignedOnCallGroups()
        {
            return BvPersonAdapter.GetByCondition("[CallGroupID] IS NULL");
        }

        public static List<BvPersonEntity> GetAllAssignedOnCallGroup(int callGroupId)
        {
            return BvPersonAdapter.GetByCondition("[CallGroupID] = @CallGroupID", new SqlParameter("@CallGroupID", callGroupId));
        }

        // TODO: assing totalCount.
        public static List<BvSpGetPersonsListPageEntity> GetPage(
            string parentGroupsIDs,
            PagingArgs pagingArgs,
            int callCenterId,
            out int totalCount)
        {
            return BvSpGetPersonsListPageAdapter.ExecuteEntityList(
                parentGroupsIDs,
                pagingArgs.PageIndex,
                pagingArgs.PageSize,
                pagingArgs.SortField,
                pagingArgs.SortOrderAsc,
                SearchManager.GetSqlCondition(pagingArgs.SearchParameters),
                callCenterId,
                out totalCount
            );
        }

        public int Insert([NotNull] BvPersonEntity person)
        {
            if (person.SID != 0)
            {
                throw ExceptionManager.NewArgumentException("SID");
            }

            person.SID = SiteService.GetNewSid();

            BvSpPerson_InsertAdapter.ExecuteNonQuery(
                person.SID,
                person.Name,
                person.FullName,
                person.Description,
                person.ManualSelection,
                person.AssignmentsListMode,
                person.PwdSaltTxt,
                person.CallGroupID,
                person.CallCenterID,
                person.Location,
                person.DialTypeId,
                person.Type,
                person.EnableSoftphoneIntegration,
                person.PasswordNeedsChange,
                person.Attribute1,
                person.Attribute2,
                person.Attribute3,
                person.Attribute4,
                person.Attribute5);

            //TODO: Do we need to add person in the root CATI group while inserting it.

            BvSpPerson_SpinUpAdapter.ExecuteNonQuery(person.SID);
            RefreshCache();
            
            return person.SID;
        }

        public static void Update([NotNull] BvPersonEntity person, bool updateCache = true)
        {
            if (person.SID == 0)
            {
                throw ExceptionManager.NewArgumentException("SID");
            }

            // TODO: BvPersonAdapter must return ROWCOUNT, PersonNotFoundException must be thrown if rowcount = 0
            BvPersonAdapter.Update(person);

            if (updateCache)
            {
                RefreshCache();
            }
        }

        public static void RefreshCache()
        {
            BvPersonCache.Instance.OnTableChanged();
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishPersonUpdated();
        }

        void IPersonRepository.Update([NotNull] BvPersonEntity person, bool updateCache)
        {
            Update(person, updateCache);
        }

        public void Delete(int sid, bool updateCache)
        {
            if (sid == 0)
            {
                throw ExceptionManager.NewArgumentException("sid");
            }

            var person = GetById(sid);

            var evt = new DeleteInterviewerEvent(sid, person.Name, (AgentType)person.Type);

            BvSpPerson_DeleteAdapter.ExecuteNonQuery(sid);

            if (updateCache)
            {
                RefreshCache();
            }
            
            evt.Finish();
        }
    }
}
