using System;
using System.Collections.Generic;
using System.Linq;

using ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    public enum CallOutcomeGenerationMethod
    {
        Sequence,
        Random
    };
    
    public class CallOutcomeSequence
    {
        private readonly List<CallOutcome> _outcomeList;
        private CallOutcomeGenerationMethod GenerationMethod { get; set; }
        private readonly Random _random = new Random();
        private int _nextOutcomeIndex;

        public int Count { get { return _outcomeList.Count; } }

        public CallOutcomeSequence(List<CallOutcome> outcomeList, CallOutcomeGenerationMethod outcomeGenerationMethod)
            : this(outcomeList, outcomeGenerationMethod, false)
        {
        }

        public CallOutcomeSequence(List<CallOutcome> outcomeList, CallOutcomeGenerationMethod outcomeGenerationMethod, bool skipConnected)
        {
            _outcomeList = skipConnected ?
                outcomeList.Where(element => (element != CallOutcome.Connected)).ToList() : outcomeList;

            GenerationMethod = outcomeGenerationMethod;
            _nextOutcomeIndex = 0;
        }

        public CallOutcome GetOutcome()
        {
            if (GenerationMethod == CallOutcomeGenerationMethod.Sequence)
            {
                return _outcomeList[_nextOutcomeIndex++ % _outcomeList.Count];
            }

            lock (_random) // Random class implementation is not thread safe
            {
                return _outcomeList[_random.Next(_outcomeList.Count)];
            }
        }
    }
}