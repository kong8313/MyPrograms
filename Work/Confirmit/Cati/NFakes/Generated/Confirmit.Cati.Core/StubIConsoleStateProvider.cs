using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.Telephony.Console;
using Confirmit.CATI.Common.ConsoleService.Abstract;

namespace Confirmit.CATI.Core.Telephony.Console.Fakes
{
    public class StubIConsoleStateProvider : IConsoleStateProvider 
    {
        private IConsoleStateProvider _inner;

        public StubIConsoleStateProvider()
        {
            _inner = null;
        }

        public IConsoleStateProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate State GetStateBvTasksEntityBvPersonEntityGetStateEventUrlGeneratedInGetStateEventDelegate(BvTasksEntity task, BvPersonEntity person, GetStateEvent evt, UrlGeneratedInGetStateEvent activityEvent);
        public GetStateBvTasksEntityBvPersonEntityGetStateEventUrlGeneratedInGetStateEventDelegate GetStateBvTasksEntityBvPersonEntityGetStateEventUrlGeneratedInGetStateEvent;

        State IConsoleStateProvider.GetState(BvTasksEntity task, BvPersonEntity person, GetStateEvent evt, UrlGeneratedInGetStateEvent activityEvent)
        {


            if (GetStateBvTasksEntityBvPersonEntityGetStateEventUrlGeneratedInGetStateEvent != null)
            {
                return GetStateBvTasksEntityBvPersonEntityGetStateEventUrlGeneratedInGetStateEvent(task, person, evt, activityEvent);
            } else if (_inner != null)
            {
                return ((IConsoleStateProvider)_inner).GetState(task, person, evt, activityEvent);
            }

            return default(State);
        }

    }
}