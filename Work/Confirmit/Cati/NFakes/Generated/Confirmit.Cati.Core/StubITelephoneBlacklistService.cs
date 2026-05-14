using System;
using Confirmit.CATI.Core.Services.Interfaces;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubITelephoneBlacklistService : ITelephoneBlacklistService 
    {
        private ITelephoneBlacklistService _inner;

        public StubITelephoneBlacklistService()
        {
            _inner = null;
        }

        public ITelephoneBlacklistService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool IsTelephoneNumberFilteredByBlacklistStringDelegate(string telephoneNumber);
        public IsTelephoneNumberFilteredByBlacklistStringDelegate IsTelephoneNumberFilteredByBlacklistString;

        bool ITelephoneBlacklistService.IsTelephoneNumberFilteredByBlacklist(string telephoneNumber)
        {


            if (IsTelephoneNumberFilteredByBlacklistString != null)
            {
                return IsTelephoneNumberFilteredByBlacklistString(telephoneNumber);
            } else if (_inner != null)
            {
                return ((ITelephoneBlacklistService)_inner).IsTelephoneNumberFilteredByBlacklist(telephoneNumber);
            }

            return default(bool);
        }

        public delegate List<string> GetBlacklistedNumbersIEnumerableOfStringDelegate(IEnumerable<string> phoneNumbers);
        public GetBlacklistedNumbersIEnumerableOfStringDelegate GetBlacklistedNumbersIEnumerableOfString;

        List<string> ITelephoneBlacklistService.GetBlacklistedNumbers(IEnumerable<string> phoneNumbers)
        {


            if (GetBlacklistedNumbersIEnumerableOfString != null)
            {
                return GetBlacklistedNumbersIEnumerableOfString(phoneNumbers);
            } else if (_inner != null)
            {
                return ((ITelephoneBlacklistService)_inner).GetBlacklistedNumbers(phoneNumbers);
            }

            return default(List<string>);
        }

        public delegate string NormalizeTelephoneNumberStringDelegate(string telephoneNumber);
        public NormalizeTelephoneNumberStringDelegate NormalizeTelephoneNumberString;

        string ITelephoneBlacklistService.NormalizeTelephoneNumber(string telephoneNumber)
        {


            if (NormalizeTelephoneNumberString != null)
            {
                return NormalizeTelephoneNumberString(telephoneNumber);
            } else if (_inner != null)
            {
                return ((ITelephoneBlacklistService)_inner).NormalizeTelephoneNumber(telephoneNumber);
            }

            return default(string);
        }

    }
}