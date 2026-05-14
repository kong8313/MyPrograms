using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.UsersApi
{
    public interface IUsersApiService
    {
        IEnumerable<ConfirmitUser> GetUsersByName(string userName);
    }
}