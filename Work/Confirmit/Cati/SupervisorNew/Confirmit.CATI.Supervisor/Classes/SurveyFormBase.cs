


using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Supervisor.Classes
{
    public class SurveyFormBase : BaseForm 
    {
        private readonly IUserSurveyListRepository _userSurveyListRepository;

        public SurveyFormBase()
        {
            _userSurveyListRepository = ServiceLocator.Resolve<IUserSurveyListRepository>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                int surveyId;
                
                if (Int32.TryParse(Request["ID"], out surveyId))
                {
                    _userSurveyListRepository.Insert(UserSurveyListType.Recent, surveyId);
                }
            }
        }
    }
}