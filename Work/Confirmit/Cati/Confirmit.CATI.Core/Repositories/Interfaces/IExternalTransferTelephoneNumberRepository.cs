using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IExternalTransferTelephoneNumberRepository
    {
        List<BvSpTransfer_GetExternalListEntity> GetAll();

        BvExternalTransferTelephoneNumberEntity TryGetById(int id);
        BvExternalTransferTelephoneNumberEntity TryGetByTelephoneNumber(string telNumber);

        int Insert(BvExternalTransferTelephoneNumberEntity number);
        void Update(BvExternalTransferTelephoneNumberEntity number);
        void Delete(int id);
    }
}
