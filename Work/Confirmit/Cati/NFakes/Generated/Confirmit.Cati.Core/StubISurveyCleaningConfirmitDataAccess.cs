using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.Services.CleaningService;

namespace Confirmit.CATI.Core.Services.CleaningService.Fakes
{
    public class StubISurveyCleaningConfirmitDataAccess : ISurveyCleaningConfirmitDataAccess 
    {
        private ISurveyCleaningConfirmitDataAccess _inner;

        public StubISurveyCleaningConfirmitDataAccess()
        {
            _inner = null;
        }

        public ISurveyCleaningConfirmitDataAccess Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SetCreatorsListOfCleaningServiceEmailInfoDelegate(List<CleaningServiceEmailInfo> emailInfo);
        public SetCreatorsListOfCleaningServiceEmailInfoDelegate SetCreatorsListOfCleaningServiceEmailInfo;

        void ISurveyCleaningConfirmitDataAccess.SetCreators(List<CleaningServiceEmailInfo> emailInfo)
        {

            if (SetCreatorsListOfCleaningServiceEmailInfo != null)
            {
                SetCreatorsListOfCleaningServiceEmailInfo(emailInfo);
            } else if (_inner != null)
            {
                ((ISurveyCleaningConfirmitDataAccess)_inner).SetCreators(emailInfo);
            }
        }

    }
}