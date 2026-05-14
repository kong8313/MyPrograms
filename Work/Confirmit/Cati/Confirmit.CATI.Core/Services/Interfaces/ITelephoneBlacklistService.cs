using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface ITelephoneBlacklistService
    {
        bool IsTelephoneNumberFilteredByBlacklist(string telephoneNumber);

        List<string> GetBlacklistedNumbers(IEnumerable<string> phoneNumbers);

        string NormalizeTelephoneNumber(string telephoneNumber);
    }
}
