using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ConfirmitDialerInterface;
using SimulatorDialerDriver.Models;
using SimulatorDialerDriver.Services;

namespace SimulatorDialerDriver
{
    public class CallManager : ICallManager
    {
        public enum CallType
        {
            Outbound = 0,
            Inbound = 1,
            Transfer = 2
        }

        public class CallInfoEx
        {
            public CallInfoEx(CallInfo info, long campaignId, CallType type)
                :this(info, campaignId, new[] { campaignId }, type)
            {
            }

            public CallInfoEx(CallInfo info, long campaignId, long[] borrowAgentsFrom, CallType type)
            {
                Info = info;
                Type = type;
                CampaignId = campaignId;
                BorrowAgentsFrom = borrowAgentsFrom;

                if (info.agingTimeout > 0)
                {
                    TimeToExpire = DateTime.UtcNow + TimeSpan.FromMinutes(info.agingTimeout);
                }
                else
                {
                    TimeToExpire = DateTime.MaxValue;
                }
            }
            public CallInfo Info { get; }
            public CallType Type { get; }
            public DateTime TimeToExpire { get; }
            public long CampaignId { get; set; }
            public long[] BorrowAgentsFrom { get; set; }
        }

        private LinkedList<CallInfoEx> Calls { get; set; }
        private readonly ManualResetEvent _addCallsEvent = new ManualResetEvent(false);

        private int _isCallDemanded;

        public CallManager()
        {
            Calls = new LinkedList<CallInfoEx>();
        }

        public int CallsCount
        {
            get { return Calls.Count; }
        }

        public bool MoveCallTo(int callId, CallManager to)
        {
            var call = RemoveCallByIdIfExists(callId);
            if (call == null)
                return false;

             to.AddFirstCall(call);
             return true;
        }

        public CallInfoEx RemoveCallByIdIfExists(long callId)
        {
            lock (Calls)
            {
                var call = Calls.SingleOrDefault(x => x.Info.callId == callId);
                if (call == null)
                    return null;

                Calls.Remove(call);
                return call;
            }
        }

        public CallInfoEx GetCallWithRemove(Interviewer interviewer)
        {
            lock (Calls)
            {
                var result = Calls.Find(x =>
                    (
                        x.BorrowAgentsFrom == null || 
                        x.BorrowAgentsFrom.Contains(interviewer.CampaignId)
                    ) && ( 
                        x.Info.agentId == 0 && x.Info.agentGroupId == 0 ||
                        x.Info.agentId == interviewer.AgentId || 
                        interviewer.Groups.Contains(x.Info.agentGroupId) 
                    ));

                if (result == null)
                {
                    _addCallsEvent.Reset();
                    return null;
                }

                Calls.Remove(result);
                SimulatorDialerDriverPerformanceCounters.NumberOfCachedPredictiveCalls.Decrement();

                return result.Value;
            }
        }

        public void AddCalls(long campaignId, List<CallInfo> calls)
        {
            lock(Calls)
            {
                foreach (var call in calls)
                    Calls.AddLast(new CallInfoEx(call, campaignId, CallType.Outbound));

                _addCallsEvent.Set();
            }


            SimulatorDialerDriverPerformanceCounters.NumberOfCachedPredictiveCalls.IncrementBy(calls.Count);
            SimulatorDialerDriverPerformanceCounters.NumberOfReceivedPredictiveCalls.IncrementBy(calls.Count);
            
            Interlocked.CompareExchange(ref _isCallDemanded, 0, 1);
        }

        public CallInfo[] GetExpiredCallsAndRemove()
        {
            var result = new List<CallInfo>();
            lock (Calls)
            {
                var node = Calls.First;

                while (node != null)
                {
                    var next = node.Next;
                    if (node.Value.TimeToExpire < DateTime.UtcNow) 
                    {
                        result.Add(node.Value.Info);
                        Calls.Remove(node);
                    }
                    node = next;
                }

                if (Calls.First == null)
                {
                    _addCallsEvent.Reset();
                }
            }

            return result.ToArray();
        }

        public void DemandCall()
        {
            Interlocked.CompareExchange(ref _isCallDemanded, 1, 0);
        }

        public bool WasCallDeliveredSinceLastDemand()
        {
            return _isCallDemanded == 0;
        }

        public int RemoveCalls(List<CallInfo> callList)
        {
            int count = 0;

            lock (Calls)
            {
                var node = Calls.First;

                while (node != null)
                {
                    var next = node.Next;

                    // TODO: not sure which one variant is faster
                    //if (callList.Contains(node.Value.Info))
                    //if (callList.IndexOf(node.Value.Info) != 0)
                    if (callList.Find(x => x.callId == node.Value.Info.callId) != null)
                    {
                        Calls.Remove(node);
                        count++;
                    }

                    node = next;
                }
            }

            return count;
        }

        public List<CallInfo> RemoveAll()
        {
            List<CallInfo> removedCalls;

            lock (Calls)
            {
                removedCalls = Calls.Select(x => x.Info).ToList();

                Calls.Clear();
            }

            return removedCalls;
        }

        public void AddInboundCall(long campaignId, long[] borrowAgentsFrom, CallInfo callInfo)
        {
            AddFirstCall(new CallInfoEx(callInfo, campaignId, borrowAgentsFrom, CallType.Inbound));
        }

        public void AddTranferCall(long campaignId, long[] borrowAgentsFrom, CallInfo callInfo)
        {
            AddFirstCall(new CallInfoEx(callInfo, campaignId, borrowAgentsFrom, CallType.Transfer));
        }

        private void AddFirstCall(CallInfoEx call)
        {
            lock (Calls)
            {
                Calls.AddFirst(call);

                _addCallsEvent.Set();
            }

            Interlocked.CompareExchange(ref _isCallDemanded, 0, 1);
        }

        internal CallInfoEx[] GetCalls()
        {
            lock (Calls)
            {
                return Calls.ToArray();
            }
        }

        internal CallInfoEx TryGetCallWithRemove(long campaignId, int interviewId)
        {
            lock (Calls)
            {
                var result = Calls.Find(x => x.CampaignId == campaignId && x.Info.interviewId == interviewId);

                if (result == null)
                {
                    return null;
                }

                Calls.Remove(result);
                
                return result.Value;
            }
        }
    }
}
