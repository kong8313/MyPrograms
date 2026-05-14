using System;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    //---------------------------------------------------------------------------
    public delegate void SrvSampleAddEventHandler(
        object sender,
        SrvSampleAddEventArgs e);

    //---------------------------------------------------------------------------
    public class SrvSampleAddEventArgs: EventArgs
    {
        private bool m_bAdd;

        //---------------------------------------------------------------------------
        public SrvSampleAddEventArgs(bool add)
        {
            m_bAdd = add;
        }

        //---------------------------------------------------------------------------
        public bool Add
        {
            get { return (m_bAdd); }
        }
    }
}