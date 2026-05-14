using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Common;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface IContextInfoService
    {
        void WriteContextInfo(int operationId, OperationType operationType, int callcenterId, int its = 0, DialingMode dialMode = 0);
        void ResetContextInfo();
    }
}
