using System;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Misc.Fakes
{
    public class StubICompanyInfo : ICompanyInfo 
    {
        private ICompanyInfo _inner;

        public StubICompanyInfo()
        {
            _inner = null;
        }

        public ICompanyInfo Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate int GetCompanyIdInt32StringDelegate(int id, string source);
        public GetCompanyIdInt32StringDelegate GetCompanyIdInt32String;

        int ICompanyInfo.GetCompanyId(int id, string source)
        {


            if (GetCompanyIdInt32String != null)
            {
                return GetCompanyIdInt32String(id, source);
            } else if (_inner != null)
            {
                return ((ICompanyInfo)_inner).GetCompanyId(id, source);
            }

            return default(int);
        }

        private int _CompanyId;
        public Func<int> CompanyIdGet;
        public Action<int> CompanyIdSetInt32;

        int ICompanyInfo.CompanyId
        {
            get
            {
                if (CompanyIdGet != null)
                {
                    return CompanyIdGet();
                } else if (_inner != null)
                {
                    return ((ICompanyInfo)_inner).CompanyId;
                }

                if (CompanyIdSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CompanyId;
                }

                return default(int);
            }

        }

        private string _CompanyName;
        public Func<string> CompanyNameGet;
        public Action<string> CompanyNameSetString;

        string ICompanyInfo.CompanyName
        {
            get
            {
                if (CompanyNameGet != null)
                {
                    return CompanyNameGet();
                } else if (_inner != null)
                {
                    return ((ICompanyInfo)_inner).CompanyName;
                }

                if (CompanyNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CompanyName;
                }

                return default(string);
            }

        }

        private string _CompanyAlias;
        public Func<string> CompanyAliasGet;
        public Action<string> CompanyAliasSetString;

        string ICompanyInfo.CompanyAlias
        {
            get
            {
                if (CompanyAliasGet != null)
                {
                    return CompanyAliasGet();
                } else if (_inner != null)
                {
                    return ((ICompanyInfo)_inner).CompanyAlias;
                }

                if (CompanyAliasSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CompanyAlias;
                }

                return default(string);
            }

        }

    }
}