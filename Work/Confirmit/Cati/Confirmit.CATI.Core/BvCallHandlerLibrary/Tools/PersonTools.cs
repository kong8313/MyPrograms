using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;

namespace BvCallHandlerLibrary.Tools
{
    class PersonTools
    {
        internal static int[] GetUserGroups(int personSID)
        {
            var result = BvSpGetUserGroupsAdapter.ExecuteEntityList(personSID);
            return result.Select(x => (int)x.GroupSID).ToArray();
        }

    }
}
