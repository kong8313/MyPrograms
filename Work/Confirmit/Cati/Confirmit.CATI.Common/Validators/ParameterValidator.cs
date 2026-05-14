using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.Common.Validators
{
    public static class ParameterValidator
    {
        public static void ValidateNotNull<T>(T parameter, string parameterName)
        {
            if (parameter == null) throw ExceptionManager.NewArgumentNullException(parameterName);         
        }

        public static void ValidateNotEqual<T>(T parameter, T value, string parameterName) where T: IEquatable<T>
        {
            if ((parameter == null) && (value == null)) throw ExceptionManager.NewArgumentException(parameterName);
            if ((parameter != null) && (parameter.Equals(value))) throw ExceptionManager.NewArgumentException(parameterName);
        }

        public static void ValidateNotNullOrEmpty(string parameter, string parameterName)
        {
            if (String.IsNullOrEmpty(parameter)) throw ExceptionManager.NewArgumentException(parameterName);
        }

        public static void ValidateCondition(bool expected, bool condition, string exceptionMessage)
        {
            if (expected != condition) throw ExceptionManager.NewUserMessageException(exceptionMessage);
        }


        public static void ValidateIsOfType<T>(object parameter, string parameterName)
        {
            if (!(parameter is T))
                throw new InvalidCastException(String.Format("The '{0}' argument is not a {1} object.", parameterName, typeof(T).Name));
        }

        // a>b
        public static void GreaterThan<T>(T a, T b, string parameterName) where T: IComparable<T>
        {
            if (a.CompareTo(b) <= 0) throw ExceptionManager.NewArgumentException(parameterName);         
        }

        // a>=b
        public static void GreaterOrEqualThan<T>(T a, T b, string parameterName) where T : IComparable<T>
        {
            if (a.CompareTo(b) < 0) throw ExceptionManager.NewArgumentException(parameterName);
        }

        // a<b
        public static void LessThan<T>(T a, T b, string parameterName) where T : IComparable<T>
        {
            if (a.CompareTo(b) >= 0) throw ExceptionManager.NewArgumentException(parameterName);
        }

        // a<=b
        public static void LessOrEqualThan<T>(T a, T b, string parameterName) where T : IComparable<T>
        {
            if (a.CompareTo(b) > 0) throw ExceptionManager.NewArgumentException(parameterName);
        }

    }
}
