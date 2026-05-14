using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;
using ConfirmitDialerInterface;
using Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.Fakes
{
    public class StubIDialerOperation : IDialerOperation 
    {
        private IDialerOperation _inner;

        public StubIDialerOperation()
        {
            _inner = null;
        }

        public IDialerOperation Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void FlushCallsIfNeededBvSurveyEntityListOfCallInfoDelegate(BvSurveyEntity surveyEntity, List<CallInfo> callsToFlush);
        public FlushCallsIfNeededBvSurveyEntityListOfCallInfoDelegate FlushCallsIfNeededBvSurveyEntityListOfCallInfo;

        void IDialerOperation.FlushCallsIfNeeded(BvSurveyEntity surveyEntity, List<CallInfo> callsToFlush)
        {

            if (FlushCallsIfNeededBvSurveyEntityListOfCallInfo != null)
            {
                FlushCallsIfNeededBvSurveyEntityListOfCallInfo(surveyEntity, callsToFlush);
            } else if (_inner != null)
            {
                ((IDialerOperation)_inner).FlushCallsIfNeeded(surveyEntity, callsToFlush);
            }
        }

    }
}