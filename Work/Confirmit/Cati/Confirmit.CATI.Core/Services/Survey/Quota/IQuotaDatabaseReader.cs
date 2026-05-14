using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.Survey.Quota
{
    public interface IQuotaDatabaseReader
    {
        /// <summary>
        /// returns sorted list of using columns for all quotas in survey
        /// </summary>
        /// <param name="surveySid">survey id</param>
        IEnumerable<string> GetAllFields(int surveySid);

        /// <summary>
        /// returns collection of quota info
        /// </summary>
        /// <param name="surveySid">survey id</param>
        IEnumerable<ClrQuotaInfo> GetQuotas(int surveySid);

        IEnumerable<string> GetQuotaFields(int surveySid, int quotaId);

        IEnumerable<string> GetFieldPrecodes(int surveySid, int quotaId, string fieldName);

        Dictionary<string, HashSet<string>> GetFieldPrecodes(int surveySid, int quotaId);

        IEnumerable<QuotaCellInfo> GetQuotaCells(int surveySid, int quotaId, string[] fields, bool isSupportOptimisticQuota);
    }
}
