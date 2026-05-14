using System;
using Confirmit.CATI.Supervisor.Core.UsersApi;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.UsersApi.Fakes
{
    public class StubIUsersApiService : IUsersApiService 
    {
        private IUsersApiService _inner;

        public StubIUsersApiService()
        {
            _inner = null;
        }

        public IUsersApiService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IEnumerable<ConfirmitUser> GetUsersByNameStringDelegate(string userName);
        public GetUsersByNameStringDelegate GetUsersByNameString;

        IEnumerable<ConfirmitUser> IUsersApiService.GetUsersByName(string userName)
        {


            if (GetUsersByNameString != null)
            {
                return GetUsersByNameString(userName);
            } else if (_inner != null)
            {
                return ((IUsersApiService)_inner).GetUsersByName(userName);
            }

            return default(IEnumerable<ConfirmitUser>);
        }

    }
}