using System;
using Confirmit.CATI.Core.Telephony;

namespace Confirmit.CATI.Core.Telephony.Fakes
{
    public class StubIDialerOverridenFeature : IDialerOverridenFeature 
    {
        private IDialerOverridenFeature _inner;

        public StubIDialerOverridenFeature()
        {
            _inner = null;
        }

        public IDialerOverridenFeature Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private string _Name;
        public Func<string> NameGet;
        public Action<string> NameSetString;

        string IDialerOverridenFeature.Name
        {
            get
            {
                if (NameGet != null)
                {
                    return NameGet();
                } else if (_inner != null)
                {
                    return ((IDialerOverridenFeature)_inner).Name;
                }

                if (NameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Name;
                }

                return default(string);
            }

        }

        private bool _DefaultValue;
        public Func<bool> DefaultValueGet;
        public Action<bool> DefaultValueSetBoolean;

        bool IDialerOverridenFeature.DefaultValue
        {
            get
            {
                if (DefaultValueGet != null)
                {
                    return DefaultValueGet();
                } else if (_inner != null)
                {
                    return ((IDialerOverridenFeature)_inner).DefaultValue;
                }

                if (DefaultValueSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DefaultValue;
                }

                return default(bool);
            }

        }

        private bool? _OverridenValue;
        public Func<bool?> OverridenValueGet;
        public Action<bool?> OverridenValueSetNullableOfBoolean;

        bool? IDialerOverridenFeature.OverridenValue
        {
            get
            {
                if (OverridenValueGet != null)
                {
                    return OverridenValueGet();
                } else if (_inner != null)
                {
                    return ((IDialerOverridenFeature)_inner).OverridenValue;
                }

                if (OverridenValueSetNullableOfBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _OverridenValue;
                }

                return default(bool?);
            }

        }

    }
}