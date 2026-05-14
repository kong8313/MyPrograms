using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations
{
    public interface IDialerOperation
    {
        void FlushCallsIfNeeded(BvSurveyEntity surveyEntity, List<CallInfo> callsToFlush);
    }
}