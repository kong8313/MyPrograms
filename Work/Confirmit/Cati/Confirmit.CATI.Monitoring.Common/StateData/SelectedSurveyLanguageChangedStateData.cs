using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    [Serializable]
    public class SelectedSurveyLanguageChangedStateData  : BaseStateData
    {
		#region Properties

        public string PreviousLanguageName
        {
            get;
            set;
        }

        public string NewLanguageName
        {
            get;
            set;
        }

        #endregion
    }
}