using System;
using Confirmit.CATI.Common.Validators;

namespace Confirmit.CATI.Common.Validators.Fakes
{
    public class StubIInputParameterValidator : IInputParameterValidator 
    {
        private IInputParameterValidator _inner;

        public StubIInputParameterValidator()
        {
            _inner = null;
        }

        public IInputParameterValidator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool IsValidStringDelegate(string value);
        public IsValidStringDelegate IsValidString;

        bool IInputParameterValidator.IsValid(string value)
        {


            if (IsValidString != null)
            {
                return IsValidString(value);
            } else if (_inner != null)
            {
                return ((IInputParameterValidator)_inner).IsValid(value);
            }

            return default(bool);
        }

        public delegate bool IsValidEmailStringDelegate(string emailString);
        public IsValidEmailStringDelegate IsValidEmailString;

        bool IInputParameterValidator.IsValidEmail(string emailString)
        {


            if (IsValidEmailString != null)
            {
                return IsValidEmailString(emailString);
            } else if (_inner != null)
            {
                return ((IInputParameterValidator)_inner).IsValidEmail(emailString);
            }

            return default(bool);
        }

        public delegate bool IsValidQuestionIdStringDelegate(string questionId);
        public IsValidQuestionIdStringDelegate IsValidQuestionIdString;

        bool IInputParameterValidator.IsValidQuestionId(string questionId)
        {


            if (IsValidQuestionIdString != null)
            {
                return IsValidQuestionIdString(questionId);
            } else if (_inner != null)
            {
                return ((IInputParameterValidator)_inner).IsValidQuestionId(questionId);
            }

            return default(bool);
        }

        private string _InvalidSymbols;
        public Func<string> InvalidSymbolsGet;
        public Action<string> InvalidSymbolsSetString;

        string IInputParameterValidator.InvalidSymbols
        {
            get
            {
                if (InvalidSymbolsGet != null)
                {
                    return InvalidSymbolsGet();
                } else if (_inner != null)
                {
                    return ((IInputParameterValidator)_inner).InvalidSymbols;
                }

                if (InvalidSymbolsSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InvalidSymbols;
                }

                return default(string);
            }

        }

        private string _ValidStringMask;
        public Func<string> ValidStringMaskGet;
        public Action<string> ValidStringMaskSetString;

        string IInputParameterValidator.ValidStringMask
        {
            get
            {
                if (ValidStringMaskGet != null)
                {
                    return ValidStringMaskGet();
                } else if (_inner != null)
                {
                    return ((IInputParameterValidator)_inner).ValidStringMask;
                }

                if (ValidStringMaskSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ValidStringMask;
                }

                return default(string);
            }

        }

    }
}