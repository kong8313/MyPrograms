using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Adapter.TableType;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services
{
    public class TelephoneBlacklistService : ITelephoneBlacklistService
    {
        /// <summary>
        /// Gets if specified telephone number exists in the black list.
        /// </summary>
        /// <param name="telephoneNumber">The telephone Number</param>
        /// <returns>If blacklist contains specified number</returns>
        public bool IsTelephoneNumberFilteredByBlacklist(string telephoneNumber)
        {
            if (String.IsNullOrEmpty(telephoneNumber))
            {
                return false;
            }

            return BvSpTelephoneBlacklist_FilterAdapter.ExecuteEntityList(
                       BvStringArrayTypeAdapter.CreateTable(new[] { telephoneNumber })).Single().IsFiltered > 0;
        }

        public List<string> GetBlacklistedNumbers(IEnumerable<string> phoneNumbers)
        {
            return BvSpTelephoneBlacklist_FilterAdapter.ExecuteEntityList(
                    BvStringArrayTypeAdapter.CreateTable(phoneNumbers))
                .Where(x => x.IsFiltered > 0)
                .Select(f => f.TelephoneNumber)
                .ToList();
        }

        public string NormalizeTelephoneNumber(string telephoneNumber)
        {
            return new string(telephoneNumber.Where(char.IsDigit).ToArray());
        }
    }
}
