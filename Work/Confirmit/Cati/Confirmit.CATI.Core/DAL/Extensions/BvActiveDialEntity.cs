using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using TransferType = ConfirmitDialerInterface.TransferType;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.DAL.Generated.Entity.Table
{
    public partial class BvActiveDialEntity
    {
        public DialState DialState
        {
            get { return (DialState) State; }
            set { State = (byte)value; }
        }

        public CallTypes CallType
        {
            get { return (CallTypes) Type; }
            set { Type = (byte)value; }
        }

        private ConsoleTransferState _transferState;

        public ConsoleTransferState TransferState
        {
            get
            {
                if (_transferState == null && JsonTransferState != null)
                {
                    _transferState = JsonConvert.DeserializeObject<ConsoleTransferState>(JsonTransferState);
                }
                return _transferState;
            }
            set
            {
                _transferState = value;
                JsonTransferState = JsonConvert.SerializeObject(value);
            }
        }

        public TransferType? DialTransferType
        {
            get { return (TransferType?)TransferType; }
            set { TransferType = (byte?)value; }
        }


        partial void OnBeforeGetJsonTransferState()
        {
            if (_transferState != null)
            {
                m_jsontransferstate = JsonConvert.SerializeObject(_transferState);
            }
        }

        partial void OnBeforeSetJsonTransferState()
        {
            _transferState = null;
        }

    }
}
