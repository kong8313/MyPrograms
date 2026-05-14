namespace Confirmit.CATI.Core.DAL.Framework
{
    public interface ITableCache
    {
        /// <summary>
        /// Returns cached table name.
        /// Needed to inform DatabaseTransactionScope 
        /// what tables changed in transaction.
        /// </summary>
        string CachedTableName
        { 
            get;
        }

        /// <summary>
        /// If called inside transaction (inside DatabaseTransactionScope) then 
        /// registers cache to be notified after transaction successfull commit.
        /// After transaction commited DatabaseTransactionScope will call OnCacheExpired.
        /// 
        /// If called outside transaction (outside DatabaseTransactionScope) then
        /// expires the cache (calls OnCacheExpired immidiatelly.)
        /// </summary>
        void OnTableChanged();

        /// <summary>
        /// Sets expired state.
        /// Cache will read new data from the database during
        /// next attempt to read data. Should be called ONLY outside transaction
        /// or from DatabaseTransactionScope after successfull commit.
        /// </summary>
        void OnCacheExpired();
    }
}
