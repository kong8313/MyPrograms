using System;

namespace Confirmit.CATI.Common.ConsoleService.Abstract
{
    public class TransferOptions
    {
        public string Resource;

        public ConsoleTransferType Type;

        public bool AllowInterviewing;
    }

    [Serializable]
    public enum ConsoleTransferType
    {
        InternalCold = 1,
        ExternalCold = 2,
        InternalWarm = 3,
        ExternalWarm = 4,
    }
}
