using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Types;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Adapter.TableType;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    /// <summary>
    /// Class represents repository for the blacklist, contains methods for inserting, updating, deleting numbers from blacklist.
    /// </summary>
    public class TelephoneBlacklistRepository : ITelephoneBlacklistRepository
    {
        private const int ImportBatchSize = 10000;
        private const int ImportBulkTimeout  = 60 * 10;
        private const string TelephoneNumberMask = @"^[0-9]{1,255}[\*]?$";
        private const string SequenceName = "BvTelephoneBlacklistIdSequence";
        private const string SequenceFullName = "[dbo].[BvTelephoneBlacklistIdSequence]";

        /// <summary>
        /// Gets all telephone blacklist entities from the database.
        /// </summary>
        /// <returns>Telephone blacklist</returns>
        public List<BvTelephoneBlacklistEntity> GetAll()
        {
            return BvTelephoneBlacklistAdapter.GetAll();
        }

        /// <summary>
        /// Inserts telephone blacklist entity into the database.
        /// </summary>
        /// <param name="entity">The telephone Number Entity</param>
        public int Insert(BvTelephoneBlacklistEntity entity)
        {
            Check(entity);

            int id;

            BvSpTelephoneBlacklist_InsertAdapter.ExecuteNonQuery(entity.Type, entity.TelephoneNumber, entity.Comment, out id);

            return id;
        }

        /// <summary>
        /// Updates telephone blacklist entity in the database.
        /// </summary>
        /// <param name="entity">The telephone Number Entity</param>
        public void Update(BvTelephoneBlacklistEntity entity)
        {
            Check(entity);

            BvTelephoneBlacklistAdapter.Update(entity);
        }

        /// <summary>
        /// Deletes entire telephone blacklist.
        /// Returns number of records.
        /// </summary>
        public int DeleteAll()
        {
            var recordsDeleted = BvTelephoneBlacklistAdapter.DeleteByConditionAndOutput("1 = 1").Count;
            new SequenceProvider().RestartSequence(SequenceFullName);
            return recordsDeleted;
        }

        /// <summary>
        /// Deletes the list of telephone blacklist entities from the database.
        /// </summary>
        /// <param name="ids">Ids of telephone numbers entities to delete from blacklist</param>
        public void Delete(IEnumerable<int> ids)
        {
            if (ids == null)
            {
                throw ExceptionManager.NewArgumentException("ids");
            }
            
            BvTelephoneBlacklistAdapter.DeleteByCondition("EXISTS( SELECT 1 FROM @ids where Value = id)",
                BvIntArrayTypeAdapter.CreateSqlParameter("@ids", ids));
        }

        /// <summary>
        /// Imports numbers to the blacklist.
        /// </summary>
        /// <param name="entities">The list of telephone numbers entities</param>
        public Range<int> Import(List<BvTelephoneBlacklistEntity> entities)
        {
            var evt = new ImportTelephoneNumbersToBlacklistEvent(entities.Count());

            if (entities.Any(entity => !IsNumberValid(entity.TelephoneNumber)))
                throw new UserMessageException("The list cannot be uploaded because some numbers contain special symbols. " +
                                               "The telephone number should only contain digits 0-9 or the * symbol (which may be placed at the end of the number). " +
                                               "The total number length cannot be more than 255 characters.");

            int reservedItemsStartNumber = new SequenceProvider().ReserveRange(SequenceName,
                entities.Count);

            int id = reservedItemsStartNumber;
            foreach (var number in entities)
            {
                number.Id = id++;
                number.Timestamp = DateTime.UtcNow;
            }

            var blacklistTable = BvTelephoneBlacklistAdapter.CreateDataTable();
            DatabaseTools.BulkAdd(
                blacklistTable,
                BvTelephoneBlacklistAdapter.SaveEntity2DataTable,
                entities,
                ImportBatchSize,
                ImportBulkTimeout);

            evt.Details.ValidNumbersCount = entities.Count;

            evt.Finish();

            return new Range<int>(reservedItemsStartNumber, reservedItemsStartNumber + entities.Count - 1);
        }

        /// <summary>
        /// Validates telephony number 
        /// </summary>
        /// <param name="telephoneNumber">Telephony number for validation</param>
        /// <returns>True if number is correct otherwise false</returns>
        private bool IsNumberValid(string telephoneNumber)
        {
            return Regex.IsMatch(telephoneNumber, TelephoneNumberMask);
        }

        /// <summary>
        /// Gets the page of Telephone Blacklist.
        /// </summary>
        /// <param name="pageArguments">Page arguments</param>
        /// <param name="totalCount">The total count of records returned</param>
        /// <returns>The list of numbers for the page</returns>
        public List<BvTelephoneBlacklistEntity> GetPage(PagingArgs pageArguments, out int totalCount)
        {
            var reader = BvSpGetObjectsPageAdapter.ExecuteReader(
                pageArguments.PageIndex,
                pageArguments.PageSize,
                pageArguments.SortField,
                pageArguments.SortOrderAsc,
                BvTelephoneBlacklistAdapter.selectSql,
                "Id",
                SearchManager.GetSqlCondition(pageArguments.SearchParameters),
                null,
                out totalCount);

            return BvTelephoneBlacklistAdapter.ReadList(reader);
        }

        public BvTelephoneBlacklistEntity GetByDisplayPattern(string displayPattern)
        {
            var entity = new BvTelephoneBlacklistEntity() { DisplayPattern = displayPattern };
            
            return BvTelephoneBlacklistAdapter.GetByCondition("Type = @Type AND TelephoneNumber = @TelephoneNumber",
                new SqlParameter("@TelephoneNumber", entity.TelephoneNumber),
                new SqlParameter("@Type", entity.Type)).SingleOrDefault();
            
        }

        public BvTelephoneBlacklistEntity GetById(int id)
        {
            return BvTelephoneBlacklistAdapter.GetByCondition("Id = @Id", new SqlParameter("@Id", id))
                .SingleOrDefault();
        }

        public BvTelephoneBlacklistEntity GetByNumber(string telephoneNumber)
        {
            var type = 0;
            if (telephoneNumber.EndsWith("*"))
            {
                type = 1;
                telephoneNumber = telephoneNumber.TrimEnd('*');
            }

            return BvTelephoneBlacklistAdapter.GetByCondition("TelephoneNumber = @TelephoneNumber AND Type = @Type", 
                    new SqlParameter("@TelephoneNumber", telephoneNumber),
                    new SqlParameter("@Type", type))
                .SingleOrDefault();
        }
            
        private void Check(BvTelephoneBlacklistEntity entity)
        {
            if (IsNumberValid(entity.TelephoneNumber) == false)
            {
                throw new UserMessageException(String.Format(
                    "Telephone number '{0}' is invalid.", entity.TelephoneNumber));
            } 
            
            var exists = GetByDisplayPattern(entity.DisplayPattern);

            if (exists != null && exists.Id != entity.Id)
            {
                throw new UserMessageException(String.Format(
                    "Telephone number '{0}' already exists in the blacklist.", entity.TelephoneNumber));
            }
        }
    }
}
