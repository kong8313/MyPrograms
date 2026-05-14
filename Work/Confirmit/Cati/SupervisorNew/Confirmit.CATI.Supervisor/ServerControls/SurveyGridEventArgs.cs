using System;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    //---------------------------------------------------------------------------
    public delegate void SurveyGridEventHandler(
        object sender,
        SurveyGridEventArgs e);

    //---------------------------------------------------------------------------
    public class SurveyGridEventArgs: EventArgs
    {
        private int m_nSurveyId;

        //---------------------------------------------------------------------------
        public SurveyGridEventArgs()
        {
        }

        //---------------------------------------------------------------------------
        public SurveyGridEventArgs(int nSurveyId)
        {
            m_nSurveyId = nSurveyId;
        }

        //---------------------------------------------------------------------------
        public int SurveyId
        {
            get { return (m_nSurveyId); }
        }
    }
}
