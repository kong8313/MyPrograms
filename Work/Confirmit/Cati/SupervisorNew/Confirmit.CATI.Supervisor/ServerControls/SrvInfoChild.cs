using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    public class SrvInfoChild : BaseWUC, ISrvInfo
    {
        private BvSurveyEntity _survey;
        public BvSurveyEntity Survey
        {
            get
            {
                if (_survey == null)
                {
                    _survey = SurveyRepository.GetById(Int32.Parse(Request["ID"]));
                }

                return _survey;
            }
        }

        public virtual void Save()
        {
        }
    }
}