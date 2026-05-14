using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemState;

namespace Confirmit.CATI.Core.Repositories
{
    public class SystemStateRepository : ISystemStateRepository
    {
        public string Get(string systemStateName)
        {
            var state = GetState(systemStateName);
            return state != null ? state.Value : null;
        }

        public void Set(string systemStateName, string value)
        {
            var entity = GetState(systemStateName);
            if (entity == null)
            {
                entity = new BvSystemStateEntity
                {
                    SystemName = systemStateName,
                    Value = value
                };

                BvSystemStateAdapter.Insert(entity);
            }
            else
            {
                entity.Value = value;
                BvSystemStateAdapter.Update(entity);
            }
        }

        public DateTime? GetReviewerLastInterviewStatusChange()
        {
            var lastInterviewStatusChange =
                Get(SystemStateTypes.ReviewerLastInterviewStatusChange);

            DateTime? lastInterviewStatusChangeDate = null;
            if (!string.IsNullOrEmpty(lastInterviewStatusChange))
            {
                lastInterviewStatusChangeDate = DateTime.ParseExact(lastInterviewStatusChange, "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            }

            return lastInterviewStatusChangeDate;
        }

        public void SetReviewerLastInterviewStatusChange(DateTime value)
        {
            Set(SystemStateTypes.ReviewerLastInterviewStatusChange, value.ToString("o"));
        }

        private BvSystemStateEntity GetState(string systemStateName)
        {
            return BvSystemStateAdapter
                .GetByCondition("[SystemName] = @SystemName",
                    new SqlParameter("@SystemName", systemStateName))
                .FirstOrDefault();
        }
    }
}