using System;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling.Fakes
{
    public class StubIExternalVerifiable : IExternalVerifiable 
    {
        private IExternalVerifiable _inner;

        public StubIExternalVerifiable()
        {
            _inner = null;
        }

        public IExternalVerifiable Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public event ValidationEventHandler Validating;
        public void OnValidating(ValidationEventArgs e)
        {
            if (Validating != null)
            {
                Validating(this, e);
            }
        }

    }
}