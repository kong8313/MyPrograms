using System;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubICompanyRepository : ICompanyRepository 
    {
        private ICompanyRepository _inner;

        public StubICompanyRepository()
        {
            _inner = null;
        }

        public ICompanyRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SetCatiSqlServerIdInt32NullableOfInt32Delegate(int companyId, int? sqlServerId);
        public SetCatiSqlServerIdInt32NullableOfInt32Delegate SetCatiSqlServerIdInt32NullableOfInt32;

        void ICompanyRepository.SetCatiSqlServerId(int companyId, int? sqlServerId)
        {

            if (SetCatiSqlServerIdInt32NullableOfInt32 != null)
            {
                SetCatiSqlServerIdInt32NullableOfInt32(companyId, sqlServerId);
            } else if (_inner != null)
            {
                ((ICompanyRepository)_inner).SetCatiSqlServerId(companyId, sqlServerId);
            }
        }

    }
}