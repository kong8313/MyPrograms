using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class TimezoneRepository : ITimezoneRepository
    {
        private readonly ISqlTableUpdatedPublisher _sqlTableUpdatedPublisher;
        private readonly IConnectionStrings _connectionStrings;
        
        public TimezoneRepository()
        {
            _sqlTableUpdatedPublisher = ServiceLocator.Resolve<ISqlTableUpdatedPublisher>();
            _connectionStrings = ServiceLocator.Resolve<IConnectionStrings>();
        }

        public static BvTimezoneEntity GetById(int sid)
        {
            return new TimezoneRepository().Get(sid);
        }

        public BvTimezoneEntity Get(int timezoneId)
        {
            return BvTimezoneCache.Instance.GetByID(timezoneId);
        }

        public BvTimezoneEntity GetMasterTimezone(int timezoneId)
        {
            return ConvertToTimezoneEntity(BvTimezoneMasterAdapter.GetByCondition($"ID={timezoneId}").First());
        }

        List<BvTimezoneEntity> ITimezoneRepository.GetActiveList()
        {
            return BvTimezoneCache.Instance.GetAll();
        }

        public List<BvTimezoneEntity> GetMasterList()
        {
            return BvTimezoneMasterAdapter.GetAll().Select(ConvertToTimezoneEntity).ToList();
        }

        public List<BvTimezoneEntity> GetMasterListFromDefaultDatabase()
        {
            DatabaseEngine dbEngine = new DatabaseEngine(_connectionStrings.DefaultInstanceConnectionString);
            var timezones = dbEngine.ExecuteDataTableInNewConnection<DataTable>("SELECT * FROM BvTimezoneMaster", CommandType.Text);

            return timezones.AsEnumerable().Select(ConvertToTimezoneEntity).ToList();
        }
            
        public void InsertMasterEntity(BvTimezoneEntity entity)
        {
            var masterEntity = ConvertToTimezoneMasterEntity(entity);
            BvTimezoneMasterAdapter.Insert(masterEntity);
        }

        public void UpdateMasterEntity(BvTimezoneEntity entity, bool isActiveTimezone)
        {
            var masterEntity = ConvertToTimezoneMasterEntity(entity);
            BvTimezoneMasterAdapter.Update(masterEntity);

            if (isActiveTimezone)
            {
                BvTimezoneAdapter.Update(entity);
                UpdateCustomTimezonesForParent(entity);
            }
            BvTimezoneCache.Instance.OnTableChanged();
            _sqlTableUpdatedPublisher.PublishTimeZoneUpdated();
        }

        private void UpdateCustomTimezonesForParent(BvTimezoneEntity parentTimezone)
        {
            var customTimezones = GetCustomTimezones(parentTimezone.ID);
            foreach (var customTimezone in customTimezones)
            {
                var updatedCustomTimezone = parentTimezone;
                updatedCustomTimezone.ID = customTimezone.ID;
                updatedCustomTimezone.ParentID = customTimezone.ParentID;
                updatedCustomTimezone.Name = customTimezone.Name;

                BvTimezoneAdapter.Update(updatedCustomTimezone);
            }
        }

        public List<BvTimezoneEntity> GetCustomTimezones(int parentTimezoneId)
        {
            var allActiveTimezones = BvTimezoneCache.Instance.GetAll();
            return allActiveTimezones.Where(x => x.ParentID == parentTimezoneId).ToList();
        }

        public void InsertCustomTimezone(BvTimezoneEntity customTimezone)
        {
            BvTimezoneAdapter.Insert(customTimezone);
            BvTimezoneCache.Instance.OnTableChanged();
            _sqlTableUpdatedPublisher.PublishTimeZoneUpdated();
        }

        public void UpdateCustomTimezone(BvTimezoneEntity customTimezone)
        {
            BvTimezoneAdapter.Update(customTimezone);
            BvTimezoneCache.Instance.OnTableChanged();
            _sqlTableUpdatedPublisher.PublishTimeZoneUpdated();
        }

        private BvTimezoneEntity ConvertToTimezoneEntity(BvTimezoneMasterEntity entity)
        {
            var timezoneEntity = new BvTimezoneEntity
            {
                ID = entity.ID,
                Name = entity.Name,
                Bias = entity.Bias,
                DaylightType = entity.DaylightType,
                StandardName = entity.StandardName,
                StandardStart = entity.StandardStart,
                StandardDayOfWeek = entity.StandardDayOfWeek,
                StandardBias = entity.StandardBias,
                DaylightName = entity.DaylightName,
                DaylightStart = entity.DaylightStart,
                DaylightDayOfWeek = entity.DaylightDayOfWeek,
                DaylightBias = entity.DaylightBias
            };

            return timezoneEntity;
        }
        
        private BvTimezoneEntity ConvertToTimezoneEntity(DataRow entity)
        {
            var timezoneEntity = new BvTimezoneEntity
            {
                ID = entity.Field<int>("ID"),
                Name = entity.Field<string>("Name"),
                Bias = entity.Field<int>("Bias"),
                DaylightType = entity.Field<int>("DaylightType"),
                StandardName = entity.Field<string>("StandardName"),
                StandardStart = entity.Field<DateTime?>("StandardStart"),
                StandardDayOfWeek = entity.Field<int?>("StandardDayOfWeek"),
                StandardBias = entity.Field<int>("StandardBias"),
                DaylightName = entity.Field<string>("DaylightName"),
                DaylightStart = entity.Field<DateTime?>("DaylightStart"),
                DaylightDayOfWeek = entity.Field<int?>("DaylightDayOfWeek"),
                DaylightBias = entity.Field<int>("DaylightBias"),
            };

            return timezoneEntity;
        }
        
        private BvTimezoneMasterEntity ConvertToTimezoneMasterEntity(BvTimezoneEntity entity)
        {
            var timezoneMasterEntity = new BvTimezoneMasterEntity
            {
                ID = entity.ID,
                Name = entity.Name,
                Bias = entity.Bias,
                DaylightType = entity.DaylightType,
                StandardName = entity.StandardName,
                StandardStart = entity.StandardStart,
                StandardDayOfWeek = entity.StandardDayOfWeek,
                StandardBias = entity.StandardBias,
                DaylightName = entity.DaylightName,
                DaylightStart = entity.DaylightStart,
                DaylightDayOfWeek = entity.DaylightDayOfWeek,
                DaylightBias = entity.DaylightBias
            };

            return timezoneMasterEntity;
        }
    }
}