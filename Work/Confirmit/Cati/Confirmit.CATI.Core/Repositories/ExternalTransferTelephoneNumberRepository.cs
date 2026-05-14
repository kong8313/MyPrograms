using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class ExternalTransferTelephoneNumberRepository : IExternalTransferTelephoneNumberRepository
    {
        public List<BvSpTransfer_GetExternalListEntity> GetAll()
        {
            return BvSpTransfer_GetExternalListAdapter.ExecuteEntityList();
        }

        public BvExternalTransferTelephoneNumberEntity TryGetById(int id)
        {
            return BvExternalTransferTelephoneNumberAdapter.GetByCondition("ID = @ID", new SqlParameter("@ID", id)).SingleOrDefault();
        }

        public BvExternalTransferTelephoneNumberEntity TryGetByTelephoneNumber(string telNumber)
        {
            return BvExternalTransferTelephoneNumberAdapter.GetByCondition("TelephoneNumber = @TelephoneNumber", new SqlParameter("@TelephoneNumber", telNumber)).SingleOrDefault();
        }

        public int Insert(BvExternalTransferTelephoneNumberEntity entity)
        {
            return BvExternalTransferTelephoneNumberAdapter.InsertWithReturnIdentityValue(entity);
        }

        public void Update(BvExternalTransferTelephoneNumberEntity entity)
        {
            BvExternalTransferTelephoneNumberAdapter.Update(entity);
        }

        public void Delete(int id)
        {
            BvExternalTransferTelephoneNumberAdapter.DeleteByCondition("ID = @ID", new SqlParameter("@ID", id));
        }
    }
}
