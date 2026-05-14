using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.Services.CallDelivery.Interfaces
{
    public interface IQuotaClusterService
    {
        bool TryIncrenent(int surveyId, int callId);
        bool Increnent(int surveyId, int callId);
        void Decrement(int surveyId, int cellId);
    }
}
