using System;
using ConfirmitDialerInterface;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Telephony.Console;
using Confirmit.CATI.Common.ConsoleService.Abstract;

namespace Confirmit.CATI.Core.Telephony.Console.Fakes
{
    public class StubITransferService : ITransferService 
    {
        private ITransferService _inner;

        public StubITransferService()
        {
            _inner = null;
        }

        public ITransferService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate ConsoleTransferState GetTransferStateTransferStateBvActiveDialEntityDelegate(TransferState transferState, BvActiveDialEntity dial);
        public GetTransferStateTransferStateBvActiveDialEntityDelegate GetTransferStateTransferStateBvActiveDialEntity;

        ConsoleTransferState ITransferService.GetTransferState(TransferState transferState, BvActiveDialEntity dial)
        {


            if (GetTransferStateTransferStateBvActiveDialEntity != null)
            {
                return GetTransferStateTransferStateBvActiveDialEntity(transferState, dial);
            } else if (_inner != null)
            {
                return ((ITransferService)_inner).GetTransferState(transferState, dial);
            }

            return default(ConsoleTransferState);
        }

    }
}