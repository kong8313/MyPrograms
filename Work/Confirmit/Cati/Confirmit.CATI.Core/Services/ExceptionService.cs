using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.Core.Services
{
    public class ExceptionService
    {
        public static string GetClientMessageText( Exception exception )
        {
            return exception is UserMessageException ? exception.Message  : "Internal server error";
        }

        public static UserMessageException CreateUserMessageException(string title, IEnumerable<string> messages)
        {
            var sb = new StringBuilder(title);
            foreach (var message in messages)
            {
                sb.AppendLine();
                sb.AppendFormat(" - {0}", message);
            }

            return new UserMessageException(sb.ToString());
        }

        public static UserMessageException CreateUserMessageExceptionFromOperationResult(string operationName, IDictionary<int,Exception> batchExceptions )
        {
            var title = String.Format("Operation '{0}' was partially successfull.", operationName);
            var messages = batchExceptions.Select(x => String.Format("Batch {0} was failed: {1}", x.Key, GetClientMessageText(x.Value)));
            
            return CreateUserMessageException(title, messages);
        }
    }
}
