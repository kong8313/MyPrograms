using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        public static BvScheduleEntity GetById(int scheduleId)
        {
            return ServiceLocator.Resolve<IScheduleRepository>().GetById(scheduleId);
        }

        BvScheduleEntity IScheduleRepository.GetById(int scheduleId)
        {
            return BvScheduleCache.Instance.GetByScheduleID(scheduleId);
        }

        public static BvScheduleEntity GetByIdWithCheck(int sid)
        {
            var schedule = GetById(sid);

            if (schedule == null)
            {
                throw new InternalErrorException(string.Format("Schedule with SID '{0}' does not exist.", sid));
            }

            return schedule;
        }

        public static BvScheduleEntity GetByName(string name)
        {
            return ServiceLocator.Resolve<IScheduleRepository>().GetByName(name);
        }

        BvScheduleEntity IScheduleRepository.GetByName(string name)
        {
            return BvScheduleCache.Instance.GetByName(name);
        }

        public static bool IsNameUsed(string name)
        {
            return GetByName(name) != null;
        }

        public static List<BvScheduleEntity> GetAll()
        {
            return BvScheduleCache.Instance.GetAll().ToList();
        }

        public static List<BvSpSchedule_ListPageEntity> GetPage(
            PagingArgs pagingArgs,
            int timezoneID,
            out int totalCount)
        {
            return BvSpSchedule_ListPageAdapter.ExecuteEntityList(
                pagingArgs.PageIndex,
                pagingArgs.PageSize,
                pagingArgs.SortField,
                (pagingArgs.SortOrderAsc) ? 1 : 0,
                SearchManager.GetSqlCondition(pagingArgs.SearchParameters, timezoneID),
                out totalCount);
        }

        public static int Insert(BvScheduleEntity schedule)
        {
            if (schedule.ScheduleID != 0)
            {
                throw ExceptionManager.NewArgumentException("ScheduleID");
            }

            schedule.ScheduleID = SiteService.GetNewSid();

            return InsertWithSpecificId(schedule);
        }

        public static int InsertWithSpecificId(BvScheduleEntity schedule)
        {
            return ServiceLocator.Resolve<IScheduleRepository>().InsertWithSpecificId(schedule);
        }

        int IScheduleRepository.InsertWithSpecificId(BvScheduleEntity schedule)
        {
            if (schedule.ScheduleID == 0)
            {
                throw ExceptionManager.NewArgumentException("ScheduleID");
            }

            BvSpSchedule_InsertAdapter.ExecuteNonQuery(
                schedule.ScheduleID, schedule.Name, schedule.XmlUnderDev, schedule.ScriptSource, schedule.DesignStateGroupID);

            BvScheduleCache.Instance.OnTableChanged();

            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishScheduleUpdated();
            
            return schedule.ScheduleID;
        }

        public static void Update(BvScheduleEntity schedule)
        {
            UpdateByCondition(schedule, null);
        }

        public static void UpdateByCondition(
            BvScheduleEntity schedule, 
            string condition, 
            params SqlParameter[] parameters)
        {
            if (schedule.ScheduleID == 0)
            {
                throw ExceptionManager.NewArgumentException("ScheduleID");
            }

            // Scr_UpdDate must always be rewritten by current UTC time
            // otherwise scheduling relaunch will not work: old version
            // of scheduling assembly used.
            schedule.ModifyDate = System.DateTime.UtcNow;

            BvScheduleAdapter.UpdateByCondition(schedule, condition, parameters);

            BvScheduleCache.Instance.OnTableChanged();
            
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishScheduleUpdated();
        }

        public static void Delete(int scheduleId)
        {
            if (scheduleId == 0)
            {
                throw ExceptionManager.NewArgumentException("scheduleId");
            }

            BvSpSchedule_DeleteAdapter.ExecuteNonQuery(scheduleId);

            BvScheduleCache.Instance.OnTableChanged();
            
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishShiftsUpdated();
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishScheduleUpdated();
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishScheduleParamsUpdated();
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishSurveyUpdated();
        }
    }
}
