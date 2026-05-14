using System;
using System.Web.UI;
using Confirmit.CATI.Supervisor.Controls.Grid;

namespace Confirmit.CATI.Supervisor.Controls.Grid.Fakes
{
    public class StubIRequiresPreInitialization : IRequiresPreInitialization 
    {
        private IRequiresPreInitialization _inner;

        public StubIRequiresPreInitialization()
        {
            _inner = null;
        }

        public IRequiresPreInitialization Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void PreInitializeControlDelegate(Control owner);
        public PreInitializeControlDelegate PreInitializeControl;

        void IRequiresPreInitialization.PreInitialize(Control owner)
        {

            if (PreInitializeControl != null)
            {
                PreInitializeControl(owner);
            } else if (_inner != null)
            {
                ((IRequiresPreInitialization)_inner).PreInitialize(owner);
            }
        }

    }
}