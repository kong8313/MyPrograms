using System;
using Confirmit.CATI.Supervisor.ServerControls;

namespace Confirmit.CATI.Supervisor.ServerControls.Fakes
{
    public class StubIMinWidth : IMinWidth 
    {
        private IMinWidth _inner;

        public StubIMinWidth()
        {
            _inner = null;
        }

        public IMinWidth Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int _MinWidth;
        public Func<int> MinWidthGet;
        public Action<int> MinWidthSetInt32;

        int IMinWidth.MinWidth
        {
            get
            {
                if (MinWidthGet != null)
                {
                    return MinWidthGet();
                } else if (_inner != null)
                {
                    return ((IMinWidth)_inner).MinWidth;
                }

                if (MinWidthSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MinWidth;
                }

                return default(int);
            }

            set
            {
                if (MinWidthSetInt32 != null)
                {
                    MinWidthSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMinWidth)_inner).MinWidth = value;
                    return;
                }

                if (MinWidthGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _MinWidth = value;
                }

            }
        }

    }
}