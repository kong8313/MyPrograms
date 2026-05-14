using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.DAL.Generated.Entity.Table
{
    public partial class BvPersonGroupEntity
    {
        public InboundGroupBehavior InboundBehavior
        {
            get { return (InboundGroupBehavior) InboundCallBehavior; }
            set { InboundCallBehavior = (byte) value; }
        }

        public TransferGroupBehavior TransferBehavior
        {
            get { return (TransferGroupBehavior)CallTransferBehavior; }
            set { CallTransferBehavior = (byte)value; }
        }
    }
}
