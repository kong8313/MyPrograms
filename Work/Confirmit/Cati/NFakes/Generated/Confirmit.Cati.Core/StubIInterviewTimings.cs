using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.Services.Fakes
{
    public class StubIInterviewTimings : IInterviewTimings 
    {
        private IInterviewTimings _inner;

        public StubIInterviewTimings()
        {
            _inner = null;
        }

        public IInterviewTimings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvInterviewTimings GetInterviewTimingsBvTasksEntityBvSurveyEntityDelegate(BvTasksEntity task, BvSurveyEntity survey);
        public GetInterviewTimingsBvTasksEntityBvSurveyEntityDelegate GetInterviewTimingsBvTasksEntityBvSurveyEntity;

        BvInterviewTimings IInterviewTimings.GetInterviewTimings(BvTasksEntity task, BvSurveyEntity survey)
        {


            if (GetInterviewTimingsBvTasksEntityBvSurveyEntity != null)
            {
                return GetInterviewTimingsBvTasksEntityBvSurveyEntity(task, survey);
            } else if (_inner != null)
            {
                return ((IInterviewTimings)_inner).GetInterviewTimings(task, survey);
            }

            return default(BvInterviewTimings);
        }

    }
}