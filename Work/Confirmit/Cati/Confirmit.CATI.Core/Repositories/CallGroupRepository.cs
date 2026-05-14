using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class CallGroupRepository : ICallGroupRepository
    {
        public BvCallGroupEntity Get(string name)
        {
            return BvCallGroupAdapter.GetByCondition("Name = @Name", new SqlParameter("@Name", name)).SingleOrDefault();
        }

        public BvCallGroupEntity Get(int callGroupId)
        {
            return BvCallGroupAdapter.GetByCondition("Id = @Id", new SqlParameter("@Id", callGroupId)).SingleOrDefault();
        }

        public void Insert(BvCallGroupEntity callGroup)
        {
            var evt = new CallGroupInsertEvent(callGroup.Name);

            BvCallGroupAdapter.Insert(callGroup);
            callGroup.Id = this.Get(callGroup.Name).Id;
            
            evt.ObjectId = callGroup.Id;
            
            evt.Finish();
        }

        public void Update(BvCallGroupEntity callGroup)
        {
            var evt = new CallGroupUpdateEvent(callGroup.Id, callGroup.Name);

            BvCallGroupAdapter.Update(callGroup);
            
            evt.Finish();
        }

        public void Delete(int groupId)
        {
            var evt = new CallGroupDeleteEvent(groupId);

            BvCallGroupAdapter.DeleteByCondition("Id = @Id", new SqlParameter("@Id", groupId));

            evt.Finish();
        }

        public List<BvCallGroupEntity> GetAllGroups()
        {
            return BvCallGroupAdapter.GetAll();
        }
    }
}