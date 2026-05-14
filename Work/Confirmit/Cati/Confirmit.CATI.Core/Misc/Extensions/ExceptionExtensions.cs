using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Misc.Extensions
{
    public static class ExceptionExtensions
    {
        public static List<Exception> GetAllInnerExceptions(this Exception ex)
        {
            var exceptions = new List<Exception>();
            while (ex != null)
            {
                exceptions.Add(ex);
                ex = ex.InnerException;
            }

            return exceptions;
        }
    }
}