using System;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.SchedulingScriptNotificationServiceImplementation;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubISchedulingScriptNotificatorCreator : ISchedulingScriptNotificatorCreator 
    {
        private ISchedulingScriptNotificatorCreator _inner;

        public StubISchedulingScriptNotificatorCreator()
        {
            _inner = null;
        }

        public ISchedulingScriptNotificatorCreator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate ISchedulingScriptNotificator CreateInt32Int32Int32Delegate(int batchId, int surveyId, int scheduleId);
        public CreateInt32Int32Int32Delegate CreateInt32Int32Int32;

        ISchedulingScriptNotificator ISchedulingScriptNotificatorCreator.Create(int batchId, int surveyId, int scheduleId)
        {


            if (CreateInt32Int32Int32 != null)
            {
                return CreateInt32Int32Int32(batchId, surveyId, scheduleId);
            } else if (_inner != null)
            {
                return ((ISchedulingScriptNotificatorCreator)_inner).Create(batchId, surveyId, scheduleId);
            }

            return default(ISchedulingScriptNotificator);
        }

    }
}