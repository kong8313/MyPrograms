using System;
using Confirmit.CATI.Common;
using ConfirmitDialerInterface;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIContextInfoService : IContextInfoService 
    {
        private IContextInfoService _inner;

        public StubIContextInfoService()
        {
            _inner = null;
        }

        public IContextInfoService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void WriteContextInfoInt32OperationTypeInt32Int32DialingModeDelegate(int operationId, OperationType operationType, int callcenterId, int its, DialingMode dialMode);
        public WriteContextInfoInt32OperationTypeInt32Int32DialingModeDelegate WriteContextInfoInt32OperationTypeInt32Int32DialingMode;

        void IContextInfoService.WriteContextInfo(int operationId, OperationType operationType, int callcenterId, int its, DialingMode dialMode)
        {

            if (WriteContextInfoInt32OperationTypeInt32Int32DialingMode != null)
            {
                WriteContextInfoInt32OperationTypeInt32Int32DialingMode(operationId, operationType, callcenterId, its, dialMode);
            } else if (_inner != null)
            {
                ((IContextInfoService)_inner).WriteContextInfo(operationId, operationType, callcenterId, its, dialMode);
            }
        }

        public delegate void ResetContextInfoDelegate();
        public ResetContextInfoDelegate ResetContextInfo;

        void IContextInfoService.ResetContextInfo()
        {

            if (ResetContextInfo != null)
            {
                ResetContextInfo();
            } else if (_inner != null)
            {
                ((IContextInfoService)_inner).ResetContextInfo();
            }
        }

    }
}