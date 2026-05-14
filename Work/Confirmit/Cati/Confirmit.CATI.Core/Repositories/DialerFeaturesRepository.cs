using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    /// <summary>
    /// Class contains methods that implement operations with dialer features.
    /// </summary>
    public class DialerFeaturesRepository : IDialerFeaturesRepository
    {
        public List<BvDialerFeaturesEntity> GetAll(int dialerId)
        {
            var entities = BvDialerFeaturesAdapter.GetByCondition(
                "[DialerId] = @DialerId",
                new SqlParameter("@DialerId", dialerId));

            return entities;
        }

        private const int ImportBatchSize = 10000;
        private const int ImportBulkTimeout = 60 * 10;

        public void UpdateOrInsert([NotNull] BvDialerFeaturesEntity dialerFeaturesEntity)
        {
            BvDialerFeaturesAdapter.Merge(dialerFeaturesEntity);
        }

        public void Delete(int dialerId, string name)
        {
            BvDialerFeaturesAdapter.DeleteByCondition(
                "[DialerId] = @DialerId AND [Name] = @Name",
                new SqlParameter("@DialerId", dialerId),
                new SqlParameter("@Name", name));
        }

        public void DeleteAll(int dialerId)
        {
            BvDialerFeaturesAdapter.DeleteByCondition(
                "[DialerId] = @DialerId",
                new SqlParameter("@DialerId", dialerId));
        }
    }
}
