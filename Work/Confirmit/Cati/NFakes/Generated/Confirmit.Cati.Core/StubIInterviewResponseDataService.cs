using System;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;

namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data.Fakes
{
    public class StubIInterviewResponseDataService : IInterviewResponseDataService 
    {
        private IInterviewResponseDataService _inner;

        public StubIInterviewResponseDataService()
        {
            _inner = null;
        }

        public IInterviewResponseDataService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetInterviewVariableValueStringInt32StringDelegate(string projectId, int interviewId, string variableName);
        public GetInterviewVariableValueStringInt32StringDelegate GetInterviewVariableValueStringInt32String;

        string IInterviewResponseDataService.GetInterviewVariableValue(string projectId, int interviewId, string variableName)
        {


            if (GetInterviewVariableValueStringInt32String != null)
            {
                return GetInterviewVariableValueStringInt32String(projectId, interviewId, variableName);
            } else if (_inner != null)
            {
                return ((IInterviewResponseDataService)_inner).GetInterviewVariableValue(projectId, interviewId, variableName);
            }

            return default(string);
        }

    }
}