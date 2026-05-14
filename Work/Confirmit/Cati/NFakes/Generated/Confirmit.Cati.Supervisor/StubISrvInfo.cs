using System;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Supervisor.ServerControls.Fakes
{
    public class StubISrvInfo : ISrvInfo 
    {
        private ISrvInfo _inner;

        public StubISrvInfo()
        {
            _inner = null;
        }

        public ISrvInfo Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private BvSurveyEntity _Survey;
        public Func<BvSurveyEntity> SurveyGet;
        public Action<BvSurveyEntity> SurveySetBvSurveyEntity;

        BvSurveyEntity ISrvInfo.Survey
        {
            get
            {
                if (SurveyGet != null)
                {
                    return SurveyGet();
                } else if (_inner != null)
                {
                    return ((ISrvInfo)_inner).Survey;
                }

                if (SurveySetBvSurveyEntity == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Survey;
                }

                return default(BvSurveyEntity);
            }

        }

    }
}