using System;

namespace Confirmit.TelephonyProblemStates.ProblemState
{
    [Serializable]
    public abstract class CatiProblemState
    {
        protected CatiProblemState(int errorCode)
        {
            State = errorCode;
        }

        public int State { get; protected set; }
        
        public abstract string Message { get; }

        public virtual bool IsProblem
        {
            get { return true; }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Message, State);
        }
    }
}
