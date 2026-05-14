using System;
using ConfirmitDialerInterface;
using System.Collections.Generic;
using Confirmit.CATI.Core.BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.Services.CallDelivery.Interfaces;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubICallDeliveryService : ICallDeliveryService 
    {
        private ICallDeliveryService _inner;

        public StubICallDeliveryService()
        {
            _inner = null;
        }

        public ICallDeliveryService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<CallInfo> LookupCallsInt32Int32NullableOfInt32CallsSelectionAlgorithmInt32BooleanListOfGroupInfoOutDelegate(int surveySid, int dialerId, int? groupId, CallsSelectionAlgorithm callsSelectionAlgorithm, int count, bool isRecording, out List<GroupInfo> aggregatedGroupsInfo);
        public LookupCallsInt32Int32NullableOfInt32CallsSelectionAlgorithmInt32BooleanListOfGroupInfoOutDelegate LookupCallsInt32Int32NullableOfInt32CallsSelectionAlgorithmInt32BooleanListOfGroupInfoOut;

        List<CallInfo> ICallDeliveryService.LookupCalls(int surveySid, int dialerId, int? groupId, CallsSelectionAlgorithm callsSelectionAlgorithm, int count, bool isRecording, out List<GroupInfo> aggregatedGroupsInfo)
        {
            aggregatedGroupsInfo = default(List<GroupInfo>);


            if (LookupCallsInt32Int32NullableOfInt32CallsSelectionAlgorithmInt32BooleanListOfGroupInfoOut != null)
            {
                return LookupCallsInt32Int32NullableOfInt32CallsSelectionAlgorithmInt32BooleanListOfGroupInfoOut(surveySid, dialerId, groupId, callsSelectionAlgorithm, count, isRecording, out aggregatedGroupsInfo);
            } else if (_inner != null)
            {
                return ((ICallDeliveryService)_inner).LookupCalls(surveySid, dialerId, groupId, callsSelectionAlgorithm, count, isRecording, out aggregatedGroupsInfo);
            }

            return default(List<CallInfo>);
        }

        public delegate CallRequestResult LookupCallInt32Int32Int32GetCallEventDelegate(int personId, int surveyId, int interviewId, GetCallEvent activityEvent);
        public LookupCallInt32Int32Int32GetCallEventDelegate LookupCallInt32Int32Int32GetCallEvent;

        CallRequestResult ICallDeliveryService.LookupCall(int personId, int surveyId, int interviewId, GetCallEvent activityEvent)
        {


            if (LookupCallInt32Int32Int32GetCallEvent != null)
            {
                return LookupCallInt32Int32Int32GetCallEvent(personId, surveyId, interviewId, activityEvent);
            } else if (_inner != null)
            {
                return ((ICallDeliveryService)_inner).LookupCall(personId, surveyId, interviewId, activityEvent);
            }

            return default(CallRequestResult);
        }

        public delegate void WrapupCallBvCallEntityDelegate(BvCallEntity call);
        public WrapupCallBvCallEntityDelegate WrapupCallBvCallEntity;

        void ICallDeliveryService.WrapupCall(BvCallEntity call)
        {

            if (WrapupCallBvCallEntity != null)
            {
                WrapupCallBvCallEntity(call);
            } else if (_inner != null)
            {
                ((ICallDeliveryService)_inner).WrapupCall(call);
            }
        }

    }
}